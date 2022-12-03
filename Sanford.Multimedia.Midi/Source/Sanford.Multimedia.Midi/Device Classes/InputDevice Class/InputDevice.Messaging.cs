#region

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

#endregion

namespace Sanford.Multimedia.Midi
{
    internal struct MidiInParams
    {
        public readonly IntPtr Param1;
        public readonly IntPtr Param2;

        public MidiInParams(IntPtr param1, IntPtr param2)
        {
            Param1 = param1;
            Param2 = param2;
        }
    }

    public sealed partial class InputDevice : MidiDevice
    {
        /// <summary>
        ///     Gets or sets a value indicating whether the midi input driver callback should be posted on a delegate queue with
        ///     its own thread.
        ///     Default is <c>true</c>. If set to <c>false</c> the driver callback directly calls the events for lowest possible
        ///     latency.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the midi input driver callback should be posted on a delegate queue with its own thread; otherwise,
        ///     <c>false</c>.
        /// </value>
        public bool PostDriverCallbackToDelegateQueue { get; }

        private void HandleMessage(IntPtr hnd, int msg, IntPtr instance, IntPtr param1, IntPtr param2)
        {
            var param = new MidiInParams(param1, param2);

            switch (msg)
            {
                case MIM_OPEN:
                    break;
                case MIM_CLOSE:
                    break;
                case MIM_DATA when PostDriverCallbackToDelegateQueue:
                    delegateQueue.Post(HandleShortMessage, param);
                    break;
                case MIM_DATA:
                    HandleShortMessage(param);
                    break;
                case MIM_MOREDATA when PostDriverCallbackToDelegateQueue:
                    delegateQueue.Post(HandleShortMessage, param);
                    break;
                case MIM_MOREDATA:
                    HandleShortMessage(param);
                    break;
                case MIM_LONGDATA when PostDriverCallbackToDelegateQueue:
                    delegateQueue.Post(HandleSysExMessage, param);
                    break;
                case MIM_LONGDATA:
                    HandleSysExMessage(param);
                    break;
                case MIM_ERROR when PostDriverCallbackToDelegateQueue:
                    delegateQueue.Post(HandleInvalidShortMessage, param);
                    break;
                case MIM_ERROR:
                    HandleInvalidShortMessage(param);
                    break;
                case MIM_LONGERROR when PostDriverCallbackToDelegateQueue:
                    delegateQueue.Post(HandleInvalidSysExMessage, param);
                    break;
                case MIM_LONGERROR:
                    HandleInvalidSysExMessage(param);
                    break;
            }
        }

        private void HandleShortMessage(object state)
        {
            var param = (MidiInParams)state;
            var message = param.Param1.ToInt32();
            var timestamp = param.Param2.ToInt32();

            //first send RawMessage
            OnShortMessage(new ShortMessageEventArgs(message, timestamp));

            var status = ShortMessage.UnpackStatus(message);

            if (status >= (int)ChannelCommand.NoteOff &&
                status <= (int)ChannelCommand.PitchWheel +
                ChannelMessage.MidiChannelMaxValue)
            {
                cmBuilder.Message = message;
                cmBuilder.Build();

                cmBuilder.Result.Timestamp = timestamp;
                OnMessageReceived(cmBuilder.Result);
                OnChannelMessageReceived(new ChannelMessageEventArgs(null, cmBuilder.Result));
            }
            else if (status == (int)SysCommonType.MidiTimeCode ||
                     status == (int)SysCommonType.SongPositionPointer ||
                     status == (int)SysCommonType.SongSelect ||
                     status == (int)SysCommonType.TuneRequest)
            {
                scBuilder.Message = message;
                scBuilder.Build();

                scBuilder.Result.Timestamp = timestamp;
                OnMessageReceived(scBuilder.Result);
                OnSysCommonMessageReceived(new SysCommonMessageEventArgs(scBuilder.Result));
            }
            else
            {
                SysRealtimeMessageEventArgs e = null;

                switch ((SysRealtimeType)status)
                {
                    case SysRealtimeType.ActiveSense:
                        e = SysRealtimeMessageEventArgs.ActiveSense;
                        break;

                    case SysRealtimeType.Clock:
                        e = SysRealtimeMessageEventArgs.Clock;
                        break;

                    case SysRealtimeType.Continue:
                        e = SysRealtimeMessageEventArgs.Continue;
                        break;

                    case SysRealtimeType.Reset:
                        e = SysRealtimeMessageEventArgs.Reset;
                        break;

                    case SysRealtimeType.Start:
                        e = SysRealtimeMessageEventArgs.Start;
                        break;

                    case SysRealtimeType.Stop:
                        e = SysRealtimeMessageEventArgs.Stop;
                        break;

                    case SysRealtimeType.Tick:
                        e = SysRealtimeMessageEventArgs.Tick;
                        break;
                }

                e.Message.Timestamp = timestamp;
                OnMessageReceived(e.Message);
                OnSysRealtimeMessageReceived(e);
            }
        }

        private void HandleSysExMessage(object state)
        {
            lock (lockObject)
            {
                var param = (MidiInParams)state;
                var headerPtr = param.Param1;

                var header = (MidiHeader)Marshal.PtrToStructure(headerPtr, typeof(MidiHeader));

                if (!resetting)
                {
                    for (var i = 0; i < header.bytesRecorded; i++) sysExData.Add(Marshal.ReadByte(header.data, i));

                    if (sysExData.Count > 1 && sysExData[0] == 0xF0 && sysExData[sysExData.Count - 1] == 0xF7)
                    {
                        var message = new SysExMessage(sysExData.ToArray())
                        {
                            Timestamp = param.Param2.ToInt32()
                        };

                        sysExData.Clear();

                        OnMessageReceived(message);
                        OnSysExMessageReceived(new SysExMessageEventArgs(null, message));
                    }

                    var result = AddSysExBuffer();

                    if (result != DeviceException.MMSYSERR_NOERROR)
                    {
                        Exception ex = new InputDeviceException(result);

                        OnError(new ErrorEventArgs(ex));
                    }
                }

                ReleaseBuffer(headerPtr);
            }
        }

        private void HandleInvalidShortMessage(object state)
        {
            var param = (MidiInParams)state;
            OnInvalidShortMessageReceived(new InvalidShortMessageEventArgs(param.Param1.ToInt32()));
        }

        private void HandleInvalidSysExMessage(object state)
        {
            lock (lockObject)
            {
                var param = (MidiInParams)state;
                var headerPtr = param.Param1;

                var header = (MidiHeader)Marshal.PtrToStructure(headerPtr, typeof(MidiHeader));

                if (!resetting)
                {
                    var data = new byte[header.bytesRecorded];

                    Marshal.Copy(header.data, data, 0, data.Length);

                    OnInvalidSysExMessageReceived(new InvalidSysExMessageEventArgs(data));

                    var result = AddSysExBuffer();

                    if (result != DeviceException.MMSYSERR_NOERROR)
                    {
                        Exception ex = new InputDeviceException(result);

                        OnError(new ErrorEventArgs(ex));
                    }
                }

                ReleaseBuffer(headerPtr);
            }
        }

        private void ReleaseBuffer(IntPtr headerPtr)
        {
            var result = midiInUnprepareHeader(Handle, headerPtr, SizeOfMidiHeader);

            if (result != DeviceException.MMSYSERR_NOERROR)
            {
                Exception ex = new InputDeviceException(result);

                OnError(new ErrorEventArgs(ex));
            }

            MidiHeaderBuilder.Destroy(headerPtr);

            bufferCount--;

            Debug.Assert(bufferCount >= 0);

            Monitor.Pulse(lockObject);
        }

        public int AddSysExBuffer()
        {
            // Initialize the MidiHeader builder.
            headerBuilder.BufferLength = sysExBufferSize;
            headerBuilder.Build();

            // Get the pointer to the built MidiHeader.
            var headerPtr = headerBuilder.Result;

            // Prepare the header to be used.
            var result = midiInPrepareHeader(Handle, headerPtr, SizeOfMidiHeader);

            // If the header was perpared successfully.
            if (result == DeviceException.MMSYSERR_NOERROR)
            {
                bufferCount++;

                // Add the buffer to the InputDevice.
                result = midiInAddBuffer(Handle, headerPtr, SizeOfMidiHeader);

                // If the buffer could not be added.
                if (result == DeviceException.MMSYSERR_NOERROR) return result;
                // Unprepare header - there's a chance that this will fail
                // for whatever reason, but there's not a lot that can be
                // done at this point.
                midiInUnprepareHeader(Handle, headerPtr, SizeOfMidiHeader);

                bufferCount--;

                // Destroy header.
                headerBuilder.Destroy();
            }
            // Else the header could not be prepared.
            else
            {
                // Destroy header.
                headerBuilder.Destroy();
            }

            return result;
        }
    }
}
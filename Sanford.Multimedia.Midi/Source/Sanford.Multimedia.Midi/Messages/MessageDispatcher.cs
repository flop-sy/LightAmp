#region

using System;

#endregion

namespace Sanford.Multimedia.Midi
{
    /// <summary>
    ///     Dispatches IMidiMessages to their corresponding sink.
    /// </summary>
    public sealed class MessageDispatcher
    {
        #region MessageDispatcher Members

        #region Events

        public event EventHandler<ChannelMessageEventArgs> ChannelMessageDispatched;

        public event EventHandler<SysExMessageEventArgs> SysExMessageDispatched;

        public event EventHandler<SysCommonMessageEventArgs> SysCommonMessageDispatched;

        public event EventHandler<SysRealtimeMessageEventArgs> SysRealtimeMessageDispatched;

        public event EventHandler<MetaMessageEventArgs> MetaMessageDispatched;

        #endregion

        /// <summary>
        ///     Dispatches IMidiMessages to their corresponding sink.
        /// </summary>
        /// <param name="message">
        ///     The IMidiMessage to dispatch.
        /// </param>
        public void Dispatch(Track track, IMidiMessage message)
        {
            #region Require

            if (message == null) throw new ArgumentNullException(nameof(message));

            #endregion

            switch (message.MessageType)
            {
                case MessageType.Channel:
                    OnChannelMessageDispatched(new ChannelMessageEventArgs(track, (ChannelMessage)message));
                    break;

                case MessageType.SystemExclusive:
                    OnSysExMessageDispatched(new SysExMessageEventArgs(track, (SysExMessage)message));
                    break;

                case MessageType.Meta:
                    OnMetaMessageDispatched(new MetaMessageEventArgs(track, (MetaMessage)message));
                    break;

                case MessageType.SystemCommon:
                    OnSysCommonMessageDispatched(new SysCommonMessageEventArgs((SysCommonMessage)message));
                    break;

                case MessageType.SystemRealtime:
                    switch (((SysRealtimeMessage)message).SysRealtimeType)
                    {
                        case SysRealtimeType.ActiveSense:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.ActiveSense);
                            break;

                        case SysRealtimeType.Clock:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Clock);
                            break;

                        case SysRealtimeType.Continue:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Continue);
                            break;

                        case SysRealtimeType.Reset:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Reset);
                            break;

                        case SysRealtimeType.Start:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Start);
                            break;

                        case SysRealtimeType.Stop:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Stop);
                            break;

                        case SysRealtimeType.Tick:
                            OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs.Tick);
                            break;
                    }

                    break;
            }
        }

        private void OnChannelMessageDispatched(ChannelMessageEventArgs e)
        {
            var handler = ChannelMessageDispatched;

            handler?.Invoke(this, e);
        }

        private void OnSysExMessageDispatched(SysExMessageEventArgs e)
        {
            var handler = SysExMessageDispatched;

            handler?.Invoke(this, e);
        }

        private void OnSysCommonMessageDispatched(SysCommonMessageEventArgs e)
        {
            var handler = SysCommonMessageDispatched;

            handler?.Invoke(this, e);
        }

        private void OnSysRealtimeMessageDispatched(SysRealtimeMessageEventArgs e)
        {
            var handler = SysRealtimeMessageDispatched;

            handler?.Invoke(this, e);
        }

        private void OnMetaMessageDispatched(MetaMessageEventArgs e)
        {
            var handler = MetaMessageDispatched;

            handler?.Invoke(this, e);
        }

        #endregion
    }
}
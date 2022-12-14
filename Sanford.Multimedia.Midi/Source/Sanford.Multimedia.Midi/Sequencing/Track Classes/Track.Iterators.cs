#region

using System;
using System.Collections.Generic;

#endregion

namespace Sanford.Multimedia.Midi;

public sealed partial class Track
{
    #region Iterators

    public IEnumerable<MidiEvent> Iterator()
    {
        var current = head;

        while (current != null)
        {
            yield return current;

            current = current.Next;
        }

        current = endOfTrackMidiEvent;

        yield return current;
    }

    public IEnumerable<int> DispatcherIterator(MessageDispatcher dispatcher)
    {
        var enumerator = Iterator().GetEnumerator();

        while (enumerator.MoveNext())
        {
            yield return enumerator.Current.AbsoluteTicks;

            dispatcher.Dispatch(this, enumerator.Current.MidiMessage);
        }
    }

    public IEnumerable<int> TickIterator(int startPosition,
        ChannelChaser chaser, MessageDispatcher dispatcher)
    {
        #region Require

        if (startPosition < 0)
            throw new ArgumentOutOfRangeException(nameof(startPosition), startPosition,
                "Start position out of range.");

        #endregion

        var enumerator = Iterator().GetEnumerator();

        var notFinished = enumerator.MoveNext();

        while (enumerator.Current != null && notFinished && enumerator.Current.AbsoluteTicks < startPosition)
        {
            var message = enumerator.Current.MidiMessage;

            switch (message.MessageType)
            {
                case MessageType.Channel:
                    chaser.Process((ChannelMessage)message);
                    break;
                case MessageType.Meta:
                {
                    if (Listen) dispatcher.Dispatch(this, message);

                    break;
                }
            }

            notFinished = enumerator.MoveNext();
        }

        chaser.Chase();

        var ticks = startPosition;

        while (notFinished)
        {
            while (enumerator.Current != null && ticks < enumerator.Current.AbsoluteTicks)
            {
                yield return ticks;

                ticks++;
            }

            yield return ticks;

            while (enumerator.Current != null && notFinished && enumerator.Current.AbsoluteTicks == ticks)
            {
                var mb = enumerator.Current.MidiMessage;

                if (Listen) dispatcher.Dispatch(this, enumerator.Current.MidiMessage);

                notFinished = enumerator.MoveNext();
            }

            ticks++;
        }
    }

    #endregion
}
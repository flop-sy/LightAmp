#region

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Maestro.Events;

#endregion

namespace BardMusicPlayer.Maestro
{
    public sealed partial class BmpMaestro
    {
        private ConcurrentQueue<MaestroEvent> _eventQueue;
        private bool _eventQueueOpen;

        private CancellationTokenSource _eventsTokenSource;
        public EventHandler<OctaveShiftChangedEvent> OnOctaveShiftChanged;
        public EventHandler<bool> OnPerformerChanged;
        public EventHandler<PerformerUpdate> OnPerformerUpdate;
        public EventHandler<bool> OnPlaybackStarted;
        public EventHandler<bool> OnPlaybackStopped;
        public EventHandler<CurrentPlayPositionEvent> OnPlaybackTimeChanged;
        public EventHandler<SongLoadedEvent> OnSongLoaded;
        public EventHandler<MaxPlayTimeEvent> OnSongMaxTime;
        public EventHandler<SpeedShiftEvent> OnSpeedChanged;
        public EventHandler<TrackNumberChangedEvent> OnTrackNumberChanged;

        private async Task RunEventsHandler(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                while (_eventQueue.TryDequeue(out var meastroEvent))
                {
                    if (token.IsCancellationRequested)
                        break;

                    try
                    {
                        switch (meastroEvent)
                        {
                            case CurrentPlayPositionEvent currentPlayPosition:
                                OnPlaybackTimeChanged(this, currentPlayPosition);
                                break;
                            case MaxPlayTimeEvent maxPlayTime:
                                OnSongMaxTime(this, maxPlayTime);
                                break;
                            case SongLoadedEvent songloaded:

                                OnSongLoaded?.Invoke(this, songloaded);
                                break;
                            case PlaybackStartedEvent playbackStarted:

                                OnPlaybackStarted?.Invoke(this, playbackStarted.Started);
                                break;
                            case PlaybackStoppedEvent playbackStopped:

                                OnPlaybackStopped?.Invoke(this, playbackStopped.Stopped);
                                break;
                            case PerformersChangedEvent performerChanged:

                                OnPerformerChanged?.Invoke(this, performerChanged.Changed);
                                break;
                            case TrackNumberChangedEvent trackNumberChanged:

                                OnTrackNumberChanged?.Invoke(this, trackNumberChanged);
                                break;
                            case OctaveShiftChangedEvent octaveShiftChanged:

                                OnOctaveShiftChanged?.Invoke(this, octaveShiftChanged);
                                break;
                            case SpeedShiftEvent speedChanged:

                                OnSpeedChanged?.Invoke(this, speedChanged);
                                break;
                            case PerformerUpdate performerUpdate:

                                OnPerformerUpdate?.Invoke(this, performerUpdate);
                                break;
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                await Task.Delay(25, token).ContinueWith(static tsk => { }, token);
            }
        }

        private void StartEventsHandler()
        {
            _eventQueue = new ConcurrentQueue<MaestroEvent>();
            _eventsTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => RunEventsHandler(_eventsTokenSource.Token), TaskCreationOptions.LongRunning);
            _eventQueueOpen = true;
        }

        private void StopEventsHandler()
        {
            _eventQueueOpen = false;
            _eventsTokenSource.Cancel();
            while (_eventQueue.TryDequeue(out _))
            {
            }
        }

        internal void PublishEvent(MaestroEvent meastroEvent)
        {
            if (!_eventQueueOpen)
                return;

            _eventQueue.Enqueue(meastroEvent);
        }
    }
}
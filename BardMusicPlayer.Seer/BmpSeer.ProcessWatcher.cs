﻿#region

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.UtcMilliTime;
using BardMusicPlayer.Seer.Events;

#endregion

namespace BardMusicPlayer.Seer;

public sealed partial class BmpSeer
{
    private CancellationTokenSource _watcherTokenSource;

    private void StartProcessWatcher()
    {
        _watcherTokenSource = new CancellationTokenSource();

        Task.Factory.StartNew(() => RunProcessWatcher(_watcherTokenSource.Token), TaskCreationOptions.LongRunning);
    }

    private async Task RunProcessWatcher(CancellationToken token)
    {
        long coolDown = 0;
        while (!_watcherTokenSource.IsCancellationRequested)
        {
            try
            {
                var processes = Process.GetProcessesByName("ffxiv_dx11");

                foreach (var game in _games.Values.TakeWhile(game => !token.IsCancellationRequested).Where(game =>
                             game.Process is null || game.Process.HasExited || !game.Process.Responding ||
                             processes.All(process => process.Id != game.Pid)))
                {
                    _games.TryRemove(game.Pid, out _);
                    game?.Dispose();
                }

                foreach (var process in processes)
                {
                    if (token.IsCancellationRequested)
                        break;

                    // Add new games.
                    if (process is null || _games.ContainsKey(process.Id) || process.HasExited ||
                        !process.Responding)
                        continue;

                    // Adding a game spikes the cpu when sharlayan scans memory.
                    var timeNow = Clock.Time.Now;
                    if (coolDown + BmpPigeonhole.Instance.SeerGameScanCooldown > timeNow)
                        continue;

                    coolDown = timeNow;

                    var game = new Game(process);
                    if (!game.Initialize())
                    {
                        game.Dispose();
                    }
                    else
                    {
                        if (!_games.TryAdd(process.Id, game))
                            game.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                PublishEvent(new SeerExceptionEvent(ex));
            }

            await Task.Delay(1, token).ContinueWith(static tsk => { }, token);
        }
    }

    private void StopProcessWatcher()
    {
        _watcherTokenSource.Cancel();
    }
}
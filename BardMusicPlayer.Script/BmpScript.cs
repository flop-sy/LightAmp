#region

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;
using BasicSharp;

#endregion

namespace BardMusicPlayer.Script
{
    public class BmpScript
    {
        private static readonly Lazy<BmpScript> LazyInstance = new(() => new BmpScript());
        private Interpreter basic;

        private Thread thread;

        private BmpScript()
        {
        }

        /// <summary>
        /// </summary>
        public bool Started { get; private set; }

        public static BmpScript Instance => LazyInstance.Value;

        private int selectedBard { get; set; }
        private string selectedBardName { get; set; } = "";

        public event EventHandler<bool> OnRunningStateChanged;

        #region accessors

        public void StopExecution()
        {
            if (thread == null)
                return;
            if (basic == null)
                return;

            basic.StopExec();

            if (thread.ThreadState == ThreadState.Running)
                thread.Abort();
        }

        #endregion

        public void LoadAndRun(string basicfile)
        {
            var task = Task.Run(() =>
            {
                thread = Thread.CurrentThread;
                if (OnRunningStateChanged != null)
                    OnRunningStateChanged(this, true);
                basic = new Interpreter(File.ReadAllText(basicfile));
                basic.printHandler += Print;
                basic.selectedBardHandler += SetSelectedBard;
                basic.selectedBardAsStringHandler += SetSelectedBardName;
                try
                {
                    basic.Exec();
                }
                catch (Exception)
                {
                    Console.WriteLine("Error");
                }

                if (OnRunningStateChanged != null)
                    OnRunningStateChanged(this, false);

                basic.printHandler -= Print;
                basic.selectedBardHandler -= SetSelectedBard;
                basic.selectedBardAsStringHandler -= SetSelectedBardName;
                basic = null;
            });
        }

        /// <summary>
        ///     Start Script.
        /// </summary>
        public void Start()
        {
            if (Started) return;
            if (!BmpPigeonhole.Initialized)
                throw new BmpScriptException("Script requires Pigeonhole to be initialized.");
            if (!BmpSeer.Instance.Started) throw new BmpScriptException("Script requires Seer to be running.");
            Started = true;
        }

        /// <summary>
        ///     Stop Script.
        /// </summary>
        public void Stop()
        {
            if (!Started) return;
            Started = false;
        }

        ~BmpScript()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }

        #region Routine Handlers

        public void SetSelectedBard(int num)
        {
            selectedBardName = "";
            selectedBard = num;
        }

        public void SetSelectedBardName(string name)
        {
            selectedBard = -1;
            selectedBardName = name;
        }

        public void Print(ChatMessageChannelType type, string text)
        {
            if (selectedBard != -1)
                BmpMaestro.Instance.SendText(selectedBard, type, text);
            else
                BmpMaestro.Instance.SendText(selectedBardName, type, text);
        }

        #endregion
    }
}
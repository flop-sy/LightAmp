#region

using System;
using System.Threading.Tasks;

#endregion

namespace BardMusicPlayer.Quotidian
{
    public sealed class TaskQueue
    {
        private readonly object key = new();
        private Task previous = Task.FromResult(false);

        public Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            lock (key)
            {
                var next = previous.ContinueWith(t => taskGenerator()).Unwrap();
                previous = next;
                return next;
            }
        }

        public Task Enqueue(Func<Task> taskGenerator)
        {
            lock (key)
            {
                var next = previous.ContinueWith(t => taskGenerator()).Unwrap();
                previous = next;
                return next;
            }
        }
    }
}
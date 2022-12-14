#region

using System;
using System.Collections;
using System.Collections.Generic;
using BardMusicPlayer.Transmogrify.Song;

#endregion

namespace BardMusicPlayer.Coffer
{
    public sealed class BmpPlaylistDecorator : IPlaylist
    {
        private readonly BmpPlaylist target;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="target"></param>
        internal BmpPlaylistDecorator(BmpPlaylist target)
        {
            this.target = target ?? throw new NullReferenceException();
        }

        /// <inheritdoc />
        void IPlaylist.Add(BmpSong song)
        {
            target.Songs.Add(song);
        }

        /// <inheritdoc />
        void IPlaylist.Add(int idx, BmpSong song)
        {
            target.Songs.Insert(idx, song);
        }

        /// <inheritdoc />
        IEnumerator<BmpSong> IEnumerable<BmpSong>.GetEnumerator()
        {
            return target.Songs.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return target.Songs.GetEnumerator();
        }

        /// <inheritdoc />
        string IPlaylist.GetName()
        {
            return target.Name;
        }

        /// <inheritdoc />
        void IPlaylist.Move(int sourceIdx, int targetIdx)
        {
            var contents = target.Songs;
            var moveMe = contents[sourceIdx];
            contents.RemoveAt(sourceIdx);
            contents.Insert(targetIdx, moveMe);
        }

        /// <inheritdoc />
        void IPlaylist.Remove(int idx)
        {
            target.Songs.RemoveAt(idx);
        }

        /// <inheritdoc />
        void IPlaylist.Remove(BmpSong song)
        {
            target.Songs.Remove(song);
        }

        /// <inheritdoc />
        void IPlaylist.SetName(string name)
        {
            target.Name = name ?? throw new ArgumentNullException();
        }

        internal BmpPlaylist GetBmpPlaylist() => this.target;
    }
}
#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BardMusicPlayer.Coffer;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Transmogrify.Song;
using Microsoft.Win32;
using UI.Resources;

#endregion

namespace BardMusicPlayer.Ui.Functions
{
    /// <summary>
    ///     simplyfied functions both Ui are using
    /// </summary>
    public static class PlaylistFunctions
    {
        /// <summary>
        ///     Add file(s) to the playlist
        /// </summary>
        /// <param name="currentPlaylist"></param>
        /// <returns>true if success</returns>
        public static bool AddFilesToPlaylist(IPlaylist currentPlaylist)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = Globals.Globals.FileFilters,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true)
                return false;

            foreach (var song in openFileDialog.FileNames.Select(static d => BmpSong.OpenFile(d).Result))
            {
                if (currentPlaylist.SingleOrDefault(x => x.Title.Equals(song.Title)) == null)
                    currentPlaylist.Add(song);

                BmpCoffer.Instance.SaveSong(song);
            }

            BmpCoffer.Instance.SavePlaylist(currentPlaylist);
            return true;
        }

        /// <summary>
        ///     Add a folder + subfolders to the playlist
        /// </summary>
        /// <param name="currentPlaylist"></param>
        /// <returns>true if success</returns>
        public static bool AddFolderToPlaylist(IPlaylist currentPlaylist)
        {
            var dlg = new FolderPicker
            {
                InputPath = Directory.Exists(BmpPigeonhole.Instance.SongDirectory)
                    ? Path.GetFullPath(BmpPigeonhole.Instance.SongDirectory)
                    : Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };

            if (dlg.ShowDialog() != true) return false;

            var path = dlg.ResultPath;

            if (!Directory.Exists(path))
                return false;

            var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(static s => s.EndsWith(".mid", StringComparison.Ordinal) ||
                                   s.EndsWith(".mml", StringComparison.Ordinal) ||
                                   s.EndsWith(".mmsong", StringComparison.Ordinal)).ToArray();
            foreach (var song in files.Select(static d => BmpSong.OpenFile(d).Result))
            {
                if (currentPlaylist.SingleOrDefault(x => x.Title.Equals(song.Title)) == null)
                    currentPlaylist.Add(song);
                BmpCoffer.Instance.SaveSong(song);
            }

            BmpCoffer.Instance.SavePlaylist(currentPlaylist);
            return true;
        }

        /// <summary>
        ///     gets the first playlist or null if none was found
        /// </summary>
        public static IPlaylist GetFirstPlaylist()
        {
            return BmpCoffer.Instance.GetPlaylistNames().Count > 0
                ? BmpCoffer.Instance.GetPlaylist(BmpCoffer.Instance.GetPlaylistNames()[0])
                : null;
        }

        /// <summary>
        ///     Creates and return a new playlist or return the existing one with the given name
        /// </summary>
        /// <param name="playlistname"></param>
        public static IPlaylist CreatePlaylist(string playlistname)
        {
            return BmpCoffer.Instance.GetPlaylistNames().Contains(playlistname)
                ? BmpCoffer.Instance.GetPlaylist(playlistname)
                : BmpCoffer.CreatePlaylist(playlistname);
        }

        /// <summary>
        ///     Get a song fromt the playlist
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="songname"></param>
        public static BmpSong GetSongFromPlaylist(IPlaylist playlist, string songname)
        {
            return playlist?.FirstOrDefault(item => item.Title == songname);
        }

        /// <summary>
        ///     get the songnames as list
        /// </summary>
        /// <param name="playlist"></param>
        /// used: classic view
        public static List<string> GetCurrentPlaylistItems(IPlaylist playlist)
        {
            var data = new List<string>();
            if (playlist == null)
                return data;

            data.AddRange(playlist.Select(static item => item.Title));
            return data;
        }

        public static IEnumerable<string> GetCurrentPlaylistItems(IPlaylist playlist, bool withupselector = false)
        {
            var data = new List<string>();
            if (playlist == null)
                return data;

            if (withupselector)
                data.Add("..");
            data.AddRange(playlist.Select(static item => item.Title));
            return data;
        }

        public static TimeSpan GetTotalTime(IPlaylist playlist)
        {
            var totalTime = new TimeSpan(0);
            return playlist.Aggregate(totalTime, static (current, p) => current + p.Duration);
        }
    }
}
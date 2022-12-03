#region

using System.Collections.Generic;
using BardMusicPlayer.Transmogrify.Song;
using LiteDB;

#endregion

namespace BardMusicPlayer.Coffer
{
    public sealed class BmpPlaylist
    {
        [BsonId] public ObjectId Id { get; set; }

        public string Name { get; set; }

        [BsonRef(Constants.SONG_COL_NAME)] public List<BmpSong> Songs { get; set; }
    }
}
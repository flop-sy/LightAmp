#region

using LiteDB;

#endregion

namespace BardMusicPlayer.Coffer
{
    public sealed class LiteDBSchema
    {
        [BsonId] public int Id { get; set; } = Constants.SCHEMA_DOCUMENT_ID;

        public static byte Version => Constants.SCHEMA_VERSION;
    }
}
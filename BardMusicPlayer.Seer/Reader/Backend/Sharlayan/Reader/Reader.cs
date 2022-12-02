namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Reader
{
    internal partial class Reader
    {
        public Reader(MemoryHandler memoryHandler)
        {
            Scanner = memoryHandler.Scanner;
            MemoryHandler = memoryHandler;
            MemoryHandler.Reader = this;
            _chatLogReader = new ChatLogReader(memoryHandler);
        }

        public Scanner Scanner { get; set; }

        public MemoryHandler MemoryHandler { get; set; }
    }
}
namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan
{
    public class BmpSeerSharlayanSigException : BmpSeerException
    {
        public BmpSeerSharlayanSigException(string message) : base("Unable to find memory signature for: " + message)
        {
        }
    }
}
namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan;

public sealed class BmpSeerSharlayanSigException : BmpSeerException
{
    public BmpSeerSharlayanSigException(string message) : base("Unable to find memory signature for: " + message)
    {
    }
}
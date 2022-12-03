namespace BardMusicPlayer.Siren.AlphaTab.Collections
{
    internal sealed class StringBuilder
    {
        private readonly System.Text.StringBuilder _sb;

        public StringBuilder()
        {
            _sb = new System.Text.StringBuilder();
        }

        public void AppendChar(int i)
        {
            _sb.Append(Platform.StringFromCharCode(i));
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
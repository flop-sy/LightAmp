#region

using System.Linq;
using System.Text;

#endregion

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities
{
    internal static class XMLCleaner
    {
        public static string SanitizeXmlString(string xValue)
        {
            if (xValue == null) return string.Empty;

            var stringBuilder = new StringBuilder(xValue.Length);

            foreach (var item in xValue.Where(static xChar => IsLegalXmlChar(xChar))) stringBuilder.Append(item);

            return stringBuilder.ToString();
        }

        private static bool IsLegalXmlChar(int xChar)
        {
            return xChar is 9 or 10 or 13 or >= 32 and <= 55295 or >= 57344 and <= 65533 or >= 65536 and <= 1114111;
        }
    }
}
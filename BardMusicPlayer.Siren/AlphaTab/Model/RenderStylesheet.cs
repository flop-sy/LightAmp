namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    ///     This class represents the rendering stylesheet.
    ///     It contains settings which control the display of the score when rendered.
    /// </summary>
    internal sealed class RenderStylesheet
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RenderStylesheet" /> class.
        /// </summary>
        public RenderStylesheet()
        {
            HideDynamics = false;
        }

        /// <summary>
        ///     Gets or sets whether dynamics are hidden.
        /// </summary>
        public bool HideDynamics { get; set; }

        internal static void CopyTo(RenderStylesheet src, RenderStylesheet dst)
        {
            dst.HideDynamics = src.HideDynamics;
        }
    }
}
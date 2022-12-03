#region

using System;
using BardMusicPlayer.Ui.Globals.SkinContainer;

#endregion

namespace BardMusicPlayer.Ui.Skinned
{
    /// <summary>
    ///     does nothing, but looks fancy
    /// </summary>
    public sealed partial class Skinned_MainView_Ex
    {
        public Skinned_MainView_Ex()
        {
            InitializeComponent();
            SkinContainer.OnNewSkinLoaded += SkinContainer_OnNewSkinLoaded;
            SkinContainer_OnNewSkinLoaded(null, null);
        }

        private void SkinContainer_OnNewSkinLoaded(object sender, EventArgs e)
        {
            Background = SkinContainer.EQUALIZER[0]; //Temp
        }
    }
}
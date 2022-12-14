#region

using System;
using System.Windows;
using System.Windows.Input;
using BardMusicPlayer.Ui.Globals.SkinContainer;

#endregion

namespace BardMusicPlayer.Ui.Skinned;

/// <summary>
///     Only a container window
/// </summary>
public sealed partial class BardsWindow
{
    public BardsWindow()
    {
        InitializeComponent();
        ApplySkin();
        SkinContainer.OnNewSkinLoaded += SkinContainer_OnNewSkinLoaded;
    }

    #region Skinning

    private void SkinContainer_OnNewSkinLoaded(object sender, EventArgs e)
    {
        ApplySkin();
    }

    public void ApplySkin()
    {
        BARDS_TOP_LEFT.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_LEFT_CORNER];
        BARDS_TOP_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_TILE];
        BARDS_TOP_RIGHT.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_TOP_RIGHT_CORNER];

        BARDS_BOTTOM_LEFT_CORNER.Fill =
            SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_LEFT_CORNER];
        BARDS_BOTTOM_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_TILE];
        BARDS_BOTTOM_RIGHT_CORNER.Fill =
            SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_BOTTOM_RIGHT_CORNER];

        BARDS_LEFT_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_LEFT_TILE];
        BARDS_RIGHT_TILE.Fill = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_RIGHT_TILE];

        Close_Button.Background = SkinContainer.SWINDOW[SkinContainer.SWINDOW_TYPES.SWINDOW_CLOSE_SELECTED];
        Close_Button.Background.Opacity = 0;
    }

    #endregion

    #region Titlebar functions and buttons

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }


    private void Close_Button_Click(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Hidden;
    }

    private void Close_Button_Down(object sender, MouseButtonEventArgs e)
    {
        Close_Button.Background.Opacity = 1;
    }

    private void Close_Button_Up(object sender, MouseButtonEventArgs e)
    {
        Close_Button.Background.Opacity = 0;
    }

    #endregion
}
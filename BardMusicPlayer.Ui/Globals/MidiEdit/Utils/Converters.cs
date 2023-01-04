#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BardMusicPlayer.Ui.MidiEdit.Managers;

#endregion

namespace BardMusicPlayer.Ui.MidiEdit.Utils.Converters;

/// Permits cell tiling zoom binding
public class DoubleToRectConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new Rect(0, 0,
            UiManager.Instance.mainWindow.Model.XZoom * 5,
            UiManager.Instance.mainWindow.Model.YZoom * 5
        );
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
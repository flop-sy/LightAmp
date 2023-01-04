#region

using System.ComponentModel;

#endregion

namespace BardMusicPlayer.Ui.MidiEdit.Utils;

/// heritate this instead of INotifyPropertyChanged
/// prevent from implementing it in each model
public class HandleBinding : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(string property)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
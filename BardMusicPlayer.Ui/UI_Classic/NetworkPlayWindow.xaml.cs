#region

using System.ComponentModel;
using System.Windows;

#endregion

namespace BardMusicPlayer.Ui.Classic
{
    /// <summary>
    ///     Interaktionslogik für NetworkPlayWindow.xaml
    /// </summary>
    public partial class NetworkPlayWindow
    {
        public NetworkPlayWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}
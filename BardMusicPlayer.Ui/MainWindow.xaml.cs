#region

using System.Reflection;
using System.Windows;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Ui.Classic;
using BardMusicPlayer.Ui.Skinned;

#endregion

namespace BardMusicPlayer.Ui
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (BmpPigeonhole.Instance.ClassicUi)
                SwitchClassicStyle();
            else
                SwitchSkinnedStyle();
        }

        public void SwitchClassicStyle()
        {
            Title = "LightAmp Ver:" + Assembly.GetExecutingAssembly().GetName().Version;
            DataContext = new Classic_MainView();
            AllowsTransparency = false;
            WindowStyle = WindowStyle.SingleBorderWindow;
            Height = 520;
            Width = 830;
            ResizeMode = ResizeMode.CanResizeWithGrip;
        }

        public void SwitchSkinnedStyle()
        {
            DataContext = new Skinned_MainView();
            AllowsTransparency = true;
            Height = 174;
            Width = 412;
            ResizeMode = ResizeMode.NoResize;
        }
    }
}
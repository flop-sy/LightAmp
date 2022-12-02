#region

using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using BardMusicPlayer.Ui.Functions;

#endregion

namespace BardMusicPlayer.Ui.Controls
{
    /// <summary>
    ///     Interaktionslogik für TrackNumericUpDown.xaml
    /// </summary>
    public partial class TrackNumericUpDown : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(TrackNumericUpDown),
                new PropertyMetadata(OnValueChangedCallBack));


        /* Track UP/Down */
        private int _numValue = 1;
        public EventHandler<int> OnValueChanged;

        public TrackNumericUpDown()
        {
            InitializeComponent();
        }

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public int NumValue
        {
            get => _numValue;
            set
            {
                _numValue = value;
                Text.Text = "T" + NumValue;
                OnValueChanged?.Invoke(this, _numValue);
            }
        }

        private static void OnValueChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as TrackNumericUpDown;
            if (c != null) c.OnValueChangedC(c.Value);
        }

        protected virtual void OnValueChangedC(string c)
        {
            NumValue = Convert.ToInt32(c);
        }

        private void NumUp_Click(object sender, RoutedEventArgs e)
        {
            if (PlaybackFunctions.CurrentSong == null)
                return;
            if (NumValue + 1 > PlaybackFunctions.CurrentSong.TrackContainers.Count)
                return;
            NumValue++;
        }

        private void NumDown_Click(object sender, RoutedEventArgs e)
        {
            if (NumValue - 1 < 0)
                return;
            NumValue--;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Text == null)
                return;

            var val = 0;
            var str = Regex.Replace(Text.Text, "[^0-9]", "");
            if (int.TryParse(str, out val))
            {
                if (PlaybackFunctions.CurrentSong == null)
                    return;

                if (val < 0 || NumValue + 1 > PlaybackFunctions.CurrentSong.TrackContainers.Count)
                {
                    NumValue = NumValue;
                    return;
                }

                NumValue = val;
            }
        }
    }
}
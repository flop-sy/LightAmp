#region

using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace BardMusicPlayer.Ui.Controls
{
    /// <summary>
    ///     Interaktionslogik für NumericUpDown.xaml
    /// </summary>
    public sealed partial class NumericUpDown
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumericUpDown),
                new PropertyMetadata(OnValueChangedCallBack));


        /* Track UP/Down */
        private int _numValue;
        public EventHandler<int> OnValueChanged;

        public NumericUpDown()
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
                Text.Text = NumValue.ToString();
                OnValueChanged?.Invoke(this, _numValue);
            }
        }

        private static void OnValueChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as NumericUpDown;
            c?.OnValueChangedC(c.Value);
        }

        private void OnValueChangedC(string c)
        {
            NumValue = Convert.ToInt32(c);
        }

        private void NumUp_Click(object sender, RoutedEventArgs e)
        {
            NumValue++;
        }

        private void NumDown_Click(object sender, RoutedEventArgs e)
        {
            NumValue--;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Text == null)
                return;

            var val = 0;
            var str = Regex.Replace(Text.Text, @"[^\d|\.\-]", "");
            if (int.TryParse(str, out val)) NumValue = val;
        }
    }
}
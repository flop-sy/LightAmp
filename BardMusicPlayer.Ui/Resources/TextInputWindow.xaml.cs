#region

using System.Windows;

#endregion

namespace UI.Resources;

/// <summary>
///     Interaktionslogik für TextInputWindow.xaml
/// </summary>
public sealed partial class TextInputWindow
{
    public TextInputWindow(string infotext, int maxinputlength = 42)
    {
        InitializeComponent();
        InfoText.Text = infotext;
        ResponseTextBox.Focus();
        ResponseTextBox.MaxLength = maxinputlength;
    }

    public string ResponseText
    {
        get => ResponseTextBox.Text;
        set => ResponseTextBox.Text = value;
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
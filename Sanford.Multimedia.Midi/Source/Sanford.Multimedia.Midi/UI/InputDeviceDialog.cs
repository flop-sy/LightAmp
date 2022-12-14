#region

using System;
using System.Windows.Forms;

#endregion

namespace Sanford.Multimedia.Midi.UI;

public partial class InputDeviceDialog : Form
{
    private int inputDeviceID;

    public InputDeviceDialog()
    {
        InitializeComponent();

        if (InputDevice.DeviceCount <= 0) return;

        for (var i = 0; i < InputDevice.DeviceCount; i++)
            inputComboBox.Items.Add(InputDevice.GetDeviceCapabilities(i).name);

        inputComboBox.SelectedIndex = inputDeviceID;
    }

    public int InputDeviceID
    {
        get
        {
            #region Require

            if (InputDevice.DeviceCount == 0) throw new InvalidOperationException();

            #endregion

            return inputDeviceID;
        }
    }

    protected override void OnShown(EventArgs e)
    {
        if (InputDevice.DeviceCount > 0) inputComboBox.SelectedIndex = inputDeviceID;

        base.OnShown(e);
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        if (InputDevice.DeviceCount > 0) inputDeviceID = inputComboBox.SelectedIndex;

        DialogResult = DialogResult.OK;
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
    }
}
#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

#region

using System;
using System.Windows.Forms;

#endregion

namespace Sanford.Multimedia.Midi.UI
{
    public partial class OutputDeviceDialog : Form
    {
        private int outputDeviceID;

        public OutputDeviceDialog()
        {
            InitializeComponent();

            if (OutputDeviceBase.DeviceCount <= 0) return;

            for (var i = 0; i < OutputDeviceBase.DeviceCount; i++)
                outputComboBox.Items.Add(OutputDeviceBase.GetDeviceCapabilities(i).name);

            outputComboBox.SelectedIndex = outputDeviceID;
        }

        public int OutputDeviceID
        {
            get
            {
                #region Require

                if (OutputDeviceBase.DeviceCount == 0) throw new InvalidOperationException();

                #endregion

                return outputDeviceID;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            if (OutputDeviceBase.DeviceCount > 0) outputComboBox.SelectedIndex = outputDeviceID;

            base.OnShown(e);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (OutputDeviceBase.DeviceCount > 0) outputDeviceID = outputComboBox.SelectedIndex;

            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
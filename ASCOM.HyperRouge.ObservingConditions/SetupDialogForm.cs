using ASCOM.Utilities;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ASCOM.HyperRouge {
   [ComVisible(false)]              // Form not registered for COM!
   public partial class SetupDialogForm : Form {
      readonly TraceLogger tl; // Holder for a reference to the driver's trace logger

      public SetupDialogForm(TraceLogger tlDriver) {
         InitializeComponent();

         // Save the provided trace logger for use within the setup dialogue
         tl = tlDriver;

         // Initialise current values of user settings from the ASCOM Profile
         InitUI();
      }

      private void cmdOK_Click(object sender, EventArgs e) {
         // Place any validation constraint checks here
         // Update the state variables with results from the dialogue
         ObservingConditions.DatasUrl = txtDataUrl.Text;
         ObservingConditions.TraceState = chkTrace.Checked;
         tl.Enabled = chkTrace.Checked;
      }

      private void cmdCancel_Click(object sender, EventArgs e) => Close();

      private void BrowseToAscom(object sender, EventArgs e) {
         try {
            System.Diagnostics.Process.Start("https://ascom-standards.org/");
         } catch (System.ComponentModel.Win32Exception noBrowser) {
            if (noBrowser.ErrorCode == -2147467259)
               MessageBox.Show(noBrowser.Message);
         } catch (System.Exception other) {
            MessageBox.Show(other.Message);
         }
      }

      private void InitUI() {
         chkTrace.Checked = ObservingConditions.TraceState;
         txtDataUrl.Text = ObservingConditions.DatasUrl;
      }
   }
}
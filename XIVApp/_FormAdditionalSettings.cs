using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FFACETools;

namespace XIVApp
{
    public partial class FormAdditionalSettings : Form
    {
        private RichTextBox DebugLog { get; set; }
        private FFACE Session { get; set; }

        public FormAdditionalSettings (FFACE session, RichTextBox debuglog)
        {
            InitializeComponent();
            Session = session;
            DebugLog = debuglog;

            InitializeFormValues();
        }

        private void InitializeFormValues()
        {
            //RestUntilMPTextBox.Text = Session.Player.MPMax.ToString();
        }

        private void FormAdditionalSettings_FormClosing (object sender, FormClosingEventArgs e)
        {
            // Put code to save settings here and other crap
        }
        //
        private void RestBelowTextbox_TextChanged (object sender, EventArgs e)
        {

        }

        private void RestUntilHPPTextbox_TextChanged (object sender, EventArgs e)
        {

        }

        private void RestBelowMPTextbox_TextChanged (object sender, EventArgs e)
        {

        }

        private void RestUntilMPTextBox_TextChanged (object sender, EventArgs e)
        {

        }
    }
}

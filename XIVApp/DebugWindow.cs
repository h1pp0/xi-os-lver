using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XIVApp
{
    public partial class DebugWindow : Form
    {
        public List<string> DebugLines = new List<string>();

        public DebugWindow()
        {
            InitializeComponent();
        }

        private void Logger_Tick(object sender, EventArgs e)
        {
            try
            {
                DebugWindower.Clear();
                foreach (String line in DebugLines)
                {
                    DebugWindower.AppendText(line + Environment.NewLine);
                }
            }
            catch (Exception exc)
            {

            }
        }

        private void DebugWindow_Load(object sender, EventArgs e)
        {
            DebugWindower.AppendText("Debugger Loaded...");
        }
    }
}

namespace XIVApp
{
    partial class DebugWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugWindow));
            this.DebugWindower = new System.Windows.Forms.RichTextBox();
            this.Logger = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // DebugWindower
            // 
            this.DebugWindower.BackColor = System.Drawing.Color.Black;
            this.DebugWindower.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DebugWindower.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DebugWindower.ForeColor = System.Drawing.Color.White;
            this.DebugWindower.Location = new System.Drawing.Point(0, 0);
            this.DebugWindower.Name = "DebugWindower";
            this.DebugWindower.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.DebugWindower.Size = new System.Drawing.Size(1022, 571);
            this.DebugWindower.TabIndex = 0;
            this.DebugWindower.Text = "";
            // 
            // Logger
            // 
            this.Logger.Enabled = true;
            this.Logger.Interval = 2000;
            this.Logger.Tick += new System.EventHandler(this.Logger_Tick);
            // 
            // DebugWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1022, 571);
            this.Controls.Add(this.DebugWindower);
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DebugWindow";
            this.Text = "DebugWindow";
            this.Load += new System.EventHandler(this.DebugWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox DebugWindower;
        private System.Windows.Forms.Timer Logger;
    }
}
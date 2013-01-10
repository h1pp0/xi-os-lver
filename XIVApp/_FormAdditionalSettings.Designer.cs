namespace XIVApp
{
    partial class FormAdditionalSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing && ( components != null ))
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
        private void InitializeComponent ()
        {
            this.AdditionalSettingsTabControl = new System.Windows.Forms.TabControl();
            this.SpellsAbilitiesAdvSettingsTab = new System.Windows.Forms.TabPage();
            this.LevelingAdvSettingsTab = new System.Windows.Forms.TabPage();
            this.SpellorAbilityLabel = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SpellorAbilityWhen = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.SpellorAbilityWhenIsLabel = new System.Windows.Forms.Label();
            this.CommandParameterLabelBox = new System.Windows.Forms.ListBox();
            this.AdvancedSettingsSaveButton = new System.Windows.Forms.Button();
            this.AdvancedSettingsCancelButton = new System.Windows.Forms.Button();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.SpellorAbilityWhenIsPriorityLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.DashLabel = new System.Windows.Forms.Label();
            this.AdditionalSettingsTabControl.SuspendLayout();
            this.SpellsAbilitiesAdvSettingsTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // AdditionalSettingsTabControl
            // 
            this.AdditionalSettingsTabControl.Controls.Add(this.SpellsAbilitiesAdvSettingsTab);
            this.AdditionalSettingsTabControl.Controls.Add(this.LevelingAdvSettingsTab);
            this.AdditionalSettingsTabControl.Location = new System.Drawing.Point(12, 12);
            this.AdditionalSettingsTabControl.Name = "AdditionalSettingsTabControl";
            this.AdditionalSettingsTabControl.SelectedIndex = 0;
            this.AdditionalSettingsTabControl.Size = new System.Drawing.Size(594, 241);
            this.AdditionalSettingsTabControl.TabIndex = 39;
            // 
            // SpellsAbilitiesAdvSettingsTab
            // 
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.DashLabel);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.textBox2);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.textBox1);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.label1);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.SpellorAbilityWhenIsPriorityLabel);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.comboBox4);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.AdvancedSettingsCancelButton);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.AdvancedSettingsSaveButton);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.CommandParameterLabelBox);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.SpellorAbilityWhenIsLabel);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.comboBox3);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.comboBox2);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.SpellorAbilityWhen);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.comboBox1);
            this.SpellsAbilitiesAdvSettingsTab.Controls.Add(this.SpellorAbilityLabel);
            this.SpellsAbilitiesAdvSettingsTab.Location = new System.Drawing.Point(4, 22);
            this.SpellsAbilitiesAdvSettingsTab.Name = "SpellsAbilitiesAdvSettingsTab";
            this.SpellsAbilitiesAdvSettingsTab.Padding = new System.Windows.Forms.Padding(3);
            this.SpellsAbilitiesAdvSettingsTab.Size = new System.Drawing.Size(586, 215);
            this.SpellsAbilitiesAdvSettingsTab.TabIndex = 0;
            this.SpellsAbilitiesAdvSettingsTab.Text = "Spells and Abilities";
            this.SpellsAbilitiesAdvSettingsTab.UseVisualStyleBackColor = true;
            // 
            // LevelingAdvSettingsTab
            // 
            this.LevelingAdvSettingsTab.Location = new System.Drawing.Point(4, 22);
            this.LevelingAdvSettingsTab.Name = "LevelingAdvSettingsTab";
            this.LevelingAdvSettingsTab.Padding = new System.Windows.Forms.Padding(3);
            this.LevelingAdvSettingsTab.Size = new System.Drawing.Size(586, 215);
            this.LevelingAdvSettingsTab.TabIndex = 1;
            this.LevelingAdvSettingsTab.Text = "Leveling";
            this.LevelingAdvSettingsTab.UseVisualStyleBackColor = true;
            // 
            // SpellorAbilityLabel
            // 
            this.SpellorAbilityLabel.AutoSize = true;
            this.SpellorAbilityLabel.Location = new System.Drawing.Point(35, 19);
            this.SpellorAbilityLabel.Name = "SpellorAbilityLabel";
            this.SpellorAbilityLabel.Size = new System.Drawing.Size(72, 13);
            this.SpellorAbilityLabel.TabIndex = 56;
            this.SpellorAbilityLabel.Text = "Spell or Ability";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(7, 35);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(130, 21);
            this.comboBox1.TabIndex = 57;
            // 
            // SpellorAbilityWhen
            // 
            this.SpellorAbilityWhen.AutoSize = true;
            this.SpellorAbilityWhen.Location = new System.Drawing.Point(162, 19);
            this.SpellorAbilityWhen.Name = "SpellorAbilityWhen";
            this.SpellorAbilityWhen.Size = new System.Drawing.Size(36, 13);
            this.SpellorAbilityWhen.TabIndex = 58;
            this.SpellorAbilityWhen.Text = "When";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Ability",
            "Mob HPP",
            "PL HP",
            "PL MP",
            "PL TP",
            "Self HP",
            "Self MP",
            "Self TP",
            "Spell"});
            this.comboBox2.Location = new System.Drawing.Point(143, 35);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(74, 21);
            this.comboBox2.Sorted = true;
            this.comboBox2.TabIndex = 59;
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "<",
            "<=",
            ">",
            ">=",
            "Is Active",
            "Range",
            "Wears Off"});
            this.comboBox3.Location = new System.Drawing.Point(223, 35);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(67, 21);
            this.comboBox3.Sorted = true;
            this.comboBox3.TabIndex = 60;
            // 
            // SpellorAbilityWhenIsLabel
            // 
            this.SpellorAbilityWhenIsLabel.AutoSize = true;
            this.SpellorAbilityWhenIsLabel.Location = new System.Drawing.Point(248, 16);
            this.SpellorAbilityWhenIsLabel.Name = "SpellorAbilityWhenIsLabel";
            this.SpellorAbilityWhenIsLabel.Size = new System.Drawing.Size(15, 13);
            this.SpellorAbilityWhenIsLabel.TabIndex = 61;
            this.SpellorAbilityWhenIsLabel.Text = "Is";
            // 
            // CommandParameterLabelBox
            // 
            this.CommandParameterLabelBox.FormattingEnabled = true;
            this.CommandParameterLabelBox.Location = new System.Drawing.Point(296, 14);
            this.CommandParameterLabelBox.Name = "CommandParameterLabelBox";
            this.CommandParameterLabelBox.Size = new System.Drawing.Size(283, 186);
            this.CommandParameterLabelBox.TabIndex = 62;
            // 
            // AdvancedSettingsSaveButton
            // 
            this.AdvancedSettingsSaveButton.Location = new System.Drawing.Point(134, 177);
            this.AdvancedSettingsSaveButton.Name = "AdvancedSettingsSaveButton";
            this.AdvancedSettingsSaveButton.Size = new System.Drawing.Size(75, 23);
            this.AdvancedSettingsSaveButton.TabIndex = 63;
            this.AdvancedSettingsSaveButton.Text = "Save";
            this.AdvancedSettingsSaveButton.UseVisualStyleBackColor = true;
            // 
            // AdvancedSettingsCancelButton
            // 
            this.AdvancedSettingsCancelButton.Location = new System.Drawing.Point(215, 177);
            this.AdvancedSettingsCancelButton.Name = "AdvancedSettingsCancelButton";
            this.AdvancedSettingsCancelButton.Size = new System.Drawing.Size(75, 23);
            this.AdvancedSettingsCancelButton.TabIndex = 64;
            this.AdvancedSettingsCancelButton.Text = "Cancel";
            this.AdvancedSettingsCancelButton.UseVisualStyleBackColor = true;
            // 
            // comboBox4
            // 
            this.comboBox4.FormatString = "N0";
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Location = new System.Drawing.Point(223, 62);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(67, 21);
            this.comboBox4.Sorted = true;
            this.comboBox4.TabIndex = 65;
            // 
            // SpellorAbilityWhenIsPriorityLabel
            // 
            this.SpellorAbilityWhenIsPriorityLabel.AutoSize = true;
            this.SpellorAbilityWhenIsPriorityLabel.Location = new System.Drawing.Point(174, 65);
            this.SpellorAbilityWhenIsPriorityLabel.Name = "SpellorAbilityWhenIsPriorityLabel";
            this.SpellorAbilityWhenIsPriorityLabel.Size = new System.Drawing.Size(38, 13);
            this.SpellorAbilityWhenIsPriorityLabel.TabIndex = 66;
            this.SpellorAbilityWhenIsPriorityLabel.Text = "Priority";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 67;
            this.label1.Text = "Range:";
            this.label1.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(52, 99);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(40, 20);
            this.textBox1.TabIndex = 68;
            this.textBox1.Visible = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(114, 99);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(40, 20);
            this.textBox2.TabIndex = 69;
            this.textBox2.Visible = false;
            // 
            // DashLabel
            // 
            this.DashLabel.AutoSize = true;
            this.DashLabel.Location = new System.Drawing.Point(98, 102);
            this.DashLabel.Name = "DashLabel";
            this.DashLabel.Size = new System.Drawing.Size(10, 13);
            this.DashLabel.TabIndex = 70;
            this.DashLabel.Text = "-";
            this.DashLabel.Visible = false;
            // 
            // FormAdditionalSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 265);
            this.Controls.Add(this.AdditionalSettingsTabControl);
            this.Name = "FormAdditionalSettings";
            this.Text = "FormAdditionalSettings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAdditionalSettings_FormClosing);
            this.AdditionalSettingsTabControl.ResumeLayout(false);
            this.SpellsAbilitiesAdvSettingsTab.ResumeLayout(false);
            this.SpellsAbilitiesAdvSettingsTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl AdditionalSettingsTabControl;
        private System.Windows.Forms.TabPage SpellsAbilitiesAdvSettingsTab;
        private System.Windows.Forms.TabPage LevelingAdvSettingsTab;
        private System.Windows.Forms.Label DashLabel;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label SpellorAbilityWhenIsPriorityLabel;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.Button AdvancedSettingsCancelButton;
        private System.Windows.Forms.Button AdvancedSettingsSaveButton;
        private System.Windows.Forms.ListBox CommandParameterLabelBox;
        private System.Windows.Forms.Label SpellorAbilityWhenIsLabel;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label SpellorAbilityWhen;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label SpellorAbilityLabel;

    }
}
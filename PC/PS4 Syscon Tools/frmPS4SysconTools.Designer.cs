namespace PS4_Syscon_Tools
{
    partial class frmPS4SysconTools
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPS4SysconTools));
            staMain = new System.Windows.Forms.StatusStrip();
            tslSysconTool = new System.Windows.Forms.ToolStripStatusLabel();
            tslSysconToolValue = new System.Windows.Forms.ToolStripStatusLabel();
            tslVersion = new System.Windows.Forms.ToolStripStatusLabel();
            tslVersionValue = new System.Windows.Forms.ToolStripStatusLabel();
            grbSysconProcess = new System.Windows.Forms.GroupBox();
            chkAutoVerify = new System.Windows.Forms.CheckBox();
            chkEnableDebugMode = new System.Windows.Forms.CheckBox();
            nudNoOfDumps = new System.Windows.Forms.NumericUpDown();
            lblNoOfDumps = new System.Windows.Forms.Label();
            nudEndBlock = new System.Windows.Forms.NumericUpDown();
            nudStartBlock = new System.Windows.Forms.NumericUpDown();
            lblEndBlock = new System.Windows.Forms.Label();
            lblStartBlock = new System.Windows.Forms.Label();
            btnBrowse = new System.Windows.Forms.Button();
            txtInputOutputFile = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            cboSysconProcess = new System.Windows.Forms.ComboBox();
            sysconProcessBindingSource = new System.Windows.Forms.BindingSource(components);
            lblSysconProcessType = new System.Windows.Forms.Label();
            txtLog = new System.Windows.Forms.TextBox();
            btnStart = new System.Windows.Forms.Button();
            mnuMain = new System.Windows.Forms.MenuStrip();
            mnuDevices = new System.Windows.Forms.ToolStripMenuItem();
            mnuResetPS4SysconFlasher = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
            mnuEnableAdvancedOptions = new System.Windows.Forms.ToolStripMenuItem();
            mnuEnableAutoDebugMode = new System.Windows.Forms.ToolStripMenuItem();
            mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            grbProcess = new System.Windows.Forms.GroupBox();
            prbProgress = new System.Windows.Forms.ProgressBar();
            lblProgress = new System.Windows.Forms.Label();
            grbLog = new System.Windows.Forms.GroupBox();
            dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            deviceWatcher = new System.ComponentModel.BackgroundWorker();
            staMain.SuspendLayout();
            grbSysconProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudNoOfDumps).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudEndBlock).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudStartBlock).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sysconProcessBindingSource).BeginInit();
            mnuMain.SuspendLayout();
            grbProcess.SuspendLayout();
            grbLog.SuspendLayout();
            SuspendLayout();
            // 
            // staMain
            // 
            staMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            staMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tslSysconTool, tslSysconToolValue, tslVersion, tslVersionValue });
            staMain.Location = new System.Drawing.Point(0, 607);
            staMain.Name = "staMain";
            staMain.Padding = new System.Windows.Forms.Padding(1, 0, 17, 0);
            staMain.Size = new System.Drawing.Size(616, 22);
            staMain.TabIndex = 8;
            staMain.Text = "staMain";
            // 
            // tslSysconTool
            // 
            tslSysconTool.ForeColor = System.Drawing.Color.Blue;
            tslSysconTool.Name = "tslSysconTool";
            tslSysconTool.Size = new System.Drawing.Size(75, 17);
            tslSysconTool.Text = "Syscon Tool :";
            // 
            // tslSysconToolValue
            // 
            tslSysconToolValue.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
            tslSysconToolValue.Name = "tslSysconToolValue";
            tslSysconToolValue.Size = new System.Drawing.Size(0, 17);
            // 
            // tslVersion
            // 
            tslVersion.ForeColor = System.Drawing.Color.Blue;
            tslVersion.Margin = new System.Windows.Forms.Padding(80, 3, 0, 2);
            tslVersion.Name = "tslVersion";
            tslVersion.Size = new System.Drawing.Size(48, 17);
            tslVersion.Text = "Version:";
            // 
            // tslVersionValue
            // 
            tslVersionValue.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
            tslVersionValue.Name = "tslVersionValue";
            tslVersionValue.Size = new System.Drawing.Size(0, 17);
            // 
            // grbSysconProcess
            // 
            grbSysconProcess.Controls.Add(chkAutoVerify);
            grbSysconProcess.Controls.Add(chkEnableDebugMode);
            grbSysconProcess.Controls.Add(nudNoOfDumps);
            grbSysconProcess.Controls.Add(lblNoOfDumps);
            grbSysconProcess.Controls.Add(nudEndBlock);
            grbSysconProcess.Controls.Add(nudStartBlock);
            grbSysconProcess.Controls.Add(lblEndBlock);
            grbSysconProcess.Controls.Add(lblStartBlock);
            grbSysconProcess.Controls.Add(btnBrowse);
            grbSysconProcess.Controls.Add(txtInputOutputFile);
            grbSysconProcess.Controls.Add(label1);
            grbSysconProcess.Controls.Add(cboSysconProcess);
            grbSysconProcess.Controls.Add(lblSysconProcessType);
            grbSysconProcess.Location = new System.Drawing.Point(10, 25);
            grbSysconProcess.Margin = new System.Windows.Forms.Padding(4);
            grbSysconProcess.Name = "grbSysconProcess";
            grbSysconProcess.Padding = new System.Windows.Forms.Padding(4);
            grbSysconProcess.Size = new System.Drawing.Size(595, 165);
            grbSysconProcess.TabIndex = 6;
            grbSysconProcess.TabStop = false;
            grbSysconProcess.Text = "PS4 Syscon Process";
            // 
            // chkAutoVerify
            // 
            chkAutoVerify.AutoSize = true;
            chkAutoVerify.Location = new System.Drawing.Point(297, 136);
            chkAutoVerify.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            chkAutoVerify.Name = "chkAutoVerify";
            chkAutoVerify.Size = new System.Drawing.Size(115, 19);
            chkAutoVerify.TabIndex = 24;
            chkAutoVerify.Text = "Verify After Write";
            chkAutoVerify.UseVisualStyleBackColor = true;
            chkAutoVerify.CheckedChanged += chkAutoVerify_CheckedChanged;
            // 
            // chkEnableDebugMode
            // 
            chkEnableDebugMode.AutoSize = true;
            chkEnableDebugMode.Location = new System.Drawing.Point(122, 136);
            chkEnableDebugMode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            chkEnableDebugMode.Name = "chkEnableDebugMode";
            chkEnableDebugMode.Size = new System.Drawing.Size(133, 19);
            chkEnableDebugMode.TabIndex = 23;
            chkEnableDebugMode.Text = "Enable Debug Mode";
            chkEnableDebugMode.UseVisualStyleBackColor = true;
            chkEnableDebugMode.CheckedChanged += chkEnableDebugMode_CheckedChanged;
            // 
            // nudNoOfDumps
            // 
            nudNoOfDumps.Location = new System.Drawing.Point(514, 104);
            nudNoOfDumps.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            nudNoOfDumps.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
            nudNoOfDumps.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudNoOfDumps.Name = "nudNoOfDumps";
            nudNoOfDumps.Size = new System.Drawing.Size(71, 23);
            nudNoOfDumps.TabIndex = 21;
            nudNoOfDumps.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            nudNoOfDumps.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // lblNoOfDumps
            // 
            lblNoOfDumps.AutoSize = true;
            lblNoOfDumps.Location = new System.Drawing.Point(413, 106);
            lblNoOfDumps.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNoOfDumps.Name = "lblNoOfDumps";
            lblNoOfDumps.Size = new System.Drawing.Size(86, 15);
            lblNoOfDumps.TabIndex = 20;
            lblNoOfDumps.Text = "No. Of Dumps:";
            // 
            // nudEndBlock
            // 
            nudEndBlock.Location = new System.Drawing.Point(297, 104);
            nudEndBlock.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            nudEndBlock.Maximum = new decimal(new int[] { 511, 0, 0, 0 });
            nudEndBlock.Name = "nudEndBlock";
            nudEndBlock.Size = new System.Drawing.Size(71, 23);
            nudEndBlock.TabIndex = 19;
            nudEndBlock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            nudEndBlock.Value = new decimal(new int[] { 511, 0, 0, 0 });
            // 
            // nudStartBlock
            // 
            nudStartBlock.Location = new System.Drawing.Point(122, 104);
            nudStartBlock.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            nudStartBlock.Maximum = new decimal(new int[] { 511, 0, 0, 0 });
            nudStartBlock.Name = "nudStartBlock";
            nudStartBlock.Size = new System.Drawing.Size(71, 23);
            nudStartBlock.TabIndex = 18;
            nudStartBlock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblEndBlock
            // 
            lblEndBlock.AutoSize = true;
            lblEndBlock.Location = new System.Drawing.Point(216, 106);
            lblEndBlock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblEndBlock.Name = "lblEndBlock";
            lblEndBlock.Size = new System.Drawing.Size(62, 15);
            lblEndBlock.TabIndex = 17;
            lblEndBlock.Text = "End Block:";
            // 
            // lblStartBlock
            // 
            lblStartBlock.AutoSize = true;
            lblStartBlock.Location = new System.Drawing.Point(36, 106);
            lblStartBlock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStartBlock.Name = "lblStartBlock";
            lblStartBlock.Size = new System.Drawing.Size(66, 15);
            lblStartBlock.TabIndex = 16;
            lblStartBlock.Text = "Start Block:";
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new System.Drawing.Point(532, 66);
            btnBrowse.Margin = new System.Windows.Forms.Padding(4);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new System.Drawing.Size(52, 24);
            btnBrowse.TabIndex = 13;
            btnBrowse.Text = "....";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // txtInputOutputFile
            // 
            txtInputOutputFile.AllowDrop = true;
            txtInputOutputFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtInputOutputFile.Location = new System.Drawing.Point(120, 67);
            txtInputOutputFile.Margin = new System.Windows.Forms.Padding(4);
            txtInputOutputFile.MaxLength = 25;
            txtInputOutputFile.Name = "txtInputOutputFile";
            txtInputOutputFile.Size = new System.Drawing.Size(405, 23);
            txtInputOutputFile.TabIndex = 12;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 70);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(102, 15);
            label1.TabIndex = 11;
            label1.Text = "Input/Output File:";
            // 
            // cboSysconProcess
            // 
            cboSysconProcess.DataSource = sysconProcessBindingSource;
            cboSysconProcess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboSysconProcess.FormattingEnabled = true;
            cboSysconProcess.Location = new System.Drawing.Point(120, 32);
            cboSysconProcess.Margin = new System.Windows.Forms.Padding(4);
            cboSysconProcess.Name = "cboSysconProcess";
            cboSysconProcess.Size = new System.Drawing.Size(465, 23);
            cboSysconProcess.TabIndex = 3;
            cboSysconProcess.SelectedIndexChanged += cboSysconProcess_SelectedIndexChanged;
            // 
            // lblSysconProcessType
            // 
            lblSysconProcessType.AutoSize = true;
            lblSysconProcessType.Location = new System.Drawing.Point(6, 37);
            lblSysconProcessType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSysconProcessType.Name = "lblSysconProcessType";
            lblSysconProcessType.Size = new System.Drawing.Size(90, 15);
            lblSysconProcessType.TabIndex = 2;
            lblSysconProcessType.Text = "Syscon Process:";
            // 
            // txtLog
            // 
            txtLog.Location = new System.Drawing.Point(10, 22);
            txtLog.Margin = new System.Windows.Forms.Padding(4);
            txtLog.MaxLength = 0;
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtLog.Size = new System.Drawing.Size(579, 308);
            txtLog.TabIndex = 9;
            // 
            // btnStart
            // 
            btnStart.Location = new System.Drawing.Point(500, 23);
            btnStart.Margin = new System.Windows.Forms.Padding(4);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(88, 26);
            btnStart.TabIndex = 5;
            btnStart.Text = "&Start ";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // mnuMain
            // 
            mnuMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuDevices, mnuOptions, mnuHelp });
            mnuMain.Location = new System.Drawing.Point(0, 0);
            mnuMain.Name = "mnuMain";
            mnuMain.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            mnuMain.Size = new System.Drawing.Size(616, 24);
            mnuMain.TabIndex = 7;
            mnuMain.Text = "menuStrip1";
            // 
            // mnuDevices
            // 
            mnuDevices.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuResetPS4SysconFlasher, toolStripMenuItem1, mnuExit });
            mnuDevices.Name = "mnuDevices";
            mnuDevices.Size = new System.Drawing.Size(59, 20);
            mnuDevices.Text = "&Devices";
            // 
            // mnuResetPS4SysconFlasher
            // 
            mnuResetPS4SysconFlasher.Name = "mnuResetPS4SysconFlasher";
            mnuResetPS4SysconFlasher.Size = new System.Drawing.Size(204, 22);
            mnuResetPS4SysconFlasher.Text = "&Reset PS4 Syscon Flasher";
            mnuResetPS4SysconFlasher.Click += mnuResetPS4SysconFlasher_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(201, 6);
            // 
            // mnuExit
            // 
            mnuExit.Name = "mnuExit";
            mnuExit.Size = new System.Drawing.Size(204, 22);
            mnuExit.Text = "E&xit";
            mnuExit.Click += mnuExit_Click;
            // 
            // mnuOptions
            // 
            mnuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { mnuEnableAdvancedOptions, mnuEnableAutoDebugMode });
            mnuOptions.Name = "mnuOptions";
            mnuOptions.Size = new System.Drawing.Size(61, 20);
            mnuOptions.Text = "&Options";
            // 
            // mnuEnableAdvancedOptions
            // 
            mnuEnableAdvancedOptions.Name = "mnuEnableAdvancedOptions";
            mnuEnableAdvancedOptions.Size = new System.Drawing.Size(210, 22);
            mnuEnableAdvancedOptions.Text = "Enable &Advanced Options";
            mnuEnableAdvancedOptions.Click += mnuEnableAdvancedOptions_Click;
            // 
            // mnuEnableAutoDebugMode
            // 
            mnuEnableAutoDebugMode.Name = "mnuEnableAutoDebugMode";
            mnuEnableAutoDebugMode.Size = new System.Drawing.Size(210, 22);
            mnuEnableAutoDebugMode.Text = "Enable Auto &Debug Mode";
            mnuEnableAutoDebugMode.Click += mnuEnableAutoDebugMode_Click;
            // 
            // mnuHelp
            // 
            mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { helpToolStripMenuItem1, toolStripMenuItem2, mnuAbout });
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new System.Drawing.Size(44, 20);
            mnuHelp.Text = "&Help";
            // 
            // helpToolStripMenuItem1
            // 
            helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            helpToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            helpToolStripMenuItem1.Text = "&Help";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(104, 6);
            // 
            // mnuAbout
            // 
            mnuAbout.Name = "mnuAbout";
            mnuAbout.Size = new System.Drawing.Size(107, 22);
            mnuAbout.Text = "&About";
            mnuAbout.Click += mnuAbout_Click;
            // 
            // grbProcess
            // 
            grbProcess.Controls.Add(btnStart);
            grbProcess.Controls.Add(prbProgress);
            grbProcess.Controls.Add(lblProgress);
            grbProcess.Location = new System.Drawing.Point(10, 196);
            grbProcess.Margin = new System.Windows.Forms.Padding(4);
            grbProcess.Name = "grbProcess";
            grbProcess.Padding = new System.Windows.Forms.Padding(4);
            grbProcess.Size = new System.Drawing.Size(595, 64);
            grbProcess.TabIndex = 9;
            grbProcess.TabStop = false;
            // 
            // prbProgress
            // 
            prbProgress.Location = new System.Drawing.Point(91, 23);
            prbProgress.Margin = new System.Windows.Forms.Padding(4);
            prbProgress.Name = "prbProgress";
            prbProgress.Size = new System.Drawing.Size(384, 26);
            prbProgress.TabIndex = 4;
            // 
            // lblProgress
            // 
            lblProgress.AutoSize = true;
            lblProgress.Location = new System.Drawing.Point(17, 28);
            lblProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new System.Drawing.Size(55, 15);
            lblProgress.TabIndex = 3;
            lblProgress.Text = "Progress:";
            // 
            // grbLog
            // 
            grbLog.Controls.Add(txtLog);
            grbLog.Location = new System.Drawing.Point(10, 268);
            grbLog.Margin = new System.Windows.Forms.Padding(4);
            grbLog.Name = "grbLog";
            grbLog.Padding = new System.Windows.Forms.Padding(4);
            grbLog.Size = new System.Drawing.Size(595, 337);
            grbLog.TabIndex = 10;
            grbLog.TabStop = false;
            grbLog.Text = "Process Log";
            // 
            // dlgSaveFile
            // 
            dlgSaveFile.DefaultExt = "bin";
            dlgSaveFile.Filter = "PS4 Syscon FW Dump (*.bin)|*bin|All Files (*.*)|*.*";
            dlgSaveFile.Title = "Please Select Where You Want To Save Syscon FW Dump File";
            // 
            // dlgOpenFile
            // 
            dlgOpenFile.Filter = "PS4 Syscon FW Dump (*.bin)|*bin|All Files (*.*)|*.*";
            dlgOpenFile.Title = "Please Select Syscon FW Dump File";
            // 
            // frmPS4SysconTools
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(616, 629);
            Controls.Add(grbLog);
            Controls.Add(grbProcess);
            Controls.Add(staMain);
            Controls.Add(grbSysconProcess);
            Controls.Add(mnuMain);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "frmPS4SysconTools";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "PS4 Syscon Tools v2.2.0  by Abkarino & EgyCnq";
            Load += frmPS4SysconTools_Load;
            staMain.ResumeLayout(false);
            staMain.PerformLayout();
            grbSysconProcess.ResumeLayout(false);
            grbSysconProcess.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudNoOfDumps).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudEndBlock).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudStartBlock).EndInit();
            ((System.ComponentModel.ISupportInitialize)sysconProcessBindingSource).EndInit();
            mnuMain.ResumeLayout(false);
            mnuMain.PerformLayout();
            grbProcess.ResumeLayout(false);
            grbProcess.PerformLayout();
            grbLog.ResumeLayout(false);
            grbLog.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.StatusStrip staMain;
        private System.Windows.Forms.GroupBox grbSysconProcess;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ComboBox cboSysconProcess;
        private System.Windows.Forms.Label lblSysconProcessType;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuDevices;
        private System.Windows.Forms.ToolStripMenuItem mnuResetPS4SysconFlasher;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.GroupBox grbProcess;
        private System.Windows.Forms.ProgressBar prbProgress;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.GroupBox grbLog;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtInputOutputFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.ToolStripStatusLabel tslSysconTool;
        private System.Windows.Forms.ToolStripStatusLabel tslSysconToolValue;
        private System.Windows.Forms.NumericUpDown nudNoOfDumps;
        private System.Windows.Forms.Label lblNoOfDumps;
        private System.Windows.Forms.NumericUpDown nudEndBlock;
        private System.Windows.Forms.NumericUpDown nudStartBlock;
        private System.Windows.Forms.Label lblEndBlock;
        private System.Windows.Forms.Label lblStartBlock;
        private System.Windows.Forms.CheckBox chkEnableDebugMode;
        private System.Windows.Forms.CheckBox chkAutoVerify;
        private System.Windows.Forms.ToolStripMenuItem mnuOptions;
        private System.Windows.Forms.ToolStripMenuItem mnuEnableAdvancedOptions;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.BindingSource sysconProcessBindingSource;
        private System.Windows.Forms.ToolStripMenuItem mnuEnableAutoDebugMode;
        private System.Windows.Forms.ToolStripStatusLabel tslVersion;
        private System.Windows.Forms.ToolStripStatusLabel tslVersionValue;
        private System.ComponentModel.BackgroundWorker deviceWatcher;
    }
}


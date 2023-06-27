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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPS4SysconTools));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tslSysconToolPort = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslSysconToolPortValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslSysconToolDebugMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslSysconToolDebugModeValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.grbSysconProcess = new System.Windows.Forms.GroupBox();
            this.nudEndBlock = new System.Windows.Forms.NumericUpDown();
            this.nudStartBlock = new System.Windows.Forms.NumericUpDown();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtInputOutputFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblEndBlock = new System.Windows.Forms.Label();
            this.lblStartBlock = new System.Windows.Forms.Label();
            this.cboSysconProcess = new System.Windows.Forms.ComboBox();
            this.lblSysconProcessType = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.grbDeviceConnection = new System.Windows.Forms.GroupBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.cboCOMPorts = new System.Windows.Forms.ComboBox();
            this.lblCOMPort = new System.Windows.Forms.Label();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuDevices = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuScanCOMPort = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.grbProcess = new System.Windows.Forms.GroupBox();
            this.prbProgress = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.grbLog = new System.Windows.Forms.GroupBox();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1.SuspendLayout();
            this.grbSysconProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEndBlock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartBlock)).BeginInit();
            this.grbDeviceConnection.SuspendLayout();
            this.mnuMain.SuspendLayout();
            this.grbProcess.SuspendLayout();
            this.grbLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslSysconToolPort,
            this.tslSysconToolPortValue,
            this.tslSysconToolDebugMode,
            this.tslSysconToolDebugModeValue});
            this.statusStrip1.Location = new System.Drawing.Point(0, 610);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(704, 25);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "staMain";
            // 
            // tslSysconToolPort
            // 
            this.tslSysconToolPort.ForeColor = System.Drawing.Color.Blue;
            this.tslSysconToolPort.Name = "tslSysconToolPort";
            this.tslSysconToolPort.Size = new System.Drawing.Size(120, 20);
            this.tslSysconToolPort.Text = "Syscon Tool Port:";
            // 
            // tslSysconToolPortValue
            // 
            this.tslSysconToolPortValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.tslSysconToolPortValue.Name = "tslSysconToolPortValue";
            this.tslSysconToolPortValue.Size = new System.Drawing.Size(0, 20);
            // 
            // tslSysconToolDebugMode
            // 
            this.tslSysconToolDebugMode.ForeColor = System.Drawing.Color.Blue;
            this.tslSysconToolDebugMode.Margin = new System.Windows.Forms.Padding(100, 3, 0, 2);
            this.tslSysconToolDebugMode.Name = "tslSysconToolDebugMode";
            this.tslSysconToolDebugMode.Size = new System.Drawing.Size(100, 20);
            this.tslSysconToolDebugMode.Text = "Debug Mode:";
            // 
            // tslSysconToolDebugModeValue
            // 
            this.tslSysconToolDebugModeValue.Name = "tslSysconToolDebugModeValue";
            this.tslSysconToolDebugModeValue.Size = new System.Drawing.Size(0, 20);
            // 
            // grbSysconProcess
            // 
            this.grbSysconProcess.Controls.Add(this.nudEndBlock);
            this.grbSysconProcess.Controls.Add(this.nudStartBlock);
            this.grbSysconProcess.Controls.Add(this.btnBrowse);
            this.grbSysconProcess.Controls.Add(this.txtInputOutputFile);
            this.grbSysconProcess.Controls.Add(this.label1);
            this.grbSysconProcess.Controls.Add(this.lblEndBlock);
            this.grbSysconProcess.Controls.Add(this.lblStartBlock);
            this.grbSysconProcess.Controls.Add(this.cboSysconProcess);
            this.grbSysconProcess.Controls.Add(this.lblSysconProcessType);
            this.grbSysconProcess.Location = new System.Drawing.Point(10, 101);
            this.grbSysconProcess.Margin = new System.Windows.Forms.Padding(4);
            this.grbSysconProcess.Name = "grbSysconProcess";
            this.grbSysconProcess.Padding = new System.Windows.Forms.Padding(4);
            this.grbSysconProcess.Size = new System.Drawing.Size(683, 148);
            this.grbSysconProcess.TabIndex = 6;
            this.grbSysconProcess.TabStop = false;
            this.grbSysconProcess.Text = "PS4 Syscon Process";
            // 
            // nudEndBlock
            // 
            this.nudEndBlock.Location = new System.Drawing.Point(439, 75);
            this.nudEndBlock.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
            this.nudEndBlock.Name = "nudEndBlock";
            this.nudEndBlock.Size = new System.Drawing.Size(81, 22);
            this.nudEndBlock.TabIndex = 15;
            this.nudEndBlock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudEndBlock.Value = new decimal(new int[] {
            511,
            0,
            0,
            0});
            // 
            // nudStartBlock
            // 
            this.nudStartBlock.Location = new System.Drawing.Point(137, 75);
            this.nudStartBlock.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
            this.nudStartBlock.Name = "nudStartBlock";
            this.nudStartBlock.Size = new System.Drawing.Size(81, 22);
            this.nudStartBlock.TabIndex = 14;
            this.nudStartBlock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(594, 108);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(81, 28);
            this.btnBrowse.TabIndex = 13;
            this.btnBrowse.Text = "....";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtInputOutputFile
            // 
            this.txtInputOutputFile.Location = new System.Drawing.Point(137, 111);
            this.txtInputOutputFile.Margin = new System.Windows.Forms.Padding(4);
            this.txtInputOutputFile.MaxLength = 25;
            this.txtInputOutputFile.Name = "txtInputOutputFile";
            this.txtInputOutputFile.Size = new System.Drawing.Size(452, 22);
            this.txtInputOutputFile.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 114);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "Input/Output File:";
            // 
            // lblEndBlock
            // 
            this.lblEndBlock.AutoSize = true;
            this.lblEndBlock.Location = new System.Drawing.Point(335, 77);
            this.lblEndBlock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEndBlock.Name = "lblEndBlock";
            this.lblEndBlock.Size = new System.Drawing.Size(75, 17);
            this.lblEndBlock.TabIndex = 9;
            this.lblEndBlock.Text = "End Block:";
            // 
            // lblStartBlock
            // 
            this.lblStartBlock.AutoSize = true;
            this.lblStartBlock.Location = new System.Drawing.Point(22, 77);
            this.lblStartBlock.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStartBlock.Name = "lblStartBlock";
            this.lblStartBlock.Size = new System.Drawing.Size(80, 17);
            this.lblStartBlock.TabIndex = 7;
            this.lblStartBlock.Text = "Start Block:";
            // 
            // cboSysconProcess
            // 
            this.cboSysconProcess.FormattingEnabled = true;
            this.cboSysconProcess.Items.AddRange(new object[] {
            "Dump Full Syscon Flash",
            "Dump Partial Syscon Flash",
            "Erase Full Syscon Flash (Danger)",
            "Erase Full Syscon Flash Expect Boot0 Block (Safe)",
            "Erase Partial Syscon Flash",
            "Write Full Syscon Flash",
            "Write Partial Syscon Flash",
            "Write Syscon NVS/SNVS Only"});
            this.cboSysconProcess.Location = new System.Drawing.Point(137, 35);
            this.cboSysconProcess.Margin = new System.Windows.Forms.Padding(4);
            this.cboSysconProcess.Name = "cboSysconProcess";
            this.cboSysconProcess.Size = new System.Drawing.Size(517, 24);
            this.cboSysconProcess.TabIndex = 3;
            this.cboSysconProcess.SelectedIndexChanged += new System.EventHandler(this.cboTransactionsType_SelectedIndexChanged);
            // 
            // lblSysconProcessType
            // 
            this.lblSysconProcessType.AutoSize = true;
            this.lblSysconProcessType.Location = new System.Drawing.Point(7, 39);
            this.lblSysconProcessType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSysconProcessType.Name = "lblSysconProcessType";
            this.lblSysconProcessType.Size = new System.Drawing.Size(113, 17);
            this.lblSysconProcessType.TabIndex = 2;
            this.lblSysconProcessType.Text = "Syscon Process:";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(10, 23);
            this.txtLog.Margin = new System.Windows.Forms.Padding(4);
            this.txtLog.MaxLength = 0;
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(661, 244);
            this.txtLog.TabIndex = 9;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(572, 24);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 28);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "&Start ";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // grbDeviceConnection
            // 
            this.grbDeviceConnection.Controls.Add(this.btnConnect);
            this.grbDeviceConnection.Controls.Add(this.btnScan);
            this.grbDeviceConnection.Controls.Add(this.cboCOMPorts);
            this.grbDeviceConnection.Controls.Add(this.lblCOMPort);
            this.grbDeviceConnection.Location = new System.Drawing.Point(10, 33);
            this.grbDeviceConnection.Margin = new System.Windows.Forms.Padding(4);
            this.grbDeviceConnection.Name = "grbDeviceConnection";
            this.grbDeviceConnection.Padding = new System.Windows.Forms.Padding(4);
            this.grbDeviceConnection.Size = new System.Drawing.Size(683, 69);
            this.grbDeviceConnection.TabIndex = 5;
            this.grbDeviceConnection.TabStop = false;
            this.grbDeviceConnection.Text = "PS4 Syscon Tool Device Connection";
            this.grbDeviceConnection.Enter += new System.EventHandler(this.grbDeviceConnection_Enter);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(439, 25);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 28);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "&Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(575, 23);
            this.btnScan.Margin = new System.Windows.Forms.Padding(4);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(100, 28);
            this.btnScan.TabIndex = 2;
            this.btnScan.Text = "&Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // cboCOMPorts
            // 
            this.cboCOMPorts.FormattingEnabled = true;
            this.cboCOMPorts.Location = new System.Drawing.Point(169, 28);
            this.cboCOMPorts.Margin = new System.Windows.Forms.Padding(4);
            this.cboCOMPorts.Name = "cboCOMPorts";
            this.cboCOMPorts.Size = new System.Drawing.Size(160, 24);
            this.cboCOMPorts.TabIndex = 1;
            // 
            // lblCOMPort
            // 
            this.lblCOMPort.AutoSize = true;
            this.lblCOMPort.Location = new System.Drawing.Point(11, 31);
            this.lblCOMPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCOMPort.Name = "lblCOMPort";
            this.lblCOMPort.Size = new System.Drawing.Size(150, 17);
            this.lblCOMPort.TabIndex = 0;
            this.lblCOMPort.Text = "PS4 Syscon Tool Port:";
            // 
            // mnuMain
            // 
            this.mnuMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDevices});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.mnuMain.Size = new System.Drawing.Size(704, 28);
            this.mnuMain.TabIndex = 7;
            this.mnuMain.Text = "menuStrip1";
            // 
            // mnuDevices
            // 
            this.mnuDevices.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuScanCOMPort,
            this.toolStripMenuItem1,
            this.mnuExit});
            this.mnuDevices.Name = "mnuDevices";
            this.mnuDevices.Size = new System.Drawing.Size(72, 24);
            this.mnuDevices.Text = "&Devices";
            // 
            // mnuScanCOMPort
            // 
            this.mnuScanCOMPort.Name = "mnuScanCOMPort";
            this.mnuScanCOMPort.Size = new System.Drawing.Size(188, 26);
            this.mnuScanCOMPort.Text = "&Scan COM Ports";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(185, 6);
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(188, 26);
            this.mnuExit.Text = "E&xit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // grbProcess
            // 
            this.grbProcess.Controls.Add(this.btnStart);
            this.grbProcess.Controls.Add(this.prbProgress);
            this.grbProcess.Controls.Add(this.lblProgress);
            this.grbProcess.Location = new System.Drawing.Point(13, 257);
            this.grbProcess.Margin = new System.Windows.Forms.Padding(4);
            this.grbProcess.Name = "grbProcess";
            this.grbProcess.Padding = new System.Windows.Forms.Padding(4);
            this.grbProcess.Size = new System.Drawing.Size(680, 69);
            this.grbProcess.TabIndex = 9;
            this.grbProcess.TabStop = false;
            // 
            // prbProgress
            // 
            this.prbProgress.Location = new System.Drawing.Point(104, 24);
            this.prbProgress.Margin = new System.Windows.Forms.Padding(4);
            this.prbProgress.Name = "prbProgress";
            this.prbProgress.Size = new System.Drawing.Size(439, 28);
            this.prbProgress.TabIndex = 4;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(18, 30);
            this.lblProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(69, 17);
            this.lblProgress.TabIndex = 3;
            this.lblProgress.Text = "Progress:";
            // 
            // grbLog
            // 
            this.grbLog.Controls.Add(this.txtLog);
            this.grbLog.Location = new System.Drawing.Point(13, 334);
            this.grbLog.Margin = new System.Windows.Forms.Padding(4);
            this.grbLog.Name = "grbLog";
            this.grbLog.Padding = new System.Windows.Forms.Padding(4);
            this.grbLog.Size = new System.Drawing.Size(680, 275);
            this.grbLog.TabIndex = 10;
            this.grbLog.TabStop = false;
            this.grbLog.Text = "Process Log";
            // 
            // dlgSaveFile
            // 
            this.dlgSaveFile.DefaultExt = "bin";
            this.dlgSaveFile.Filter = "PS4 Syscon FW Dump (*.bin)|*bin|All Files (*.*)|*.*";
            this.dlgSaveFile.Title = "Please Select Where You Want To Save Syscon FW Dump File";
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.FileName = "dlgOpenFile";
            this.dlgOpenFile.Filter = "PS4 Syscon FW Dump (*.bin)|*bin|All Files (*.*)|*.*";
            this.dlgOpenFile.Title = "Please Select Syscon FW Dump File";
            // 
            // frmPS4SysconTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 635);
            this.Controls.Add(this.grbLog);
            this.Controls.Add(this.grbProcess);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.grbSysconProcess);
            this.Controls.Add(this.grbDeviceConnection);
            this.Controls.Add(this.mnuMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmPS4SysconTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PS4 Syscon Tools v1.00 by Abkarino & EgyCnq";
            this.Load += new System.EventHandler(this.frmPS4SysconTools_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.grbSysconProcess.ResumeLayout(false);
            this.grbSysconProcess.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEndBlock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartBlock)).EndInit();
            this.grbDeviceConnection.ResumeLayout(false);
            this.grbDeviceConnection.PerformLayout();
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.grbProcess.ResumeLayout(false);
            this.grbProcess.PerformLayout();
            this.grbLog.ResumeLayout(false);
            this.grbLog.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.GroupBox grbSysconProcess;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblStartBlock;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ComboBox cboSysconProcess;
        private System.Windows.Forms.Label lblSysconProcessType;
        private System.Windows.Forms.GroupBox grbDeviceConnection;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.ComboBox cboCOMPorts;
        private System.Windows.Forms.Label lblCOMPort;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem mnuDevices;
        private System.Windows.Forms.ToolStripMenuItem mnuScanCOMPort;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.GroupBox grbProcess;
        private System.Windows.Forms.ProgressBar prbProgress;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.GroupBox grbLog;
        private System.Windows.Forms.Label lblEndBlock;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtInputOutputFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.NumericUpDown nudStartBlock;
        private System.Windows.Forms.NumericUpDown nudEndBlock;
        private System.Windows.Forms.ToolStripStatusLabel tslSysconToolPort;
        private System.Windows.Forms.ToolStripStatusLabel tslSysconToolPortValue;
        private System.Windows.Forms.ToolStripStatusLabel tslSysconToolDebugMode;
        private System.Windows.Forms.ToolStripStatusLabel tslSysconToolDebugModeValue;
    }
}


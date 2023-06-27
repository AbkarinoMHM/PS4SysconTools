using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace PS4_Syscon_Tools
{
    public partial class frmPS4SysconTools : Form
    {
        PS4SysconTool ps4SysconTool;
        string version;
        string freememory;
        string sysconFWFilePath;
        short startBlock;
        short endBlock;
        short noOfBlocks;
        bool debugMode;
        BackgroundWorker bgWorker;
        int iRet = 0;
        PS4SysconTool.SYSCON_PROCESS sysconProcess = PS4SysconTool.SYSCON_PROCESS.NONE;

        delegate void SetTextCallback(string text);
        delegate void SetProgressValueCallback(int value);

        public frmPS4SysconTools()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void frmPS4SysconTools_Load(object sender, EventArgs e)
        {

            ps4SysconTool = new PS4SysconTool();
            ps4SysconTool.UpdateProcessEvent += HandleUpdateProcessEvent;

            grbSysconProcess.Enabled = false;
            grbProcess.Enabled = false;

            ScanCOMPorts();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void grbDeviceConnection_Enter(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            ScanCOMPorts();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            if (!debugMode)
            {
                iRet = ps4SysconTool.PS4SysconToolConnect(cboCOMPorts.SelectedItem.ToString(),out version,out freememory, out debugMode);
                if (iRet == 0)
                {
                    tslSysconToolPortValue.Text = "Connected";
                    tslSysconToolDebugModeValue.Text = (debugMode ? "Enabled" : "Disabled");
                    MessageBox.Show("PS4 Syscon Tool Connected Successfully!!", "Done!!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    grbSysconProcess.Enabled = true;
                    grbProcess.Enabled = true;
                    cboSysconProcess.SelectedIndex = 0;
                    btnConnect.Text = "Disconnect";
                }
                else
                {
                    tslSysconToolPortValue.Text = "Disconnected";
                    tslSysconToolDebugModeValue.Text = "Unknown";
                    MessageBox.Show("Error Connecting To PS4 Syscon Tool!!", "Connection Error!!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    grbSysconProcess.Enabled = false;
                    grbProcess.Enabled = false;
                    cboSysconProcess.SelectedIndex = 0;
                    btnConnect.Text = "Connect";
                }
            }
            else
            {
                iRet = ps4SysconTool.PS4SysconToolDisconnect(cboCOMPorts.SelectedItem.ToString(), out debugMode);
                if (iRet == 0)
                {
                    grbSysconProcess.Enabled = false;
                    grbProcess.Enabled = true;
                    cboSysconProcess.SelectedIndex = 0;
                    btnConnect.Text = "Connect";
                }
            }
        }

        private void cboTransactionsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            sysconProcess = (PS4SysconTool.SYSCON_PROCESS)cboSysconProcess.SelectedIndex;
            switch (sysconProcess)
            {
                case PS4SysconTool.SYSCON_PROCESS.DUMP_FULL:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = 0;
                    nudEndBlock.Value = 511;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled  = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.DUMP_PARTIAL:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = 0;
                    nudEndBlock.Value = 511;
                    nudStartBlock.Enabled = true;
                    nudEndBlock.Enabled = true;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.ERASE_FULL:
                    btnBrowse.Enabled = false;
                    txtInputOutputFile.Enabled = false;
                    nudStartBlock.Value = 0;
                    nudEndBlock.Value = 511;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.ERASE_EXCEPT_BOOT0:
                    btnBrowse.Enabled = false;
                    txtInputOutputFile.Enabled = false;
                    nudStartBlock.Value = 2;
                    nudEndBlock.Value = 511;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.ERASE_PARTIAL:
                    btnBrowse.Enabled = false;
                    txtInputOutputFile.Enabled = false;
                    nudStartBlock.Value = 2;
                    nudEndBlock.Value = 511;
                    nudStartBlock.Enabled = true;
                    nudEndBlock.Enabled = true;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.WRITE_FULL:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = 0;
                    nudEndBlock.Value = 511;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.WRITE_PARTIAL:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = 2;
                    nudEndBlock.Value = 511;
                    nudStartBlock.Enabled = true;
                    nudEndBlock.Enabled = true;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.WRITE_NVS_SNVS:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = 384;  // NVS-SNVS Start address 0x60000 block 384
                    nudEndBlock.Value = 511;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
          
            if (sysconProcess == PS4SysconTool.SYSCON_PROCESS.DUMP_FULL || sysconProcess == PS4SysconTool.SYSCON_PROCESS.DUMP_PARTIAL)
            {
                if (dlgSaveFile.ShowDialog() == DialogResult.OK) {
                    txtInputOutputFile.Text = dlgSaveFile.FileName;
                }
            }
            else if (sysconProcess != PS4SysconTool.SYSCON_PROCESS.NONE)
            {
                if (dlgOpenFile.ShowDialog() == DialogResult.OK) {
                    txtInputOutputFile.Text = dlgOpenFile.FileName;
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            
            grbDeviceConnection.Enabled = false;
            grbProcess.Enabled = false;
            btnStart.Enabled = false;

            prbProgress.Minimum = 0;
            txtLog.Clear();

            try
            {
                switch (sysconProcess)
                {
                    case PS4SysconTool.SYSCON_PROCESS.DUMP_FULL:
                        sysconFWFilePath = txtInputOutputFile.Text;
                        prbProgress.Maximum = (int)PS4SysconTool.SYSCON_FULL_DUMP_SIZE;

                        iRet = ps4SysconTool.PS4SysconToolFullDump(sysconFWFilePath);

                        break;
                    case PS4SysconTool.SYSCON_PROCESS.DUMP_PARTIAL:
                        sysconFWFilePath = txtInputOutputFile.Text;
                        startBlock = (short)nudStartBlock.Value;
                        endBlock = (short)nudEndBlock.Value;
                        noOfBlocks = (short)(endBlock - startBlock + 1);
                        prbProgress.Maximum = noOfBlocks;

                        iRet = ps4SysconTool.PS4SysconToolPartialDump(sysconFWFilePath, startBlock, endBlock);

                        break;
                    case PS4SysconTool.SYSCON_PROCESS.ERASE_PARTIAL:
                        sysconFWFilePath = txtInputOutputFile.Text;
                        startBlock = (short)nudStartBlock.Value;
                        endBlock = (short)nudEndBlock.Value;
                        noOfBlocks = (short)(endBlock - startBlock + 1);
                        prbProgress.Maximum = noOfBlocks;

                        iRet = ps4SysconTool.PS4SysconToolPartialErase(startBlock, endBlock);
                        break;
                    case PS4SysconTool.SYSCON_PROCESS.WRITE_FULL:
                        sysconFWFilePath = txtInputOutputFile.Text;
                        prbProgress.Maximum = (int)PS4SysconTool.SYSCON_FULL_DUMP_SIZE;

                        iRet = ps4SysconTool.PS4SysconToolFullWrite(sysconFWFilePath);
                        break;
                    case PS4SysconTool.SYSCON_PROCESS.WRITE_PARTIAL:
                        sysconFWFilePath = txtInputOutputFile.Text;
                        startBlock = (short)nudStartBlock.Value;
                        endBlock = (short)nudEndBlock.Value;
                        noOfBlocks = (short)(endBlock - startBlock + 1);
                        prbProgress.Maximum = noOfBlocks;

                        iRet = ps4SysconTool.PS4SysconToolPartialWrite(sysconFWFilePath, startBlock, endBlock);
                        break;
                    default:
                        break;
                }

                if (iRet == 0) {
                    MessageBox.Show("Process Done Successfully!!!", "Done!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }              
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error: {0}.", ex.Message), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                grbDeviceConnection.Enabled = true;
                grbProcess.Enabled = true;
                btnStart.Enabled = true;
            }
        }

        private void ScanCOMPorts() {
            try
            {
                // Get a list of serial port names.
                string[] strCOMPorts = SerialPort.GetPortNames();

                if ((strCOMPorts != null) && (strCOMPorts.Count() > 0))
                {
                    cboCOMPorts.Items.Clear();
                    cboCOMPorts.Items.AddRange(strCOMPorts);
                    cboCOMPorts.Sorted = true;
                    cboCOMPorts.SelectedIndex = 0;
                    btnConnect.Enabled = true;
                    //cboTransactionsType.SelectedIndex = 0;
                    //grbSysconProcess.Enabled = true;
                    //btnStart.Enabled = true;
                }
                else
                {
                    btnStart.Enabled = false;
                    btnConnect.Enabled = false;

                    //cboTransactionsType.SelectedIndex = -1;
                    cboCOMPorts.SelectedIndex = -1;
                    //grbSysconProcess.Enabled = false;
                    MessageBox.Show("Error no COM Ports available", "Error Scanning COM Port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(String.Format("Error during scanning COM Ports.\nError: {0}", ex.Message), "Error Scanning COM Port", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetText(string text)
        {
            if (this.txtLog.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(SetText), new object[] { text });
            }
            else
            {
                txtLog.SuspendLayout();
                txtLog.Text += text + Environment.NewLine;
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
                txtLog.ResumeLayout();
            }
        }

        private void SetProgressValue(int value) {
            if (this.prbProgress.InvokeRequired)
            {
                this.BeginInvoke(new Action<int>(SetProgressValue), new object[] { value });
            }
            else
            {
                prbProgress.SuspendLayout();
                //prbProgress.Text += text;
                prbProgress.Value = value;
                prbProgress.ResumeLayout();
            }
        }

        void HandleUpdateProcessEvent(object sender, UpdateProcessEventArgs e)
        {
            string message = e.Message;
            int value = e.Value;

            if (!string.IsNullOrEmpty(message))
            {
                SetText(message);
            }

            if (value >= 0) {
                SetProgressValue(value);
            }

            Application.DoEvents();
        }
    }
}

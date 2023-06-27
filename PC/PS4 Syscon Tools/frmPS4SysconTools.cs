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
        const int dangBlock = 4;
        string version;
        string freememory;
        string sysconFWFilePath;
        short startBlock;
        short endBlock;
        short noOfBlocks;
        short noOfDumps;
        bool debugMode;
        bool isFilesAreIdentical;
        bool enableAutoErase;
        bool enableDebugMode;
        bool processCancelled;
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
            prbProgress.Value = 0;
            txtLog.Clear();

            sysconProcess = (PS4SysconTool.SYSCON_PROCESS)cboSysconProcess.SelectedIndex;

            switch (sysconProcess)
            {
                case PS4SysconTool.SYSCON_PROCESS.DUMP_FULL:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_FLASH_START_BLOCK;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled  = false;
                    nudNoOfDumps.Enabled = true;
                    chkAutoErase.Checked = false;
                    chkAutoErase.Enabled = false;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.DUMP_PARTIAL:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_FLASH_START_BLOCK;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = true;
                    nudEndBlock.Enabled = true;
                    nudNoOfDumps.Enabled = true;
                    chkAutoErase.Checked = false;
                    chkAutoErase.Enabled = false;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.DUMP_NVS_SNVS:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_NVS_SNVS_START_BLOCK;  // NVS-SNVS Start address 0x60000 block 384
                    nudEndBlock.Value = PS4SysconTool.SYSCON_NVS_SNVS_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    nudNoOfDumps.Enabled = true;
                    chkAutoErase.Checked = false;
                    chkAutoErase.Enabled = false;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.ERASE_FULL:
                    btnBrowse.Enabled = false;
                    txtInputOutputFile.Enabled = false;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_FLASH_START_BLOCK;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    chkAutoErase.Checked = false;
                    chkAutoErase.Enabled = false;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.ERASE_EXCEPT_BOOT0:
                    btnBrowse.Enabled = false;
                    txtInputOutputFile.Enabled = false;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_BOOT0_BLOCKS;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    chkAutoErase.Checked = false;
                    chkAutoErase.Enabled = false;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.ERASE_PARTIAL:
                    btnBrowse.Enabled = false;
                    txtInputOutputFile.Enabled = false;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_BOOT0_BLOCKS;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = true;
                    nudEndBlock.Enabled = true;
                    nudNoOfDumps.Enabled = false;
                    chkAutoErase.Checked = false;
                    chkAutoErase.Enabled = false;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.ERASE_NVS_SNVS:
                    btnBrowse.Enabled = false;
                    txtInputOutputFile.Enabled = false;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_NVS_SNVS_START_BLOCK;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_NVS_SNVS_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    chkAutoErase.Checked = false;
                    chkAutoErase.Enabled = false;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.WRITE_FULL:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_FLASH_START_BLOCK;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    chkAutoErase.Checked = true;
                    chkAutoErase.Enabled = true;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = true;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.WRITE_PARTIAL:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_FLASH_PARTIAL_START_BLOCK;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = true;
                    nudEndBlock.Enabled = true;
                    nudNoOfDumps.Enabled = false;
                    chkAutoErase.Checked = true;
                    chkAutoErase.Enabled = true;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.WRITE_NVS_SNVS:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_NVS_SNVS_START_BLOCK;  // NVS-SNVS Start address 0x60000 block 384
                    nudEndBlock.Value = PS4SysconTool.SYSCON_NVS_SNVS_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    chkAutoErase.Checked = true;
                    chkAutoErase.Enabled = true;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.ENABLE_DEBUG_MODE:
                    btnBrowse.Enabled = false;
                    txtInputOutputFile.Enabled = false;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_FLASH_START_BLOCK;  
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    chkAutoErase.Checked = true;
                    chkAutoErase.Enabled = false;
                    chkEnableDebugMode.Checked = true;
                    chkEnableDebugMode.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (sysconProcess == PS4SysconTool.SYSCON_PROCESS.DUMP_FULL || sysconProcess == PS4SysconTool.SYSCON_PROCESS.DUMP_PARTIAL || sysconProcess == PS4SysconTool.SYSCON_PROCESS.DUMP_NVS_SNVS)
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
            noOfDumps = 0;
            txtLog.Clear();

            try
            {
                switch (sysconProcess)
                {
                    case PS4SysconTool.SYSCON_PROCESS.DUMP_FULL:
                        sysconFWFilePath = txtInputOutputFile.Text;
                        prbProgress.Maximum = (int)PS4SysconTool.SYSCON_FLASH_SIZE;
                        noOfDumps = (short)nudNoOfDumps.Value;

                        if (noOfDumps > 1)
                        {
                            string sysconFWFile1 = Path.Combine(Path.GetDirectoryName(sysconFWFilePath),
                                String.Format("{0}-01{1}", Path.GetFileNameWithoutExtension(sysconFWFilePath),
                                    Path.GetExtension(sysconFWFilePath)));

                            string sysconFWFile2 = Path.Combine(Path.GetDirectoryName(sysconFWFilePath),
                                String.Format("{0}-02{1}", Path.GetFileNameWithoutExtension(sysconFWFilePath),
                                    Path.GetExtension(sysconFWFilePath)));

                            txtInputOutputFile.Text = sysconFWFile1;

                            iRet = ps4SysconTool.PS4SysconToolFullDump(sysconFWFile1);
                            if (iRet != 0)
                            {
                                break;
                            }

                            txtInputOutputFile.Text = sysconFWFile2;

                            iRet = ps4SysconTool.PS4SysconToolFullDump(sysconFWFile2);
                            if (iRet != 0)
                            {
                                break;
                            }

                            isFilesAreIdentical = Util.isFilesIdentical(sysconFWFile1, sysconFWFile2);
                        }
                        else
                        {
                            iRet = ps4SysconTool.PS4SysconToolFullDump(sysconFWFilePath);
                        }


                        break;
                    case PS4SysconTool.SYSCON_PROCESS.DUMP_PARTIAL:
                        sysconFWFilePath = txtInputOutputFile.Text;
                        startBlock = (short)nudStartBlock.Value;
                        endBlock = (short)nudEndBlock.Value;
                        noOfBlocks = (short)(endBlock - startBlock + 1);
                        prbProgress.Maximum = noOfBlocks;

                        iRet = ps4SysconTool.PS4SysconToolDump(sysconFWFilePath, startBlock, endBlock);
                        break;
                    case PS4SysconTool.SYSCON_PROCESS.DUMP_NVS_SNVS:
                        sysconFWFilePath = txtInputOutputFile.Text;
                        startBlock = (short)nudStartBlock.Value;
                        endBlock = (short)nudEndBlock.Value;
                        noOfBlocks = (short)(endBlock - startBlock + 1);
                        prbProgress.Maximum = noOfBlocks;

                        iRet = ps4SysconTool.PS4SysconToolNVSSNVSDump(sysconFWFilePath);
                        break;
                    case PS4SysconTool.SYSCON_PROCESS.ERASE_FULL:
                        if (nudStartBlock.Value < dangBlock)
                        {
                            DialogResult result = MessageBox.Show("You are about to erase/rewrite a dangerous area that can lead to bricking your Syscon Chip Do you want to continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.No)
                            {
                                processCancelled = true;
                                break;
                            }

                        }

                        prbProgress.Maximum = (int)PS4SysconTool.SYSCON_BLOCKS_NO;

                        iRet = ps4SysconTool.PS4SysconToolErase(PS4SysconTool.SYSCON_FLASH_START_BLOCK, PS4SysconTool.SYSCON_FLASH_END_BLOCK);
                        break;
                    case PS4SysconTool.SYSCON_PROCESS.ERASE_EXCEPT_BOOT0:
                        prbProgress.Maximum = (int)PS4SysconTool.SYSCON_NVS_SNVS_SIZE;

                        iRet = ps4SysconTool.PS4SysconToolErase(PS4SysconTool.SYSCON_NVS_SNVS_START_BLOCK, PS4SysconTool.SYSCON_NVS_SNVS_END_BLOCK);
                        break;
                    case PS4SysconTool.SYSCON_PROCESS.ERASE_PARTIAL:
                        if (nudStartBlock.Value < dangBlock)
                        {
                            DialogResult result = MessageBox.Show("You are about to erase/rewrite a dangerous area that can lead to bricking your Syscon Chip Do you want to continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.No)
                            {
                                processCancelled = true;
                                break;
                            }

                        }
                        startBlock = (short)nudStartBlock.Value;
                        endBlock = (short)nudEndBlock.Value;
                        noOfBlocks = (short)(endBlock - startBlock + 1);
                        prbProgress.Maximum = noOfBlocks;

                        iRet = ps4SysconTool.PS4SysconToolErase(startBlock, endBlock);
                        break;
                    case PS4SysconTool.SYSCON_PROCESS.WRITE_FULL:
                        if (nudStartBlock.Value < dangBlock)
                        {
                            DialogResult result = MessageBox.Show("You are about to erase/rewrite a dangerous area that can lead to bricking your Syscon Chip Do you want to continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.No)
                            {
                                processCancelled = true;
                                break;
                            }

                        }
                        sysconFWFilePath = txtInputOutputFile.Text;
                        enableAutoErase = chkAutoErase.Checked;
                        prbProgress.Maximum = (int)PS4SysconTool.SYSCON_BLOCKS_NO;

                        if (enableAutoErase)
                        {
                            iRet = ps4SysconTool.PS4SysconToolErase(PS4SysconTool.SYSCON_FLASH_START_BLOCK, PS4SysconTool.SYSCON_FLASH_END_BLOCK);
                        }

                        //prbProgress.Maximum = (int)PS4SysconTool.SYSCON_FLASH_SIZE;

                        iRet = ps4SysconTool.PS4SysconToolFullWrite(sysconFWFilePath);
                        break;
                    case PS4SysconTool.SYSCON_PROCESS.WRITE_PARTIAL:
                        if (nudStartBlock.Value < dangBlock)
                        {
                            DialogResult result = MessageBox.Show("You are about to erase/rewrite a dangerous area that can lead to bricking your Syscon Chip Do you want to continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.No)
                            {
                                processCancelled = true;
                                break;
                            }
      
                        }
                        sysconFWFilePath = txtInputOutputFile.Text;
                        startBlock = (short)nudStartBlock.Value;
                        endBlock = (short)nudEndBlock.Value;
                        noOfBlocks = (short)(endBlock - startBlock + 1);
                        prbProgress.Maximum = noOfBlocks;
                        enableAutoErase = chkAutoErase.Checked;

                        if (enableAutoErase)
                        {
                            iRet = ps4SysconTool.PS4SysconToolErase(startBlock, endBlock);
                        }

                        iRet = ps4SysconTool.PS4SysconToolWrite(sysconFWFilePath, startBlock, endBlock);
                        break;

                    case PS4SysconTool.SYSCON_PROCESS.WRITE_NVS_SNVS:
                        sysconFWFilePath = txtInputOutputFile.Text;
                        startBlock = (short)nudStartBlock.Value;
                        endBlock = (short)nudEndBlock.Value;
                        noOfBlocks = (short)(endBlock - startBlock + 1);
                        prbProgress.Maximum = noOfBlocks;
                        enableAutoErase = chkAutoErase.Checked;

                        if (enableAutoErase)
                        {
                            iRet = ps4SysconTool.PS4SysconToolErase(startBlock, endBlock);
                        }

                        iRet = ps4SysconTool.PS4SysconToolWrite(sysconFWFilePath, startBlock, endBlock);
                        break;
                    case PS4SysconTool.SYSCON_PROCESS.ENABLE_DEBUG_MODE:
                        prbProgress.Maximum = 3;
                        iRet = ps4SysconTool.PS4SysconToolEnableDebugMode();
                        break;
                    default:
                        break;
                }

                if (iRet == 0)
                {
                    if (noOfDumps > 1)
                    {
                        if (isFilesAreIdentical)
                        {
                            MessageBox.Show("Files are identical!\r\nProcess Done Successfully!!!", "Done!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Files are not identical!\r\nProcess Failed!!!", "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if (processCancelled)
                    {
                        MessageBox.Show("Process Cancelled!!!", "Cancelled!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        processCancelled = false;
                    }
                    else
                    {
                        MessageBox.Show("Process Done Successfully!!!", "Done!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Process Failed!!!", "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

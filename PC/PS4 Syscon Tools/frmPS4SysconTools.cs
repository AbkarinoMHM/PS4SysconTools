using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Collections.Generic;
using PS4_Syscon_Tools.Properties;

namespace PS4_Syscon_Tools
{
    public partial class frmPS4SysconTools : Form
    {
        PS4SysconTool ps4SysconTool;
        const int dangBlock = 4;
        string comPort;
        string version;
        string freememory;
        string sysconFWFilePath;
        short startBlock;
        short endBlock;
        short noOfBlocks;
        short noOfDumps;
        bool debugMode;
        bool isFilesAreIdentical;
        bool enableAutoConnect;
        bool enableEraseOptions;
        bool enableAutoDebugMode;
        bool enableAutoErase;
        bool enableDebugMode;
        bool enableAutoVerify;
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

            enableAutoConnect = Settings.Default.autoConnectMode;
            enableEraseOptions = Settings.Default.enableEraseOptions;
            enableAutoDebugMode = Settings.Default.enableAutoDebugMode;

            mnuEnableAutoConnect.Checked = enableAutoConnect;
            mnuEnableErase.Checked = enableEraseOptions;
            mnuEnableAutoDebugMode.Checked = enableAutoDebugMode;

            SetSysconProcess(enableEraseOptions);

            ScanCOMPorts();

            comPort = Settings.Default.comPort;
            if (!String.IsNullOrEmpty(comPort) && cboCOMPorts.Items.Contains(comPort))
            {
                cboCOMPorts.SelectedItem = comPort;

                if (enableAutoConnect) {
                    btnConnect.PerformClick();
                }
            }
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
            comPort = cboCOMPorts.SelectedItem.ToString();

            if (!debugMode)
            {
                iRet = ps4SysconTool.PS4SysconToolConnect(comPort,out version,out freememory, out debugMode);
                if (iRet == 0)
                {
                    tslSysconToolValue.Text = "Connected";
                    tslVersionValue.Text = version;
                    tslDebugModeValue.Text = (debugMode ? "Enabled" : "Disabled");
                    btnScan.Enabled = false;

                    if (!enableAutoConnect)
                    {
                        MessageBox.Show("PS4 Syscon Tool Connected Successfully!!", "Done!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    SetSysconProcess(enableEraseOptions);

                    grbSysconProcess.Enabled = true;
                    grbProcess.Enabled = true;
                    cboSysconProcess.SelectedIndex = 0;
                    btnConnect.Text = "Disconnect";

                    cboCOMPorts.Enabled = false;
                }
                else
                {
                    tslSysconToolValue.Text = "Disconnected";
                    tslVersionValue.Text = "";
                    tslDebugModeValue.Text = "";
                    btnScan.Enabled = true;

                    MessageBox.Show("Error Connecting To PS4 Syscon Tool!", "Connection Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    cboSysconProcess.SelectedIndex = -1;
                    grbSysconProcess.Enabled = false;
                    grbProcess.Enabled = false;
                    btnConnect.Text = "Connect";

                    cboCOMPorts.Enabled = true;
                }
            }
            else
            {
                iRet = ps4SysconTool.PS4SysconToolDisconnect(comPort, out debugMode);
                if (iRet == 0)
                {
                    grbSysconProcess.Enabled = false;
                    grbProcess.Enabled = true;
                    cboSysconProcess.SelectedIndex = 0;
                    btnConnect.Text = "Connect";
                    btnScan.Enabled = true;

                    cboCOMPorts.Enabled = true;
                }
            }

            Settings.Default.comPort = comPort;
            Settings.Default.Save();
        }

        private void cboSysconProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSysconProcess.SelectedIndex == -1) {
                return;
            }

            prbProgress.Value = 0;
            btnStart.Enabled = true;    // fix bug that start button still disabled if there is no COM ports detected at application launch.
            //txtLog.Clear();

            PS4SysconTool.SysconProcess process = (PS4SysconTool.SysconProcess)(cboSysconProcess.SelectedItem);

            sysconProcess = (PS4SysconTool.SYSCON_PROCESS)process.Value;

            switch (sysconProcess)
            {
                case PS4SysconTool.SYSCON_PROCESS.DUMP_FULL:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_FLASH_START_BLOCK;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    nudNoOfDumps.Enabled = true;
                    chkAutoErase.Checked = false;
                    chkAutoErase.Enabled = false;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    chkAutoVerify.Checked = false;
                    chkAutoVerify.Enabled = false;
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
                    chkAutoVerify.Checked = false;
                    chkAutoVerify.Enabled = false;
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
                    chkAutoVerify.Checked = false;
                    chkAutoVerify.Enabled = false;
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
                    chkAutoVerify.Checked = false;
                    chkAutoVerify.Enabled = false;
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
                    chkAutoVerify.Checked = false;
                    chkAutoVerify.Enabled = false;
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
                    chkAutoVerify.Checked = false;
                    chkAutoVerify.Enabled = false;
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
                    chkAutoVerify.Checked = true;
                    chkAutoVerify.Enabled = true;
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
                    chkAutoVerify.Checked = true;
                    chkAutoVerify.Enabled = true;
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
                    chkAutoVerify.Checked = true;
                    chkAutoVerify.Enabled = true;
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
                    chkAutoVerify.Checked = true;
                    chkAutoVerify.Enabled = true;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.GET_DUMP_INFO:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_FLASH_START_BLOCK;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    chkAutoErase.Checked = false;
                    chkAutoErase.Enabled = false;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    chkAutoVerify.Checked = false;
                    chkAutoVerify.Enabled = false;
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

                            isFilesAreIdentical = Util.IsFilesIdentical(sysconFWFile1, sysconFWFile2);
                            if (isFilesAreIdentical) {
                                iRet = ps4SysconTool.PS4SysconToolGetFWInfo(sysconFWFile1);
                            }
                        }
                        else
                        {
                            iRet = ps4SysconTool.PS4SysconToolFullDump(sysconFWFilePath);
                            if (iRet == 0) {
                                iRet = ps4SysconTool.PS4SysconToolGetFWInfo(sysconFWFilePath);
                            }
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
                            DialogResult result = MessageBox.Show("You Are About To Erase/Rewrite a Dangerous Area That Can Lead To Bricking Your Syscon Chip.\n Do You Want To Continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
                            DialogResult result = MessageBox.Show("You Are About To Erase/Rewrite a Dangerous Area That Can Lead To Bricking Your Syscon Chip.\n Do You Want To Continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
                        byte[] sysconFWBuffer = new byte[PS4SysconTool.SYSCON_FLASH_SIZE];

                        if (nudStartBlock.Value < dangBlock)
                        {
                            DialogResult result = MessageBox.Show("You Are About To Erase/Rewrite a Dangerous Area That Can Lead To Bricking Your Syscon Chip.\n Do You Want To Continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.No)
                            {
                                processCancelled = true;
                                break;
                            }

                        }
                        sysconFWFilePath = txtInputOutputFile.Text;

                        enableAutoErase = chkAutoErase.Checked;
                        enableDebugMode = chkEnableDebugMode.Checked;

                        prbProgress.Maximum = (int)PS4SysconTool.SYSCON_BLOCKS_NO;

                        iRet = Util.LoadFile(sysconFWFilePath, PS4SysconTool.SYSCON_FLASH_START_BLOCK, PS4SysconTool.SYSCON_FLASH_END_BLOCK, out sysconFWBuffer);
                        if (iRet != 0) {
                            MessageBox.Show("Error Loading Syscon Firmware File.\nPlease Select a Correct File Then Try Again.", 
                                "Error Loading Syscon Dump.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            iRet = -1;
                            break;
                        }

                        PS4SysconInfo.PS4SysconFWInfo ps4SysconFWInfo = ps4SysconTool.PS4SysconToolGetFWInfo(sysconFWBuffer);
                        if (ps4SysconFWInfo == null) {
                            MessageBox.Show("Error Getting Syscon Firmware Info From The Loaded File.\nPlease Select a Correct File Then Try Again.", 
                                "Error Getting Syscon FW Info!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            iRet = -1;
                            break;
                        }

                        // check if debug mode not patched and not enabled
                        if (ps4SysconFWInfo.debugMode == PS4SysconInfo.SYSCON_DEBUG_MODES.NONE && !enableDebugMode) {
                            DialogResult result = MessageBox.Show("Debug Mode Not Enabled On The Selected Dump.\nIt Is Recommened To Enable It Now.\nDo you want to enable Debug mode now?",
                                "Debug mode disabled!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes) {
                                enableAutoDebugMode = true;
                            }
                        }

                        // enable debug mode if enabled
                        if (enableAutoDebugMode)
                        {
                            sysconFWBuffer[PS4SysconTool.SYSCON_DEBUG_SETTING_OFFSET] = (byte)PS4SysconInfo.SYSCON_DEBUG_MODES.DEBUG_85;
                        }

                        iRet = ps4SysconTool.PS4SysconToolWrite(sysconFWBuffer, PS4SysconTool.SYSCON_FLASH_START_BLOCK, PS4SysconTool.SYSCON_FLASH_END_BLOCK, 
                            enableAutoErase);
                        if (iRet != 0) {
                            /// todo: to add error message here
                            break;
                        }

                        if (enableAutoVerify) {
                            byte[] dumpBuffer = new byte[PS4SysconTool.SYSCON_FLASH_SIZE];
                            iRet = ps4SysconTool.PS4SysconToolDump(out dumpBuffer, PS4SysconTool.SYSCON_FLASH_START_BLOCK, PS4SysconTool.SYSCON_FLASH_END_BLOCK);
                            if (iRet != 0)
                            {
                                isFilesAreIdentical = false;
                                /// todo: to add error message here
                                break;
                            }

                            isFilesAreIdentical = Util.IsByteArrayIdentical(dumpBuffer, sysconFWBuffer);
                           
                        }

                        break;
                    case PS4SysconTool.SYSCON_PROCESS.WRITE_PARTIAL:
                        if (nudStartBlock.Value < dangBlock)
                        {
                            DialogResult result = MessageBox.Show("You Are About To Erase/Rewrite a Dangerous Area That Can Lead To Bricking Your Syscon Chip.\n Do You Want To Continue ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
                        enableDebugMode = chkEnableDebugMode.Checked;

                        iRet = ps4SysconTool.PS4SysconToolWrite(sysconFWFilePath, startBlock, endBlock, enableAutoErase);
                        break;

                    case PS4SysconTool.SYSCON_PROCESS.WRITE_NVS_SNVS:
                        sysconFWFilePath = txtInputOutputFile.Text;
                        startBlock = (short)nudStartBlock.Value;
                        endBlock = (short)nudEndBlock.Value;
                        noOfBlocks = (short)(endBlock - startBlock + 1);
                        prbProgress.Maximum = noOfBlocks;

                        enableAutoErase = chkAutoErase.Checked;

                        iRet = ps4SysconTool.PS4SysconToolWrite(sysconFWFilePath, startBlock, endBlock, enableAutoErase);
                        break;
                    case PS4SysconTool.SYSCON_PROCESS.ENABLE_DEBUG_MODE:
                        prbProgress.Maximum = 3;
                        iRet = ps4SysconTool.PS4SysconToolEnableDebugMode();
                        if (iRet == -10) { // debug mode already enabled
                            MessageBox.Show("Debug Mode Already Enabled\nNo Need To Re-Enable It Again.",
                                "Debug Mode Enabled.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            iRet = 0;
                        } 

                        break;
                    case PS4SysconTool.SYSCON_PROCESS.GET_DUMP_INFO:
                        sysconFWFilePath = txtInputOutputFile.Text;
                        prbProgress.Maximum = 1;
                        iRet = ps4SysconTool.PS4SysconToolGetFWInfo(sysconFWFilePath);
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
                        if (enableAutoVerify)
                        {
                            if (isFilesAreIdentical)
                            {
                                MessageBox.Show("PS4 Syscon FW Data Written and Verified Successfully!", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Error Verifying PS4 Syscon FW Written Data!", "Verify Data Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                        else {
                            MessageBox.Show("Process Done Successfully!!!", "Done!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
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
                }
                else
                {
                    btnStart.Enabled = false;
                    btnConnect.Enabled = false;

                    cboCOMPorts.SelectedIndex = -1;
                    MessageBox.Show("Error no COM Ports available", "Error Scanning COM Port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(String.Format("Error during scanning COM Ports.\nError: {0}", ex.Message), "Error Scanning COM Port", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetSysconProcess(bool enableEraseOptions) {

            PS4SysconTool.PS4SysconProcess[4].Enabled = enableEraseOptions;
            PS4SysconTool.PS4SysconProcess[5].Enabled = enableEraseOptions;
            PS4SysconTool.PS4SysconProcess[6].Enabled = enableEraseOptions;
            PS4SysconTool.PS4SysconProcess[7].Enabled = enableEraseOptions;

            //cboSysconProcess.Items.Clear();
            cboSysconProcess.DataSource = (PS4SysconTool.PS4SysconProcess).Where(x => x.Enabled == true).ToList();
            cboSysconProcess.DisplayMember = "Name";
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

        private void mnuEnableErase_Click(object sender, EventArgs e)
        {
            enableEraseOptions = !enableEraseOptions;
            mnuEnableErase.Checked = enableEraseOptions;

            SetSysconProcess(enableEraseOptions);

            Settings.Default.enableEraseOptions = enableEraseOptions;
            Settings.Default.Save();

        }

        private void mnuEnableAutoConnect_Click(object sender, EventArgs e)
        {
            enableAutoConnect = !enableAutoConnect;
            mnuEnableAutoConnect.Checked = enableAutoConnect;

            Settings.Default.autoConnectMode = enableAutoConnect;
            Settings.Default.Save();
        }

        private void mnuEnableAutoDebugMode_Click(object sender, EventArgs e)
        {
            enableAutoDebugMode = !enableAutoDebugMode;
            mnuEnableAutoDebugMode.Checked = enableAutoDebugMode;
            chkEnableDebugMode.Checked = enableAutoDebugMode;

            Settings.Default.enableAutoDebugMode = enableAutoDebugMode;
            Settings.Default.Save();
        }

        private void cboCOMPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            comPort = cboCOMPorts.SelectedItem.ToString();
        }

        private void chkAutoVerify_CheckedChanged(object sender, EventArgs e)
        {
            enableAutoVerify = chkAutoVerify.Checked;
        }

        private void chkEnableDebugMode_CheckedChanged(object sender, EventArgs e)
        {
            enableDebugMode = chkEnableDebugMode.Checked;
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }
    }
}

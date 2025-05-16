using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using PS4_Syscon_Tools.Properties;
using System.Management;
using static PS4_Syscon_Tools.PS4SysconTool;
using System.Drawing;
using RJCP.IO.Ports;

namespace PS4_Syscon_Tools
{
    public partial class frmPS4SysconTools : Form
    {
        static PS4SysconTool ps4SysconTool;

        static float minimumSupportedFW = 2.02F;

        const int dangBlock = 4;
        static string comPort = string.Empty;
        static string version = string.Empty;
        static string freememory = string.Empty;
        static bool debugMode = false;
        static bool isDetected = false;
        static bool isInit = false;
        string sysconFWFilePath = string.Empty;
        short startBlock;
        short endBlock;
        short noOfBlocks;
        short noOfDumps;

        bool isFilesAreIdentical;
        bool enableAdvancedOptions;
        bool enableAutoDebugMode;
        bool enableDebugMode;
        bool enableAutoVerify;
        bool processCancelled;

        int iRet = 0;
        static PS4SysconTool.SYSCON_PROCESS sysconProcess = PS4SysconTool.SYSCON_PROCESS.GET_DUMP_INFO;

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

            enableAdvancedOptions = Settings.Default.enableEraseOptions;
            enableAutoDebugMode = Settings.Default.enableAutoDebugMode;

            mnuEnableAdvancedOptions.Checked = enableAdvancedOptions;
            mnuEnableAutoDebugMode.Checked = enableAutoDebugMode;

            SetSysconProcess(false, enableAdvancedOptions);

            // Check for already connected Syscon Flasher (Teensy) devices
            CheckForConnectedPS4SysconTool();

            // Set up event watchers for device connection and disconnection
            SetupEventWatchers();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cboSysconProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSysconProcess.SelectedIndex == -1)
            {
                return;
            }

            prbProgress.Value = 0;
            btnStart.Enabled = true;    // fix bug that start button still disabled if there is no COM ports detected at application launch.
            //txtLog.Clear();

            PS4SysconTool.SysconProcess process = (PS4SysconTool.SysconProcess)(cboSysconProcess.SelectedItem);

            sysconProcess = (PS4SysconTool.SYSCON_PROCESS)process.Value;

            switch (sysconProcess)
            {
                case PS4SysconTool.SYSCON_PROCESS.GET_DUMP_INFO:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_FLASH_START_BLOCK;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    nudNoOfDumps.Enabled = false;
                    chkEnableDebugMode.Checked = false;
                    chkEnableDebugMode.Enabled = false;
                    chkAutoVerify.Checked = false;
                    chkAutoVerify.Enabled = false;
                    break;
                case PS4SysconTool.SYSCON_PROCESS.DUMP_FULL:
                    btnBrowse.Enabled = true;
                    txtInputOutputFile.Enabled = true;
                    nudStartBlock.Value = PS4SysconTool.SYSCON_FLASH_START_BLOCK;
                    nudEndBlock.Value = PS4SysconTool.SYSCON_FLASH_END_BLOCK;
                    nudStartBlock.Enabled = false;
                    nudEndBlock.Enabled = false;
                    nudNoOfDumps.Enabled = true;
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
                    chkEnableDebugMode.Checked = true;
                    chkEnableDebugMode.Enabled = false;
                    chkAutoVerify.Checked = true;
                    chkAutoVerify.Enabled = true;
                    break;
                default:
                    break;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (sysconProcess == PS4SysconTool.SYSCON_PROCESS.DUMP_FULL || sysconProcess == PS4SysconTool.SYSCON_PROCESS.DUMP_PARTIAL || sysconProcess == PS4SysconTool.SYSCON_PROCESS.DUMP_NVS_SNVS)
            {
                if (dlgSaveFile.ShowDialog() == DialogResult.OK)
                {
                    txtInputOutputFile.Text = dlgSaveFile.FileName;
                }
            }
            else
            {
                if (dlgOpenFile.ShowDialog() == DialogResult.OK)
                {
                    txtInputOutputFile.Text = dlgOpenFile.FileName;
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            byte[] sysconFWBuffer = new byte[] { };
            btnStart.Enabled = false;

            prbProgress.Minimum = 0;
            noOfDumps = 0;
            txtLog.Clear();

            btnStart.Enabled = false;
            btnBrowse.Enabled = false;
            txtInputOutputFile.Enabled = false;
            cboSysconProcess.Enabled = false;

            try
            {

                if (sysconProcess == PS4SysconTool.SYSCON_PROCESS.GET_DUMP_INFO)
                {
                    sysconFWFilePath = txtInputOutputFile.Text;

                    prbProgress.Maximum = 1;
                    iRet = ps4SysconTool.PS4SysconToolGetFWInfo(sysconFWFilePath);
                }
                else 
                {

                    if (!isDetected || String.IsNullOrEmpty(comPort))
                    {
                        _ = MessageBox.Show("Error Detecting PS4 Syscon Flasher, Please Check Connection And Try Again.", 
                            "Error Detecting PS4 Syscon Flasher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    using (SerialPortStream ps4SysconFlasher = new SerialPortStream(comPort, 115200, 8, RJCP.IO.Ports.Parity.None, RJCP.IO.Ports.StopBits.One))
                    {

                        ps4SysconFlasher.Open();
                        ps4SysconFlasher.DiscardOutBuffer();
                        ps4SysconFlasher.DiscardInBuffer();

                        if (!ps4SysconFlasher.IsOpen)
                        {
                            comPort = string.Empty;
                            version = string.Empty;
                            isDetected = false;

                            _ = MessageBox.Show("Error Connecting To PS4 Syscon Flasher, Please Check Connection And Try Again.", 
                                "Error Connecting PS4 Syscon Flasher", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            return;
                        }

                        if (!isInit)
                        {
                            isInit = ps4SysconTool.PS4SysconToolInit(ps4SysconFlasher);
                            if (!isInit)
                            {
                                _ = MessageBox.Show("Error while exploiting PS4 Syscon!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            UpdateStatus(isDetected, version);
                        }

                        switch (sysconProcess)
                        {
                            case PS4SysconTool.SYSCON_PROCESS.DUMP_FULL:
                                sysconFWFilePath = txtInputOutputFile.Text;

                                if (String.IsNullOrEmpty(sysconFWFilePath))
                                {
                                    iRet = -1;
                                    break;
                                }

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

                                    iRet = ps4SysconTool.PS4SysconToolFullDump(ps4SysconFlasher, sysconFWFile1);
                                    if (iRet != 0)
                                    {
                                        break;
                                    }

                                    txtInputOutputFile.Text = sysconFWFile2;

                                    iRet = ps4SysconTool.PS4SysconToolFullDump(ps4SysconFlasher, sysconFWFile2);
                                    if (iRet != 0)
                                    {
                                        break;
                                    }

                                    isFilesAreIdentical = Util.IsFilesIdentical(sysconFWFile1, sysconFWFile2);
                                    if (isFilesAreIdentical)
                                    {
                                        iRet = ps4SysconTool.PS4SysconToolGetFWInfo(sysconFWFile1);
                                    }
                                }
                                else
                                {
                                    iRet = ps4SysconTool.PS4SysconToolFullDump(ps4SysconFlasher, sysconFWFilePath);
                                    if (iRet == 0)
                                    {
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

                                    iRet = ps4SysconTool.PS4SysconToolDump(ps4SysconFlasher, sysconFWFile1, startBlock, endBlock);
                                    if (iRet != 0)
                                    {
                                        break;
                                    }

                                    txtInputOutputFile.Text = sysconFWFile2;

                                    iRet = ps4SysconTool.PS4SysconToolDump(ps4SysconFlasher, sysconFWFile2, startBlock, endBlock);
                                    if (iRet != 0)
                                    {
                                        break;
                                    }

                                    isFilesAreIdentical = Util.IsFilesIdentical(sysconFWFile1, sysconFWFile2);
                                }
                                else
                                {
                                    iRet = ps4SysconTool.PS4SysconToolDump(ps4SysconFlasher, sysconFWFilePath, startBlock, endBlock);
                                }

                                break;
                            case PS4SysconTool.SYSCON_PROCESS.DUMP_NVS_SNVS:
                                sysconFWFilePath = txtInputOutputFile.Text;
                                startBlock = PS4SysconTool.SYSCON_NVS_SNVS_START_BLOCK;
                                endBlock = PS4SysconTool.SYSCON_NVS_SNVS_END_BLOCK;
                                noOfBlocks = PS4SysconTool.SYSCON_NVS_SNVS_BLOCKS;
                                prbProgress.Maximum = PS4SysconTool.SYSCON_NVS_SNVS_BLOCKS;

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

                                    iRet = ps4SysconTool.PS4SysconToolNVSSNVSDump(ps4SysconFlasher, sysconFWFile1);
                                    if (iRet != 0)
                                    {
                                        break;
                                    }

                                    txtInputOutputFile.Text = sysconFWFile2;

                                    iRet = ps4SysconTool.PS4SysconToolNVSSNVSDump(ps4SysconFlasher, sysconFWFile2);
                                    if (iRet != 0)
                                    {
                                        break;
                                    }

                                    isFilesAreIdentical = Util.IsFilesIdentical(sysconFWFile1, sysconFWFile2);
                                }
                                else
                                {
                                    iRet = ps4SysconTool.PS4SysconToolNVSSNVSDump(ps4SysconFlasher, sysconFWFilePath);
                                }

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

                                iRet = ps4SysconTool.PS4SysconToolErase(ps4SysconFlasher, PS4SysconTool.SYSCON_FLASH_START_BLOCK, PS4SysconTool.SYSCON_FLASH_END_BLOCK);
                                break;
                            case PS4SysconTool.SYSCON_PROCESS.ERASE_EXCEPT_BOOT0:
                                startBlock = (short)nudStartBlock.Value;
                                endBlock = (short)nudEndBlock.Value;
                                noOfBlocks = (short)(endBlock - startBlock + 1);
                                prbProgress.Maximum = noOfBlocks;

                                iRet = ps4SysconTool.PS4SysconToolErase(ps4SysconFlasher, PS4SysconTool.SYSCON_FLASH_PARTIAL_START_BLOCK, PS4SysconTool.SYSCON_FLASH_END_BLOCK);
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

                                iRet = ps4SysconTool.PS4SysconToolErase(ps4SysconFlasher, startBlock, endBlock);
                                break;
                            case PS4SysconTool.SYSCON_PROCESS.ERASE_NVS_SNVS:
                                prbProgress.Maximum = (int)PS4SysconTool.SYSCON_NVS_SNVS_SIZE;

                                iRet = ps4SysconTool.PS4SysconToolErase(ps4SysconFlasher, PS4SysconTool.SYSCON_NVS_SNVS_START_BLOCK, PS4SysconTool.SYSCON_NVS_SNVS_END_BLOCK);
                                break;
                            case PS4SysconTool.SYSCON_PROCESS.WRITE_FULL:
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

                                enableDebugMode = chkEnableDebugMode.Checked;

                                prbProgress.Maximum = (int)PS4SysconTool.SYSCON_BLOCKS_NO;

                                sysconFWBuffer = new byte[PS4SysconTool.SYSCON_FLASH_SIZE];

                                iRet = Util.LoadFile(sysconFWFilePath, PS4SysconTool.SYSCON_FLASH_START_BLOCK, PS4SysconTool.SYSCON_FLASH_END_BLOCK, out sysconFWBuffer);
                                if (iRet != 0)
                                {
                                    _ = MessageBox.Show("Error Loading Syscon Firmware File.\nPlease Select a Correct File Then Try Again.",
                                        "Error Loading Syscon Dump.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    iRet = -1;
                                    break;
                                }

                                PS4SysconInfo.PS4SysconFWInfo ps4SysconFWInfo = ps4SysconTool.PS4SysconToolGetFWInfo(sysconFWBuffer);
                                if (ps4SysconFWInfo == null)
                                {
                                    _ = MessageBox.Show("Error Getting Syscon Firmware Info From The Loaded File.\nPlease Select a Correct File Then Try Again.",
                                        "Error Getting Syscon FW Info!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    iRet = -1;
                                    break;
                                }

                                // check if debug mode not patched and not enabled
                                if (ps4SysconFWInfo.debugMode == PS4SysconInfo.SYSCON_DEBUG_MODES.NONE && !enableDebugMode)
                                {
                                    DialogResult result = MessageBox.Show("Debug Mode Not Enabled On The Selected Dump.\nIt Is Recommened To Enable It Now.\nDo you want to enable Debug mode now?",
                                        "Debug mode disabled!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (result == DialogResult.Yes)
                                    {
                                        enableAutoDebugMode = true;
                                    }
                                }

                                // enable debug mode if enabled
                                if (enableAutoDebugMode)
                                {
                                    sysconFWBuffer[PS4SysconTool.SYSCON_DEBUG_SETTING_OFFSET] = (byte)PS4SysconInfo.SYSCON_DEBUG_MODES.DEBUG_85;
                                }

                                iRet = ps4SysconTool.PS4SysconToolWrite(ps4SysconFlasher, sysconFWBuffer, PS4SysconTool.SYSCON_FLASH_START_BLOCK, PS4SysconTool.SYSCON_FLASH_END_BLOCK,
                                    true);
                                if (iRet != 0)
                                {
                                    /// todo: to add error message here
                                    break;
                                }

                                if (enableAutoVerify)
                                {
                                    byte[] dumpBuffer = new byte[PS4SysconTool.SYSCON_FLASH_SIZE];
                                    iRet = ps4SysconTool.PS4SysconToolDump(ps4SysconFlasher, out dumpBuffer, PS4SysconTool.SYSCON_FLASH_START_BLOCK, PS4SysconTool.SYSCON_FLASH_END_BLOCK);
                                    if (iRet != 0)
                                    {
                                        isFilesAreIdentical = false;
                                        /// todo: to add error message here
                                        break;
                                    }

                                    SetText("Verifying written data!");

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

                                enableDebugMode = chkEnableDebugMode.Checked;

                                sysconFWBuffer = new byte[noOfBlocks * PS4SysconTool.SYSCON_BLOCK_SIZE];

                                iRet = Util.LoadFile(sysconFWFilePath, startBlock, endBlock, out sysconFWBuffer);
                                if (iRet != 0)
                                {
                                    MessageBox.Show("Error Loading Syscon Firmware File.\nPlease Select a Correct File Then Try Again.",
                                        "Error Loading Syscon Dump.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    iRet = -1;
                                    break;
                                }

                                iRet = ps4SysconTool.PS4SysconToolWrite(ps4SysconFlasher, sysconFWBuffer, startBlock, endBlock, true);
                                if (iRet != 0)
                                {
                                    /// todo: to add error message here
                                    break;
                                }

                                if (enableAutoVerify)
                                {
                                    byte[] dumpBuffer = new byte[noOfBlocks * PS4SysconTool.SYSCON_BLOCK_SIZE];
                                    iRet = ps4SysconTool.PS4SysconToolDump(ps4SysconFlasher, out dumpBuffer, startBlock, endBlock);
                                    if (iRet != 0)
                                    {
                                        isFilesAreIdentical = false;
                                        /// todo: to add error message here
                                        break;
                                    }

                                    isFilesAreIdentical = Util.IsByteArrayIdentical(dumpBuffer, sysconFWBuffer);

                                }

                                break;
                            case PS4SysconTool.SYSCON_PROCESS.WRITE_NVS_SNVS:
                                sysconFWFilePath = txtInputOutputFile.Text;
                                startBlock = PS4SysconTool.SYSCON_NVS_SNVS_START_BLOCK;
                                endBlock = PS4SysconTool.SYSCON_NVS_SNVS_END_BLOCK;
                                noOfBlocks = PS4SysconTool.SYSCON_NVS_SNVS_BLOCKS;
                                prbProgress.Maximum = PS4SysconTool.SYSCON_NVS_SNVS_BLOCKS;

                                sysconFWBuffer = new byte[PS4SysconTool.SYSCON_NVS_SNVS_SIZE];

                                // detect file type automatically (full dump/snvs-nvs dump).
                                FileInfo dumpFileSize = new FileInfo(sysconFWFilePath);
                                if (dumpFileSize.Exists && (dumpFileSize.Length == PS4SysconTool.SYSCON_FLASH_SIZE))
                                {   // full dump.
                                    iRet = Util.LoadFile(sysconFWFilePath, PS4SysconTool.SYSCON_NVS_SNVS_START_BLOCK, PS4SysconTool.SYSCON_NVS_SNVS_END_BLOCK, out sysconFWBuffer);
                                }
                                else if (dumpFileSize.Exists && (dumpFileSize.Length == PS4SysconTool.SYSCON_NVS_SNVS_SIZE))
                                {     // snvs-nvs dump.
                                    iRet = Util.LoadFile(sysconFWFilePath, 0, PS4SysconTool.SYSCON_NVS_SNVS_BLOCKS - 1, out sysconFWBuffer);
                                }
                                else
                                {
                                    MessageBox.Show("Error Loading Syscon SNVS/NVS Dump File.\nYou Must Select Eiher Full Syscon Dump Or NVS/SNVS Dump only.\nPlease Select a Correct File Then Try Again.",
                                                                    "Error Loading Syscon SNVS/NVS Dump.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    iRet = -1;
                                    break;
                                }


                                if (iRet != 0)
                                {
                                    _ = MessageBox.Show("Error Loading Syscon SNVS/NVS Dump File.\nPlease Select a Correct File Then Try Again.",
                                        "Error Loading Syscon SNVS/NVS Dump.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    iRet = -1;
                                    break;
                                }

                                iRet = ps4SysconTool.PS4SysconToolWrite(ps4SysconFlasher, sysconFWBuffer, startBlock, endBlock, true);
                                if (iRet != 0)
                                {
                                    /// todo: to add error message here
                                    break;
                                }

                                if (enableAutoVerify)
                                {
                                    byte[] dumpBuffer = new byte[PS4SysconTool.SYSCON_NVS_SNVS_SIZE];
                                    iRet = ps4SysconTool.PS4SysconToolDump(ps4SysconFlasher, out dumpBuffer, startBlock, endBlock);
                                    if (iRet != 0)
                                    {
                                        isFilesAreIdentical = false;
                                        /// todo: to add error message here
                                        break;
                                    }

                                    isFilesAreIdentical = Util.IsByteArrayIdentical(dumpBuffer, sysconFWBuffer);

                                }

                                break;
                            case PS4SysconTool.SYSCON_PROCESS.ENABLE_DEBUG_MODE:
                                prbProgress.Maximum = 2;
                                iRet = ps4SysconTool.PS4SysconToolEnableDebugMode(ps4SysconFlasher);
                                if (iRet == -10)
                                { // debug mode already enabled
                                    _ = MessageBox.Show("Debug Mode Already Enabled\nNo Need To Re-Enable It Again.",
                                        "Debug Mode Enabled.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    iRet = 0;
                                    enableAutoVerify = false;
                                }

                                break;
                            default:
                                break;
                        }
                    }
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
                                SetText("PS4 Syscon FW Data Written and Verified Successfully!");
                            }
                            else
                            {
                                MessageBox.Show("Error Verifying PS4 Syscon FW Written Data!", "Verify Data Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                SetText("Error Verifying PS4 Syscon FW Written Data!");
                            }

                        }
                        else
                        {
                            MessageBox.Show("Process Done Successfully!!!", "Done!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            SetText("Process Done Successfully!");
                        }
                    }
                }
                else if (iRet == -1) // wrong file path of file path is empty
                {
                    _ = MessageBox.Show("Error Dump File Path Can't be Empty.\nPlease Select a Correct Path Then Try Again!.",
                        "Process Fail!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (iRet == -2) // file was not exist
                {
                    _ = MessageBox.Show("Error Selected Dump File Is Not Exist, Please Select An Exist Dump File First Then Try Again!.",
                        "Process Fail!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    _ = MessageBox.Show("Process Failed!!!", "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(string.Format("Error: {0}.", ex.Message), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cboSysconProcess.Enabled = true;
                btnStart.Enabled = true;
                btnBrowse.Enabled = true;
                txtInputOutputFile.Enabled = true;
            }
        }

        private void SetSysconProcess(bool enableAllOptions, bool enableAdvancedOptions = false)
        {
            if (enableAllOptions)
            {
                PS4SysconTool.PS4SysconProcess[2].Enabled = enableAdvancedOptions;
                PS4SysconTool.PS4SysconProcess[3].Enabled = enableAdvancedOptions;
                PS4SysconTool.PS4SysconProcess[4].Enabled = enableAdvancedOptions;
                PS4SysconTool.PS4SysconProcess[5].Enabled = enableAdvancedOptions;
                PS4SysconTool.PS4SysconProcess[6].Enabled = enableAdvancedOptions;
                PS4SysconTool.PS4SysconProcess[7].Enabled = enableAdvancedOptions;
                PS4SysconTool.PS4SysconProcess[8].Enabled = enableAdvancedOptions;
                PS4SysconTool.PS4SysconProcess[9].Enabled = enableAdvancedOptions;

                cboSysconProcess.DataSource = (PS4SysconTool.PS4SysconProcess).Where(x => x.Enabled == true).ToList();
            }
            else
            {
                cboSysconProcess.DataSource = (PS4SysconTool.PS4SysconProcess).Where(x => x.Value == (short)SYSCON_PROCESS.GET_DUMP_INFO).ToList();
            }


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

        private void SetProgressValue(int value)
        {
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

            if (value >= 0)
            {
                SetProgressValue(value);
            }

            Application.DoEvents();
        }

        private void mnuEnableAdvancedOptions_Click(object sender, EventArgs e)
        {
            if (!enableAdvancedOptions)
            {
                DialogResult result = MessageBox.Show("You Are Going To Enable Advanced Options, That Is Meant For Developers And People Who Know How To Use It Only.\n Are You Sure That You Want To Enable Advanced Options?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }

            }
            enableAdvancedOptions = !enableAdvancedOptions;
            mnuEnableAdvancedOptions.Checked = enableAdvancedOptions;

            SetSysconProcess(isDetected, enableAdvancedOptions);

            Settings.Default.enableEraseOptions = enableAdvancedOptions;
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
            _ = about.ShowDialog();
        }

        private void UpdateStatus(bool connected, string version)
        {
            if (connected)
            {
                tslSysconToolValue.ForeColor = Color.FromArgb(0, 192, 0);
                tslSysconToolValue.Text = "Connected";

                tslVersionValue.Text = version;

                if (btnStart.Enabled)
                {
                    SetSysconProcess(connected, enableAdvancedOptions);
                    cboSysconProcess.SelectedIndex = 1;
                }

            }
            else
            {
                tslSysconToolValue.ForeColor = Color.Red;
                tslSysconToolValue.Text = "Disconnected";
                tslVersionValue.Text = "";
                isInit = false;

                SetSysconProcess(connected, enableAdvancedOptions);
            }

        }

        private void CheckForConnectedPS4SysconTool()
        {
            // Create a WMI query to check for connected devices
            string query = $"SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE '%USB\\\\VID_{PS4SysconTool.PS4_SYSCON_FLASHER_VID}%&PID_{PS4SysconTool.PS4_SYSCON_FLASHER_PID}%'";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject device in searcher.Get())
                {
                    string deviceId = device["DeviceID"].ToString();
                    comPort = GetComPortFromDeviceId(deviceId);
                    if (comPort != null)
                    {
                        isDetected = ps4SysconTool.PS4SysconToolDetect(comPort, out version, out freememory);
                        if (isDetected) {
                            float currentVersion = 0;
                            if (float.TryParse(version, out currentVersion))
                            {
                                if (currentVersion < minimumSupportedFW) {
                                    isDetected = false;
                                    txtLog.Clear();
                                    _ = MessageBox.Show($"Error Detected PS4 Syscon Flasher Have Old Unsupported Firmware, Please Update It First To FW >= {minimumSupportedFW}!.",
                                    "PS4 Syscon Flasher Firmware Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else {
                                isDetected = false;
                                txtLog.Clear();
                                _ = MessageBox.Show("Error Detected PS4 Syscon Flasher Have An Unsupported Firmware Or Firmware Can not Be Detected!.", 
                                    "PS4 Syscon Flasher Firmware Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        UpdateStatus(isDetected, version);
                    }
                    else
                    {
                        isDetected = false;
                        comPort = "";
                        version = "";
                        isInit = false;
                    }
                }
            }
        }

        void SetupEventWatchers()
        {
            // Watcher for device arrival
            string arrivalQuery = "SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'";
            var arrivalWatcher = new ManagementEventWatcher(arrivalQuery);
            arrivalWatcher.EventArrived += (sender, e) =>
            {
                var instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                string deviceId = instance["DeviceID"].ToString();
                if (deviceId.Contains($"USB\\VID_{PS4SysconTool.PS4_SYSCON_FLASHER_VID}&PID_{PS4SysconTool.PS4_SYSCON_FLASHER_PID}"))
                {
                    comPort = GetComPortFromDeviceId(deviceId);
                    if (comPort != null)
                    {
                        isDetected = ps4SysconTool.PS4SysconToolDetect(comPort, out version, out freememory);
                    }
                    else
                    {
                        isDetected = false;
                        comPort = "";
                        version = "";
                        isInit = false;
                        txtLog.Clear();
                    }

                    UpdateStatus(isDetected, version);
                }
            };
            arrivalWatcher.Start();

            // Watcher for device removal
            string removalQuery = "SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'";
            var removalWatcher = new ManagementEventWatcher(removalQuery);
            removalWatcher.EventArrived += (sender, e) =>
            {
                var instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                string deviceId = instance["DeviceID"].ToString();
                if (deviceId.Contains($"USB\\VID_{PS4SysconTool.PS4_SYSCON_FLASHER_VID}&PID_{PS4SysconTool.PS4_SYSCON_FLASHER_PID}"))
                {
                    isDetected = false;
                    comPort = "";
                    version = "";
                    isInit = false;
                    txtLog.Clear();

                    UpdateStatus(isDetected, version);
                }


            };
            removalWatcher.Start();
        }

        private static string GetComPortName(ManagementBaseObject targetInstance)
        {
            // Extract the COM port name from the device description
            object caption = targetInstance["Caption"];
            if (caption != null)
            {
                string captionString = caption.ToString();
                int comIndex = captionString.IndexOf("(COM");
                if (comIndex != -1)
                {
                    return captionString.Substring(comIndex + 1, captionString.IndexOf(")", comIndex) - comIndex - 1);
                }
            }

            return null;
        }

        static string GetComPortFromDeviceId(string deviceId)
        {
            string escapedDeviceId = deviceId.Replace("\\", "\\\\");
            string query = $"SELECT * FROM Win32_SerialPort WHERE PNPDeviceID = '{escapedDeviceId}'";
            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject serialPort in searcher.Get())
                {
                    return serialPort["DeviceID"].ToString();
                }
            }
            return null;
        }

        private void mnuResetPS4SysconFlasher_Click(object sender, EventArgs e)
        {
            if (!isDetected || String.IsNullOrEmpty(comPort))
            {
                _ = MessageBox.Show("Error Detecting PS4 Syscon Flasher, Please Check Connection And Try Again.", "Error Detecting PS4 Syscon Flasher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SerialPortStream ps4SysconFlasher = new SerialPortStream(comPort, 115200, 8, RJCP.IO.Ports.Parity.None, RJCP.IO.Ports.StopBits.One))
            {
                isInit = !ps4SysconTool.PS4SysconToolReset(ps4SysconFlasher);
            }

            CheckForConnectedPS4SysconTool();
        }
    }
}

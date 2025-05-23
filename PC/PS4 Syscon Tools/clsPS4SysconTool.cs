﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

//using System.IO.Ports;
using RJCP.IO.Ports;

namespace PS4_Syscon_Tools
{
    class PS4SysconTool
    {

        public const string PS4_SYSCON_FLASHER_VID = "16C0";
        public const string PS4_SYSCON_FLASHER_PID = "0483";

        public const int SYSCON_BLOCK_SIZE = 0x400;
        public const int SYSCON_BUFFER_SIZE = 0x80;
        public const int SYSCON_WRITE_BUFFER_SIZE = 0x100;
        public const int SYSCON_SERIAL_BUFFER_SIZE = 0x400;
        public const int SYSCON_BLOCKS_NO = 0x200;
        public const long SYSCON_FLASH_SIZE = SYSCON_BLOCK_SIZE * SYSCON_BLOCKS_NO;
        public const int SYSCON_BOOT0_BLOCKS = 4;
        public const int SYSCON_FLASH_START_BLOCK = 0;
        public const int SYSCON_FLASH_PARTIAL_START_BLOCK = 4;
        public const int SYSCON_FLASH_END_BLOCK = 511;
        public const int SYSCON_FW_BLOCKS = 384;
        public const int SYSCON_NVS_SNVS_BLOCKS = 128;
        public const int SYSCON_NVS_SNVS_START_BLOCK = 384;
        public const int SYSCON_NVS_SNVS_END_BLOCK = 511;
        public const long SYSCON_NVS_SNVS_SIZE = SYSCON_NVS_SNVS_BLOCKS * SYSCON_BLOCK_SIZE;
        public const long SYSCON_FW_SIZE = SYSCON_FW_BLOCKS * SYSCON_BLOCK_SIZE;
        public const int SYSCON_DEBUG_SETTING_OFFSET = 0xC3;

        public const int SYSCON_OK = 0x00;
        public const int SYSCON_ERR_INT = 0xF0;
        public const int SYSCON_ERR_READ = 0xF1;
        public const int SYSCON_ERR_ERSASE = 0xF4;
        public const int SYSCON_ERR_WRITE = 0xF6;
        public const int SYSCON_ERR_CMD_LEN = 0xFA;
        public const int SYSCON_ERR_CMD_EXEC = 0xFE;
        public const int SYSCON_ERR = 0xFF;

        public static bool isDebugMode = false;
        private static bool isConnected = false;

        //private SerialPortStream serialPort;
        //private SerialPort serialPort;
        private static bool bolFinished;
        private static bool bolReceiveError;
        private static long lTimeout = 10000;
        private Stopwatch stopWatch;
        
        int response = 0;
        byte bRet;

        public enum SYSCON_PROCESS {
            GET_DUMP_INFO,          // Get Syscon Dump Info
            DUMP_FULL,              // Dump Full Syscon Flash
            DUMP_PARTIAL,           // Dump Partial Syscon Flash
            DUMP_NVS_SNVS,          // Dump Syscon NVS/SNVS Only
            ERASE_FULL,             // Erase Full Syscon Flash (Danger)
            ERASE_EXCEPT_BOOT0,     // Erase Full Syscon Flash Expect Boot0 Block (Safe)
            ERASE_PARTIAL,          // Erase Partial Syscon Flash
            ERASE_NVS_SNVS,         // Erase Syscon NVS/SNVS only
            WRITE_FULL,             // Write Full Syscon Flash
            WRITE_PARTIAL,          // Write Partial Syscon Flash
            WRITE_NVS_SNVS,         // Write Syscon NVS/SNVS Only
            ENABLE_DEBUG_MODE,      // Enable Syscon Debug Mode (ReWrite Boot0 Blocks)
        };

        public class SysconProcess
        {
            private short value;
            private string name;
            private bool enabled;

            public short Value
            {
                get
                {
                    return value;
                }

                set
                {
                    this.value = value;
                }
            }

            public string Name
            {
                get
                {
                    return name;
                }

                set
                {
                    name = value;
                }
            }

            public bool Enabled
            {
                get
                {
                    return enabled;
                }

                set
                {
                    enabled = value;
                }
            }

            public SysconProcess(short value, string name, bool enabled = true)
            {
                this.Value = value;
                this.Name = name;
                this.Enabled = enabled;
            }

        };

        public static SysconProcess[] PS4SysconProcess = {
            new SysconProcess((short)SYSCON_PROCESS.GET_DUMP_INFO, "Get Syscon Dump Info", true),
            new SysconProcess((short)SYSCON_PROCESS.DUMP_FULL, "Dump Full Syscon Flash ", true),
            new SysconProcess((short)SYSCON_PROCESS.DUMP_PARTIAL, "Dump Partial Syscon Flash", false),
            new SysconProcess((short)SYSCON_PROCESS.DUMP_NVS_SNVS, "Dump Syscon NVS/SNVS Only", false),
            new SysconProcess((short)SYSCON_PROCESS.ERASE_FULL, "Erase Full Syscon Flash (Danger)", false),
            new SysconProcess((short)SYSCON_PROCESS.ERASE_EXCEPT_BOOT0, "Erase Full Syscon Flash Expect Boot0 Block (Safe)", false),
            new SysconProcess((short)SYSCON_PROCESS.ERASE_PARTIAL, "Erase Partial Syscon Flash", false),
            new SysconProcess((short)SYSCON_PROCESS.ERASE_NVS_SNVS, "Erase Syscon NVS/SNVS Only", false),
            new SysconProcess((short)SYSCON_PROCESS.WRITE_FULL, "Write Full Syscon Flash", false),
            new SysconProcess((short)SYSCON_PROCESS.WRITE_PARTIAL, "Write Partial Syscon Flash", false),
            new SysconProcess((short)SYSCON_PROCESS.WRITE_NVS_SNVS, "Write Syscon NVS/SNVS Only", true),
            new SysconProcess((short)SYSCON_PROCESS.ENABLE_DEBUG_MODE, "Enable Syscon Debug Mode (Rewrite Boot0 Blocks)", true),
        };

        public enum SYSCON_COMMANDS
        {
            SYSCON_CMD_PING = 0,        // Check if Syscon tool connected and get its version major value
            SYSCON_CMD_INFO,            // Check if Syscon tool connected and get its version minor value + free ram size
            SYSCON_CMD_READ_BLOCK,      // Read block data
            SYSCON_CMD_READ_CHIP,       // Read full chip data
            SYSCON_CMD_ERASE_BLOCK,     // Erase block data
            SYSCON_CMD_ERASE_CHIP,      // Erase full chip data
            SYSCON_CMD_WRITE_BLOCK,     // Write block data
            SYSCON_CMD_WRITE_BLOCK_EX,  // Extended Write block data
            SYSCON_CMD_SET_DATA = 0x0A, // Set data to be written into syscon write buffer
            SYSCON_CMD_INIT = 0x10,     // Initialize syscon
            SYSCON_CMD_UNINIT = 0x20,   // Uninitialize syscon
            SYSCON_CMD_RESET = 0x80     // Reset syscon tool
        };


        // Declare the event using EventHandler<T>
        public event EventHandler<UpdateProcessEventArgs> UpdateProcessEvent;

        public bool PS4SysconToolDetect(string port, out string version, out string freeMemory)
        {
            string toolVersion = string.Empty;
            string freeMemorySize = string.Empty;
            byte[] buffer = new byte[1024];

            int recievedDataLen = 0;

            bolFinished = false;


            version = string.Empty;
            freeMemory = string.Empty;

            try
            {
                //serialPort = new SerialPort(port, 115200, Parity.None, 8, StopBits.One);
                using (SerialPortStream ps4SysconFlasher = new SerialPortStream(port, 115200, 8, RJCP.IO.Ports.Parity.None, RJCP.IO.Ports.StopBits.One))
                {
                    ps4SysconFlasher.Open();
                    ps4SysconFlasher.DiscardOutBuffer();
                    ps4SysconFlasher.DiscardInBuffer();

                    if (!ps4SysconFlasher.IsOpen)
                    {
                        version = string.Empty;
                        freeMemory = string.Empty;
                        isConnected = false;
                        return false;
                    }

                    OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "PS4 Syscon Tool Started."));

                    OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Connecting To PS4 Syscon Tool."));

                    ps4SysconFlasher.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_PING, (byte)SYSCON_COMMANDS.SYSCON_CMD_INFO }, 0, 2);   // get version command

                    while (ps4SysconFlasher.BytesToRead < 4)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    recievedDataLen = ps4SysconFlasher.Read(buffer, 0, 4);
                    if (recievedDataLen > 0)
                    {
                        OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Connected To PS4 Syscon Tool Successfuly."));

                        toolVersion = String.Format("{0:X02}.{1:X02}", buffer[0], buffer[1]);
                        freeMemorySize = String.Format("{0}", ((buffer[2] << 8) | buffer[3]));

                        isConnected = true;

                        OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, String.Format("PS4 Syscon Tool Version: {0} - Free Memory: {1} bytes.", toolVersion, freeMemorySize)));
                    }
                    else
                    {
                        isConnected = false;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, String.Format("Error While Detected PS4 Syscon Flasher: {0}{1}.", Environment.NewLine, ex.Message)));
                return false;
            }
            finally
            {                
                version = toolVersion;
                freeMemory = freeMemorySize;
            }

            return true;
        }

        public bool PS4SysconToolInit(SerialPortStream ps4SysconFlasher) 
        {
            try {
                OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Trying to Enter PS4 Syscon OCD Mode."));

                // send enter debug mode command
                ps4SysconFlasher.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_INIT }, 0, 1);

                while (ps4SysconFlasher.BytesToRead < 1)
                {
                    System.Threading.Thread.Sleep(10);
                }

                response = ps4SysconFlasher.ReadByte();
                if (response == 0x00)
                {
                    isDebugMode = true;
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Entered PS4 Syscon OCD Mode Successfully."));
                }
                else
                {
                    isDebugMode = false;
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Error Entering PS4 Syscon OCD Mode."));
                }
            } 
            catch (Exception ex)
            {
                isDebugMode = false;
            }

            return isDebugMode;
        }

        public bool PS4SysconToolReset(SerialPortStream ps4SysconFlasher)
        {
            try
            {
                OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Resetting PS4 Syscon Flasher."));

                // send enter debug mode command
                ps4SysconFlasher.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_RESET }, 0, 1);

            }
            catch (Exception ex)
            {
                return false;
            }

            //ps4SysconFlasher.Close();
            isConnected = false;
            isDebugMode = false;

            return true;
        }

        public int PS4SysconToolDisconnect(SerialPortStream ps4SysconFlasher, string port, out bool debugMode) {
            int iRet = -1;
            bool isDebugMode = false;
            byte[] buffer = new byte[1024];

            bolFinished = false;

            if (!isConnected)
            {
                debugMode = false;
                return 0;
            }

            try
            {
                //stopWatch = new Stopwatch();

                OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Disconnecting from PS4 Syscon Tool."));

                if (!ps4SysconFlasher.IsOpen)
                {
                    debugMode = false;
                    isConnected = false;
                    return 0;
                }

                ps4SysconFlasher.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_UNINIT }, 0, 1);   // uninit command

                //stopWatch.Reset();

                //stopWatch.Start();

                while (ps4SysconFlasher.BytesToRead < 1)
                {
                    System.Threading.Thread.Sleep(10);
                }

                response = ps4SysconFlasher.ReadByte();
                if (response == 0x00)
                {
                    isDebugMode = false;
                    isConnected = false;
                    bolFinished = true;
                    iRet = 0;
                    ps4SysconFlasher.Close();
                }
                else
                {
                    iRet = -1;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                debugMode = isDebugMode;
            }

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, (!debugMode) ? "Disconnected From PS4 Syscon Tool Successfully." : "Failed to Disconnect From PS4 Syscon Tool."));

            return iRet;
        }

        public bool PS4SysconToolDisconnect(SerialPortStream ps4SysconFlasher)
        {
            bool bolResult = false;

            if (!isConnected)
            {
                return true;
            }

            try
            {
                //stopWatch = new Stopwatch();

                OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Disconnecting from PS4 Syscon Tool."));

                ps4SysconFlasher.Close();

                if (!ps4SysconFlasher.IsOpen)
                {
                    bolResult = true;
                    isConnected = false;
                }
            }
                catch (Exception ex)
            {

                bolResult = false;
            }
            

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, (!bolResult) ? "Disconnected From PS4 Syscon Tool Successfully." : "Failed to Disconnect From PS4 Syscon Tool."));

            return bolResult;
        }

        public int PS4SysconToolDump(SerialPortStream ps4SysconFlasher, out byte[] buffer, int startBlock, int endBlock) {
            int iRet = -1;
            int iBlockNo = 1;
            int iNoOfBlocks = 0;
            int iReadedData = 0;
            byte[] sysconBuffer;

            if (startBlock > endBlock)
            {
                buffer = new byte[] { };
                return -2;
            }

            iNoOfBlocks = (endBlock - startBlock) + 1;

            sysconBuffer = new byte[iNoOfBlocks * SYSCON_BLOCK_SIZE];

            try
            {

                byte[] startblockNo = BitConverter.GetBytes((short)startBlock);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(startblockNo);
                }

                byte[] endblockNo = BitConverter.GetBytes((short)endBlock);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(endblockNo);
                }

                if (!ps4SysconFlasher.IsOpen || !isConnected)
                {
                    buffer = new byte[] { };
                    return -2;
                }

                ps4SysconFlasher.DiscardInBuffer();
                ps4SysconFlasher.DiscardOutBuffer();

                // write syscon read block command 0x04
                ps4SysconFlasher.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_READ_BLOCK, startblockNo[0], startblockNo[1], endblockNo[0], endblockNo[1] }, 0, 5);

                for (int i = startBlock; i <= endBlock; i++)
                {
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(iBlockNo, String.Format("Dumping Block No: {0:D03} At Address: 0x{1:X06}.", i, i * SYSCON_BLOCK_SIZE)));

                    while (ps4SysconFlasher.BytesToRead < SYSCON_BLOCK_SIZE)
                    {
                    }


                    iRet = ps4SysconFlasher.Read(sysconBuffer, iReadedData, SYSCON_BLOCK_SIZE);
                    if (iRet != SYSCON_BLOCK_SIZE)
                    {
                        buffer = new byte[] { };
                        return -3;
                    }

                    iBlockNo++;
                    iReadedData += SYSCON_BLOCK_SIZE;
                }

                if (iReadedData == sysconBuffer.Length)
                {
                    buffer = new byte[sysconBuffer.Length];

                    Array.Copy(sysconBuffer, buffer, sysconBuffer.Length);

                    iRet = 0;

                    OnUpdateProcessEvent(new UpdateProcessEventArgs((int)iNoOfBlocks, "Dump Syscon Firmware Process Finished.."));

                }
                else
                {
                    buffer = new byte[] { };
                    return -4;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return iRet;
        }

        public int PS4SysconToolDump(SerialPortStream ps4SysconFlasher, string filePath, int startBlock, int endBlock) {
            int iRet = -1;
            int iNoOfBlocks = 0;
            byte[] sysconBuffer;

            if (startBlock > endBlock)
            {
                return -2;
            }

            iNoOfBlocks = (endBlock - startBlock) + 1;

            sysconBuffer = new byte[iNoOfBlocks * SYSCON_BLOCK_SIZE];

            try
            {

                iRet = PS4SysconToolDump(ps4SysconFlasher, out sysconBuffer, startBlock, endBlock);
                if (iRet == 0) {
                    using (FileStream dumpFile = new FileStream(filePath, FileMode.OpenOrCreate))
                    {
                        using (BinaryWriter dumpFileWriter = new BinaryWriter(dumpFile))
                        {
                            dumpFileWriter.Write(sysconBuffer);
                            dumpFileWriter.Flush();
                            dumpFileWriter.Close();
                        }

                        dumpFile.Close();

                        //OnUpdateProcessEvent(new UpdateProcessEventArgs((int)iNoOfBlocks, "Dump Syscon Firmware Process Finished.."));
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return iRet;
        }

        public int PS4SysconToolFullDump(SerialPortStream ps4SysconFlasher, string filePath)
        {
            int iRet = -1;
            int iReadedData = 0;
            int iBlockNo = 0;
            byte[] sysconFWBuffer = new byte[SYSCON_FLASH_SIZE];

            if (String.IsNullOrEmpty(filePath))
            {
                return -1;
            }

            try
            {
                if (!ps4SysconFlasher.IsOpen || !isConnected)
                {
                    return -2;
                }

                stopWatch = new Stopwatch();

                ps4SysconFlasher.DiscardInBuffer();
                ps4SysconFlasher.DiscardOutBuffer();

                // write syscon full dump command 0x03
                ps4SysconFlasher.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_READ_CHIP }, 0, 1);

                stopWatch.Reset();

                stopWatch.Start();

                iBlockNo = 0;

                while (iReadedData < SYSCON_FLASH_SIZE)
                {

                    OnUpdateProcessEvent(new UpdateProcessEventArgs(iReadedData, String.Format("Dumping Block No: {0:D03} At Address: 0x{1:X06}.", iBlockNo, iReadedData)));

                    for (int i = 0; i < SYSCON_BLOCK_SIZE; i += SYSCON_BUFFER_SIZE)
                    {
                        iRet = ps4SysconFlasher.Read(sysconFWBuffer, iReadedData, SYSCON_BUFFER_SIZE);
                        if (iRet != SYSCON_BUFFER_SIZE)
                        {
                            return -3;
                        }

                        iReadedData += SYSCON_BUFFER_SIZE;
                        
                    }

                    //while ((ps4SysconFlasher.BytesToRead < SYSCON_BLOCK_SIZE))
                    //{
                    //    //System.Threading.Thread.Sleep(100);
                    //}


                    //iRet = ps4SysconFlasher.Read(sysconFWBuffer, iReadedData, SYSCON_BLOCK_SIZE);
                    //if (iRet != SYSCON_BLOCK_SIZE)
                    //{
                    //    return -3;
                    //}

                    // iReadedData += SYSCON_BLOCK_SIZE;

                    iBlockNo++;
                }

                if (iReadedData == SYSCON_FLASH_SIZE)
                {
                    using (FileStream dumpFile = new FileStream(filePath, FileMode.OpenOrCreate))
                    {
                        using (BinaryWriter dumpFileWriter = new BinaryWriter(dumpFile))
                        {
                            dumpFileWriter.Write(sysconFWBuffer);
                            sysconFWBuffer = new byte[SYSCON_FLASH_SIZE];
                            dumpFileWriter.Flush();
                            dumpFileWriter.Close();
                        }

                        dumpFile.Close();

                        OnUpdateProcessEvent(new UpdateProcessEventArgs((int)SYSCON_FLASH_SIZE, "Dumping Syscon Firmware Process Finished Successfully.."));
                    }

                    iRet = 0;

                }

            }
            catch (Exception ex)
            {
                iRet = -1;
                throw ex;
            }

            return iRet;
        }

        public int PS4SysconToolNVSSNVSDump(SerialPortStream ps4SysconFlasher, string filePath) {
            return PS4SysconToolDump(ps4SysconFlasher, filePath, SYSCON_NVS_SNVS_START_BLOCK, SYSCON_NVS_SNVS_END_BLOCK);
        }

        public int PS4SysconToolErase(SerialPortStream ps4SysconFlasher, int startBlock, int endBlock)
        {
            int iRet = -1;
            int iBlockNo = 0;
            int iNoOfBlocks = 0;

            if (startBlock > endBlock)
            {
                return -2;
            }

            iNoOfBlocks = (endBlock - startBlock) + 1;

            try
            {
                byte[] startblockNo = BitConverter.GetBytes((short)startBlock);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(startblockNo);
                }

                byte[] endblockNo = BitConverter.GetBytes((short)endBlock);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(endblockNo);
                }

                if (!ps4SysconFlasher.IsOpen || !isConnected)
                {
                    return -2;
                }

                ps4SysconFlasher.DiscardInBuffer();
                ps4SysconFlasher.DiscardOutBuffer();

                // write syscon erase block command 0x04
                ps4SysconFlasher.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_ERASE_BLOCK, startblockNo[0], startblockNo[1], endblockNo[0], endblockNo[1] }, 0, 5);

                for (int i = 0; i < iNoOfBlocks; i++)
                {
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(iBlockNo, String.Format("Erasing Block No: {0:D03} At Address: 0x{1:X06}.", (startBlock + i), (startBlock + i) * SYSCON_BLOCK_SIZE)));

                    while (ps4SysconFlasher.BytesToRead < 1)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    response = ps4SysconFlasher.ReadByte();
                    if (response != SYSCON_OK)
                    {
                        return -4;
                    }

                    iBlockNo++;
                }

                OnUpdateProcessEvent(new UpdateProcessEventArgs((int)iNoOfBlocks, "Erase Partial Syscon FW Finished.."));
                iRet = 0;

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return iRet;
        }

        public int PS4SysconToolFullErase(SerialPortStream ps4SysconFlasher)
        {
            return PS4SysconToolErase(ps4SysconFlasher, SYSCON_FLASH_START_BLOCK, SYSCON_FLASH_END_BLOCK);
        }

        public int PS4SysconToolWrite(SerialPortStream ps4SysconFlasher, byte[] buffer, int startBlock, int endBlock, bool extendedMode = false)
        {
            int iRet = -1;
            int iWrittenData = 0;
            int iBlockNo = 0;
            int iNoOfBlocks = 0;
            int iCounter = 0;
            int iWriteDataLen = 0;

            if (buffer == null)
            {
                return -1;
            }

            if (buffer.Length <= 0 || ((buffer.Length % SYSCON_BLOCK_SIZE) != 0))
            {
                return -1;
            }

            if (startBlock > endBlock)
            {
                return -2;
            }

            iNoOfBlocks = (endBlock - startBlock) + 1;

            iWriteDataLen = iNoOfBlocks * SYSCON_BLOCK_SIZE;

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Writting PS4 Syscon Firmware Process Started."));

            try
            {
                if (!ps4SysconFlasher.IsOpen || !isConnected)
                {
                    return -2;
                }

                iCounter = 1;
                iWrittenData = 0;

                for (iBlockNo = startBlock; iBlockNo <= endBlock; iBlockNo++)
                {

                    ps4SysconFlasher.DiscardInBuffer();
                    ps4SysconFlasher.DiscardOutBuffer();

                    byte[] blockNo = BitConverter.GetBytes((short)iBlockNo);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(blockNo);
                    }

                    OnUpdateProcessEvent(new UpdateProcessEventArgs(iCounter, String.Format("Writting Block No: {0:D03} At Address: 0x{1:X06}.", iBlockNo, iBlockNo * SYSCON_BLOCK_SIZE)));

                    if (extendedMode)
                    {
                        ps4SysconFlasher.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_WRITE_BLOCK_EX, blockNo[0], blockNo[1] }, 0, 3); // syscon write command
                    }
                    else
                    {
                        ps4SysconFlasher.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_WRITE_BLOCK, blockNo[0], blockNo[1] }, 0, 3); // syscon write command
                    }

                    
                    ps4SysconFlasher.Write(buffer, iWrittenData, SYSCON_BLOCK_SIZE);


                    while (ps4SysconFlasher.BytesToRead < 1)
                    {
                        //int x  = ps4SysconFlasher.BytesToRead;
                    }

                    bRet = (byte)ps4SysconFlasher.ReadByte();
                    if (bRet != 0)
                    {
                        return iRet;
                    }

                    iWrittenData += SYSCON_BLOCK_SIZE;
                    iCounter++;
                }

                OnUpdateProcessEvent(new UpdateProcessEventArgs(iNoOfBlocks, "Writting Syscon Firmware Process Finished Successfully!."));
                iRet = 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return iRet;
        }

        public int PS4SysconToolWrite(SerialPortStream ps4SysconFlasher, string filePath, int startBlock, int endBlock, bool extendedMode = false)
        {
            int iRet = -1;
            int iNoOfBlocks = 0;
            int iWriteDataLen = 0;
            byte[] sysconFWBuffer;

            if (String.IsNullOrEmpty(filePath))
            {
                return -1;
            }

            if (!File.Exists(filePath))
            {
                return -2;
            }

            if (startBlock > endBlock)
            {
                return -2;
            }

            iNoOfBlocks = (endBlock - startBlock) + 1;

            iWriteDataLen = iNoOfBlocks * SYSCON_BLOCK_SIZE;

            sysconFWBuffer = new byte[iWriteDataLen];

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Writting PS4 Syscon Firmware Process Started."));

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Loading PS4 Syscon Firmware File Into Memory."));

            using (FileStream fwFile = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader fwFileReader = new BinaryReader(fwFile))
                {
                    int startBlockAddress = startBlock * SYSCON_BLOCK_SIZE;
                    fwFile.Seek(startBlockAddress, SeekOrigin.Begin);
                    sysconFWBuffer = fwFileReader.ReadBytes(iWriteDataLen);
                    fwFileReader.Close();
                }

                fwFile.Close();
            }

            try
            {
                iRet = PS4SysconToolWrite(ps4SysconFlasher, sysconFWBuffer, startBlock, endBlock, extendedMode);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return iRet;
        }

        public int PS4SysconToolFullWrite(SerialPortStream ps4SysconFlasher, string filePath, bool extendedMode = false)
        {
            return PS4SysconToolWrite(ps4SysconFlasher, filePath, SYSCON_FLASH_START_BLOCK, SYSCON_FLASH_END_BLOCK, extendedMode);
        }
       
        public int PS4SysconToolEnableDebugMode(SerialPortStream ps4SysconFlasher) {
            int iRet = SYSCON_OK;
            byte[] sysconBoot0Buffer = new byte[SYSCON_BLOCK_SIZE];

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Enabling Syscon Debug Mode Process Started.."));
            OnUpdateProcessEvent(new UpdateProcessEventArgs(1, "Dumping Syscon Boot0 Blocks.."));

            iRet = PS4SysconToolDump(ps4SysconFlasher, out sysconBoot0Buffer, SYSCON_FLASH_START_BLOCK, SYSCON_FLASH_START_BLOCK + 1);
            if (iRet != 0) 
            {
                return iRet;
            }

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Checking Syscon Boot0 Block If Debug Mode Already Enabled.."));

            if (sysconBoot0Buffer[SYSCON_DEBUG_SETTING_OFFSET] == 0x04)
            {
                OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Enabling Syscon Debug Mode.."));
                sysconBoot0Buffer[SYSCON_DEBUG_SETTING_OFFSET] = 0x85;
            } else if ((sysconBoot0Buffer[SYSCON_DEBUG_SETTING_OFFSET] == 0x84) || (sysconBoot0Buffer[SYSCON_DEBUG_SETTING_OFFSET] == 0x85)) {
              OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Syscon Debug Mode Already Enabled.."));
              return -10;
            }     

            OnUpdateProcessEvent(new UpdateProcessEventArgs(2, "Writing Syscon Boot0 Blocks.."));
            iRet = PS4SysconToolWrite(ps4SysconFlasher, sysconBoot0Buffer, SYSCON_FLASH_START_BLOCK, SYSCON_FLASH_START_BLOCK, true);
            if (iRet != 0)
            {
                return iRet;
            }
            OnUpdateProcessEvent(new UpdateProcessEventArgs(2, "Syscon Boot0 Block Written Successfully.."));

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Enabling Syscon Debug Mode Process Finished Successfully!!!.."));

            return iRet;
        }

        public PS4SysconInfo.PS4SysconFWInfo PS4SysconToolGetFWInfo(byte[] buffer)
        {
            PS4SysconInfo.PS4SysconFWInfo ps4SysconFwInfo = null;
            byte[] sysconFWBuffer;

            if (buffer == null || buffer.Length < PS4SysconTool.SYSCON_FW_SIZE) {
                return null;
            }

            sysconFWBuffer = new byte[PS4SysconTool.SYSCON_FW_SIZE];

            Array.Copy(buffer, sysconFWBuffer, PS4SysconTool.SYSCON_FW_SIZE);

            ps4SysconFwInfo = PS4SysconInfo.GetPS4SysconFWInfo(sysconFWBuffer);
            
            return ps4SysconFwInfo;
        }

        public int PS4SysconToolGetFWInfo(string filePath) {
            int iRet = -1;
            byte[] sysconFWBuffer;

            if (String.IsNullOrEmpty(filePath))
            {
                return -1;
            }

            if (!File.Exists(filePath))
            {
                return -2;
            }

            sysconFWBuffer = new byte[PS4SysconTool.SYSCON_FW_SIZE];

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Getting PS4 Syscon Dump Info:"));

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Loading PS4 Syscon Firmware File Into Memory."));

            using (FileStream fwFile = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader fwFileReader = new BinaryReader(fwFile))
                {
                    sysconFWBuffer = fwFileReader.ReadBytes(sysconFWBuffer.Length);

                    fwFileReader.Close();
                }

                fwFile.Close();
            }

            try
            {
                PS4SysconInfo.PS4SysconFWInfo ps4SysconInfo = PS4SysconInfo.GetPS4SysconFWInfo(sysconFWBuffer);
                if (ps4SysconInfo != null)
                {
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "PS4 Firmware Dump Info:"));
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, String.Format("Firmware Version: {0}", ps4SysconInfo.version)));
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, String.Format("Firmware Hash:    {0}", ps4SysconInfo.hash)));
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, String.Format("Firmware Magic:   {0}", (ps4SysconInfo.magic == true? "True" : "False"))));
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(1,  String.Format("Debug Mode:       {0}", (ps4SysconInfo.debugMode == PS4SysconInfo.SYSCON_DEBUG_MODES.NONE ? "Disabled" : "Enabled"))));
                    iRet = 0;
                }
                else
                {
                    iRet = -3;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return iRet;

        }

        protected virtual void OnUpdateProcessEvent(UpdateProcessEventArgs e)
        {
            UpdateProcessEvent?.Invoke(this, e);
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
//using System.IO.Ports;
using RJCP.IO.Ports;

namespace PS4_Syscon_Tools
{
    class PS4SysconTool
    {

        public enum SYSCON_PROCESS {
            NONE = -1,
            DUMP_FULL = 0,          // Dump Full Syscon Flash
            DUMP_PARTIAL,           // Dump Partial Syscon Flash
            ERASE_FULL,             // Erase Full Syscon Flash (Danger)
            ERASE_EXCEPT_BOOT0,     // Erase Full Syscon Flash Expect Boot0 Block (Safe)
            ERASE_PARTIAL,          // Erase Partial Syscon Flash
            WRITE_FULL,             // Write Full Syscon Flash
            WRITE_PARTIAL,          // Write Partial Syscon Flash
            WRITE_NVS_SNVS          // Write Syscon NVS/SNVS Only
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
            SYSCON_CMD_SET_DATA = 0x0A, // Set data to be written into syscon write buffer
            SYSCON_CMD_INIT = 0x10,     // Initialize syscon
            SYSCON_CMD_UNINIT = 0x20,   // Uninitialize syscon
            SYSCON_CMD_RESET = 0x80     // Reset syscon tool
        };

        public const int SYSCON_BLOCK_SIZE = 0x400;
        public const int SYSCON_BUFFER_SIZE = 0x40;
        public const int SYSCON_WRITE_BUFFER_SIZE = 0x100;
        public const int SYSCON_SERIAL_BUFFER_SIZE = 0x400;
        public const int SYSCON_BLOCKS_NO = 0x200;
        public const long SYSCON_FULL_DUMP_SIZE = SYSCON_BLOCK_SIZE * SYSCON_BLOCKS_NO;
        public const int SYSCON_BOOT0_BLOCK = 3;

        public const int SYSCON_OK = 0x00;
        public const int SYSCON_WRITE_OK = 0x40;
        public const int SYSCON_CMD_OK = 0x55;
        public const int SYSCON_ERR_INT = 0xF0;
        public const int SYSCON_ERR_READ = 0xF1;
        public const int SYSCON_ERR_ERSASE = 0xF4;
        public const int SYSCON_ERR_WRITE = 0xF6;
        public const int SYSCON_ERR_CMD_LEN = 0xFA;
        public const int SYSCON_ERR_CMD_EXEC = 0xFE;
        public const int SYSCON_ERR = 0xFF;

        private SerialPortStream serialPort;
        //private SerialPort serialPort;
        private static bool bolFinished;
        private static bool bolReceiveError;
        private static long lTimeout = 180000; //180000;
        private Stopwatch stopWatch;
        private bool isConnected = false;
        int response = 0;
        byte bRet;

        // Declare the event using EventHandler<T>
        public event EventHandler<UpdateProcessEventArgs> UpdateProcessEvent;

        public int PS4SysconToolConnect(string port, out string version, out string freeMemory, out bool debugMode) {
            int iRet = -1;
            string toolVersion = string.Empty;
            string freeMemorySize = string.Empty;
            bool isDebugMode = false;
            byte[] buffer = new byte[1024];
            
            int recievedDataLen = 0;

            bolFinished = false;


            version = string.Empty;
            freeMemory = string.Empty;

            if (isConnected && isDebugMode) {
                debugMode = true;
                return 0;
            }

            try
            {
                stopWatch = new Stopwatch();

                serialPort = new SerialPortStream(port, 115200, 8, RJCP.IO.Ports.Parity.None, RJCP.IO.Ports.StopBits.One);
                //serialPort = new SerialPort(port, 115200, Parity.None, 8, StopBits.One);

                serialPort.Open();
                serialPort.DiscardOutBuffer();
                serialPort.DiscardInBuffer();

                if (!serialPort.IsOpen)
                {
                    version = string.Empty;
                    freeMemory = string.Empty;
                    debugMode = false;
                    isConnected = false;
                    return -1;
                }

                OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "PS4 Syscon Tool Started."));

                OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Connecting To PS4 Syscon Tool."));

                serialPort.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_PING, (byte)SYSCON_COMMANDS.SYSCON_CMD_INFO }, 0, 2);   // get version command

                stopWatch.Reset();

                stopWatch.Start();

                while (serialPort.BytesToRead < 4)
                {
                    System.Threading.Thread.Sleep(10);
                } 

                recievedDataLen = serialPort.Read(buffer, 0, 4);
                if (recievedDataLen > 0)
                {
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Connected To PS4 Syscon Tool Successfuly."));

                    toolVersion = String.Format("{0:X02}.{1:X02}", buffer[0], buffer[1]);
                    freeMemorySize = String.Format("{0}", ((buffer[2] << 8) | buffer[3]));

                    OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, String.Format("PS4 Syscon Tool Version: {0} - Free Memory: {1} bytes.", toolVersion, freeMemorySize)));

                    OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Trying to Enter PS4 Syscon Debug Mode."));

                    // send enter debug mode command
                    serialPort.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_INIT }, 0, 1);

                    while (serialPort.BytesToRead < 1)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    response = serialPort.ReadByte();
                    if (response == 0x00)
                    {
                        isDebugMode = true;
                        isConnected = true;
                        bolFinished = true;
                        iRet = 0;
                    }
                    else
                    {
                        isDebugMode = false;
                        isConnected = false;
                        bolFinished = true;
                        iRet = -1;
                        serialPort.Close();
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                //serialPort.Close();
                version = toolVersion;
                freeMemory = freeMemorySize;
                debugMode = isDebugMode;
            }

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, (debugMode) ?  "Entered PS4 Syscon Debug Mode Successfully." : "Failed to Enter PS4 Syscon Debug Mode."));

            return iRet;
        }

        public int PS4SysconToolDisconnect(string port, out bool debugMode) {
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
                stopWatch = new Stopwatch();

                if (!serialPort.IsOpen)
                {
                    debugMode = false;
                    isConnected = false;
                    return 0;
                }

                OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Disconnecting from PS4 Syscon Tool."));

                serialPort.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_UNINIT }, 0, 1);   // uninit command

                stopWatch.Reset();

                stopWatch.Start();

                while (serialPort.BytesToRead < 1)
                {
                    System.Threading.Thread.Sleep(10);
                }

                response = serialPort.ReadByte();
                if (response == 0x00)
                {
                    isDebugMode = false;
                    isConnected = false;
                    bolFinished = true;
                    iRet = 0;
                    serialPort.Close();
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

        public int PS4SysconToolFullDump(string filePath) {
            int iRet = -1;
            int iReadedData = 0;
            int iBlockNo = 0;
            byte[] sysconFWBuffer = new byte[SYSCON_FULL_DUMP_SIZE];

            if (String.IsNullOrEmpty(filePath)) {
                return -1;
            }

            try
            {
                if (!serialPort.IsOpen || !isConnected) {
                    return -2;
                }

                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();

                // write syscon full dump command 0x03
                serialPort.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_READ_CHIP }, 0, 1);

                iBlockNo = 0;

                while (iReadedData < SYSCON_FULL_DUMP_SIZE)
                {

                    OnUpdateProcessEvent(new UpdateProcessEventArgs(iReadedData, String.Format("Dumping Block No: {0:D03} At Address: 0x{1:X06}.", iBlockNo, iReadedData)));

                    while (serialPort.BytesToRead < SYSCON_BLOCK_SIZE) {
                    }


                    iRet = serialPort.Read(sysconFWBuffer, iReadedData, SYSCON_BLOCK_SIZE);
                    if (iRet != SYSCON_BLOCK_SIZE)
                    {
                        return -3;
                    }

                    iReadedData += SYSCON_BLOCK_SIZE;

                    iBlockNo++;
                }

                if (iReadedData == SYSCON_FULL_DUMP_SIZE) {
                    using (FileStream dumpFile = new FileStream(filePath, FileMode.OpenOrCreate))
                    {
                        using (BinaryWriter dumpFileWriter = new BinaryWriter(dumpFile))
                        {
                            dumpFileWriter.Write(sysconFWBuffer);
                            sysconFWBuffer = new byte[SYSCON_FULL_DUMP_SIZE];
                            dumpFileWriter.Flush();
                            dumpFileWriter.Close();
                        }

                        dumpFile.Close();

                        OnUpdateProcessEvent(new UpdateProcessEventArgs((int)SYSCON_FULL_DUMP_SIZE, "Dump Syscon FW Finsihed.."));
                    }

                    iRet = 0;

                }

            }
            catch (Exception ex)
            {
                iRet = -1;
                throw;
            }

            return iRet;
        }

        public int PS4SysconToolPartialDump(string filePath, int startBlock, int endBlock) {
            int iRet = -1;
            int iBlockNo = 0;
            int iNoOfBlocks = 0;
            int iReadedData = 0;
            byte[] sysconBuffer;

            if (startBlock > endBlock)
            {
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

                if (!serialPort.IsOpen || !isConnected)
                {
                    return -2;
                }

                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();

                // write syscon erase block command 0x04
                serialPort.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_READ_BLOCK, startblockNo[0], startblockNo[1], endblockNo[0], endblockNo[1] }, 0, 5);

                for (int i = startBlock; i <= endBlock; i++)
                {
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(iBlockNo, String.Format("Dumping Block No: {0:D03} At Address: 0x{1:X06}.", i, i * SYSCON_BLOCK_SIZE)));

                    while (serialPort.BytesToRead < SYSCON_BLOCK_SIZE)
                    {
                    }


                    iRet = serialPort.Read(sysconBuffer, iReadedData, SYSCON_BLOCK_SIZE);
                    if (iRet != SYSCON_BLOCK_SIZE)
                    {
                        return -3;
                    }

                    iBlockNo++;
                    iReadedData += SYSCON_BLOCK_SIZE;
                }

                if (iReadedData == sysconBuffer.Length)
                {
                    using (FileStream dumpFile = new FileStream(filePath, FileMode.OpenOrCreate))
                    {
                        using (BinaryWriter dumpFileWriter = new BinaryWriter(dumpFile))
                        {
                            dumpFileWriter.Write(sysconBuffer);
                            dumpFileWriter.Flush();
                            dumpFileWriter.Close();
                        }

                        dumpFile.Close();

                        OnUpdateProcessEvent(new UpdateProcessEventArgs((int)iNoOfBlocks, "Dump Partial Syscon FW Finsihed.."));
                    }

                    iRet = 0;

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return iRet;
        }

        public int PS4SysconToolPartialErase(int startBlock, int endBlock)
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

                if (!serialPort.IsOpen || !isConnected)
                {
                    return -2;
                }

                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();

                // write syscon erase block command 0x04
                serialPort.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_ERASE_BLOCK, startblockNo[0], startblockNo[1], endblockNo[0], endblockNo[1] }, 0, 5);

                for (int i = 0; i < iNoOfBlocks; i++)
                {
                    OnUpdateProcessEvent(new UpdateProcessEventArgs(iBlockNo, String.Format("Erasing Block No: {0:D03} At Address: 0x{1:X06}.", (startBlock + i), (startBlock + i) * SYSCON_BLOCK_SIZE)));

                    while (serialPort.BytesToRead < 1)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    response = serialPort.ReadByte();
                    if (response != SYSCON_OK)
                    {
                        return -4;
                    }

                    iBlockNo++;
                }

                OnUpdateProcessEvent(new UpdateProcessEventArgs((int)iNoOfBlocks, "Erase Partial Syscon FW Finsihed.."));
                iRet = 0;

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return iRet;
        }

        public int PS4SysconToolFullWrite(string filePath) {
            int iRet = -1;
            int iWrittenData = 0;
            int iBlockNo = 0;
            byte[] sysconFWBuffer = new byte[SYSCON_FULL_DUMP_SIZE];

            if (String.IsNullOrEmpty(filePath))
            {
                return -1;
            }

            if (!File.Exists(filePath))
            {
                return -2;
            }

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Writting Syscon Full FW Process Started."));

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Loading PS4 Syscon FW File Into Memory."));

            using (FileStream fwFile = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader fwFileReader = new BinaryReader(fwFile))
                {
                    sysconFWBuffer = fwFileReader.ReadBytes((int)SYSCON_FULL_DUMP_SIZE);

                    fwFileReader.Close();
                }

                fwFile.Close();
            }

            try
            {
                if (!serialPort.IsOpen || !isConnected)
                {
                    return -2;
                }

                while (iWrittenData < SYSCON_FULL_DUMP_SIZE)
                {

                    OnUpdateProcessEvent(new UpdateProcessEventArgs(iBlockNo, String.Format("Writting Block No: {0:D03} At Address: 0x{1:X06}.", iBlockNo, iBlockNo * SYSCON_BLOCK_SIZE)));

                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();

                    byte[] blockNo = BitConverter.GetBytes((short)iBlockNo);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(blockNo);
                    }

                    byte[] writeCmd = new byte[3 + SYSCON_WRITE_BUFFER_SIZE];

                    writeCmd[0] = 0x06;
                    writeCmd[1] = blockNo[0];
                    writeCmd[2] = blockNo[1];

                    Array.Copy(sysconFWBuffer, iWrittenData, writeCmd, 3, SYSCON_WRITE_BUFFER_SIZE);

                    serialPort.Write(writeCmd, 0, writeCmd.Length); // write syscon write block command 0x06 1st half of block data

                    while (serialPort.BytesToRead < 1)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    response = serialPort.ReadByte();
                    if (response != 0x00)
                    {
                        return -3;
                    }

                    iWrittenData += SYSCON_WRITE_BUFFER_SIZE;

                    Array.Copy(sysconFWBuffer, iWrittenData, writeCmd, 3, SYSCON_WRITE_BUFFER_SIZE);

                    serialPort.Write(writeCmd, 0, writeCmd.Length); // write syscon write block command 0x06 2nd half of block data

                    while (serialPort.BytesToRead < 1)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    response = serialPort.ReadByte();
                    if (response != 0x00)
                    {
                        return -4;
                    }

                    iWrittenData += SYSCON_WRITE_BUFFER_SIZE;

                    iBlockNo++;
                }

                OnUpdateProcessEvent(new UpdateProcessEventArgs((int)SYSCON_BLOCKS_NO, "Writting Syscon Full FW Finsihed Successfully!."));
                iRet = 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return iRet;
        }

        public int PS4SysconToolPartialWrite(string filePath, int startBlock, int endBlock)
        {
            int iRet = -1;
            int iWrittenData = 0;
            int iBlockNo = 0;
            int iNoOfBlocks = 0;
            int iCounter = 0;
            int iOffset = 0;
            int iWriteDataLen = 0;
            int iDataLen = 0;
            byte[] sysconFWBuffer;
            byte[] setDataCmd;

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

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Writting Syscon Full FW Process Started."));

            OnUpdateProcessEvent(new UpdateProcessEventArgs(-1, "Loading PS4 Syscon FW File Into Memory."));

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
                if (!serialPort.IsOpen || !isConnected)
                {
                    return -2;
                }

                iCounter = 1;
                iWrittenData = 0;

                for (iBlockNo = startBlock; iBlockNo <= endBlock; iBlockNo++)
                {

                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();

                    byte[] blockNo = BitConverter.GetBytes((short)iBlockNo);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(blockNo);
                    }

                    OnUpdateProcessEvent(new UpdateProcessEventArgs(iCounter, String.Format("Writting Block No: {0:D03} At Address: 0x{1:X06}.", iBlockNo, iBlockNo * SYSCON_BLOCK_SIZE)));


                    serialPort.Write(new byte[] { (byte)SYSCON_COMMANDS.SYSCON_CMD_WRITE_BLOCK, blockNo[0], blockNo[1] }, 0, 3); // syscon write command

                    iDataLen = 0;

                    //System.Threading.Thread.Sleep(1);
                    //System.Threading.Thread.Sleep(new TimeSpan(5000));

                    serialPort.Write(sysconFWBuffer, iWrittenData, SYSCON_BLOCK_SIZE);

                    //while (iDataLen < SYSCON_BLOCK_SIZE)    // SYSCON_BLOCK_SIZE
                    //{
                    //    serialPort.Write(sysconFWBuffer, iWrittenData + iDataLen, SYSCON_SERIAL_BUFFER_SIZE);
                    //    //serialPort.Write(setDataCmd, 2 + iDataLen, SYSCON_SERIAL_BUFFER_SIZE); // write syscon write block command 0x06 1st half of block data
                    //    iDataLen += SYSCON_SERIAL_BUFFER_SIZE;

                    //    //serialPort.BaseStream.Flush();
                    //    System.Threading.Thread.Sleep(new TimeSpan(5000));
                    //}

                    ////serialPort.Flush();


                    while (serialPort.BytesToRead < 1) {
                       int x  = serialPort.BytesToRead;
                    }

                    bRet = (byte)serialPort.ReadByte();
                    if (bRet != 0) {
                        return iRet;
                    }

                    iWrittenData += SYSCON_BLOCK_SIZE;
                    iCounter++;


                }

                OnUpdateProcessEvent(new UpdateProcessEventArgs(iNoOfBlocks, "Writting Syscon Partial FW Finsihed Successfully!."));
                iRet = 0;
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

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PS4_Syscon_Tools
{
    class Util
    {
        public static bool GetBlockAddress(int blockNo, out byte[] address) {
            long offset;
            byte[] addressValue = new byte[3];
            if ((blockNo < 0) || (blockNo > 511))
            {
                address = null;
                return false;
            }

            offset = blockNo * PS4SysconTool.SYSCON_BLOCK_SIZE;

            addressValue[0] = (byte)(offset & 0xFF);
            addressValue[1] = (byte)((offset >> 8) & 0xFF);
            addressValue[2] = (byte)((offset >> 16) & 0xFF);

            address = addressValue;
            return true;
        }

        public static bool IsByteArrayIdentical(byte[] buffer1, byte[] buffer2)
        {
            if (buffer1 == null || buffer2 == null) {
                return false;
            }

            if (buffer1.Length != buffer2.Length)
            {
                return false;
            }

            int lastBlockIndex = buffer1.Length - (buffer1.Length % sizeof(ulong));

            var totalProcessed = 0;
            while (totalProcessed < lastBlockIndex)
            {
                if (System.BitConverter.ToUInt64(buffer1, totalProcessed) != System.BitConverter.ToUInt64(buffer2, totalProcessed))
                {
                    return false;
                }
                totalProcessed += sizeof(ulong);
            }
            return true;
        }

        public static bool IsFilesIdentical(string file1Path, string file2Path)
        {
            byte[] buffer1;
            byte[] buffer2;

            if (!File.Exists(file1Path) || !File.Exists(file2Path))
            {
                return false;
            }

            buffer1 = File.ReadAllBytes(file1Path);
            buffer2 = File.ReadAllBytes(file2Path);

            return IsByteArrayIdentical(buffer1, buffer2);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static int LoadFile(string filePath, int startBlock, int endBlock, out byte[] buffer) {
            int iNoOfBlocks = 0;

            if (String.IsNullOrEmpty(filePath))
            {
                buffer = null;
                return -1;
            }

            if (!File.Exists(filePath))
            {
                buffer = null;
                return -2;
            }

            if (startBlock > endBlock)
            {
                buffer = null;
                return -3;
            }

            iNoOfBlocks = (endBlock - startBlock) + 1;

            buffer = new byte[iNoOfBlocks * PS4SysconTool.SYSCON_BLOCK_SIZE];

            try
            {
                using (FileStream fwFile = new FileStream(filePath, FileMode.Open))
                {
                    using (BinaryReader fwFileReader = new BinaryReader(fwFile))
                    {
                        int startBlockAddress = startBlock * PS4SysconTool.SYSCON_BLOCK_SIZE;
                        fwFile.Seek(startBlockAddress, SeekOrigin.Begin);
                        buffer = fwFileReader.ReadBytes(buffer.Length);
                        fwFileReader.Close();
                    }

                    fwFile.Close();
                }
            }
            catch (Exception ex)
            {
                buffer = null;
                return -4;
            }
           
            return 0;
        }

        public static List<string> ComPortNames(String VID, String PID)
        {
            String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;
        }

        public static String GetPS4SysconFlasherPort() 
        {
            String comPort = String.Empty;

            return comPort;
            
        }

    }
}

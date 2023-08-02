using System;
using System.IO;

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

    }
}

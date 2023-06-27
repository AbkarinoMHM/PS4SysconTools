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

        public static bool isFilesIdentical(string file1Path, string file2Path) {
            byte[] buffer1;
            byte[] buffer2;

            if (!File.Exists(file1Path) || !File.Exists(file2Path)) {
                return false;
            }

            buffer1 = File.ReadAllBytes(file1Path);
            buffer2 = File.ReadAllBytes(file2Path);

            if (buffer1.Length != buffer2.Length) {
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

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    }
}

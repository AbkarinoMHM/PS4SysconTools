using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PS4_Syscon_Tools
{
    class PS4SysconInfo
    {
        public enum SYSCON_DEBUG_MODES {
            NONE = 0x04,
            DEBUG_84 = 0x84,
            DEBUG_85 = 0x85
        }
        public class PS4SysconFWInfo {
            public string version;
            public string hash;
            public SYSCON_DEBUG_MODES debugMode;
            public bool magic;

            public PS4SysconFWInfo(string version, string hash, SYSCON_DEBUG_MODES debugMode = SYSCON_DEBUG_MODES.NONE) {
                this.version = version;
                this.hash = hash;
                this.debugMode = debugMode;
                this.magic = true;
            }

        }

        public static PS4SysconFWInfo[] PS4SysconFWInfos = {
            new PS4SysconFWInfo("1.00", "125CD5CC2D854E5F2812F060A7031044", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("1.00", "FA6C81BDEEB77F0C29F2084BA15120BB", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("1.00", "1F302ACB86F76D8148A2697E396A332A", SYSCON_DEBUG_MODES.DEBUG_85),
            new PS4SysconFWInfo("1.00b", "BAEA46D4D5BF6D9B00B51BDA40DB0F48", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("1.00b", "4B6720789ADF1974E9A7411257AA60D2", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("1.00b", "B57B4A9F72FEAC1D212E577D6B8E8098", SYSCON_DEBUG_MODES.DEBUG_85),
            new PS4SysconFWInfo("2.06_1", "6741017A1499384DD7B07DC6DEF28D1E", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("2.06_1", "FFED342CF68B2A58713E3C46CBB4E0C5", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("2.06_1", "AAA48C7B49237A1618CF97CB2BB0F6A2", SYSCON_DEBUG_MODES.DEBUG_85),
            new PS4SysconFWInfo("2.06_2", "FA4DDDB3F17315ECC028BF725B7702B1", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("2.06_2", "E5727AC79BBE8F8246F7648024F04AFD", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("2.06_2", "827CA23CD1B853D730FB9FC6C67EE783", SYSCON_DEBUG_MODES.DEBUG_85),
            new PS4SysconFWInfo("2.13_1", "1C70248C249F0AC4F0C5555499AFA6EF", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("2.13_1", "6D5BCE956F0A64A10EBFF4FC82F3D4FC", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("2.13_1", "C2480931C397B2E8A92995751E7E07DF", SYSCON_DEBUG_MODES.DEBUG_85),
            new PS4SysconFWInfo("2.13_2", "45EBE778279CA58B6BF200FF1BD2CB9E", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("2.13_2", "AB9C039AA9E4629BA41203321A319733", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("2.13_2", "4EE8F0F7DA0B991F6EBBCF3256F355DE", SYSCON_DEBUG_MODES.DEBUG_85),
            new PS4SysconFWInfo("2.13_3", "581D42D6A6C83992521420A23F02427C", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("2.13_3", "0E1A45E21E4EF8602B66605B57621FBF", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("2.13_3", "DA0624402A72BFE44E5E6B15282B00E3", SYSCON_DEBUG_MODES.DEBUG_85),
            new PS4SysconFWInfo("2.23_1", "39A1BDD270D0DC2BDCE8D81E7525AF41", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("2.23_1", "F7B1D89356414AD81AAE20C341B46316", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("2.23_1", "7023E24401BEB142BEC7C644E5343996", SYSCON_DEBUG_MODES.DEBUG_85),
            new PS4SysconFWInfo("2.23_2", "A7D36425E5881770B2E9C4F925CED39F", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("2.23_2", "005C2C80A017B60B78FDB480D27FDF49", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("2.23_2", "F2FA41E9D693BF5A4FCF3DBD8189483E", SYSCON_DEBUG_MODES.DEBUG_85),
            new PS4SysconFWInfo("2.23_3", "C42C250BBB7B30ACD2F3960CFAD9C8E3", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("2.23_3", "1631FF85FC74312AF645D2EADF54CD9C", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("2.23_3", "F49E143B692CAF13688ED853874C7390", SYSCON_DEBUG_MODES.DEBUG_85),
            new PS4SysconFWInfo("2.23_4", "F7E0A6157FA9C04944B927051B5D4196", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("2.23_4", "10722BE25DFC93B8DBE4A40F7A87312E", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("2.23_4", "27CA52CADF19BA6A33D2DF321A1FC0B2", SYSCON_DEBUG_MODES.DEBUG_85),
            new PS4SysconFWInfo("2.26", "263BD07F5B80F64ACA8A107FEE27EE08", SYSCON_DEBUG_MODES.NONE),
            new PS4SysconFWInfo("2.26", "3E010DB66CAA7A41A376ACC1480AEAF9", SYSCON_DEBUG_MODES.DEBUG_84),
            new PS4SysconFWInfo("2.26", "62703902E60DFDCC3FDB8749433DBFEC", SYSCON_DEBUG_MODES.DEBUG_85),
        };

        public static PS4SysconFWInfo GetPS4SysconFWInfo(byte[] fwData) {
            PS4SysconFWInfo sysconFWInfo = new PS4SysconFWInfo("Unknown", "");
            if ((fwData == null) || (fwData.Length != PS4SysconTool.SYSCON_FW_SIZE)) {
                sysconFWInfo = new PS4SysconFWInfo("Unknown", "");
                return sysconFWInfo;
            }

            //check magic values 1
            if (fwData[0] != 0x80 && fwData[1] != 0x00)
            {
                sysconFWInfo.magic = false;
                return sysconFWInfo;
            }

            //check magic values 2
            if (String.Compare(BitConverter.ToString(fwData, 0xC4, 0x0A), ":Not:Used:") != 0) {
                sysconFWInfo.magic = false;
                return sysconFWInfo;
            }

            //check magic values 3
            if (String.Compare(BitConverter.ToString(fwData, 0x133, 0x20), "Sony Computer Entertainment Inc.") != 0)
            {
                sysconFWInfo.magic = false;
                return sysconFWInfo;
            }

            sysconFWInfo.magic = true;

            using (MD5 md5Creator = MD5.Create())
            {
                byte[] hash = md5Creator.ComputeHash(fwData);
                if (hash == null) {
                    return sysconFWInfo;
                }

                string hashValue = Util.ByteArrayToString(hash);

                sysconFWInfo.hash = hashValue;

                foreach (PS4SysconFWInfo fwInfo in PS4SysconFWInfos)
                {
                    if (string.Compare(hashValue, fwInfo.hash, true) == 0) {
                        sysconFWInfo = new PS4SysconFWInfo(fwInfo.version, fwInfo.hash, fwInfo.debugMode);
                    }
                }
            }

           return sysconFWInfo;
        }

    }
}

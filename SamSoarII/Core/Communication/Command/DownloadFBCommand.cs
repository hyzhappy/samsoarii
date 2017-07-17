using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Core.Communication
{
    public class DownloadFBCommand : ICommunicationCommand
    {
        private byte[] retdata;
        public byte[] RetData
        {
            get { return this.retdata; }
            set { this.retdata = value; }
        }
        public int RecvDataLen {
            get { return 0; }
            set { }
        }
        public bool IsComplete {
            get { return retdata != null && retdata.Length >= RecvDataLen; }
            set { }
        }
        public bool IsSuccess {
            get { return retdata != null && retdata.Length >= 2 && retdata[0] == 0x00 && retdata[1] == 0x20; }
            set { }
        }
        
        public byte[] GetBytes()
        {
            byte[] ret = new byte[5];
            ret[0] = 0xFB;
            ret[1] = (byte)('B');
            ret[2] = (byte)('I');
            ret[3] = (byte)('A');
            ret[4] = (byte)('P');
            byte[] crc = CRC16.GetCRC(ret);
            return ret.Concat(crc).ToArray();
        }

        public void UpdataValues() { }
        
    }
}

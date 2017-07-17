using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Core.Communication
{
    public class DownloadFCCommand : ICommunicationCommand
    {
        private byte[] retdata;
        public byte[] RetData
        {
            get { return this.retdata; }
            set { this.retdata = value; }
        }
        public int RecvDataLen
        {
            get { return 2; }
            set { }
        }
        public bool IsComplete
        {
            get { return retdata != null && retdata.Length >= RecvDataLen; }
            set { }
        }
        public bool IsSuccess
        {
            get { return retdata != null && retdata.Length >= 2 && retdata[0] == 0x00 && retdata[1] == 0x20; }
            set { }
        }

        public byte[] GetBytes()
        {
            Random rand = new Random();
            byte[] ret = new byte[9];
            ret[0] = 0xFC;
            for (int i = 1; i < 9; i++)
            {
                ret[i] = (byte)(rand.Next()&0xFF);
            }
            byte[] crc = CRC16.GetCRC(ret);
            return ret.Concat(crc).ToArray();
        }

        public void UpdataValues()
        {
        }
    }
}

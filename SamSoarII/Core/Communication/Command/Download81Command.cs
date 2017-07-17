using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Core.Communication
{
    public class Download81Command : ICommunicationCommand
    {
        public byte[] RetData { get; set; }
        public bool IsComplete { get { return true; } set { } }
        public bool IsSuccess { get { return true; } set { } }
        public int RecvDataLen { get { return 0; } set { } }
        
        public byte[] GetBytes()
        {
            Random rand = new Random();
            byte[] ret = new byte[9];
            ret[0] = 0x81;
            for (int i = 1; i < 9; i++)
            {
                ret[i] = (byte)(rand.Next() & 0xFF);
            }
            byte[] crc = CRC16.GetCRC(ret);
            return ret.Concat(crc).ToArray();
        }

        public void UpdataValues()
        {
        }
    }
}

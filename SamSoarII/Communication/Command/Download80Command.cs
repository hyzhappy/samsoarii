using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Communication.Command
{
    public class Download80Command : ICommunicationCommand
    {
        public byte[] RetData { get; set; }
        public bool IsComplete { get { return true; } set { } }
        public bool IsSuccess { get { return true; } set { } }
        public int RecvDataLen { get { return 0; } set { } }

        private byte[] bytes;
        public byte[] GetBytes()
        {
            return bytes;
        }

        public void UpdataValues()
        {
        }

        public Download80Command(int id, byte[] data)
        {
            bytes = new byte[] { (byte)(id&0xff) }
                .Concat(data).ToArray();
            bytes = bytes.Concat(CRC16.GetCRC(bytes)).ToArray();
        }
    }
}

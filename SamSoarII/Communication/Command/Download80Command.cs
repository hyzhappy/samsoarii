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
            bytes = new byte[] { 0x80, (byte)(id&0xff) }
                .Concat(data).ToArray();
            bytes = bytes.Concat(CRC16.GetCRC(bytes)).ToArray();
            Encrypt(bytes);
        }
        
        private void Encrypt(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                byte di = data[i];
                byte bs = 0x01;
                int os = 7;
                data[i] = 0;
                while (os >= -7)
                {
                    data[i] |= (byte)(os > 0 ? ((di & bs) << os) : ((di & bs) >> (-os)));
                    bs <<= 1; os -= 2;
                }
                data[i] ^= 0x02;
            }
        }

        
    }
}

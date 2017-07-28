using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Core.Communication
{
    public class Download80Command : ICommunicationCommand
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

        private byte[] bytes;
        public byte[] GetBytes()
        {
            return bytes;
        }
        
        public Download80Command(int id, byte[] data)
        {
            Encrypt(id, data);
            bytes = new byte[] { 0x80, (byte)(id&0xff) }
                .Concat(data).ToArray();
            bytes = bytes.Concat(CRC16.GetCRC(bytes)).ToArray();
        }
        
        private void Encrypt(int id, byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                byte di = data[i];
                byte bs = 0x01;
                int os = 7;
                data[i] = 0;
                while (os >= -7)
                {
                    data[i] |= (byte)((os > 0) ? ((di & bs) << os) : ((di & bs) >> (-os)));
                    bs <<= 1; os -= 2;
                }
                data[i] ^= (byte)id;
            }
        }
    }
}

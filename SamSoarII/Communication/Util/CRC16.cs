using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Communication
{
    public class CRC16
    {
        public static byte[] GetCRC(byte[] data)
        {
            int len = data.Length;
            if (len > 0)
            {
                ushort crc = 0xFFFF;
                for (int i = 0; i < len; i++)
                {
                    crc = (ushort)(crc ^ (data[i]));
                    for (int j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                    }
                }
                byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
                byte lo = (byte)(crc & 0x00FF);         //低位置
                return new byte[] { lo, hi};
            }
            return new byte[] { 0, 0 };
        }
        public static bool CheckCRC(byte[] data,byte[] CRCCode)
        {
            byte[] TestCode = GetCRC(data);
            return TestCode[0] == CRCCode[0] && TestCode[1] == CRCCode[1];
        }
    }
}

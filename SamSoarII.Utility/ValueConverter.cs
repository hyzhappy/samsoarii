using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Utility
{
    public class ValueConverter
    {
        public static ushort ToUINT16(ushort BCDValue)
        {
            ushort value = 0;
            value += (ushort)(BCDValue & 0x000F);
            value += (ushort)(((BCDValue & 0x00F0) >> 4) * 10);
            value += (ushort)(((BCDValue & 0x0F00) >> 8) * 100);
            value += (ushort)(((BCDValue & 0xF000) >> 12) * 1000);
            return value;
        }
        /// <summary>
        /// value 值小于等于9999
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ushort ToBCD(ushort value)
        {
            ushort BCDValue = 0;
            BCDValue += (ushort)((value / 1000) << 12);
            value = (ushort)(value % 1000);
            BCDValue += (ushort)((value / 100) << 8);
            value = (ushort)(value % 100);
            BCDValue += (ushort)((value / 10) << 4);
            value = (ushort)(value % 10);
            BCDValue += value;
            return BCDValue;
        }
        public static byte[] GetBytes(ushort value, bool isLowHead = false)
        {
            byte byte1 = (byte)(value & 0x00FF);
            byte byte2 = (byte)((value & 0xFF00) >> 8);
            if(!isLowHead)
                return new byte[] {byte2,byte1 };
            return new byte[] { byte1, byte2 };
        }

        public static int GetValueByBytes(params byte[] value)
        {
            int retvalue = 0;
            for (int i = 0; i < value.Length; i++)
            {
                retvalue += (value[i] << (8 * i));
            }
            return retvalue;
        }

        public static byte[] GetBytes(uint value,bool isLowHead = false)
        {
            byte byte1 = (byte)(value & 0x000000FF);
            byte byte2 = (byte)((value & 0x0000FF00) >> 8);
            byte byte3 = (byte)((value & 0x00FF0000) >> 16);
            byte byte4 = (byte)((value & 0xFF000000) >> 24);
            if(!isLowHead)
                return new byte[] {byte2,byte1,byte4,byte3 };
            return new byte[] { byte1, byte2, byte3, byte4 };
        }

        public static byte[] GetBytes(string value)
        {
            byte[] data = new byte[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                data[i] = (byte)value[i];
            }
            return data;
        }
        public static uint GetValue(byte[] data)
        {
            if (data.Length == 2)
            {
                return (uint)((data[0] << 8) + data[1]);
            }
            else
            {
                return (uint)((data[0] << 8) + data[1] + (data[2] << 24) + (data[3] << 16));
            }
        }
        unsafe
        public static uint ParseShowValue(string showValue,WordType type)
        {
            switch (type)
            {
                case WordType.INT16:
                    return (uint)short.Parse(showValue);
                case WordType.POS_INT16:
                    return ushort.Parse(showValue);
                case WordType.INT32:
                    return (uint)int.Parse(showValue);
                case WordType.POS_INT32:
                    return uint.Parse(showValue);
                case WordType.BCD:
                    return ToUINT16(ushort.Parse(showValue));
                case WordType.FLOAT:
                    return FloatToUInt(float.Parse(showValue));
                default:
                    throw new FormatException();
            }
        }
        unsafe
        public static Int64 DoubleToInt64(double value)
        {
            return *((Int64*)&value);
        }
        unsafe
        public static double Int64ToDouble(Int64 value)
        {
            return *((double*)&value);
        }
        unsafe
        public static uint FloatToUInt(float value)
        {
            return *((uint*)&value);
        }
        unsafe 
        public static float UIntToFloat(uint value)
        {
            return *((float*)&value);
        }
        unsafe
        public static string ChangeShowValue(WordType sourceType,WordType desType,uint value)
        {
            switch (desType)
            {
                case WordType.INT16:
                    return ((short)value).ToString();
                case WordType.POS_INT16:
                    return ((ushort)value).ToString();
                case WordType.INT32:
                    return ((int)value).ToString();
                case WordType.POS_INT32:
                    return value.ToString();
                case WordType.BCD:
                    if (!AssertValue(sourceType,value))
                    {
                        throw new Exception();
                    }
                    else
                    {
                        return ToBCD((ushort)value).ToString();
                    }
                case WordType.FLOAT:
                    return (*(float*)&value).ToString();
                default:
                    throw new Exception();
            }
        }
        private static bool AssertValue(WordType type,uint value)
        {
            switch (type)
            {
                case WordType.INT16:
                    var truevalue1 = (short)value;
                    if (truevalue1 < 0 || truevalue1 > 9999) return false;
                    else return true;
                case WordType.POS_INT16:
                    var truevalue2 = (ushort)value;
                    if (truevalue2 < 0 || truevalue2 > 9999) return false;
                    else return true;
                case WordType.INT32:
                    var truevalue3 = (int)value;
                    if (truevalue3 < 0 || truevalue3 > 9999) return false;
                    else return true;
                case WordType.POS_INT32:
                    if (value < 0 || value > 9999) return false;
                    else return true;
                case WordType.BCD:
                    return true;
                case WordType.FLOAT:
                    return false;
                default:
                    return false;
            }
        }
    }
}

using SamSoarII.Utility;
using SamSoarII.PLCDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using SamSoarII.Core.Models;

namespace SamSoarII.Core.Communication
{
    public class CommandHelper
    {
        public static int GetLengthByAddrType(byte addrType)
        {
            switch (addrType)
            {
                case CommunicationDataDefine.ADDRESS_TYPE_X:
                case CommunicationDataDefine.ADDRESS_TYPE_Y:
                case CommunicationDataDefine.ADDRESS_TYPE_M:
                case CommunicationDataDefine.ADDRESS_TYPE_S:
                case CommunicationDataDefine.ADDRESS_TYPE_C:
                case CommunicationDataDefine.ADDRESS_TYPE_T:
                    return 1;
                case CommunicationDataDefine.ADDRESS_TYPE_AI:
                case CommunicationDataDefine.ADDRESS_TYPE_AO:
                case CommunicationDataDefine.ADDRESS_TYPE_D:
                case CommunicationDataDefine.ADDRESS_TYPE_V:
                case CommunicationDataDefine.ADDRESS_TYPE_Z:
                case CommunicationDataDefine.ADDRESS_TYPE_CV:
                case CommunicationDataDefine.ADDRESS_TYPE_TV:
                    return 2;
                case CommunicationDataDefine.ADDRESS_TYPE_CV32:
                    return 4;
                default:
                    return -1;
            }
        }
        public static FGs_ERR_CODE GetERRCODEType(byte value)
        {
            switch (value)
            {
                case 0x20:
                    return FGs_ERR_CODE.FGs_CARRY_OK;
                case 0x21:
                    return FGs_ERR_CODE.FGs_CRC_ERR;
                case 0x22:
                    return FGs_ERR_CODE.FGs_LENTH_ERR;
                case 0x23:
                    return FGs_ERR_CODE.FGs_ADDRESS_ERR;
                case 0x24:
                    return FGs_ERR_CODE.FGs_ADDRESS_TYPE_ERR;
                case 0x25:
                    return FGs_ERR_CODE.FGs_DES_ERR;
                case 0x26:
                    return FGs_ERR_CODE.FGs_ADDRESS_BEYOND_ERR;
                default:
                    return FGs_ERR_CODE.FGs_ISNOTANERRCODE;
            }
        }
        public static byte? GetAddrType(ValueModel.Bases type,uint offset)
        {
            switch (type)
            {
                case ValueModel.Bases.X:
                    return CommunicationDataDefine.ADDRESS_TYPE_X;
                case ValueModel.Bases.Y:
                    return CommunicationDataDefine.ADDRESS_TYPE_Y;
                case ValueModel.Bases.M:
                    return CommunicationDataDefine.ADDRESS_TYPE_M;
                case ValueModel.Bases.S:
                    return CommunicationDataDefine.ADDRESS_TYPE_S;
                case ValueModel.Bases.C:
                    return CommunicationDataDefine.ADDRESS_TYPE_C;
                case ValueModel.Bases.T:
                    return CommunicationDataDefine.ADDRESS_TYPE_T;
                case ValueModel.Bases.D:
                    return CommunicationDataDefine.ADDRESS_TYPE_D;
                case ValueModel.Bases.V:
                    return CommunicationDataDefine.ADDRESS_TYPE_V;
                case ValueModel.Bases.Z:
                    return CommunicationDataDefine.ADDRESS_TYPE_Z;
                case ValueModel.Bases.CV:
                    if (offset >= 200)
                    {
                        return CommunicationDataDefine.ADDRESS_TYPE_CV32;
                    }
                    else
                    {
                        return CommunicationDataDefine.ADDRESS_TYPE_CV;
                    }
                case ValueModel.Bases.TV:
                    return CommunicationDataDefine.ADDRESS_TYPE_TV;
                case ValueModel.Bases.AI:
                    return CommunicationDataDefine.ADDRESS_TYPE_AI;
                case ValueModel.Bases.AO:
                    return CommunicationDataDefine.ADDRESS_TYPE_AO;
                default:
                    return null;
            }
        }
        public static AddrSegment GetAddrSegment(ValueModel.Bases bas, int ofs, int len)
        {
            AddrSegment ret = new AddrSegment();
            ret.AddrLow = (byte)(ofs & 0xff);
            ret.AddrHigh = (byte)(ofs >> 8);
            ret.Length = (byte)len;
            switch (bas)
            {
                case ValueModel.Bases.X: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_X; break;
                case ValueModel.Bases.Y: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_Y; break;
                case ValueModel.Bases.S: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_S; break;
                case ValueModel.Bases.M: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_M; break;
                case ValueModel.Bases.T: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_T; break;
                case ValueModel.Bases.C: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_C; break;
                case ValueModel.Bases.D: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_D; break;
                case ValueModel.Bases.TV: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_TV; break;
                case ValueModel.Bases.CV: ret.Type = ofs < 200 ? CommunicationDataDefine.ADDRESS_TYPE_CV : CommunicationDataDefine.ADDRESS_TYPE_CV32; break;
                case ValueModel.Bases.AI: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_AI; break;
                case ValueModel.Bases.AO: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_AO; break;
                case ValueModel.Bases.V: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_V; break;
                case ValueModel.Bases.Z: ret.Type = CommunicationDataDefine.ADDRESS_TYPE_Z; break;
            }
            return ret;
        }
    }
}

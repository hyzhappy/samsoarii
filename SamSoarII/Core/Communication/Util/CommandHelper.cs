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
                case 0x27:
                    return FGs_ERR_CODE.FGs_ISNOTANERRCODE;
                case 0x60:
                    return FGs_ERR_CODE.COMCODE_CARRY_OK;
                case 0x61:
                    return FGs_ERR_CODE.COMCODE_LENGTH_ERR;
                case 0x62:
                    return FGs_ERR_CODE.COMCODE_NODEID_ERR;
                case 0x63:
                    return FGs_ERR_CODE.COMCODE_CRC_ERR;
                case 0x64:
                    return FGs_ERR_CODE.COMCODE_INVALID_CMD;
                case 0x65:
                    return FGs_ERR_CODE.COMCODE_DOWNLOAD_BEYOND;
                case 0x66:
                    return FGs_ERR_CODE.COMCODE_PASSWD_ERR;
                default:
                    return FGs_ERR_CODE.COMCODE_CARRY_OK;
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
        public static void Encrypt(int id, byte[] data)
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

        public static byte[] Decrypt(int id, byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= (byte)id;
                byte di = data[i];
                ushort bs = 0x0101;
                int os = 7;
                data[i] = 0;
                while (os >= -7)
                {
                    data[i] |= (byte)((os > 0) ? ((di & bs) << os) : ((di & bs) >> (-os)));
                    bs <<= 1; os -= 2;
                }
            }
            return data;
        }
        public static void CheckRetData(ICommunicationCommand command, byte[] _retData)
        {
            //数据长度为0视为为未完
            if (_retData.Length == 0)
            {
                command.IsComplete = false;
                command.IsSuccess = false;
                return;
            }
            //数据不是以CMD_DOWNLOAD_FLAG开头，视为接受完成但错误
            if (_retData[0] != CommunicationDataDefine.CMD_COMMU_FLAG)
            {
                command.IsComplete = true;
                command.IsSuccess = false;
                return;
            }
            if (_retData.Length < 3)
            {
                command.IsComplete = false;
                command.IsSuccess = false;
                return;
            }
            int len = ValueConverter.GetValueByBytes(_retData[1], _retData[2]);
            command.IsComplete = _retData.Length >= len;
            //长度大于应接受的长度，视为接受完成但错误
            if (_retData.Length > len)
            {
                command.IsSuccess = false;
                return;
            }
            command.IsComplete &= CRC16.CheckCRC(command);
            if (command.IsComplete)
            {
                FGs_ERR_CODE errCodeType = GetERRCODEType(_retData[3]);
                command.IsSuccess = errCodeType == FGs_ERR_CODE.COMCODE_CARRY_OK;
                if(command is UploadTypeStart)
                {
                    command.RecvDataLen = command.IsSuccess ? ValueConverter.GetValueByBytes(_retData[6], _retData[7], _retData[8], _retData[9]) : -1;
                }
            }
            else command.IsSuccess = false;
        }
    }
}

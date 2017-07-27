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
        public static bool checkAddrRange(byte addrType, byte length,ushort startAddr)
        {
            switch (addrType)
            {
                case CommunicationDataDefine.ADDRESS_TYPE_X:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.XRange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.XRange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_Y:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.YRange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.YRange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_M:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.MRange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.MRange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_S:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.SRange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.SRange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_C:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CRange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CRange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_T:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.TRange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.TRange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_AI:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.AIRange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.AIRange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_AO:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.AORange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.AORange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_D:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.DRange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.DRange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_V:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.VRange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.VRange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_Z:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.ZRange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.ZRange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_CV:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CV16Range.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CV16Range.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_TV:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.TVRange.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.TVRange.AssertValue((uint)(startAddr + length - 1));
                case CommunicationDataDefine.ADDRESS_TYPE_CV32:
                    return PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CV32Range.AssertValue(startAddr) && PLCDeviceManager.GetPLCDeviceManager().SelectDevice.CV32Range.AssertValue((uint)(startAddr + length - 1));
                default:
                    return false;
            }
        }
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
        public static byte? GetAddrType(ElementAddressType type,uint offset)
        {
            switch (type)
            {
                case ElementAddressType.X:
                    return CommunicationDataDefine.ADDRESS_TYPE_X;
                case ElementAddressType.Y:
                    return CommunicationDataDefine.ADDRESS_TYPE_Y;
                case ElementAddressType.M:
                    return CommunicationDataDefine.ADDRESS_TYPE_M;
                case ElementAddressType.S:
                    return CommunicationDataDefine.ADDRESS_TYPE_S;
                case ElementAddressType.C:
                    return CommunicationDataDefine.ADDRESS_TYPE_C;
                case ElementAddressType.T:
                    return CommunicationDataDefine.ADDRESS_TYPE_T;
                case ElementAddressType.D:
                    return CommunicationDataDefine.ADDRESS_TYPE_D;
                case ElementAddressType.V:
                    return CommunicationDataDefine.ADDRESS_TYPE_V;
                case ElementAddressType.Z:
                    return CommunicationDataDefine.ADDRESS_TYPE_Z;
                case ElementAddressType.CV:
                    if (offset >= 200)
                    {
                        return CommunicationDataDefine.ADDRESS_TYPE_CV32;
                    }
                    else
                    {
                        return CommunicationDataDefine.ADDRESS_TYPE_CV;
                    }
                case ElementAddressType.TV:
                    return CommunicationDataDefine.ADDRESS_TYPE_TV;
                case ElementAddressType.AI:
                    return CommunicationDataDefine.ADDRESS_TYPE_AI;
                case ElementAddressType.AO:
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
        public static void UpdateElements(List<AddrSegment> Segments,byte[] data)
        {
            byte addrType = Segments.First().Type;
            int startAddr = Segments.OrderBy(x => { return x.Model.StartAddr; }).First().Model.StartAddr;
            int typeLen = GetLengthByAddrType(addrType);
            foreach (var segment in Segments)
            {
                int span = segment.Model.StartAddr - startAddr;
                if (typeLen == 1)
                {
                    int index = (int)span / 8;
                    int offset = (int)span % 8;
                    if (((data[index] >> offset) & 0x01) == 0x00)
                    {
                        segment.Model.CurrentValue = 0;
                    }
                    else
                    {
                        segment.Model.CurrentValue = 1;
                    }
                }
                else
                {
                    WordType type = (WordType)Enum.ToObject(typeof(WordType), segment.Model.DataType);
                    byte[] value = new byte[segment.Model.ByteCount];
                    for (int i = 0; i < segment.Model.ByteCount; i++)
                    {
                        value[i] = data[typeLen * span + i];
                    }
                    uint showValue = ValueConverter.GetValue(value);
                    switch (type)
                    {
                        case WordType.INT16:
                            segment.Model.CurrentValue = (short)showValue;
                            break;
                        case WordType.POS_INT16:
                            segment.Model.CurrentValue = (ushort)showValue;
                            break;
                        case WordType.INT32:
                            segment.Model.CurrentValue = (int)showValue;
                            break;
                        case WordType.POS_INT32:
                            segment.Model.CurrentValue = (uint)showValue;
                            break;
                        case WordType.BCD:
                            if (showValue > 9999)
                            {
                                segment.Model.CurrentValue = string.Format("???");
                            }
                            else
                            {
                                segment.Model.CurrentValue = ValueConverter.ToBCD((ushort)showValue);
                            }
                            break;
                        case WordType.FLOAT:
                            segment.Model.CurrentValue = ValueConverter.UIntToFloat(showValue);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public static void UpdateElementsByIntra(List<IntraSegment> Segments, byte[] data)
        {
            List<AddrSegment> segments = new List<AddrSegment>();
            foreach (var segment in Segments)
            {
                segments.Add(segment.Base);
            }
            UpdateElements(segments,data);
        }
    }
}

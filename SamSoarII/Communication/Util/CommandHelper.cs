using SamSoarII.Utility;
using SamSoarII.PLCDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.AppMain.UI.Monitor;

namespace SamSoarII.Communication
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
                    return FGs_ERR_CODE.FGs_CARRY_OK;
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
        public static void UpdataElements(List<ElementModel> elements, byte[] retData, byte length)
        {
            if (elements.Count != 0)
            {
                int typelen = retData.Length / length;
                for (int i = 0; i < length; i++)
                {
                    if (typelen == 1)
                    {
                        if (retData[i] == 0x00)
                        {
                            elements[i].CurrentValue = string.Format("OFF");
                        }
                        else
                        {
                            elements[i].CurrentValue = string.Format("ON");
                        }
                    }
                    else
                    {
                        WordType type = (WordType)Enum.ToObject(typeof(WordType), elements.First().DataType);
                        byte[] data = new byte[typelen];
                        for (int j = 0; j < typelen; j++)
                        {
                            data[j] = retData[i * typelen + j];
                        }
                        uint value = ValueConverter.GetValue(data);
                        switch (type)
                        {
                            case WordType.INT16:
                                elements[i].CurrentValue = string.Format("{0}", (short)value);
                                break;
                            case WordType.POS_INT16:
                                elements[i].CurrentValue = string.Format("{0}", (ushort)value);
                                break;
                            case WordType.INT32:
                                elements[i].CurrentValue = string.Format("{0}", (int)value);
                                break;
                            case WordType.POS_INT32:
                                elements[i].CurrentValue = string.Format("{0}", value);
                                break;
                            case WordType.BCD:
                                if (value > 9999)
                                {
                                    elements[i].CurrentValue = string.Format("????");
                                }
                                else
                                {
                                    elements[i].CurrentValue = string.Format("{0}", ValueConverter.ToBCD((ushort)value));
                                }
                                break;
                            case WordType.FLOAT:
                                elements[i].CurrentValue = string.Format("{0}", ValueConverter.UIntToFloat(value));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}

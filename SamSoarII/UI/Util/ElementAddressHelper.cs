using SamSoarII.PLCDevice;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.UI
{
    public class ElementAddressHelper
    {
        public static bool IsBitAddr(ElementAddressType Type)
        {
            switch (Type)
            {
                case ElementAddressType.X:
                case ElementAddressType.Y:
                case ElementAddressType.M:
                case ElementAddressType.S:
                case ElementAddressType.C:
                case ElementAddressType.T:
                    return true;
                default:
                    return false;
            }
        }
        public static ElementAddressType GetIntrasegmentAddrType(int index)
        {
            if (index == 0)
            {
                return ElementAddressType.V;
            }
            else
            {
                return ElementAddressType.Z;
            }
        }
        public static bool AssertAddrRange(ElementAddressType type, uint value, Device CurrentDevice)
        {
            switch (type)
            {
                case ElementAddressType.X:
                    return CurrentDevice.XRange.AssertValue(value);
                case ElementAddressType.Y:
                    return CurrentDevice.YRange.AssertValue(value);
                case ElementAddressType.M:
                    return CurrentDevice.MRange.AssertValue(value);
                case ElementAddressType.S:
                    return CurrentDevice.SRange.AssertValue(value);
                case ElementAddressType.C:
                    return CurrentDevice.CRange.AssertValue(value);
                case ElementAddressType.T:
                    return CurrentDevice.TRange.AssertValue(value);
                case ElementAddressType.D:
                    return CurrentDevice.DRange.AssertValue(value);
                case ElementAddressType.V:
                    return CurrentDevice.VRange.AssertValue(value);
                case ElementAddressType.Z:
                    return CurrentDevice.ZRange.AssertValue(value);
                case ElementAddressType.CV:
                    return CurrentDevice.CVRange.AssertValue(value);
                case ElementAddressType.TV:
                    return CurrentDevice.TVRange.AssertValue(value);
                case ElementAddressType.AI:
                    return CurrentDevice.AIRange.AssertValue(value);
                case ElementAddressType.AO:
                    return CurrentDevice.AORange.AssertValue(value);
                default:
                    return false;
            }
        }
    }
}

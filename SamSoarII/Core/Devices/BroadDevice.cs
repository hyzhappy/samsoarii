using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Utility;

namespace SamSoarII.PLCDevice
{

    /// <summary>
    /// 使用这个设备类时，将不判段输入的范围条件
    /// </summary>
    public class BroadDevice : Device
    {
        public override int BitNumber { get { return 32; } }
        public override IntRange AIRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override IntRange AORange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override IntRange CRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override IntRange CVRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override string DeviceName
        {
            get
            {
                return "BroadDevice";
            }
        }

        public override IntRange DRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override IntRange MRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override IntRange SRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override IntRange TRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override IntRange TVRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override IntRange VRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override IntRange XRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override IntRange YRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }

        public override IntRange ZRange
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }
        public override IntRange CV16Range
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }
        public override IntRange CV32Range
        {
            get
            {
                return new IntRange(uint.MinValue, uint.MaxValue);
            }
        }
        public override PLC_FGs_Type Type
        {
            get
            {
                return PLC_FGs_Type.FGs_16MR_A;
            }
        }
    }
}

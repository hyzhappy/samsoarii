using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Utility;

namespace SamSoarII.PLCDevice
{
    class FGs16MTDevice : Device
    {
        public override IntRange AIRange
        {
            get
            {
                return new IntRange(0, 8);
            }
        }

        public override IntRange AORange
        {
            get
            {
                return new IntRange(0, 8);
            }
        }

        public override IntRange CRange
        {
            get
            {
                return new IntRange(0, 256);
            }
        }

        public override IntRange CV16Range
        {
            get
            {
                return new IntRange(0, 200);
            }
        }

        public override IntRange CV32Range
        {
            get
            {
                return new IntRange(200, 256);
            }
        }

        public override IntRange CVRange
        {
            get
            {
                return new IntRange(0, 256);
            }
        }

        public override string DeviceName
        {
            get
            {
                return "FGs16MT";
            }
        }

        public override IntRange DRange
        {
            get
            {
                return new IntRange(0, 8192);
            }
        }

        public override IntRange MRange
        {
            get
            {
                return new IntRange(0, 8192);
            }
        }

        public override IntRange SRange
        {
            get
            {
                return new IntRange(0, 1000);
            }
        }

        public override IntRange TRange
        {
            get
            {
                return new IntRange(0, 256);
            }
        }

        public override IntRange TVRange
        {
            get
            {
                return new IntRange(0, 256);
            }
        }

        public override IntRange VRange
        {
            get
            {
                return new IntRange(0, 8);
            }
        }

        public override IntRange XRange
        {
            get
            {
                return new IntRange(0, 64);
            }
        }

        public override IntRange YRange
        {
            get
            {
                return new IntRange(0, 64);
            }
        }

        public override IntRange ZRange
        {
            get
            {
                return new IntRange(0, 8);
            }
        }
    }
}

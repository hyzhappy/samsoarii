using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice
{
    class FGs64MRDevice : Device
    {
        public FGs64MRDevice() { }
        public FGs64MRDevice(PLC_FGs_Type type)
        {
            _type = type;
        }
        public override int BitNumber { get { return 32; } }
        public override IntRange AIRange
        {
            get
            {
                return new IntRange(0, 32);
            }
        }

        public override IntRange AORange
        {
            get
            {
                return new IntRange(0, 32);
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
                return "FGs64MR";
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
                return new IntRange(0, 128);
            }
        }

        public override IntRange YRange
        {
            get
            {
                return new IntRange(0, 128);
            }
        }

        public override IntRange ZRange
        {
            get
            {
                return new IntRange(0, 8);
            }
        }
        public override IntRange PulseRange
        {
            get
            {
                return new IntRange(0, 4);
            }
        }
        public override IntRange EXRange
        {
            get
            {
                return new IntRange(512, 512 + 1024);
            }
        }

        public override IntRange EYRange
        {
            get
            {
                return new IntRange(512, 512 + 1024);
            }
        }
        private PLC_FGs_Type _type;
        public override PLC_FGs_Type Type
        {
            get
            {
                return _type;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Utility;
namespace SamSoarII.PLCDevice
{
    public abstract class Device
    {
        public abstract string DeviceName { get; }
        public abstract PLCDeviceType Type { get; }
        private static BroadDevice _defaultDevice = new BroadDevice();
        public static Device DefaultDevice { get { return _defaultDevice; } }
        
        public abstract IntRange XRange { get; }
        public abstract IntRange YRange { get; }
        public abstract IntRange MRange { get; }
        public abstract IntRange CRange { get; }
        public abstract IntRange TRange { get; }
        public abstract IntRange SRange { get; }
        public abstract IntRange DRange { get; }
        public abstract IntRange CVRange { get; }
        public abstract IntRange TVRange { get; }
        public abstract IntRange AIRange { get; }
        public abstract IntRange AORange { get; }
        public abstract IntRange VRange { get; }
        public abstract IntRange ZRange { get; }
        public abstract IntRange CV16Range { get; }
        public abstract IntRange CV32Range { get; }
    }
}

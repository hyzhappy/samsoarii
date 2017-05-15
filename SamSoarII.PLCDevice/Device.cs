using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Utility;
using System.Xml.Linq;
using System.IO;

namespace SamSoarII.PLCDevice
{
    public abstract class Device
    {
        public abstract string DeviceName { get; }
        public abstract int BitNumber { get; }
        public abstract PLCDeviceType Type { get; }
        private static BroadDevice _defaultDevice;
        private static MaxRangeDevice _maxRangeDevice;
        public static Device DefaultDevice { get { return _defaultDevice; } }
        public static Device MaxRangeDevice { get { return _maxRangeDevice; } }
        public static List<SpecialRegister> SpecialRegisters;
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
        static Device()
        {
            _defaultDevice = new BroadDevice();
            _maxRangeDevice = new MaxRangeDevice(PLCDeviceType.FGs_64MT_D);
            SpecialRegisters = new List<SpecialRegister>();
        }
        public static void InitializeSpecialRegisters()
        {
            XDocument xDoc = XDocument.Load(Directory.GetCurrentDirectory() + @"\SystemSetting" + @"\SpecialRegisters.xml");
            var rootNode = xDoc.Root;
            foreach (var ele in rootNode.Elements())
            {
                SpecialRegister register = new SpecialRegister();
                register.ID = int.Parse(ele.Attribute("ID").Value);
                register.Base = ele.Attribute("Base").Value;
                register.Offset = int.Parse(ele.Attribute("Offset").Value);
                register.CanRead = bool.Parse(ele.Attribute("CanRead").Value);
                register.CanWrite = bool.Parse(ele.Attribute("CanWrite").Value);
                register.Describe = ele.Attribute("Describe").Value;
                SpecialRegisters.Add(register);
            }
        }
    }
}

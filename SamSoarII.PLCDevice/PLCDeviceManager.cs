using SamSoarII.PLCDevice.DeviceDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice
{
    public class PLCDeviceManager
    {
        private static  Device _selectDevice = Device.DefaultDevice;
        public static Device SelectDevice
        {
            get
            {
                return _selectDevice;
            }
        }
        public static void SetSelectDeviceType(PLCDeviceType type)
        {
            switch (type)
            {
                case PLCDeviceType.FGs16MT:
                    _selectDevice = new FGs16MTDevice();
                    break;
                case PLCDeviceType.FGs16MR:
                    _selectDevice = new FGs16MRDevice();
                    break;
                case PLCDeviceType.FGs32MT:
                    _selectDevice = new FGs32MTDevice();
                    break;
                case PLCDeviceType.FGs32MR:
                    _selectDevice = new FGs32MRDevice();
                    break;
                case PLCDeviceType.FGs64MT:
                    _selectDevice = new FGs64MTDevice();
                    break;
                case PLCDeviceType.FGs64MR:
                    _selectDevice = new FGs64MRDevice();
                    break;
                default:
                    _selectDevice = new BroadDevice();
                    break;
            }
        }
        public static BaseDeviceMessageDialog GetDeviceMessageDialog(int index)
        {
            switch (index)
            {
                case 0:
                    return new FGs16MRDeviceMessageDialog();
                case 1:
                    return new FGs16MTDeviceMessageDialog();
                case 2:
                    return new FGs32MRDeviceMessageDialog();
                case 3:
                    return new FGs32MTDeviceMessageDialog();
                case 4:
                    return new FGs64MRDeviceMessageDialog();
                case 5:
                    return new FGs64MTDeviceMessageDialog();
                default:
                    return null;
            }
        }
    }
}

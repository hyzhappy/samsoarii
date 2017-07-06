using SamSoarII.PLCDevice.DeviceDialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCDevice
{
    public class PLCDeviceManager : INotifyPropertyChanged
    {
        private static PLCDeviceManager _PLCDeviceManager = new PLCDeviceManager();
        private Device _selectDevice = Device.MaxRangeDevice;
        private List<BaseDeviceMessageDialog> DeviceDialogs;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public int SelectIndex
        {
            get
            {
                return GetIndexValue(_selectDevice.Type);
            }
        }
        public static PLCDeviceManager GetPLCDeviceManager()
        {
            return _PLCDeviceManager;
        }
        public PLCDeviceManager()
        {
            DeviceDialogs = new List<BaseDeviceMessageDialog>();
            DeviceDialogs.Add(new FGs16MRDeviceMessageDialog());
            DeviceDialogs.Add(new FGs16MTDeviceMessageDialog());
            DeviceDialogs.Add(new FGs32MRDeviceMessageDialog());
            DeviceDialogs.Add(new FGs32MTDeviceMessageDialog());
            DeviceDialogs.Add(new FGs64MRDeviceMessageDialog());
            DeviceDialogs.Add(new FGs64MTDeviceMessageDialog());
            DeviceDialogs.Add(new FGs32MRYTJDeviceMessageDialog());
            DeviceDialogs.Add(new FGs32MTYTJDeviceMessageDialog());
            DeviceDialogs.Add(new FGs20MRBYKDeviceMessageDialog());
        }
        public Device SelectDevice
        {
            get
            {
                return _selectDevice;
            }
        }
        public static int GetIndexValue(PLCDeviceType type)
        {
            switch (type)
            {
                case PLCDeviceType.FGs_16MR_A:
                    return 0;
                case PLCDeviceType.FGs_16MR_D:
                    return 1;
                case PLCDeviceType.FGs_16MT_A:
                    return 2;
                case PLCDeviceType.FGs_16MT_D:
                    return 3;
                case PLCDeviceType.FGs_32MR_A:
                    return 4;
                case PLCDeviceType.FGs_32MR_D:
                    return 5;
                case PLCDeviceType.FGs_32MT_A:
                    return 6;
                case PLCDeviceType.FGs_32MT_D:
                    return 7;
                case PLCDeviceType.FGs_64MR_A:
                    return 8;
                case PLCDeviceType.FGs_64MR_D:
                    return 9;
                case PLCDeviceType.FGs_64MT_A:
                    return 10;
                case PLCDeviceType.FGs_64MT_D:
                    return 11;
                case PLCDeviceType.FGs_32MR_YTJ:
                    return 12;
                case PLCDeviceType.FGs_32MT_YTJ:
                    return 13;
                case PLCDeviceType.FGs_20MR_BYK:
                    return 14;
                default:
                    return -1;
            }
        }
        public void SetSelectDeviceType(PLCDeviceType type)
        {
            switch (type)
            {
                case PLCDeviceType.FGs_16MR_A:
                case PLCDeviceType.FGs_16MR_D:
                    _selectDevice = new FGs16MRDevice(type);
                    break;
                case PLCDeviceType.FGs_16MT_A:
                case PLCDeviceType.FGs_16MT_D:
                    _selectDevice = new FGs16MTDevice(type);
                    break;
                case PLCDeviceType.FGs_32MR_A:
                case PLCDeviceType.FGs_32MR_D:
                    _selectDevice = new FGs32MRDevice(type);
                    break;
                case PLCDeviceType.FGs_32MT_A:
                case PLCDeviceType.FGs_32MT_D:
                    _selectDevice = new FGs32MTDevice(type);
                    break;
                case PLCDeviceType.FGs_64MR_A:
                case PLCDeviceType.FGs_64MR_D:
                    _selectDevice = new FGs64MRDevice(type);
                    break;
                case PLCDeviceType.FGs_64MT_A:
                case PLCDeviceType.FGs_64MT_D:
                    _selectDevice = new FGs64MTDevice(type);
                    break;
                case PLCDeviceType.FGs_32MR_YTJ:
                    _selectDevice = new FGs32MR_YTJDevice(type);
                    break;
                case PLCDeviceType.FGs_32MT_YTJ:
                    _selectDevice = new FGs32MT_YTJDevice(type);
                    break;
                case PLCDeviceType.FGs_20MR_BYK:
                    _selectDevice = new FGs20MR_BYKDevice(type);
                    break;
                default:
                    _selectDevice = Device.DefaultDevice;
                    break;
            }
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SelectIndex"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SelectDevice"));
        }
        public List<BaseDeviceMessageDialog> GetDeviceMessageDialogs()
        {
            return DeviceDialogs;
        }
    }
}

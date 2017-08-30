﻿using SamSoarII.PLCDevice.DeviceDialog;
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
        public static int GetIndexValue(object type)
        {
            if(type is PLC_FGs_Type)
            {
                switch ((PLC_FGs_Type)type)
                {
                    case PLC_FGs_Type.FGs_16MR_A:
                        return 0;
                    case PLC_FGs_Type.FGs_16MR_D:
                        return 1;
                    case PLC_FGs_Type.FGs_16MT_A:
                        return 2;
                    case PLC_FGs_Type.FGs_16MT_D:
                        return 3;
                    case PLC_FGs_Type.FGs_32MR_A:
                        return 4;
                    case PLC_FGs_Type.FGs_32MR_D:
                        return 5;
                    case PLC_FGs_Type.FGs_32MT_A:
                        return 6;
                    case PLC_FGs_Type.FGs_32MT_D:
                        return 7;
                    case PLC_FGs_Type.FGs_64MR_A:
                        return 8;
                    case PLC_FGs_Type.FGs_64MR_D:
                        return 9;
                    case PLC_FGs_Type.FGs_64MT_A:
                        return 10;
                    case PLC_FGs_Type.FGs_64MT_D:
                        return 11;
                    case PLC_FGs_Type.FGs_32MR_YTJ:
                        return 12;
                    case PLC_FGs_Type.FGs_32MT_YTJ:
                        return 13;
                    case PLC_FGs_Type.FGs_20MR_BYK:
                        return 14;
                    default:
                        return -1;
                }
            }
            else
            {
                switch ((PLC_FGm_Type)type)
                {
                    case PLC_FGm_Type.FGm_32MT_A:
                        return 0;
                    case PLC_FGm_Type.FGm_48MT_A:
                        return 1;
                    case PLC_FGm_Type.FGm_64MT_A:
                        return 2;
                    default:
                        return -1;
                }
            }
        }
        public void SetSelectDeviceType(object type)
        {
            if(type is PLC_FGs_Type)
            {
                PLC_FGs_Type fgs_type = (PLC_FGs_Type)type;
                switch (fgs_type)
                {
                    case PLC_FGs_Type.FGs_16MR_A:
                    case PLC_FGs_Type.FGs_16MR_D:
                        _selectDevice = new FGs16MRDevice(fgs_type);
                        break;
                    case PLC_FGs_Type.FGs_16MT_A:
                    case PLC_FGs_Type.FGs_16MT_D:
                        _selectDevice = new FGs16MTDevice(fgs_type);
                        break;
                    case PLC_FGs_Type.FGs_32MR_A:
                    case PLC_FGs_Type.FGs_32MR_D:
                        _selectDevice = new FGs32MRDevice(fgs_type);
                        break;
                    case PLC_FGs_Type.FGs_32MT_A:
                        _selectDevice = new FGs32MTADevice(fgs_type);
                        break;
                    case PLC_FGs_Type.FGs_32MT_D:
                        _selectDevice = new FGs32MTDevice(fgs_type);
                        break;
                    case PLC_FGs_Type.FGs_64MR_A:
                        _selectDevice = new FGs64MTADevice(fgs_type);
                        break;
                    case PLC_FGs_Type.FGs_64MR_D:
                        _selectDevice = new FGs64MRDevice(fgs_type);
                        break;
                    case PLC_FGs_Type.FGs_64MT_A:
                    case PLC_FGs_Type.FGs_64MT_D:
                        _selectDevice = new FGs64MTDevice(fgs_type);
                        break;
                    case PLC_FGs_Type.FGs_32MR_YTJ:
                        _selectDevice = new FGs32MR_YTJDevice(fgs_type);
                        break;
                    case PLC_FGs_Type.FGs_32MT_YTJ:
                        _selectDevice = new FGs32MT_YTJDevice(fgs_type);
                        break;
                    case PLC_FGs_Type.FGs_20MR_BYK:
                        _selectDevice = new FGs20MR_BYKDevice(fgs_type);
                        break;
                    default:
                        _selectDevice = Device.DefaultDevice;
                        break;
                }
                PropertyChanged.Invoke(_PLCDeviceManager, new PropertyChangedEventArgs("SelectIndex"));
            }
        }

        public List<BaseDeviceMessageDialog> GetDeviceMessageDialogs()
        {
            return DeviceDialogs;
        }

        public static bool IsFGsType(object type)
        {
            return (int)type < 15;
        }
    }
}

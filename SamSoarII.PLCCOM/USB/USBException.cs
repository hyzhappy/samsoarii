using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCCOM.USB
{
    public class USBException : Exception
    {
        public const int DEVICE_ACTIVE = 0x01;
        public const int HANDLE_ACTIVE = 0x02;
        public const int DEVICE_NULL = 0x03;
        public const int HANDLE_NULL = 0x04;
        public const int CANNOT_GET_DEVICE = 0x05;
        public const int CANNOT_GET_HANDLE = 0x06;
        public const int CANNOT_TRANSFER = 0x07;
        public const int CANNOT_SEND_COMMAND = 0x08;
        public const int CANNOT_RECV_REPORT = 0x09;
        public const int ERROR_RECV_REPORT = 0x0a;
        public const int OUT_OF_LENGTH_LIMIT = 0x0b;
        public const int CANNOT_FOUND_FILE = 0x0c;
        public const int UNDEFINED_USBMODEL_STATUS = 0x0d;

        public int ErrorCode { get; private set; }

        public USBException(int _errorcode) 
            : base(_GetMessage(_errorcode))
        {
            ErrorCode = _errorcode;
        }

        static private string _GetMessage(int _errorcode)
        {
            switch (_errorcode&0xff)
            {
                case DEVICE_ACTIVE:
                    return "Cannot create anothor usblib_device when it has been already existed.";
                case HANDLE_ACTIVE:
                    return "Cannot create anothor usblib_device_handle when it has been already existed.";
                case DEVICE_NULL:
                    return "Cannot do something in usblib_device when it has been already closed.";
                case HANDLE_NULL:
                    return "Cannot do something in usblib_device_handle when it has been already closed.";
                case CANNOT_GET_DEVICE:
                    return "Cannot create a new usblib_device.";
                case CANNOT_GET_HANDLE:
                    return "Cannot create a new usblib_device_handle.";
                case CANNOT_TRANSFER:
                    return "Cannot transfer datas between PC and USB port.";
                case CANNOT_SEND_COMMAND:
                    return "Cannot send communication request protocal to PLC Device.";
                case CANNOT_RECV_REPORT:
                    return "Cannot receive report from PLC Device.";
                case ERROR_RECV_REPORT:
                    return String.Format("Error happen while communicating : {0:x}", _errorcode >> 8);
                case OUT_OF_LENGTH_LIMIT:
                    return "The length of one data package is more than maximum(256).";
                case CANNOT_FOUND_FILE:
                    return "Cannot found the file.";
                case UNDEFINED_USBMODEL_STATUS:
                    return String.Format("Try to assign USBModel.Status to an unknown value : {0:x}", _errorcode >> 8);
                default:
                    return "Unknown error.";
            }
        }
    }
}

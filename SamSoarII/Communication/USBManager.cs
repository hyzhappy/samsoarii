using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Communication.Command;
using System.Runtime.InteropServices;
using SamSoarII.AppMain.UI.Monitor;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.DeviceNotify;

namespace SamSoarII.Communication
{
    public class USBManager : ICommunicationManager
    {
        public const int PID = 0x5189;
        public const int VID = 0x04e8;

        public static UsbDevice USBDevice;//USB设备
        public static UsbEndpointWriter writer = null;
        public static UsbEndpointReader reader = null;
        
        
        public int Abort()
        {
            CloseUSB();
            return 0;
        }
        private bool AssertCommand(ICommunicationCommand cmd)
        {
            return cmd is IntrasegmentWriteCommand || cmd is GeneralWriteCommand
                    || cmd is ForceCancelCommand;
        }
        public int Start()
        {
            if (FindAndOpenUSB(VID, PID))
            {
                writer = USBDevice.OpenEndpointWriter(WriteEndpointID.Ep02);
                reader = USBDevice.OpenEndpointReader(ReadEndpointID.Ep01);
                return 0;
            }
            else
                return 1;
        }
        static int readBuffercount = 0;
        public int Read(ICommunicationCommand cmd)
        {
            ErrorCode ec = ErrorCode.None;
            byte[] tempdata = new byte[4096];
            byte[] data = new byte[cmd.RecvDataLen];
            int bytesRead;
            try
            {
                ec = reader.Read(tempdata, 100, out bytesRead);
                if (AssertCommand(cmd))
                {
                    for (int i = 0; i < bytesRead; i++)
                        data[readBuffercount++] = tempdata[i];
                }
                else
                {
                    int cnt = bytesRead / CommunicationDataDefine.USB_MAX_READ_LEN;
                    int res = bytesRead % CommunicationDataDefine.USB_MAX_READ_LEN;
                    for (int i = 0; i < cnt; i++)
                    {
                        for (int j = 2; j < CommunicationDataDefine.USB_MAX_READ_LEN; j++)
                        {
                            data[readBuffercount++] = tempdata[j + CommunicationDataDefine.USB_MAX_READ_LEN * i];
                        }
                    }
                    for (int i = 2; i < res; i++)
                    {
                        data[readBuffercount++] = tempdata[i + CommunicationDataDefine.USB_MAX_READ_LEN * cnt];
                    }
                }
                if (ec != ErrorCode.None)
                    return 1;
            }
            catch (Exception)
            {
                return 1;
            }
            if (readBuffercount != cmd.RecvDataLen)
                return 1;
            cmd.RetData = data;
            readBuffercount = 0;
            return 0;
        }
        public int Write(ICommunicationCommand cmd)
        {
            ErrorCode ec = ErrorCode.None;
            int bytesWritten;
            try
            {
                ec = writer.Write(cmd.GetBytes(), 2000, out bytesWritten);
                if (ec != ErrorCode.None)
                    return 1;
            }
            catch (Exception)
            {
                return 1;
            }
            return 0;
        }
        private void CloseUSB()
        {
            if (!ReferenceEquals(reader, null))
                reader.Dispose();
            if (!ReferenceEquals(writer, null))
                writer.Dispose();
            if (!ReferenceEquals(USBDevice, null))
                USBDevice.Close();
        }
        private bool FindAndOpenUSB(int VID, int PID)
        {
            UsbDeviceFinder usbFinder = new UsbDeviceFinder(VID, PID);
            UsbRegistry usbRegistry = UsbDevice.AllDevices.Find(usbFinder);
            int cnt = UsbDevice.AllDevices.Count;
            if (ReferenceEquals(usbRegistry, null))
                return false;
            // Open this usb device.
            if (!usbRegistry.Open(out USBDevice))
                return false;
            ((LibUsbDevice)USBDevice).SetConfiguration(1);
            ((LibUsbDevice)USBDevice).ClaimInterface(0);
            return true;
        }
    }
}

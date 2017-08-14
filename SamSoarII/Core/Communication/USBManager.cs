using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.DeviceNotify;
using System.Threading;

namespace SamSoarII.Core.Communication
{
    public class USBManager : IPortManager
    {
        public const int PID = 0x5189;
        public const int VID = 0x04e8;

        public static UsbDevice USBDevice;//USB设备
        public static UsbEndpointWriter writer = null;
        public static UsbEndpointReader reader = null;

        private CommunicationManager parent;
        public CommunicationManager Parent { get { return this.parent; } }

        public USBManager(CommunicationManager _parent)
        {
            parent = _parent;
        }

        public int Abort()
        {
            CloseUSB();
            return 0;
        }
        private bool AssertCommand(ICommunicationCommand cmd)
        {
            return cmd is GeneralReadCommand;
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
        static byte[] readbuffer = new byte[4096];
        static int readBuffercount = 0;
        public int Read(ICommunicationCommand cmd)
        {
            ErrorCode ec = ErrorCode.None;
            int bytesRead;
            try
            {
                ec = reader.Read(readbuffer, readBuffercount, 4096 - readBuffercount, 1000, out bytesRead);
                readBuffercount += bytesRead;
                byte[] data = new byte[readBuffercount];
                if (ec != ErrorCode.None)
                    return 1;
                if (!AssertCommand(cmd))
                {
                    for (int i = 0; i < readBuffercount; i++)
                        data[i] = readbuffer[i];
                }
                else
                {
                    int cnt = readBuffercount / CommunicationDataDefine.USB_MAX_READ_LEN;
                    int res = readBuffercount % CommunicationDataDefine.USB_MAX_READ_LEN;
                    data = new byte[readBuffercount - 2 * cnt - (res > 0 ? 2 : 0)];
                    int cursor = 0;
                    for (int i = 0; i < cnt; i++)
                    {
                        for (int j = 2; j < CommunicationDataDefine.USB_MAX_READ_LEN; j++)
                        {
                            data[cursor++] = readbuffer[j + CommunicationDataDefine.USB_MAX_READ_LEN * i];
                        }
                    }
                    for (int i = 2; i < res; i++)
                    {
                        data[cursor++] = readbuffer[i + CommunicationDataDefine.USB_MAX_READ_LEN * cnt];
                    }
                }
                cmd.RetData = data;
            }
            catch (Exception)
            {
                return 1;
            }
            if (!cmd.IsComplete)
                return 1;
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
                {
                    CloseUSB();
                    Start();
                    Thread.Sleep(500);
                    return 1;
                }
            }
            catch (Exception)
            {
                CloseUSB();
                Start();
                Thread.Sleep(500);
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
            {
                USBDevice.Close();
                USBDevice = null;
            }
        }
        private bool FindAndOpenUSB(int VID, int PID)
        {
            UsbDeviceFinder usbFinder = new UsbDeviceFinder(VID, PID);
            UsbRegistry usbRegistry = UsbDevice.AllDevices.Find(usbFinder);
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

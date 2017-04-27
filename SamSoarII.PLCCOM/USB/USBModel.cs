﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.PLCCOM.USB
{
    public class USBModel
    {
        #region DLL Import

        [DllImport("plcusb.dll", EntryPoint = "open")]
        static extern private int USB_Open();

        [DllImport("plcusb.dll", EntryPoint = "close")]
        static extern private int USB_Close();

        [DllImport("plcusb.dll", EntryPoint = "transfer")]
        static extern private int USB_Transfer
        (
            [MarshalAs(UnmanagedType.LPArray)]
            char[] data,
            int length 
        );

        [DllImport("plcusb.dll", EntryPoint = "config")]
        static extern private void USB_Config
        (
            int bit
        );

        [DllImport("plcusb.dll", EntryPoint = "read")]
        static extern public int USB_Read
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            char[] input
        );

        [DllImport("plcusb.dll", EntryPoint = "read16")]
        static extern public int USB_Read16
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt16[] input
        );
        
        [DllImport("plcusb.dll", EntryPoint = "read32")]
        static extern public int USB_Read32
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt32[] input
        );

        [DllImport("plcusb.dll", EntryPoint = "read64")]
        static extern public int USB_Read64
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt64[] input
        );

        [DllImport("plcusb.dll", EntryPoint = "read32f")]
        static extern public int USB_Read32f
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            float[] input
        );

        [DllImport("plcusb.dll", EntryPoint = "read64f")]
        static extern public int USB_Read64f
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            double[] input
        );

        [DllImport("plcusb.dll", EntryPoint = "write")]
        static extern public int USB_Write
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            char[] output
        );


        [DllImport("plcusb.dll", EntryPoint = "write16")]
        static extern public int USB_Write16
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt16[] output
        );

        [DllImport("plcusb.dll", EntryPoint = "write32")]
        static extern public int USB_Write32
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt32[] output
        );

        [DllImport("plcusb.dll", EntryPoint = "write64")]
        static extern public int USB_Write64
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt64[] output
        );

        [DllImport("plcusb.dll", EntryPoint = "write32f")]
        static extern public int USB_Write32f
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            float[] output
        );

        [DllImport("plcusb.dll", EntryPoint = "write64f")]
        static extern public int USB_Write64f
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int length,
            [MarshalAs(UnmanagedType.LPArray)]
            double[] output
        );

        [DllImport("plcusb.dll", EntryPoint = "download")]
        static extern private void USB_Download
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string file_bin,
            [MarshalAs(UnmanagedType.LPStr)]
            string file_pro
        );

        [DllImport("plcusb.dll", EntryPoint = "upload")]
        static extern private void USB_Upload
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string file_bin,
            [MarshalAs(UnmanagedType.LPStr)]
            string file_pro
        );

        #endregion

        #region Numbers

        #region Status

        public const int STATUS_OPEN = 0x01;
        public const int STATUS_CLOSE = 0x00;
        private int status;
        public int Status
        {
            get
            {
                return this.status;
            }
            private set
            {
                this.status = value;
                int ret = 0;
                switch (value)
                {
                    case STATUS_OPEN:
                        ret = USB_Open();
                        break;
                    case STATUS_CLOSE:
                        ret = USB_Close();
                        break;
                    default:
                        ret = USBException.UNDEFINED_USBMODEL_STATUS | (value << 8);
                        break;
                }    
                if (ret != 0)
                {
                    throw new USBException(ret);
                }
            }
        }

        #endregion

        #region Device
        private PLCDevice.Device plcdevice;
        public PLCDevice.Device PLCDevice
        {
            get
            {
                return this.plcdevice;
            }
            set
            {
                this.plcdevice = value;
                USB_Config(value.BitNumber);
            }
        }
        #endregion

        #endregion
        
        public void Upload(string file_bin, string file_pro)
        {
            Status = STATUS_OPEN;
            USB_Upload(file_bin, file_pro);
            Status = STATUS_CLOSE;
        }
        
        public void Download(string file_bin, string file_pro)
        {
            Status = STATUS_OPEN;
            USB_Download(file_bin, file_pro);
            Status = STATUS_CLOSE;
        }
        
    }
}

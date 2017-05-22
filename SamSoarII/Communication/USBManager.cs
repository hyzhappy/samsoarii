using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.Communication.Command;
using System.Runtime.InteropServices;

namespace SamSoarII.Communication
{
    public class USBManager : ICommunicationManager
    {
        [DllImport("plcusb.dll", EntryPoint = "Open")]
        static extern private int Open();

        [DllImport("plcusb.dll", EntryPoint = "Close")]
        static extern private int Close();

        [DllImport("plcusb.dll", EntryPoint = "Transfer")]
        static extern private int Transfer
        (
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] data,
            int len
        );

        public int Start()
        {
            return Open();
        }

        public int Abort()
        {
            return Close();
        }
        
        public int Write(ICommunicationCommand cmd)
        {
            byte[] data = cmd.GetBytes();
            return Transfer(data, data.Length);
        }

        private byte[] readbuffer = new byte[1024];
        public int Read(ICommunicationCommand cmd)
        {
            return Transfer(readbuffer, 1024);   
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamSoarII.Communication
{
    public class SerialPortManager
    {
        public static bool CheckSerialPort()
        {
            Microsoft.VisualBasic.Devices.Computer pc = new Microsoft.VisualBasic.Devices.Computer();
            foreach (var name in pc.Ports.SerialPortNames)
            {
                MessageBox.Show(name);
            }
            SerialPort port = new SerialPort();
            return SerialPort.GetPortNames().Count() == 0;
        }
    }
}

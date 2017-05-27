using SamSoarII.AppMain.UI;
using SamSoarII.Communication.Command;
using SamSoarII.UserInterface;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace SamSoarII.Communication
{
    public class SerialPortManager : ICommunicationManager
    {
        static private string[] PORTNAMES
        {
            get
            {
                return SerialPort.GetPortNames();
            }
        }
        static private int[] BAUDRATES = { 4800, 9600, 19200, 38400, 57600, 115200 };
        static private int[] DATABITS = { 8 };
        static private int[] STOPBITS = { 1, 2 };
        static private string[] PARITYS = { "NONE", "ODD", "EVEN" };

        private SerialPort port = new SerialPort();

        public string PortName
        {
            get
            {
                return port.PortName;
            }
            set
            {
                port.PortName = value;
            }
        }

        public int BaudRate
        {
            get
            {
                return port.BaudRate;
            }
            set
            {
                port.BaudRate = value;
            }
        }

        public int DataBits
        {
            get
            {
                return port.DataBits;
            }
            set
            {
                port.DataBits = value;
            }
        }

        public int StopBits
        {
            get
            {
                switch (port.StopBits)
                {
                    case System.IO.Ports.StopBits.None: return 0;
                    case System.IO.Ports.StopBits.One: return 1;
                    case System.IO.Ports.StopBits.Two: return 2;
                    default: return 0;
                }
            }
            set
            {
                switch (value)
                {
                    case 0: port.StopBits = System.IO.Ports.StopBits.None; break;
                    case 1: port.StopBits = System.IO.Ports.StopBits.One; break;
                    case 2: port.StopBits = System.IO.Ports.StopBits.Two; break;
                }
            }
        }

        public int Timeout
        {
            get
            {
                return port.ReadTimeout;
            }
            set
            {
                port.ReadTimeout = value;
                port.WriteTimeout = value;
            }
        }

        public string Parity
        {
            get
            {
                switch (port.Parity)
                {
                    case System.IO.Ports.Parity.Even: return "EVEN";
                    case System.IO.Ports.Parity.Mark: return "MARK";
                    case System.IO.Ports.Parity.None: return "NONE";
                    case System.IO.Ports.Parity.Odd: return "ODD";
                    case System.IO.Ports.Parity.Space: return "SPACE";
                    default: return String.Empty;
                }
            }
            set
            {
                switch (value)
                {
                    case "EVEN": port.Parity = System.IO.Ports.Parity.Even; break;
                    case "MARK": port.Parity = System.IO.Ports.Parity.Mark; break;
                    case "NONE": port.Parity = System.IO.Ports.Parity.None; break;
                    case "ODD": port.Parity = System.IO.Ports.Parity.Odd; break;
                    case "SPACE": port.Parity = System.IO.Ports.Parity.Space; break;
                }
            }
        }
        public void InitializePort()
        {
            CommunicationParams paras = (CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"];
            PortName = PORTNAMES[paras.SerialPortIndex];
            BaudRate = BAUDRATES[paras.BaudRateIndex];
            DataBits = DATABITS[paras.DataBitIndex];
            StopBits = STOPBITS[paras.StopBitIndex];
            Timeout = paras.Timeout;
            Parity = PARITYS[paras.CheckCodeIndex];
        }
        public int Start()
        {
            try
            {
                if (!((CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"]).IsAutoCheck)
                {
                    InitializePort();
                }
                if (!port.IsOpen)
                {
                    port.Open();
                }
            }
            catch (Exception)
            {
                return 1;
            }
            return 0;
        }

        public int Abort()
        {
            try
            {
                port.Close();
            }
            catch (Exception)
            {
                return 1;
            }
            return 0;
        }

        public int Write(ICommunicationCommand cmd)
        {
            try
            {
                byte[] data = cmd.GetBytes();
                port.Write(data, 0, data.Length);
            }
            catch (Exception)
            {
                return 1;
            }
            return 0;
        }

        static byte[] readbuffer = new byte[1024];
        public int Read(ICommunicationCommand cmd)
        {
            try
            {
                int count = port.Read(readbuffer, 0, 1024);
                byte[] data = new byte[count];
                readbuffer.CopyTo(data, count);
                cmd.RetData = data;
            }
            catch (Exception)
            {
                return 1;
            }
            return 0;
        }
        
        //public void Load()
        //{
        //    CommunicationParams paras = (CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"];
        //    PortName = PORTNAMES[paras.SerialPortIndex];
        //    BaudRate = BAUDRATES[paras.BaudRateIndex];
        //    DataBits = DATABITS[paras.DataBitIndex];
        //    StopBits = STOPBITS[paras.StopBitIndex];
        //    Timeout = paras.Timeout;
        //    Parity = PARITYS[paras.CheckCodeIndex];
        //}
        
        public bool AutoCheck()
        {
            foreach (string _portname in PORTNAMES)
            {
                if (port != null && port.PortName != _portname)
                {
                    port = new SerialPort(_portname);
                }
                try
                {
                    if (!port.IsOpen)
                    {
                        port.Open();
                        port.Close();
                    }
                    return true;
                }
                catch (Exception)
                {
                }
            }
            return false;
        }
        
    }
}

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
using SamSoarII.AppMain.UI.Monitor;
using System.Threading;

namespace SamSoarII.Communication
{
    public class SerialPortManager : ICommunicationManager
    {
        static private byte[] TESTBYTES;
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
        static SerialPortManager()
        {
            byte[] tempbytes = new byte[] { 0X01, 0XFE, 0X01, 0X00, 0X01, 0X00, 0X00 };
            byte[] CRC = CRC16.GetCRC(tempbytes);
            TESTBYTES = new byte[tempbytes.Length + 2];
            for (int i = 0; i < tempbytes.Length; i++)
            {
                TESTBYTES[i] = tempbytes[i];
            }
            TESTBYTES[TESTBYTES.Length - 2] = CRC[0];
            TESTBYTES[TESTBYTES.Length - 1] = CRC[1];
        }
        private bool IsSuccess = false;
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
            if (port.IsOpen) port.Close();
            CommunicationParams paras = (CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"];
            PortName = PORTNAMES[paras.SerialPortIndex];
            BaudRate = BAUDRATES[paras.BaudRateIndex];
            DataBits = DATABITS[paras.DataBitIndex];
            StopBits = STOPBITS[paras.StopBitIndex];
            Timeout = paras.Timeout;
            Parity = PARITYS[paras.CheckCodeIndex];
        }
        private void SetParas()
        {
            CommunicationParams paras = (CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"];
            paras.SerialPortIndex = PORTNAMES.ToList().IndexOf(port.PortName);
            paras.BaudRateIndex = BAUDRATES.ToList().IndexOf(port.BaudRate);
            paras.StopBitIndex = STOPBITS.ToList().IndexOf(GetStopBits(port.StopBits));
            paras.Timeout = Timeout;
            paras.CheckCodeIndex = PARITYS.ToList().IndexOf(GetParity(port.Parity));
        }
        private int GetStopBits(StopBits stopBits)
        {
            switch (stopBits)
            {
                case System.IO.Ports.StopBits.None:
                    return 0;
                case System.IO.Ports.StopBits.One:
                    return 1;
                case System.IO.Ports.StopBits.Two:
                    return 2;
                default:
                    return 0;
            }
        }
        private string GetParity(Parity parity)
        {
            switch (parity)
            {
                case System.IO.Ports.Parity.None:
                    return "NONE";
                case System.IO.Ports.Parity.Odd:
                    return "ODD";
                case System.IO.Ports.Parity.Even:
                    return "EVEN";
                case System.IO.Ports.Parity.Mark:
                    return "MARK";
                case System.IO.Ports.Parity.Space:
                    return "SPACE";
                default:
                    return "NONE";
            }
        }
        public int Start()
        {
            try
            {
                if (!((CommunicationParams)ProjectPropertyManager.ProjectPropertyDic["CommunicationParams"]).IsAutoCheck)
                {
                    InitializePort();
                    if (!PortTest())
                    {
                        IsSuccess = false;
                        return 1;
                    } 
                }
            }
            catch (Exception)
            {
                return 1;
            }
            readbuffercount = 0;
            IsSuccess = true;
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
        static byte[] readbuffer = new byte[4096];
        static int readbuffercount = 0;
        public int Read(ICommunicationCommand cmd)
        {
            try
            {
                int count = port.Read(readbuffer, readbuffercount, 4096 - readbuffercount);
                readbuffercount += count;
                byte[] data = new byte[readbuffercount];
                for (int i = 0; i < readbuffercount; i++)
                    data[i] = readbuffer[i];
                cmd.RetData = data;
            }
            catch (Exception)
            {
                return 1;
            }
            if (!cmd.IsComplete)
                return 1;
            readbuffercount = 0;
            return 0;
        }
        
        public bool AutoCheck()
        {
            if (IsSuccess) return true;
            foreach (string _portname in PORTNAMES)
            {
                if (port != null && port.PortName != _portname)
                {
                    port = new SerialPort(_portname);
                }
                try
                {
                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                    return ParamsTest();
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
        private bool ParamsTest()
        {
            for (int i = BAUDRATES.Length - 1; i > 0 ; i--)
            {
                BaudRate = BAUDRATES[i];
                for (int j = 0; j < PARITYS.Length; j++)
                {
                    Parity = PARITYS[j];
                    for (int k = 0; k < STOPBITS.Length; k++)
                    {
                        StopBits = STOPBITS[k];
                        if (PortTest())
                        {
                            SetParas();
                            return true;
                        } 
                    }
                }
            }
            return false;
        }
        private bool PortTest()
        {
            port.Open();
            port.Write(TESTBYTES, 0, TESTBYTES.Length);
            Thread.Sleep(10);
            try
            {
                int recvcount = 0;
                readbuffercount = 0;
                do
                {
                    readbuffercount += port.BytesToRead;
                    recvcount++;
                    Thread.Sleep(10);
                }
                while (readbuffercount != 8 && recvcount < 5);
                if (readbuffercount == 8)
                {
                    port.Read(new byte[8],0, readbuffercount);
                    readbuffercount = 0;
                    return true;
                }
            }
            catch (TimeoutException)
            {
                port.Close();
                return false;
            }
            port.Close();
            return false;
        }
    }
}

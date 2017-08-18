using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace SamSoarII.Core.Communication
{
    public class SerialPortManager : IPortManager
    {
        static private CommunicationTestCommand testCommand = new CommunicationTestCommand();
        static public string[] PORTNAMES
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
        private CommunicationManager parent;
        public CommunicationManager Parent { get { return this.parent; } }
        public SerialPortManager(CommunicationManager _parent)
        {
            parent = _parent;
        }
        private bool IsSuccess = false;
        public int OverTime
        {
            get
            {
                if (port == null) return 50;
                else
                {
                    switch (port.BaudRate)
                    {
                        case 4800:
                            return 300;
                        case 9600:
                            return 150;
                        case 19200:
                            return 100;
                        case 38400:
                            return 80;
                        case 57600:
                            return 50;
                        case 115200:
                            return 20;
                        default:
                            return 50;
                    }
                }
            }
        }

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
            CommunicationParams paras = parent.PARACom;
            if(PORTNAMES.Count() > 0)
                PortName = PORTNAMES[paras.SerialPortIndex];
            BaudRate = BAUDRATES[paras.BaudRateIndex];
            DataBits = DATABITS[paras.DataBitIndex];
            StopBits = STOPBITS[paras.StopBitIndex];
            Timeout = paras.Timeout;
            Parity = PARITYS[paras.CheckCodeIndex];
        }
        private void SetParas()
        {
            CommunicationParams paras = parent.PARACom;
            paras.SerialPortIndex = PORTNAMES.ToList().IndexOf(port.PortName);
            paras.BaudRateIndex = BAUDRATES.ToList().IndexOf(port.BaudRate);
            paras.StopBitIndex = STOPBITS.ToList().IndexOf(GetStopBits(port.StopBits));
            paras.Timeout = Timeout > 0 ? Timeout : 200;
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
            InitializePort();
            try
            {
                CommunicationParams paras = parent.PARACom;
                if (!paras.IsAutoCheck)
                {
                    if (!PortTest())
                    {
                        IsSuccess = false;
                        return 1;
                    }
                }
            }
            catch (Exception e)
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
                IsSuccess = false;
                readbuffercount = 0;
                if (port.IsOpen) port.Close();
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
                if (!port.IsOpen) port.Open();
                byte[] data = cmd.GetBytes();
                port.Write(data, 0, data.Length);
            }
            catch (Exception e)
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
            catch (Exception e)
            {
                if ((e.GetType() == typeof(TimeoutException) || e.GetType() == typeof(InvalidOperationException)) && AssertCmd(cmd))
                {
                    cmd.IsComplete = true;
                    cmd.IsSuccess = false;
                    readbuffercount = 0;
                    Thread.Sleep(200);
                    return 0;
                }
                return 1;
            }
            if (!cmd.IsComplete)
                return 1;
            readbuffercount = 0;
            return 0;
        }
        private bool AssertCmd(ICommunicationCommand cmd)
        {
            return cmd is GeneralReadCommand || cmd is ForceCancelCommand
                || cmd is GeneralWriteCommand || cmd is IntrasegmentWriteCommand;
        }

        public bool AutoCheck()
        {
            if (IsSuccess) return true;
            foreach (string _portname in PORTNAMES)
            {
                if (port?.PortName != _portname)
                {
                    port.Close();
                    port = new SerialPort(_portname);
                }
                try
                {
                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                    bool ret = ParamsTest();
                    if (ret) return true;
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
            for (int i = BAUDRATES.Length - 1; i > 0; i--)
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
            byte[] command = testCommand.GetBytes();
            port.Write(command, 0, command.Length);
            try
            {
                int recvcount = 0;
                readbuffercount = 0;
                do
                {
                    readbuffercount += port.BytesToRead;
                    recvcount++;
                    Thread.Sleep(OverTime);
                }
                while (readbuffercount < 3 && recvcount < 5);
                if (readbuffercount >= 3)
                {
                    byte[] data = new byte[readbuffercount];
                    port.Read(data, 0, readbuffercount);
                    int len = ValueConverter.GetValueByBytes(data[1],data[2]);
                    recvcount = 0;
                    while (readbuffercount != len && recvcount < 5)
                    {
                        readbuffercount += port.BytesToRead;
                        recvcount++;
                        Thread.Sleep(10);
                    }
                    if (readbuffercount == len)
                    {
                        byte[] data2 = new byte[len - data.Length];
                        port.Read(data2, 0, len - data.Length);
                        data = data.Concat(data2).ToArray();
                        testCommand.RetData = data;
                        return testCommand.IsSuccess;
                    }
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

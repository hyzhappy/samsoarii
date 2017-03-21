using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSoarII.Simulation.Core
{
    public class SimulateExecModel : Form
    {
        private const int USER = 0x500;
        private const int WM_GETBASEADDRESS = USER + 1;
        private const int WM_SIMUMODEL_CLOSE = USER + 2;

        private const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        private const int PROCESS_VM_READ = 0x0010;
        private const int PROCESS_VM_WRITE = 0x0020;

        private class TagPOINT
        {
            public long x;
            public long y;
        }

        private class TagMSG
        {
            public IntPtr hWnd;
            public uint msg;
            public int wPara;
            public long lPara;
            public uint time;
            //public TagPOINT pt;
            public int pt;
        }

        private class SendDataStruct
        {
            public IntPtr dwData;
            public int length;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
        
        private class RecvDataStruct_GetBaseAddr
        {
            public IntPtr XBit;
            public IntPtr YBit;
            public IntPtr MBit;
            public IntPtr CBit;
            public IntPtr TBit;
            public IntPtr SBit;
            public IntPtr DWord;
            public IntPtr CVWord;
            public IntPtr CVDoubleWord;
            public IntPtr TVWord;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10 * 4)]
            //public IntPtr[] baseaddrs;
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern IntPtr SendMessage
        (
            IntPtr hWnd, 
            int wMsg, 
            int wPara, 
            ref SendDataStruct lPara
        );
        
        [DllImport("User32.dll", EntryPoint = "GetMessage")]
        private static extern int GetMessage
        (
            ref TagMSG lpMsg, 
            IntPtr hWnd, 
            int wFMin, 
            int wFMax
        );

        [DllImport("User32.dll", EntryPoint = "TranslateMessage")]
        private static extern int TranslateMessage
        (
            ref TagMSG lpMsg
        );

        [DllImport("User32.dll", EntryPoint = "DispatchMessage")]
        private static extern int DispatchMessage
        (
            ref TagMSG lpMsg
        );

        [DllImport("User32.dll")]
        private static extern int GetWindowThreadProcessId
        (
            IntPtr hWnd,
            ref int lpdwProcessId
        );
        
        [DllImport("Kernel32.dll")]
        private static extern IntPtr OpenProcess
        (
            int dwDesireAccess,
            bool bInheritHandle,
            int dwProcessId
        );

        [DllImport("Kernel32.dll")]
        private static extern bool ReadProcessMemory
        (
            IntPtr hWnd,
            IntPtr lpBaseAddress,
            [MarshalAs(UnmanagedType.LPArray)]
            ref byte[] lpBuffer,
            int nSize,
            ref int lpNumberOfBytesRead
        );

        [DllImport("Kernel32.dll")]
        private static extern bool WriteProcessMemory
        (
            IntPtr hWnd,
            IntPtr lpBaseAddress,
            [MarshalAs(UnmanagedType.LPArray)]
            ref byte[] lpBuffer,
            int nSize,
            ref int lpNumberOfBytesWrite
        );

        private IntPtr eWnd;
        private IntPtr ePro;

        private bool BAenable = false;

        public bool BaseAddrEnable
        {
            get { return this.BAenable; }
        }
        
        private IntPtr XBit;
        private IntPtr YBit;
        private IntPtr MBit;
        private IntPtr CBit;
        private IntPtr TBit;
        private IntPtr SBit;
        private IntPtr DWord;
        private IntPtr CVWord;
        private IntPtr CVDoubleWord;
        private IntPtr TVWord; 
    
        public SimulateExecModel() : base()
        {
            XBit = YBit = MBit = CBit = TBit = SBit
          = DWord = CVWord = CVDoubleWord = TVWord = IntPtr.Zero;
        }

        public void Start(string execpath)
        {
            this.BAenable = false;
            //Process cproc = Process.GetCurrentProcess();
            //IntPtr cphd = cproc.MainWindowHandle;
            IntPtr cphd = this.Handle;
            Process eproc = new Process();
            eproc.StartInfo.FileName = execpath;
            eproc.StartInfo.Arguments = String.Format("{0:d}", cphd);
            eproc.StartInfo.UseShellExecute = false;
            eproc.StartInfo.RedirectStandardInput = true;
            eproc.StartInfo.RedirectStandardOutput = true;
            eproc.StartInfo.RedirectStandardError = true;
            
            if (eproc.Start())
            {
                eWnd = eproc.MainWindowHandle;
                int procId = 0;
                GetWindowThreadProcessId(eWnd, ref procId);
                ePro = OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE, false, procId);

                Thread thread = new Thread(GetProcessThread);
                thread.Start();
            }
        }

        private void GetProcessThread()
        {
            TagMSG msg = new TagMSG();
            while (this.Enabled)
            {
                if (GetMessage(ref msg, IntPtr.Zero, 0, 0) > 0)
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }
            }
        }

        private void SendMessageToProcess(int msgId)
        {
            SendDataStruct sds = new SendDataStruct();
            SendMessage(eWnd, msgId, 0, ref sds);
        }
        
        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_GETBASEADDRESS:
                    RecvDataStruct_GetBaseAddr rds = new RecvDataStruct_GetBaseAddr();
                    rds = (RecvDataStruct_GetBaseAddr)(m.GetLParam(rds.GetType()));
                    XBit = rds.XBit;
                    YBit = rds.YBit;
                    MBit = rds.MBit;
                    CBit = rds.CBit;
                    TBit = rds.TBit;
                    SBit = rds.SBit;
                    DWord = rds.DWord;
                    CVWord = rds.CVWord;
                    CVDoubleWord = rds.CVDoubleWord;
                    TVWord = rds.TVWord;
                    this.BAenable = true;
                    break;
                default:

                    base.DefWndProc(ref m);
                    break;
            }
        }

        public new void Close()
        {
            base.Close();
            SendMessageToProcess(WM_SIMUMODEL_CLOSE);
        }
        
        public void SetBit(string name, UInt32 value)
        {
            UInt32[] varray = { value };
            SetBit(name, varray);
        }
        
        public void SetBit(string name, UInt32[] value)
        {
            string type = name.Substring(0, 1);
            int baseaddr = int.Parse(name.Substring(1));
            IntPtr Bit;
            switch (type)
            {
                case "X": Bit = XBit; break;
                case "Y": Bit = YBit; break;
                case "M": Bit = MBit; break;
                case "S": Bit = SBit; break;
                case "C": Bit = CBit; break;
                case "T": Bit = TBit; break;
                default: return;
            }
            byte[] wDatas = new byte[value.Length * 4];
            for (int i = 0; i < value.Length; i++)
            {
                wDatas[i << 2] = (byte)(value[i]&1);
                wDatas[(i << 2) + 1] = wDatas[(i << 2) + 2] = wDatas[(i << 2) + 3] = 0;
            }
            int writeByte = 0; 
            WriteProcessMemory(ePro, Bit + (baseaddr << 2), ref wDatas, wDatas.Length, ref writeByte);
        }

        public UInt32 GetBit(string name)
        {
            UInt32[] ret = GetBit(name, 1);
            if (ret != null && ret.Length > 0)
                return ret[0];
            return 0;
        }

        public UInt32[] GetBit(string name, int size)
        {
            string type = name.Substring(0, 1);
            int baseaddr = int.Parse(name.Substring(1));
            IntPtr Bit;
            switch (type)
            {
                case "X": Bit = XBit; break;
                case "Y": Bit = YBit; break;
                case "M": Bit = MBit; break;
                case "S": Bit = SBit; break;
                case "C": Bit = CBit; break;
                case "T": Bit = TBit; break;
                default: return null;
            }
            byte[] rDatas = new byte[size * 4];
            int readByte = 0;
            ReadProcessMemory(ePro, Bit + (baseaddr << 2), ref rDatas, rDatas.Length, ref readByte);
            UInt32[] ret = new UInt32[size];
            for (int i = 0; i < size; i++)
            {
                ret[i] = (UInt32)(rDatas[i << 2]);
            }
            return ret;
        }
        
        public void SetWord(string name, UInt16 value)
        {
            UInt16[] varray = { value };
            SetWord(name, varray);
        }

        public void SetWord(string name, UInt16[] value)
        {
            string type = name.Substring(0, 1);
            int baseaddr = int.Parse(name.Substring(1));
            IntPtr Word;
            switch (type)
            {
                case "D": Word = DWord; break;
                case "CV": Word = CVWord; break;
                case "TV": Word = TVWord; break;
                default: return;
            }
            byte[] wDatas = new byte[value.Length * 4];
            for (int i = 0; i < value.Length; i++)
            {
                wDatas[i << 1] = (byte)(value[i]&255);
                wDatas[(i << 1) + 1] = (byte)(value[i]>>8);
            }
            int writeByte = 0;
            WriteProcessMemory(ePro, Word + (baseaddr << 1), ref wDatas, wDatas.Length, ref writeByte);
        }

        public UInt16 GetWord(string name)
        {
            UInt16[] ret = GetWord(name, 1);
            if (ret != null && ret.Length > 0)
                return ret[0];
            return 0;
        }

        public UInt16[] GetWord(string name, int size)
        {
            string type = name.Substring(0, 1);
            int baseaddr = int.Parse(name.Substring(1));
            IntPtr Word;
            switch (type)
            {
                case "D": Word = DWord; break;
                case "CV": Word = CVWord; break;
                case "TV": Word = TVWord; break;
                default: return null;
            }
            byte[] rDatas = new byte[size << 1];
            int readByte = 0;
            ReadProcessMemory(ePro, Word + (baseaddr << 1), ref rDatas, rDatas.Length, ref readByte);
            UInt16[] ret = new UInt16[size];
            for (int i = 0; i < size; i++)
            {
                ret[i] = (UInt16)(((UInt16)(rDatas[i<<1])) | (((UInt16)(rDatas[(i<<1)+1]))<<8));
            }
            return ret;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamSoarII.Simulation.Core
{
    public class SimulateDllModel
    {
        [DllImport("simu.dll", EntryPoint = "CreateDll")]
        public static extern void CreateDll
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string simucPath,
            [MarshalAs(UnmanagedType.LPStr)]
            string simufPath,
            [MarshalAs(UnmanagedType.LPStr)]
            string simudllPath,
            [MarshalAs(UnmanagedType.LPStr)]
            string simuaPath
        );

        public const int LOADDLL_OK = 0x00;
        public const int LOADDLL_CANNOT_FOUND_DLLFILE = 0x01;
        public const int LOADDLL_CANNOT_FOUND_RUNLADDER = 0x02;
        public const int LOADDLL_CANNOT_FOUND_GETBIT = 0x03;
        public const int LOADDLL_CANNOT_FOUND_GETWORD = 0x04;
        public const int LOADDLL_CANNOT_FOUND_GETDWORD = 0x05;
        public const int LOADDLL_CANNOT_FOUND_GETFLOAT = 0x06;
        public const int LOADDLL_CANNOT_FOUND_GETDOUBLE = 0x07;
        public const int LOADDLL_CANNOT_FOUND_SETBIT = 0x08;
        public const int LOADDLL_CANNOT_FOUND_SETWORD = 0x09;
        public const int LOADDLL_CANNOT_FOUND_SETDWORD = 0x0A;
        public const int LOADDLL_CANNOT_FOUND_SETFLOAT = 0x0B;
        public const int LOADDLL_CANNOT_FOUND_SETDOUBLE = 0x0C;

        [DllImport("simu.dll", EntryPoint = "LoadDll")]
        public static extern int LoadDll
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string simudllPath
        );

        [DllImport("simu.dll", EntryPoint = "RunLadder")]
        private static extern void RunLadder();

        [DllImport("simu.dll", EntryPoint = "GetBit")]
        private static extern void GetBit
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt32[] output
        );

        [DllImport("simu.dll", EntryPoint = "GetWord")]
        private static extern void GetWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt16[] output
        );

        [DllImport("simu.dll", EntryPoint = "GetDoubleWord")]
        private static extern void GetDoubleWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt32[] output
        );

        [DllImport("simu.dll", EntryPoint = "GetFloat")]
        private static extern void GetFloat
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            float[] output
        );

        [DllImport("simu.dll", EntryPoint = "GetDouble")]
        private static extern void GetDouble
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            double[] output
        );

        [DllImport("simu.dll", EntryPoint = "SetBit")]
        private static extern void SetBit
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt32[] input
        );

        [DllImport("simu.dll", EntryPoint = "SetWord")]
        private static extern void SetWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt16[] input
        );

        [DllImport("simu.dll", EntryPoint = "SetDoubleWord")]
        private static extern void SetDoubleWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt32[] input
        );

        [DllImport("simu.dll", EntryPoint = "SetFloat")]
        private static extern void SetFloat
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            float[] input
        );

        [DllImport("simu.dll", EntryPoint = "SetDouble")]
        private static extern void SetDouble
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            double[] input
        );

        [DllImport("simu.dll", EntryPoint = "SetEnable")]
        private static extern void SetEnable
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            int value
        );

        private bool simulateActive;
        private Thread simulateThread;

        public SimulateDllModel()
        {
            simulateActive = true;
            //simulateThread = new Thread(_SimulateThread);
        }
        
        private void _SimulateThread()
        {
            while (simulateActive)
            {
                RunLadder();
                Thread.Sleep(20);
            }
        }

        public void Start()
        {
            simulateThread = new Thread(_SimulateThread);
            simulateThread.Start();
        }
        
        public void Abort()
        {
            simulateThread.Abort();
        }

        public int[] GetValue_Bit(string var, int size)
        {
            int i = 0;
            while (char.IsLetter(var[i])) i++;

            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            int[] ret = new int[size];
            UInt32[] _ret32;
            switch (name)
            {
                case "X": case "Y": case "M":case "C":case "T":case "S":
                    _ret32 = new UInt32[size];
                    GetBit(var, size, _ret32);
                    for (i = 0; i < size; i++)
                        ret[i] = (int)(_ret32[i]);
                    return ret;
                default:
                    return null;
            }
        }

        public int[] GetValue_Word(string var, int size)
        {
            int i = 0;
            while (char.IsLetter(var[i])) i++;

            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            int[] ret = new int[size];
            UInt32[] _ret32;
            UInt16[] _ret16;
            switch (name)
            {
                case "X": case "Y": case "M": case "C": case "T": case "S":
                    _ret32 = new UInt32[size];
                    GetBit(var, size, _ret32);
                    for (i = 0; i < size; i++)
                        ret[i] = (int)(_ret32[i]);
                    return ret;
                case "D": case "TV":
                    _ret16 = new UInt16[size];
                    GetWord(var, size, _ret16);
                    for (i = 0; i < size; i++)
                        ret[i] = (int)(_ret16[i]);
                    return ret;
                case "CV":
                    if (addr < 200)
                    {
                        _ret16 = new UInt16[size];
                        GetWord(var, size, _ret16);
                        for (i = 0; i < size; i++)
                            ret[i] = _ret16[i];
                        return ret;
                    }
                    else
                    {
                        _ret32 = new UInt32[size];
                        GetDoubleWord(var, size, _ret32);
                        for (i = 0; i < size; i++)
                            ret[i] = (int)(_ret32[i]);
                        return ret;
                    }
                default:
                    return null;
            }
        }

        public int[] GetValue_DWord(string var, int size)
        {
            int i = 0;
            while (char.IsLetter(var[i])) i++;

            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            int[] ret = new int[size];
            UInt32[] _ret32;
            
            switch (name)
            {
                case "D": case "CV":
                    _ret32 = new UInt32[size];
                    GetDoubleWord(var, size, _ret32);
                    for (i = 0; i < size; i++)
                        ret[i] = (int)(_ret32[i]);
                    return ret;
                default:
                    return null;
            }
        }

        public float[] GetValue_Float(string var, int size)
        {
            int i = 0;
            while (char.IsLetter(var[i])) i++;

            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            float[] ret = new float[size];
            
            switch (name)
            {
                case "D":
                    GetFloat(var, size, ret);
                    return ret;
                default:
                    return null;
            }
        }

        public double[] GetValue_Double(string var, int size)
        {
            int i = 0;
            while (char.IsLetter(var[i])) i++;

            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            double[] ret = new double[size];

            switch (name)
            {
                case "D":
                    GetDouble(var, size, ret);
                    return ret;
                default:
                    return null;
            }
        }

        public void SetValue_Bit(string var, int size, int[] input)
        {
            int i = 0;
            while (char.IsLetter(var[i])) i++;

            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            UInt32[] _ret32;
            switch (name)
            {
                case "X":
                case "Y":
                case "M":
                case "C":
                case "T":
                case "S":
                    _ret32 = new UInt32[size];
                    for (i = 0; i < size; i++)
                        _ret32[i] = (UInt32)(input[i]);
                    SetBit(var, size, _ret32);
                    break;
                default:
                    break;
            }
        }

        public void SetValue_Word(string var, int size, int[] input)
        {
            int i = 0;
            while (char.IsLetter(var[i])) i++;

            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            UInt32[] _ret32;
            UInt16[] _ret16;
            switch (name)
            {
                case "X":
                case "Y":
                case "M":
                case "C":
                case "T":
                case "S":
                    _ret32 = new UInt32[size];
                    for (i = 0; i < size; i++)
                        _ret32[i] = (UInt32)(input[i]);
                    SetBit(var, size, _ret32);
                    break;
                case "D":
                case "TV":
                    _ret16 = new UInt16[size];
                    for (i = 0; i < size; i++)
                        _ret16[i] = (UInt16)(input[i]);
                    SetWord(var, size, _ret16);
                    break;
                case "CV":
                    if (addr < 200)
                    {
                        _ret16 = new UInt16[size];
                        for (i = 0; i < size; i++)
                            _ret16[i] = (UInt16)(input[i]);
                        SetWord(var, size, _ret16);
                        break;
                    }
                    else
                    {
                        _ret32 = new UInt32[size];
                        for (i = 0; i < size; i++)
                            _ret32[i] = (UInt32)(input[i]);
                        SetDoubleWord(var, size, _ret32);
                        break;
                    }
                default:
                    break;
            }
        }

        public void SetValue_DWord(string var, int size, int[] input)
        {
            int i = 0;
            while (char.IsLetter(var[i])) i++;

            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            UInt32[] _ret32;

            switch (name)
            {
                case "D":
                case "CV":
                    _ret32 = new UInt32[size];
                    for (i = 0; i < size; i++)
                        _ret32[i] = (UInt32)(input[i]);
                    GetDoubleWord(var, size, _ret32);
                    break;
                default:
                    break;
            }
        }

        public void SetValue_Float(string var, int size, float[] input)
        {
            int i = 0;
            while (char.IsLetter(var[i])) i++;

            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));

            switch (name)
            {
                case "D":
                    SetFloat(var, size, input);
                    break;
                default:
                    break;
            }
        }

        public void SetValue_Double(string var, int size, double[] input)
        {
            int i = 0;
            while (char.IsLetter(var[i])) i++;

            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));

            switch (name)
            {
                case "D":
                    SetDouble(var, size, input);
                    break;
                default:
                    break;
            }
        }

        public void Lock(string var, int size = 1)
        {
            SetEnable(var, size, 1);
        }

        public void Unlock(string var, int size = 1)
        {
            SetEnable(var, size, 0);
        }
    }
}

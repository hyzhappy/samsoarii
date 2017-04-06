using SamSoarII.Simulation.Core.DataModel;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Windows;
using SamSoarII.Simulation.UI.Chart;

namespace SamSoarII.Simulation.Core
{
    public class SimulateDllModel
    {
        #region Import DLL

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
        public const int LOADDLL_CANNOT_FOUNF_SETENABLE = 0x0D;
        public const int LOADDLL_CANNOT_FOUND_INITDATAPOINT = 0x0E;
        public const int LOADDLL_CANNOT_FOUND_ADDBITDATAPOINT = 0x0F;
        public const int LOADDLL_CANNOT_FOUND_ADDWORDDATAPOINT = 0x10;
        public const int LOADDLL_CANNOT_FOUND_ADDDWORDDATAPOINT = 0x11;
        public const int LOADDLL_CANNOT_FOUND_ADDFLOATDATAPOINT = 0x12;
        public const int LOADDLL_CANNOT_FOUND_ADDDOUBLEATAPOINT = 0x13;
        public const int LOADDLL_CANNOT_FOUND_REMOVEBITDATAPOINT = 0x14;
        public const int LOADDLL_CANNOT_FOUND_REMOVEWORDDATAPOINT = 0x15;
        public const int LOADDLL_CANNOT_FOUND_REMOVEDWORDDATAPOINT = 0x16;
        public const int LOADDLL_CANNOT_FOUND_REMOVEFLOATDATAPOINT = 0x17;
        public const int LOADDLL_CANNOT_FOUND_REMOVEDOUBLEATAPOINT = 0x18;
        public const int LOADDLL_CANNOT_FOUND_ADDVIEWINPUT = 0x19;
        public const int LOADDLL_CANNOT_FOUND_ADDVIEWOUTPUT = 0x1A;
        public const int LOADDLL_CANNOT_FOUND_REMOVEVIEWINPUT = 0x1B;
        public const int LOADDLL_CANNOT_FOUNd_REMOVEVIEWOUTPUT = 0x1C;
        public const int LOADDLL_CANNOT_FOUND_BEFORERUNLADDER = 0x1D;
        public const int LOADDLL_CANNOT_FOUND_AFTERRUNLADDER = 0x1E;
        public const int LOADDLL_CANNOT_FOUND_RUNDATA = 0x1F;

        [DllImport("simu.dll", EntryPoint = "LoadDll")]
        public static extern int LoadDll
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string simudllPath
        );

        [DllImport("simu.dll", EntryPoint = "FreeDll")]
        public static extern void FreeDll
        (
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

        [DllImport("simu.dll", EntryPoint = "InitDataPoint")]
        public static extern void InitDataPoint
        (
        );

        [DllImport("simu.dll", EntryPoint = "AddBitDataPoint")]
        private static extern void AddBitDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt32 value
        );

        [DllImport("simu.dll", EntryPoint = "AddWordDataPoint")]
        private static extern void AddWordDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt16 value
        );

        [DllImport("simu.dll", EntryPoint = "AddDWordDataPoint")]
        private static extern void AddDWordDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt32 value
        );

        [DllImport("simu.dll", EntryPoint = "AddFloatDataPoint")]
        private static extern void AddFloatDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            float value
        );

        [DllImport("simu.dll", EntryPoint = "AddDoubleDataPoint")]
        private static extern void AddDoubleDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            double value
        );

        [DllImport("simu.dll", EntryPoint = "RemoveBitDataPoint")]
        private static extern void RemoveBitDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt32 value
        );

        [DllImport("simu.dll", EntryPoint = "RemoveWordDataPoint")]
        private static extern void RemoveWordDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt16 value
        );

        [DllImport("simu.dll", EntryPoint = "RemoveDWordDataPoint")]
        private static extern void RemoveDWordDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt32 value
        );

        [DllImport("simu.dll", EntryPoint = "RemoveFloatDataPoint")]
        private static extern void RemoveFloatDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            float value
        );

        [DllImport("simu.dll", EntryPoint = "RemoveDoubleDataPoint")]
        private static extern void RemoveDoubleDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            double value
        );

        private const int DP_TYPE_BIT = 0x01;
        private const int DP_TYPE_WORD = 0x02;
        private const int DP_TYPE_DOUBLEWORD = 0x03;
        private const int DP_TYPE_FLOAT = 0x04;
        private const int DP_TYPE_DOUBLE = 0x05;

        [DllImport("simu.dll", EntryPoint = "AddViewInput")]
        private static extern void AddViewInput
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int type
        );

        [DllImport("simu.dll", EntryPoint = "AddViewOutput")]
        private static extern void AddViewOutput
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int type
        );

        [DllImport("simu.dll", EntryPoint = "RemoveViewInput")]
        private static extern void RemoveViewInput
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int type
        );

        [DllImport("simu.dll", EntryPoint = "RemoveViewOutput")]
        private static extern void RemoveViewOutput
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int type
        );

        [DllImport("simu.dll", EntryPoint = "BeforeRunLadder")]
        private static extern void BeforeRunLadder();

        [DllImport("simu.dll", EntryPoint = "AfterRunLadder")]
        private static extern void AfterRunLadder();

        [DllImport("simu.dll", EntryPoint = "RunData")]
        private static extern void RunData
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string outputFile,
            int starttime,
            int endtime
        );
        #endregion

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
                BeforeRunLadder();
                RunLadder();
                AfterRunLadder();
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

        public void RunData(double starttime, double endtime)
        {
            RunData("simulog.log", (int)(starttime), (int)(endtime));
            SimulateDataModelEventArgs e = new SimulateDataModelEventArgs();
            e.TimeStart = starttime;
            e.TimeEnd = endtime;
            if (RunDataFinished != null)
            {
                RunDataFinished(this, e);
            }
        }

        public void RunDraw(double starttime, double endtime)
        {
            RunData("simulog.log", (int)(starttime), (int)(endtime));
            SimulateDataModelEventArgs e = new SimulateDataModelEventArgs();
            e.TimeStart = starttime;
            e.TimeEnd = endtime;
            if (RunDrawFinished != null)
            {
                RunDrawFinished(this, e);
            }
        }

        #region Get & Set Value

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

        #endregion

        #region Lock Value

        public void Lock(string var, int size = 1)
        {
            SetEnable(var, size, 1);
        }

        public void Unlock(string var, int size = 1)
        {
            SetEnable(var, size, 0);
        }
        
        private List<SimulateDataModel> locksdmodels = new List<SimulateDataModel>();
        
        public void Lock(SimulateDataModel sdmodel)
        {
            foreach (ValueSegment vs in sdmodel.Values)
            {
                switch (sdmodel.Type)
                {
                    case "BIT":
                        AddBitDataPoint(sdmodel.Name, vs.TimeStart, (uint)((int)(vs.Value)));
                        break;
                    case "WORD":
                        AddWordDataPoint(sdmodel.Name, vs.TimeStart, (UInt16)((int)(vs.Value)));
                        break;
                    case "DWORD":
                        AddDWordDataPoint(sdmodel.Name, vs.TimeStart, (uint)((int)(vs.Value)));
                        break;
                    case "FLOAT":
                        AddFloatDataPoint(sdmodel.Name, vs.TimeStart, (float)(vs.Value));
                        break;
                    case "DOUBLE":
                        AddDoubleDataPoint(sdmodel.Name, vs.TimeStart, (double)(vs.Value));
                        break;
                }
            }
        }
        
        public void Unlock(SimulateDataModel sdmodel)
        {
            /*
            foreach (ValueSegment vs in sdmodel.Values)
            {
                switch (sdmodel.Type)
                {
                    case "BIT":
                        RemoveBitDataPoint(sdmodel.Name, vs.TimeStart, (uint)(vs.Value));
                        break;
                    case "WORD":
                        RemoveWordDataPoint(sdmodel.Name, vs.TimeStart, (UInt16)(vs.Value));
                        break;
                    case "DWORD":
                        RemoveDWordDataPoint(sdmodel.Name, vs.TimeStart, (uint)(vs.Value));
                        break;
                    case "FLOAT":
                        RemoveFloatDataPoint(sdmodel.Name, vs.TimeStart, (float)(vs.Value));
                        break;
                    case "DOUBLE":
                        RemoveDoubleDataPoint(sdmodel.Name, vs.TimeStart, (double)(vs.Value));
                        break;
                }
            }
            */
            switch (sdmodel.Type)
            {
                case "BIT":
                    RemoveViewInput(sdmodel.Name, DP_TYPE_BIT);
                    break;
                case "WORD":
                    RemoveViewInput(sdmodel.Name, DP_TYPE_WORD);
                    break;
                case "DWORD":
                    RemoveViewInput(sdmodel.Name, DP_TYPE_DOUBLEWORD);
                    break;
                case "FLOAT":
                    RemoveViewInput(sdmodel.Name, DP_TYPE_FLOAT);
                    break;
                case "DOUBLE":
                    RemoveViewInput(sdmodel.Name, DP_TYPE_DOUBLE);
                    break;
            }
        }
        #endregion

        #region View Value

        //private List<SimulateDataModel> viewsdmodels = new List<SimulateDataModel>();
        private Dictionary<string, SimulateDataModel> viewsdmodels = new Dictionary<string, SimulateDataModel>();
        
        public void View(SimulateDataModel sdmodel)
        {
            switch (sdmodel.Type)
            {
                case "BIT":
                    AddViewOutput(sdmodel.Name, DP_TYPE_BIT);
                    break;
                case "WORD":
                    AddViewOutput(sdmodel.Name, DP_TYPE_WORD);
                    break;
                case "DWORD":
                    AddViewOutput(sdmodel.Name, DP_TYPE_DOUBLEWORD);
                    break;
                case "FLOAT":
                    AddViewOutput(sdmodel.Name, DP_TYPE_FLOAT);
                    break;
                case "DOUBLE":
                    AddViewOutput(sdmodel.Name, DP_TYPE_DOUBLE);
                    break;
            }
        }
        
        public void Unview(SimulateDataModel sdmodel)
        {
            switch (sdmodel.Type)
            {
                case "BIT":
                    RemoveViewOutput(sdmodel.Name, DP_TYPE_BIT);
                    break;
                case "WORD":
                    RemoveViewOutput(sdmodel.Name, DP_TYPE_WORD);
                    break;
                case "DWORD":
                    RemoveViewOutput(sdmodel.Name, DP_TYPE_DOUBLEWORD);
                    break;
                case "FLOAT":
                    RemoveViewOutput(sdmodel.Name, DP_TYPE_FLOAT);
                    break;
                case "DOUBLE":
                    RemoveViewOutput(sdmodel.Name, DP_TYPE_DOUBLE);
                    break;
            }
        }

        #endregion

        #region Event Handler
        public event SimulateDataModelEventHandler RunDataFinished;
        public event SimulateDataModelEventHandler RunDrawFinished;
        #endregion

    }
}

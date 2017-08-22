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
using System.Collections.ObjectModel;
using SamSoarII.PLCDevice;
using SamSoarII.Threads;

/// <summary>
/// Namespace : SamSoarII.Simulation
/// ClassName : SimulateModel
/// Version   : 1.0
/// Date      : 2017/3/31
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 和外部调用的DLL交互的接口
/// 支持读写外部内存，锁定和监视寄存器等功能
/// </remarks>

namespace SamSoarII.Core.Simulate
{
    public class SimulateDllModel : BaseThreadManager
    {
        #region Import DLL
        /// <summary>
        /// 动态库接口：创建仿真代码的编译环境
        /// </summary>
        [DllImport(@"simug\simu.dll", EntryPoint = "Encode")]
        public static extern void Encode(string ifile, string ofile);

        public static void CreateSource()
        {
            string currentPath = Utility.FileHelper.AppRootPath;
            string libcoPath = String.Format(@"{0:s}\simug\simulib.o", currentPath);
            string libhoPath = String.Format(@"{0:s}\simug\_simulib.o", currentPath);
            string choPath = String.Format(@"{0:s}\simug\simuc.o", currentPath);
            string libcPath = String.Format(@"{0:s}\simug\simulib.c", currentPath);
            string libhPath = String.Format(@"{0:s}\simug\simulib.h", currentPath);
            string chPath = String.Format(@"{0:s}\simug\simuc.h", currentPath);
            Encode(libcoPath, libcPath);
            Encode(libhoPath, libhPath);
            Encode(choPath, chPath);
        }

        /// <summary> LoadDll返回结果：成功</summary>
        public const int LOADDLL_OK = 0x00;
        /// <summary> LoadDll返回结果：没找到dll</summary>
        public const int LOADDLL_CANNOT_FOUND_DLLFILE = 0x01;

        /// <summary>
        /// 动态库接口：dll是否存在？
        /// </summary>
        /// <returns></returns>
        [DllImport(@"simug\simu.dll", EntryPoint = "IsDllAlive")]
        public static extern int IsDllAlive();

        /// <summary>
        /// 动态库接口：读取工程dll
        /// </summary>
        /// <param name="simudllPath">dll文件路径</param>
        /// <returns></returns>
        [DllImport(@"simug\simu.dll", EntryPoint = "LoadDll")]
        public static extern int LoadDll
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string simudllPath
        );

        /// <summary>
        /// 动态库接口：释放dll
        /// </summary>
        [DllImport(@"simug\simu.dll", EntryPoint = "FreeDll")]
        public static extern void FreeDll();

        /// <summary>
        /// 动态库接口：PLC进行单独一次运行
        /// </summary>
        [DllImport(@"simug\simu.dll", EntryPoint = "RunLadder")]
        private static extern int RunLadder();

        /// <summary>
        /// 动态库接口：获取位变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续获取大小</param>
        /// <param name="output">写入结果的空间</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetBit")]
        private static extern int GetBit
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] output
        );

        /// <summary>
        /// 动态库接口：获取字变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续获取大小</param>
        /// <param name="output">写入结果的空间</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetWord")]
        private static extern int GetWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            short[] output
        );
        
        /// <summary>
        /// 动态库接口：获取双字变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续获取大小</param>
        /// <param name="output">写入结果的空间</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetDoubleWord")]
        private static extern int GetDoubleWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            int[] output
        );
        
        /// <summary>
        /// 动态库接口：获取浮点变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续获取大小</param>
        /// <param name="output">写入结果的空间</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetFloat")]
        private static extern int GetFloat
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            float[] output
        );

        /// <summary>
        /// 动态库接口：获取脉冲频率
        /// </summary>
        /// <param name="name">脉冲输出口</param>
        /// <param name="output">写入结果的空间</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetFeq")]
        private static extern int GetFeq
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            [MarshalAs(UnmanagedType.LPArray)]
            int[] output
        );
        
        /// <summary>
        /// 动态库接口：写入位变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续写入大小</param>
        /// <param name="output">要写入的值的空间</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetBit")]
        private static extern int SetBit
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] input
        );
        
        /// <summary>
        /// 动态库接口：写入字变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续写入大小</param>
        /// <param name="output">要写入的值的空间</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetWord")]
        private static extern int SetWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            short[] input
        );

        /// <summary>
        /// 动态库接口：写入双字变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续写入大小</param>
        /// <param name="output">要写入的值的空间</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetDoubleWord")]
        private static extern int SetDoubleWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            int[] input
        );

        /// <summary>
        /// 动态库接口：写入浮点变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续写入大小</param>
        /// <param name="output">要写入的值的空间</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetFloat")]
        private static extern int SetFloat
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            float[] input
        );

        /// <summary>
        /// 动态库接口：设置变量的修改使能
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续写入大小</param>
        /// <param name="value">设置的使能位值</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetEnable")]
        private static extern void SetEnable
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            byte value
        );

        /// <summary>
        /// 在开始仿真之前，需要做的初始化工作
        /// </summary>
        [DllImport(@"simug\simu.dll", EntryPoint = "InitRunLadder")]
        private static extern void InitRunLadder();

        /// <summary>
        /// 每运行一次仿真PLC之前，要运行的函数
        /// </summary>
        [DllImport(@"simug\simu.dll", EntryPoint = "BeforeRunLadder")]
        private static extern void BeforeRunLadder();

        /// <summary>
        /// 每运行一次仿真PLC之后，要运行的函数
        /// </summary>
        [DllImport(@"simug\simu.dll", EntryPoint = "AfterRunLadder")]
        private static extern void AfterRunLadder();

        /// <summary>
        /// 初始化仿真计时时间
        /// </summary>
        /// <param name="time">要初始化的值</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "InitClock")]
        private static extern void InitClock(int time);

        /// <summary>
        /// 获取当前的计时时间
        /// </summary>
        /// <returns>计时时间</returns>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetClock")]
        private static extern int GetClock();

        /// <summary>
        /// 设置时间速率
        /// </summary>
        /// <param name="timerate">时间速率，越小越快，但精度越差</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetClockRate")]
        private static extern void SetClockRate(int timerate);

        /// <summary>
        /// 设置位数
        /// </summary>
        /// <param name="basebit">位数</param>
        /*
        [DllImport(@"simug\simu.dll", EntryPoint = "SetBaseBit")]
        private static extern void SetBaseBit(int basebit);
        */
        /// <summary>
        /// 得到子函数调用的次数
        /// </summary>
        /// <returns></returns>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetCallCount")]
        public static extern int GetCallCount();

        /// <summary>
        /// 获得当前的断点地址
        /// </summary>
        /// <returns></returns>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetBPAddr")]
        public static extern int GetBPAddr();

        /// <summary>
        /// 设置或取消一个普通断点
        /// </summary>
        /// <param name="bpaddr">断点地址</param>
        /// <param name="islock">设置(1)或取消(0)</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetBPAddr")]
        public static extern void SetBPAddr(int bpaddr, byte islock);

        /// <summary>
        /// 设置断点的运行次数
        /// </summary>
        /// <param name="bpaddr">断点地址</param>
        /// <param name="maxcount">最大运行次数</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetBPCount")]
        public static extern void SetBPCount(int bpaddr, int maxcount);

        /// <summary>
        /// 设置一个条件断点
        /// </summary>
        /// <param name="cpaddr">断点地址</param>
        /// <param name="cpmsg">条件信息</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetCPAddr")]
        public static extern void SetCPAddr(int cpaddr, int cpmsg);

        /// <summary>
        /// 获取当前断点状态
        /// </summary>
        /// <returns></returns>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetBPPause")]
        public static extern byte GetBPPause();

        /// <summary>
        /// 设置当前断点状态（可以继续运行）
        /// </summary>
        /// <param name="bppause"></param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetBPPause")]
        public static extern void SetBPPause(byte bppause);
        
        /// <summary>
        /// 设置断点使能
        /// </summary>
        /// <param name="bpenable"></param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetBPEnable")]
        public static extern void SetBPEnable(byte bpenable);
        
        /// <summary>
        /// 获得当前栈地址
        /// </summary>
        /// <returns></returns>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetRBP")]
        unsafe public static extern void* GetRBP();

        /// <summary>
        /// 获得调用信息栈
        /// </summary>
        /// <param name="data">保存的空间</param>
        /// <returns>长度</returns>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetBackTrace")]
        public static extern int GetBackTrace
        (
            [MarshalAs(UnmanagedType.LPArray)]
            int[] data
        );

        /// <summary>
        /// 单步运行（不进入子程序）
        /// </summary>
        [DllImport(@"simug\simu.dll", EntryPoint = "MoveStep")]
        public static extern void MoveStep();

        /// <summary>
        /// 单步运行（进入子程序）
        /// </summary>
        [DllImport(@"simug\simu.dll", EntryPoint = "CallStep")]
        public static extern void CallStep();

        /// <summary>
        /// 跳转到断点
        /// </summary>
        /// <param name="bpaddr">断点地址</param>
        [DllImport(@"simug\simu.dll", EntryPoint = "JumpTo")]
        public static extern void JumpTo(int bpaddr);
        
        /// <summary>
        /// 从子程序中跳出
        /// </summary>
        [DllImport(@"simug\simu.dll", EntryPoint = "JumpOut")]
        public static extern void JumpOut();

        /// <summary>
        /// 设置判断中断软元件的dll程序
        /// </summary>
        /// <param name="dllpath"></param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetItrpDll")]
        public static extern int SetItrpDll(string dllpath);
        
        /// <summary>
        /// 释放判断中断软元件的dll程序
        /// </summary>
        [DllImport(@"simug\simu.dll", EntryPoint = "FreeItrpDll")]
        public static extern int FreeItrpDll();
        
        /// <summary>
        /// 强制释放判断中断软元件的dll程序
        /// </summary>
        [DllImport(@"simug\simu.dll", EntryPoint = "ForceFreeItrpDll")]
        public static extern void ForceFreeItrpDll();

        /// <summary>
        /// 获得当前触发的中断软元件ID
        /// </summary>
        /// <returns></returns>
        [DllImport(@"simug\simu.dll", EntryPoint = "GetItrpID")]
        public static extern int GetItrpID();


        #endregion

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public SimulateDllModel(SimulateManager _parent) : base(false, true)
        {
            parent = _parent;
        }

        private SimulateManager parent;
        public SimulateManager Parent { get { return this.parent; } }
        public SimulateViewer Viewer { get { return parent.Viewer; } }

        #region Simulate Process

        [HandleProcessCorruptedStateExceptions]
        protected override void Before()
        {
            base.Before();
            //PLCDevice.Device device = PLCDeviceManager.GetPLCDeviceManager().SelectDevice;
            //SetBaseBit(device.BitNumber);
            SetBPEnable(1);
            SetClockRate(2);
            try
            {
                InitRunLadder();
            }
            catch (Exception exce)
            {
                Pause();
                SimulateException(exce, new RoutedEventArgs());
            }
        }

        [HandleProcessCorruptedStateExceptions]
        protected override void Handle()
        {
            BeforeRunLadder();
            try
            {
                RunLadder();
            }
            catch (Exception exce)
            {
                Pause();
                SimulateException(exce, new RoutedEventArgs());
            }
            AfterRunLadder();
        }

        #endregion

        #region Get & Set Value

        /// <summary>
        /// 位变量缓存区
        /// </summary>
        private byte[] bbuf = new byte[1];
        /// <summary>
        /// 获取一个位变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <returns>获取的值</returns>
        public byte GetValue_Bit(string var)
        {
            //var = var.ToUpper();
            GetBit(var, 1, bbuf);
            return bbuf[0];
        }
        /// <summary>
        /// 获取一段位变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public byte[] GetValue_Bit(string var, int size)
        {
            //var = var.ToUpper();
            byte[] ret = new byte[size];
            GetBit(var, size, ret);
            return ret;
        }

        /// <summary>
        /// 字变量缓存区
        /// </summary>
        private short[] wbuf = new short[1];
        /// <summary>
        /// 获取一个字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <returns>获取的值</returns>
        public short GetValue_Word(string var)
        {
            //var = var.ToUpper();
            GetWord(var, 1, wbuf);
            return wbuf[0];
        }
        /// <summary>
        /// 获取一段字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public short[] GetValue_Word(string var, int size)
        {
            //var = var.ToUpper();
            short[] ret = new short[size];
            GetWord(var, size, ret);
            return ret;
        }


        /// <summary>
        /// 双字变量缓存区
        /// </summary>
        private int[] dbuf = new int[1];
        /// <summary>
        /// 获取一个双字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <returns>获取的值</returns>
        public int GetValue_DWord(string var)
        {
            //var = var.ToUpper();
            GetDoubleWord(var, 1, dbuf);
            return dbuf[0];
        }
        /// <summary>
        /// 获取一段双字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public int[] GetValue_DWord(string var, int size)
        {
            //var = var.ToUpper();
            int[] ret = new int[size];
            GetDoubleWord(var, size, ret);
            return ret;
        }
        
        /// <summary>
        /// 浮点变量缓存区
        /// </summary>
        private float[] fbuf = new float[1];
        /// <summary>
        /// 获取一个浮点变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <returns>获取的值</returns>
        public float GetValue_Float(string var)
        {
            //var = var.ToUpper();
            GetFloat(var, 1, fbuf);
            return fbuf[0];
        }
        /// <summary>
        /// 获取一段浮点变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public float[] GetValue_Float(string var, int size)
        {
            //var = var.ToUpper();
            float[] ret = new float[size];
            GetFloat(var, size, ret);
            return ret;
        }

        /// <summary>
        /// 获取脉冲信号的频率
        /// </summary>
        /// <param name="var">脉冲输出口</param>
        /// <returns></returns>
        public int GetFeq(string var)
        {
            //var = var.ToUpper();
            GetFeq(var, dbuf);
            return dbuf[0];
        }


        /// <summary>
        /// 设置一个位变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Bit(string var, byte input)
        {
            //var = var.ToUpper();
            bbuf[0] = input;
            SetBit(var, 1, bbuf);
        }
        /// <summary>
        /// 设置一段位变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Bit(string var, int size, byte[] input)
        {
            //var = var.ToUpper();
            SetBit(var, size, input);
        }
        
        /// <summary>
        /// 设置一个字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Word(string var, short input)
        {
            //var = var.ToUpper();
            wbuf[0] = input;
            SetWord(var, 1, wbuf);
        }
        /// <summary>
        /// 设置一段字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Word(string var, int size, short[] input)
        {
            var = var.ToUpper();
            SetWord(var, size, input);
        }

        /// <summary>
        /// 设置一个双字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="input">输入的值</param>
        public void SetValue_DWord(string var, int input)
        {
            //var = var.ToUpper();
            dbuf[0] = input;
            SetDoubleWord(var, 1, dbuf);
        }
        /// <summary>
        /// 设置一段双字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_DWord(string var, int size, int[] input)
        {
            //var = var.ToUpper();
            SetDoubleWord(var, size, input);
        }

        /// <summary>
        /// 设置一个双字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Float(string var, float input)
        {
            //var = var.ToUpper();
            fbuf[0] = input;
            SetFloat(var, 1, fbuf);
        }
        /// <summary>
        /// 设置一段浮点变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Float(string var, int size, float[] input)
        {
            //var = var.ToUpper();
            SetFloat(var, size, input);
        }
   
        #endregion

        #region Lock Value

        /// <summary>
        /// 锁定一段变量
        /// </summary>
        /// <param name="var">变量</param>
        /// <param name="size">长度</param>
        public void Lock(string var, int size = 1)
        {
            SetEnable(var, size, 1);
        }
        
        /// <summary>
        /// 解锁一段变量
        /// </summary>
        /// <param name="var">变量</param>
        /// <param name="size">长度</param>
        public void Unlock(string var, int size = 1)
        {
            SetEnable(var, size, 0);
        }
        
        #endregion
        
        #region Event Handler
        
        /// <summary>
        /// 当仿真发生异常时，触发这个代理
        /// </summary>
        public event RoutedEventHandler SimulateException = delegate { };
        
        #endregion

    }
}

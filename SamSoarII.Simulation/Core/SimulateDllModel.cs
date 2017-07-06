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
using SamSoarII.Simulation.Core.DataModel;
using SamSoarII.Simulation.UI.Chart;
using System.Collections.ObjectModel;
using SamSoarII.PLCDevice;

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

namespace SamSoarII.Simulation.Core
{
    public class SimulateDllModel
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
        /// <summary> LoadDll返回结果：没找到RunLadder的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_RUNLADDER = 0x02;
        /// <summary> LoadDll返回结果：没找到GetBit的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_GETBIT = 0x03;
        /// <summary> LoadDll返回结果：没找到GetWord的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_GETWORD = 0x04;
        /// <summary> LoadDll返回结果：没找到GetDWord的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_GETDWORD = 0x05;
        /// <summary> LoadDll返回结果：没找到GetFloat的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_GETFLOAT = 0x06;
        /// <summary> LoadDll返回结果：没找到GetFeq的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_GETFEQ = 0x07;
        /// <summary> LoadDll返回结果：没找到SetBit的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETBIT = 0x08;
        /// <summary> LoadDll返回结果：没找到SetWord的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETWORD = 0x09;
        /// <summary> LoadDll返回结果：没找到SetDWord的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETDWORD = 0x0A;
        /// <summary> LoadDll返回结果：没找到SetFloat的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETFLOAT = 0x0B;
        /// <summary> LoadDll返回结果：没找到SetFeq的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETFEQ = 0x0C;
        /// <summary> LoadDll返回结果：没找到SetEnable的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETENABLE = 0x0D;
        /// <summary> LoadDll返回结果：没找到BeforeRunLadder的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_BEFORERUNLADDER = 0x0E;
        /// <summary> LoadDll返回结果：没找到AfterRunLadder的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_AFTERRUNLADDER = 0x0F;
        /// <summary> LoadDll返回结果：没找到InitRunLadder的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_INITRUNLADDER = 0x10;
        /// <summary> LoadDll返回结果：没找到InitClock的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_INITCLOCK = 0x11;
        /// <summary> LoadDll返回结果：没找到GetClock的入口</summary>
        public const int LOADDLL_CANNNT_FOUND_GETCLOCK = 0x12;
        /// <summary> LoadDll返回结果：没找到SetClockRate的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETCLOCKRATE = 0x13;

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
        public static extern void FreeDll
        (
        );

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
            Int32[] output
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
            Int32[] output
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
            Int64[] output
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
            double[] output
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
            Int64[] output
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
            Int32[] input
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
            Int32[] input
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
            Int64[] input
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
            double[] input
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
            int value
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
        [DllImport(@"simug\simu.dll", EntryPoint = "SetBaseBit")]
        private static extern void SetBaseBit(int basebit);

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
        public static extern void SetBPAddr(int bpaddr, int islock);

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
        public static extern int GetBPPause();

        /// <summary>
        /// 设置当前断点状态（可以继续运行）
        /// </summary>
        /// <param name="bppause"></param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetBPPause")]
        public static extern void SetBPPause(int bppause);
        
        /// <summary>
        /// 设置断点使能
        /// </summary>
        /// <param name="bpenable"></param>
        [DllImport(@"simug\simu.dll", EntryPoint = "SetBPEnable")]
        public static extern void SetBPEnable(int bpenable);
        
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

        #endregion

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public SimulateDllModel()
        {
            // 初始化线程
            simulateActive = false;
            simulateThread = null;
            SimulateAbort += OnSimulateThreadClosed;
        }

        #region Simulate Process

        /// <summary> 仿真线程是否存活 </summary>
        private bool simulateActive;
        /// <summary> 仿真线程是否正在运行 </summary>
        private bool simulateRunning;
        /// <summary> 仿真线程 </summary>
        private Thread simulateThread;

        /// <summary> 仿真状态：停止 </summary>
        public const int SIMULATE_STOP = 0x00;
        /// <summary> 仿真状态：运行 </summary>
        public const int SIMULATE_RUNNING = 0x01;
        /// <summary> 仿真状态：暂停 </summary>
        public const int SIMULATE_PAUSE = 0x02;
        
        /// <summary> 仿真模式：正常 </summary>
        public const int SIMUMODE_NORMAL = 0x00;
        /// <summary> 仿真模式：图表绘制 </summary>
        public const int SIMUMODE_CHART = 0x01;

        /// <summary> 仿真状态 </summary>
        public int SimulateStatus
        {
            get
            {
                // 如果当前线程为空或者不存活
                if (simulateThread == null || !simulateActive)
                {
                    // 线程取消存活状态
                    simulateActive = false;
                    // 线程设为空
                    simulateThread = null;
                    // 返回停止状态
                    return SIMULATE_STOP;
                }
                // 如果当前线程不为空，并且存活
                else
                {
                    // 正在运行状态
                    if (simulateRunning)
                    {
                        return SIMULATE_RUNNING;
                    }   
                    // 正在暂停状态
                    else
                    {
                        return SIMULATE_PAUSE;
                    }
                }
            }
            set
            {
                switch (value)
                {
                    case SIMULATE_STOP: Abort(); break;
                    case SIMULATE_PAUSE: Pause(); break;
                    case SIMULATE_RUNNING: Start(); break;
                }
            }
        }

        /// <summary> 仿真模式 </summary>
        private int simumode = SIMUMODE_NORMAL;
        /// <summary> 仿真模式 </summary>
        public int SimuMode
        {
            get
            {
                return this.simumode;
            }
            set
            {
                Abort();
                this.simumode = value;
            }
        }

        public int TimeStart { get; set; }

        public int TimeEnd { get; set; }
        
        /// <summary>
        /// 仿真线程要运行的函数
        /// </summary>
        [HandleProcessCorruptedStateExceptions]
        private void _SimulateThread()
        {
            PLCDevice.Device device = PLCDeviceManager.GetPLCDeviceManager().SelectDevice;
            SetBaseBit(device.BitNumber);
            SetBPEnable(1);
            SetClockRate(1);
            // 初始化
            try
            {
                InitRunLadder();
            }
            // 触发异常时暂停并发送事件等待处理
            catch (Exception exce)
            {
                Pause();
                SimulateException(exce, new RoutedEventArgs());
                //SimulateAbort(this, new RoutedEventArgs());
            }
            // 存活状态下运行循环
            while (simulateActive)
            {
                // 暂停状态下运行这个循环
                while (!simulateRunning)
                {
                    // 未存活时跳出这个循环
                    if (!simulateActive) break;
                    // 等待
                    Thread.Sleep(10);
                }
                // 未存活时跳出这个循环
                if (!simulateActive) break;
                // 运行一次仿真PLC
                BeforeRunLadder();
                try
                {
                    RunLadder();
                }
                // 触发异常时暂停并发送事件等待处理
                catch (Exception exce)
                {
                    Pause();
                    SimulateException(exce, new RoutedEventArgs());
                    //SimulateAbort(this, new RoutedEventArgs());
                }
                AfterRunLadder();
            }
            // 发送线程终止的事件
            SimulateAbort(this, new RoutedEventArgs());
        }
        
        /// <summary>
        /// 仿真线程要运行的函数（图表模式）
        /// </summary>
        public void _SimulateThread_Chart()
        {
            PLCDevice.Device device = PLCDeviceManager.GetPLCDeviceManager().SelectDevice;
            SetBaseBit(device.BitNumber);
            SetClockRate(1);
            InitRunLadder();
            InitClock(TimeStart);
            // 当前时间
            int time = GetClock();
            int count = 0;
            double progress = 0.0;
            while (simulateActive)
            {
                count++;
                // 设置锁定的视点
                foreach (SimulateDataModel sdmodel in locksdmodels)
                {
                    sdmodel.Set(this, time);
                }

                while (!simulateRunning)
                {
                    if (!simulateActive) break;
                    Thread.Sleep(10);
                }
                if (!simulateActive) break;
                BeforeRunLadder();
                try
                {
                    RunLadder();
                }
                // 触发异常时发送事件，并暂停运行等待处理
                catch (Exception exce)
                {
                    SimulateException(exce, new RoutedEventArgs());
                    simulateRunning = false;
                }
                AfterRunLadder();
                // 检查是否超过终止时间
                time = GetClock();
                if (time >= TimeEnd)
                    simulateActive = false;
                if (!simulateActive) break;

                double _progress = (double)(time - TimeStart) / (TimeEnd - TimeStart) * 100;
                if (_progress - progress > 1.0)
                {
                    progress = _progress;
                    SimulateProgress(progress, new RoutedEventArgs());
                }
                // 读取监视的视点
                foreach (SimulateDataModel sdmodel in viewsdmodels)
                {
                    sdmodel.Update(this, time);
                }
            }
            // 发送线程终止的事件
            SimulateAbort(this, new RoutedEventArgs());
        }
        
        /// <summary>
        /// 开始仿真
        /// </summary>
        public void Start()
        {
            // 存活和运行
            simulateActive = true;
            simulateRunning = true;
            // 设置新线程
            if (simulateThread == null)
            {
                switch (SimuMode)
                {
                    case SIMUMODE_NORMAL:
                        simulateThread = new Thread(_SimulateThread);
                        break;
                    case SIMUMODE_CHART:
                        simulateThread = new Thread(_SimulateThread_Chart);
                        break;
                }
                simulateThread.Start();
            }
            SimulateStart(this, new RoutedEventArgs());
        }

        /// <summary>
        /// 暂停仿真
        /// </summary>
        public void Pause()
        {
            // 设置运行状态为false
            simulateRunning = false;
            SimulatePause(this, new RoutedEventArgs());
        }

        /// <summary>
        /// 停止仿真
        /// </summary>
        public void Abort(bool force = false)
        {
            // 已经关闭？
            if (!simulateActive || simulateThread == null)
                return;
            // 关闭线程并设为null
            simulateActive = false;
            simulateRunning = false;
            // 强制关闭运行
            if (force)
            {
                simulateThread.Abort();
                SimulateAbort(this, new RoutedEventArgs());
            }
            simulateThread = null;
        }

        /// <summary>
        /// 进行一次数据运行，获得时间段内所有输入视点的值的变化情况，并绘制波形
        /// </summary>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        public void RunData(double starttime, double endtime)
        {
        }

        /// <summary>
        /// 进行一次图表运行，获得时间段内所有输入视点的值的变化情况，并绘制图表
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        public void RunDraw(double starttime, double endtime)
        {
        }

        #endregion
        
        #region Get & Set Value

        /// <summary>
        /// 获取一段位变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public Int32[] GetValue_Bit(string var, int size)
        {
            var = var.ToUpper();
            Int32[] ret = new Int32[size];
            GetBit(var, size, ret);
            return ret;
        }
        
        /// <summary>
        /// 获取一段字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public Int32[] GetValue_Word(string var, int size)
        {
            var = var.ToUpper();
            Int32[] ret = new Int32[size];
            GetWord(var, size, ret);
            return ret;
        }
        
        /// <summary>
        /// 获取一段双字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public Int64[] GetValue_DWord(string var, int size)
        {
            var = var.ToUpper();
            Int64[] ret = new Int64[size];
            GetDoubleWord(var, size, ret);
            return ret;
        }
        
        /// <summary>
        /// 获取一段浮点变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public double[] GetValue_Float(string var, int size)
        {
            var = var.ToUpper();
            double[] ret = new double[size];
            GetFloat(var, size, ret);
            return ret;
        }

        /// <summary>
        /// 获取脉冲信号的频率
        /// </summary>
        /// <param name="var">脉冲输出口</param>
        /// <returns></returns>
        public Int64 GetFeq(string var)
        {
            var = var.ToUpper();
            Int64[] ret = new Int64[1];
            GetFeq(var, ret);
            return ret[0];
        }

        /// <summary>
        /// 设置一段位变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Bit(string var, int size, Int32[] input)
        {
            var = var.ToUpper();
            SetBit(var, size, input);
        }

        /// <summary>
        /// 设置一段字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Word(string var, int size, Int32[] input)
        {
            var = var.ToUpper();
            SetWord(var, size, input);
        }
        
        /// <summary>
        /// 设置一段双字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_DWord(string var, int size, Int64[] input)
        {
            var = var.ToUpper();
            SetDoubleWord(var, size, input);
        }

        /// <summary>
        /// 设置一段浮点变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Float(string var, int size, double[] input)
        {
            var = var.ToUpper();
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

        /// <summary>
        /// 锁定视点的集合
        /// </summary>
        private ObservableCollection<SimulateDataModel> locksdmodels 
            = new ObservableCollection<SimulateDataModel>();
        
        /// <summary>
        /// 锁定一个视点
        /// </summary>
        /// <param name="sdmodel">视点模型</param>
        public void Lock(SimulateDataModel sdmodel)
        {
            if (!locksdmodels.Contains(sdmodel))
            {
                locksdmodels.Add(sdmodel);
                SetEnable(sdmodel.Name, 1, 1);
            }
        }
        
        /// <summary>
        /// 解锁一个视点
        /// </summary>
        /// <param name="sdmodel">视点模型</param>
        public void Unlock(SimulateDataModel sdmodel)
        {
            if (locksdmodels.Contains(sdmodel))
            {
                locksdmodels.Remove(sdmodel);
                SetEnable(sdmodel.Name, 1, 0);
            }
        }
        #endregion

        #region View Value
        
        /// <summary>
        /// 监视视点的集合
        /// </summary>
        private ObservableCollection<SimulateDataModel> viewsdmodels
            = new ObservableCollection<SimulateDataModel>();


        /// <summary>
        /// 监视一个视点
        /// </summary>
        /// <param name="sdmodel">视点模型</param>
        public void View(SimulateDataModel sdmodel)
        {
            if (!viewsdmodels.Contains(sdmodel))
            {
                viewsdmodels.Add(sdmodel);
            }
        }
        
        /// <summary>
        /// 解除监视一个视点
        /// </summary>
        /// <param name="sdmodel"></param>
        public void Unview(SimulateDataModel sdmodel)
        {
            if (viewsdmodels.Contains(sdmodel))
            {
                viewsdmodels.Remove(sdmodel);
            }
        }

        #endregion
        
        #region Event Handler

        #region Thread

        /// <summary>
        /// 当仿真线程开始时，触发这个代理
        /// </summary>
        public event RoutedEventHandler SimulateStart = delegate { };

        /// <summary>
        /// 当仿真线程暂停时，触发这个代理
        /// </summary>
        public event RoutedEventHandler SimulatePause = delegate { };

        /// <summary>
        /// 当仿真线程停止时，触发这个代理
        /// </summary>
        public event RoutedEventHandler SimulateAbort = delegate { };

        /// <summary>
        /// 当仿真发生异常时，触发这个代理
        /// </summary>
        public event RoutedEventHandler SimulateException = delegate { };

        /// <summary>
        /// 每次仿真进度更新时，触发这个代理
        /// </summary>
        public event RoutedEventHandler SimulateProgress = delegate { }; 
        
        /// <summary>
        /// 当线程终止时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnSimulateThreadClosed(object sender, RoutedEventArgs e)
        {
            // 将线程设为null
            simulateThread = null;
        }

        #endregion



        #endregion

    }
}

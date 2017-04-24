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
        /// 动态库接口：根据工程源码创建仿真dll
        /// </summary>
        /// <param name="simucPath">simuc.c的文件路径</param>
        /// <param name="simufPath">simuf.c的文件路径</param>
        /// <param name="simudllPath">simuc.dll的输出路径</param>
        /// <param name="simuaPath">simua.a的输出路径</param>
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

        /// <summary> LoadDll返回结果：成功</summary>
        public const int LOADDLL_OK = 0x00;
        /// <summary> LoadDll返回结果：没找到dll</summary>
        public const int LOADDLL_CANNOT_FOUND_DLLFILE = 0x010000;
        /// <summary> LoadDll返回结果：没找到RunLadder的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_RUNLADDER = 0x020000;
        /// <summary> LoadDll返回结果：没找到GetBit的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_GETBIT = 0x030000;
        /// <summary> LoadDll返回结果：没找到GetWord的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_GETWORD = 0x040000;
        /// <summary> LoadDll返回结果：没找到GetDWord的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_GETDWORD = 0x050000;
        /// <summary> LoadDll返回结果：没找到GetFloat的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_GETFLOAT = 0x060000;
        /// <summary> LoadDll返回结果：没找到GetDouble的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_GETDOUBLE = 0x070000;
        /// <summary> LoadDll返回结果：没找到SetBit的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETBIT = 0x080000;
        /// <summary> LoadDll返回结果：没找到SetWord的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETWORD = 0x090000;
        /// <summary> LoadDll返回结果：没找到SetDWord的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETDWORD = 0x0A0000;
        /// <summary> LoadDll返回结果：没找到SetFloat的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETFLOAT = 0x0B0000;
        /// <summary> LoadDll返回结果：没找到SetDouble的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_SETDOUBLE = 0x0C0000;
        /// <summary> LoadDll返回结果：没找到SetEnable的入口</summary>
        public const int LOADDLL_CANNOT_FOUNF_SETENABLE = 0x0D0000;
        /// <summary> LoadDll返回结果：没找到InitDataPoint的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_INITDATAPOINT = 0x0E0000;
        /// <summary> LoadDll返回结果：没找到AddBitDataPoint的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_ADDBITDATAPOINT = 0x0F0000;
        /// <summary> LoadDll返回结果：没找到AddWordDataPoint的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_ADDWORDDATAPOINT = 0x100000;
        /// <summary> LoadDll返回结果：没找到AddDWordDataPoint的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_ADDDWORDDATAPOINT = 0x110000;
        /// <summary> LoadDll返回结果：没找到AddFloatDataPoint的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_ADDFLOATDATAPOINT = 0x120000;
        /// <summary> LoadDll返回结果：没找到AddDoubleDataPoint的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_ADDDOUBLEATAPOINT = 0x130000;
        /// <summary> LoadDll返回结果：没找到RemoveBitDataPoint的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_REMOVEBITDATAPOINT = 0x140000;
        /// <summary> LoadDll返回结果：没找到RemoveWordDataPoint的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_REMOVEWORDDATAPOINT = 0x150000;
        /// <summary> LoadDll返回结果：没找到RemoveDWordDataPoint的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_REMOVEDWORDDATAPOINT = 0x160000;
        /// <summary> LoadDll返回结果：没找到RemoveFloatDataPoint的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_REMOVEFLOATDATAPOINT = 0x170000;
        /// <summary> LoadDll返回结果：没找到RemoveDoubleDataPoint的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_REMOVEDOUBLEATAPOINT = 0x180000;
        /// <summary> LoadDll返回结果：没找到AddViewInput的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_ADDVIEWINPUT = 0x190000;
        /// <summary> LoadDll返回结果：没找到AddViewOutput的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_ADDVIEWOUTPUT = 0x1A0000;
        /// <summary> LoadDll返回结果：没找到RemoveViewInput的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_REMOVEVIEWINPUT = 0x1B0000;
        /// <summary> LoadDll返回结果：没找到RemoveViewOutput的入口</summary>
        public const int LOADDLL_CANNOT_FOUNd_REMOVEVIEWOUTPUT = 0x1C0000;
        /// <summary> LoadDll返回结果：没找到BeforeRunLadder的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_BEFORERUNLADDER = 0x1D0000;
        /// <summary> LoadDll返回结果：没找到AfterRunLadder的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_AFTERRUNLADDER = 0x1E0000;
        /// <summary> LoadDll返回结果：没找到RunData的入口</summary>
        public const int LOADDLL_CANNOT_FOUND_RUNDATA = 0x1F0000;

        /// <summary>
        /// 动态库接口：读取工程dll
        /// </summary>
        /// <param name="simudllPath">dll文件路径</param>
        /// <returns></returns>
        [DllImport("simu.dll", EntryPoint = "LoadDll")]
        public static extern int LoadDll
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string simudllPath
        );

        /// <summary>
        /// 动态库接口：释放dll
        /// </summary>
        [DllImport("simu.dll", EntryPoint = "FreeDll")]
        public static extern void FreeDll
        (
        );

        /// <summary>
        /// 动态库接口：PLC进行单独一次运行
        /// </summary>
        [DllImport("simu.dll", EntryPoint = "RunLadder")]
        private static extern void RunLadder();

        /// <summary>
        /// 动态库接口：获取位变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续获取大小</param>
        /// <param name="output">写入结果的空间</param>
        [DllImport("simu.dll", EntryPoint = "GetBit")]
        private static extern void GetBit
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt32[] output
        );

        /// <summary>
        /// 动态库接口：获取字变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续获取大小</param>
        /// <param name="output">写入结果的空间</param>
        [DllImport("simu.dll", EntryPoint = "GetWord")]
        private static extern void GetWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt32[] output
        );
        
        /// <summary>
        /// 动态库接口：获取双字变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续获取大小</param>
        /// <param name="output">写入结果的空间</param>
        [DllImport("simu.dll", EntryPoint = "GetDoubleWord")]
        private static extern void GetDoubleWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt64[] output
        );
        
        /// <summary>
        /// 动态库接口：获取浮点变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续获取大小</param>
        /// <param name="output">写入结果的空间</param>
        [DllImport("simu.dll", EntryPoint = "GetFloat")]
        private static extern void GetFloat
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
        [DllImport("simu.dll", EntryPoint = "GetFeq")]
        private static extern void GetFeq
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
        [DllImport("simu.dll", EntryPoint = "SetBit")]
        private static extern void SetBit
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt32[] input
        );
        
        /// <summary>
        /// 动态库接口：写入字变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续写入大小</param>
        /// <param name="output">要写入的值的空间</param>
        [DllImport("simu.dll", EntryPoint = "SetWord")]
        private static extern void SetWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt32[] input
        );

        /// <summary>
        /// 动态库接口：写入双字变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续写入大小</param>
        /// <param name="output">要写入的值的空间</param>
        [DllImport("simu.dll", EntryPoint = "SetDoubleWord")]
        private static extern void SetDoubleWord
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            [MarshalAs(UnmanagedType.LPArray)]
            UInt64[] input
        );

        /// <summary>
        /// 动态库接口：写入浮点变量的值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <param name="size">连续写入大小</param>
        /// <param name="output">要写入的值的空间</param>
        [DllImport("simu.dll", EntryPoint = "SetFloat")]
        private static extern void SetFloat
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
        [DllImport("simu.dll", EntryPoint = "SetEnable")]
        private static extern void SetEnable
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int size,
            int value
        );

        /// <summary>
        /// 初始化数据点系统
        /// </summary>
        [DllImport("simu.dll", EntryPoint = "InitDataPoint")]
        public static extern void InitDataPoint
        (
        );

        /// <summary>
        /// 添加一个类型为位的数据点
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="time">时间</param>
        /// <param name="value">值</param>
        [DllImport("simu.dll", EntryPoint = "AddBitDataPoint")]
        private static extern void AddBitDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt32 value
        );
        
        /// <summary>
        /// 添加一个类型为字的数据点
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="time">时间</param>
        /// <param name="value">值</param>
        [DllImport("simu.dll", EntryPoint = "AddWordDataPoint")]
        private static extern void AddWordDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt32 value
        );
        
        /// <summary>
        /// 添加一个类型为双字的数据点
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="time">时间</param>
        /// <param name="value">值</param>
        [DllImport("simu.dll", EntryPoint = "AddDWordDataPoint")]
        private static extern void AddDWordDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt64 value
        );
        
        /// <summary>
        /// 添加一个类型为浮点的数据点
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="time">时间</param>
        /// <param name="value">值</param>
        [DllImport("simu.dll", EntryPoint = "AddFloatDataPoint")]
        private static extern void AddFloatDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            double value
        );
        
        /// <summary>
        /// 删除一个类型为位的数据点
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="time">时间</param>
        /// <param name="value">值</param>
        [DllImport("simu.dll", EntryPoint = "RemoveBitDataPoint")]
        private static extern void RemoveBitDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt32 value
        );

        /// <summary>
        /// 删除一个类型为字的数据点
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="time">时间</param>
        /// <param name="value">值</param>
        [DllImport("simu.dll", EntryPoint = "RemoveWordDataPoint")]
        private static extern void RemoveWordDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt32 value
        );

        /// <summary>
        /// 删除一个类型为双字的数据点
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="time">时间</param>
        /// <param name="value">值</param>
        [DllImport("simu.dll", EntryPoint = "RemoveDWordDataPoint")]
        private static extern void RemoveDWordDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            UInt64 value
        );

        /// <summary>
        /// 删除一个类型为浮点的数据点
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="time">时间</param>
        /// <param name="value">值</param>
        [DllImport("simu.dll", EntryPoint = "RemoveFloatDataPoint")]
        private static extern void RemoveFloatDataPoint
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int time,
            double value
        );
        
        /// <summary> 视点类型：位 </summary>
        private const int DP_TYPE_BIT = 0x01;
        /// <summary> 视点类型：字 </summary>
        private const int DP_TYPE_WORD = 0x02;
        /// <summary> 视点类型：双字 </summary>
        private const int DP_TYPE_DOUBLEWORD = 0x03;
        /// <summary> 视点类型：浮点 </summary>
        private const int DP_TYPE_FLOAT = 0x04;

        /// <summary>
        /// 添加新的输入视点（锁定）
        /// </summary>
        /// <param name="name">视点名称</param>
        /// <param name="type">视点类型</param>
        [DllImport("simu.dll", EntryPoint = "AddViewInput")]
        private static extern void AddViewInput
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int type
        );
        
        /// <summary>
        /// 添加新的输出视点（监视）
        /// </summary>
        /// <param name="name">视点名称</param>
        /// <param name="type">视点类型</param>
        [DllImport("simu.dll", EntryPoint = "AddViewOutput")]
        private static extern void AddViewOutput
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int type
        );
        
        /// <summary>
        /// 删除输入视点（解锁）
        /// </summary>
        /// <param name="name">视点名称</param>
        /// <param name="type">视点类型</param>
        [DllImport("simu.dll", EntryPoint = "RemoveViewInput")]
        private static extern void RemoveViewInput
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int type
        );

        /// <summary>
        /// 删除输出视点（解除监视）
        /// </summary>
        /// <param name="name">视点名称</param>
        /// <param name="type">视点类型</param>
        [DllImport("simu.dll", EntryPoint = "RemoveViewOutput")]
        private static extern void RemoveViewOutput
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int type
        );

        /// <summary>
        /// 每运行一次仿真PLC之前，要运行的函数
        /// </summary>
        [DllImport("simu.dll", EntryPoint = "BeforeRunLadder")]
        private static extern void BeforeRunLadder();

        /// <summary>
        /// 每运行一次仿真PLC之后，要运行的函数
        /// </summary>
        [DllImport("simu.dll", EntryPoint = "AfterRunLadder")]
        private static extern void AfterRunLadder();

        /// <summary>
        /// 进行一次数据运行，获得输出视点在时间段内的数据情况
        /// </summary>
        /// <param name="outputFile">输出的报告文件</param>
        /// <param name="starttime">时间段起点</param>
        /// <param name="endtime">时间段终点</param>
        [DllImport("simu.dll", EntryPoint = "RunData")]
        private static extern void RunData
        (
            [MarshalAs(UnmanagedType.LPStr)]
            string outputFile,
            int starttime,
            int endtime
        );
        #endregion
        
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public SimulateDllModel()
        {
            // 初始化线程
            simulateActive = false;
            simulateThread = null;
        }

        #region Simulate Process

        /// <summary>
        /// 仿真线程是否存活
        /// </summary>
        private bool simulateActive;
        /// <summary>
        /// 仿真线程是否正在运行
        /// </summary>
        private bool simulateRunning;
        /// <summary>
        /// 仿真线程
        /// </summary>
        private Thread simulateThread;

        /// <summary> 仿真状态：停止 </summary>
        public const int SIMULATE_STOP = 0x00;
        /// <summary> 仿真状态：运行 </summary>
        public const int SIMULATE_RUNNING = 0x01;
        /// <summary> 仿真状态：暂停 </summary>
        public const int SIMULATE_PAUSE = 0x02;

        /// <summary>
        /// 仿真状态
        /// </summary>
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
        
        /// <summary>
        /// 仿真线程要运行的函数
        /// </summary>
        private void _SimulateThread()
        {
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
                RunLadder();
                AfterRunLadder();
            }
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
                simulateThread = new Thread(_SimulateThread);
                simulateThread.Start();
            } 
        }

        /// <summary>
        /// 暂停仿真
        /// </summary>
        public void Pause()
        {
            // 设置运行状态为false
            simulateRunning = false;
        }
        
        /// <summary>
        /// 停止仿真
        /// </summary>
        public void Abort()
        {
            // 取消存活和运行状态
            simulateActive = false;
            simulateRunning = false;
            // 关闭线程并设为null
            if (simulateThread != null)
            {
                simulateThread.Abort();
                simulateThread = null;
            }
        }

        /// <summary>
        /// 进行一次数据运行，获得时间段内所有输入视点的值的变化情况，并绘制波形
        /// </summary>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        public void RunData(double starttime, double endtime)
        {
            // 运行并将记录返回到simulog.log中
            RunData("simulog.log", (int)(starttime), (int)(endtime));
            // 发送数据运行完毕的事件
            SimulateDataModelEventArgs e = new SimulateDataModelEventArgs();
            e.TimeStart = starttime;
            e.TimeEnd = endtime;
            RunDataFinished(this, e);
        }

        /// <summary>
        /// 进行一次图表运行，获得时间段内所有输入视点的值的变化情况，并绘制图表
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        public void RunDraw(double starttime, double endtime)
        {
            // 运行并将记录返回到simulog.log中
            RunData("simulog.log", (int)(starttime), (int)(endtime)); 
            // 发送图表运行完毕的事件
            SimulateDataModelEventArgs e = new SimulateDataModelEventArgs();
            e.TimeStart = starttime;
            e.TimeEnd = endtime;
            if (RunDrawFinished != null)
            {
                RunDrawFinished(this, e);
            }
        }

        #endregion
        
        #region Get & Set Value

        /// <summary>
        /// 获取一段位变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public int[] GetValue_Bit(string var, int size)
        {
            // 找到第一个非字母的字符
            int i = 0;
            while (char.IsLetter(var[i])) i++;
            // 获得基名称和偏移地址
            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            // 设置返回的数组
            int[] ret = new int[size];
            UInt32[] _ret32;
            switch (name)
            {
                // 基名称为位变量的名称
                case "X": case "Y": case "M":case "C":case "T":case "S":
                    // 调用dll函数，获得u32位整数的值
                    _ret32 = new UInt32[size];
                    GetBit(var, size, _ret32);
                    // 类型转换并返回
                    for (i = 0; i < size; i++)
                        ret[i] = (int)(_ret32[i]);
                    return ret;
                default:
                    throw new ArgumentException("Unidentified variable {0:s}", var);
            }
        }
        
        /// <summary>
        /// 获取一段字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public int[] GetValue_Word(string var, int size)
        {
            // 找到第一个非字母的字符
            int i = 0;
            while (char.IsLetter(var[i])) i++;
            // 获得基名称和偏移地址
            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            // 设置返回的数组
            int[] ret = new int[size];
            UInt32[] _ret32;
            switch (name)
            {
                // 基名称为字变量的名称
                case "D": case "TV": case "CV":
                    // 基名称为CV时，注意CV后面存在双字变量
                    if (name.Equals("CV") && addr >= 200)
                    {
                        throw new ArgumentException("{0:s} is DoubleWord.", var);
                    }
                    _ret32 = new UInt32[size];
                    GetWord(var, size, _ret32);
                    // 类型转换，注意还原符号位
                    for (i = 0; i < size; i++)
                        ret[i] = (int)((Int32)(_ret32[i]));
                    return ret;
                default:
                    throw new ArgumentException("Unidentified variable {0:s}", var);
            }
        }
        
        /// <summary>
        /// 获取一段双字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public long[] GetValue_DWord(string var, int size)
        {
            // 找到第一个非字母的字符
            int i = 0;
            while (char.IsLetter(var[i])) i++;
            // 获得基名称和偏移地址
            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            // 设置返回的数组
            long[] ret = new long[size];
            UInt64[] _ret64;
            switch (name)
            {
                // 基名称为双字变量的名称
                case "D": case "CV":
                    if (name.Equals("CV") && addr < 200)
                    {
                        throw new ArgumentException("{0:s} is Word.", var);
                    }
                    _ret64 = new UInt64[size];
                    GetDoubleWord(var, size, _ret64);
                    for (i = 0; i < size; i++)
                        ret[i] = (long)((Int64)(_ret64[i]));
                    return ret;
                default:
                    throw new ArgumentException("Unidentified variable {0:s}", var);
            }
        }
        
        /// <summary>
        /// 获取一段浮点变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">长度</param>
        /// <returns>获取的值的数组</returns>
        public double[] GetValue_Float(string var, int size)
        {
            // 找到第一个非字母的字符
            int i = 0;
            while (char.IsLetter(var[i])) i++;
            // 获得基名称和偏移地址
            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            // 设置返回的数组
            double[] ret = new double[size];
            switch (name)
            {
                // 基名称为浮点变量的名称
                case "D":
                    GetFloat(var, size, ret);
                    return ret;
                default:
                    throw new ArgumentException("Unidentified variable {0:s}", var);
            }
        }

        /// <summary>
        /// 获取脉冲信号的频率
        /// </summary>
        /// <param name="var">脉冲输出口</param>
        /// <returns></returns>
        public Int64 GetFeq(string var)
        {
            // 找到第一个非字母的字符
            int i = 0;
            while (char.IsLetter(var[i])) i++;
            // 获得基名称和偏移地址
            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            // 设置返回的数组
            Int64[] ret = new Int64[1];
            switch (name)
            {
                // 基名称为浮点变量的名称
                case "D":
                    GetFeq(var, ret);
                    return ret[0];
                default:
                    throw new ArgumentException("Unidentified variable {0:s}", var);
            }
        }

        /// <summary>
        /// 设置一段位变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Bit(string var, int size, int[] input)
        {
            // 找到第一个非字母的字符
            int i = 0;
            while (char.IsLetter(var[i])) i++;
            // 获得基名称和偏移地址
            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            // 设置返回的数组
            UInt32[] _ret32;
            switch (name)
            {
                // 基名称为位变量的名称
                case "X": case "Y": case "M": case "C": case "T": case "S":
                    _ret32 = new UInt32[size];
                    for (i = 0; i < size; i++)
                        _ret32[i] = (UInt32)(input[i]);
                    SetBit(var, size, _ret32);
                    break;
                default:
                    throw new ArgumentException("Unidentified variable {0:s}", var);
            }
        }

        /// <summary>
        /// 设置一段字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Word(string var, int size, int[] input)
        {
            // 找到第一个非字母的字符
            int i = 0;
            while (char.IsLetter(var[i])) i++;
            // 获得基名称和偏移地址
            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            // 设置返回的数组
            UInt32[] _ret32;
            switch (name)
            {
                // 基名称为位变量的名称
                case "D": case "TV": case "CV":
                    if (name.Equals("CV") && addr >= 200)
                    {
                        throw new ArgumentException("{0:s} is DoubleWord.", var);
                    }
                    _ret32 = new UInt32[size];
                    for (i = 0; i < size; i++)
                        _ret32[i] = (UInt32)(input[i]);
                    SetWord(var, size, _ret32);
                    break;
                default:
                    throw new ArgumentException("Unidentified variable {0:s}", var);
            }
        }
        
        /// <summary>
        /// 设置一段双字变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_DWord(string var, int size, long[] input)
        {
            // 找到第一个非字母的字符
            int i = 0;
            while (char.IsLetter(var[i])) i++;
            // 获得基名称和偏移地址
            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            // 设置返回的数组
            UInt64[] _ret64;
            switch (name)
            {
                case "D": case "CV":
                    if (name.Equals("CV") && addr < 200)
                    {
                        throw new ArgumentException("{0:s} is Word.", var);
                    }
                    _ret64 = new UInt64[size];
                    for (i = 0; i < size; i++)
                        _ret64[i] = (UInt64)(input[i]);
                    SetDoubleWord(var, size, _ret64);
                    break;
                default:
                    throw new ArgumentException("Unidentified variable {0:s}", var);
            }
        }

        /// <summary>
        /// 设置一段浮点变量的值
        /// </summary>
        /// <param name="var">名称</param>
        /// <param name="size">大小</param>
        /// <param name="input">输入的值</param>
        public void SetValue_Float(string var, int size, double[] input)
        {
            // 找到第一个非字母的字符
            int i = 0;
            while (char.IsLetter(var[i])) i++;
            // 获得基名称和偏移地址
            string name = var.Substring(0, i);
            int addr = int.Parse(var.Substring(i));
            // 设置返回的数组
            switch (name)
            {
                case "D":
                    SetFloat(var, size, input);
                    break;
                default:
                    throw new ArgumentException("Unidentified variable {0:s}", var);
            }
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
        
        //private List<SimulateDataModel> locksdmodels = new List<SimulateDataModel>();
        
        /// <summary>
        /// 锁定一个视点
        /// </summary>
        /// <param name="sdmodel">视点模型</param>
        public void Lock(SimulateDataModel sdmodel)
        {
            // 将视点内的数据点依次添加
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
                }
            }
        }
        
        /// <summary>
        /// 解锁一个视点
        /// </summary>
        /// <param name="sdmodel">视点模型</param>
        public void Unlock(SimulateDataModel sdmodel)
        {
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
            }
        }
        #endregion

        #region View Value

        //private List<SimulateDataModel> viewsdmodels = new List<SimulateDataModel>();
        //private Dictionary<string, SimulateDataModel> viewsdmodels = new Dictionary<string, SimulateDataModel>();
        
        /// <summary>
        /// 监视一个视点
        /// </summary>
        /// <param name="sdmodel">视点模型</param>
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
            }
        }
        
        /// <summary>
        /// 解除监视一个视点
        /// </summary>
        /// <param name="sdmodel"></param>
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
            }
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// 当完成一次数据运行后，触发这个代理
        /// </summary>
        public event SimulateDataModelEventHandler RunDataFinished = delegate { };

        /// <summary>
        /// 当完成一次图表运行后，触发这个代理
        /// </summary>
        public event SimulateDataModelEventHandler RunDrawFinished = delegate { };

        #endregion

    }
}

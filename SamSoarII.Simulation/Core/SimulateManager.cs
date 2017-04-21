using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

using SamSoarII.Simulation.Core.VariableModel;
using SamSoarII.Simulation.Core.DataModel;
using SamSoarII.Simulation.UI.Chart;

/// <summary>
/// Namespace : SamSoarII.Simulation
/// ClassName : SimulateModel
/// Version   : 1.0
/// Date      : 2017/4/13
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 对仿真的变量，视点和仿真过程进行管理的类
/// </remarks>

namespace SamSoarII.Simulation.Core
{
    public class SimulateManager
    {
        #region Numbers
        /// <summary>
        /// dll模型
        /// </summary>
        private SimulateDllModel dllmodel;
        /// <summary>
        /// 变量模型的列表
        /// </summary>
        private LinkedList<SimulateVariableModel> vlist;
        /// <summary>
        /// 未锁定的变量单元的字典，根据名称来获得匹配集合
        /// </summary>
        private Dictionary<string, List<SimulateVariableUnit> > udict;
        /// <summary>
        /// 已锁定的变量单元的字典，根据名称来获得匹配集合
        /// </summary>
        private Dictionary<string, List<SimulateVariableUnit> > ldict;
        /// <summary>
        /// 变量名称和别名的字典
        /// </summary>
        private Dictionary<string, string> vndict;
        /// <summary>
        /// 已锁定的视点（输入视点）的字典，根据名称获得匹配集合
        /// </summary>
        private Dictionary<string, SimulateDataModel> lddict;
        /// <summary>
        /// 已监视的视点（输出视点）的字段，根据名称获得匹配集合
        /// </summary>
        private Dictionary<string, SimulateDataModel> vddict;
        #endregion

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public SimulateManager()
        {
            // 初始化成员
            dllmodel = new SimulateDllModel();
            vlist = new LinkedList<SimulateVariableModel>();
            udict = new Dictionary<string, List<SimulateVariableUnit> >();
            ldict = new Dictionary<string, List<SimulateVariableUnit> >();
            vndict = new Dictionary<string, string>();
            lddict = new Dictionary<string, SimulateDataModel>();
            vddict = new Dictionary<string, SimulateDataModel>();
            // 初始化事件监听
            dllmodel.RunDataFinished += OnRunDataFinished;
            dllmodel.RunDrawFinished += OnRunDrawFinished;
            // 初始化更新线程
            updateactive = false;
            updatethread = null;
        }
        
        /// <summary>
        /// 所有变量模型
        /// </summary>
        public IEnumerable<SimulateVariableModel> Variables
        {
            get { return this.vlist; }
        }

        /// <summary>
        /// 所有变量单元
        /// </summary>
        public IEnumerable<SimulateVariableUnit> Values
        {
            get
            {
                // 新建返回的列表
                List<SimulateVariableUnit> ret = new List<SimulateVariableUnit>();
                // 获得所有未锁定的变量单元
                foreach (List<SimulateVariableUnit> list in udict.Values)
                {
                    foreach (SimulateVariableUnit svunit in list)
                    {
                        ret.Add(svunit);
                    }
                }
                // 获得所有已锁定的变量单元
                foreach (List<SimulateVariableUnit> list in ldict.Values)
                {
                    foreach (SimulateVariableUnit svunit in list)
                    {
                        ret.Add(svunit);
                    }
                }
                return ret;
            }
        }

        #region Update Control
        /// <summary>
        /// 更新线程
        /// </summary>
        private Thread updatethread;
        /// <summary>
        /// 更新线程是否存活
        /// </summary>
        private bool updateactive;
        /// <summary>
        /// 更新线程运行的更新方法
        /// </summary>
        private void Update()
        {
            // 存活时运行循环
            while (updateactive)
            {
                // 更新所有变量模型
                foreach (SimulateVariableModel svmodel in Variables)
                {
                    svmodel.Update(dllmodel);
                }
                // 更新所有未锁定的变量单元
                foreach (List<SimulateVariableUnit> svulist in udict.Values)
                {
                    foreach (SimulateVariableUnit svunit in svulist)
                    {
                        svunit.Update(dllmodel);
                    }
                }
                // 更新所有已锁定的变量单元
                foreach (List<SimulateVariableUnit> svulist in ldict.Values)
                {
                    foreach (SimulateVariableUnit svunit in svulist)
                    {
                        svunit.Set(dllmodel);
                    }
                }
                // 等待
                Thread.Sleep(50);
            }
        }
        /// <summary>
        /// 更新线程开始
        /// </summary>
        private void UpdateStart()
        {
            // 已经处于运行状态则忽略
            if (SimuStatus != SIMU_RUNNING)
                return;
            // 新建更新线程
            if (updatethread == null)
            {
                updateactive = true;
                updatethread = new Thread(Update);
                updatethread.Start();
            }
        }
        /// <summary>
        /// 更新线程终止
        /// </summary>
        private void UpdateStop()
        {
            // 终止更新线程
            if (updatethread != null)
            {
                updateactive = false;
                updatethread.Abort();
                updatethread = null;
            }
        }
        #endregion

        #region Simulation Control
        /// <summary> 仿真状态：停止 </summary>
        private const int SIMU_STOP = 0x00;
        /// <summary> 仿真状态：运行 </summary>
        private const int SIMU_RUNNING = 0x01;
        /// <summary> 仿真状态：暂停 </summary>
        private const int SIMU_PAUSE = 0x02;
        /// <summary>
        /// 仿真状态
        /// </summary>
        private int simustatus;
        /// <summary>
        /// 仿真状态
        /// </summary>
        private int SimuStatus
        {
            get { return this.simustatus; }
            set { this.simustatus = value; }
        }
        /// <summary>
        /// 开始仿真
        /// </summary>
        public void Start()
        {
            // 已处于仿真状态则忽略
            if (SimuStatus == SIMU_RUNNING)
                return;
            // 开始仿真
            SimuStatus = SIMU_RUNNING;
            dllmodel.Start();
            UpdateStart();
        }
        /// <summary>
        /// 暂停仿真
        /// </summary>
        public void Pause()
        {
            // 已处于停止或暂停状态则忽略
            if (SimuStatus == SIMU_PAUSE || SimuStatus == SIMU_STOP)
                return;
            // 暂停仿真
            SimuStatus = SIMU_PAUSE;
            dllmodel.Pause();
            UpdateStop();
        }
        /// <summary>
        /// 停止仿真
        /// </summary>
        public void Stop()
        {
            // 已处于停止状态则忽略
            if (SimuStatus == SIMU_STOP)
                return;
            // 停止仿真
            SimuStatus = SIMU_STOP;
            dllmodel.Abort();
            UpdateStop();
            // 初始化所有变量
            int[] emptyBuffer = new int[8192];
            long[] emptyBufferLong = new long[56];
            for (int i = 0; i < emptyBuffer.Length; i++)
            {
                emptyBuffer[i] = 0;
            }
            dllmodel.SetValue_Bit("X0", 128, emptyBuffer);
            dllmodel.SetValue_Bit("Y0", 128, emptyBuffer);
            dllmodel.SetValue_Bit("M0", 256 << 5, emptyBuffer);
            dllmodel.SetValue_Bit("C0", 256, emptyBuffer);
            dllmodel.SetValue_Bit("T0", 256, emptyBuffer);
            dllmodel.SetValue_Bit("S0", 32 << 5, emptyBuffer);
            dllmodel.SetValue_Word("D0", 8192, emptyBuffer);
            dllmodel.SetValue_Word("TV0", 256, emptyBuffer);
            dllmodel.SetValue_Word("CV0", 200, emptyBuffer);
            dllmodel.SetValue_DWord("CV200", 56, emptyBufferLong);
        }
        #endregion

        #region Variable Manipulation
        /// <summary>
        /// 添加一个变量模型
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="size">大小</param>
        /// <param name="type">类型</param>
        public void Add(string name, int size, string type)
        {
            // 终止更新线程，防止资源冲突
            UpdateStop();
            // 根据给定参数，创建新的模型
            SimulateVariableModel svmodel = SimulateVariableModel.Create(name, size, type);
            vlist.AddLast(svmodel);
            // 重启更新线程
            UpdateStart();
        }
        /// <summary>
        /// 添加一个变量模型
        /// </summary>
        /// <param name="svmodel">变量模型</param>
        public void Add(SimulateVariableModel svmodel)
        {
            // 终止更新线程，防止资源冲突
            UpdateStop();
            // 添加变量模型
            vlist.AddLast(svmodel);
            // 重启更新线程
            UpdateStart();
        }
        /// <summary>
        /// 添加一个变量单元
        /// </summary>
        /// <param name="svunit">变量单元</param>
        public void Add(SimulateVariableUnit svunit)
        {
            // 终止更新线程，防止资源冲突
            UpdateStop();

            // 变量单元名称所在的非锁定变量列表
            List<SimulateVariableUnit> svulist = null;
            // 变量单元名称所在的锁定变量列表
            List<SimulateVariableUnit> svllist = null;
            // 非锁定变量列表不存在则新建
            if (!udict.ContainsKey(svunit.Name))
            {
                svulist = new List<SimulateVariableUnit>();
                udict.Add(svunit.Name, svulist);
            }
            // 非锁定变量列表存在则获取
            else
            {
                svulist = udict[svunit.Name];
            }
            // 锁定变量列表不存在则新建
            if (!ldict.ContainsKey(svunit.Name))
            {
                svllist = new List<SimulateVariableUnit>();
                ldict.Add(svunit.Name, svllist);
            }
            // 锁定变量列表存在则获取
            else
            {
                svllist = ldict[svunit.Name];
            }
            // 如果存在同名的锁定变量
            if (svllist.Count() > 0)
            {
                SimulateVariableUnit lunit = svllist.First();
                // 将同名的变量单元都设为锁定并添加到锁定列表中
                foreach (SimulateVariableUnit _svunit in svulist)
                {
                    _svunit.Value = lunit.Value;
                    _svunit.Islocked = true;
                    svllist.Add(_svunit);
                }
                svunit.Value = lunit.Value;
                svunit.Islocked = true;
                svllist.Add(svunit);
                // 非锁定列表清空
                svulist.Clear();
            }
            // 不存在同名的锁定变量
            else
            {
                svulist.Add(svunit);
            }
            // 存在别名则设置
            if (vndict.ContainsKey(svunit.Name))
            {
                svunit.Var = vndict[svunit.Name];
            }
            // 安装管理器
            svunit.Setup(this);

            // 重启更新线程
            UpdateStart();
        }
        /// <summary>
        /// 移除一个变量模型
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="size">长度</param>
        public void Remove(string name, int size)
        {
            // 终止更新线程，防止资源冲突
            UpdateStop();
            // 找到符合参数条件的模型并删除
            IEnumerable<SimulateVariableModel> fit = Variables.Where(
                (SimulateVariableModel svmodel) =>
                    {
                        return (svmodel.Name.Equals(name) && svmodel.Size == size);
                    }
            );
            foreach (SimulateVariableModel svmodel in fit)
            {
                vlist.Remove(svmodel);
            }
            // 重启更新线程
            UpdateStart();
        }
        /// <summary>
        /// 移除一个变量模型
        /// </summary>
        /// <param name="svmodel">变量模型</param>
        public void Remove(SimulateVariableModel svmodel)
        {
            // 终止更新线程，防止资源冲突
            UpdateStop();
            // 移除这个变量模型
            vlist.Remove(svmodel);
            // 重启更新线程
            UpdateStart();
        }
        /// <summary>
        /// 移除一个变量单元
        /// </summary>
        /// <param name="svunit"></param>
        public void Remove(SimulateVariableUnit svunit)
        {
            // 终止更新线程，防止资源冲突
            UpdateStop();

            // 变量单元名称所在的非锁定变量列表
            List<SimulateVariableUnit> svulist = null;
            // 变量单元名称所在的锁定变量列表
            List<SimulateVariableUnit> svllist = null;
            // 非锁定变量列表不存在则新建
            if (!udict.ContainsKey(svunit.Name))
            {
                svulist = new List<SimulateVariableUnit>();
                udict.Add(svunit.Name, svulist);
            }
            // 非锁定变量列表存在则获取
            else
            {
                svulist = udict[svunit.Name];
            }
            // 锁定变量列表不存在则新建
            if (!ldict.ContainsKey(svunit.Name))
            {
                svllist = new List<SimulateVariableUnit>();
                ldict.Add(svunit.Name, svllist);
            }
            // 锁定变量列表存在则获取
            else
            {
                svllist = ldict[svunit.Name];
            }
            // 非锁定变量列表存在则移除
            if (svulist.Contains(svunit))
            {
                svulist.Remove(svunit);
            }
            // 锁定变量列表存在则移除
            if (svllist.Contains(svunit))
            {
                svllist.Remove(svunit);
                // 注意要解锁
                svunit.Islocked = false;
            }

            // 重启更新线程
            UpdateStart();
        }
        /// <summary>
        /// 替换一个变量单元
        /// </summary>
        /// <param name="oldUnit">旧的变量单元</param>
        /// <param name="newUnit">新的变量单元</param>
        public void Replace(SimulateVariableUnit oldUnit, SimulateVariableUnit newUnit)
        {
            // 终止更新线程，防止资源冲突
            // 调用的子方法来处理，这里不用
            // UpdateStop();

            // 旧的不为空则删除
            if (oldUnit != null)
            {
                Remove(oldUnit);
            }
            // 新的不为空则添加
            if (newUnit != null)
            {
                Add(newUnit);
                // 如果是已经锁定的变量，注册到锁定侧
                if (newUnit.Islocked)
                {
                    Lock(newUnit);
                }
            }
            
            // 重启更新线程
            //UpdateStart();
        }
        /// <summary>
        /// 替换变量单元的别名
        /// </summary>
        /// <param name="bname">变量名字</param>
        /// <param name="vname">新的变量别名</param>
        public void Rename(string bname, string vname)
        {
            // 终止更新线程，防止资源冲突
            UpdateStop();
            // 如果新的别名为空，视为删除别名
            if (vname.Equals(String.Empty))
            {
                if (vndict.ContainsKey(bname))
                {
                    vndict.Remove(bname);
                }
            }
            // 删除旧别名并添加新别名
            else
            {
                if (vndict.ContainsKey(bname))
                {
                    vndict.Remove(bname);
                }
                vndict.Add(bname, vname);
            }

            // 替换当前所有对应变量的别名
            if (udict.ContainsKey(bname))
            {
                List<SimulateVariableUnit> svulist = udict[bname];
                foreach (SimulateVariableUnit svunit in svulist)
                {
                    svunit.Var = vname;
                }
            }
            if (ldict.ContainsKey(bname))
            {
                List<SimulateVariableUnit> svllist = ldict[bname];
                foreach (SimulateVariableUnit svunit in svllist)
                {
                    svunit.Var = vname;
                }
            }
            // 重启更新线程
            UpdateStart();
        }
        /// <summary>
        /// 获得第一个符合参数条件的变量单元
        /// </summary>
        /// <returns></returns>
        public SimulateVariableUnit Find(string name, string type)
        {
            if (udict.ContainsKey(name))
            {
                List<SimulateVariableUnit> svulist = udict[name];
                IEnumerable<SimulateVariableUnit> fit = svulist.Where(
                    (SimulateVariableUnit svunit) =>
                        {
                            return (svunit.Name.Equals(name) && svunit.Type.Equals(type));
                        }
                );
                if (fit.Count() > 0)
                {
                    return fit.First();
                }
            }
            if (ldict.ContainsKey(name))
            {
                List<SimulateVariableUnit> svulist = ldict[name];
                IEnumerable<SimulateVariableUnit> fit = svulist.Where(
                    (SimulateVariableUnit svunit) =>
                    {
                        return (svunit.Name.Equals(name) && svunit.Type.Equals(type));
                    }
                );
                if (fit.Count() > 0)
                {
                    return fit.First();
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="svunit"></param>
        /// <returns></returns>
        public string GetVariableName(SimulateVariableUnit svunit)
        {
            if (vndict.ContainsKey(svunit.Name))
            {
                return vndict[svunit.Name];
            }
            return svunit.Name;
        }
        #endregion

        #region Lock & View
        /// <summary>
        /// 锁定一个变量单元
        /// </summary>
        /// <param name="svunit">变量单元</param>
        public void Lock(SimulateVariableUnit svunit)
        {
            // 终止更新线程，防止资源冲突
            UpdateStop();

            // 变量单元名称所在的非锁定变量列表
            List<SimulateVariableUnit> svulist = null;
            // 变量单元名称所在的锁定变量列表
            List<SimulateVariableUnit> svllist = null;
            // 非锁定变量列表不存在则新建
            if (!udict.ContainsKey(svunit.Name))
            {
                svulist = new List<SimulateVariableUnit>();
                udict.Add(svunit.Name, svulist);
            }
            // 非锁定变量列表存在则获取
            else
            {
                svulist = udict[svunit.Name];
            }
            // 锁定变量列表不存在则新建
            if (!ldict.ContainsKey(svunit.Name))
            {
                svllist = new List<SimulateVariableUnit>();
                ldict.Add(svunit.Name, svllist);
            }
            // 锁定变量列表存在则获取
            else
            {
                svllist = ldict[svunit.Name];
            }
            // 非锁定变量列表不存在
            if (!svulist.Contains(svunit))
            {
                // 存在锁定重设的情况，不算做错误
                //throw new ArgumentException(String.Format("Cannot found {0:s} in variable unit collection.", svunit.ToString()));
            }
            // 将非锁定列表中的变量加入到锁定列表中
            foreach (SimulateVariableUnit _svunit in svulist)
            {
                //_svunit.Value = svunit.Value;
                //_svunit.Islocked = true;
                svllist.Add(_svunit);
            }
            // 清空非锁定列表
            svulist.Clear();
            // 将锁定列表中的变量重设锁定
            foreach (SimulateVariableUnit _svunit in svllist)
            {
                _svunit.Value = svunit.Value;
                _svunit.Islocked = true;
            }
            // 调用dll的锁定接口
            dllmodel.Lock(svunit.Name);

            // 重启更新线程
            UpdateStart();
        }
        /// <summary>
        /// 解锁一个变量单元
        /// </summary>
        /// <param name="svunit">变量单元</param>
        public void Unlock(SimulateVariableUnit svunit)
        {
            // 终止更新线程，防止资源冲突
            UpdateStop();

            // 变量单元名称所在的非锁定变量列表
            List<SimulateVariableUnit> svulist = null;
            // 变量单元名称所在的锁定变量列表
            List<SimulateVariableUnit> svllist = null;
            // 非锁定变量列表不存在则新建
            if (!udict.ContainsKey(svunit.Name))
            {
                svulist = new List<SimulateVariableUnit>();
                udict.Add(svunit.Name, svulist);
            }
            // 非锁定变量列表存在则获取
            else
            {
                svulist = udict[svunit.Name];
            }
            // 锁定变量列表不存在则新建
            if (!ldict.ContainsKey(svunit.Name))
            {
                svllist = new List<SimulateVariableUnit>();
                ldict.Add(svunit.Name, svllist);
            }
            // 锁定变量列表存在则获取
            else
            {
                svllist = ldict[svunit.Name];
            }
            // 锁定变量列表不存在
            if (!svllist.Contains(svunit))
            {
                throw new ArgumentException(String.Format("Cannot found {0:s} in variable unit collection.", svunit.ToString()));
            }
            // 将锁定列表中的变量单元变为非锁定，并加入到非锁定列表中
            foreach (SimulateVariableUnit _svunit in svllist)
            {
                _svunit.Islocked = false;
                svulist.Add(_svunit);
            }
            // 清空非锁定列表
            svllist.Clear();
            // 调用dll的解锁接口
            dllmodel.Unlock(svunit.Name);

            // 重启更新线程
            UpdateStart();
        }

        public void Lock(SimulateDataModel sdmodel)
        {
            UpdateStop();
            if (!lddict.ContainsKey(sdmodel.Name))
            {
                lddict.Add(sdmodel.Name, sdmodel);
                //sdmodel.IsLock = true;
                dllmodel.Lock(sdmodel);
            }
            UpdateStart();
        }
        
        public void View(SimulateDataModel sdmodel)
        {
            UpdateStop();
            if (!vddict.ContainsKey(sdmodel.Name))
            {
                vddict.Add(sdmodel.Name, sdmodel);
                //sdmodel.IsView = true;
                dllmodel.View(sdmodel);
            }
            UpdateStart();
        }

        public void Unlock(SimulateDataModel sdmodel)
        {
            UpdateStop();
            if (lddict.ContainsKey(sdmodel.Name))
            {
                lddict.Remove(sdmodel.Name);
                //sdmodel.IsLock = false;
                dllmodel.Unlock(sdmodel);
            }
            UpdateStart();
        }
        
        public void Unview(SimulateDataModel sdmodel)
        {
            UpdateStop();
            if (vddict.ContainsKey(sdmodel.Name))
            {
                vddict.Remove(sdmodel.Name);
                //sdmodel.IsView = false;
                dllmodel.Unview(sdmodel);
            }
            UpdateStart();
        }
        #endregion

        public void RunData(double timestart, double timeend)
        {
            dllmodel.RunData(timestart, timeend);
        }

        public void RunDraw(double timestart, double timeend)
        {
            dllmodel.RunDraw(timestart, timeend);
        }

        public void UpdateView(double timestart, double timeend)
        {
            StreamReader fin = new StreamReader("simulog.log");
            while (!fin.EndOfStream)
            {
                string text = fin.ReadLine();
                string[] args = text.Split(' ');
                string name = args[0];
                int time = int.Parse(args[1]);
                ValueSegment vs = null, vsp = null;
                SimulateDataModel sdmodel = vddict[name];
                switch (sdmodel.Type)
                {
                    case "BIT": case "WORD": case "DWORD":
                        vs = new IntSegment();
                        vs.Value = int.Parse(args[2]);
                        break;
                    case "FLOAT":
                        vs = new FloatSegment();
                        vs.Value = float.Parse(args[2]);
                        break;
                    case "DOUBLE":
                        vs = new DoubleSegment();
                        vs.Value = double.Parse(args[2]);
                        break;
                }
                if (sdmodel.Values.Count() == 0)
                {
                    vsp = vs.Clone();
                    vsp.Value = 0;
                    vsp.TimeStart = (int)(timestart);
                    vs.TimeStart = vsp.TimeEnd = time;
                    sdmodel.Add(vsp);
                    sdmodel.Add(vs);
                }
                else
                {
                    vsp = sdmodel.Values.Last();
                    vs.TimeStart = vsp.TimeEnd = time;
                    sdmodel.Add(vs);
                }
            }
            foreach (SimulateDataModel sdmodel in vddict.Values)
            {
                if (sdmodel.Values.Count() > 0)
                {
                    sdmodel.Values.Last().TimeEnd = (int)(timeend);
                }
            }
        }

        #region Event Handler
        public event SimulateDataModelEventHandler RunDataFinished;
        private void OnRunDataFinished(object sender, SimulateDataModelEventArgs e)
        {
            UpdateView(e.TimeStart, e.TimeEnd);
            if (RunDataFinished != null)
            {
                RunDataFinished(this, e);
            }
        }

        public event SimulateDataModelEventHandler RunDrawFinished;
        private void OnRunDrawFinished(object sender, SimulateDataModelEventArgs e)
        {
            UpdateView(e.TimeStart, e.TimeEnd);
            if (RunDrawFinished != null)
            {
                RunDrawFinished(this, e);
            }
        }

        #endregion

    }
}

using SamSoarII.AppMain.UI;
using SamSoarII.AppMain.UI.Monitor;
using SamSoarII.Communication;
using SamSoarII.Communication.Command;
using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.Utility;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

/// <summary>
/// Namespace : SamSoarII.AppMain.Project
/// ClassName : DownloadHelper
/// Version   : 1.0
/// Date      : 2017/6/14
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 处理工程相关信息，打包成二进制数据并传到下位PLC
/// </remarks>

namespace SamSoarII.AppMain.Project
{
    /// <summary> 寄存器类型 </summary>
    public enum DownloadRegisterType
    {
        NULL = 0x00,
        K, K32, H, H32, F, STR,
        X, Y, S, M, C, T,
        D, TV, CV, AI, AO, V, Z,
        CV32
    }

    /// <summary> 下载的帮助类 </summary>
    public class DownloadHelper
    {
        #region Constant Property
        
        /// <summary> 错误码：梯形图检查错误 </summary>
        public const int DOWNLOAD_LADDER_ERROR = 0x01;
        /// <summary> 错误码：函数功能块检查错误 </summary>
        public const int DOWNLOAD_FUNCBLOCK_ERROR = 0x02;

        /// <summary> 选项：是否包含程序 </summary>
        public const int OPTION_PROGRAM = 0x01;
        /// <summary> 选项：是否包含注释 </summary>
        public const int OPTION_COMMENT = 0x02;
        /// <summary> 选项：是否包含初始化 </summary>
        public const int OPTION_INITIALIZE = 0x04;
        /// <summary> 选项：是否包含设置 </summary>
        public const int OPTION_SETTING = 0x08;
        /// <summary> 选项：是否包含监视 </summary>
        public const int OPTION_MONITOR = 0x10;
        
        #endregion

        #region Data
        /// <summary> 未压缩的原数据，通常需要PLC去识别分析 </summary>
        static private List<byte> odata 
            = new List<byte>();
        /// <summary> 压缩为rar包的数据 </summary>
        static private List<byte> edata
            = new List<byte>();
        /// <summary> 软元件表 </summary>
        static private List<IValueModel> regs
            = new List<IValueModel>();
        /// <summary> 软元件ID表 </summary>
        static private Dictionary<string, int> regids 
            = new Dictionary<string, int>(); 

        static public int DataLen
        {
            get { return odata.Count() + edata.Count(); }
        }

        #endregion

        #region Generate Data

        /// <summary>
        /// 将工程文件写入二进制数据，保存在根目录下的down.bin中
        /// </summary>
        /// <param name="pmodel">工程类</param>
        /// <param name="option">选项</param>
        static public void Write(ProjectModel pmodel, int option)
        {
            // 初始化，清空所有记录
            odata.Clear();
            edata.Clear();
            regs.Clear();
            regids.Clear();
            // 保存选项
            edata.Add(Int32_Low(option));
            // 写入设置参数
            WriteParameters(pmodel);
            // 获取所有已经使用的软元件
            GetRegisters(pmodel.MainRoutine);
            foreach (LadderDiagramViewModel ldvmodel in pmodel.SubRoutines)
            {
                GetRegisters(ldvmodel);
            }
            if ((option & OPTION_MONITOR) != 0)
            {
                foreach (MonitorVariableTable mvtable in pmodel.MMonitorManager.MMWindow.tables)
                {
                    GetRegisters(mvtable);
                }
            }
            // 写入所有软元件
            WriteRegisters(option);
            // 写入所有梯形图
            Write(pmodel.MainRoutine, option);
            foreach (LadderDiagramViewModel ldvmodel in pmodel.SubRoutines)
            {
                Write(ldvmodel, option);
            }
            // 写入所有函数功能块
            foreach (FuncBlockViewModel fbvmdoel in pmodel.FuncBlocks)
            {
                Write(fbvmdoel, option);
            }
            Write(pmodel.MTVModel, option);
            // 写入监视表
            Write(pmodel.MMonitorManager.MMWindow, option);
            // 写入工程树
            ProjectTreeView ptview = pmodel.IFacade.PTView;
            Write(ptview, option);
            // 设置路径
            string currentpath = Environment.CurrentDirectory;
            string execfile = String.Format(@"{0:s}\downc.bin", currentpath);
            string datafile = String.Format(@"{0:s}\downe.bin", currentpath);
            string packfile = String.Format(@"{0:s}\downe.rar", currentpath);
            // 写入待压缩的压缩数据
            BinaryWriter bw = new BinaryWriter(
                new FileStream(datafile, FileMode.Create));
            bw.Write(edata.ToArray());
            bw.Close();
            // 后台调用Rar进行压缩
            Process cmd = null;
            cmd = new Process();
            cmd.StartInfo.FileName
                = String.Format(@"{0:s}\rar\Rar", currentpath);
            cmd.StartInfo.Arguments
                = String.Format("a -m5 -ep \"{0:s}\" \"{1:s}\"",
                    packfile, datafile);
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();
            cmd.WaitForExit();
            BinaryReader br = null;
            // 获得执行程序
            br = new BinaryReader(
                new FileStream(execfile, FileMode.Open));
            odata.Clear();
            while (br.BaseStream.CanRead)
            {
                try
                {
                    odata.Add(br.ReadByte());
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
            br.Close();
            // 获得压缩后的数据
            br = new BinaryReader(
                new FileStream(packfile, FileMode.Open));
            edata.Clear();
            /*
            while (br.BaseStream.CanRead)
            {
                try
                {
                    edata.Add(br.ReadByte());
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
            br.Close();
            */
        }
        /// <summary>
        /// 写入所有参数设置
        /// </summary>
        /// <param name="pmodel">工程类</param>
        static private void WriteParameters(ProjectModel pmodel)
        {
            int szid = odata.Count();
            odata.Add(0x00);
            odata.Add(0x00);
            CommunicationInterfaceParams com232params =
                (CommunicationInterfaceParams)(ProjectPropertyManager.ProjectPropertyDic["CommunParams232"]);
            CommunicationInterfaceParams com485params =
                (CommunicationInterfaceParams)(ProjectPropertyManager.ProjectPropertyDic["CommunParams485"]);
            PasswordParams pwparams =
                (PasswordParams)(ProjectPropertyManager.ProjectPropertyDic["PasswordParams"]);
            FilterParams ftparams =
                (FilterParams)(ProjectPropertyManager.ProjectPropertyDic["FilterParams"]);
            HoldingSectionParams hsparams =
                (HoldingSectionParams)(ProjectPropertyManager.ProjectPropertyDic["HoldingSectionParams"]);
            odata.Add(Int32_Low(com232params.BaudRateIndex));
            odata.Add(Int32_Low(com232params.DataBitIndex));
            odata.Add(Int32_Low(com232params.StopBitIndex));
            odata.Add(Int32_Low(com232params.CheckCodeIndex));
            odata.Add(Int32_Low(com485params.BaudRateIndex));
            odata.Add(Int32_Low(com485params.DataBitIndex));
            odata.Add(Int32_Low(com485params.StopBitIndex));
            odata.Add(Int32_Low(com485params.CheckCodeIndex));

            odata.Add(Int32_Low(com232params.StationNum));
            odata.Add(Int32_Low(Int32_Low(com232params.Timeout)));
            odata.Add(Int32_Low(Int32_High(com232params.Timeout)));
            odata.Add((byte)(pwparams.UPIsChecked ? 1 : 0));
            Write(pwparams.UPassword);
            odata.Add((byte)(com232params.CommuType));
            odata.Add((byte)(com485params.CommuType));
            
            odata.Add((byte)(pwparams.DPIsChecked ? 1 : 0));
            Write(pwparams.DPassword);
            odata.Add((byte)(pwparams.MPIsChecked ? 1 : 0));
            Write(pwparams.MPassword);
            odata.Add((byte)(ftparams.IsChecked ? 1 : 0));
            int fttime = 1 << (ftparams.FilterTimeIndex + 1);
            odata.Add(Int32_Low(fttime));
            odata.Add(Int32_High(fttime));
            odata.Add(Int32_Low(hsparams.MStartAddr));
            odata.Add(Int32_High(hsparams.MStartAddr));
            odata.Add(Int32_Low(hsparams.MLength));
            odata.Add(Int32_High(hsparams.MLength));
            odata.Add(Int32_Low(hsparams.SStartAddr));
            odata.Add(Int32_High(hsparams.SStartAddr));
            odata.Add(Int32_Low(hsparams.SLength));
            odata.Add(Int32_High(hsparams.SLength));
            odata.Add(Int32_Low(hsparams.DStartAddr));
            odata.Add(Int32_High(hsparams.DStartAddr));
            odata.Add(Int32_Low(hsparams.DLength));
            odata.Add(Int32_High(hsparams.DLength));
            odata.Add(Int32_Low(hsparams.CVStartAddr));
            odata.Add(Int32_High(hsparams.CVStartAddr));
            odata.Add(Int32_Low(hsparams.CVLength));
            odata.Add(Int32_High(hsparams.CVLength));
            int sz = odata.Count() - szid - 2;
            odata[szid] = Int32_Low(sz);
            odata[szid + 1] = Int32_High(sz);
        }
        /// <summary>
        /// 写入所有软元件的信息
        /// </summary>
        /// <param name="option">选项</param>
        unsafe static private void WriteRegisters(int option)
        {
            // 头标志（0xfc）和长度
            edata.Add(0xfc);
            edata.Add(Int32_Low(regs.Count()));
            edata.Add(Int32_High(regs.Count()));
            // 将表中的所有软元件逐一写入
            foreach (IValueModel ivmodel in regs)
            {
                // 整数常量（包括单字和双字）
                if (ivmodel is KWordValue
                 || ivmodel is KDoubleWordValue)
                {
                    if (ivmodel is KWordValue)
                        edata.Add((byte)(DownloadRegisterType.K));
                    else
                        edata.Add((byte)(DownloadRegisterType.K32));
                    int value = int.Parse(ivmodel.ValueString.Substring(1));
                    for (int i = 0; i < 4; i++)
                    {
                        edata.Add((byte)(value & 0xff));
                        value >>= 8;
                    }
                }
                // 16进制表示的整数常量（包括单字和双字）
                else if (ivmodel is HWordValue
                      || ivmodel is HDoubleWordValue)
                {
                    if (ivmodel is HWordValue)
                        edata.Add((byte)(DownloadRegisterType.H));
                    else
                        edata.Add((byte)(DownloadRegisterType.H32));
                    string value = ivmodel.ValueString;
                    for (int i = 0; i < 4; i++)
                    {
                        int p = value.Length - i * 2 - 2;
                        byte valueb = p < 1
                            ? (byte)(0x00)
                            : char.IsDigit(value[p])
                                ? (byte)(value[p] - '0')
                                : (byte)(char.ToUpper(value[p]) - 'A' + 10);
                        p = value.Length - i * 2 - 1;
                        valueb <<= 4;
                        valueb |= p < 1
                            ? (byte)(0x00)
                            : char.IsDigit(value[p])
                                ? (byte)(value[p] - '0')
                                : (byte)(char.ToUpper(value[p]) - 'A' + 10);
                        edata.Add(valueb);
                    }
                }
                // 浮点常量
                else if (ivmodel is KFloatValue)
                {
                    edata.Add((byte)(DownloadRegisterType.F));
                    float valuef = float.Parse(((KFloatValue)ivmodel).ValueString.Substring(1));
                    int value = *((int*)(&valuef));
                    for (int i = 0; i < 4; i++)
                    {
                        edata.Add((byte)(value & 0xff));
                        value >>= 8;
                    }
                }
                // 字符串
                else if (ivmodel is StringValue)
                {
                    edata.Add((byte)(DownloadRegisterType.STR));
                    Write(ivmodel.ValueString);
                }
                // 寄存器
                else
                {
                    // 函数参数获取原型
                    IValueModel _ivmodel = ivmodel;
                    if (_ivmodel is ArgumentValue)
                        _ivmodel = ((ArgumentValue)_ivmodel).Argument;
                    // 判断基地址并写入
                    if (_ivmodel is XBitValue)
                        edata.Add((byte)(DownloadRegisterType.X));
                    if (_ivmodel is YBitValue)
                        edata.Add((byte)(DownloadRegisterType.Y));
                    if (_ivmodel is SBitValue)
                        edata.Add((byte)(DownloadRegisterType.S));
                    if (_ivmodel is MBitValue)
                        edata.Add((byte)(DownloadRegisterType.M));
                    if (_ivmodel is CBitValue)
                        edata.Add((byte)(DownloadRegisterType.C));
                    if (_ivmodel is TBitValue)
                        edata.Add((byte)(DownloadRegisterType.T));
                    if (_ivmodel is DWordValue)
                        edata.Add((byte)(DownloadRegisterType.D));
                    if (_ivmodel is AIWordValue)
                        edata.Add((byte)(DownloadRegisterType.AI));
                    if (_ivmodel is AOWordValue)
                        edata.Add((byte)(DownloadRegisterType.AO));
                    if (_ivmodel is CVWordValue)
                        edata.Add((byte)(DownloadRegisterType.CV));
                    if (_ivmodel is TVWordValue)
                        edata.Add((byte)(DownloadRegisterType.TV));
                    if (_ivmodel is VWordValue)
                        edata.Add((byte)(DownloadRegisterType.V));
                    if (_ivmodel is ZWordValue)
                        edata.Add((byte)(DownloadRegisterType.Z));
                    if (_ivmodel is CV32DoubleWordValue)
                        edata.Add((byte)(DownloadRegisterType.CV32));
                    // 写入下标位置
                    edata.Add(Int32_Low((int)(_ivmodel.Index)));
                    edata.Add(Int32_High((int)(_ivmodel.Index)));
                    // 写入变址信息（基地址，下标）
                    if (_ivmodel.IsVariable)
                    {
                        if (_ivmodel.Offset is VWordValue)
                            edata.Add((byte)(DownloadRegisterType.V));
                        else if (_ivmodel.Offset is ZWordValue)
                            edata.Add((byte)(DownloadRegisterType.Z));
                        else
                            edata.Add(0x00);
                        edata.Add(Int32_Low((int)(_ivmodel.Offset.Index)));
                    }
                    // 非变址软元件则写入空数据
                    else
                    {
                        edata.Add(0x00);
                        edata.Add(0x00);
                    }
                }
                // 根据选项选择是否写入注释
                if ((option & OPTION_COMMENT) != 0)
                {
                    string comment = ValueCommentManager.GetComment(ivmodel);
                    Write(comment);
                }
            }
        }
        /// <summary>
        /// 写入一个梯形图程序
        /// </summary>
        /// <param name="ldvmodel">梯形图程序</param>
        /// <param name="option">选项</param>
        static private void Write(LadderDiagramViewModel ldvmodel, int option)
        {
            if ((option & OPTION_PROGRAM) == 0) return;
            // 写入头标志
            edata.Add(0xff);
            // 记录长度的位置，待写入
            int szid = edata.Count();
            edata.Add(0x00);
            edata.Add(0x00);
            // 依次写入所有网络
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                Write(lnvmodel, option);
            }
            // 计算长度，写入之前记录的位置
            int sz = edata.Count() - szid - 2;
            edata[szid] = Int32_Low(sz);
            edata[szid+1] = Int32_High(sz);
        }
        /// <summary>
        /// 写入一个梯形图网络
        /// </summary>
        /// <param name="lnvmodel">梯形图网络</param>
        /// <param name="option">选项</param>
        static private void Write(LadderNetworkViewModel lnvmodel, int option)
        {
            // 头标志和长度
            edata.Add(0xfe);
            int szid = edata.Count();
            edata.Add(0x00);
            edata.Add(0x00);
            // 是否写入网络注释？
            if ((option & OPTION_COMMENT) != 0)
            {
                Write(lnvmodel.NetworkBrief);
            }
            // 行数和每格的行列联通信息
            edata.Add(Int32_Low(lnvmodel.RowCount));
            int st = edata.Count();
            int le = (lnvmodel.RowCount * GlobalSetting.LadderXCapacity) >> 2;
            edata.AddRange(new byte[le]);
            foreach (BaseViewModel bvmodel in lnvmodel.GetElements())
            {
                int x = bvmodel.X;
                int y = bvmodel.Y;
                int p = y * GlobalSetting.LadderXCapacity + x;
                int p1 = p >> 2;
                int p2 = (p & 3) * 2;
                edata[st + p1] |= (byte)(1 << p2);
                // 写入一个梯形图元件
                Write(bvmodel);
            }
            foreach (VerticalLineViewModel vlvmodel in lnvmodel.GetVerticalLines())
            {
                int x = vlvmodel.X;
                int y = vlvmodel.Y;
                int p = y * GlobalSetting.LadderXCapacity + x;
                int p1 = p >> 2;
                int p2 = (p & 3) * 2 + 1;
                edata[st + p1] |= (byte)(1 << p2);
            }
            int sz = edata.Count() - szid - 2;
            edata[szid] = Int32_Low(sz);
            edata[szid + 1] = Int32_High(sz);
        }
        /// <summary>
        /// 写入一个梯形图元件
        /// </summary>
        /// <param name="bvmodel">元件</param>
        static private void Write(BaseViewModel bvmodel)
        {
            // 连线不必写入
            if (bvmodel is HorizontalLineViewModel
             || bvmodel is VerticalLineViewModel)
            {
                return;
            }
            // 坐标，指令ID
            edata.Add(Int32_Low(bvmodel.X));
            edata.Add(Int32_Low(bvmodel.Y));
            edata.Add(Int32_Low(LadderInstViewModelPrototype.GetOrderFromCatalog(bvmodel.GetCatalogID())));
            // 写入每个软元件参数
            for (int i = 0; i < bvmodel.Model.ParaCount; i++)
            {
                IValueModel ivmodel = bvmodel.Model.GetPara(i);
                // 在软元件表中找到，写入ID
                if (regids.ContainsKey(ivmodel.ValueString))
                {
                    int regid = regids[ivmodel.ValueString];
                    edata.Add(Int32_Low(regid));
                    edata.Add(Int32_High(regid));
                }
                // 写入0xFFFF表示空参数
                else
                {
                    edata.Add(0xFF);
                    edata.Add(0xFF);
                }
            }
        }
        /// <summary>
        /// 写入函数块内的代码文本
        /// </summary>
        /// <param name="fbvmodel">函数块</param>
        /// <param name="option">选项</param>
        static private void Write(FuncBlockViewModel fbvmodel, int option)
        {
            edata.Add(0xfd);
            Write(fbvmodel.ProgramName);
            if ((option & OPTION_COMMENT) == 0)
            {
                Write32(fbvmodel.Code);
                return;
            }
            List<FuncBlock_Comment> comments = new List<FuncBlock_Comment>();
            GetComments(fbvmodel.Model.Root, comments);
            comments.Sort((fb1, fb2) =>
            {
                return fb1.IndexStart.CompareTo(fb2.IndexStart);
            });
            int start = 0, end = 0;
            int szid = edata.Count();
            edata.Add(0x00);
            edata.Add(0x00);
            foreach (FuncBlock_Comment comment in comments)
            {
                end = comment.IndexStart;
                for (int i = start; i < end; i++)
                {
                    edata.Add((byte)fbvmodel.Code[i]);
                }
                start = comment.IndexEnd + 1;
            }
            end = fbvmodel.Code.Count();
            for (int i = start; i < end; i++)
            {
                edata.Add((byte)fbvmodel.Code[i]);
            }
            int sz = edata.Count() - szid - 2;
            edata[szid] = Int32_Low(sz);
            edata[szid + 1] = Int32_High(sz);
        }
        /// <summary>
        /// 写入Modbus表格
        /// </summary>
        /// <param name="mtvmodel">Modbus表格</param>
        /// <param name="option">选项</param>
        static private void Write(ModbusTableViewModel mtvmodel, int option)
        {
            edata.Add(0xfb);
            int szid = edata.Count();
            edata.Add(0x00);
            edata.Add(0x00);
            byte mtid = 0;
            foreach (ModbusTableModel mtmodel in mtvmodel.Models)
            {
                edata.Add(mtid++);
                Write(mtmodel.Name);
                if ((option & OPTION_COMMENT) != 0)
                    Write(mtmodel.Comment);
            }
            int sz = edata.Count() - szid - 2;
            edata[szid] = Int32_Low(sz);
            edata[szid] = Int32_High(sz);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mmoni"></param>
        /// <param name="option"></param>
        static private void Write(MainMonitor mmoni, int option)
        {
            edata.Add(0xf9);
            int szid = edata.Count();
            edata.Add(0x00);
            edata.Add(0x00);
            foreach (MonitorVariableTable mvtable in mmoni.tables)
            {
                Write(mvtable.TableName);
                edata.Add(Int32_Low(mvtable.Elements.Count()));
                foreach (ElementModel emodel in mvtable.Elements)
                {
                    string vname = emodel.ShowName;
                    if (regids.ContainsKey(vname))
                    {
                        int id = regids[vname];
                        edata.Add(Int32_Low(id));
                        edata.Add(Int32_High(id));
                    }
                    else
                    {
                        edata.Add(0x00);
                        edata.Add(0x00);
                    }
                }
            }
            int sz = edata.Count() - szid - 2;
            edata[szid] = Int32_Low(sz);
            edata[szid + 1] = Int32_High(sz);
        }
        /// <summary>
        /// 写入工程树
        /// </summary>
        /// <param name="ptview">工程资源管理器</param>
        /// <param name="option">选项</param>
        static private void Write(ProjectTreeView ptview, int option)
        {
            XElement xele = new XElement("ProjectTreeView");
            ptview.Save(xele);
            string xtext = xele.ToString();
            edata.Add(0xfa);
            Write32(xtext);
        }
        /// <summary>
        /// 写入一段字符串（长度保证不超过256）
        /// </summary>
        /// <param name="text">字符串</param>
        static private void Write(string text)
        {
            edata.Add(Int32_Low(text.Count()));
            //edata.Add(Int32_High(text.Count()));
            for (int i = 0; i < text.Count(); i++)
            {
                edata.Add((byte)(text[i]));
            }
        }
        /// <summary>
        /// 写入一段字符串（长度保证不超过65536）
        /// </summary>
        /// <param name="text">字符串</param>
        static private void Write32(string text)
        {
            edata.Add(Int32_Low(text.Count()));
            edata.Add(Int32_High(text.Count()));
            for (int i = 0; i < text.Count(); i++)
            {
                edata.Add((byte)(text[i]));
            }
        }
        /// <summary>
        /// 写入一个32位整数的低位
        /// </summary>
        /// <param name="i">整数</param>
        static private byte Int32_Low(int i)
        {
            return (byte)(i & 0xff);
        }
        /// <summary>
        /// 写入一个32位整数的高位
        /// </summary>
        /// <param name="i">整数</param>
        static private byte Int32_High(int i)
        {
            return (byte)((i >> 8) & 0xff);
        }
        /// <summary>
        /// 获得一个梯形图程序中所有已经使用的软元件
        /// </summary>
        /// <param name="ldvmodel">梯形图程序</param>
        static private void GetRegisters(LadderDiagramViewModel ldvmodel)
        {
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                foreach (BaseViewModel bvmodel in lnvmodel.GetElements())
                {
                    if (bvmodel.Model == null) continue;
                    for (int i = 0; i < bvmodel.Model.ParaCount; i++)
                    {
                        IValueModel ivmodel = bvmodel.Model.GetPara(i);
                        string ivname = ivmodel.ValueString;
                        if (!regids.ContainsKey(ivname))
                        {
                            regs.Add(ivmodel);
                            regids.Add(ivname, regids.Count());
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取一个监视表格内所有被监视的软元件
        /// </summary>
        /// <param name="table">监视表格</param>
        static private void GetRegisters(MonitorVariableTable table)
        {
            foreach (ElementModel emodel in table.Elements)
            {
                string ivname = emodel.ShowName;
                if (!regids.ContainsKey(ivname))
                {
                    IValueModel ivmodel = null;
                    switch (emodel.ShowType)
                    {
                        case "BOOL":
                            ivmodel = ValueParser.ParseBitValue(ivname);
                            break;
                        case "WORD": case "UWORD": case "BCD":
                            ivmodel = ValueParser.ParseWordValue(ivname);
                            break;
                        case "DWORD": case "UDWORD":
                            ivmodel = ValueParser.ParseDoubleWordValue(ivname);
                            break;
                        case "FLOAT":
                            ivmodel = ValueParser.ParseFloatValue(ivname);
                            break;
                    }
                    regs.Add(ivmodel);
                    regids.Add(ivname, regids.Count());
                }
            }
        }
        /// <summary>
        /// 获得一个函数块程序中所有的注释块
        /// </summary>
        /// <param name="ldvmodel">梯形图程序</param>
        static private void GetComments(FuncBlock fblock, List<FuncBlock_Comment> comments)
        {
            if (fblock is FuncBlock_Comment)
            {
                comments.Add((FuncBlock_Comment)fblock);
            }
            foreach (FuncBlock child in fblock.Childrens)
            {
                GetComments(child, comments);
            }
        }

        #endregion

        #region Download Communication

        static private ICommunicationManager commanager;

        static public bool Download(ICommunicationManager _commanager)
        {
            commanager = _commanager;
            DownloadFBCommand dFBCmd = new DownloadFBCommand();
            DownloadFCCommand dFCCmd = new DownloadFCCommand();
            Download80Command d80Cmd = null;
            Download81Command d81Cmd = new Download81Command();
            if (!Handle(dFBCmd)) return false;
            while (!Handle(dFCCmd)) ;
            //if (!Handle(dFCCmd)) return false;
            byte[] data = odata.Concat(edata).ToArray();
            byte[] pack = new byte[1024];
            int len = data.Length / 1024;
            int rem = data.Length % 1024;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < 1024; j++)
                    pack[j] = data[i * 1024 + j];
                d80Cmd = new Download80Command(i, pack);
                if (!Handle(d80Cmd)) return false;
            }
            if (rem > 0)
            {
                pack = new byte[rem];
                for (int j = 0; j < rem; j++)
                    pack[j] = data[len * 1024 + j];
                d80Cmd = new Download80Command(len, pack);
                if (!Handle(d80Cmd)) return false;
            }
            if (!Handle(d81Cmd)) return false;
            return true;
        }

        static private bool Handle(ICommunicationCommand cmd, int waittime = 10)
        {
            bool hassend = false;
            bool hasrecv = false;
            int sendtime = 0;
            int recvtime = 0;
            while (sendtime < 5)
            {
                if (commanager.Write(cmd) == 0)
                {
                    hassend = true;
                    break;
                }
                sendtime++;
            }
            if (!hassend) return false;
            Thread.Sleep(waittime);
            if (cmd.RecvDataLen == 0) return true;
            while (true)
            {
                if (commanager.Read(cmd) == 0)
                {
                    hasrecv = true;
                    break;
                }
                recvtime++;
            }
            return hasrecv && cmd.IsComplete && cmd.IsSuccess;
        }

        #endregion
    }
}

using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Namespace : SamSoarII.AppMain.Project
/// ClassName : DownloadHelper
/// Version   : 1.0
/// Date      : 2017/6/14
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 将从PLC传入的数据进行解析，生成新的工程
/// </remarks>

namespace SamSoarII.AppMain.Project
{
    /// <summary> 寄存器类型 </summary>
    public enum UploadRegisterType
    {
        NULL = 0x00,
        K, K32, H, H32, F, STR,
        X, Y, S, M, C, T,
        D, TV, CV, AI, AO, V, Z,
        CV32
    }

    /// <summary> 上载的帮助类 </summary>
    public class UploadHelper
    {
        /// <summary> 选项：是否包含程序 </summary>
        public const int OPTION_PROGRAM = 0x01;
        /// <summary> 选项：是否包含注释 </summary>
        public const int OPTION_COMMENT = 0x02;
        /// <summary> 选项：是否包含初始化 </summary>
        public const int OPTION_INITIALIZE = 0x04;
        /// <summary> 选项：是否包含设置 </summary>
        public const int OPTION_SETTING = 0x08;

        /// <summary> 数据选项 </summary>
        static private int option;
        /// <summary> 未压缩的原数据，通常需要PLC去识别分析 </summary>
        static private IList<byte> odata;
        /// <summary> 原数据读取到的位置 </summary>
        static private int oid;
        /// <summary> 压缩为rar包的数据 </summary>
        static private IList<byte> edata;
        /// <summary> 压缩数据读取到的位置 </summary>
        static private int eid;
        /// <summary> 软元件表 </summary>
        static private List<IValueModel> regs
            = new List<IValueModel>();
        /// <summary> 软元件ID表 </summary>
        //static private Dictionary<string, int> regids
        //    = new Dictionary<string, int>();

        static public void Read(ref ProjectModel pmodel)
        {
            string currentpath = Environment.CurrentDirectory;
            string packfile = String.Format(@"{0:s}\downe.rar", currentpath);
            Process cmd = null;
            cmd = new Process();
            cmd.StartInfo.FileName
                = String.Format(@"{0:s}\rar\Unrar", currentpath);
            cmd.StartInfo.Arguments
                = String.Format("p \"{0:s}\"",
                    packfile);
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();
            cmd.WaitForExit();
            string edatas = cmd.StandardOutput.ReadToEnd();
            Read(pmodel, new byte[0], edatas.Select((c) => { return (byte)(c); }).ToArray());
        }
        
        static private void Read(ProjectModel pmodel, IList<byte> _odata, IList<byte> _edata)
        {
            odata = _odata;
            oid = 0;
            edata = _edata;
            eid = 0;
            regs.Clear();
            try
            {
                option = edata[eid++];
                while (eid < edata.Count)
                {
                    int head = edata[eid++];
                    switch (head)
                    {
                        case 0xff:
                            LadderDiagramViewModel ldvmodel = new LadderDiagramViewModel(String.Empty, pmodel);
                            Read(ldvmodel);
                            if (ldvmodel.ProgramName.Equals("main"))
                                pmodel.MainRoutine = ldvmodel;
                            else
                                pmodel.Add(ldvmodel);
                            break;
                        case 0xfd:
                            FuncBlockViewModel fbvmodel = new FuncBlockViewModel(String.Empty, pmodel);
                            Read(fbvmodel);
                            pmodel.Add(fbvmodel);
                            break;
                        default:
                            throw new FormatException(
                                String.Format("非法头标志符0x{0x2X}", head));
                    }
                }
                ReadRegisters();
                pmodel.MainRoutine = new LadderDiagramViewModel("main", pmodel);
                
            }
            catch (OutOfMemoryException)
            {

            }
            catch (FormatException)
            {

            }
        }
        unsafe static private void ReadRegisters()
        {
            int sz = ReadE32();
            for (int i = 0; i < sz; i++)
            {
                UploadRegisterType type = (UploadRegisterType)(edata[eid++]);
                IValueModel ivmodel = null;
                switch (type)
                {
                    case UploadRegisterType.K:
                        ivmodel = new KWordValue(ReadE16());
                        break;
                    case UploadRegisterType.K32:
                        ivmodel = new KDoubleWordValue(ReadE32());
                        break;
                    case UploadRegisterType.H:
                        ivmodel = new HWordValue(ReadE16());
                        break;
                    case UploadRegisterType.H32:
                        ivmodel = new HDoubleWordValue(ReadE32());
                        break;
                    case UploadRegisterType.F:
                        int value = ReadE32();
                        ivmodel = new KFloatValue(*(float*)(&value));
                        break;
                    case UploadRegisterType.STR:
                        ivmodel = new StringValue(ReadTextE8());
                        break;
                }
                if (ivmodel == null)
                {
                    int index = ReadE16();
                    UploadRegisterType itype = (UploadRegisterType)(edata[eid++]);
                    int iindex = edata[eid++];
                    WordValue intra = null;
                    switch (itype)
                    {
                        case UploadRegisterType.V:
                            intra = new VWordValue((uint)iindex);
                            break;
                        case UploadRegisterType.Z:
                            intra = new ZWordValue((uint)iindex);
                            break;
                        default:
                            intra = WordValue.Null;
                            break;
                    }
                    switch (type)
                    {
                        case UploadRegisterType.X:
                            ivmodel = new XBitValue((uint)index, intra);
                            break;
                        case UploadRegisterType.Y:
                            ivmodel = new YBitValue((uint)index, intra);
                            break;
                        case UploadRegisterType.S:
                            ivmodel = new SBitValue((uint)index, intra);
                            break;
                        case UploadRegisterType.M:
                            ivmodel = new MBitValue((uint)index, intra);
                            break;
                        case UploadRegisterType.C:
                            ivmodel = new CBitValue((uint)index, intra);
                            break;
                        case UploadRegisterType.T:
                            ivmodel = new TBitValue((uint)index, intra);
                            break;
                        case UploadRegisterType.D:
                            ivmodel = new DWordValue((uint)index, intra);
                            break;
                        case UploadRegisterType.AI:
                            ivmodel = new AIWordValue((uint)index, intra);
                            break;
                        case UploadRegisterType.AO:
                            ivmodel = new AOWordValue((uint)index, intra);
                            break;
                        case UploadRegisterType.CV:
                            ivmodel = new CVWordValue((uint)index, intra);
                            break;
                        case UploadRegisterType.TV:
                            ivmodel = new TVWordValue((uint)index, intra);
                            break;
                        case UploadRegisterType.CV32:
                            ivmodel = new CV32DoubleWordValue((uint)index, intra);
                            break;
                        case UploadRegisterType.V:
                            ivmodel = new VWordValue((uint)index);
                            break;
                        case UploadRegisterType.Z:
                            ivmodel = new ZWordValue((uint)index);
                            break;
                    }
                }
                regs.Add(ivmodel);
                if ((option & OPTION_COMMENT) != 0)
                {
                    string comment = ReadTextE8();
                    ValueCommentManager.UpdateComment(ivmodel, comment);
                }
            }     
        }
        static private void Read(LadderDiagramViewModel ldvmodel)
        {
            int sz = ReadE32();
            sz += eid;
            ldvmodel.ProgramName = ReadTextE8();
            while (eid < sz)
            {
                int head = edata[eid++];
                switch (head)
                {
                    case 0xfe:
                        LadderNetworkViewModel lnvmodel
                            = new LadderNetworkViewModel(ldvmodel, ldvmodel.NetworkCount);
                        ldvmodel.AppendNetwork(lnvmodel);
                        Read(lnvmodel);
                        break;
                    default:
                        throw new FormatException(
                            String.Format("非法头标志符0x{0x2X}", head));
                }
            }
        }
        static private void Read(LadderNetworkViewModel lnvmodel)
        {
            int sz = ReadE32();
            sz += eid;
            if ((option & OPTION_COMMENT) != 0)
            {
                lnvmodel.NetworkBrief = ReadTextE8();
            }
            lnvmodel.RowCount = edata[eid++];
            int le = (lnvmodel.RowCount * GlobalSetting.LadderXCapacity) >> 2;
            for (int y = 0; y < lnvmodel.RowCount; y++)
                for (int x = 0; x < GlobalSetting.LadderXCapacity; x++)
                {
                    int p = y * GlobalSetting.LadderXCapacity + x;
                    int p1 = p >> 2;
                    int p2 = (p & 3) * 2;
                    if ((edata[eid + p1] & (1 << p2)) != 0)
                    {
                        HorizontalLineViewModel hlvmodel = new HorizontalLineViewModel();
                        hlvmodel.X = x; hlvmodel.Y = y;
                        lnvmodel.ReplaceElement(hlvmodel);
                    }
                    if ((edata[eid + p1] & (1 << (p2 + 1))) != 0)
                    {
                        VerticalLineViewModel vlvmodel = new VerticalLineViewModel();
                        vlvmodel.X = x; vlvmodel.Y = y;
                        lnvmodel.ReplaceVerticalLine(vlvmodel);
                    }
                }
            while (eid < sz)
            {
                BaseViewModel bvmodel = null;
                int x = edata[eid++];
                int y = edata[eid++];
                int catalog = LadderInstViewModelPrototype.GetCatalogFromOrder(edata[eid++]);
                bvmodel = LadderInstViewModelPrototype.Clone(catalog);
                bvmodel.X = x;
                bvmodel.Y = y;
                for (int i = 0; i < bvmodel.Model.ParaCount; i++)
                {
                    IValueModel ivmold = bvmodel.Model.GetPara(i);
                    IValueModel ivmnew = regs[ReadE16()];
                    if (ivmnew is DWordValue || ivmnew is DDoubleWordValue || ivmnew is DFloatValue)
                    {
                        if (ivmold is WordValue)
                            ivmnew = new DWordValue(ivmnew.Index, ivmnew.Offset);
                        if (ivmold is DoubleWordValue)
                            ivmnew = new DDoubleWordValue(ivmnew.Index, ivmnew.Offset);
                        if (ivmold is FloatValue)
                            ivmnew = new DFloatValue(ivmnew.Index, ivmnew.Offset);
                    }
                    bvmodel.Model.SetPara(i, ivmnew);
                }
                lnvmodel.ReplaceElement(bvmodel);
            }
        }
        static private void Read(FuncBlockViewModel fbvmodel)
        {
            fbvmodel.ProgramName = ReadTextE8();
            fbvmodel.Code = ReadTextE16();
        }
        static private short ReadE16()
        {
            short ret = edata[eid++];
            ret |= (short)(((int)edata[eid++]) << 8);
            return ret;
        }
        static private int ReadE32()
        {
            int ret = 0, bit = 0;
            for (int i = 0; i < 4; i++)
            {
                ret |= (((int)edata[eid++]) << bit);
                bit += 8;
            }
            return ret;
        }
        static private string ReadTextE8()
        {
            return ReadTextE(edata[eid++]);
        }
        static private string ReadTextE16()
        {
            return ReadTextE(ReadE16());
        }
        static private string ReadTextE(int sz)
        {
            char[] str = new char[sz];
            for (int i = 0; i < sz; i++)
            {
                str[i] = (char)edata[eid++];
            }
            return new string(str);
        }

    }
}

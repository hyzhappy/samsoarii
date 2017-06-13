using SamSoarII.Extend.FuncBlockModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.Project
{
    public enum DownloadRegisterType
    {
        NULL = 0x00,
        K, H, F, STR,
        X, Y, S, M, C, T,
        D, TV, CV, AI, AO, V, Z,
        CV32
    }

    public class DownloadHelper
    {
        public const int OPTION_PROGRAM = 0x01;
        public const int OPTION_COMMENT = 0x02;
        public const int OPTION_INITIALIZE = 0x04;
        public const int OPTION_SETTING = 0x08;

        static private List<byte> odata 
            = new List<byte>();
        static private List<byte> edata 
            = new List<byte>();
        static private List<IValueModel> regs
            = new List<IValueModel>();
        static private Dictionary<string, int> regids 
            = new Dictionary<string, int>(); 

        public void Write(ProjectModel pmodel, string filename, int option)
        {
            odata.Clear();
            edata.Clear();
            regs.Clear();
            regids.Clear();
            edata.Add(Int32_Low(option));
            GetRegisters(pmodel.MainRoutine);
            foreach (LadderDiagramViewModel ldvmodel in pmodel.SubRoutines)
            {
                GetRegisters(ldvmodel);
            }
            WriteRegisters(option);
            Write(pmodel.MainRoutine, option);
            foreach (LadderDiagramViewModel ldvmodel in pmodel.SubRoutines)
            {
                Write(ldvmodel, option);
            }
            foreach (FuncBlockViewModel fbvmdoel in pmodel.FuncBlocks)
            {
                Write(fbvmdoel, option);
            }
            Write(pmodel.MTVModel, option);
        }
        unsafe private void WriteRegisters(int option)
        {
            edata.Add(0xfc);
            edata.Add(Int32_Low(regs.Count()));
            edata.Add(Int32_High(regs.Count()));
            foreach (IValueModel ivmodel in regs)
            {
                if (ivmodel is KWordValue)
                {
                    edata.Add((byte)(DownloadRegisterType.K));
                    int value = int.Parse(((KWordValue)ivmodel).ValueString.Substring(1));
                    for (int i = 0; i < 4; i++)
                    {
                        edata.Add((byte)(value & 0xff));
                        value >>= 8;
                    }
                }
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
                else if (ivmodel is HWordValue)
                {
                    edata.Add((byte)(DownloadRegisterType.H));
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
                    continue;
                }
                else if (ivmodel is StringValue)
                {
                    edata.Add((byte)(DownloadRegisterType.STR));
                    Write(ivmodel.ValueString);
                }
                else
                {
                    IValueModel _ivmodel = ivmodel;
                    if (_ivmodel is ArgumentValue)
                        _ivmodel = ((ArgumentValue)_ivmodel).Argument;
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
                    edata.Add(Int32_Low((int)(_ivmodel.Index)));
                    edata.Add(Int32_High((int)(_ivmodel.Index)));
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
                    else
                    {
                        edata.Add(0x00);
                        edata.Add(0x00);
                    }
                }
            }
        }
        private void Write(LadderDiagramViewModel ldvmodel, int option)
        {
            if ((option & OPTION_PROGRAM) == 0) return;
            edata.Add(0xff);
            int szid = edata.Count();
            edata.Add(0x00);
            edata.Add(0x00);
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                Write(lnvmodel, option);
            }
            int sz = edata.Count() - szid - 2;
            edata[szid] = Int32_Low(sz);
            edata[szid+1] = Int32_High(sz);
        }
        private void Write(LadderNetworkViewModel lnvmodel, int option)
        {
            edata.Add(0xfe);
            int szid = edata.Count();
            edata.Add(0x00);
            edata.Add(0x00);
            if ((option & OPTION_COMMENT) != 0)
            {
                Write(lnvmodel.NetworkBrief);
            }
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
        }
        private void Write(BaseViewModel bvmodel)
        {
            if (bvmodel is HorizontalLineViewModel
             || bvmodel is VerticalLineViewModel)
            {
                return;
            }
            edata.Add(Int32_Low(bvmodel.X));
            edata.Add(Int32_Low(bvmodel.Y));
            edata.Add(Int32_Low(bvmodel.GetCatalogID()));
            for (int i = 0; i < bvmodel.Model.ParaCount; i++)
            {
                IValueModel ivmodel = bvmodel.Model.GetPara(i);
                if (regids.ContainsKey(ivmodel.ValueString))
                {
                    int regid = regids[ivmodel.ValueString];
                    edata.Add(Int32_Low(regid));
                    edata.Add(Int32_High(regid));
                }
                else
                {
                    edata.Add(0xFF);
                    edata.Add(0xFF);
                }
            }
        }
        private void Write(FuncBlockViewModel fbvmodel, int option)
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
        private void Write(ModbusTableViewModel mtvmodel, int option)
        {
            if ((option & OPTION_COMMENT) == 0)
            {
                return;
            }
            edata.Add(0xfb);
            int szid = edata.Count();
            edata.Add(0x00);
            edata.Add(0x00);
            byte mtid = 0;
            foreach (ModbusTableModel mtmodel in mtvmodel.Models)
            {
                edata.Add(mtid++); 
                Write(mtmodel.Comment);
            }
            int sz = edata.Count() - szid - 2;
            edata[szid] = Int32_Low(sz);
            edata[szid] = Int32_High(sz);
        }
        private void Write(string text)
        {
            edata.Add(Int32_Low(text.Count()));
            //edata.Add(Int32_High(text.Count()));
            for (int i = 0; i < text.Count(); i++)
            {
                edata.Add((byte)(text[i]));
            }
        }
        private void Write32(string text)
        {
            edata.Add(Int32_Low(text.Count()));
            edata.Add(Int32_High(text.Count()));
            for (int i = 0; i < text.Count(); i++)
            {
                edata.Add((byte)(text[i]));
            }
        }

        private byte Int32_Low(int i)
        {
            return (byte)(i & 0xff);
        }
        private byte Int32_High(int i)
        {
            return (byte)((i >> 8) & 0xff);
        }
        private void GetRegisters(LadderDiagramViewModel ldvmodel)
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
        private void GetComments(FuncBlock fblock, List<FuncBlock_Comment> comments)
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
    }
}

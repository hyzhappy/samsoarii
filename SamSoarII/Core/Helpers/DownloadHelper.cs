using SamSoarII.Core.Communication;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SamSoarII.Core.Models;
using SamSoarII.Global;
using System.Threading;

namespace SamSoarII.Core.Helpers
{
    public enum DownloadError
    {
        Cancel,
        None,
        CommuicationFailed,
        DownloadFailed
    }

    public static class DownloadHelper
    {
        #region option
        private static int downloadoption;
        public static int DownloadOption
        {
            get { return downloadoption; }
            set { downloadoption = value; }
        }
        public static bool IsDownloadProgram
        {
            get { return (downloadoption & CommunicationDataDefine.OPTION_PROGRAM) != 0; }
        }
        public static bool IsDownloadComment
        {
            get { return (downloadoption & CommunicationDataDefine.OPTION_COMMENT) != 0; }
        }
        public static bool IsDownloadInitialize
        {
            get { return (downloadoption & CommunicationDataDefine.OPTION_INITIALIZE) != 0; }
        }
        public static bool IsDownloadSetting
        {
            get { return (downloadoption & CommunicationDataDefine.OPTION_SETTING) != 0; }
        }
        #endregion

        #region data
        /// <summary> 程序 </summary>
        //static private List<byte> dtBinary;
        /// <summary> 条形码 </summary>
        static private List<byte> dtIcon;
        /// <summary> 工程 </summary>
        //static private List<byte> dtProject;
        /// <summary> 注释 </summary>
        //static private List<byte> dtComment;
        /// <summary> 元件表 </summary>
        //static private List<byte> dtEleList;
        /// <summary> 配置 </summary>
        static private List<byte> dtConfig;
        /// <summary> Modbus表 </summary>
        static private List<byte> dtModbus;
        /// <summary> Table表 </summary>
        static private List<byte> dtTable;
        /// <summary> Block表 </summary>
        static private List<byte> dtBlock;
        #endregion

        #region InitializeData

        static private void InitializeData(ProjectModel project)
        {
            //工程，注释，软元件等用于上载的信息直接压缩xml
            
            // 初始化
            dtIcon = new List<byte>();
            dtConfig = new List<byte>();
            dtModbus = new List<byte>();
            dtTable = new List<byte>();
            dtBlock = new List<byte>();
            
            // 条形码

            // 配置
            WriteConfig(project.PARAProj);
            // Modbus
            Write(project.Modbus);

        }

        #region Base

        static private void Write(List<byte> data, bool value)
        {
            data.Add((byte)(value ? 1 : 0));
        }

        static private void Write(List<byte> data, short value)
        {
            for (int i = 0; i < 2; i++)
            {
                data.Add((byte)(value & 0xff));
                value >>= 8;
            }
        }

        static private void Write(List<byte> data, ushort value)
        {
            for (int i = 0; i < 2; i++)
            {
                data.Add((byte)(value & 0xff));
                value >>= 8;
            }
        }

        static private void Write(List<byte> data, int value)
        {
            for (int i = 0; i < 4; i++)
            {
                data.Add((byte)(value & 0xff));
                value >>= 8;
            }
        }

        static private void Write(List<byte> data, Int64 value)
        {
            for (int i = 0; i < 8; i++)
            {
                data.Add((byte)(value & 0xff));
                value >>= 8;
            }
        }

        static private void Write8(List<byte> data, string value)
        {
            data.Add((byte)(value.Length));
            for (int i = 0; i < value.Length; i++)
                data.Add((byte)value[i]);
        }

        static private void Write(List<byte> data, string value)
        {
            Write(data, value.Length);
            for (int i = 0; i < value.Length; i++)
                data.Add((byte)value[i]);
        }

        static private void Write(List<byte> data, ExpansionUnitModuleParams eumparams)
        {
            Write(data, 4);

            Write(data, eumparams.IP_Channel_CB_Enabled1);
            data.Add((byte)eumparams.IP_Mode_Index1);
            Write(data, (ushort)eumparams.IP_EndRange1);
            Write(data, (ushort)eumparams.IP_StartRange1);
            data.Add((byte)Math.Pow(2,eumparams.IP_SampleTime_Index1 + 2));
            Write(data, short.Parse(eumparams.SampleValue1));

            Write(data, eumparams.IP_Channel_CB_Enabled2);
            data.Add((byte)eumparams.IP_Mode_Index2);
            Write(data, (ushort)eumparams.IP_EndRange2);
            Write(data, (ushort)eumparams.IP_StartRange2);
            data.Add((byte)Math.Pow(2, eumparams.IP_SampleTime_Index2 + 2));
            Write(data, short.Parse(eumparams.SampleValue2));

            Write(data, eumparams.IP_Channel_CB_Enabled3);
            data.Add((byte)eumparams.IP_Mode_Index3);
            Write(data, (ushort)eumparams.IP_EndRange3);
            Write(data, (ushort)eumparams.IP_StartRange3);
            data.Add((byte)Math.Pow(2, eumparams.IP_SampleTime_Index3 + 2));
            Write(data, short.Parse(eumparams.SampleValue3));

            Write(data, eumparams.IP_Channel_CB_Enabled4);
            data.Add((byte)eumparams.IP_Mode_Index4);
            Write(data, (ushort)eumparams.IP_EndRange4);
            Write(data, (ushort)eumparams.IP_StartRange4);
            data.Add((byte)Math.Pow(2, eumparams.IP_SampleTime_Index4 + 2));
            Write(data, short.Parse(eumparams.SampleValue4));

            Write(data, 4);

            Write(data, eumparams.OP_Channel_CB_Enabled1);
            data.Add((byte)eumparams.OP_Mode_Index1);
            Write(data, (ushort)eumparams.OP_EndRange1);
            Write(data, (ushort)eumparams.OP_StartRange1);
            data.Add(4);
            Write(data, (short)1000);

            Write(data, eumparams.OP_Channel_CB_Enabled2);
            data.Add((byte)eumparams.OP_Mode_Index2);
            Write(data, (ushort)eumparams.OP_EndRange2);
            Write(data, (ushort)eumparams.OP_StartRange2);
            data.Add(4);
            Write(data, (short)1000);

            Write(data, eumparams.OP_Channel_CB_Enabled1);
            data.Add((byte)eumparams.OP_Mode_Index1);
            Write(data, (ushort)eumparams.OP_EndRange1);
            Write(data, (ushort)eumparams.OP_StartRange1);
            data.Add(4);
            Write(data, (short)1000);

            Write(data, eumparams.OP_Channel_CB_Enabled2);
            data.Add((byte)eumparams.OP_Mode_Index2);
            Write(data, (ushort)eumparams.OP_EndRange2);
            Write(data, (ushort)eumparams.OP_StartRange2);
            data.Add(4);
            Write(data, (short)1000);
        }
        #endregion
        
        //工程，注释，软元件等用于上载的信息直接压缩xml
        #region Initialize Project

        //static private void Write(ValueManager ValueManager)
        //{
        //    int id = 0;
        //    foreach (ValueInfo vinfo in ValueManager)
        //        foreach (ValueStore vstore in vinfo.Stores)
        //            vstore.ID = ++id;
        //    Write(dtProject, (short)id);
        //    foreach (ValueInfo vinfo in ValueManager)
        //        foreach (ValueStore vstore in vinfo.Stores)
        //            Write(vstore, ValueManager);
        //    foreach (ValueStore vstore in ValueManager.EmptyInfo.Stores)
        //        vstore.ID = ++id;
        //    Write(dtProject, (short)id);
        //    foreach (ValueStore vstore in ValueManager.EmptyInfo.Stores)
        //        Write(vstore, ValueManager);

        //}

        //static private void Write(ValueStore vstore, ValueManager ValueManager)
        //{
        //    if (vstore.Parent == ValueManager.EmptyInfo)
        //    {
        //        switch (vstore.Type)
        //        {
        //            case ValueModel.Types.BOOL:
        //            case ValueModel.Types.WORD:
        //            case ValueModel.Types.DWORD:
        //                Write(dtProject, int.Parse(vstore.Value.ToString()));
        //                break;
        //            case ValueModel.Types.FLOAT:
        //                Write(dtProject, (int)(ValueConverter.FloatToUInt((float)(vstore.Value))));
        //                break;
        //            default:
        //                Write(dtProject, (int)0);
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        Write(dtProject, (short)(ValueManager.IndexOf(vstore.Parent)));
        //        int i = 0;
        //        i |= ((int)(vstore.Type) & 0x0f);
        //        i <<= 4;
        //        i |= ((int)(vstore.Flag - 1) & 0x1f);
        //        i <<= 5;
        //        switch (vstore.Intra)
        //        {
        //            case ValueModel.Bases.V: i |= 0x01; break;
        //            case ValueModel.Bases.Z: i |= 0x02; break;
        //        }
        //        i <<= 2;
        //        i |= (vstore.IntraOffset & 0x07);
        //        Write(dtProject, (short)i);
        //    }
        //}

        //static private void Write(LadderDiagramModel ldmodel)
        //{
        //    Write(dtProject, ldmodel.IsMainLadder);
        //    Write(dtProject, ldmodel.IsExpand);
        //    Write(dtProject, ldmodel.Name);
        //    Write(dtProject, ldmodel.Brief);
        //    Write(dtProject, ldmodel.NetworkCount);
        //    foreach (LadderNetworkModel lnmodel in ldmodel.Children)
        //        Write(lnmodel);
        //}

        //static private void Write(LadderNetworkModel lnmodel)
        //{
        //    Write(dtProject, lnmodel.IsExpand);
        //    Write(dtProject, lnmodel.IsMasked);
        //    Write(dtProject, lnmodel.Description);
        //    Write(dtProject, lnmodel.Brief);
        //    Write(dtProject, lnmodel.RowCount);
        //    for (int y = 0; y < lnmodel.RowCount; y++)
        //    {
        //        int link = 0;
        //        for (int x = 0; x < GlobalSetting.LadderXCapacity; x++)
        //        {
        //            link |= (lnmodel.Children[x, y] != null ? 1 : 0) << (x + x);
        //            link |= (lnmodel.VLines[x, y] != null ? 1 : 0) << (x + x + 1);
        //        }
        //        Write(dtProject, link);
        //    }
        //    Write(dtProject, lnmodel.Children.Count());
        //    foreach (LadderUnitModel unit in lnmodel.Children)
        //        Write(unit);
        //}

        //static private void Write(LadderUnitModel unit)
        //{
        //    Write(dtProject, (short)(unit.X | (unit.Y << 4)));
        //    dtProject.Add((byte)unit.Type);
        //    if (unit.Type == LadderUnitModel.Types.CALLM)
        //    {
        //        FuncModel func = ltFuncs.Where(f => f.Name.Equals(unit.Children[0].Text)).First();
        //        Write(dtProject, (short)(ltFuncs.IndexOf(func)));
        //        Write(dtProject, (byte)(unit.Children.Count - 1));
        //        for (int i = 1; i < unit.Children.Count; i++)
        //            Write(dtProject, (short)(unit.Children[i].Store.ID));
        //    }
        //    else
        //    {
        //        for (int i = 0; i < unit.Children.Count; i++)
        //            if (unit.Children[i].Type == ValueModel.Types.STRING)
        //            {
        //                if (unit.Type == LadderUnitModel.Types.CALLM && i == 0
        //                 || unit.Type == LadderUnitModel.Types.ATCH && i == 1)
        //                {
        //                    LadderDiagramModel diagram = unit.Project.Diagrams.Where(d => d.Name.Equals(unit.Children[i].Text)).First();
        //                    Write(dtProject, (short)(unit.Project.Diagrams.IndexOf(diagram)));
        //                }
        //                if (unit.Type == LadderUnitModel.Types.MBUS && i == 1)
        //                {
        //                    ModbusModel modbus = unit.Project.Modbus.Children.Where(m => m.Name.Equals(unit.Children[i].Text)).First();
        //                    Write(dtProject, (short)(unit.Project.Modbus.Children.IndexOf(modbus)));
        //                }
        //            }
        //            else
        //                Write(dtProject, (short)(unit.Children[i].Store.ID));
        //    }
        //}

        //static private void Write(FuncBlockModel fbmodel)
        //{
        //    Write(dtProject, fbmodel.IsLibrary);
        //    if (!fbmodel.IsLibrary)
        //    {
        //        Write(dtProject, fbmodel.Name);
        //        Write(dtProject, fbmodel.View != null ? fbmodel.View.Code : fbmodel.Code);
        //    }
        //}

        //static private void Write(MonitorTable mtable)
        //{
        //    Write(dtProject, mtable.Name);
        //    Write(dtProject, mtable.Children.Count);
        //    foreach (MonitorElement melement in mtable.Children) Write(melement);
        //}

        //static private void Write(MonitorElement melement)
        //{
        //    Write(dtProject, (short)(melement.Store.ID));
        //}

        #endregion
        
        #region Initialize Config data
        
        static private void WriteConfig(ProjectPropertyParams pparams)
        {
            CommunicationInterfaceParams com232params = pparams.PARACom232;
            CommunicationInterfaceParams com485params = pparams.PARACom485;
            USBCommunicationParams usbparams = pparams.PARAUsb;
            PasswordParams pwparams = pparams.PARAPassword;
            FilterParams ftparams = pparams.PARAFilter;
            HoldingSectionParams hsparams = pparams.PARAHolding;
            AnalogQuantityParams aqparams = pparams.PARAAnalog;
            ExpansionModuleParams emparams = pparams.PARAExpansion;
            
            //空6个字节留作长度使用
            dtConfig.AddRange(new byte[6]);
            //com1
            dtConfig.Add((byte)com232params.BaudRateIndex);
            dtConfig.Add(0);
            dtConfig.Add((byte)com232params.StopBitIndex);
            dtConfig.Add((byte)com232params.CheckCodeIndex);
            //com2
            dtConfig.Add((byte)com485params.BaudRateIndex);
            dtConfig.Add(0);
            dtConfig.Add((byte)com485params.StopBitIndex);
            dtConfig.Add((byte)com485params.CheckCodeIndex);

            dtConfig.Add((byte)pparams.StationNumber);

            Write(dtConfig,(short)usbparams.Timeout);

            Write(dtConfig,pwparams.PWENUpload);
            Write8(dtConfig, pwparams.PWUpload);

            dtConfig.Add((byte)com232params.ComType);
            dtConfig.Add((byte)com485params.ComType);

            Write(dtConfig, pwparams.PWENDownload);
            Write8(dtConfig, pwparams.PWDownload);

            Write(dtConfig, pwparams.PWENMonitor);
            Write8(dtConfig, pwparams.PWMonitor);

            Write(dtConfig, ftparams.IsChecked);
            Write(dtConfig, (short)Math.Pow(2, ftparams.FilterTimeIndex + 1));

            Write(dtConfig, (short)hsparams.MStartAddr);
            Write(dtConfig, (short)hsparams.MLength);
            Write(dtConfig, (short)hsparams.DStartAddr);
            Write(dtConfig, (short)hsparams.DLength);
            Write(dtConfig, (short)hsparams.SStartAddr);
            Write(dtConfig, (short)hsparams.SLength);
            Write(dtConfig, (short)hsparams.CVStartAddr);
            Write(dtConfig, (short)hsparams.CVLength);

            Write(dtConfig, 4);

            Write(dtConfig, aqparams.IP_Channel_CB_Enabled1);
            dtConfig.Add((byte)aqparams.IP_Mode_Index1);
            Write(dtConfig, (ushort)aqparams.IP_EndRange1);
            Write(dtConfig, (ushort)aqparams.IP_StartRange1);
            dtConfig.Add((byte)aqparams.IP_SampleTime_Index1);
            Write(dtConfig, short.Parse(aqparams.SampleValue1));

            Write(dtConfig, aqparams.IP_Channel_CB_Enabled2);
            dtConfig.Add((byte)aqparams.IP_Mode_Index2);
            Write(dtConfig, (ushort)aqparams.IP_EndRange2);
            Write(dtConfig, (ushort)aqparams.IP_StartRange2);
            dtConfig.Add((byte)aqparams.IP_SampleTime_Index2);
            Write(dtConfig, short.Parse(aqparams.SampleValue2));

            Write(dtConfig, aqparams.IP_Channel_CB_Enabled3);
            dtConfig.Add((byte)aqparams.IP_Mode_Index3);
            Write(dtConfig, (ushort)aqparams.IP_EndRange3);
            Write(dtConfig, (ushort)aqparams.IP_StartRange3);
            dtConfig.Add((byte)aqparams.IP_SampleTime_Index3);
            Write(dtConfig, short.Parse(aqparams.SampleValue3));

            Write(dtConfig, aqparams.IP_Channel_CB_Enabled4);
            dtConfig.Add((byte)aqparams.IP_Mode_Index4);
            Write(dtConfig, (ushort)aqparams.IP_EndRange4);
            Write(dtConfig, (ushort)aqparams.IP_StartRange4);
            dtConfig.Add((byte)aqparams.IP_SampleTime_Index4);
            Write(dtConfig, short.Parse(aqparams.SampleValue4));

            Write(dtConfig, 4);

            Write(dtConfig, aqparams.OP_Channel_CB_Enabled1);
            Write(dtConfig, (ushort)aqparams.OP_EndRange1);
            Write(dtConfig, (ushort)aqparams.OP_StartRange1);
            dtConfig.Add((byte)aqparams.OP_Mode_Index1);

            Write(dtConfig, aqparams.OP_Channel_CB_Enabled2);
            Write(dtConfig, (ushort)aqparams.OP_EndRange2);
            Write(dtConfig, (ushort)aqparams.OP_StartRange2);
            dtConfig.Add((byte)aqparams.OP_Mode_Index2);

            Write(dtConfig, aqparams.OP_Channel_CB_Enabled3);
            Write(dtConfig, (ushort)aqparams.OP_EndRange3);
            Write(dtConfig, (ushort)aqparams.OP_StartRange3);
            dtConfig.Add((byte)aqparams.OP_Mode_Index3);

            Write(dtConfig, aqparams.OP_Channel_CB_Enabled4);
            Write(dtConfig, (ushort)aqparams.OP_EndRange4);
            Write(dtConfig, (ushort)aqparams.OP_StartRange4);
            dtConfig.Add((byte)aqparams.OP_Mode_Index4);

            Write(dtConfig, emparams.UseExpansionModule);

            Write(dtConfig, emparams.ExpansionUnitParams[0].UseModule);
            dtConfig.Add((byte)emparams.ExpansionUnitParams[0].ModuleTypeIndex);
            Write(dtConfig, emparams.ExpansionUnitParams[1].UseModule);
            dtConfig.Add((byte)emparams.ExpansionUnitParams[1].ModuleTypeIndex);
            Write(dtConfig, emparams.ExpansionUnitParams[2].UseModule);
            dtConfig.Add((byte)emparams.ExpansionUnitParams[2].ModuleTypeIndex);
            Write(dtConfig, emparams.ExpansionUnitParams[3].UseModule);
            dtConfig.Add((byte)emparams.ExpansionUnitParams[3].ModuleTypeIndex);
            Write(dtConfig, emparams.ExpansionUnitParams[4].UseModule);
            dtConfig.Add((byte)emparams.ExpansionUnitParams[4].ModuleTypeIndex);
            Write(dtConfig, emparams.ExpansionUnitParams[5].UseModule);
            dtConfig.Add((byte)emparams.ExpansionUnitParams[5].ModuleTypeIndex);
            Write(dtConfig, emparams.ExpansionUnitParams[6].UseModule);
            dtConfig.Add((byte)emparams.ExpansionUnitParams[6].ModuleTypeIndex);
            Write(dtConfig, emparams.ExpansionUnitParams[7].UseModule);
            dtConfig.Add((byte)emparams.ExpansionUnitParams[7].ModuleTypeIndex);

            dtConfig.Add((byte)com232params.DataBitIndex);
            dtConfig.Add((byte)com485params.DataBitIndex);

            Write(dtConfig, (short)(hsparams.NotClear ? 1 : 0));

            Write(dtConfig, (short)com232params.Timeout);
            Write(dtConfig, (short)com485params.Timeout);

            dtConfig.Add(0);
            dtConfig.Add(0);
            dtConfig.Add(0);
            Write(dtConfig, ValueConverter.DoubleToInt64(0.0));
            Write(dtConfig, (short)emparams.ExpansionUnitParams[0].FilterTime_Index);

            dtConfig.Add(0);
            dtConfig.Add(0);
            dtConfig.Add(0);
            Write(dtConfig, ValueConverter.DoubleToInt64(0.0));
            Write(dtConfig, (short)emparams.ExpansionUnitParams[1].FilterTime_Index);

            dtConfig.Add(0);
            dtConfig.Add(0);
            dtConfig.Add(0);
            Write(dtConfig, ValueConverter.DoubleToInt64(0.0));
            Write(dtConfig, (short)emparams.ExpansionUnitParams[2].FilterTime_Index);

            dtConfig.Add(0);
            dtConfig.Add(0);
            dtConfig.Add(0);
            Write(dtConfig, ValueConverter.DoubleToInt64(0.0));
            Write(dtConfig, (short)emparams.ExpansionUnitParams[3].FilterTime_Index);

            dtConfig.Add(0);
            dtConfig.Add(0);
            dtConfig.Add(0);
            Write(dtConfig, ValueConverter.DoubleToInt64(0.0));
            Write(dtConfig, (short)emparams.ExpansionUnitParams[4].FilterTime_Index);

            dtConfig.Add(0);
            dtConfig.Add(0);
            dtConfig.Add(0);
            Write(dtConfig, ValueConverter.DoubleToInt64(0.0));
            Write(dtConfig, (short)emparams.ExpansionUnitParams[5].FilterTime_Index);

            dtConfig.Add(0);
            dtConfig.Add(0);
            dtConfig.Add(0);
            Write(dtConfig, ValueConverter.DoubleToInt64(0.0));
            Write(dtConfig, (short)emparams.ExpansionUnitParams[6].FilterTime_Index);

            dtConfig.Add(0);
            dtConfig.Add(0);
            dtConfig.Add(0);
            Write(dtConfig, ValueConverter.DoubleToInt64(0.0));
            Write(dtConfig, (short)emparams.ExpansionUnitParams[7].FilterTime_Index);

            Write(dtConfig, 4);

            Write(dtConfig, aqparams.IP_Channel_CB_Enabled5);
            dtConfig.Add((byte)aqparams.IP_Mode_Index5);
            Write(dtConfig, (ushort)65535);
            Write(dtConfig, (ushort)0);
            dtConfig.Add((byte)aqparams.IP_SampleTime_Index5);
            Write(dtConfig, short.Parse(aqparams.SampleValue5));

            Write(dtConfig, aqparams.IP_Channel_CB_Enabled6);
            dtConfig.Add((byte)aqparams.IP_Mode_Index6);
            Write(dtConfig, (ushort)65535);
            Write(dtConfig, (ushort)0);
            dtConfig.Add((byte)aqparams.IP_SampleTime_Index6);
            Write(dtConfig, short.Parse(aqparams.SampleValue6));

            Write(dtConfig, aqparams.IP_Channel_CB_Enabled7);
            dtConfig.Add((byte)aqparams.IP_Mode_Index7);
            Write(dtConfig, (ushort)65535);
            Write(dtConfig, (ushort)0);
            dtConfig.Add((byte)aqparams.IP_SampleTime_Index7);
            Write(dtConfig, short.Parse(aqparams.SampleValue7));

            Write(dtConfig, aqparams.IP_Channel_CB_Enabled8);
            dtConfig.Add((byte)aqparams.IP_Mode_Index8);
            Write(dtConfig, (ushort)65535);
            Write(dtConfig, (ushort)0);
            dtConfig.Add((byte)aqparams.IP_SampleTime_Index8);
            Write(dtConfig, short.Parse(aqparams.SampleValue8));

            Write(dtConfig, 4);

            Write(dtConfig, (short)1000);
            Write(dtConfig, (short)1000);
            Write(dtConfig, (short)1000);
            Write(dtConfig, (short)1000);

            Write(dtConfig, 8);

            for (int i = 0; i < 8; i++)
                Write(dtConfig, emparams.ExpansionUnitParams[i]);

            byte[] lens = ValueConverter.GetBytes((uint)dtConfig.Count,true);
            for (int i = 0; i < 4; i++)
                dtConfig[i] = lens[i];
            lens = ValueConverter.GetBytes((ushort)(dtConfig.Count - 4), true);
            for (int i = 0; i < 2; i++)
                dtConfig[i + 4] = lens[i];
        }

        #endregion
        
        #region Initialize Modbus data
        
        static private void Write(ModbusTableModel mtmodel)
        {
            for (int i = 0; i < 9; i++)
                dtModbus.Add(0x00);
            dtModbus.Add((byte)(mtmodel.Children.Count));
            for (int i = 0; i < mtmodel.Children.Count; i++)
            {
                dtModbus.Add((byte)(i));
                Write(mtmodel.Children[i]);
            }
            foreach (ModbusModel mmodel in mtmodel.Children) Write(mmodel);
            int sz = dtModbus.Count() - 10;
            for (int i = 5; i < 9; i++)
            {
                dtModbus[i] = (byte)(sz & 0xff);
                sz >>= 8;
            }
        }

        static private void Write(ModbusModel mmodel)
        {
            dtModbus.Add((byte)(mmodel.Children.Count));
            foreach (ModbusItem mitem in mmodel.Children) Write(mitem);
            Write8(dtModbus, mmodel.Name);
            Write8(dtModbus, mmodel.Comment);
        }

        static private void Write(ModbusItem mitem)
        {
            dtModbus.Add(byte.Parse(mitem.SlaveID));
            dtModbus.Add(mitem.HandleCodes[mitem.SelectedHandleCodes().IndexOf(mitem.HandleCode)]);
            Write(dtModbus, short.Parse(mitem.SlaveRegister));
            Write(dtModbus, int.Parse(mitem.SlaveCount));
            Write(dtModbus, (short)(mitem.MasteRegisterAddress));
        }

        #endregion

        #endregion
        
        #region start download
        //下载前用于获取当前PLC信息(PLC型号，PLC运行状态，PLC当前程序，是否需要下载密码等)
        private static PLCMessage plcMessage;

        #region main download process
        public static DownloadError DownloadExecute(CommunicationManager communManager)
        {
            //初始化要下载的数据
            InitializeData(communManager.IFParent.MDProj);
            //首先通信测试获取底层PLC的状态
            CommunicationTestCommand CTCommand = new CommunicationTestCommand();
            if (!communManager.CommunicationHandle(CTCommand))
                return DownloadError.CommuicationFailed;
            else plcMessage = new PLCMessage(CTCommand);
            //首先判断PLC运行状态
            if (plcMessage.RunStatus == RunStatus.Run)
            {
                if (LocalizedMessageBox.Show(Properties.Resources.PLC_Status_To_Stop, LocalizedMessageButton.YesNo, LocalizedMessageIcon.Information) == LocalizedMessageResult.Yes)
                {
                    if (!communManager.CommunicationHandle(new SwitchPLCStatusCommand()))
                        return DownloadError.CommuicationFailed;
                }
                else return DownloadError.Cancel;
            }
            //验证是否需要下载密码
            if (plcMessage.IsDPNeed)
            {

            }
            
            DownloadError ret = DownloadError.None;
            if (IsDownloadProgram)
            {
                //下载Bin文件
                ret = DownloadBinExecute(communManager);
                if (ret != DownloadError.None)
                    return ret;

                //下载用于上载的XML压缩文件（包括程序，注释（可选），软元件表（可选）等）
                ret = DownloadProjExecute(communManager);
                if (ret != DownloadError.None)
                    return ret;

                //下载Modbus表格
                ret = DownloadModbusTableExecute(communManager);
                if (ret != DownloadError.None)
                    return ret;

                //下载 PlsTable
                ret = DownloadPlsTableExecute(communManager);
                if (ret != DownloadError.None)
                    return ret;

                //下载 PlsBlock
                ret = DownloadPlsBlockExecute(communManager);
                if (ret != DownloadError.None)
                    return ret;
            }
            //下载 Config
            if (IsDownloadSetting)
            {
                ret = DownloadConfigExecute(communManager);
                if (ret != DownloadError.None)
                    return ret;
            }
            return DownloadError.None;
        }
        #endregion

        #region Bin download
        private static DownloadError DownloadBinExecute(CommunicationManager communManager)
        {
            int time = 0;
            ICommunicationCommand command = new SwitchToIAPCommand();
            for (time = 0; time < 10 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 10) return DownloadError.DownloadFailed;
            command = new IAPDESKEYCommand(communManager.ExecLen);
            for (time = 0; time < 10 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 10) return DownloadError.DownloadFailed;
            byte[] data = communManager.ExecData.ToArray();
            byte[] pack = new byte[communManager.COMMU_MAX_DATALEN];
            int len = data.Length / communManager.COMMU_MAX_DATALEN;
            int rem = data.Length % communManager.COMMU_MAX_DATALEN;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < communManager.COMMU_MAX_DATALEN; j++)
                    pack[j] = data[i * communManager.COMMU_MAX_DATALEN + j];
                command = new TransportBinCommand(i, pack);
                for (time = 0; time < 3 && !communManager.CommunicationHandle(command);) time++;
                if (time >= 3) return DownloadError.DownloadFailed;
            }
            if (rem > 0)
            {
                pack = new byte[rem];
                for (int j = 0; j < rem; j++)
                    pack[j] = data[len * communManager.COMMU_MAX_DATALEN + j];
                command = new TransportBinCommand(len, pack);
                for (time = 0; time < 3 && !communManager.CommunicationHandle(command);) time++;
                if (time >= 3) return DownloadError.DownloadFailed;
            }
            command = new BinFinishedCommand();
            for (time = 0; time < 10 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 10) return DownloadError.DownloadFailed;
            return DownloadError.None;
        }
        #endregion

        #region Proj Download
        //工程文件（包括程序，注释（可选），软元件表（可选）等）
        private static DownloadError DownloadProjExecute(CommunicationManager communManager)
        {
            string genPath = string.Format(@"{0}\rar\temp", FileHelper.AppRootPath);
            if (!Directory.Exists(genPath))
                Directory.CreateDirectory(genPath);
            string _filename = communManager.IFParent.MDProj.FileName;
            _filename = string.Format(@"{0}\{1}.{2}", genPath,
                FileHelper.InvalidFileName(_filename) ? "tempdfile" : FileHelper.GetFileName(_filename),
                FileHelper.NewFileExtension);
            if (File.Exists(_filename)) File.Delete(_filename);

            //重新生成程序
            communManager.IFParent.MDProj.SaveToPLC(_filename);
            //返回生成的压缩文件全名
            string genFile = FileHelper.CompressFile(_filename);
            try
            {
                byte[] tempdata = FileHelper.GetBytesByBinaryFile(genFile);
                if (tempdata.Length == 0) return DownloadError.None;
                //先将传送的数据加密(注意密钥为数据的长度)
                CommandHelper.Encrypt(tempdata.Length, tempdata);
                //传送前，须在传送数据前加上4字节的数据长度，供上载时使用。
                byte[] data = ValueConverter.GetBytes((uint)tempdata.Length + 4, true);
                data = data.Concat(tempdata).ToArray();
                return _DownloadHandle(communManager, data, CommunicationDataDefine.CMD_DOWNLOAD_PRO);
            }
            catch (Exception)
            {
                return DownloadError.DownloadFailed;
            }
            finally
            {
                //下载完毕，删除生成的文件
                if (File.Exists(_filename)) File.Delete(_filename);
                if (File.Exists(genFile)) File.Delete(genFile);
            }
        }
        #endregion

        #region Modbus download
        private static DownloadError DownloadModbusTableExecute(CommunicationManager communManager)
        {
            if (dtModbus.Count == 0) return DownloadError.None;
            return _DownloadHandle(communManager, dtModbus.ToArray(), CommunicationDataDefine.CMD_DOWNLOAD_MODBUSTABLE);
        }
        #endregion

        #region PlsTable download
        private static DownloadError DownloadPlsTableExecute(CommunicationManager communManager)
        {
            if (dtTable.Count == 0) return DownloadError.None;
            return _DownloadHandle(communManager, dtTable.ToArray(), CommunicationDataDefine.CMD_DOWNLOAD_PLSTABLE);
        }
        #endregion

        #region PlsBlock download
        private static DownloadError DownloadPlsBlockExecute(CommunicationManager communManager)
        {
            if (dtBlock.Count == 0) return DownloadError.None;
            return _DownloadHandle(communManager, dtBlock.ToArray(), CommunicationDataDefine.CMD_DOWNLOAD_PLSBLOCK);
        }
        #endregion

        #region Config download
        private static DownloadError DownloadConfigExecute(CommunicationManager communManager)
        {
            if (dtConfig.Count == 0) return DownloadError.None;
            //byte[] data = ValueConverter.GetBytes((uint)dtConfig.Count + 4, true);
            //dtConfig = data.Concat(dtConfig).ToList();
            return _DownloadHandle(communManager, dtConfig.ToArray(), CommunicationDataDefine.CMD_DOWNLOAD_CONFIG);
        }
        #endregion

        #region DownloadHandle
        private static DownloadError _DownloadHandle(CommunicationManager communManager, byte[] data, byte funcCode)
        {
            if (data.Length == 0) return DownloadError.None;
            int time = 0;
            ICommunicationCommand command = new DownloadTypeStart(funcCode, data.Length);
            for (time = 0; time < 10 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 10) return DownloadError.DownloadFailed;
            byte[] pack = new byte[communManager.COMMU_MAX_DATALEN];
            int len = data.Length / communManager.COMMU_MAX_DATALEN;
            int rem = data.Length % communManager.COMMU_MAX_DATALEN;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < communManager.COMMU_MAX_DATALEN; j++)
                    pack[j] = data[i * communManager.COMMU_MAX_DATALEN + j];
                command = new DownloadTypeData(i, pack, funcCode);
                for (time = 0; time < 3 && !communManager.CommunicationHandle(command);) time++;
                if (time >= 3) return DownloadError.DownloadFailed;
            }
            if (rem > 0)
            {
                pack = new byte[rem];
                for (int j = 0; j < rem; j++)
                    pack[j] = data[len * communManager.COMMU_MAX_DATALEN + j];
                command = new DownloadTypeData(len, pack, funcCode);
                for (time = 0; time < 3 && !communManager.CommunicationHandle(command);) time++;
                if (time >= 3) return DownloadError.DownloadFailed;
            }
            command = new DownloadTypeOver(funcCode);
            for (time = 0; time < 10 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 10) return DownloadError.DownloadFailed;
            return DownloadError.None;
        }
        #endregion

        #region tools
        public static bool CheckOption(int oldoption, int newoption)
        {
            return ((oldoption & CommunicationDataDefine.OPTION_INITIALIZE) == (newoption & CommunicationDataDefine.OPTION_INITIALIZE)) && 
                ((oldoption & CommunicationDataDefine.OPTION_PROGRAM) == (newoption & CommunicationDataDefine.OPTION_PROGRAM));
        }
        #endregion

        #endregion
    }
}
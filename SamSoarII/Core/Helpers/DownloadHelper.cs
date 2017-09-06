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
using System.Windows.Threading;
using SamSoarII.PLCDevice;
using SamSoarII.Utility.DXF;

namespace SamSoarII.Core.Helpers
{
    public static class DownloadHelper
    {
        #region option
        private static int downloadoption = 9;
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
        static private List<byte> dtProject;
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

        static public void InitializeData(ProjectModel project)
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
            // Table表
            WritePLSTable(project);
        }

        #region Base

        static private void Write(List<byte> data, bool value, int index = -1)
        {
            if (index == -1)
                data.Add((byte)(value ? 1 : 0));
            else
                data[index] = (byte)(value ? 1 : 0);
        }

        static private void Write(List<byte> data, short value, int index = -1)
        {
            for (int i = 0; i < 2; i++)
            {
                if (index == -1)
                    data.Add((byte)(value & 0xff));
                else
                    data[index++] = (byte)(value & 0xff);
                value >>= 8;
            }
        }

        static private void Write(List<byte> data, ushort value, int index = -1)
        {
            for (int i = 0; i < 2; i++)
            {
                if (index == -1)
                    data.Add((byte)(value & 0xff));
                else
                    data[index++] = (byte)(value & 0xff);
                value >>= 8;
            }
        }

        static private void Write(List<byte> data, int value, int index = -1)
        {
            for (int i = 0; i < 4; i++)
            {
                if (index == -1)
                    data.Add((byte)(value & 0xff));
                else
                    data[index++] = (byte)(value & 0xff);
                value >>= 8;
            }
        }

        unsafe static private void Write(List<byte> data, float value, int index = -1)
        {
            Write(data, *((int*)(&value)), index);
        }

        static private void Write(List<byte> data, Int64 value, int index = -1)
        {
            for (int i = 0; i < 8; i++)
            {
                if (index == -1)
                    data.Add((byte)(value & 0xff));
                else
                    data[index++] = (byte)(value & 0xff);
                value >>= 8;
            }
        }

        static private void Write8(List<byte> data, string value)
        {
            data.Add((byte)(value.Length));
            for (int i = 0; i < value.Length; i++)
                data.Add((byte)value[i]);
        }

        static private void Write16(List<byte> data, string value)
        {
            Write(data, (short)(value.Length));
            for (int i = 0; i < value.Length; i++)
                data.Add((byte)value[i]);
        }
        static private void Write(List<byte> data, string value)
        {
            Write(data, value.Length);
            for (int i = 0; i < value.Length; i++)
                data.Add((byte)value[i]);
        }
        
        private const ushort PLC_REG_X = 0;
        private const ushort PLC_REG_Y = 10000;
        private const ushort PLC_REG_AI = 20000;
        private const ushort PLC_REG_AO = 20512;
        private const ushort PLC_REG_M = 30000;
        private const ushort PLC_REG_S = 50000;
        private const ushort PLC_REG_D = 40000;
        private const ushort PLC_REG_T = 60768;
        private const ushort PLC_REG_C = 60512;
        private const ushort PLC_REG_TV = 60256;
        private const ushort PLC_REG_CV = 60000;
        private const ushort PLC_REG_K = 0x10;
        private const ushort PLC_REG_H = 0x11;
        private const ushort PLC_REG_V = 48192;
        private const ushort PLC_REG_Z = 48200;
        private static void Write(List<byte> data, ValueModel value)
        {
            switch (value.Base)
            {
                case ValueModel.Bases.X: Write(data, PLC_REG_X); break;
                case ValueModel.Bases.Y: Write(data, PLC_REG_Y); break;
                case ValueModel.Bases.AI: Write(data, PLC_REG_AI); break;
                case ValueModel.Bases.AO: Write(data, PLC_REG_AO); break;
                case ValueModel.Bases.M: Write(data, PLC_REG_M); break;
                case ValueModel.Bases.S: Write(data, PLC_REG_S); break;
                case ValueModel.Bases.D: Write(data, PLC_REG_D); break;
                case ValueModel.Bases.T: Write(data, PLC_REG_T); break;
                case ValueModel.Bases.C: Write(data, PLC_REG_C); break;
                case ValueModel.Bases.TV: Write(data, PLC_REG_TV); break;
                case ValueModel.Bases.CV: Write(data, PLC_REG_CV); break;
                case ValueModel.Bases.K: Write(data, PLC_REG_K); break;
                case ValueModel.Bases.H: Write(data, PLC_REG_H); break;
                case ValueModel.Bases.V: Write(data, PLC_REG_V); break;
                case ValueModel.Bases.Z: Write(data, PLC_REG_Z); break;
            }
            if (value.Base == ValueModel.Bases.K || value.Base == ValueModel.Bases.H)
                Write(data, int.Parse(value.Value.ToString()));
            else
                Write(data, value.Offset);
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

            dtConfig.Add((byte)com232params.StationNumber);

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

            dtConfig.Add((byte)com485params.StationNumber);

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
            dtModbus[4] = 0x01;
            dtModbus.Add((byte)(mtmodel.Children.Count));
            for (int i = 0; i < mtmodel.Children.Count; i++)
            {
                dtModbus.Add((byte)(i));
                Write(mtmodel.Children[i]);
            }
            int sz = dtModbus.Count();
            for (int i = 0; i < 4; i++)
            {
                dtModbus[i] = (byte)(sz & 0xff);
                sz >>= 8;
            }
        }

        static private void Write(ModbusModel mmodel)
        {
            dtModbus.Add((byte)(mmodel.Children.Count));
            foreach (ModbusItem mitem in mmodel.Children) Write(mitem);
        }

        static private void Write(ModbusItem mitem)
        {
            dtModbus.Add(byte.Parse(mitem.SlaveID));
            dtModbus.Add(mitem.HandleCodes[mitem.SelectedHandleCodes().IndexOf(mitem.HandleCode)]);
            Write(dtModbus, ushort.Parse(mitem.SlaveRegister));
            Write(dtModbus, ushort.Parse(mitem.SlaveCount));
            Write(dtModbus, mitem.MasteRegisterAddress);
        }

        #endregion

        #region Initialize PLSTable data

        private static void WritePLSTable(ProjectModel project)
        {
            int count = 0;
            int idLength = dtTable.Count();
            Write(dtTable, (int)(0));
            int idCount = dtTable.Count();
            Write(dtTable, (int)(0));
            foreach (PolylineSystemModel polyline in project.Polylines)
            {
                polyline.PLSID = count++;
                WritePLSTable(polyline);
            }
            foreach (LadderDiagramModel diagram in project.Diagrams)
                foreach (LadderNetworkModel network in diagram.Children)
                {
                    if (network.IsMasked) continue;
                    foreach (LadderUnitModel unit in network.Children)
                    {
                        if (!unit.IsPLSTable) continue;
                        unit.PLSID = count++;
                        WritePLSTable(unit);
                    }
                }
            Write(dtTable, dtTable.Count() - idLength, idLength);
            Write(dtTable, count, idCount);
        }

        private static void WritePLSTable(PolylineSystemModel polyline)
        {
            int index = dtTable.Count();
            Write(dtTable, (int)(0));
            Write(dtTable, (short)(223));
            Write(dtTable, (int)(polyline.PLSID));
            Write(dtTable, (byte)(polyline.ID));
            Write(dtTable, polyline.IsEnabled);
            Write(dtTable, (byte)(polyline.Unit));
            Write(dtTable, polyline.X.PLS);
            Write(dtTable, polyline.X.DIR);
            Write(dtTable, polyline.X.WEI);
            Write(dtTable, polyline.X.LIM);
            Write(dtTable, polyline.X.CLM);
            Write(dtTable, polyline.X.ITV);
            Write(dtTable, polyline.Y.PLS);
            Write(dtTable, polyline.Y.DIR);
            Write(dtTable, polyline.Y.WEI);
            Write(dtTable, polyline.Y.LIM);
            Write(dtTable, polyline.Y.CLM);
            Write(dtTable, polyline.Y.ITV);
            Write(dtTable, (byte)(polyline.Overflow));
            Write(dtTable, (short)(0));
            Write(dtTable, polyline.IsHMIEnabled);
            Write(dtTable, polyline.HMI);
            Write(dtTable, dtTable.Count() - index, index);
        }

        private static void WritePLSTable(LadderUnitModel unit)
        {
            int index = dtTable.Count();
            Write(dtTable, (int)(0));
            if (unit is TBLModel)
            {
                TBLModel tbl = (TBLModel)unit;
                Write(dtTable, (ushort)(221));
                Write(dtTable, tbl.PLSID);
                for (int i = 0; i < 3; i++)
                    Write(dtTable, tbl.Children[i]);
                Write(dtTable, tbl.Elements.Count());
                for (int i = 0; i < tbl.Elements.Count; i++)
                {
                    Write(dtTable, tbl.Elements[i].Freq);
                    Write(dtTable, tbl.Elements[i].Number);
                    Write(dtTable, tbl.Elements[i].Cond);
                    Write(dtTable, tbl.Elements[i].End);
                    Write(dtTable, (ushort)(tbl.Elements[i].Jump));
                }
            }
            else if (unit is CAMModel)
            {
                CAMModel cam = (CAMModel)unit;
                Write(dtTable, (ushort)(228));
                Write(dtTable, cam.PLSID);
                Write(dtTable, cam.Children[0].Offset);
                Write(dtTable, cam.NumStore.Offset);
                Write(dtTable, cam.MaxTarget);
                Write(dtTable, (byte)(cam.RefMode));
                Write(dtTable, cam.RefAddr);
                Write(dtTable, cam.Elements.Count);
                for (int i = 0; i < cam.Elements.Count; i++)
                {
                    Write(dtTable, cam.Elements[i].Target);
                    Write(dtTable, cam.Elements[i].Address);
                    Write(dtTable, (byte)(cam.Elements[i].Mode));
                }
            }
            else
            {
                IEnumerable<Polyline> polylines = null;
                switch (unit.Type)
                {
                    case LadderUnitModel.Types.POLYLINEF:
                        Write(dtTable, (ushort)(222));
                        polylines = ((POLYLINEFModel)unit).Polylines.Cast<Polyline>();
                        break;
                    case LadderUnitModel.Types.POLYLINEI:
                        Write(dtTable, (ushort)(230));
                        polylines = ((POLYLINEIModel)unit).Polylines.Cast<Polyline>();
                        break;
                    case LadderUnitModel.Types.LINEF:
                        Write(dtTable, (ushort)(224));
                        polylines = new Polyline[] { ((LINEFModel)unit).Line };
                        break;
                    case LadderUnitModel.Types.LINEI:
                        Write(dtTable, (ushort)(231));
                        polylines = new Polyline[] { ((LINEIModel)unit).Line };
                        break;
                    case LadderUnitModel.Types.ARCF:
                        Write(dtTable, (ushort)(225));
                        polylines = new Polyline[] { ((ARCHFModel)unit).Arch };
                        break;
                    case LadderUnitModel.Types.ARCI:
                        Write(dtTable, (ushort)(232));
                        polylines = new Polyline[] { ((ARCHIModel)unit).Arch };
                        break;
                }
                Write(dtTable, unit.PLSID);
                Write(dtTable, byte.Parse(unit.Children[0].Value.ToString()));
                POLYLINEModel poly = (POLYLINEModel)unit;
                Write(dtTable, (byte)(poly.Unit));
                Write(dtTable, poly.RefAddr);
                Write(dtTable, (byte)(poly.RefMode));
                Write(dtTable, polylines.Count() + 1);
                foreach (Polyline polyline in polylines)
                {
                    if (polyline is IntLine || polyline is FloatLine)
                        Write(dtTable, (byte)(0));
                    else
                        Write(dtTable, (byte)(1));
                    Write(dtTable, polyline.X);
                    Write(dtTable, polyline.Y);
                    Write(dtTable, (byte)(polyline.Mode));
                    if (polyline is IntArch)
                    {
                        IntArch arch = (IntArch)polyline;
                        Write(dtTable, (byte)(arch.Type));
                        switch (arch.Type)
                        {
                            case IntArch.ArchTypes.TwoPoint:
                                Write(dtTable, arch.R);
                                Write(dtTable, (byte)(arch.Dir));
                                Write(dtTable, (byte)(arch.Qua));
                                break;
                            case IntArch.ArchTypes.ThreePoint:
                                Write(dtTable, arch.CX);
                                Write(dtTable, arch.CY);
                                break;
                        }
                        Write(dtTable, arch.V);
                        Write(dtTable, arch.AC);
                        Write(dtTable, arch.DC);
                    }
                    if (polyline is FloatArch)
                    {
                        FloatArch arch = (FloatArch)polyline;
                        Write(dtTable, (byte)(arch.Type));
                        switch (arch.Type)
                        {
                            case FloatArch.ArchTypes.TwoPoint:
                                Write(dtTable, arch.R);
                                Write(dtTable, (byte)(arch.Dir));
                                Write(dtTable, (byte)(arch.Qua));
                                break;
                            case FloatArch.ArchTypes.ThreePoint:
                                Write(dtTable, arch.CX);
                                Write(dtTable, arch.CY);
                                break;
                        }
                        Write(dtTable, arch.V);
                        Write(dtTable, arch.AC);
                        Write(dtTable, arch.DC);
                    }
                }
                Write(dtTable, (byte)(0xff));
            }
            Write(dtTable, dtTable.Count() - index, index);
        }

        #endregion

        #region Initialize PLSBlock data

        static public List<byte> GetData(PLSBlockModel plsblock)
        {
            List<byte> data = new List<byte>();
            Write(data, (int)(0));
            Write16(data, plsblock.Name);
            Write(data, (short)(plsblock.SystemID));
            Write(data, plsblock.Velocity);
            Write(data, plsblock.ACTime);
            Write(data, plsblock.DCTime);
            Write(data, plsblock.Elements.Count);
            for (int i = 0; i < plsblock.Elements.Count; i++)
            {
                DXFEntity element = plsblock.Elements[i];
                if (element is DXFLine)
                {
                    DXFLine line = (DXFLine)(element);
                    Write(data, (byte)(0));
                    Write(data, line.IsReal ? (byte)(1) : (byte)(0));
                    Write(data, (float)line.EndP.X);
                    Write(data, (float)line.EndP.Y);
                }
                if (element is DXFArc)
                {
                    DXFArc arch = (DXFArc)(element);
                    Write(data, (byte)(1));
                    Write(data, (byte)(1));
                    Write(data, (float)(0.0));
                    Write(data, (float)(0.0));
                    Write(data, (byte)(0));
                    Write(data, (float)(0.0));
                    Write(data, (float)(0.0));
                }
                if (element is DXFCircle)
                {
                    DXFCircle circle = (DXFCircle)(element);
                    Write(data, (byte)(2));
                    Write(data, (byte)(1));
                    Write(data, (float)(0.0));
                    Write(data, (float)(0.0));
                    Write(data, (byte)(0));
                    Write(data, (float)(0.0));
                    Write(data, (float)(0.0));
                }
            }
            Write(data, data.Count(), 0);
            return data;
        }

        #endregion

        #endregion

        #region start download

        static ProgressBarHandle Handle;
        static uint TotalLen;
        static uint currentLen;

        #region main download process
        public static CommuicationError DownloadExecute(CommunicationManager communManager, ProgressBarHandle handle)
        {
            TotalLen = 0;
            currentLen = 0;
            Handle = handle;
            Handle.UpdateMessage(Properties.Resources.Initialize_Data);
            //初始化要下载的数据
            //InitializeData(communManager.IFParent.MDProj);

            //判断当前工程PLC型号与底层是否一致
            if ((int)PLCDeviceManager.GetPLCDeviceManager().SelectDevice.Type != (int)communManager.PLCMessage.PLCType)
            {
                if (LocalizedMessageBox.Show(Properties.Resources.PLC_Type_Not_Equal, LocalizedMessageButton.YesNo, LocalizedMessageIcon.Information) == LocalizedMessageResult.Yes)
                {
                    PLCDeviceManager.GetPLCDeviceManager().SetSelectDeviceType(communManager.PLCMessage.PLCType);
                }
            }

            //首先判断PLC运行状态
            if (communManager.PLCMessage.RunStatus == RunStatus.Run)
            {
                if (LocalizedMessageBox.Show(Properties.Resources.PLC_Status_To_Stop, LocalizedMessageButton.YesNo, LocalizedMessageIcon.Information) == LocalizedMessageResult.Yes)
                {
                    if (!communManager.CommunicationHandle(new SwitchPLCStatusCommand()))
                        return CommuicationError.CommuicationFailed;
                }
                else return CommuicationError.Cancel;
            }

            CommuicationError ret = CommuicationError.None;
            if (IsDownloadProgram)
            {
                //先初始化project的压缩文件,文件大小用于记录进度条
                string genFile, _filename;
                dtProject = PrepareForProj(communManager, out genFile, out _filename);

                //记录下载数据的总长度
                TotalLen += (uint)communManager.ExecData.ToArray().Length;
                TotalLen += (uint)dtProject.Count;
                TotalLen += (uint)dtModbus.Count;
                TotalLen += (uint)dtTable.Count;
                TotalLen += (uint)dtBlock.Count;
                TotalLen += (uint)dtIcon.Count;
                if(IsDownloadSetting) TotalLen += (uint)dtConfig.Count;

                Handle.UpdateMessage(Properties.Resources.Project_Download);
                //下载Bin文件
                ret = DownloadBinExecute(communManager);
                if (ret != CommuicationError.None)
                    return ret;

                //下载用于上载的XML压缩文件（包括程序，注释（可选），软元件表（可选）等）
                ret = DownloadProjExecute(communManager, genFile, _filename);
                if (ret != CommuicationError.None)
                    return ret;

                //下载Modbus表格
                ret = DownloadModbusTableExecute(communManager);
                if (ret != CommuicationError.None)
                    return ret;

                //下载 PlsTable
                ret = DownloadPlsTableExecute(communManager);
                if (ret != CommuicationError.None)
                    return ret;

                //下载 PlsBlock
                ret = DownloadPlsBlockExecute(communManager);
                if (ret != CommuicationError.None)
                    return ret;
            }
            //下载 Config
            if (IsDownloadSetting)
            {
                handle.UpdateMessage(Properties.Resources.Config_Download);
                ret = DownloadConfigExecute(communManager);
                if (ret != CommuicationError.None)
                    return ret;
            }
            Handle = null;
            return CommuicationError.None;
        }
        #endregion

        #region Bin download
        private static CommuicationError DownloadBinExecute(CommunicationManager communManager)
        {
            int time = 0;
            ICommunicationCommand command = new SwitchToIAPCommand();

            for (time = 0; time < 5 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 5) return CommuicationError.DownloadFailed;

            command = new IAPDESKEYCommand(communManager.ExecLen);
            for (time = 0; time < 5 && !communManager.CommunicationHandle(command,true,2000);)
            {
                if(command.ErrorCode == FGs_ERR_CODE.COMCODE_DOWNLOAD_BEYOND)
                    return CommuicationError.DataSizeBeyond;
                Thread.Sleep(200);
                time++;
            }
            if (time >= 5) return CommuicationError.DownloadFailed;

            byte[] data = communManager.ExecData.ToArray();
            byte[] pack = new byte[communManager.DOWN_MAX_DATALEN];
            int len = data.Length / communManager.DOWN_MAX_DATALEN;
            int rem = data.Length % communManager.DOWN_MAX_DATALEN;

            for (int i = 0; i < len; i++)
            {
                ReportProgress(communManager, (uint)communManager.DOWN_MAX_DATALEN);

                for (int j = 0; j < communManager.DOWN_MAX_DATALEN; j++)
                    pack[j] = data[i * communManager.DOWN_MAX_DATALEN + j];

                command = new TransportBinCommand(i, pack);
                for (time = 0; time < 3 && !communManager.CommunicationHandle(command);) time++;
                if (time >= 3) return CommuicationError.DownloadFailed;
            }
            if (rem > 0)
            {
                pack = new byte[rem];
                for (int j = 0; j < rem; j++)
                    pack[j] = data[len * communManager.DOWN_MAX_DATALEN + j];

                ReportProgress(communManager, (uint)pack.Length);

                command = new TransportBinCommand(len, pack);
                for (time = 0; time < 3 && !communManager.CommunicationHandle(command);) time++;
                if (time >= 3) return CommuicationError.DownloadFailed;
            }
            command = new BinFinishedCommand();
            for (time = 0; time < 5 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 5) return CommuicationError.DownloadFailed;
            return CommuicationError.None;
        }
        #endregion

        #region Proj Download
        private static List<byte> PrepareForProj(CommunicationManager communManager ,out string genFile,out string _filename)
        {
            string genPath = string.Format(@"{0}\rar\temp", FileHelper.AppRootPath);
            if (!Directory.Exists(genPath))
                Directory.CreateDirectory(genPath);
            _filename = communManager.IFParent.MDProj.FileName;
            _filename = string.Format(@"{0}\{1}.{2}", genPath,
                FileHelper.InvalidFileName(_filename) ? "tempdfile" : Path.GetFileNameWithoutExtension(_filename),
                FileHelper.NewFileExtension);
            if (File.Exists(_filename)) File.Delete(_filename);

            //重新生成程序
            communManager.IFParent.MDProj.SaveToPLC(_filename);
            //返回生成的压缩文件全名
            genFile = FileHelper.CompressFile(_filename);
            try
            {
                byte[] tempdata = FileHelper.GetBytesByBinaryFile(genFile);
                if (tempdata.Length == 0) return new List<byte>();
                //先将传送的数据加密(注意密钥为数据的长度)
                CommandHelper.Encrypt(tempdata.Length, tempdata);
                //传送前，须在传送数据前加上4字节的数据长度，供上载时使用。
                byte[] data = ValueConverter.GetBytes((uint)tempdata.Length + 4, true);
                return data.Concat(tempdata).ToList();
            }
            catch
            {
                return new List<byte>();
            }
        }
        //工程文件（包括程序，注释（可选），软元件表（可选）等）
        private static CommuicationError DownloadProjExecute(CommunicationManager communManager, string genFile, string _filename)
        {
            try
            {
                if (dtProject.Count == 0) return CommuicationError.None;
                return _DownloadHandle(communManager, dtProject.ToArray(), CommunicationDataDefine.CMD_DOWNLOAD_PRO);
            }
            catch (Exception)
            {
                return CommuicationError.DownloadFailed;
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
        private static CommuicationError DownloadModbusTableExecute(CommunicationManager communManager)
        {
            if (dtModbus.Count == 0) return CommuicationError.None;
            return _DownloadHandle(communManager, dtModbus.ToArray(), CommunicationDataDefine.CMD_DOWNLOAD_MODBUSTABLE);
        }
        #endregion

        #region PlsTable download
        private static CommuicationError DownloadPlsTableExecute(CommunicationManager communManager)
        {
            if (dtTable.Count == 0) return CommuicationError.None;
            return _DownloadHandle(communManager, dtTable.ToArray(), CommunicationDataDefine.CMD_DOWNLOAD_PLSTABLE);
        }
        #endregion

        #region PlsBlock download
        private static CommuicationError DownloadPlsBlockExecute(CommunicationManager communManager)
        {
            if (dtBlock.Count == 0) return CommuicationError.None;
            return _DownloadHandle(communManager, dtBlock.ToArray(), CommunicationDataDefine.CMD_DOWNLOAD_PLSBLOCK);
        }
        #endregion

        #region Config download
        private static CommuicationError DownloadConfigExecute(CommunicationManager communManager)
        {
            if (dtConfig.Count == 0) return CommuicationError.None;
            return _DownloadHandle(communManager, dtConfig.ToArray(), CommunicationDataDefine.CMD_DOWNLOAD_CONFIG);
        }
        #endregion

        #region DownloadHandle
        private static CommuicationError _DownloadHandle(CommunicationManager communManager, byte[] data, byte funcCode)
        {
            if (data.Length == 0) return CommuicationError.None;
            int time = 0;
            ICommunicationCommand command = new DownloadTypeStart(funcCode, data.Length);

            int overTime = funcCode == CommunicationDataDefine.CMD_DOWNLOAD_PRO ? 2000 : 100 ;

            for (time = 0; time < 5 && !communManager.CommunicationHandle(command,true,overTime);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 5) return CommuicationError.DownloadFailed;
            byte[] pack = new byte[communManager.DOWN_MAX_DATALEN];
            int len = data.Length / communManager.DOWN_MAX_DATALEN;
            int rem = data.Length % communManager.DOWN_MAX_DATALEN;
            for (int i = 0; i < len; i++)
            {
                ReportProgress(communManager, (uint)communManager.DOWN_MAX_DATALEN);

                for (int j = 0; j < communManager.DOWN_MAX_DATALEN; j++)
                    pack[j] = data[i * communManager.DOWN_MAX_DATALEN + j];
                
                command = new DownloadTypeData(i, pack, funcCode);
                for (time = 0; time < 3 && !communManager.CommunicationHandle(command);) time++;
                if (time >= 3) return CommuicationError.DownloadFailed;
            }
            if (rem > 0)
            {
                pack = new byte[rem];
                for (int j = 0; j < rem; j++)
                    pack[j] = data[len * communManager.DOWN_MAX_DATALEN + j];

                ReportProgress(communManager, (uint)pack.Length);

                command = new DownloadTypeData(len, pack, funcCode);
                for (time = 0; time < 3 && !communManager.CommunicationHandle(command);) time++;
                if (time >= 3) return CommuicationError.DownloadFailed;
            }
            command = new DownloadTypeOver(funcCode);
            for (time = 0; time < 5 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 5) return CommuicationError.DownloadFailed;
            return CommuicationError.None;
        }
        #endregion

        #region tools
        public static bool CheckOption(int oldoption, int newoption)
        {
            return ((oldoption & CommunicationDataDefine.OPTION_INITIALIZE) == (newoption & CommunicationDataDefine.OPTION_INITIALIZE)) && 
                ((oldoption & CommunicationDataDefine.OPTION_PROGRAM) == (newoption & CommunicationDataDefine.OPTION_PROGRAM));
        }

        private static void ReportProgress(CommunicationManager communManager,uint datalen)
        {
            if (communManager.MNGCurrent == communManager.MNGUSB)
                Handle.ReportProgress(currentLen * 100 / TotalLen, 0.05);
            else Handle.ReportProgress(currentLen * 100 / TotalLen, 36 * 115200 / communManager.MNGPort.BaudRate);
            currentLen += datalen;
        }
        #endregion

        #endregion
    }
}
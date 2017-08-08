using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System.IO;
using SamSoarII.Global;

namespace SamSoarII.Core.Helpers
{
    public abstract class DownloadHelper : IHelper
    {
        #region Write Datas 

        /// <summary> 程序 </summary>
        static private List<byte> dtBinary;
        /// <summary> 条形码 </summary>
        static private List<byte> dtIcon;
        /// <summary> 工程 </summary>
        static private List<byte> dtProject;
        /// <summary> 注释 </summary>
        static private List<byte> dtComment;
        /// <summary> 元件表 </summary>
        static private List<byte> dtEleList;
        /// <summary> 配置 </summary>
        static private List<byte> dtConfig;
        /// <summary> Modbus表 </summary>
        static private List<byte> dtModbus;
        /// <summary> Table表 </summary>
        static private List<byte> dtTable;
        /// <summary> Block表 </summary>
        static private List<byte> dtBlock;

        static private List<FuncModel> ltFuncs;

        static public void Write(ProjectModel project)
        {
            // 初始化
            dtBinary = new List<byte>();
            dtIcon = new List<byte>();
            dtProject = new List<byte>();
            dtComment = new List<byte>();
            dtEleList = new List<byte>();
            dtConfig = new List<byte>();
            dtModbus = new List<byte>();
            dtTable = new List<byte>();
            dtBlock = new List<byte>();
            // 读取bin
            string currentpath = FileHelper.AppRootPath;
            string execfile = String.Format(@"{0:s}\downc.bin", currentpath);
            BinaryReader br = new BinaryReader(new FileStream(execfile, FileMode.Open));
            while (br.BaseStream.CanRead)
                try { dtBinary.Add(br.ReadByte()); }
                catch (EndOfStreamException) { break; }
            br.Close();
            // 条形码

            // 工程
            ltFuncs = project.Funcs.ToList();
            Write(project.ValueManager);
            Write(dtProject, project.Diagrams.Count);
            foreach (LadderDiagramModel ldmodel in project.Diagrams) Write(ldmodel);
            Write(dtProject, project.FuncBlocks.Count);
            foreach (FuncBlockModel fbmodel in project.FuncBlocks) Write(fbmodel);
            Write(dtProject, project.Monitor.Children.Count);
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

        static private void Write(List<byte> data, int value)
        {
            for (int i = 0; i < 4; i++)
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

        #endregion

        #region Project
        
        static private void Write(ValueManager ValueManager)
        {
            int id = 0;
            foreach (ValueInfo vinfo in ValueManager)
                foreach (ValueStore vstore in vinfo.Stores)
                    vstore.ID = ++id;
            Write(dtProject, (short)id);
            foreach (ValueInfo vinfo in ValueManager)
                foreach (ValueStore vstore in vinfo.Stores)
                    Write(vstore, ValueManager);
            foreach (ValueStore vstore in ValueManager.EmptyInfo.Stores)
                vstore.ID = ++id;
            Write(dtProject, (short)id);
            foreach (ValueStore vstore in ValueManager.EmptyInfo.Stores)
                Write(vstore, ValueManager);

        }

        static private void Write(ValueStore vstore, ValueManager ValueManager)
        {
            if (vstore.Parent == ValueManager.EmptyInfo)
            {
                switch (vstore.Type)
                {
                    case ValueModel.Types.BOOL:
                    case ValueModel.Types.WORD:
                    case ValueModel.Types.DWORD:
                        Write(dtProject, int.Parse(vstore.Value.ToString()));
                        break;
                    case ValueModel.Types.FLOAT:
                        Write(dtProject, (int)(ValueConverter.FloatToUInt((float)(vstore.Value))));
                        break;
                    default:
                        Write(dtProject, (int)0);
                        break;
                }
            }
            else
            {
                Write(dtProject, (short)(ValueManager.IndexOf(vstore.Parent)));
                int i = 0;
                i |= ((int)(vstore.Type) & 0x0f);
                i <<= 4;
                i |= ((int)(vstore.Flag - 1) & 0x1f);
                i <<= 5;
                switch (vstore.Intra)
                {
                    case ValueModel.Bases.V: i |= 0x01; break;
                    case ValueModel.Bases.Z: i |= 0x02; break;
                }
                i <<= 2;
                i |= (vstore.IntraOffset & 0x07);
                Write(dtProject, (short)i);
            }
        }

        static private void Write(LadderDiagramModel ldmodel)
        {
            Write(dtProject, ldmodel.IsMainLadder);
            Write(dtProject, ldmodel.IsExpand);
            Write(dtProject, ldmodel.Name);
            Write(dtProject, ldmodel.Brief);
            Write(dtProject, ldmodel.NetworkCount);
            foreach (LadderNetworkModel lnmodel in ldmodel.Children)
                Write(lnmodel);
        }

        static private void Write(LadderNetworkModel lnmodel)
        {
            Write(dtProject, lnmodel.IsExpand);
            Write(dtProject, lnmodel.IsMasked);
            Write(dtProject, lnmodel.Description);
            Write(dtProject, lnmodel.Brief);
            Write(dtProject, lnmodel.RowCount);
            for (int y = 0; y < lnmodel.RowCount; y++)
            {
                int link = 0;
                for (int x = 0; x < GlobalSetting.LadderXCapacity; x++)
                {
                    link |= (lnmodel.Children[x, y] != null ? 1 : 0) << (x + x);
                    link |= (lnmodel.VLines[x, y] != null ? 1 : 0) << (x + x + 1);
                }
                Write(dtProject, link);
            }
            Write(dtProject, lnmodel.Children.Count());
            foreach (LadderUnitModel unit in lnmodel.Children)
                Write(unit);
        }

        static private void Write(LadderUnitModel unit)
        {
            Write(dtProject, (short)(unit.X | (unit.Y << 4)));
            dtProject.Add((byte)unit.Type);
            if (unit.Type == LadderUnitModel.Types.CALLM)
            {
                FuncModel func = ltFuncs.Where(f => f.Name.Equals(unit.Children[0].Text)).First();
                Write(dtProject, (short)(ltFuncs.IndexOf(func)));
                Write(dtProject, (byte)(unit.Children.Count - 1));
                for (int i = 1; i < unit.Children.Count; i++)
                    Write(dtProject, (short)(unit.Children[i].Store.ID));
            }
            else
            {
                for (int i = 0; i < unit.Children.Count; i++)
                    if (unit.Children[i].Type == ValueModel.Types.STRING)
                    {
                        if (unit.Type == LadderUnitModel.Types.CALLM && i == 0
                         || unit.Type == LadderUnitModel.Types.ATCH && i == 1)
                        {
                            LadderDiagramModel diagram = unit.Project.Diagrams.Where(d => d.Name.Equals(unit.Children[i].Text)).First();
                            Write(dtProject, (short)(unit.Project.Diagrams.IndexOf(diagram)));
                        }
                        if (unit.Type == LadderUnitModel.Types.MBUS && i == 1)
                        {
                            ModbusModel modbus = unit.Project.Modbus.Children.Where(m => m.Name.Equals(unit.Children[i].Text)).First();
                            Write(dtProject, (short)(unit.Project.Modbus.Children.IndexOf(modbus)));
                        }
                    }
                    else
                        Write(dtProject, (short)(unit.Children[i].Store.ID));
            }
        }
        
        static private void Write(FuncBlockModel fbmodel)
        {
            Write(dtProject, fbmodel.IsLibrary);
            if (!fbmodel.IsLibrary)
            {
                Write(dtProject, fbmodel.Name);
                Write(dtProject, fbmodel.View != null ? fbmodel.View.Code : fbmodel.Code);
            }
        }

        static private void Write(MonitorTable mtable)
        {
            Write(dtProject, mtable.Name);
            Write(dtProject, mtable.Children.Count);
            foreach (MonitorElement melement in mtable.Children) Write(melement);
        }

        static private void Write(MonitorElement melement)
        {
            Write(dtProject, (short)(melement.Store.ID));
        }

        #endregion

        #region Config
        
        static private void WriteConfig(ProjectPropertyParams pparams)
        {
            dtConfig.Add(0x00);
            dtConfig.Add(0x00);
            CommunicationInterfaceParams com232params = pparams.PARACom232;
            CommunicationInterfaceParams com485params = pparams.PARACom485;
            PasswordParams pwparams = pparams.PARAPassword;
            FilterParams ftparams = pparams.PARAFilter;
            HoldingSectionParams hsparams = pparams.PARAHolding;

            dtConfig.Add((byte)(com232params.BaudRateIndex));
            dtConfig.Add((byte)(com232params.DataBitIndex));
            dtConfig.Add((byte)(com232params.StopBitIndex));
            dtConfig.Add((byte)(com232params.CheckCodeIndex));
            dtConfig.Add((byte)(com485params.BaudRateIndex));
            dtConfig.Add((byte)(com485params.DataBitIndex));
            dtConfig.Add((byte)(com485params.StopBitIndex));
            dtConfig.Add((byte)(com485params.CheckCodeIndex));
            dtConfig.Add((byte)(com232params.StationNumber));
            Write(dtConfig, (short)(com232params.Timeout));
            dtConfig.Add((byte)(pwparams.PWENUpload ? 1 : 0));
            Write8(dtConfig, pwparams.PWUpload);
            dtConfig.Add((byte)(com232params.ComType));
            dtConfig.Add((byte)(com485params.ComType));

            dtConfig.Add((byte)(pwparams.PWENDownload ? 1 : 0));
            Write8(dtConfig, pwparams.PWDownload);
            dtConfig.Add((byte)(pwparams.PWENMonitor ? 1 : 0));
            Write8(dtConfig, pwparams.PWMonitor);
            dtConfig.Add((byte)(ftparams.IsChecked ? 1 : 0));
            int fttime = 1 << (ftparams.FilterTimeIndex + 1);
            Write(dtConfig, (short)(fttime));

            Write(dtConfig, (short)(hsparams.MStartAddr));
            Write(dtConfig, (short)(hsparams.MLength));
            Write(dtConfig, (short)(hsparams.SStartAddr));
            Write(dtConfig, (short)(hsparams.SLength));
            Write(dtConfig, (short)(hsparams.DStartAddr));
            Write(dtConfig, (short)(hsparams.DLength));
            Write(dtConfig, (short)(hsparams.CVStartAddr));
            Write(dtConfig, (short)(hsparams.CVLength));
            
            int sz = dtConfig.Count() - 2;
            dtConfig[0] = (byte)(sz & 0xff);
            dtConfig[1] = (byte)(sz >> 8);
        }

        #endregion

        #region Modbus

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
    }
}

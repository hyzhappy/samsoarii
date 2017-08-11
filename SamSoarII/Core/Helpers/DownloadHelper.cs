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

    public class DownloadHelper
    {
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
        
        #region InitializeData
        
        static private void InitializeData(ProjectModel project)
        {
            //工程，注释，软元件等用于上载的信息直接压缩xml

            //dtBinary = new List<byte>();
            //dtProject = new List<byte>();
            //dtComment = new List<byte>();
            //dtEleList = new List<byte>();

            // 初始化
            dtIcon = new List<byte>();
            dtConfig = new List<byte>();
            dtModbus = new List<byte>();
            dtTable = new List<byte>();
            dtBlock = new List<byte>();
            
            // 条形码

            // 工程
            //ltFuncs = project.Funcs.ToList();
            //Write(project.ValueManager);
            //Write(dtProject, project.Diagrams.Count);
            //foreach (LadderDiagramModel ldmodel in project.Diagrams) Write(ldmodel);
            //Write(dtProject, project.FuncBlocks.Count);
            //foreach (FuncBlockModel fbmodel in project.FuncBlocks) Write(fbmodel);
            //Write(dtProject, project.Monitor.Children.Count);

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
            if (!communManager.DownloadHandle(CTCommand))
                return DownloadError.CommuicationFailed;
            else plcMessage = new PLCMessage(CTCommand);
            //首先判断PLC运行状态
            if (plcMessage.RunStatus == RunStatus.Run)
            {
                if (LocalizedMessageBox.Show(Properties.Resources.PLC_Status_To_Stop, LocalizedMessageButton.YesNo, LocalizedMessageIcon.Information) == LocalizedMessageResult.Yes)
                {
                    if (!communManager.DownloadHandle(new SwitchPLCStatusCommand()))
                        return DownloadError.CommuicationFailed;
                }
                else return DownloadError.Cancel;
            }
            //验证是否需要下载密码
            if (plcMessage.IsDPNeed)
            {

            }

            //下载Bin文件
            DownloadError ret = DownloadError.None;
            ret = DownloadBinExecute(communManager);
            if (ret != DownloadError.None)
                return ret;

            //下载用于上载的XML压缩文件（包括程序，注释（可选），软元件表（可选）等）
            ret = DownloadProjExecute(communManager,communManager.IFParent.MDProj.PARAProj.PARACom.DownloadOption);
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

            //下载 Config
            if (communManager.IFParent.MDProj.PARAProj.PARACom.IsDownloadSetting)
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
            for (time = 0; time < 10 && !communManager.DownloadHandle(command);)
            {
                Thread.Sleep(500);
                time++;
            }
            if (time >= 10) return DownloadError.DownloadFailed;
            command = new IAPDESKEYCommand(communManager.ExecLen);
            for (time = 0; time < 10 && !communManager.DownloadHandle(command);)
            {
                Thread.Sleep(500);
                time++;
            }
            if (time >= 10) return DownloadError.DownloadFailed;
            byte[] data = communManager.ExecData.ToArray();
            byte[] pack = new byte[communManager.DOWNLOAD_MAX_DATALEN];
            int len = data.Length / communManager.DOWNLOAD_MAX_DATALEN;
            int rem = data.Length % communManager.DOWNLOAD_MAX_DATALEN;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < communManager.DOWNLOAD_MAX_DATALEN; j++)
                    pack[j] = data[i * communManager.DOWNLOAD_MAX_DATALEN + j];
                command = new TransportBinCommand(i, pack);
                for (time = 0; time < 3 && !communManager.DownloadHandle(command);) time++;
                if (time >= 3) return DownloadError.DownloadFailed;
            }
            if (rem > 0)
            {
                pack = new byte[rem];
                for (int j = 0; j < rem; j++)
                    pack[j] = data[len * communManager.DOWNLOAD_MAX_DATALEN + j];
                command = new TransportBinCommand(len, pack);
                for (time = 0; time < 3 && !communManager.DownloadHandle(command);) time++;
                if (time >= 3) return DownloadError.DownloadFailed;
            }
            command = new BinFinishedCommand();
            for (time = 0; time < 10 && !communManager.DownloadHandle(command);)
            {
                Thread.Sleep(500);
                time++;
            }
            if (time >= 10) return DownloadError.DownloadFailed;
            return DownloadError.None;
        }
        #endregion

        #region Proj Download
        //工程文件（包括程序，注释（可选），软元件表（可选）等）
        private static DownloadError DownloadProjExecute(CommunicationManager communManager,int flag)
        {
            if (flag == 0) return DownloadError.None;
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
                byte[] tempdata = FileHelper.GenerateBinaryFile(genFile);
                if (tempdata.Length == 0) return DownloadError.None;
                //先将传送的数据加密(注意密钥为数据的长度)
                CommandHelper.Encrypt(tempdata.Length, tempdata);
                //传送前，须在传送数据前加上4字节的数据长度，供上载时使用。
                byte[] data = ValueConverter.GetBytes((uint)tempdata.Length, true);
                data = data.Concat(tempdata).ToArray();
                return _DownloadHandle(communManager, data,CommunicationDataDefine.CMD_DOWNLOAD_PRO);
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
            return _DownloadHandle(communManager, dtConfig.ToArray(), CommunicationDataDefine.CMD_DOWNLOAD_CONFIG);
        }
        #endregion

        #region DownloadHandle
        private static DownloadError _DownloadHandle(CommunicationManager communManager, byte[] data, byte funcCode)
        {
            if (data.Length == 0) return DownloadError.None;
            int time = 0;
            ICommunicationCommand command = new DownloadTypeStart(funcCode, data.Length);
            for (time = 0; time < 10 && !communManager.DownloadHandle(command);)
            {
                Thread.Sleep(500);
                time++;
            }
            if (time >= 10) return DownloadError.DownloadFailed;
            byte[] pack = new byte[communManager.DOWNLOAD_MAX_DATALEN];
            int len = data.Length / communManager.DOWNLOAD_MAX_DATALEN;
            int rem = data.Length % communManager.DOWNLOAD_MAX_DATALEN;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < communManager.DOWNLOAD_MAX_DATALEN; j++)
                    pack[j] = data[i * communManager.DOWNLOAD_MAX_DATALEN + j];
                command = new DownloadTypeData(i, pack, funcCode);
                for (time = 0; time < 3 && !communManager.DownloadHandle(command);) time++;
                if (time >= 3) return DownloadError.DownloadFailed;
            }
            if (rem > 0)
            {
                pack = new byte[rem];
                for (int j = 0; j < rem; j++)
                    pack[j] = data[len * communManager.DOWNLOAD_MAX_DATALEN + j];
                command = new DownloadTypeData(len, pack, funcCode);
                for (time = 0; time < 3 && !communManager.DownloadHandle(command);) time++;
                if (time >= 3) return DownloadError.DownloadFailed;
            }
            command = new DownloadTypeOver(funcCode);
            for (time = 0; time < 10 && !communManager.DownloadHandle(command);)
            {
                Thread.Sleep(500);
                time++;
            }
            if (time >= 10) return DownloadError.DownloadFailed;
            return DownloadError.None;
        }
        #endregion

        #region tools
        public static bool CheckOption(int oldoption, int newoption)
        {
            return (oldoption & CommunicationDataDefine.OPTION_INITIALIZE) == (newoption & CommunicationDataDefine.OPTION_INITIALIZE);
        }
        #endregion

        #endregion
    }
}
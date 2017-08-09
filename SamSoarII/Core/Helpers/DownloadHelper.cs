using SamSoarII.Core.Communication;
using SamSoarII.Shell.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SamSoarII.Utility;
using SamSoarII.Core.Models;
using SamSoarII.Global;

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
            // 初始化
            dtIcon = new List<byte>();
            dtConfig = new List<byte>();
            dtModbus = new List<byte>();
            dtTable = new List<byte>();
            dtBlock = new List<byte>();
            // 条形码
            
            // 配置
            Write(project.PARAProj);
            // Modbus
            Write(project.Modbus);
            // Table表

            // Block表

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

        #region Initialize Config data
        private static DownloadError DownloadConfigExecute()
        {
            DownloadError ret = DownloadError.None;
            return ret;
        }

        static private void Write(ProjectPropertyParams pparams)
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
        private static DownloadError DownloadModbusTableExecute()
        {
            DownloadError ret = DownloadError.None;
            return ret;
        }
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
        public static DownloadError DownloadExecute(CommunicationManager communManager)
        {
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
            //先下载Bin文件
            DownloadError ret = DownloadError.None;
            ret = DownloadBinExecute(communManager);
            if (ret != DownloadError.None)
                return ret;
            //再下载用于上载的XML压缩文件（包括程序，注释（可选），软元件表（可选）等）
            ret = DownloadProjExecute(communManager,communManager.IFParent.MDProj.PARAProj.PARACom.DownloadOption);
            if (ret != DownloadError.None)
                return ret;
            return DownloadError.None;
        }

        #region Bin download
        private static DownloadError DownloadBinExecute(CommunicationManager communManager)
        {
            int time = 0;
            ICommunicationCommand command = new SwitchToIAPCommand();
            for (time = 0; time < 10 && !communManager.DownloadHandle(command);) time++;
            if (time >= 10) return DownloadError.DownloadFailed;
            command = new IAPDESKEYCommand(communManager.ExecLen);
            for (time = 0; time < 10 && !communManager.DownloadHandle(command);) time++;
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
            if (!communManager.DownloadHandle(new BinFinishedCommand()))
                return DownloadError.DownloadFailed;
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
            communManager.IFParent.MDProj.SaveToPLC(_filename);
            //返回生成的压缩文件全名
            string genFile = FileHelper.CompressFile(_filename);
            try
            {
                return _DownloadProj(communManager,FileHelper.GenerateBinaryFile(genFile));
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

        private static DownloadError _DownloadProj(CommunicationManager communManager,byte[] tempdata)
        {
            if (tempdata.Length == 0) return DownloadError.None;
            //传送前，须在传送数据前加上4字节的数据长度，供上载时使用。
            byte[] data = ValueConverter.GetBytes((uint)tempdata.Length,true);
            data = data.Concat(tempdata).ToArray();
            int time = 0;
            ICommunicationCommand command = new DownloadTypeStart(CommunicationDataDefine.CMD_DOWNLOAD_PRO,data.Length);
            for (time = 0; time < 10 && !communManager.DownloadHandle(command);) time++;
            if (time >= 10) return DownloadError.DownloadFailed;
            byte[] pack = new byte[communManager.DOWNLOAD_MAX_DATALEN];
            int len = data.Length / communManager.DOWNLOAD_MAX_DATALEN;
            int rem = data.Length % communManager.DOWNLOAD_MAX_DATALEN;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < communManager.DOWNLOAD_MAX_DATALEN; j++)
                    pack[j] = data[i * communManager.DOWNLOAD_MAX_DATALEN + j];
                command = new DownloadTypeData(i, pack, CommunicationDataDefine.CMD_DOWNLOAD_PRO);
                for (time = 0; time < 3 && !communManager.DownloadHandle(command);) time++;
                if (time >= 3) return DownloadError.DownloadFailed;
            }
            if (rem > 0)
            {
                pack = new byte[rem];
                for (int j = 0; j < rem; j++)
                    pack[j] = data[len * communManager.DOWNLOAD_MAX_DATALEN + j];
                command = new DownloadTypeData(len, pack, CommunicationDataDefine.CMD_DOWNLOAD_PRO);
                for (time = 0; time < 3 && !communManager.DownloadHandle(command);) time++;
                if (time >= 3) return DownloadError.DownloadFailed;
            }
            if (!communManager.DownloadHandle(new DownloadTypeOver(CommunicationDataDefine.CMD_DOWNLOAD_PRO)))
                return DownloadError.DownloadFailed;
            return DownloadError.None;
        }
        #endregion
        
        #region Modbus download
        #endregion

        #region PlsTable download
        private static DownloadError DownloadPlsTableExecute()
        {
            DownloadError ret = DownloadError.None;
            return ret;
        }
        #endregion

        #region PlsBlock download
        private static DownloadError DownloadPlsBlockExecute()
        {
            DownloadError ret = DownloadError.None;
            return ret;
        }
        #endregion
        
        #region Config download
        #endregion
        
        #endregion
    }
}
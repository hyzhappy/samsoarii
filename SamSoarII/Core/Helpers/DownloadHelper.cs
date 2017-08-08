using SamSoarII.Core.Communication;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        private CommunicationManager communManager;
        public DownloadHelper(CommunicationManager communicationManager)
        {
            communManager = communicationManager;
        }
        //下载前用于获取当前PLC信息(PLC型号，PLC运行状态，PLC当前程序，是否需要下载密码等)
        private PLCMessage plcMessage;
        public DownloadError DownloadExecute()
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
            ret = DownloadBinExecute();
            if (ret != DownloadError.None)
                return ret;
            //再下载用于上载的XML压缩文件（包括程序，注释（可选），软元件表（可选）等）
            ret = DownloadProjExecute(communManager.IFParent.MDProj.PARAProj.PARACom.DownloadOption);
            if (ret != DownloadError.None)
                return ret;
            return DownloadError.None;
        }
        private DownloadError DownloadBinExecute()
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
        
        private DownloadError DownloadPlsTableExecute()
        {
            DownloadError ret = DownloadError.None;
            return ret;
        }

        private DownloadError DownloadPlsBlockExecute()
        {
            DownloadError ret = DownloadError.None;
            return ret;
        }
        //配置信息
        private DownloadError DownloadConfigExecute()
        {
            DownloadError ret = DownloadError.None;
            return ret;
        }
        //modbus表
        private DownloadError DownloadModbusTableExecute()
        {
            DownloadError ret = DownloadError.None;
            return ret;
        }
        //工程文件（包括程序，注释（可选），软元件表（可选）等）
        private DownloadError DownloadProjExecute(int flag)
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
            communManager.IFParent.MDProj.GenerateFileByFlag(flag,_filename);
            //返回生成的压缩文件全名
            string genFile = FileHelper.CompressFile(_filename);
            try
            {
                DownloadError ret = DownloadError.None;
                return _DownloadProj(FileHelper.GenerateBinaryFile(genFile));
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

        private DownloadError _DownloadProj(byte[] tempdata)
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
    }
}

using SamSoarII.Core.Communication;
using SamSoarII.Shell.Dialogs;
using System;
using System.Collections.Generic;
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
            if(ret != DownloadError.None)
                return ret;

            return DownloadError.None;
        }
        private DownloadError DownloadBinExecute()
        {
            if (!communManager.DownloadHandle(new SwitchToIAPCommand()))
                return DownloadError.DownloadFailed;
            if (!communManager.DownloadHandle(new IAPDESKEYCommand(communManager.ExecLen)))
                return DownloadError.DownloadFailed;
            byte[] data = communManager.ExecData.ToArray();
            byte[] pack = new byte[communManager.DOWNLOAD_MAX_DATALEN];
            int len = data.Length / communManager.DOWNLOAD_MAX_DATALEN;
            int rem = data.Length % communManager.DOWNLOAD_MAX_DATALEN;
            TransportBinCommand TBCommand;
            int time = 0;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < communManager.DOWNLOAD_MAX_DATALEN; j++)
                    pack[j] = data[i * communManager.DOWNLOAD_MAX_DATALEN + j];
                TBCommand = new TransportBinCommand(i, pack);
                for (time = 0; time < 3 && !communManager.DownloadHandle(TBCommand);) time++;
                if (time >= 3) return DownloadError.DownloadFailed;
            }
            if (rem > 0)
            {
                pack = new byte[rem];
                for (int j = 0; j < rem; j++)
                    pack[j] = data[len * communManager.DOWNLOAD_MAX_DATALEN + j];
                TBCommand = new TransportBinCommand(len, pack);
                for (time = 0; time < 3 && !communManager.DownloadHandle(TBCommand);) time++;
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

        private DownloadError DownloadConfigExecute()
        {
            DownloadError ret = DownloadError.None;
            return ret;
        }
        private DownloadError DownloadModbusTableExecute()
        {
            DownloadError ret = DownloadError.None;
            return ret;
        }

        private DownloadError DownloadProjExecute()
        {
            DownloadError ret = DownloadError.None;
            return ret;
        }
    }
}

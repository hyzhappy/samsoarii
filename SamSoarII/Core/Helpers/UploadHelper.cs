using SamSoarII.Core.Communication;
using SamSoarII.Shell.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Helpers
{
    public enum UploadError
    {
        Cancel,
        None,
        CommuicationFailed,
        UploadFailed
    }
    public static class UploadHelper
    {
        #region option
        private static int uploadoption;
        public static int UploadOption
        {
            get { return uploadoption; }
            set { uploadoption = value; }
        }
        public static bool IsUploadProgram
        {
            get { return (uploadoption & CommunicationDataDefine.OPTION_PROGRAM) != 0; }
        }
        public static bool IsUploadComment
        {
            get { return (uploadoption & CommunicationDataDefine.OPTION_COMMENT) != 0; }
        }
        public static bool IsUploadInitialize
        {
            get { return (uploadoption & CommunicationDataDefine.OPTION_INITIALIZE) != 0; }
        }
        public static bool IsUploadSetting
        {
            get { return (uploadoption & CommunicationDataDefine.OPTION_SETTING) != 0; }
        }
        #endregion

        #region data
        /// <summary>
        /// 上载回来的工程数据,（包括工程，注释，软元件初始化）
        /// </summary>
        private static List<byte> upProj;
        /// <summary> 条形码 </summary>
        private static List<byte> upIcon;
        /// <summary> 配置 </summary>
        private static List<byte> upConfig;
        /// <summary> Modbus表 </summary>
        private static List<byte> upModbus;
        /// <summary> Table表 </summary>
        private static List<byte> upTable;
        /// <summary> Block表 </summary>
        private static List<byte> upBlock;
        #endregion

        //上载前用于获取当前PLC信息(PLC型号，PLC运行状态，PLC当前程序，是否需要上载密码等)
        private static PLCMessage plcMessage;
        public static UploadError UploadExecute(CommunicationManager communManager)
        {
            //首先通信测试获取底层PLC的状态
            CommunicationTestCommand CTCommand = new CommunicationTestCommand();
            if (!communManager.DownloadHandle(CTCommand))
                return UploadError.CommuicationFailed;
            else plcMessage = new PLCMessage(CTCommand);
            //首先判断PLC运行状态,为Iap时需要切换到App模式
            if (plcMessage.RunStatus == RunStatus.Iap)
            {
                if (!communManager.DownloadHandle(new SwitchPLCStatusCommand()))
                    return UploadError.CommuicationFailed;
            }
            //验证是否需要上载密码
            if (plcMessage.IsUPNeed)
            {

            }
            UploadError ret = UploadError.None;

            if (IsUploadProgram)
            {
                //上载经过压缩的XML文件（包括程序，注释（可选），软元件表（可选）等）
                ret = UploadProjExecute(communManager);
                if (ret != UploadError.None)
                    return ret;

                //上载Modbus表格
                ret = UploadModbusTableExecute(communManager);
                if (ret != UploadError.None)
                    return ret;

                //上载 PlsTable
                ret = UploadPlsTableExecute(communManager);
                if (ret != UploadError.None)
                    return ret;

                //上载 PlsBlock
                ret = UploadPlsBlockExecute(communManager);
                if (ret != UploadError.None)
                    return ret;
            }
            //下载 Config
            if (IsUploadSetting)
            {
                ret = UploadConfigExecute(communManager);
                if (ret != UploadError.None)
                    return ret;
            }
            return ret;
        }

        private static UploadError UploadProjExecute(CommunicationManager communManager)
        {
            return _UploadHandle(communManager, upProj, CommunicationDataDefine.CMD_UPLOAD_PRO);
        }

        private static UploadError UploadModbusTableExecute(CommunicationManager communManager)
        {
            return _UploadHandle(communManager, upModbus, CommunicationDataDefine.CMD_UPLOAD_MODBUSTABLE);
        }

        private static UploadError UploadPlsTableExecute(CommunicationManager communManager)
        {
            return _UploadHandle(communManager, upTable, CommunicationDataDefine.CMD_UPLOAD_PLSTABLE);
        }

        private static UploadError UploadPlsBlockExecute(CommunicationManager communManager)
        {
            return _UploadHandle(communManager, upBlock, CommunicationDataDefine.CMD_UPLOAD_PLSBLOCK);
        }

        private static UploadError UploadConfigExecute(CommunicationManager communManager)
        {
            return _UploadHandle(communManager, upConfig, CommunicationDataDefine.CMD_UPLOAD_CONFIG);
        }

        private static UploadError _UploadHandle(CommunicationManager communManager,List<byte> desdata,byte funcCode)
        {
            return UploadError.None;
        }
    }
}

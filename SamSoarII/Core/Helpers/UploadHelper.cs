using SamSoarII.Core.Communication;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
            if (!communManager.CommunicationHandle(CTCommand))
                return UploadError.CommuicationFailed;
            else plcMessage = new PLCMessage(CTCommand);
            //首先判断PLC运行状态,为Iap时需要切换到App模式
            if (plcMessage.RunStatus == RunStatus.Iap)
            {
                if (!communManager.CommunicationHandle(new SwitchPLCStatusCommand()))
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

        private static UploadError _UploadHandle(CommunicationManager communManager,IEnumerable<byte> desdata,byte funcCode)
        {
            if (desdata == null) desdata = new List<byte>();
            int time = 0;//记录重传次数
            ICommunicationCommand command = new UploadTypeStart(funcCode);
            for (time = 0; time < 10 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 10) return UploadError.UploadFailed;
            Dictionary<int, byte[]> data = new Dictionary<int, byte[]>();
            if (command.RecvDataLen > 0)
            {
                int len = command.RecvDataLen / communManager.COMMU_MAX_DATALEN;
                int rem = command.RecvDataLen % communManager.COMMU_MAX_DATALEN;
                if (rem > 0) len++;
                for (int i = 0; i < len; i++)
                {
                    command = new UploadTypeData(funcCode,i);
                    for (time = 0; time < 3 && !communManager.CommunicationHandle(command);) time++;
                    if (time >= 3) return UploadError.UploadFailed;
                    data.Add(i,GetRetData(command.RetData));
                }
            }
            command = new UploadTypeOver(funcCode);
            for (time = 0; time < 10 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 10) return UploadError.UploadFailed;
            foreach (var kvPair in data)
                desdata = desdata.Concat(kvPair.Value);
            if(funcCode == CommunicationDataDefine.CMD_UPLOAD_PRO)
            {
                //略去前面4字节的长度
                desdata = desdata.Skip(4);
                //将数据解密
                desdata = CommandHelper.Decrypt(desdata.Count(),desdata.ToArray());
            }
            return UploadError.None;
        }
        
        private static byte[] GetRetData(byte[] data)
        {
            int len = ValueConverter.GetValueByBytes(data[1],data[2]);
            len -= 8;
            byte[] retData = new byte[len];
            for (int i = 0; i < len; i++)
            {
                retData[i] = data[i + 6];
            }
            return retData;
        }
    }
}

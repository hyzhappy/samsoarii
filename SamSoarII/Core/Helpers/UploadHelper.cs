using SamSoarII.Core.Communication;
using SamSoarII.Core.Models;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.IO;
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
        private static int uploadoption = 9;
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

        public static bool HasConfig { get { return ProjectParams != null; } }

        private static ProjectPropertyParams projectParams;
        public static ProjectPropertyParams ProjectParams
        {
            get { return projectParams; }
            private set { projectParams = value; }
        }
        #endregion
        
        public static UploadError UploadExecute(CommunicationManager communManager)
        {
            //首先通信测试获取底层PLC的状态
            CommunicationTestCommand CTCommand = new CommunicationTestCommand();
            if (!communManager.CommunicationHandle(CTCommand))
                return UploadError.CommuicationFailed;
            else communManager.PLCMessage = new PLCMessage(CTCommand);
            //首先判断PLC运行状态,为Iap时需要切换到App模式
            if (communManager.PLCMessage.RunStatus == RunStatus.Iap)
            {
                int time;
                for (time = 0; time < 5 && !communManager.CommunicationHandle(new SwitchPLCStatusCommand());)
                {
                    Thread.Sleep(200);
                    time++;
                }
                if (time >= 5) return UploadError.UploadFailed;
            }
            //验证是否需要上载密码
            if (communManager.PLCMessage.IsUPNeed)
            {
                bool retp = false;
                LocalizedMessageResult retcl = LocalizedMessageResult.None;
                PasswordDialog dialog = new PasswordDialog();

                dialog.EnsureButtonClick += (sender, e) =>
                {
                    if (dialog.Password.Length > 12 || dialog.Password.Length < 5)
                        LocalizedMessageBox.Show(Properties.Resources.Password_Length_Error);
                    else
                    {
                        int time = 0;
                        ICommunicationCommand command = new PasswordCheckCommand(CommunicationDataDefine.CMD_PASSWD_UPLOAD, dialog.Password);
                        for (time = 0; time < 3 && !communManager.CommunicationHandle(command);)
                        {
                            Thread.Sleep(200);
                            time++;
                        }
                        if (time >= 3) retp = false;
                        else
                        {
                            retp = true;
                            dialog.Close();
                        }
                    }
                };

                dialog.Closing += (sender, e) =>
                {
                    if (!retp)
                    {
                        retcl = LocalizedMessageBox.Show(string.Format("{0}{1}", Properties.Resources.Dialog_Closing, Properties.Resources.MainWindow_Upload), LocalizedMessageButton.OKCancel);
                        if (retcl == LocalizedMessageResult.No) e.Cancel = true;
                    }
                    else retcl = LocalizedMessageResult.None;
                };

                dialog.ShowDialog();
                if (retcl == LocalizedMessageResult.Yes) return UploadError.Cancel;
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
            return _UploadHandle(communManager, ref upProj, CommunicationDataDefine.CMD_UPLOAD_PRO);
        }

        private static UploadError UploadModbusTableExecute(CommunicationManager communManager)
        {
            return _UploadHandle(communManager, ref upModbus, CommunicationDataDefine.CMD_UPLOAD_MODBUSTABLE);
        }

        private static UploadError UploadPlsTableExecute(CommunicationManager communManager)
        {
            return _UploadHandle(communManager, ref upTable, CommunicationDataDefine.CMD_UPLOAD_PLSTABLE);
        }

        private static UploadError UploadPlsBlockExecute(CommunicationManager communManager)
        {
            return _UploadHandle(communManager, ref upBlock, CommunicationDataDefine.CMD_UPLOAD_PLSBLOCK);
        }

        private static UploadError UploadConfigExecute(CommunicationManager communManager)
        {
            return _UploadHandle(communManager, ref upConfig, CommunicationDataDefine.CMD_UPLOAD_CONFIG);
        }

        private static UploadError _UploadHandle(CommunicationManager communManager,ref List<byte> desdata,byte funcCode)
        {
            if (desdata == null) desdata = new List<byte>();
            else if (desdata.Count() > 0) desdata.Clear();
            int time = 0;//记录重传次数
            ICommunicationCommand command = new UploadTypeStart(funcCode);
            for (time = 0; time < 5 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 5) return UploadError.UploadFailed;
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
            for (time = 0; time < 5 && !communManager.CommunicationHandle(command);)
            {
                Thread.Sleep(200);
                time++;
            }
            if (time >= 5) return UploadError.UploadFailed;
            foreach (var kvPair in data)
                desdata.AddRange(kvPair.Value);
            if(funcCode == CommunicationDataDefine.CMD_UPLOAD_PRO)
            {
                //略去前面4字节的长度
                desdata = desdata.ToArray().Skip(4).ToList();
                //将数据解密
                desdata = CommandHelper.Decrypt(desdata.Count(),desdata.ToArray()).ToList();
            }
            else if (funcCode == CommunicationDataDefine.CMD_UPLOAD_CONFIG)
                LoadConfig();
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

        public static bool LoadProjByUploadData(InteractionFacade ifParent, string fullFileName)
        {
            try
            {
                string dir = Directory.GetParent(fullFileName).FullName;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                FileHelper.GenerateBinaryFile(fullFileName, upProj.ToArray());
                FileHelper.DecompressFile(fullFileName, dir);
                foreach (var file in Directory.GetFiles(dir))
                {
                    if (file.EndsWith(FileHelper.NewFileExtension))
                    {
                        ifParent.LoadProject(file,true);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                Directory.Delete(Directory.GetParent(fullFileName).FullName, true);
            }
            return true;
        }

        private static void LoadConfig()
        {
            ProjectPropertyParams projectParams = new ProjectPropertyParams(null);
            CommunicationInterfaceParams com232params = projectParams.PARACom232;
            CommunicationInterfaceParams com485params = projectParams.PARACom485;
            USBCommunicationParams usbparams = projectParams.PARAUsb;
            PasswordParams pwparams = projectParams.PARAPassword;
            FilterParams ftparams = projectParams.PARAFilter;
            HoldingSectionParams hsparams = projectParams.PARAHolding;
            AnalogQuantityParams aqparams = projectParams.PARAAnalog;
            ExpansionModuleParams emparams = projectParams.PARAExpansion;
            try
            {
                //跳过前面6字节的长度
                int cursor = 6, value = 0;
                byte[] pack;
                com232params.BaudRateIndex = upConfig[cursor++];
                com232params.DataBitIndex = 0;
                cursor++;
                com232params.StopBitIndex = upConfig[cursor++];
                com232params.CheckCodeIndex = upConfig[cursor++];

                com485params.BaudRateIndex = upConfig[cursor++];
                com485params.DataBitIndex = 0;
                cursor++;
                com485params.StopBitIndex = upConfig[cursor++];
                com485params.CheckCodeIndex = upConfig[cursor++];

                projectParams.StationNumber = upConfig[cursor++];
                usbparams.Timeout = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);

                pwparams.PWENUpload = upConfig[cursor++] == 1;
                value = upConfig[cursor++];
                pack = new byte[value];
                for (int i = 0; i < value; i++)
                {
                    pack[i] = upConfig[cursor++];
                }
                pwparams.PWUpload = StringParse(pack);

                com232params.ComType = (CommunicationInterfaceParams.ComTypes)Enum.ToObject(typeof(CommunicationInterfaceParams.ComTypes), upConfig[cursor++]);
                com485params.ComType = (CommunicationInterfaceParams.ComTypes)Enum.ToObject(typeof(CommunicationInterfaceParams.ComTypes), upConfig[cursor++]);

                pwparams.PWENDownload = upConfig[cursor++] == 1;
                value = upConfig[cursor++];
                pack = new byte[value];
                for (int i = 0; i < value; i++)
                {
                    pack[i] = upConfig[cursor++];
                }
                pwparams.PWDownload = StringParse(pack);

                pwparams.PWENMonitor = upConfig[cursor++] == 1;
                value = upConfig[cursor++];
                pack = new byte[value];
                for (int i = 0; i < value; i++)
                {
                    pack[i] = upConfig[cursor++];
                }
                pwparams.PWMonitor = StringParse(pack);

                ftparams.IsChecked = upConfig[cursor++] == 1;
                value = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                ftparams.FilterTimeIndex = (int)Math.Log(value, 2) - 1;

                hsparams.MStartAddr = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                hsparams.MLength = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                hsparams.DStartAddr = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                hsparams.DLength = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                hsparams.SStartAddr = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                hsparams.SLength = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                hsparams.CVStartAddr = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                hsparams.CVLength = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);

                cursor += 4;

                aqparams.IP_Channel_CB_Enabled1 = upConfig[cursor++] == 1;
                aqparams.IP_Mode_Index1 = upConfig[cursor++];
                aqparams.IP_EndRange1 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.IP_StartRange1 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.IP_SampleTime_Index1 = upConfig[cursor++];
                aqparams.SampleValue1 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                aqparams.IP_Channel_CB_Enabled2 = upConfig[cursor++] == 1;
                aqparams.IP_Mode_Index2 = upConfig[cursor++];
                aqparams.IP_EndRange2 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.IP_StartRange2 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.IP_SampleTime_Index2 = upConfig[cursor++];
                aqparams.SampleValue2 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                aqparams.IP_Channel_CB_Enabled3 = upConfig[cursor++] == 1;
                aqparams.IP_Mode_Index3 = upConfig[cursor++];
                aqparams.IP_EndRange3 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.IP_StartRange3 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.IP_SampleTime_Index3 = upConfig[cursor++];
                aqparams.SampleValue3 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                aqparams.IP_Channel_CB_Enabled4 = upConfig[cursor++] == 1;
                aqparams.IP_Mode_Index4 = upConfig[cursor++];
                aqparams.IP_EndRange4 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.IP_StartRange4 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.IP_SampleTime_Index4 = upConfig[cursor++];
                aqparams.SampleValue4 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                cursor += 4;

                aqparams.OP_Channel_CB_Enabled1 = upConfig[cursor++] == 1;
                aqparams.OP_EndRange1 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.OP_StartRange1 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.OP_Mode_Index1 = upConfig[cursor++];

                aqparams.OP_Channel_CB_Enabled2 = upConfig[cursor++] == 1;
                aqparams.OP_EndRange2 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.OP_StartRange2 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.OP_Mode_Index2 = upConfig[cursor++];

                aqparams.OP_Channel_CB_Enabled3 = upConfig[cursor++] == 1;
                aqparams.OP_EndRange3 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.OP_StartRange3 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.OP_Mode_Index3 = upConfig[cursor++];

                aqparams.OP_Channel_CB_Enabled4 = upConfig[cursor++] == 1;
                aqparams.OP_EndRange4 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.OP_StartRange4 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                aqparams.OP_Mode_Index4 = upConfig[cursor++];

                emparams.UseExpansionModule = upConfig[cursor++] == 1;

                emparams.ExpansionUnitParams[0].UseModule = upConfig[cursor++] == 1;
                emparams.ExpansionUnitParams[0].ModuleTypeIndex = upConfig[cursor++];
                emparams.ExpansionUnitParams[1].UseModule = upConfig[cursor++] == 1;
                emparams.ExpansionUnitParams[1].ModuleTypeIndex = upConfig[cursor++];
                emparams.ExpansionUnitParams[2].UseModule = upConfig[cursor++] == 1;
                emparams.ExpansionUnitParams[2].ModuleTypeIndex = upConfig[cursor++];
                emparams.ExpansionUnitParams[3].UseModule = upConfig[cursor++] == 1;
                emparams.ExpansionUnitParams[3].ModuleTypeIndex = upConfig[cursor++];
                emparams.ExpansionUnitParams[4].UseModule = upConfig[cursor++] == 1;
                emparams.ExpansionUnitParams[4].ModuleTypeIndex = upConfig[cursor++];
                emparams.ExpansionUnitParams[5].UseModule = upConfig[cursor++] == 1;
                emparams.ExpansionUnitParams[5].ModuleTypeIndex = upConfig[cursor++];
                emparams.ExpansionUnitParams[6].UseModule = upConfig[cursor++] == 1;
                emparams.ExpansionUnitParams[6].ModuleTypeIndex = upConfig[cursor++];
                emparams.ExpansionUnitParams[7].UseModule = upConfig[cursor++] == 1;
                emparams.ExpansionUnitParams[7].ModuleTypeIndex = upConfig[cursor++];

                com232params.DataBitIndex = upConfig[cursor++];
                com485params.DataBitIndex = upConfig[cursor++];

                hsparams.NotClear = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]) == 1;

                com232params.Timeout = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                com485params.Timeout = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);

                cursor += 11;
                emparams.ExpansionUnitParams[0].FilterTime_Index = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                cursor += 11;
                emparams.ExpansionUnitParams[1].FilterTime_Index = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                cursor += 11;
                emparams.ExpansionUnitParams[2].FilterTime_Index = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                cursor += 11;
                emparams.ExpansionUnitParams[3].FilterTime_Index = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                cursor += 11;
                emparams.ExpansionUnitParams[4].FilterTime_Index = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                cursor += 11;
                emparams.ExpansionUnitParams[5].FilterTime_Index = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                cursor += 11;
                emparams.ExpansionUnitParams[6].FilterTime_Index = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                cursor += 11;
                emparams.ExpansionUnitParams[7].FilterTime_Index = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);

                cursor += 4;

                aqparams.IP_Channel_CB_Enabled5 = upConfig[cursor++] == 1;
                aqparams.IP_Mode_Index5 = upConfig[cursor++];
                cursor += 4;
                aqparams.IP_SampleTime_Index5 = upConfig[cursor++];
                aqparams.SampleValue5 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                aqparams.IP_Channel_CB_Enabled6 = upConfig[cursor++] == 1;
                aqparams.IP_Mode_Index6 = upConfig[cursor++];
                cursor += 4;
                aqparams.IP_SampleTime_Index6 = upConfig[cursor++];
                aqparams.SampleValue6 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                aqparams.IP_Channel_CB_Enabled7 = upConfig[cursor++] == 1;
                aqparams.IP_Mode_Index7 = upConfig[cursor++];
                cursor += 4;
                aqparams.IP_SampleTime_Index7 = upConfig[cursor++];
                aqparams.SampleValue7 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                aqparams.IP_Channel_CB_Enabled8 = upConfig[cursor++] == 1;
                aqparams.IP_Mode_Index8 = upConfig[cursor++];
                cursor += 4;
                aqparams.IP_SampleTime_Index8 = upConfig[cursor++];
                aqparams.SampleValue8 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                cursor += 16;

                for (int i = 0; i < 8; i++)
                {
                    cursor += 4;

                    emparams.ExpansionUnitParams[i].IP_Channel_CB_Enabled1 = upConfig[cursor++] == 1;
                    emparams.ExpansionUnitParams[i].IP_Mode_Index1 = upConfig[cursor++];
                    emparams.ExpansionUnitParams[i].IP_EndRange1 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    emparams.ExpansionUnitParams[i].IP_StartRange1 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    emparams.ExpansionUnitParams[i].IP_SampleTime_Index1 = (int)Math.Log(upConfig[cursor++], 2) - 2;
                    emparams.ExpansionUnitParams[i].SampleValue1 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                    emparams.ExpansionUnitParams[i].IP_Channel_CB_Enabled2 = upConfig[cursor++] == 1;
                    emparams.ExpansionUnitParams[i].IP_Mode_Index2 = upConfig[cursor++];
                    emparams.ExpansionUnitParams[i].IP_EndRange2 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    emparams.ExpansionUnitParams[i].IP_StartRange2 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    emparams.ExpansionUnitParams[i].IP_SampleTime_Index2 = (int)Math.Log(upConfig[cursor++], 2) - 2;
                    emparams.ExpansionUnitParams[i].SampleValue2 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                    emparams.ExpansionUnitParams[i].IP_Channel_CB_Enabled3 = upConfig[cursor++] == 1;
                    emparams.ExpansionUnitParams[i].IP_Mode_Index3 = upConfig[cursor++];
                    emparams.ExpansionUnitParams[i].IP_EndRange3 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    emparams.ExpansionUnitParams[i].IP_StartRange3 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    emparams.ExpansionUnitParams[i].IP_SampleTime_Index3 = (int)Math.Log(upConfig[cursor++], 2) - 2;
                    emparams.ExpansionUnitParams[i].SampleValue3 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                    emparams.ExpansionUnitParams[i].IP_Channel_CB_Enabled4 = upConfig[cursor++] == 1;
                    emparams.ExpansionUnitParams[i].IP_Mode_Index4 = upConfig[cursor++];
                    emparams.ExpansionUnitParams[i].IP_EndRange4 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    emparams.ExpansionUnitParams[i].IP_StartRange4 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    emparams.ExpansionUnitParams[i].IP_SampleTime_Index4 = (int)Math.Log(upConfig[cursor++], 2) - 2;
                    emparams.ExpansionUnitParams[i].SampleValue4 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]).ToString();

                    cursor += 4;

                    emparams.ExpansionUnitParams[i].OP_Channel_CB_Enabled1 = upConfig[cursor++] == 1;
                    emparams.ExpansionUnitParams[i].OP_Mode_Index1 = upConfig[cursor++];
                    emparams.ExpansionUnitParams[i].OP_EndRange1 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    emparams.ExpansionUnitParams[i].OP_StartRange1 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    cursor += 3;

                    emparams.ExpansionUnitParams[i].OP_Channel_CB_Enabled2 = upConfig[cursor++] == 1;
                    emparams.ExpansionUnitParams[i].OP_Mode_Index2 = upConfig[cursor++];
                    emparams.ExpansionUnitParams[i].OP_EndRange2 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    emparams.ExpansionUnitParams[i].OP_StartRange2 = ValueConverter.GetValueByBytes(upConfig[cursor++], upConfig[cursor++]);
                    cursor += 3;

                    cursor += 18;
                }
            }
            catch (Exception)
            {
                UploadHelper.projectParams = null;
            }
            UploadHelper.projectParams = projectParams;
        }

        private static string StringParse(params byte[] data)
        {
            char[] pack = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                pack[i] = (char)data[i];
            }
            return new string(pack);
        }
    }
}

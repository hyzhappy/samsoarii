using SamSoarII.PLCDevice;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Communication
{
    public enum RunStatus
    {
        Run,
        Stop,
        Iap
    }
    public enum PortType
    {
        Serial232,
        Serial485,
        USB,
        Unknown
    }
    public class PLCMessage
    {
        //PLC当前运行状态
        public RunStatus RunStatus;

        //版本号
        public int Ver1;
        public int Ver2;
        public int Ver3;

        //站号
        public int StationNumber;

        //PLC类型
        public PLCDeviceType PLCType;

        //通信端口类型
        public PortType PortType;

        //是否需要上载密码
        public bool IsUPNeed;

        //是否需要下载密码
        public bool IsDPNeed;

        //是否需要监视密码
        public bool IsMPNeed;
        public PLCMessage(CommunicationTestCommand command)
        {
            byte[] data = new byte[ValueConverter.GetValueByBytes(command.RetData[1], command.RetData[2]) - 7];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = command.RetData[i + 5];
            }
            int cursor = 0;
            RunStatus = GetRunStatus(data[cursor++]);
            Ver1 = ValueConverter.GetValueByBytes(data[cursor++], data[cursor++]);
            Ver2 = ValueConverter.GetValueByBytes(data[cursor++], data[cursor++]);
            Ver3 = ValueConverter.GetValueByBytes(data[cursor++], data[cursor++]);
            StationNumber = data[cursor++];
            //PLCType = PLCDeviceType.FGs_16MR_A;
            cursor += 2;
            IsUPNeed = data[cursor++] == 1;
            IsDPNeed = data[cursor++] == 1;
            IsMPNeed = data[cursor++] == 1;
            PortType = GetPortType(data[cursor]);
        }

        private RunStatus GetRunStatus(byte data)
        {
            switch (data)
            {
                case 0x00:
                    return RunStatus.Stop;
                case 0x01:
                    return RunStatus.Run;
                case 0x02:
                    return RunStatus.Iap;
                default:
                    return RunStatus.Stop;
            }
        }
        private PortType GetPortType(byte data)
        {
            switch (data)
            {
                case 0x01:
                    return PortType.Serial232;
                case 0x02:
                    return PortType.Serial485;
                case 0x03:
                    return PortType.USB;
                default:
                    return PortType.Unknown;
            }
        }
    }
}

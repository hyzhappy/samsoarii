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
        public byte StationNumber;

        //PLC类型
        public object PLCType;

        //通信端口类型
        public PortType PortType;

        //是否需要上载密码
        public bool IsUPNeed;

        //是否需要下载密码
        public bool IsDPNeed;

        //是否需要监视密码
        public bool IsMPNeed;

        //梯形图HashCode
        public byte[] HashCode;

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
            PLCType = GetPLCType(ValueConverter.GetValueByBytes(data[cursor++], data[cursor++]));
            IsUPNeed = data[cursor++] == 1;
            IsDPNeed = data[cursor++] == 1;
            IsMPNeed = data[cursor++] == 1;
            PortType = GetPortType(data[cursor++]);
            HashCode = new byte[data.Length - cursor];
            for (int i = 0; i < HashCode.Length; i++)
                HashCode[i] = data[cursor++];
        }

        private static RunStatus GetRunStatus(byte data)
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
        private static PortType GetPortType(byte data)
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

        private static object GetPLCType(int value)
        {
            switch (value)
            {
                case 0x0000:
                    return PLC_FGs_Type.FGs_32MR_D;
                case 0x0001:
                    return PLC_FGs_Type.FGs_32MR_A;
                case 0x0002:
                    return PLC_FGs_Type.FGs_32MT_D;
                case 0x0003:
                    return PLC_FGs_Type.FGs_32MT_A;
                case 0x0100:
                    return PLC_FGs_Type.FGs_16MR_D;
                case 0x0101:
                    return PLC_FGs_Type.FGs_16MR_A;
                case 0x0102:
                    return PLC_FGs_Type.FGs_16MT_D;
                case 0x0103:
                    return PLC_FGs_Type.FGs_16MT_A;
                case 0x0200:
                    return PLC_FGs_Type.FGs_64MR_D;
                case 0x0201:
                    return PLC_FGs_Type.FGs_64MR_A;
                case 0x0202:
                    return PLC_FGs_Type.FGs_64MT_D;
                case 0x0203:
                    return PLC_FGs_Type.FGs_64MT_A;
                case 0x0204:
                    return PLC_FGs_Type.FGm_64MT_A;
                case 0x0205:
                    return PLC_FGs_Type.FGm_48MT_A;
                case 0x0206:
                    return PLC_FGs_Type.FGm_32MT_A;
                default:
                    return PLC_FGs_Type.FGs_16MT_A;
            }
        }
    }
}

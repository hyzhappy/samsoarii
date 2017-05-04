using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Communication.Command
{
    public class GeneralWriteCommand : ICommunicationCommand
    {
        private const byte slaveNum = CommunicationDataDefine.SLAVE_ADDRESS;
        private const byte commandType = CommunicationDataDefine.FGS_WRITE;
        private byte addrTypeNum;
        private byte addrType1;
        private byte length1;
        private byte startLowAddr1;
        private byte startHighAddr1;
        private byte[] data1; 
        //当某数据类型长度超过32或有两个数据类型同时请求时使用
        private byte addrType2;
        private byte length2 = 0;
        private byte startLowAddr2 = 0;
        private byte startHighAddr2 = 0;
        private byte[] data2 = new byte[0];
        private byte[] command;
        //返回的数据
        private byte[] _retData;
        public byte[] RetData
        {
            get
            {
                return _retData;
            }
            set
            {
                _retData = value;
                CheckRetData();
            }
        }
        public bool IsSuccess { get; set; }
        public GeneralWriteCommand(byte addrType, byte length, byte startLowAddr, byte startHighAddr,byte[] data)
        {
            int byteCnt = CommandHelper.GetLengthByAddrType(addrType);
            if (length > CommunicationDataDefine.MAX_ELEM_NUM * 2)
            {
                return;//throw Exception
            }
            if (data.Length != byteCnt * length)
            {
                return;//throw Exception
            }
            if (startHighAddr << 8 > ushort.MaxValue - startLowAddr - length + 1)
            {
                return;//throw Exception
            }
            if (CommandHelper.checkAddrRange(addrType, length, (ushort)(startLowAddr + startHighAddr << 8)))
            {
                addrTypeNum = 0x01;
                addrType1 = addrType;
                addrType2 = addrType;
                startLowAddr1 = startLowAddr;
                startHighAddr1 = startHighAddr;
                if (length <= CommunicationDataDefine.MAX_ELEM_NUM)
                {
                    length1 = length;
                    data1 = data;
                }
                else
                {
                    length1 = CommunicationDataDefine.MAX_ELEM_NUM;
                    length2 = (byte)(length - CommunicationDataDefine.MAX_ELEM_NUM);
                    data1 = new byte[length1 * byteCnt];
                    data2 = new byte[length2 * byteCnt];
                    for (int i = 0; i < data.Count(); i++)
                    {
                        if (i < length1 * byteCnt)
                        {
                            data1[i] = data[i];
                        }
                        else
                        {
                            data2[i - length1 * byteCnt] = data[i];
                        }
                    }
                    ushort tempaddr = (ushort)(startLowAddr + startHighAddr << 8 + CommunicationDataDefine.MAX_ELEM_NUM);
                    startLowAddr2 = (byte)(tempaddr & 0x00FF);
                    startHighAddr2 = (byte)((tempaddr & 0xFF00) >> 8);
                }
                GenerateCommand();
            }
            else
            {
                return;//throw Exception
            }
        }
        public GeneralWriteCommand(byte[] addrType, byte[] length, byte[] startLowAddr, byte[] startHighAddr,byte[] data1,byte[] data2)
        {
            if (CommandHelper.GetLengthByAddrType(addrType[0]) * length[0] != data1.Length || CommandHelper.GetLengthByAddrType(addrType[1]) * length[1] != data2.Length)
            {
                return;
            }
            if (length[0] > CommunicationDataDefine.MAX_ELEM_NUM || length[1] > CommunicationDataDefine.MAX_ELEM_NUM)
            {
                return;
            }
            if (startHighAddr[0] << 8 > ushort.MaxValue - startLowAddr[0] - length[0] + 1 || startHighAddr[1] << 8 > ushort.MaxValue - startLowAddr[1] - length[1] + 1)
            {
                return;
            }
            if (CommandHelper.checkAddrRange(addrType[0], length[0], (ushort)(startLowAddr[0] + startHighAddr[0] << 8)) && CommandHelper.checkAddrRange(addrType[1], length[1], (ushort)(startLowAddr[1] + startHighAddr[1] << 8)))
            {
                addrTypeNum = 0x02;
                addrType1 = addrType[0];
                addrType2 = addrType[1];
                startLowAddr1 = startLowAddr[0];
                startHighAddr1 = startHighAddr[0];
                length1 = length[0];
                length2 = length[1];
                startLowAddr2 = startLowAddr[1];
                startHighAddr2 = startHighAddr[1];
                this.data1 = data1;
                this.data2 = data2;
                GenerateCommand();
            }
            else
            {
                return;
            }
        }
        private void GenerateCommand()
        {
            int cnt1 = data1.Count();
            int cnt2 = data2.Count();
            command = new byte[13 + cnt1 + cnt2];
            byte[] commandCache = new byte[command.Length - 2];
            byte[] CRCCode;
            for (int i = 0; i < commandCache.Length; i++)
            {
                if(i == 0) commandCache[i] = slaveNum;
                else if (i == 1) commandCache[i] = commandType;
                else if (i == 2) commandCache[i] = addrTypeNum;
                else if (i == 3) commandCache[i] = addrType1;
                else if (i == 4) commandCache[i] = length1;
                else if (i == 5) commandCache[i] = startLowAddr1;
                else if (i == 6) commandCache[i] = startHighAddr1;
                else if (i == 7 + cnt1) commandCache[i] = addrType2;
                else if (i == 8 + cnt1) commandCache[i] = length2;
                else if (i == 9 + cnt1) commandCache[i] = startLowAddr2;
                else if (i == 10 + cnt1) commandCache[i] = startHighAddr2;
                else if (i < 7 + cnt1)
                {
                    commandCache[i] = data1[i - 7];
                }
                else
                {
                    commandCache[i] = data2[i - cnt1 - 11];
                }
            }
            CRCCode = CRC16.GetCRC(commandCache);
            for (int i = 0; i < commandCache.Length; i++)
            {
                command[i] = commandCache[i];
            }
            command[command.Length - 2] = CRCCode[0];
            command[command.Length - 1] = CRCCode[1];
        }
        public byte[] GetBytes()
        {
            return command;
        }
        private void CheckRetData()
        {
            if (RetData.Length == 3)
            {
                FGs_ERR_CODE errCodeType = CommandHelper.GetERRCODEType(RetData[2]);
                if (errCodeType == FGs_ERR_CODE.FGs_CARRY_OK)
                {
                    IsSuccess = true;
                }
                else
                {
                    IsSuccess = false;
                    //抛出相应异常
                }
            }
            else
            {
                IsSuccess = false;
                //抛出相应异常
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Communication.Command
{
    public class GeneralReadCommand : ICommunicationCommand
    {
        private const byte slaveNum = CommunicationDataDefine.SLAVE_ADDRESS;
        private const byte commandType = CommunicationDataDefine.FGS_READ;
        private byte addrTypeNum;
        private byte addrType1;
        private byte length1;
        private byte startLowAddr1;
        private byte startHighAddr1;
        // 当某数据类型长度超过32或有两个数据类型同时请求时使用
        private byte addrType2;
        private byte length2 = 0;
        private byte startLowAddr2 = 0;
        private byte startHighAddr2 = 0;
        private byte[] command;
        // 返回的数据
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
        public GeneralReadCommand(byte addrType,byte length,byte startLowAddr,byte startHighAddr)
        {
            if (length > CommunicationDataDefine.MAX_ELEM_NUM * 2)
            {
                return;//throw Exception
            }
            if (startHighAddr << 8 > ushort.MaxValue - startLowAddr - length + 1)
            {
                return;//throw Exception
            }
            if (CommandHelper.checkAddrRange(addrType,length, (ushort)(startLowAddr + startHighAddr << 8)))
            {
                addrTypeNum = 0x01;
                addrType1 = addrType;
                addrType2 = addrType;
                startLowAddr1 = startLowAddr;
                startHighAddr1 = startHighAddr;
                if (length <= CommunicationDataDefine.MAX_ELEM_NUM)
                {
                    length1 = length;
                }
                else
                {
                    length1 = CommunicationDataDefine.MAX_ELEM_NUM;
                    length2 = (byte)(length - CommunicationDataDefine.MAX_ELEM_NUM);
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
        public GeneralReadCommand(byte[] addrType, byte[] length, byte[] startLowAddr, byte[] startHighAddr)
        {
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
                GenerateCommand();
            }
            else
            {
                return;
            }
        }
        private void GenerateCommand()
        {
            command = new byte[13];
            byte[] commandCache = new byte[command.Length - 2];
            byte[] CRCCode;
            commandCache[0] = slaveNum;
            commandCache[1] = commandType;
            commandCache[2] = addrTypeNum;
            commandCache[3] = addrType1;
            commandCache[4] = length1;
            commandCache[5] = startLowAddr1;
            commandCache[6] = startHighAddr1;
            commandCache[7] = addrType2;
            commandCache[8] = length2;
            commandCache[9] = startLowAddr2;
            commandCache[10] = startHighAddr2;
            CRCCode = CRC16.GetCRC(commandCache);
            for (int i = 0; i < commandCache.Length; i++)
            {
                command[i] = commandCache[i];
            }
            command[11] = CRCCode[0];
            command[12] = CRCCode[1];
        }
        public byte[] GetBytes()
        {
            return command;
        }
        private void CheckRetData()
        {
            if (RetData.Length == 3)
            {
                IsSuccess = false;
                FGs_ERR_CODE errCodeType = CommandHelper.GetERRCODEType(RetData[2]);
                //抛出相应异常
            }
            else
            {
                byte[] commandCache = new byte[RetData.Length - 2];
                byte[] CRCCode = new byte[2];
                for (int i = 0; i < commandCache.Length; i++)
                {
                    commandCache[i] = RetData[i];
                }
                CRCCode[0] = RetData[RetData.Length - 2];
                CRCCode[1] = RetData[RetData.Length - 1];
                if (!CRC16.CheckCRC(commandCache,CRCCode))
                {
                    IsSuccess = false;
                    //抛出CRC校验异常
                }
                else
                {
                    IsSuccess = true;
                }
            }
        }
        // Can be call after assign value for Property of RetData
        public List<byte[]> GetRetData()
        {
            List<byte[]> templist = new List<byte[]>(2);
            if (IsSuccess)
            {
                int type1Len = CommandHelper.GetLengthByAddrType(addrType1);
                int type2Len = CommandHelper.GetLengthByAddrType(addrType2);
                byte[] data1 = new byte[length1 * type1Len];
                byte[] data2 = new byte[length2 * type2Len];
                for (int i = 0; i < data1.Length; i++)
                {
                    data1[i] = RetData[5 + i];
                }
                for (int i = 0; i < data2.Length; i++)
                {
                    data2[i] = RetData[7 + data1.Length + i];
                }
                templist.Add(data1);
                templist.Add(data2);
                return templist;
            }
            else
            {
                return null;
            }
        }
    }
}

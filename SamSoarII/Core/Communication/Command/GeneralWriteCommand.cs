﻿using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Core.Communication
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
        
        //private byte addrType2 = 0;
        //private byte length2 = 0;
        //private byte startLowAddr2 = 0;
        //private byte startHighAddr2 = 0;
        
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
        public int RecvDataLen
        {
            get { return 3; }
            set { }
        }
        public bool IsComplete { get; set; }
        public bool IsSuccess { get; set; }
        //public List<MonitorElement> RefElements_A { get; set; } = new List<MonitorElement>();
        //public List<MonitorElement> RefElements_B { get; set; } = new List<MonitorElement>();
        public GeneralWriteCommand(byte[] data, ValueStore vstore)
        {
            data1 = data;
            InitializeCommandByElement(vstore);
            GenerateCommand();
        }
        private void InitializeCommandByElement(ValueStore vstore)
        {
            addrTypeNum = 0x01;
            addrType1 = (byte)CommandHelper.GetAddrType(vstore.Base, (uint)(vstore.Offset));
            length1 = (byte)(vstore.ByteCount == 4 ? 0x02 : 0x01);
            byte[] startaddr = ValueConverter.GetBytes((ushort)vstore.Offset);
            startLowAddr1 = startaddr[1];
            startHighAddr1 = startaddr[0];
        }
        private void GenerateCommand()
        {
            int cnt1 = data1.Count();
            command = new byte[9 + cnt1];
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
                else if (i < 7 + cnt1)
                {
                    commandCache[i] = data1[i - 7];
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
            if (RetData.Length < 3)
            {
                IsComplete = false;
                IsSuccess = false;
                return;
            }
            if (RetData.Length == 3)
            {
                FGs_ERR_CODE errCodeType = CommandHelper.GetERRCODEType(RetData[2]);
                IsComplete = true;
                IsSuccess = (errCodeType == FGs_ERR_CODE.FGs_CARRY_OK);
                return;
            }
            if (RetData.Length > 3)
            {
                IsComplete = true;
                IsSuccess = false;
                return;
            }
        }
        
    }
}

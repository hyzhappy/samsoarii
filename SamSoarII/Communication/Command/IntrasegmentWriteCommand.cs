using SamSoarII.AppMain.UI.Monitor;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Communication.Command
{
    public class IntrasegmentWriteCommand : ICommunicationCommand
    {
        private const byte slaveNum = CommunicationDataDefine.SLAVE_ADDRESS;
        private const byte commandType = CommunicationDataDefine.FGS_READ;
        private byte addrTypeNum = 0x00;
        private byte addrType1;
        private byte addrType2;
        private byte length;
        private byte startLowAddr1;
        private byte startLowAddr2;
        private byte startHighAddr;
        public IntraSegment IntraSeg
        {
            get
            {
                AddrSegment _base = new AddrSegment
                (
                    addrType1,
                    length,
                    startLowAddr1,
                    startHighAddr
                );
                AddrSegment _intra = new AddrSegment
                (
                    addrType2,
                    1,
                    startLowAddr2,
                    0
                );
                return new IntraSegment(_base, _intra);
            }
        }
        
        private byte[] data;
        private byte[] command;
        public bool IsSuccess { get; set; }
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
        public ElementModel RefElement { get; set; }
        public IntrasegmentWriteCommand(byte[] data, ElementModel RefElement)
        {
            this.data = data;
            this.RefElement = RefElement;
            InitializeCommandByElement();
            GenerateCommand();
        }
        private void InitializeCommandByElement()
        {
            addrType2 = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType),RefElement.IntrasegmentType),RefElement.IntrasegmentAddr);
            startLowAddr2 = (byte)RefElement.IntrasegmentAddr;
            addrType1 = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), RefElement.AddrType), RefElement.StartAddr);
            length = 0x01;
            byte[] startaddr = ValueConverter.GetBytes((ushort)RefElement.StartAddr);
            startLowAddr1 = startaddr[0];
            startHighAddr = startaddr[1];
        }
        public IntrasegmentWriteCommand(byte[] addrType, byte length, byte[] startLowAddr, byte startHighAddr,byte[] data)
        {
            if (length > CommunicationDataDefine.MAX_ELEM_NUM)
            {
                return;
            }
            if (length * CommandHelper.GetLengthByAddrType(addrType[0]) != data.Length)
            {
                return;
            }
            addrType1 = addrType[0];
            addrType2 = addrType[1];
            startLowAddr1 = startLowAddr[0];
            startLowAddr2 = startLowAddr[1];
            this.startHighAddr = startHighAddr;
            this.data = data;
            this.length = length;
            GenerateCommand();
        }
        public void GenerateCommand()
        {
            int cnt = data.Length;
            command = new byte[11 + cnt];
            byte[] commandCache = new byte[command.Length - 2];
            byte[] CRCCode;
            commandCache[0] = slaveNum;
            commandCache[1] = commandType;
            commandCache[2] = addrTypeNum;
            commandCache[3] = addrType2;
            commandCache[4] = startLowAddr2;
            commandCache[5] = addrType1;
            commandCache[6] = length;
            commandCache[7] = startLowAddr1;
            commandCache[8] = startHighAddr;
            for (int i = 0; i < commandCache.Length; i++)
            {
                if(i == 0) commandCache[i] = slaveNum;
                else if (i == 1) commandCache[i] = commandType;
                else if (i == 2) commandCache[i] = addrTypeNum;
                else if (i == 3) commandCache[i] = addrType2;
                else if (i == 4) commandCache[i] = startLowAddr2;
                else if (i == 5) commandCache[i] = addrType1;
                else if (i == 6) commandCache[i] = length;
                else if (i == 7) commandCache[i] = startLowAddr1;
                else if (i == 8) commandCache[i] = startHighAddr;
                else
                {
                    commandCache[i] = data[i - 9];
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

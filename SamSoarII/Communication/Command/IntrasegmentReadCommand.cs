using SamSoarII.AppMain.UI.Monitor;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Communication.Command
{
    public class IntrasegmentReadCommand : ICommunicationCommand
    {
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
                if (IsSuccess)
                {
                    UpdataValues();
                }
            }
        }
        private const byte slaveNum = CommunicationDataDefine.SLAVE_ADDRESS;
        private const byte commandType = CommunicationDataDefine.FGS_READ;
        private byte addrTypeNum = 0x00;
        private byte addrType1;
        private byte addrType2;
        private byte length;
        private byte startLowAddr1;
        private byte startLowAddr2;
        private byte startHighAddr;
        public List<ElementModel> RefElements { get; set; } = new List<ElementModel>();
        public IntrasegmentReadCommand(){}
        public void InitializeCommandByElement()
        {
            if (RefElements.Count > 0)
            {
                ElementModel element = RefElements.First();
                addrType1 = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), element.AddrType), element.StartAddr);
                length = (byte)RefElements.Count;
                byte[] startaddr = ValueConverter.GetBytes((ushort)element.StartAddr);
                startLowAddr1 = startaddr[0];
                startHighAddr = startaddr[1];
                addrType2 = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), element.IntrasegmentType), element.IntrasegmentAddr);
                startLowAddr2 = (byte)element.IntrasegmentAddr;
            }
            GenerateCommand();
        }
        public void UpdataValues()
        {
            List<byte[]> retData = GetRetData();
            CommandHelper.UpdataElements(RefElements, retData[0], length);
        }
        public IntrasegmentReadCommand(byte[] addrType, byte length, byte[] startLowAddr, byte startHighAddr)
        {
            if (length > CommunicationDataDefine.MAX_ELEM_NUM)
            {
                return;
            }
            addrType1 = addrType[0];
            addrType2 = addrType[1];
            startLowAddr1 = startLowAddr[0];
            startLowAddr2 = startLowAddr[1];
            this.startHighAddr = startHighAddr;
            this.length = length;
            GenerateCommand();
        }
        private void GenerateCommand()
        {
            command = new byte[11];
            byte[] commandCache = new byte[command.Length - 2];
            byte[] CRCCode;
            commandCache[0] = slaveNum;
            commandCache[1] = commandType;
            commandCache[2] = addrTypeNum;
            commandCache[3] = addrType1;
            commandCache[4] = length;
            commandCache[5] = startLowAddr1;
            commandCache[6] = startHighAddr;
            commandCache[7] = addrType2;
            commandCache[8] = startLowAddr2;
            CRCCode = CRC16.GetCRC(commandCache);
            for (int i = 0; i < commandCache.Length; i++)
            {
                command[i] = commandCache[i];
            }
            command[9] = CRCCode[0];
            command[10] = CRCCode[1];
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
                if (!CRC16.CheckCRC(commandCache, CRCCode))
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
        //Can be call after assign value for Property of RetData
        private List<byte[]> GetRetData()
        {
            List<byte[]> templist = new List<byte[]>(2);
            if (IsSuccess)
            {
                int type1Len = CommandHelper.GetLengthByAddrType(addrType1);
                int type2Len = CommandHelper.GetLengthByAddrType(addrType2);
                byte[] data1 = new byte[length * type1Len];
                byte[] data2 = new byte[type2Len];
                for (int i = 0; i < data1.Length; i++)
                {
                    data1[i] = RetData[i + 5];
                }
                for (int i = 0; i < data2.Length; i++)
                {
                    data2[i] = RetData[i + 6 + data1.Length];
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

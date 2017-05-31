using SamSoarII.AppMain.UI.Monitor;
using SamSoarII.Utility;
using System;
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
        private byte addrTypeNum = 0x00;
        public List<AddrSegment>[] SegmentsGroup = new List<AddrSegment>[CommunicationDataDefine.MAX_ADDRESS_TYPE];
        private bool Initialized = false;
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
                if (IsSuccess)
                {
                    UpdataValues();
                }
            }
        }
        public bool IsSuccess { get; set; }
        public GeneralReadCommand(){}
        public void InitializeCommandByElement()
        {
            foreach (var group in SegmentsGroup)
            {
                if (group != null && group.Count > 0) addrTypeNum++;
                else break;
            }
            command = new byte[4 * addrTypeNum + 5];
            GenerateCommand();
        }
        public void UpdataValues()
        {
            List<byte[]> retData = GetRetData();
            for (int i = 0; i < retData.Count; i++)
            {
                CommandHelper.UpdateElements(SegmentsGroup[i],retData[i]);
            }
        }
        private void GenerateCommand()
        {
            command[0] = slaveNum;
            command[1] = commandType;
            command[2] = addrTypeNum;
            for (int i = 0; i < addrTypeNum; i++)
            {
                command[3 + 4 * i] = SegmentsGroup[i].First().Type;
                var group = SegmentsGroup[i].OrderBy(x => { return x.Model.StartAddr; });
                command[4 + 4 * i] = (byte)(group.Last().Model.StartAddr - group.First().Model.StartAddr + 1);
                command[5 + 4 * i] = group.First().AddrLow;
                command[6 + 4 * i] = group.First().AddrHigh;
            }
            byte[] commandCache = new byte[command.Length - 2];
            byte[] CRCCode;
            for (int i = 0; i < commandCache.Length; i++)
            {
                commandCache[i] = command[i];
            }
            CRCCode = CRC16.GetCRC(commandCache);
            command[command.Length - 2] = CRCCode[0];
            command[command.Length - 1] = CRCCode[1];
        }
        public byte[] GetBytes()
        {
            if (!Initialized)
            {
                InitializeCommandByElement();
                Initialized = true;
            }
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
        //Can be call after assign value for Property of RetData
        private List<byte[]> GetRetData()
        {
            List<byte[]> templist = new List<byte[]>(RetData[2]);
            int pos = 3;
            for (int i = 0; i < RetData[2]; i++)
            {
                int typeLen = CommandHelper.GetLengthByAddrType(command[3 + 4 * i]);
                int dataLen = typeLen * command[4 + 4 * i];
                if (typeLen == 1)
                {
                    dataLen = dataLen / 8 + dataLen % 8 == 0 ? 0: 1;
                }
                byte[] data = new byte[dataLen];
                pos += 2;
                for (int j = 0; j < data.Length;j++)
                {
                    data[j] = RetData[pos++];
                }
                templist.Add(data);
            }
            return templist;
        }
    }
}

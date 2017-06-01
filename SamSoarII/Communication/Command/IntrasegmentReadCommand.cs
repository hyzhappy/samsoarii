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
        public List<IntraSegment> Segments = new List<IntraSegment>();
        public bool IsComplete { get; set; }
        public bool IsSuccess { get; set; }
        private bool Initialized = false;
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
        
        public IntrasegmentReadCommand(){}
        public void InitializeCommandByElement()
        {
            command = new byte[11];
            GenerateCommand();
        }
        public void UpdataValues()
        {
            byte[] retData = GetRetData();
            CommandHelper.UpdateElementsByIntra(Segments,retData);
        }
        private void GenerateCommand()
        {
            command[0] = slaveNum;
            command[1] = commandType;
            command[2] = addrTypeNum;
            command[3] = Segments.First().Base.Type;
            var group = Segments.OrderBy(x => { return x.Base.Model.StartAddr; });
            var model = group.Last().Base.Model;
            command[4] = (byte)(model.StartAddr - group.First().Base.Model.StartAddr + 1);
            if (model.ByteCount == 4 && !(model.AddrType == "CV" && model.StartAddr >= 200))
            {
                command[4]++;
            }
            command[5] = group.First().Base.AddrLow;
            command[6] = group.First().Base.AddrHigh;
            command[7] = group.First().Intra.Type;
            command[8] = group.First().Intra.AddrLow;
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
            if (RetData.Length < 3)
            {
                IsComplete = false;
                IsSuccess = false;
                return;
            }
            if (RetData.Length == 3)
            {
                FGs_ERR_CODE errCodeType = CommandHelper.GetERRCODEType(RetData[2]);
                //抛出相应异常
                IsComplete = (errCodeType != FGs_ERR_CODE.FGs_ISNOTANERRCODE);
                IsSuccess = false;
                return;
            }
            int cplen = command.Length;
            var group = Segments.OrderBy(x => { return x.Base.Model.StartAddr; });
            AddrSegment seg = group.First().Base;
            int bytelen = CommandHelper.GetLengthByAddrType(seg.Type);
            int bitlen = (bytelen == 1 ? 1 : bytelen * 8);
            bitlen *= seg.Length;
            bytelen = (bitlen - 1) / 8 + 1;
            cplen += bytelen;
            if (RetData.Length < cplen)
            {
                IsComplete = false;
                IsSuccess = false;
                return;
            }
            byte[] commandCache = new byte[RetData.Length - 2];
            byte[] CRCCode = new byte[2];
            for (int i = 0; i < commandCache.Length; i++)
            {
                commandCache[i] = RetData[i];
            }
            CRCCode[0] = RetData[RetData.Length - 2];
            CRCCode[1] = RetData[RetData.Length - 1];
            IsSuccess = CRC16.CheckCRC(commandCache, CRCCode);
        }
        //Can be call after assign value for Property of RetData
        private byte[] GetRetData()
        {
            int typeLen = CommandHelper.GetLengthByAddrType(command[3]);
            int dataLen = typeLen * command[4];
            if (typeLen == 1)
            {
                dataLen = dataLen / 8 + dataLen % 8 == 0 ? 0 : 1;
            }
            byte[] data = new byte[dataLen];
            for (int j = 0; j < data.Length; j++)
            {
                data[j] = RetData[5 + j];
            }
            return data;
        }
    }
}

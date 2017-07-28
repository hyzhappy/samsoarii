using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Core.Communication
{
    public class ForceCancelCommand : ICommunicationCommand
    {
        private const byte commandType = CommunicationDataDefine.FORCE_CANCEL;
        private byte addrType;
        private byte startLowAddr;
        private byte[] command;
        private bool isAll = false;
        public bool IsAll { get { return this.isAll; } }
        //public MonitorElement RefElement { get; set; }
        public ForceCancelCommand(bool isAll, ValueStore vstore)
        {
            this.isAll = isAll;
            //this.RefElement = RefElement;
            InitializeCommandByElement(vstore);
            //GenerateCommand();
        }
        public int RecvDataLen
        {
            get { return 2; } set { }
        }
        public void InitializeCommandByElement(ValueStore vstore)
        {
            addrType = (byte)CommandHelper.GetAddrType(vstore.Base, (uint)vstore.Offset);
            if (isAll)
            {
                startLowAddr = 0xFF;
            }
            else
            {
                startLowAddr = ValueConverter.GetBytes((uint)vstore.Offset)[1];
            }
        }
        private void GenerateCommand()
        {
            command = new byte[7];
            byte[] commandCache = new byte[command.Length - 2];
            byte[] CRCCode;
            commandCache[0] = commandType;
            commandCache[1] = 0x00;
            commandCache[2] = 0x00;
            commandCache[3] = addrType;
            commandCache[4] = startLowAddr;
            CRCCode = CRC16.GetCRC(commandCache);
            for (int i = 0; i < commandCache.Length; i++)
            {
                command[i] = commandCache[i];
            }
            command[5] = CRCCode[0];
            command[6] = CRCCode[1];
        }
        public bool IsComplete
        {
            get { return _retData != null && _retData.Length >= RecvDataLen; }
            set { }
        }
        public bool IsSuccess { get; set; }
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
        public byte[] GetBytes()
        {
            GenerateCommand();
            return command;
        }
        private void CheckRetData()
        {
            IsSuccess = true;
        }

        public void UpdataValues()
        {
            //to do nothing
        }
    }
}

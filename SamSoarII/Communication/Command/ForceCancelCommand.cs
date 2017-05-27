using SamSoarII.AppMain.UI.Monitor;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Communication.Command
{
    public class ForceCancelCommand : ICommunicationCommand
    {
        private const byte commandType = CommunicationDataDefine.FORCE_CANCEL;
        private byte addrType;
        private byte startLowAddr;
        private byte[] command;
        private bool isAll = false;
        public bool IsAll { get { return this.isAll; } }
        public ElementModel RefElement { get; set; }
        public ForceCancelCommand(bool isAll,ElementModel RefElement)
        {
            this.isAll = isAll;
            this.RefElement = RefElement;
            InitializeCommandByElement();
            //GenerateCommand();
        }
        public void InitializeCommandByElement()
        {
            addrType = (byte)CommandHelper.GetAddrType((ElementAddressType)Enum.Parse(typeof(ElementAddressType), RefElement.AddrType), RefElement.StartAddr);
            if (isAll)
            {
                startLowAddr = 0xFF;
            }
            else
            {
                startLowAddr = ValueConverter.GetBytes((ushort)RefElement.StartAddr)[0];
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
    }
}

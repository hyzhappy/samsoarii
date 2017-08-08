using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Communication
{
    public class TransportBinCommand : ICommunicationCommand
    {
        public TransportBinCommand(int id, byte[] data)
        {
            command = new byte[6];
            command[0] = CommunicationDataDefine.CMD_DOWNLOAD_FLAG;
            byte[] len = ValueConverter.GetLengthByInt(command.Length + data.Length + 2);
            command[1] = len[0];
            command[2] = len[1];
            command[3] = CommunicationDataDefine.CMD_IAP_PRO_BIN;
            len = ValueConverter.GetBytes((ushort)id);
            command[4] = len[0];
            command[5] = len[1];
            CommandHelper.Encrypt(id,data);
            command = command.Concat(data).ToArray();
            command = command.Concat(CRC16.GetCRC(command)).ToArray();
        }

        public bool IsComplete
        {
            get; set;
        }

        public bool IsSuccess
        {
            get; set;
        }

        public int RecvDataLen
        {
            get; set;
        }

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
                CommandHelper.CheckRetData(this, _retData);
            }
        }
        private byte[] command;
        public byte[] GetBytes()
        {
            return command;
        }
    }
}

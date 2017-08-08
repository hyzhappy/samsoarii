using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Communication
{
    public class IAPDESKEYCommand : ICommunicationCommand
    {
        public IAPDESKEYCommand(int filelength)
        {
            this.filelength = filelength;
        }
        private int filelength;
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

        public byte[] GetBytes()
        {
            Random random = new Random();
            byte[] command = new byte[16];
            command[0] = CommunicationDataDefine.CMD_DOWNLOAD_FLAG;
            byte[] len = ValueConverter.GetLengthByInt(command.Length + 2);
            command[1] = len[0];
            command[2] = len[1];
            command[3] = CommunicationDataDefine.CMD_IAP_DES_KEY;
            for (int i = 0; i < 8; i++)
            {
                command[4 + i] = (byte)(random.Next() & 0xFF);
            }
            len = ValueConverter.GetBytes((uint)filelength);
            for (int i = 0; i < len.Length; i++)
            {
                command[12 + i] = len[i];
            }
            command = command.Concat(CRC16.GetCRC(command)).ToArray();
            return command;
        }
    }
}

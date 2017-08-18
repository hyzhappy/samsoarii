using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Communication
{
    public class DownloadTypeStart : ICommunicationCommand
    {
        public DownloadTypeStart(byte downloadCode,int dataLength)
        {
            command = new byte[9];
            command[0] = CommunicationDataDefine.CMD_COMMU_FLAG;
            byte[] len = ValueConverter.GetBytes((ushort)(command.Length + 2), true);
            command[1] = len[0];
            command[2] = len[1];
            command[3] = CommunicationDataDefine.CMD_DOWNLOAD_START;
            command[4] = downloadCode;
            len = ValueConverter.GetBytes((uint)dataLength,true);
            for (int i = 0; i < len.Length; i++)
            {
                command[5 + i] = len[i];
            }
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
        public FGs_ERR_CODE ErrorCode
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

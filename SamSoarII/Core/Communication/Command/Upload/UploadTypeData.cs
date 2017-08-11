using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamSoarII.Utility;

namespace SamSoarII.Core.Communication
{
    public class UploadTypeData : ICommunicationCommand
    {
        public UploadTypeData(byte funcCode, int id)
        {
            command = new byte[6];
            command[0] = CommunicationDataDefine.CMD_COMMU_FLAG;
            byte[] len = ValueConverter.GetBytes((ushort)(command.Length + 2), true);
            command[1] = len[0];
            command[2] = len[1];
            command[3] = funcCode;
            len = ValueConverter.GetBytes((ushort)id, true);
            command[4] = len[0];
            command[5] = len[1];
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

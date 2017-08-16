using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Communication
{
    public class PasswordCheckCommand : ICommunicationCommand
    {
        public PasswordCheckCommand(byte funcCode,string password)
        {
            command = new byte[5];
            command[0] = CommunicationDataDefine.CMD_COMMU_FLAG;
            byte[] len = ValueConverter.GetBytes((ushort)(command.Length + password.Length + 2), true);
            command[1] = len[0];
            command[2] = len[1];
            command[3] = funcCode;
            command[4] = (byte)password.Length;
            command = command.Concat(ValueConverter.GetBytes(password)).ToArray();
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

﻿using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Communication
{
    public class SwitchPLCStatusCommand : ICommunicationCommand
    {
        public SwitchPLCStatusCommand(bool isRun = false)
        {
            this.isRun = isRun;
        }

        //是否切换为RUN状态
        private bool isRun;
        public bool IsComplete
        {
            get;set;
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
            byte[] command = new byte[4];
            command[0] = CommunicationDataDefine.CMD_DOWNLOAD_FLAG;
            byte[] len = ValueConverter.GetLengthByInt(command.Length + 2);
            command[1] = len[0];
            command[2] = len[1];
            command[3] = isRun ? CommunicationDataDefine.CMD_STATE_RUN : CommunicationDataDefine.CMD_STATE_STOP;
            command = command.Concat(CRC16.GetCRC(command)).ToArray();
            return command;
        }
    }
}

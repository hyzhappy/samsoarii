using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Generate
{
    public class PLCOriginInst
    {
        public PLCOriginInst(PLCInstruction _inst, string _text)
        {
            Inst = _inst;
            args = _text.Split();
        }

        private PLCInstruction inst;
        public PLCInstruction Inst
        {
            get
            {
                return this.inst;
            }
            set
            {
                if (inst == value) return;
                PLCInstruction _inst = inst;
                this.inst = value;
                if (_inst != null && _inst.Origin != null)
                    _inst.Origin = null;
                if (inst != null && inst.Origin != this)
                    inst.Origin = this;
            }
        }
        private string[] args;
        public string this[int id] { get { return id < args.Length ? args[id] : ""; } }

        public const int STATUS_ACCEPT = 0x00;
        public const int STATUS_WARNING = 0x01;
        public const int STATUS_ERROR = 0x02;
        public int Status { get; set; }
        public string Message { get; set; }
    }
}

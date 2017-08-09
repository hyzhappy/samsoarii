using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Generate
{
    public class PLCOriginInst
    {
        public PLCOriginInst(PLCInstruction _inst)
        {
            Inst = _inst;
        }

        public override string ToString()
        {
            if (inst.ProtoType == null)
            {
                StringBuilder ret = new StringBuilder();
                ret.Append(inst[0]);
                for (int i = 1; i < inst.Count; i++)
                    ret.Append(inst[i]);
                return ret.ToString();
            }
            return inst.ProtoType.ToInstString();
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
                this.inst = null;
                if (_inst != null && _inst.Origin != null)
                    _inst.Origin = null;
                this.inst = value;
                if (inst != null && inst.Origin != this)
                    inst.Origin = this;
            }
        }

        public string this[int id]
        {
            get
            {
                if (inst.ProtoType == null) return id < inst.Count ? inst[id] : "";
                return id == 0 ? inst.OldInstname : id-1 < inst.ProtoType.Children.Count ? inst.ProtoType.Children[id - 1].Text : "";
            }
        }

        public const int STATUS_ACCEPT = 0x00;
        public const int STATUS_WARNING = 0x01;
        public const int STATUS_ERROR = 0x02;
        public int Status { get; set; }
        public string Message { get; set; }
    }
}

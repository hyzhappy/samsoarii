﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class YBitValue : BitValue
    {
        public YBitValue(uint index, WordValue offset)
        {
            Base = string.Format("Y");
            Index = index;
            Offset = offset == null ? WordValue.Null : offset;
        }
        public override string ValueShowString
        {
            get
            {
                return ValueString;
            }
        }
    }
}

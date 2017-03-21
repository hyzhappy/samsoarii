﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class MBitValue : BitValue
    {
        public MBitValue(uint index, WordValue offset)
        {
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

        public override string ValueString
        {
            get
            {
                return string.Format("M{0}{1}", Index, Offset.ValueString);
            }
        }

        public override string GetValue()
        {
            if (Offset != WordValue.Null)
            {
                return string.Format("MBit[{0} + {1}]", Index, Offset.GetValue());
            }
            else
            {
                return string.Format("MBit[{0}]", Index);
            }
        }
    }
}

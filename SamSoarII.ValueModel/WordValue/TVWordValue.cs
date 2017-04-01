﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class TVWordValue : WordValue
    {
        public TVWordValue(uint index, WordValue offset)
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
                return string.Format("TV{0}{1}", Index, Offset.ValueString);
            }
        }

        public override string GetValue()
        {
            throw new NotImplementedException();
        }
    }
}

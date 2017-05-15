using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class CV32DoubleWordValue : DoubleWordValue
    {
        public CV32DoubleWordValue(uint index, WordValue offset)
        {
            Base = string.Format("CV");
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

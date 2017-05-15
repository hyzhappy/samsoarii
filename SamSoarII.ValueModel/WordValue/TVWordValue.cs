using System;
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
            Base = string.Format("TV");
            Index = index;
            Offset = offset == null ? Null : offset;
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

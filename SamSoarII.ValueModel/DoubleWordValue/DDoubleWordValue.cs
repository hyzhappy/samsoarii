using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class DDoubleWordValue : DoubleWordValue
    {
        public DDoubleWordValue(uint index, VWordValue offset)
        {
            Index = index;
            Offset = offset;
        }

        public override string GetDoubleWordValue()
        {
            return string.Empty;
        }

        public override string ToString()
        {
            return string.Format("D{0}D{1}{2}", Index, Index + 1, Offset);   
        }
    }
}

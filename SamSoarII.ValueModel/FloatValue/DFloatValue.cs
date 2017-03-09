using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class DFloatValue : FloatValue
    {
        
        public DFloatValue(uint index, VWordValue offset = null)
        {
            Index = index;
            Offset = offset;
        }

        public override string GetFloatValue()
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return string.Format("D{0}D{1}{2}", Index, Index + 1, Offset);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class CV32DoubleWordValue : DoubleWordValue
    {
        public CV32DoubleWordValue(uint index)
        {
            Index = index;
            Offset = WordValue.Null as NullWordValue;
        }

        public CV32DoubleWordValue(uint index, IVariableValueModel offset)
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
            return string.Format("CV{0}{1}", Index, Offset); 
        }
    }
}

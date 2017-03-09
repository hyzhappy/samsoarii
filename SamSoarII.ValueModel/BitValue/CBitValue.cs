using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class CBitValue : BitValue
    {

        public CBitValue(uint index)
        {
            Index = index;
            Offset = WordValue.Null as NullWordValue;
        }
        public CBitValue(uint index, IVariableValueModel offset = null)
        {
            Index = index;
            Offset = offset;
        }
        public override string GetBitValue()
        {
            if(Offset != WordValue.Null)
            {
                return string.Format("CBit[{0} + {1}]", Index, Offset.GetVariableValue());
            }
            else
            {
                return string.Format("CBit[{0}]", Index);
            }
        }

        public override string GetInputImBitAddress()
        {
            throw new NotImplementedException();
        }

        public override string GetOutputImBitAddress()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("C{0}{1}", Index, Offset);   
        }
    }
}

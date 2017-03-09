using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class SBitValue : BitValue
    {
        public SBitValue(uint index)
        {
            Index = index;
            Offset = WordValue.Null as NullWordValue;
        }
        public SBitValue(uint index, IVariableValueModel offset)
        {
            Index = index;
            Offset = offset;
        }

        public override string GetBitValue()
        {
            if (Offset != WordValue.Null)
            {
                return string.Format("SBit[{0} + {1}]", Index, Offset.GetVariableValue());
            }
            else
            {
                return string.Format("SBit[{0}]", Index);
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
            if (Offset == null)
            {
                return string.Format("S{0}", Index);
            }
            else
            {
                return string.Format("S{0}{1}", Index, Offset);
            }
        }
    }
}

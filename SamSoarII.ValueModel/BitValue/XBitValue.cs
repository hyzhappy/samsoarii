using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class XBitValue : BitValue
    {
        public XBitValue(uint index)
        {
            Index = index;
            Offset = WordValue.Null as IVariableValueModel;
        }
        public XBitValue(uint index, IVariableValueModel offset)
        {
            Index = index;
            Offset = offset;
            
        }

        public override string GetBitValue()
        {
            if (Offset != WordValue.Null)
            {
                return string.Format("XBit[{0} + {1}]", Index, Offset.GetVariableValue());
            }
            else
            {
                return string.Format("XBit[{0}]", Index);
            }
        }

        public override string GetInputImBitAddress()
        {
            throw new InputImException();
        }

        public override string GetOutputImBitAddress()
        {
            throw new OutputImException();
        }
        public override string ToString()
        {
            return string.Format("X{0}{1}", Index, Offset);
        }
    }
}

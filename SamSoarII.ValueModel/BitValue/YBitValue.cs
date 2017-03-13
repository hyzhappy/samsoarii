using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class YBitValue : BitValue
    {
        public YBitValue(uint index)
        {
            Index = index;
            Offset = WordValue.Null as IVariableValueModel;
        }
        public YBitValue(uint index, IVariableValueModel offset)
        {
            Index = index;
            Offset = offset;
        }

        public override string GetBitValue()
        {
            if (Offset != WordValue.Null)
            {
                return string.Format("YBit[{0} + {1}]", Index, Offset.GetVariableValue());
            }
            else
            {
                return string.Format("YBit[{0}]", Index);
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
            return string.Format("Y{0}{1}", Index, Offset);   
        }
    }
}

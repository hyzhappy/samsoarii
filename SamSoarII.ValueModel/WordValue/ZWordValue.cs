using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class ZWordValue : WordValue, IVariableValueModel
    {
        public ZWordValue(uint index)
        {
            Index = index;
        }

        public string GetVariableValue()
        {
            return GetWordValue();
        }

        public override string GetWordValue()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("Z{0}", Index);
        }

    }

}

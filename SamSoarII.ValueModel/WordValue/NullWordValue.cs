using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class NullWordValue : WordValue, IVariableValueModel
    {
        public override string GetWordValue()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Empty;
        }
        public override string ToShowString()
        {
            return "???";
        }

        public string GetVariableValue()
        {
            return string.Empty;
        }
    }
}

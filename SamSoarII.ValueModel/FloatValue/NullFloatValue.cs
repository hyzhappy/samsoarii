using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class NullFloatValue : FloatValue
    {
        public override string GetFloatValue()
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return "";
        }
        public override string ToShowString()
        {
            return "???";
        }
    }
}

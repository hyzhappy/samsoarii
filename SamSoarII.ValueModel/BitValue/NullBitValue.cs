using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class NullBitValue : BitValue
    {
        
        public override string GetBitValue()
        {
            throw new NotImplementedException();
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
            return "";
        }

        public override string ToShowString()
        {
            return "???";
        }

    }
}

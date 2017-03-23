using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class NullBitValue : BitValue
    {

        public override string ValueShowString
        {
            get
            {
                return "???";
            }
        }

        public override string ValueString
        {
            get
            {
                return string.Empty;
            }
        }

        public override string GetValue()
        {
            throw new InvalidOperationException();
        }
        
    }
}

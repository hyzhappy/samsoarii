using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class KDoubleWordValue : DoubleWordValue
    {
    
        private Int32 Value { get; set; }
        public KDoubleWordValue(Int32 value)
        {
            Value = value;
        }

        public override string GetDoubleWordValue()
        {
            return Convert.ToString(Value);
        }
        public override string ToString()
        {
            return string.Format("K{0}", Convert.ToString(Value));
        }
    }
}

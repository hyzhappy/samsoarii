using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{ 
    public class HDoubleWordValue : DoubleWordValue
    {
        private Int32 Value { get; set; }
        public HDoubleWordValue(Int32 value)
        {
            Value = value;
        }
        public override string GetDoubleWordValue()
        {
            return string.Format("0x{0}", Convert.ToString(Value, 16));
        }
        public override string ToString()
        {
            return string.Format("H{0}", Convert.ToString(Value, 16));
        }
    }
}

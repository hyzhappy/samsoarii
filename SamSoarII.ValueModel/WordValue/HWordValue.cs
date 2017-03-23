using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class HWordValue : WordValue
    {
        private Int16 Value { get; set; }
        public HWordValue(Int16 value)
        {
            Value = value;
        }
        public override string ValueString
        {
            get
            {
                return ValueString;
            }
        }

        public override string ValueShowString
        {
            get
            {
                return string.Format("H{0}", Value);
            }
        }

        public override string GetValue()
        {
            return string.Format("0x{0}", Convert.ToString(Value, 16));
        }
    }
}

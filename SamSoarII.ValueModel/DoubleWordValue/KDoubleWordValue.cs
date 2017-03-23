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

        public override string ValueString
        {
            get
            {
                return string.Format("K{0}", Value);
            }
        }

        public override string ValueShowString
        {
            get
            {
                return ValueString;
            }
        }

        public KDoubleWordValue(Int32 value)
        {
            Value = value;
        }

        public override string GetValue()
        {
            return Convert.ToString(Value);
        }
    }
}

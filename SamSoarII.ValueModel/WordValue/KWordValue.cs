using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class KWordValue : WordValue
    {
        private Int16 Value { get; set; }
        public KWordValue(Int16 value)
        {
            Value = value;
        }
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

        public override string GetValue()
        {
            return Convert.ToString(Value);
        }
    }
}

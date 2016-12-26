using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class KFloatValue : FloatValue
    {
        private float Value { get; set; }
        public KFloatValue(float value)
        {
            Value = value;
        }
        public override string GetFloatValue()
        {
            return Convert.ToString(Value);
        }
        public override string ToString()
        {
            return string.Format("K{0}", Convert.ToString(Value));
        }
    }
}

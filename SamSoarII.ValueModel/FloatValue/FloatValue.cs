using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public abstract class FloatValue
    {
        public uint Index { get; set; }
        public VWordValue Offset { get; set; }
        public abstract string GetFloatValue();
        public virtual string ToShowString()
        {
            return ToString();
        }
        public static FloatValue Null { get { return _nullFloatValue; } }
        private static NullFloatValue _nullFloatValue = new NullFloatValue();
    }
}

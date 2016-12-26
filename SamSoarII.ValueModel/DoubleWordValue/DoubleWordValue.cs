using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public abstract class DoubleWordValue
    {
        protected uint Index { get; set; }
        protected VWordValue Offset { get; set; }
        public abstract string GetDoubleWordValue();
        public virtual string ToShowString()
        {
            return ToString();
        }

        public static DoubleWordValue Null { get { return _nullDoubleWordValue; } }
        private static NullDoubleWordValue _nullDoubleWordValue = new NullDoubleWordValue();
    }
}

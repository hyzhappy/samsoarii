using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public abstract class BitValue
    {
        protected uint Index { get; set; }
        protected VWordValue Offset { get; set; }
        public abstract string GetBitValue();
        public abstract string GetInputImBitAddress();
        public abstract string GetOutputImBitAddress();
        public virtual string ToShowString()
        {
            return ToString();
        }

        public static BitValue Null { get { return _nullBitValue; } }
        private static NullBitValue _nullBitValue = new NullBitValue();
    }
}

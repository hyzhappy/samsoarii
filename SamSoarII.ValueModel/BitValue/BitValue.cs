using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public abstract class BitValue : IValueModel
    {
        public LadderValueType Type
        {
            get
            {
                return LadderValueType.Bool;
            }
        }
        protected uint Index { get; set; }
        protected IVariableValueModel Offset { get; set; }
        public abstract string GetBitValue();
        public abstract string GetInputImBitAddress();
        public abstract string GetOutputImBitAddress();
        public virtual string ToShowString()
        {
            return ToString();
        }

        public string GetValue()
        {
            return GetBitValue();
        }

        public static BitValue Null { get { return _nullBitValue; } }

        private static NullBitValue _nullBitValue = new NullBitValue();
    }
}

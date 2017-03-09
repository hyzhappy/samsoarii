using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public abstract class WordValue : IValueModel
    {
        public LadderValueType Type
        {
            get
            {
                return LadderValueType.Word;
            }
        }
        protected uint Index { get; set; }
        protected IVariableValueModel Offset { get; set; }
        public abstract string GetWordValue();
        public virtual string ToShowString()
        {
            return ToString();
        }

        public string GetValue()
        {
            return GetWordValue();
        }

        public static WordValue Null { get { return _nullWordValue; } }
        private static NullWordValue _nullWordValue = new NullWordValue();
    }
}

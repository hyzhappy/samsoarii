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

        public abstract string ValueString { get; }
        public abstract string ValueShowString { get; }
        protected uint Index { get; set; }
        protected WordValue Offset { get; set; }
        public abstract string GetValue();
        public virtual string Comment
        {
            get
            {
                return ValueCommentManager.GetComment(this);
            }
            set
            {
                ValueCommentManager.UpdateComment(this, value);
            }
        }
        public virtual bool IsVariable { get { return false; } }
        public static BitValue Null { get { return _nullBitValue; } }

        private static NullBitValue _nullBitValue = new NullBitValue();
    }
}

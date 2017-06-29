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

        public virtual string ValueString
        {
            get
            {
                return string.Format("{0}{1}{2}", Base, Index, Offset.ValueString);
            }
        }
        public virtual string ValueBaseString
        {
            get
            {
                return String.Format("{0}{1}", Base, Index);
            }
        }
        public abstract string ValueShowString { get; }
        public string Base { get; protected set; }
        public uint Index { get; protected set; }
        public WordValue Offset { get; protected set; }
        public virtual string GetValue()
        {
            if (Offset != WordValue.Null)
            {
                return string.Format("{0}Bit[{1} + {2}]", Base, Index, Offset.GetValue());
            }
            else
            {
                return string.Format("{0}Bit[{1}]", Base, Index);
            }
        }
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

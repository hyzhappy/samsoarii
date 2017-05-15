using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public abstract class DoubleWordValue : IValueModel
    {
        public LadderValueType Type
        {
            get
            {
                return LadderValueType.DoubleWord;
            }
        }
        public string Base { get; protected set; }
        public uint Index { get; protected set; }
        public WordValue Offset { get; protected set; }
        public virtual string ValueString
        {
            get
            {
                return string.Format("{0}{1}{2}", Base, Index, Offset.ValueString);
            }
        }
        public abstract string ValueShowString { get; }
        public string Comment
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
        public virtual string GetValue()
        {
            if (Offset != WordValue.Null)
            {
                return string.Format("{0}DoubleWord[{1} + {2}]", Base, Index, Offset.GetValue());
            }
            else
            {
                return string.Format("{0}DoubleWord[{1}]", Base, Index);
            }
        }
        public static DoubleWordValue Null { get { return _nullDoubleWordValue; } }
        private static NullDoubleWordValue _nullDoubleWordValue = new NullDoubleWordValue();
    }
}

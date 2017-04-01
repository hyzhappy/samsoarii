using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public abstract class FloatValue : IValueModel
    {
        public LadderValueType Type
        {
            get
            {
                return LadderValueType.Float;
            }
        }
        public uint Index { get; set; }
        public WordValue Offset { get; set; }
        public abstract string ValueString { get; }
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
        public abstract string GetValue();
        public static FloatValue Null { get { return _nullFloatValue; } }
        private static NullFloatValue _nullFloatValue = new NullFloatValue();
    }
}

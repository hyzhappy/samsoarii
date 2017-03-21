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
        public abstract string ValueString { get; }
        public abstract string ValueShowString { get; }
        protected uint Index { get; set; }
        protected WordValue Offset { get; set; }
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
        public static WordValue Null { get { return _nullWordValue; } }
        private static NullWordValue _nullWordValue = new NullWordValue();
    }
}

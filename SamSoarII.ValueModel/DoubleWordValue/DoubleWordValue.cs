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
        protected uint Index { get; set; }
        protected WordValue Offset { get; set; }
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
        public static DoubleWordValue Null { get { return _nullDoubleWordValue; } }
        private static NullDoubleWordValue _nullDoubleWordValue = new NullDoubleWordValue();
    }
}

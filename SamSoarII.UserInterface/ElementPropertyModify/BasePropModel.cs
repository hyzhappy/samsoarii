using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SamSoarII.UserInterface
{
    public abstract class BasePropModel : UserControl
    {
        public abstract string InstructionName { get; set; }
        public abstract int Count { get; set; }
        public abstract string ValueString1 { get; set; }
        public abstract string ValueString2 { get; set; }
        public abstract string ValueString3 { get; set; }
        public abstract string ValueString4 { get; set; }
        public abstract string ValueString5 { get; set; }
        public virtual string CommentString1 { get; set; }
        public virtual string CommentString2 { get; set; }
        public virtual string CommentString3 { get; set; }
        public virtual string CommentString4 { get; set; }
        public virtual string CommentString5 { get; set; }

        protected void UpdateComment(string valuestring)
        {
            if (ValueString1.Equals(valuestring))
                CommentString1 = ValueCommentManager.GetComment(valuestring);
            if (ValueString2.Equals(valuestring))
                CommentString2 = ValueCommentManager.GetComment(valuestring);
            if (ValueString3.Equals(valuestring))
                CommentString3 = ValueCommentManager.GetComment(valuestring);
            if (ValueString4.Equals(valuestring))
                CommentString4 = ValueCommentManager.GetComment(valuestring);
            if (ValueString5.Equals(valuestring))
                CommentString5 = ValueCommentManager.GetComment(valuestring);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class StringValue : IValueModel
    {
        public string Comment
        {
            get; set;
        }

        public bool IsVariable
        {
            get
            {
                return false;
            }
        }

        public LadderValueType Type
        {
            get
            {
                return LadderValueType.String;
            }
        }

        public string value;

        public string ValueShowString
        {
            get
            {
                return value;
            }
        }

        public string ValueString
        {
            get
            {
                return value;
            }
        }

        public string GetValue()
        {
            return value;
        }

        public StringValue(string _value)
        {
            value = _value;
        }
    }
}

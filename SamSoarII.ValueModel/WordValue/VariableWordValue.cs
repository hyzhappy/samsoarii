using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class VariableWordValue : WordValue, IVariableValue
    {
        public bool IsAnonymous
        {
            get; set;
        }

        public override bool IsVariable
        {
            get
            {
                return true;
            }
        }

        private WordValue _mappedValue;
        public IValueModel MappedValue
        {
            get
            {
                return _mappedValue;
            }
            set
            {
                var temp = value as WordValue;
                if (temp != null)
                {
                    _mappedValue = temp;
                }
                else
                {
                    _mappedValue = WordValue.Null;
                }
            }
        }

        public override string ValueShowString
        {
            get
            {
                return ValueString;
            }
        }

        public override string ValueString
        {
            get
            {
                return string.Format("{{{0}}}", VarName);
            }
        }

        public string VarName
        {
            get;set;
        }

        public override string GetValue()
        {
            if(IsAnonymous)
            {
                return MappedValue.GetValue();
            }
            else
            {
                return VarName;
            }
        }

        public VariableWordValue(string name, WordValue mappedvalue, string comment)
        {
            VarName = name;
            MappedValue = mappedvalue;
            Comment = comment;
            IsAnonymous = false;
        }

        public VariableWordValue(string name, string comment)
        {
            VarName = name;
            Comment = comment;
            IsAnonymous = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class VariableBitValue : BitValue, IVariableValue
    {
        public bool IsAnonymous
        {
            get;set;
        }

        public override bool IsVariable
        {
            get
            {
                return true;
            }
        }

        private BitValue _mappedValue;

        public IValueModel MappedValue
        {
            get
            {
                return _mappedValue;
            }
            set
            {
                var temp = value as BitValue;
                if(temp != null)
                {
                    _mappedValue = temp;
                }
                else
                {
                    _mappedValue = BitValue.Null;
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
                if (IsAnonymous)
                {
                    return string.Format("{{{0}}}", VarName);
                }
                else
                {
                    return MappedValue.ValueString;
                }
            }
        }


        public string VarName
        {
            get;set;
        }

        public override string GetValue()
        {
            if(!IsAnonymous)
            {
                return MappedValue.GetValue();
            }
            else
            {
                return VarName;
            }
        }


        /// <summary>
        /// 非匿名变量
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="mappedvalue"></param>
        /// <param name="comment"></param>
        public VariableBitValue(string varname, BitValue mappedvalue, string comment)
        {
            VarName = varname;
            MappedValue = mappedvalue;
            Comment = comment;
            IsAnonymous = false;
        }

        /// <summary>
        /// 匿名变量
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="comment"></param>
        public VariableBitValue(string varname, string comment)
        {
            VarName = varname;         
            Comment = comment;
            IsAnonymous = false;
        }
    }
}

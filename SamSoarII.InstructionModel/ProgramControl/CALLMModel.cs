using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;

namespace SamSoarII.LadderInstModel
{
    public class CALLMModel : BaseModel
    {
        public string FunctionName { get; set; }
        public ArgumentValue Value1 { get; set; }
        public ArgumentValue Value2 { get; set; }
        public ArgumentValue Value3 { get; set; }
        public ArgumentValue Value4 { get; set; }

        public CALLMModel()
        {
            FunctionName = string.Empty;
            Value1 = ArgumentValue.Null;
            Value2 = ArgumentValue.Null;
            Value3 = ArgumentValue.Null;
            Value4 = ArgumentValue.Null;
        }

        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{1}({2},{3});\r\n}}\r\n",ImportVaribleName,FunctionName,Value1.GetValue(),Value2.GetValue());
        }
        public override int ParaCount
        {
            get
            {
                return 5;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return new StringValue(FunctionName);
                case 1: return Value1;
                case 2: return Value2;
                case 3: return Value3;
                case 4: return Value4;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: FunctionName = value.ValueString; break;
                case 1: Value1 = (ArgumentValue)value; break;
                case 2: Value2 = (ArgumentValue)value; break;
                case 3: Value3 = (ArgumentValue)value; break;
                case 4: Value4 = (ArgumentValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

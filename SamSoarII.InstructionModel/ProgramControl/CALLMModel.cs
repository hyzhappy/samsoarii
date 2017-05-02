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
    }
}

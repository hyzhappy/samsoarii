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
        public WordValue Value1 { get; set; }
        public BitValue Value2 { get; set; }
        public CALLMModel()
        {
            FunctionName = string.Empty;
            Value1 = WordValue.Null;
            Value2 = BitValue.Null;
        }
        public CALLMModel(string functionName)
        {
            FunctionName = functionName;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{1}({2},{3});\r\n}}\r\n",ImportVaribleName,FunctionName,Value1.GetWordValue(),Value2.GetBitValue());
        }
    }
}

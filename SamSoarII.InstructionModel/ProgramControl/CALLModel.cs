using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class CALLModel : BaseModel
    {
        public string FunctionName { get; set; }
        public CALLModel()
        {
            FunctionName = string.Empty;
        }
        public CALLModel(string functionName)
        {
            FunctionName = functionName;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{1}();\r\n}}\r\n", ImportVaribleName, FunctionName);
        }
    }
}

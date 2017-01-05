using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.InstructionModel
{
    public class CALLMModel : BaseModel
    {
        public string FunctionName { get; set; }
        public CALLMModel()
        {

        }
        public CALLMModel(string functionName)
        {
            FunctionName = functionName;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class WTODModel : BaseModel
    {
        public WordValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }

        public WTODModel()
        {
            InputValue = WordValue.Null;
            OutputValue = DoubleWordValue.Null;
            TotalVaribleCount = 0;
        }

        public WTODModel(WordValue input, DoubleWordValue output)
        {
            InputValue = input;
            OutputValue = output;
            TotalVaribleCount = 0;
        }

        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{1}=(int32_t){2};\r\n}}\r\n", ImportVaribleName, OutputValue.GetDoubleWordValue(), InputValue.GetWordValue());
        }
    }
}

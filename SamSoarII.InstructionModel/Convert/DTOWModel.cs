using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class DTOWModel : BaseModel
    {
        public DoubleWordValue InputValue { get; set; }
        public WordValue OutputValue { get; set; }

        public DTOWModel()
        {
            InputValue = DoubleWordValue.Null;
            OutputValue = WordValue.Null;
            TotalVaribleCount = 0;
        }

        public DTOWModel(DoubleWordValue input, WordValue output)
        {
            InputValue = input;
            OutputValue = output;
            TotalVaribleCount = 0;
        }

        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{1}=(int16_t){2};\r\n}}\r\n", ImportVaribleName, InputValue.GetDoubleWordValue(), OutputValue.GetWordValue());
        }
    }
}

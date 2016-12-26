using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.InstructionModel
{
    public class BINModel : BaseModel
    {
        public WordValue InputValue { get; set; }
        public WordValue OutputValue { get; set; }
        public BINModel()
        {
            InputValue = WordValue.Null;
            OutputValue = WordValue.Null;
        }
        public BINModel(WordValue inputValue, WordValue outputValue)
        {
            InputValue = inputValue;
            OutputValue = outputValue;
        }

        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

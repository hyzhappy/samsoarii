using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class MULModel : BaseModel
    {
        public WordValue InputValue1 { get; set; }
        public WordValue InputValue2 { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public MULModel()
        {
            InputValue1 = WordValue.Null;
            InputValue2 = WordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }

        public MULModel(WordValue inputValue1, WordValue inputValue2, DoubleWordValue outputValue)
        {
            InputValue1 = inputValue1;
            InputValue2 = inputValue2;
            OutputValue = outputValue;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class BCDModel : BaseModel
    {
        public WordValue InputValue { get; set; }
        public WordValue OutputValue { get; set; }
        public BCDModel()
        {
            InputValue = WordValue.Null;
            OutputValue = WordValue.Null;
        }
        public BCDModel(WordValue inputValue, WordValue outputValue)
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

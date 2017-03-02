using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class INCModel : BaseModel
    {
        public WordValue InputValue { get; set; }
        public WordValue OutputValue { get; set; }
        public INCModel()
        {
            InputValue = WordValue.Null;
            OutputValue = WordValue.Null;
        }
        public INCModel(WordValue inputValue, WordValue outputValue)
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

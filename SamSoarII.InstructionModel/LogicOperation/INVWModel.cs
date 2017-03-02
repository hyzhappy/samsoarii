using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class INVWModel : BaseModel
    {
        public WordValue InputValue { get; set; }
        public WordValue OutputValue { get; set; }
        public INVWModel()
        {
            InputValue = WordValue.Null;
            OutputValue = WordValue.Null;
        }
        public INVWModel(WordValue inputValue, WordValue outputValue)
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

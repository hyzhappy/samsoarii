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
            return string.Format("if({0})\r\n{{\r\n{2} = ~{1};\r\n}}\r\n", ImportVaribleName,InputValue.GetValue(),OutputValue.GetValue());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class ANDWModel : BaseModel
    {
        public WordValue InputValue1 { get; set; }
        public WordValue InputValue2 { get; set; }
        public WordValue OutputValue { get; set; }
        public ANDWModel()
        {
            InputValue1 = WordValue.Null;
            InputValue2 = WordValue.Null;
            OutputValue = WordValue.Null;
        }
        public ANDWModel(WordValue inputValue1, WordValue inputValue2, WordValue outputValue)
        {
            InputValue1 = inputValue1;
            InputValue2 = inputValue2;
            OutputValue = outputValue;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{3} = {1} & {2};\r\n}}\r\n",ImportVaribleName,InputValue1.GetWordValue(),InputValue2.GetWordValue(),OutputValue.GetWordValue());
        }
    }
}

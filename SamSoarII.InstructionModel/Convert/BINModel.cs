using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
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
            return string.Format("if({0})\r\n{{\r\nint16_t InputValue = {1};\r\nif(InputValue >= 0 && InputValue <= 9999)\r\n{{\r\nint16_t temp = InputValue & 0xf000;\r\ntemp >>= 12;\r\n{2} += temp * 1000;\r\ntemp = InputValue & 0x0f00;\r\ntemp >>= 8;\r\n{2} += temp * 100;\r\ntemp = InputValue & 0x00f0;\r\ntemp >>= 4;\r\n{2} += temp * 10;\r\ntemp = InputValue & 0x000f;\r\n{2} += temp;\r\n}}\r\nelse\r\n{{\r\nMBit[8168] = 1;\r\n}}\r\n}}\r\n", ImportVaribleName,InputValue.GetWordValue(),OutputValue.GetWordValue());
        }
    }
}

using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class INCDModel : BaseModel
    {
        public DoubleWordValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public INCDModel()
        {
            InputValue = DoubleWordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }
        public INCDModel(DoubleWordValue inputValue, DoubleWordValue outputValue)
        {
            InputValue = inputValue;
            OutputValue = outputValue;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nint64_t temp = {1} + 1;\r\nif(temp != (int32_t)temp)\r\n{{\r\nMBit[8169] = 1;\r\n}}\r\nelse\r\n{{\r\n{2} = {1} + 1;\r\nif({2} < 0)\r\n{{\r\nMBit[8170] = 1;\r\n}}\r\nelse if({2} == 0)\r\n{{r\nMBit[8171] = 1;\r\n}}\r\n}}\r\n}}\r\n", ImportVaribleName, InputValue.GetValue(), OutputValue.GetValue());
        }
    }
}

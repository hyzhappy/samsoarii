using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class DIVDModel : BaseModel
    {
        public DoubleWordValue InputValue1 { get; set; }
        public DoubleWordValue InputValue2 { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public DIVDModel()
        {
            InputValue1 = DoubleWordValue.Null;
            InputValue2 = DoubleWordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }
        public DIVDModel(DoubleWordValue inputValue1, DoubleWordValue inputValue2, DoubleWordValue outputValue)
        {
            InputValue1 = inputValue1;
            InputValue2 = inputValue2;
            OutputValue = outputValue;
        }

        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nif({2} == 0)\r\n{{\r\nMBit[8172] = 1;\r\n}}\r\nelse\r\n{{\r\nint64_t temp = {1} / {2};\r\nif(temp != (int32_t)temp)\r\n{{\r\nMBit[8169] = 1;\r\n}}\r\nelse\r\n{{\r\n{3} = {1} / {2};\r\nif({3} < 0)\r\n{{\r\nMBit[8170] = 1;\r\n}}\r\nelse if({3} == 0)\r\n{{\r\nMBit[8171] = 1;\r\n}}\r\n}}\r\n}}\r\n}}\r\n", ImportVaribleName, InputValue1.GetValue(), InputValue2.GetValue(), OutputValue.GetValue());
        }
    }
}

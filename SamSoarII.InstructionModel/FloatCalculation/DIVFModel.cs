using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class DIVFModel : BaseModel
    {
        public FloatValue InputValue1 { get; set; }
        public FloatValue InputValue2 { get; set; }
        public FloatValue OutputValue { get; set; }
        public DIVFModel()
        {
            InputValue1 = FloatValue.Null;
            InputValue2 = FloatValue.Null;
            OutputValue = FloatValue.Null;
        }

        public DIVFModel(FloatValue inputValue1, FloatValue inputValue2, FloatValue outputValue)
        {
            InputValue1 = inputValue1;
            InputValue2 = inputValue2;
            OutputValue = outputValue;
        }

        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nif({2} == 0)\r\n{{\r\nMBit[8172] = 1;\r\n}}\r\nelse\r\n{{\r\n{3} = {1} / {2};\r\nif({3} == 0)\r\n{{\r\nMBit[8171] = 1;\r\n}}\r\nelse if({3} < 0)\r\n{{\r\nMBit[8170] = 1;\r\n}}\r\n}}\r\n}}\r\n", ImportVaribleName, InputValue1.GetValue(), InputValue2.GetValue(), OutputValue.GetValue());
        }
    }
}

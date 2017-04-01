using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class LNModel : BaseModel
    {
        public FloatValue InputValue { get; set; }
        public FloatValue OutputValue { get; set; }
        public LNModel(FloatValue inputValue, FloatValue outputValue)
        {
            InputValue = inputValue;
            OutputValue = outputValue;
        }
        public LNModel()
        {
            InputValue = FloatValue.Null;
            OutputValue = FloatValue.Null;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nif({1} > 0)\r\n{{\r\n{2} = log({1});\r\nif({2} == 0)\r\n{{\r\nMBit[8171] = 1;\r\n}}\r\nelse if({2} < 0)\r\n{{\r\nMBit[8170] = 1;\r\n}}\r\n}}\r\n}}\r\n", ImportVaribleName,InputValue.GetValue(),OutputValue.GetValue());
        }
    }
}

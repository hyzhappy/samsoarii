﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class XORDModel : BaseModel
    {
        public DoubleWordValue InputValue1 { get; set; }
        public DoubleWordValue InputValue2 { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public XORDModel()
        {
            InputValue1 = DoubleWordValue.Null;
            InputValue2 = DoubleWordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }
        public XORDModel(DoubleWordValue inputValue1, DoubleWordValue inputValue2, DoubleWordValue outputValue)
        {
            InputValue1 = inputValue1;
            InputValue2 = inputValue2;
            OutputValue = outputValue;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{3} = {1} ^ {2};\r\n}}\r\n", ImportVaribleName, InputValue1.GetValue(), InputValue2.GetValue(), OutputValue.GetValue());
        }
    }
}

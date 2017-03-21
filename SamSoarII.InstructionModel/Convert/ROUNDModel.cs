using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class ROUNDModel : BaseModel
    {
        public FloatValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public ROUNDModel()
        {
            InputValue = FloatValue.Null;
            OutputValue = DoubleWordValue.Null;
        }
        public ROUNDModel(FloatValue inputValue, DoubleWordValue outputValue)
        {
            InputValue = inputValue;
            OutputValue = outputValue;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nint32_t temp = (int32_t){1};\r\nif(temp + 0.5 > {1})\r\n{{\r\n{2} = temp;\r\n}}\r\nelse\r\n{{\r\n{2} = temp + 1;\r\n}}\r\n}}\r\n", ImportVaribleName,InputValue.GetValue(),OutputValue.GetValue());
        }
    }
}

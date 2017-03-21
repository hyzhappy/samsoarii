using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class TRUNCModel : BaseModel
    {
        public FloatValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public TRUNCModel()
        {
            InputValue = FloatValue.Null;
            OutputValue = DoubleWordValue.Null;
        }
        public TRUNCModel(FloatValue inputValue, DoubleWordValue outputValue)
        {
            InputValue = inputValue;
            OutputValue = outputValue;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{2} = (int32_t){1};\r\n}}\r\n",ImportVaribleName,InputValue.GetFloatValue(),OutputValue.GetDoubleWordValue());
        }
    }
}

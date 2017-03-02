using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class DTOFModel : BaseModel
    {
        public DoubleWordValue InputValue { get; set; }
        public FloatValue OutputValue { get; set; }

        public DTOFModel()
        {
            InputValue = DoubleWordValue.Null;
            OutputValue = FloatValue.Null;
            TotalVaribleCount = 0;
        }

        public DTOFModel(DoubleWordValue input, FloatValue output)
        {
            InputValue = input;
            OutputValue = output;
            TotalVaribleCount = 0;
        }

        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{1}=(float){2};\r\n}}\r\n", ImportVaribleName, OutputValue.GetFloatValue(), InputValue.GetDoubleWordValue());
        }
    }
}

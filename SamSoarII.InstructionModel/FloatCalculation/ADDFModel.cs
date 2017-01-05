using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.InstructionModel
{
    public class ADDFModel : BaseModel
    {
        public FloatValue InputValue1 { get; set; }
        public FloatValue InputValue2 { get; set; }
        public FloatValue OutputValue { get; set; }
        public ADDFModel()
        {
            InputValue1 = FloatValue.Null;
            InputValue2 = FloatValue.Null;
            OutputValue = FloatValue.Null;
        }

        public ADDFModel(FloatValue inputValue1, FloatValue inputValue2, FloatValue outputValue)
        {
            InputValue1 = inputValue1;
            InputValue2 = inputValue2;
            OutputValue = outputValue;
        }

        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

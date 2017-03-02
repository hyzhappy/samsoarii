using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class SINModel : BaseModel
    {
        public FloatValue InputValue { get; set; }
        public FloatValue OutputValue { get; set; }
        public SINModel(FloatValue inputValue, FloatValue outputValue)
        {
            InputValue = inputValue;
            OutputValue = outputValue;
        }
        public SINModel()
        {
            InputValue = FloatValue.Null;
            OutputValue = FloatValue.Null;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

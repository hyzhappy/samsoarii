using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class COSModel : BaseModel
    {
        public FloatValue InputValue { get; set; }
        public FloatValue OutputValue { get; set; }
        public COSModel(FloatValue inputValue, FloatValue outputValue)
        {
            InputValue = inputValue;
            OutputValue = outputValue;
        }
        public COSModel()
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

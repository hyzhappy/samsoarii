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
            throw new NotImplementedException();
        }
    }
}

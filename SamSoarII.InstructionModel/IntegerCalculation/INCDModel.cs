using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class INCDModel : BaseModel
    {
        public DoubleWordValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public INCDModel()
        {
            InputValue = DoubleWordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }
        public INCDModel(DoubleWordValue inputValue, DoubleWordValue outputValue)
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

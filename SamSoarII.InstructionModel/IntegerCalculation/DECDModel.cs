using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class DECDModel : BaseModel
    {
        public DoubleWordValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public DECDModel()
        {
            InputValue = DoubleWordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }
        public DECDModel(DoubleWordValue inputValue, DoubleWordValue outputValue)
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

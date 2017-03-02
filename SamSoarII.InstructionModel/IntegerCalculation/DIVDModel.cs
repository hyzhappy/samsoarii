using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class DIVDModel : BaseModel
    {
        public DoubleWordValue InputValue1 { get; set; }
        public DoubleWordValue InputValue2 { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public DIVDModel()
        {
            InputValue1 = DoubleWordValue.Null;
            InputValue2 = DoubleWordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }
        public DIVDModel(DoubleWordValue inputValue1, DoubleWordValue inputValue2, DoubleWordValue outputValue)
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

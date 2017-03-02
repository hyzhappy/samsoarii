using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class INVDModel : BaseModel
    {
        public DoubleWordValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public INVDModel()
        {
            InputValue = DoubleWordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }
        public INVDModel(DoubleWordValue inputValue, DoubleWordValue outputValue)
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

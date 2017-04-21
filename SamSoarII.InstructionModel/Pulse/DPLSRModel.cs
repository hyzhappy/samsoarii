using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class DPLSRModel : BaseModel
    {
        public DoubleWordValue ArgumentValue { get; set; }
        public DoubleWordValue VelocityValue { get; set; }
        public BitValue OutputValue { get; set; }

        public DPLSRModel()
        {
            ArgumentValue = DoubleWordValue.Null;
            VelocityValue = DoubleWordValue.Null;
            OutputValue = BitValue.Null;
        }

        public DPLSRModel(DoubleWordValue _ArgumentValue, DoubleWordValue _VelocityValue, BitValue _OutputValue)
        {
            ArgumentValue = _ArgumentValue;
            VelocityValue = _VelocityValue;
            OutputValue = _OutputValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

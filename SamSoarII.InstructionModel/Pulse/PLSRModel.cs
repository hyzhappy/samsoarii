using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class PLSRModel : BaseModel
    {
        public WordValue ArgumentValue { get; set; }
        public WordValue VelocityValue { get; set; }
        public BitValue OutputValue { get; set; }

        public PLSRModel()
        {
            ArgumentValue = WordValue.Null;
            VelocityValue = WordValue.Null;
            OutputValue = BitValue.Null;
        }

        public PLSRModel(WordValue _ArgumentValue, WordValue _VelocityValue, BitValue _OutputValue)
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

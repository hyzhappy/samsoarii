using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class PLSRDModel : BaseModel
    {
        public WordValue ArgumentValue { get; set; }
        public WordValue VelocityValue { get; set; }
        public BitValue OutputValue1 { get; set; }
        public BitValue OutputValue2 { get; set; }

        public PLSRDModel()
        {
            ArgumentValue = WordValue.Null;
            VelocityValue = WordValue.Null;
            OutputValue1 = BitValue.Null;
            OutputValue2 = BitValue.Null;
        }

        public PLSRDModel(WordValue _ArgumentValue, WordValue _VelocityValue, BitValue _OutputValue1, BitValue _OutputValue2)
        {
            ArgumentValue = _ArgumentValue;
            VelocityValue = _VelocityValue;
            OutputValue1 = _OutputValue1;
            OutputValue2 = _OutputValue2;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

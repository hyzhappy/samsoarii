using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class PLSYModel : BaseModel
    {
        public WordValue FreqValue { get; set; }
        public WordValue PulseValue { get; set; }
        public BitValue OutputValue { get; set; }

        public PLSYModel()
        {
            FreqValue = WordValue.Null;
            PulseValue = WordValue.Null;
            OutputValue = BitValue.Null;
        }

        public PLSYModel(WordValue _FreqValue, WordValue _PulseValue, BitValue _OutputValue)
        {
            FreqValue = _FreqValue;
            PulseValue = _PulseValue;
            OutputValue = _OutputValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

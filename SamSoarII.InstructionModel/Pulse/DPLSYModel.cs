using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class DPLSYModel : BaseModel
    {
        public DoubleWordValue FreqValue { get; set; }
        public DoubleWordValue PulseValue { get; set; }
        public BitValue OutputValue { get; set; }

        public DPLSYModel()
        {
            FreqValue = DoubleWordValue.Null;
            PulseValue = DoubleWordValue.Null;
            OutputValue = BitValue.Null;
        }

        public DPLSYModel(DoubleWordValue _FreqValue, DoubleWordValue _PulseValue, BitValue _OutputValue)
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

using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class DDRVIModel : BaseModel
    {
        public DoubleWordValue FreqValue;
        public DoubleWordValue PulseValue;
        public BitValue OutputValue;

        public DDRVIModel()
        {
            FreqValue = DoubleWordValue.Null;
            PulseValue = DoubleWordValue.Null;
            OutputValue = BitValue.Null;
        }

        public DDRVIModel(DoubleWordValue _FreqValue, DoubleWordValue _PulseValue, BitValue _OutputValue)
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

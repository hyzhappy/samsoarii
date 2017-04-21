using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class DRVIModel : BaseModel
    {
        public WordValue FreqValue;
        public WordValue PulseValue;
        public BitValue OutputValue;

        public DRVIModel()
        {
            FreqValue = WordValue.Null;
            PulseValue = WordValue.Null;
            OutputValue = BitValue.Null;
        }

        public DRVIModel(WordValue _FreqValue, WordValue _PulseValue, BitValue _OutputValue)
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

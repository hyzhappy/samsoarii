using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class DPLSFModel : BaseModel
    {
        public DoubleWordValue FreqValue { get; set; }
        public BitValue OutputValue { get; set; }
        public DPLSFModel()
        {
            FreqValue = DoubleWordValue.Null;
            OutputValue = BitValue.Null;
        }

        public DPLSFModel(DoubleWordValue _FreqValue, BitValue _OutputValue)
        {
            FreqValue = _FreqValue;
            OutputValue = _OutputValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }

    }
}

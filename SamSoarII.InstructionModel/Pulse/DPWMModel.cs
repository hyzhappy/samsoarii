using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class DPWMModel : BaseModel
    {
        public DoubleWordValue FreqValue { get; set; }
        public DoubleWordValue DutyCycleValue { get; set; }
        public BitValue OutputValue { get; set; }

        public DPWMModel()
        {
            FreqValue = DoubleWordValue.Null;
            DutyCycleValue = DoubleWordValue.Null;
            OutputValue = BitValue.Null;
        }

        public DPWMModel(DoubleWordValue _FreqValue, DoubleWordValue _DutyCycleValue, BitValue _OutputValue)
        {
            FreqValue = _FreqValue;
            DutyCycleValue = _DutyCycleValue;
            OutputValue = _OutputValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

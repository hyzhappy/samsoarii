using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class PWMModel : BaseModel
    {
        public WordValue FreqValue { get; set; }
        public WordValue DutyCycleValue { get; set; }
        public BitValue OutputValue { get; set; }
        
        public PWMModel()
        {
            FreqValue = WordValue.Null;
            DutyCycleValue = WordValue.Null;
            OutputValue = BitValue.Null;
        }

        public PWMModel(WordValue _FreqValue, WordValue _DutyCycleValue, BitValue _OutputValue)
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

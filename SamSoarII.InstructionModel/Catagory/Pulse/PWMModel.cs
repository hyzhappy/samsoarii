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
        public override string InstructionName => "PWM";
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
        public override int ParaCount
        {
            get
            {
                return 3;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return FreqValue;
                case 1: return DutyCycleValue;
                case 2: return OutputValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: FreqValue = (WordValue)value; break;
                case 1: DutyCycleValue = (WordValue)value; break;
                case 2: OutputValue = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

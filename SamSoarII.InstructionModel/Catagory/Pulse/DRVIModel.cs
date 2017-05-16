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
        public override string InstructionName => "DRVI";
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
                case 1: return PulseValue;
                case 2: return OutputValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: FreqValue = (WordValue)value; break;
                case 1: PulseValue = (WordValue)value; break;
                case 2: OutputValue = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

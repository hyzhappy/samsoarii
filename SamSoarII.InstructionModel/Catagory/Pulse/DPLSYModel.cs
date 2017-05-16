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
        public override string InstructionName => "DPLSY";
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
                case 0: FreqValue = (DoubleWordValue)value; break;
                case 1: PulseValue = (DoubleWordValue)value; break;
                case 2: OutputValue = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

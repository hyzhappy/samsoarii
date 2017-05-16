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
        public override string InstructionName => "DPLSF";
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

        public override int ParaCount
        {
            get
            {
                return 2;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return FreqValue;
                case 1: return OutputValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: FreqValue = (DoubleWordValue)value; break;
                case 1: OutputValue = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

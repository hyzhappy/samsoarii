using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class DPLSRModel : BaseModel
    {
        public override string InstructionName => "DPLSR";
        public DoubleWordValue ArgumentValue { get; set; }
        public DoubleWordValue VelocityValue { get; set; }
        public BitValue OutputValue { get; set; }

        public DPLSRModel()
        {
            ArgumentValue = DoubleWordValue.Null;
            VelocityValue = DoubleWordValue.Null;
            OutputValue = BitValue.Null;
        }

        public DPLSRModel(DoubleWordValue _ArgumentValue, DoubleWordValue _VelocityValue, BitValue _OutputValue)
        {
            ArgumentValue = _ArgumentValue;
            VelocityValue = _VelocityValue;
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
                case 0: return ArgumentValue;
                case 1: return VelocityValue;
                case 2: return OutputValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: ArgumentValue = (DoubleWordValue)value; break;
                case 1: VelocityValue = (DoubleWordValue)value; break;
                case 2: OutputValue = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

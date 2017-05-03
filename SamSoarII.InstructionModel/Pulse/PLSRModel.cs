using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class PLSRModel : BaseModel
    {
        public WordValue ArgumentValue { get; set; }
        public WordValue VelocityValue { get; set; }
        public BitValue OutputValue { get; set; }

        public PLSRModel()
        {
            ArgumentValue = WordValue.Null;
            VelocityValue = WordValue.Null;
            OutputValue = BitValue.Null;
        }

        public PLSRModel(WordValue _ArgumentValue, WordValue _VelocityValue, BitValue _OutputValue)
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
                case 0: ArgumentValue = (WordValue)value; break;
                case 1: VelocityValue = (WordValue)value; break;
                case 2: OutputValue = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

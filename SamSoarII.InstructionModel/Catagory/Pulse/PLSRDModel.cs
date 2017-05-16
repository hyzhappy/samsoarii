using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class PLSRDModel : BaseModel
    {
        public WordValue ArgumentValue { get; set; }
        public WordValue VelocityValue { get; set; }
        public BitValue OutputValue1 { get; set; }
        public BitValue OutputValue2 { get; set; }

        public PLSRDModel()
        {
            ArgumentValue = WordValue.Null;
            VelocityValue = WordValue.Null;
            OutputValue1 = BitValue.Null;
            OutputValue2 = BitValue.Null;
        }

        public PLSRDModel(WordValue _ArgumentValue, WordValue _VelocityValue, BitValue _OutputValue1, BitValue _OutputValue2)
        {
            ArgumentValue = _ArgumentValue;
            VelocityValue = _VelocityValue;
            OutputValue1 = _OutputValue1;
            OutputValue2 = _OutputValue2;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
        public override int ParaCount
        {
            get
            {
                return 4;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return ArgumentValue;
                case 1: return VelocityValue;
                case 2: return OutputValue1;
                case 3: return OutputValue2;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: ArgumentValue = (WordValue)value; break;
                case 1: VelocityValue = (WordValue)value; break;
                case 2: OutputValue1 = (BitValue)value; break;
                case 3: OutputValue2 = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

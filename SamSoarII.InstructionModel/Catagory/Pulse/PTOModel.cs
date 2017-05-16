using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class PTOModel : BaseModel
    {
        public override string InstructionName => "PTO";
        public WordValue ArgumentValue;
        public BitValue OutputValue1;
        public BitValue OutputValue2;


        public PTOModel()
        {
            ArgumentValue = WordValue.Null;
            OutputValue1 = BitValue.Null;
            OutputValue2 = BitValue.Null;
        }

        public PTOModel(WordValue _ArgumentValue, BitValue _OutputValue1, BitValue _OutputValue2)
        {
            ArgumentValue = _ArgumentValue;
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
                return 3;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return ArgumentValue;
                case 1: return OutputValue1;
                case 2: return OutputValue2;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: ArgumentValue = (WordValue)value; break;
                case 1: OutputValue1 = (BitValue)value; break;
                case 2: OutputValue2 = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

    }
}

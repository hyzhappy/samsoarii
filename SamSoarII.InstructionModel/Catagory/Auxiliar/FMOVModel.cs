using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class FMOVModel : BaseModel
    {
        public override string InstructionName => "FMOV";

        public WordValue SourceValue { get; set; }
        public WordValue DestinationValue { get; set; }
        public WordValue CountValue { get; set; }

        public FMOVModel()
        {
            SourceValue = WordValue.Null;
            DestinationValue = WordValue.Null;
            CountValue = WordValue.Null;
        }

        public FMOVModel(WordValue _SourceValue, WordValue _DestinationValue, WordValue _CountValue)
        {
            SourceValue = _SourceValue;
            DestinationValue = _DestinationValue;
            CountValue = _CountValue;
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
                case 0: return SourceValue;
                case 1: return DestinationValue;
                case 2: return CountValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CMP'", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: SourceValue = (WordValue)value; break;
                case 1: DestinationValue = (WordValue)value; break;
                case 2: CountValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CMP'", id));
            }
        }
    }
}

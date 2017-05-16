using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class FMOVDModel : BaseModel
    {
        public DoubleWordValue SourceValue { get; set; }
        public DoubleWordValue DestinationValue { get; set; }
        public WordValue CountValue { get; set; }

        public FMOVDModel()
        {
            SourceValue = DoubleWordValue.Null;
            DestinationValue = DoubleWordValue.Null;
            CountValue = WordValue.Null;
        }

        public FMOVDModel(DoubleWordValue _SourceValue, DoubleWordValue _DestinationValue, WordValue _CountValue)
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
                case 0: SourceValue = (DoubleWordValue)value; break;
                case 1: DestinationValue = (DoubleWordValue)value; break;
                case 2: CountValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CMP'", id));
            }
        }
    }
}

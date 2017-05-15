using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class SHLBModel : BaseModel
    {
        public BitValue SourceValue { get; set; }
        public BitValue DestinationValue { get; set; }
        public WordValue CountValue { get; set; }
        public WordValue MoveValue { get; set; }

        public SHLBModel()
        {
            SourceValue = BitValue.Null;
            DestinationValue = BitValue.Null;
            CountValue = WordValue.Null;
            MoveValue = WordValue.Null;
        }

        public override string GenerateCode()
        {
            throw new NotImplementedException();
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
                case 0: return SourceValue;
                case 1: return DestinationValue;
                case 2: return CountValue;
                case 3: return MoveValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: SourceValue = (BitValue)value; break;
                case 1: DestinationValue = (BitValue)value; break;
                case 2: CountValue = (WordValue)value; break;
                case 3: MoveValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

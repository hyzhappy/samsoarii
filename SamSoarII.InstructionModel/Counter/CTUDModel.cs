using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class CTUDModel : BaseModel
    {
        public IValueModel CountValue { get; set; }

        public IValueModel EndValue { get; set; }

        public CTUDModel()
        {

        }

        public CTUDModel(IValueModel _CountValue, IValueModel _EndValue)
        {
            CountValue = _CountValue;
            EndValue = _EndValue;
        }

        public override string GenerateCode()
        {
            return string.Empty;
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
                case 0: return CountValue;
                case 1: return EndValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: CountValue = (WordValue)value; break;
                case 1: EndValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

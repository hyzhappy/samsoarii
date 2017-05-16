using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.HighCount
{
    public class HCNTModel : BaseModel
    {
        public DoubleWordValue CountValue { get; set; }

        public DoubleWordValue DefineValue { get; set; }

        public HCNTModel()
        {
            CountValue = DoubleWordValue.Null;
            DefineValue = DoubleWordValue.Null;
        }

        public HCNTModel(DoubleWordValue _CountValue, DoubleWordValue _DefineValue)
        {
            CountValue = _CountValue;
            DefineValue = _DefineValue;
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
                case 0: return CountValue;
                case 1: return DefineValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: CountValue = (DoubleWordValue)value; break;
                case 1: DefineValue = (DoubleWordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class CTDModel : BaseModel
    {
        public override string InstructionName => "CTD";
        public IValueModel CountValue { get; set; }

        public IValueModel StartValue { get; set; }

        public CTDModel()
        {
            CountValue = WordValue.Null;
            StartValue = WordValue.Null;
        }

        public CTDModel(IValueModel _CountValue, IValueModel _StartValue)
        {
            CountValue = _CountValue;
            StartValue = _StartValue;
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
                case 1: return StartValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: CountValue = (WordValue)value; break;
                case 1: StartValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

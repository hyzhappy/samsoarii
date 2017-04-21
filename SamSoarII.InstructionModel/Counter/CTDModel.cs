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
    }
}

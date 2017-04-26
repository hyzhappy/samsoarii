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
            CountValue = WordValue.Null;
            EndValue = WordValue.Null;
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
    }
}

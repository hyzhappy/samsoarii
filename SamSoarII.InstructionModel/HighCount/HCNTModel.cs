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
    }
}

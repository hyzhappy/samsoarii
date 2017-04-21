using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class PLSFModel : BaseModel
    {
        public WordValue FreqValue { get; set; }
        public BitValue OutputValue { get; set; }
        public PLSFModel()
        {
            FreqValue = WordValue.Null;
            OutputValue = BitValue.Null;
        }

        public PLSFModel(WordValue _FreqValue, BitValue _OutputValue)
        {
            FreqValue = _FreqValue;
            OutputValue = _OutputValue;
        }
        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

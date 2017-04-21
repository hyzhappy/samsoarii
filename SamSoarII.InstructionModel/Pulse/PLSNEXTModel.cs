using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class PLSNEXTModel : BaseModel
    {
        public BitValue OutputValue { get; set; }

        public PLSNEXTModel()
        {
            OutputValue = BitValue.Null;
        }

        public PLSNEXTModel(BitValue _OutputValue)
        {
            OutputValue = _OutputValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }

}

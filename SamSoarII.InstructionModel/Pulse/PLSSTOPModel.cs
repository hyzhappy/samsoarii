using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class PLSSTOPModel : BaseModel
    {
        public BitValue OutputValue { get; set; }

        public PLSSTOPModel()
        {
            OutputValue = BitValue.Null;
        }

        public PLSSTOPModel(BitValue _OutputValue)
        {
            OutputValue = _OutputValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }

}
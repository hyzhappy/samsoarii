using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.InstructionModel
{
    public class PLSFModel : BaseModel
    {
        public WordValue FreqValue { get; set; }
        public BitValue OutputValue { get; set; }
        public PLSFModel()
        {
            FreqValue = WordValue.Null;
            
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

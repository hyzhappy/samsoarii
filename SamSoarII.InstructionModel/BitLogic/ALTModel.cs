using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.InstructionModel
{
    public class ALTModel : BaseModel
    {
        public BitValue Value { get; set; }
        public ALTModel()
        {
            Value = BitValue.Null;
        }
        public ALTModel(BitValue value)
        {
            Value = value;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

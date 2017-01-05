using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class LDIMModel : BaseModel
    {
        public BitValue Value { get; set; }
        public LDIMModel()
        {
            Value = BitValue.Null;
        }
        public LDIMModel(BitValue value)
        {
            Value = value;
        }

        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

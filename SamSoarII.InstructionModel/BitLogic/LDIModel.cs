using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.InstructionModel;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class LDIModel : BaseModel
    {
        public BitValue Value { get; set; }
        public LDIModel()
        {
            Value = BitValue.Null;
        }
        public LDIModel(BitValue value)
        {
            Value = value;
        }

        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class ALTPModel : BaseModel
    {
        public BitValue Value { get; set; }
        public ALTPModel()
        {
            Value = BitValue.Null;
        }
        public ALTPModel(BitValue value)
        {
            Value = value;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

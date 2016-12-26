using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class MEFModel : BaseModel
    {
        public BitValue Value { get; set; }
        public MEFModel()
        {
            Value = BitValue.Null;
        }
        public MEFModel(BitValue value)
        {
            Value = value;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

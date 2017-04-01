using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class LDFModel : BaseModel
    {
        public BitValue Value { get; set; }
        public LDFModel()
        {
            Value = BitValue.Null;
        }
        public LDFModel(BitValue value)
        {
            Value = value;
        }

        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

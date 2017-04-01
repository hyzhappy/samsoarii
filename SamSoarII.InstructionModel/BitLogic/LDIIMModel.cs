using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class LDIIMModel : BaseModel
    {
        public BitValue Value { get; set; }
        public LDIIMModel()
        {
            Value = BitValue.Null;
        }
        public LDIIMModel(BitValue value)
        {
            Value = value;
        }

        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

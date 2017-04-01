using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class OUTIMModel : BaseModel
    {
        public BitValue Value { get; set; }
        public OUTIMModel()
        {
            Value = BitValue.Null;
        }
        public OUTIMModel(BitValue value)
        {
            Value = value;
        }

        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

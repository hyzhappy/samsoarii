using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class MEPModel : BaseModel
    {
        public BitValue Value { get; set; }
        public MEPModel()
        {
            Value = BitValue.Null;
        }
        public MEPModel(BitValue value)
        {
            Value = value;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

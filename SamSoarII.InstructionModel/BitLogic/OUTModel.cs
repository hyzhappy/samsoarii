using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class OUTModel : BaseModel
    {

        public BitValue Value { get; set; }
        public OUTModel()
        {
            Value = BitValue.Null;
        }
        public OUTModel(BitValue value)
        {
            Value = value;
        }
        public override string GenerateCode()
        {
            return string.Format("{0} = {1};\r\n", Value.GetValue(), ImportVaribleName);
        }
    }
}

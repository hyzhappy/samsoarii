using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
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
            return string.Format("if({0})\r\n{{\r\n{1} = !{1};\r\n}}\r\n",ImportVaribleName,Value.GetValue());
        }
    }
}

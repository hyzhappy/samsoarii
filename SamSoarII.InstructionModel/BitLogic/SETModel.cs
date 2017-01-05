using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class SETModel : BaseModel
    {
        public BitValue Value { get; set; }
        public WordValue Count { get; set; }

        public SETModel()
        {
            Value = BitValue.Null;
            Count = WordValue.Null;
        }
        public SETModel(BitValue value, WordValue count)
        {
            Value = value;
            Count = count;
        }

        public override string GenerateCode()
        {
            return string.Empty;
            //return string.Format("if({0})\r\n{{\r\n for(int i = 0; i < {1}; i++)\r\n{{\r\n{2}=1;\r\n}}\r\n}}\r\n", ImportVaribleName, Count.GetWordValue(), Value.);
        }
    }
}

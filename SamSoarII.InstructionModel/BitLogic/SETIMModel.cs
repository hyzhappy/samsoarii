using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class SETIMModel : BaseModel
    {
        public BitValue Value { get; set; }
        public WordValue Count { get; set; }
        public SETIMModel()
        {
            Value = BitValue.Null;
            Count = WordValue.Null;
        }

        public SETIMModel(BitValue value, WordValue count)
        {
            Value = value;
            Count = count;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

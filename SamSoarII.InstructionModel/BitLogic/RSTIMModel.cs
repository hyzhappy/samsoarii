using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class RSTIMModel : BaseModel
    {
        public BitValue Value { get; set; }
        public WordValue Count { get; set; }
        public RSTIMModel()
        {
            Value = BitValue.Null;
            Count = WordValue.Null;
        }
        public RSTIMModel(BitValue value, WordValue count)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class RSTModel : BaseModel
    {
        public BitValue Value { get; set; }
        public WordValue Count { get; set; }
        public RSTModel()
        {
            Value = BitValue.Null;
            Count = WordValue.Null;
        }
        public RSTModel(BitValue value, WordValue count)
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

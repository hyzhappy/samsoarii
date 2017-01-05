using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.InstructionModel
{
    public class SHLDModel : BaseModel
    {
        public DoubleWordValue SourceValue { get; set; }
        public DoubleWordValue DestinationValue { get; set; }
        public WordValue Count { get; set; }
        public SHLDModel()
        {
            SourceValue = DoubleWordValue.Null;
            DestinationValue = DoubleWordValue.Null;
            Count = WordValue.Null;
        }
        public SHLDModel(DoubleWordValue sourceValue, DoubleWordValue destValue, WordValue count)
        {
            SourceValue = sourceValue;
            DestinationValue = destValue;
            Count = count;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

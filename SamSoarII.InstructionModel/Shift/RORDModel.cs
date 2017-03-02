using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class RORDModel : BaseModel
    {
        public DoubleWordValue SourceValue { get; set; }
        public DoubleWordValue DestinationValue { get; set; }
        public WordValue Count { get; set; }
        public RORDModel()
        {
            SourceValue = DoubleWordValue.Null;
            DestinationValue = DoubleWordValue.Null;
            Count = WordValue.Null;
        }
        public RORDModel(DoubleWordValue sourceValue, DoubleWordValue destValue, WordValue count)
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

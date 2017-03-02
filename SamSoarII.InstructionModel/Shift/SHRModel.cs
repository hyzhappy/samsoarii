using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class SHRModel : BaseModel
    {
        public WordValue SourceValue { get; set; }
        public WordValue DestinationValue { get; set; }
        public WordValue Count { get; set; }
        public SHRModel()
        {
            SourceValue = WordValue.Null;
            DestinationValue = WordValue.Null;
            Count = WordValue.Null;
        }
        public SHRModel(WordValue sourceValue, WordValue destValue, WordValue count)
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

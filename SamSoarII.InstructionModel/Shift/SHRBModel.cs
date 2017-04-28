using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class SHRBModel : BaseModel
    {
        public BitValue SourceValue { get; set; }
        public BitValue DestinationValue { get; set; }
        public WordValue CountValue { get; set; }
        public WordValue MoveValue { get; set; }

        public SHRBModel()
        {
            SourceValue = BitValue.Null;
            DestinationValue = BitValue.Null;
            CountValue = WordValue.Null;
            MoveValue = WordValue.Null;
        }

        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

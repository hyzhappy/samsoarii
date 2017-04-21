using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class FMOVDModel : BaseModel
    {
        public DoubleWordValue SourceValue { get; set; }
        public DoubleWordValue DestinationValue { get; set; }
        public WordValue CountValue { get; set; }

        public FMOVDModel()
        {
            SourceValue = DoubleWordValue.Null;
            DestinationValue = DoubleWordValue.Null;
            CountValue = WordValue.Null;
        }

        public FMOVDModel(DoubleWordValue _SourceValue, DoubleWordValue _DestinationValue, WordValue _CountValue)
        {
            SourceValue = _SourceValue;
            DestinationValue = _DestinationValue;
            CountValue = _CountValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

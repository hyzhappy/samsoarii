using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class FMOVModel : BaseModel
    {
        public WordValue SourceValue { get; set; }
        public WordValue DestinationValue { get; set; }
        public WordValue CountValue { get; set; }

        public FMOVModel()
        {
            SourceValue = WordValue.Null;
            DestinationValue = WordValue.Null;
            CountValue = WordValue.Null;
        }

        public FMOVModel(WordValue _SourceValue, WordValue _DestinationValue, WordValue _CountValue)
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

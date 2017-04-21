using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class SMOVModel : BaseModel
    {
        public WordValue SoruceValue { get; set; }
        public WordValue SourceStart { get; set; }
        public WordValue SourceCount { get; set; }
        public WordValue DestinationValue { get; set; }
        public WordValue DestinationStart { get; set; }

        public SMOVModel()
        {
            SoruceValue = WordValue.Null;
            SourceStart = WordValue.Null;
            SourceCount = WordValue.Null;
            DestinationValue = WordValue.Null;
            DestinationStart = WordValue.Null;
        }

        public SMOVModel(WordValue _SoruceValue, WordValue _SourceStart, WordValue _SourceCount, WordValue _DestinationValue, WordValue _DestinationStart)
        {
            SoruceValue = _SoruceValue;
            SourceStart = _SourceStart;
            SourceCount = _SourceCount;
            DestinationValue = _DestinationValue;
            DestinationStart = _DestinationStart;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

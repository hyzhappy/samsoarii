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
        public override string InstructionName => "SMOV";

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

        public override int ParaCount
        {
            get
            {
                return 5;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return SoruceValue;
                case 1: return SourceStart;
                case 2: return SourceCount;
                case 3: return DestinationValue;
                case 4: return DestinationStart;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'SMOV'", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: SoruceValue = (WordValue)value; break;
                case 1: SourceStart = (WordValue)value; break;
                case 2: SourceCount = (WordValue)value; break;
                case 3: DestinationValue = (WordValue)value; break;
                case 4: DestinationStart = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'SMOV'", id));
            }
        }
    }
}

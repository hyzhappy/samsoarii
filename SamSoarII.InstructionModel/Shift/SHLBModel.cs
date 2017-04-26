using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCElementModel.PLCElementModel.Shift
{
    public class SHLBModel : BaseModel
    {
        public BitValue SourceValue { get; set; }
        public BitValue DestinationValue { get; set; }
        public WordValue SourceLength { get; set; }
        public WordValue DestinationLength { get; set; }
        public SHLBModel()
        {
            SourceValue = BitValue.Null;
            DestinationValue = BitValue.Null;
            SourceLength = WordValue.Null;
            DestinationLength = WordValue.Null;
        }
        public SHLBModel(BitValue SourceValue, BitValue DestinationValue, WordValue SourceLength, WordValue DestinationLength)
        {
            this.SourceValue = SourceValue;
            this.DestinationValue = DestinationValue;
            this.SourceLength = SourceLength;
            this.DestinationLength = DestinationLength;
        }
        public override string GenerateCode()
        {
            return string.Empty;
        }
    }
}

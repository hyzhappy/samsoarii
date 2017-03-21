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
            return string.Format("if({0})\r\n{{\r\nif({3} >= 16)\r\n{{\r\n{2} = ({1} >> 16) & 0x0000;\r\nMBit[8166] = {1} & 0x8000;\r\n}}\r\nelse if({3} > 0)\r\n{{\r\n{2} = ({1} >> {3}) & (int16_t)(pow(2,16 - {3}) - 1);\r\nMBit[8166] = ({1} << (16 - {3})) & 0x8000;\r\n}}\r\nelse if({3} == 0)\r\n{{\r\n{2} = {1};\r\n}}\r\nif({2} == 0)\r\n{{\r\nMBit[8167] = 1;\r\n}}\r\n}}\r\n", ImportVaribleName, SourceValue.GetValue(), DestinationValue.GetValue(), Count.GetValue());
        }
    }
}

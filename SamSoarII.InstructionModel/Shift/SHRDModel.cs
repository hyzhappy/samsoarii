using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class SHRDModel : BaseModel
    {
        public DoubleWordValue SourceValue { get; set; }
        public DoubleWordValue DestinationValue { get; set; }
        public WordValue Count { get; set; }
        public SHRDModel()
        {
            SourceValue = DoubleWordValue.Null;
            DestinationValue = DoubleWordValue.Null;
            Count = WordValue.Null;
        }
        public SHRDModel(DoubleWordValue sourceValue, DoubleWordValue destValue, WordValue count)
        {
            SourceValue = sourceValue;
            DestinationValue = destValue;
            Count = count;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nif({3} >= 32)\r\n{{\r\n{2} = ({1} >> 32) & 0x00000000;\r\nMBit[8166] = {1} & 0x80000000;\r\n}}\r\nelse if({3} > 0)\r\n{{\r\n{2} = ({1} >> {3}) & (int32_t)(pow(2,32 - {3}) - 1);\r\nMBit[8166] = ({1} << (32 - {3})) & 0x8000;\r\n}}\r\nelse if({3} == 0)\r\n{{\r\n{2} = {1};\r\n}}\r\nif({2} == 0)\r\n{{\r\nMBit[8167] = 1;\r\n}}\r\n}}\r\n", ImportVaribleName, SourceValue.GetValue(), DestinationValue.GetValue(), Count.GetValue());
        }
    }
}

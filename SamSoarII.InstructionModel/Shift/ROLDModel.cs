using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class ROLDModel : BaseModel
    {
        public DoubleWordValue SourceValue { get; set; }
        public DoubleWordValue DestinationValue { get; set; }
        public WordValue Count { get; set; }
        public ROLDModel()
        {
            SourceValue = DoubleWordValue.Null;
            DestinationValue = DoubleWordValue.Null;
            Count = WordValue.Null;
        }
        public ROLDModel(DoubleWordValue sourceValue, DoubleWordValue destValue, WordValue count)
        {
            SourceValue = sourceValue;
            DestinationValue = destValue;
            Count = count;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nint16_t count = {3} % 32;\r\nif(count == 0)\r\n{{\r\n{2} = {1};\r\n}}\r\nelse if(count > 0)\r\n{{\r\nint32_t temp1,temp2;\r\ntemp1 = ({1} << count) >> count;\r\ntemp2 = (({1} - temp1) >> (32 - count)) & (int32_t)(pow(2,count) - 1);\r\n{2} = (temp1 << count) + temp2;\r\n}}\r\nif({2} == 0)\r\n{{\r\nMBit[8167] = 1;\r\n}}\r\n}}\r\n", ImportVaribleName, SourceValue.GetDoubleWordValue(), DestinationValue.GetDoubleWordValue(), Count.GetWordValue());
        }
    }
}

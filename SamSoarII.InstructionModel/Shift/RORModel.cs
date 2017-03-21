using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class RORModel : BaseModel
    {
        public WordValue SourceValue { get; set; }
        public WordValue DestinationValue { get; set; }
        public WordValue Count { get; set; }
        public RORModel()
        {
            SourceValue = WordValue.Null;
            DestinationValue = WordValue.Null;
            Count = WordValue.Null;
        }
        public RORModel(WordValue sourceValue, WordValue destValue, WordValue count)
        {
            SourceValue = sourceValue;
            DestinationValue = destValue;
            Count = count;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nint16_t count = {3} % 16;\r\nif(count == 0)\r\n{{\r\n{2} = {1};\r\n}}\r\nelse if(count > 0)\r\n{{\r\nint16_t temp1,temp2;\r\ntemp1 = ({1} >> count) << count;\r\ntemp2 = {1} - temp1;\r\ntemp1 = (temp1 >> count) & (int16_t)(pow(2,16 - count) - 1);\r\n{2} = temp1 + (temp2 << (16 - count));\r\n}}\r\nif({2} == 0)\r\n{{\r\nMBit[8167] = 1;\r\n}}\r\n}}\r\n",ImportVaribleName,SourceValue.GetWordValue(),DestinationValue.GetWordValue(),Count.GetWordValue());
        }
    }
}

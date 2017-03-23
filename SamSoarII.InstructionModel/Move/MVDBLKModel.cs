using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class MVDBLKModel : BaseModel
    {
        public DoubleWordValue SourceValue { get; set; }
        public DoubleWordValue DestinationValue { get; set; }
        public WordValue Count { get; set; }
        public MVDBLKModel()
        {
            SourceValue = DoubleWordValue.Null;
            DestinationValue = DoubleWordValue.Null;
            Count = WordValue.Null;
        }

        public MVDBLKModel(DoubleWordValue sourceValue, DoubleWordValue destValue, WordValue count)
        {
            SourceValue = sourceValue;
            DestinationValue = destValue;
            Count = count;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nif({3} <= 512 && {3} >= 1)\r\n{{\r\nint32_t* psrc = &{1},pdes = &{2};\r\nint32_t temp[{3}];\r\nfor(int i = 0;i < {3};i++)\r\n{{\r\ntemp[i] = *psrc;\r\npsrc++;\r\n}}\r\nfor(int i = 0;i < {3};i++)\r\n{{\r\n*pdes = temp[i];\r\npdes++;\r\n}}\r\n}}\r\n}}\r\n", ImportVaribleName, SourceValue.GetValue(), DestinationValue.GetValue(), Count.GetValue());
        }
    }
}

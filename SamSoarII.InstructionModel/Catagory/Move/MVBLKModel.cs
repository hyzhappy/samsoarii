using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class MVBLKModel : BaseModel
    {
        public WordValue SourceValue { get; set; }
        public WordValue DestinationValue { get; set; }
        public WordValue Count { get; set; }
        public MVBLKModel()
        {
            SourceValue = WordValue.Null;
            DestinationValue = WordValue.Null;
            Count = WordValue.Null;
        }
        public MVBLKModel(WordValue sourceValue, WordValue destValue, WordValue count)
        {
            SourceValue = sourceValue;
            DestinationValue = destValue;
            Count = count;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nif({3} <= 1024 && {3} >= 1)\r\n{{\r\nint16_t* psrc = &{1},pdes = &{2};\r\nint16_t temp[{3}];\r\nfor(int i = 0;i < {3};i++)\r\n{{\r\ntemp[i] = *psrc;\r\npsrc++;\r\n}}\r\nfor(int i = 0;i < {3};i++)\r\n{{\r\n*pdes = temp[i];\r\npdes++;\r\n}}\r\n}}\r\n}}\r\n", ImportVaribleName,SourceValue.GetValue(),DestinationValue.GetValue(),Count.GetValue());
        }
        public override int ParaCount
        {
            get
            {
                return 3;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return SourceValue;
                case 1: return DestinationValue;
                case 2: return Count;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: SourceValue = (WordValue)value; break;
                case 1: DestinationValue = (WordValue)value; break;
                case 2: Count = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

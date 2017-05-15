using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class MOVModel : BaseModel
    {
        public WordValue SourceValue { get; set; }
        public WordValue DestinationValue { get; set; }
        public MOVModel()
        {
            SourceValue = WordValue.Null;
            DestinationValue = WordValue.Null;
            TotalVaribleCount = 0;
        }
        public MOVModel(WordValue s, WordValue d)
        {
            SourceValue = s;
            DestinationValue = d;
            TotalVaribleCount = 0;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n {1} = {2}; \r\n}}\r\n", ImportVaribleName, SourceValue.GetValue(), DestinationValue.GetValue());
        }
        public override int ParaCount
        {
            get
            {
                return 2;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return SourceValue;
                case 1: return DestinationValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: SourceValue = (WordValue)value; break;
                case 1: DestinationValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

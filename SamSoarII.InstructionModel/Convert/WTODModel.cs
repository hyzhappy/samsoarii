using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class WTODModel : BaseModel
    {
        public WordValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }

        public WTODModel()
        {
            InputValue = WordValue.Null;
            OutputValue = DoubleWordValue.Null;
            TotalVaribleCount = 0;
        }

        public WTODModel(WordValue input, DoubleWordValue output)
        {
            InputValue = input;
            OutputValue = output;
            TotalVaribleCount = 0;
        }

        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{1}=(int32_t){2};\r\n}}\r\n", ImportVaribleName, OutputValue.GetValue(), InputValue.GetValue());
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
                case 0: return InputValue;
                case 1: return OutputValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: InputValue = (WordValue)value; break;
                case 1: OutputValue = (DoubleWordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class DTOWModel : BaseModel
    {
        public DoubleWordValue InputValue { get; set; }
        public WordValue OutputValue { get; set; }

        public DTOWModel()
        {
            InputValue = DoubleWordValue.Null;
            OutputValue = WordValue.Null;
            TotalVaribleCount = 0;
        }

        public DTOWModel(DoubleWordValue input, WordValue output)
        {
            InputValue = input;
            OutputValue = output;
            TotalVaribleCount = 0;
        }

        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{1}=(int16_t){2};\r\n}}\r\n", ImportVaribleName, InputValue.GetValue(), OutputValue.GetValue());
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
                case 0: InputValue = (DoubleWordValue)value; break;
                case 1: OutputValue = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

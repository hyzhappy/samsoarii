using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class TRUNCModel : BaseModel
    {
        public FloatValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public TRUNCModel()
        {
            InputValue = FloatValue.Null;
            OutputValue = DoubleWordValue.Null;
        }
        public TRUNCModel(FloatValue inputValue, DoubleWordValue outputValue)
        {
            InputValue = inputValue;
            OutputValue = outputValue;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{2} = (int32_t){1};\r\n}}\r\n",ImportVaribleName,InputValue.GetValue(),OutputValue.GetValue());
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
                case 0: InputValue = (FloatValue)value; break;
                case 1: OutputValue = (DoubleWordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

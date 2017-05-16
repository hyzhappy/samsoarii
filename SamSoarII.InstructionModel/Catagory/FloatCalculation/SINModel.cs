using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class SINModel : BaseModel
    {
        public override string InstructionName => "SIN";
        public FloatValue InputValue { get; set; }
        public FloatValue OutputValue { get; set; }
        public SINModel(FloatValue inputValue, FloatValue outputValue)
        {
            InputValue = inputValue;
            OutputValue = outputValue;
        }
        public SINModel()
        {
            InputValue = FloatValue.Null;
            OutputValue = FloatValue.Null;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n{2} = sin({1}*1.745329E-2);\r\nif({2} == 0)\r\n{{\r\nMBit[8171] = 1;\r\n}}\r\nelse if({2} < 0)\r\n{{\r\nMBit[8170] = 1;\r\n}}\r\n}}\r\n", ImportVaribleName,InputValue.GetValue(),OutputValue.GetValue());
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
                case 1: OutputValue = (FloatValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

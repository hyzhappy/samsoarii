using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class DIVFModel : BaseModel
    {
        public override string InstructionName => "DIVF";
        public FloatValue InputValue1 { get; set; }
        public FloatValue InputValue2 { get; set; }
        public FloatValue OutputValue { get; set; }
        public DIVFModel()
        {
            InputValue1 = FloatValue.Null;
            InputValue2 = FloatValue.Null;
            OutputValue = FloatValue.Null;
        }

        public DIVFModel(FloatValue inputValue1, FloatValue inputValue2, FloatValue outputValue)
        {
            InputValue1 = inputValue1;
            InputValue2 = inputValue2;
            OutputValue = outputValue;
        }

        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nif({2} == 0)\r\n{{\r\nMBit[8172] = 1;\r\n}}\r\nelse\r\n{{\r\n{3} = {1} / {2};\r\nif({3} == 0)\r\n{{\r\nMBit[8171] = 1;\r\n}}\r\nelse if({3} < 0)\r\n{{\r\nMBit[8170] = 1;\r\n}}\r\n}}\r\n}}\r\n", ImportVaribleName, InputValue1.GetValue(), InputValue2.GetValue(), OutputValue.GetValue());
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
                case 0: return InputValue1;
                case 1: return InputValue2;
                case 2: return OutputValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: InputValue1 = (FloatValue)value; break;
                case 1: InputValue2 = (FloatValue)value; break;
                case 2: OutputValue = (FloatValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class DIVModel : BaseModel
    {
        public override string InstructionName => "DIV";
        public WordValue InputValue1 { get; set; }
        public WordValue InputValue2 { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public DIVModel()
        {
            InputValue1 = WordValue.Null;
            InputValue2 = WordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }

        public DIVModel(WordValue inputValue1, WordValue inputValue2, DoubleWordValue outputValue)
        {
            InputValue1 = inputValue1;
            InputValue2 = inputValue2;
            OutputValue = outputValue;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nif({2} == 0)\r\n{{\r\nMBit[8172] = 1;\r\n}}\r\nelse\r\n{{\r\nint32_t temp = {1} / {2};\r\nif(temp != (int16_t)temp)\r\n{{\r\nMBit[8169] = 1;\r\n}}\r\nelse\r\n{{\r\n{3} = {1} / {2} + (({1} % {2}) << 16);\r\nif(temp < 0)\r\n{{\r\nMBit[8170] = 1;\r\n}}\r\nelse if(temp == 0)\r\n{{\r\nMBit[8171] = 1;\r\n}}\r\n}}\r\n}}\r\n}}\r\n", ImportVaribleName,InputValue1.GetValue(),InputValue2.GetValue(),OutputValue.GetValue());
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
                case 0: InputValue1 = (WordValue)value; break;
                case 1: InputValue2 = (WordValue)value; break;
                case 2: OutputValue = (DoubleWordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

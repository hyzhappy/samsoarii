using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class INCDModel : BaseModel
    {
        public override string InstructionName => "INCD";
        public DoubleWordValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }
        public INCDModel()
        {
            InputValue = DoubleWordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }
        public INCDModel(DoubleWordValue inputValue, DoubleWordValue outputValue)
        {
            InputValue = inputValue;
            OutputValue = outputValue;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\nint64_t temp = {1} + 1;\r\nif(temp != (int32_t)temp)\r\n{{\r\nMBit[8169] = 1;\r\n}}\r\nelse\r\n{{\r\n{2} = {1} + 1;\r\nif({2} < 0)\r\n{{\r\nMBit[8170] = 1;\r\n}}\r\nelse if({2} == 0)\r\n{{r\nMBit[8171] = 1;\r\n}}\r\n}}\r\n}}\r\n", ImportVaribleName, InputValue.GetValue(), OutputValue.GetValue());
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
                case 1: OutputValue = (DoubleWordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

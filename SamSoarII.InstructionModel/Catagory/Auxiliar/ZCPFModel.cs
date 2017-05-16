using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class ZCPFModel : BaseModel
    {
        public override string InstructionName => "ZCPF";

        public FloatValue InputValue1 { get; set; }
        public FloatValue InputValue2 { get; set; }
        public FloatValue InputValue3 { get; set; }
        public BitValue OutputValue { get; set; }

        public ZCPFModel()
        {
            InputValue1 = FloatValue.Null;
            InputValue2 = FloatValue.Null;
            InputValue3 = FloatValue.Null;
            OutputValue = BitValue.Null;
        }

        public ZCPFModel(FloatValue _InputValue1, FloatValue _InputValue2, FloatValue _InputValue3, BitValue _OutputValue)
        {
            InputValue1 = _InputValue1;
            InputValue2 = _InputValue2;
            InputValue3 = _InputValue3;
            OutputValue = _OutputValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }

        public override int ParaCount
        {
            get
            {
                return 4;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return InputValue1;
                case 1: return InputValue2;
                case 2: return InputValue3;
                case 3: return OutputValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CML'", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: InputValue1 = (FloatValue)value; break;
                case 1: InputValue2 = (FloatValue)value; break;
                case 2: InputValue3 = (FloatValue)value; break;
                case 3: OutputValue = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CML'", id));
            }
        }
    }
}

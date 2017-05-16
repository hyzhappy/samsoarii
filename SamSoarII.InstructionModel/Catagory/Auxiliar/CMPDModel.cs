using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class CMPDModel : BaseModel
    {
        public override string InstructionName => "CMPD";

        public DoubleWordValue InputValue1 { get; set; }
        public DoubleWordValue InputValue2 { get; set; }
        public BitValue OutputValue { get; set; }

        public CMPDModel()
        {
            InputValue1 = DoubleWordValue.Null;
            InputValue2 = DoubleWordValue.Null;
            OutputValue = BitValue.Null;
        }

        public CMPDModel(DoubleWordValue _InputValue1, DoubleWordValue _InputValue2, BitValue _OutputValue)
        {
            InputValue1 = _InputValue1;
            InputValue2 = _InputValue2;
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
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CMPD'", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: InputValue1 = (DoubleWordValue)value; break;
                case 1: InputValue2 = (DoubleWordValue)value; break;
                case 2: OutputValue = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CMPD'", id));
            }
        }
    }
}

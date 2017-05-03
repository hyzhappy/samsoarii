using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class ZCPDModel : BaseModel
    {
        public DoubleWordValue InputValue1 { get; set; }
        public DoubleWordValue InputValue2 { get; set; }
        public DoubleWordValue InputValue3 { get; set; }
        public BitValue OutputValue { get; set; }

        public ZCPDModel()
        {
            InputValue1 = DoubleWordValue.Null;
            InputValue2 = DoubleWordValue.Null;
            InputValue3 = DoubleWordValue.Null;
            OutputValue = BitValue.Null;
        }

        public ZCPDModel(DoubleWordValue _InputValue1, DoubleWordValue _InputValue2, DoubleWordValue _InputValue3, BitValue _OutputValue)
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
                case 0: InputValue1 = (DoubleWordValue)value; break;
                case 1: InputValue2 = (DoubleWordValue)value; break;
                case 2: InputValue3 = (DoubleWordValue)value; break;
                case 3: OutputValue = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CML'", id));
            }
        }
    }
}

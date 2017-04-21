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
    }
}

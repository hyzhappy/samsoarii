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
    }
}

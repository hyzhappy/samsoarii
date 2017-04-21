using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class ZCPModel : BaseModel
    {
        public WordValue InputValue1 { get; set; }
        public WordValue InputValue2 { get; set; }
        public WordValue InputValue3 { get; set; }
        public BitValue OutputValue { get; set; }

        public ZCPModel()
        {
            InputValue1 = WordValue.Null;
            InputValue2 = WordValue.Null;
            InputValue3 = WordValue.Null;
            OutputValue = BitValue.Null;
        }

        public ZCPModel(WordValue _InputValue1, WordValue _InputValue2, WordValue _InputValue3, BitValue _OutputValue)
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

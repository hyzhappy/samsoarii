using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class PTOModel : BaseModel
    {
        public WordValue ArgumentValue;
        public BitValue OutputValue1;
        public BitValue OutputValue2;


        public PTOModel()
        {
            ArgumentValue = WordValue.Null;
            OutputValue1 = BitValue.Null;
            OutputValue2 = BitValue.Null;
        }

        public PTOModel(WordValue _ArgumentValue, BitValue _OutputValue1, BitValue _OutputValue2)
        {
            ArgumentValue = _ArgumentValue;
            OutputValue1 = _OutputValue1;
            OutputValue2 = _OutputValue2;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }

    }
}

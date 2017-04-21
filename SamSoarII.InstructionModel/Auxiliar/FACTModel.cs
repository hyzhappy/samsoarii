using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class FACTModel : BaseModel
    {
        public WordValue InputValue { get; set; }
        public DoubleWordValue OutputValue { get; set; }

        public FACTModel()
        {
            InputValue = WordValue.Null;
            OutputValue = DoubleWordValue.Null;
        }

        public FACTModel(WordValue _InputValue, DoubleWordValue _OutputValue)
        {
            InputValue = _InputValue;
            OutputValue = _OutputValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

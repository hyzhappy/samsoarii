using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class CMLModel : BaseModel
    {
        public WordValue InputValue { get; set; }
        public WordValue OutputValue { get; set; }

        public CMLModel()
        {
            InputValue = WordValue.Null;
            OutputValue = WordValue.Null;
        }

        public CMLModel(WordValue _InputValue, WordValue _OutputValue)
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

using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class XCHModel : BaseModel
    {
        public WordValue LeftValue { get; set; }
        public WordValue RightValue { get; set; }

        public XCHModel()
        {
            LeftValue = WordValue.Null;
            RightValue = WordValue.Null;
        }

        public XCHModel(WordValue _LeftValue, WordValue _RightValue)
        {
            LeftValue = _LeftValue;
            RightValue = _RightValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class XCHFModel : BaseModel
    {
        public FloatValue LeftValue { get; set; }
        public FloatValue RightValue { get; set; }

        public XCHFModel()
        {
            LeftValue = FloatValue.Null;
            RightValue = FloatValue.Null;
        }

        public XCHFModel(FloatValue _LeftValue, FloatValue _RightValue)
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

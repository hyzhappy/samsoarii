using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class XCHDModel : BaseModel
    {
        public DoubleWordValue LeftValue { get; set; }
        public DoubleWordValue RightValue { get; set; }

        public XCHDModel()
        {
            LeftValue = DoubleWordValue.Null;
            RightValue = DoubleWordValue.Null;
        }

        public XCHDModel(DoubleWordValue _LeftValue, DoubleWordValue _RightValue)
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

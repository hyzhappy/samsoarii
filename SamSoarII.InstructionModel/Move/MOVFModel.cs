using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class MOVFModel : BaseModel
    {
        public FloatValue SourceValue { get; set; }
        public FloatValue DestinationValue { get; set; }
        public MOVFModel()
        {
            SourceValue = FloatValue.Null;
            DestinationValue = FloatValue.Null;
            TotalVaribleCount = 0;
        }
        public MOVFModel(FloatValue s, FloatValue d)
        {
            SourceValue = s;
            DestinationValue = d;
            TotalVaribleCount = 0;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n {1} = {2}; \r\n}}\r\n", ImportVaribleName, SourceValue.GetFloatValue(), DestinationValue.GetFloatValue());
        }
    }
}

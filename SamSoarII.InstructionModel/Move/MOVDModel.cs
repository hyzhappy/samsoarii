using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class MOVDModel : BaseModel
    {
        public DoubleWordValue SourceValue { get; set; }
        public DoubleWordValue DestinationValue { get; set; }
        public MOVDModel()
        {
            SourceValue = DoubleWordValue.Null;
            DestinationValue = DoubleWordValue.Null;
            TotalVaribleCount = 0;
        }
        public MOVDModel(DoubleWordValue s, DoubleWordValue d)
        {
            SourceValue = s;
            DestinationValue = d;
            TotalVaribleCount = 0;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n {1} = {2}; \r\n}}\r\n", ImportVaribleName, SourceValue.GetDoubleWordValue(), DestinationValue.GetDoubleWordValue());
        }
    }
}

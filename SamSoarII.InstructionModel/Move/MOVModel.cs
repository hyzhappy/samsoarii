using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class MOVModel : BaseModel
    {
        public WordValue SourceValue { get; set; }
        public WordValue DestinationValue { get; set; }
        public MOVModel()
        {
            SourceValue = WordValue.Null;
            DestinationValue = WordValue.Null;
            TotalVaribleCount = 0;
        }
        public MOVModel(WordValue s, WordValue d)
        {
            SourceValue = s;
            DestinationValue = d;
            TotalVaribleCount = 0;
        }
        public override string GenerateCode()
        {
            return string.Format("if({0})\r\n{{\r\n {1} = {2}; \r\n}}\r\n", ImportVaribleName, SourceValue.GetWordValue(), DestinationValue.GetWordValue());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.InstructionModel
{
    public class LDWLEModel : BaseModel
    {
        public WordValue Value1 { get; set; }
        public WordValue Value2 { get; set; }
        public LDWLEModel()
        {
            Value1 = WordValue.Null;
            Value2 = WordValue.Null;
        }
        public LDWLEModel(WordValue v1, WordValue v2)
        {
            Value1 = v1;
            Value2 = v2;
        }
        public override string GenerateCode()
        {
            return string.Format("uint32_t {0} = {1} <= {2};\r\n", ExportVaribleName, Value1.GetWordValue(), Value2.GetWordValue());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class LDWLModel : BaseModel
    {
        public WordValue Value1 { get; set; }
        public WordValue Value2 { get; set; }
        public LDWLModel()
        {
            Value1 = WordValue.Null;
            Value2 = WordValue.Null;
        }
        public LDWLModel(WordValue v1, WordValue v2)
        {
            Value1 = v1;
            Value2 = v2;
        }
        public override string GenerateCode()
        {
            return string.Format("sr_bool {0} = {1} < {2};\r\n", ExportVaribleName, Value1.GetWordValue(), Value2.GetWordValue());
        }
    }
}

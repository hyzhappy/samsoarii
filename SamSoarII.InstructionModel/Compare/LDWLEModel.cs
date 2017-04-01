using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
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
            return string.Format("sr_bool {0} = {1} <= {2};\r\n", ExportVaribleName, Value1.GetValue(), Value2.GetValue());
        }
    }
}

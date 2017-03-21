using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class LDFGEModel : BaseModel
    {
        public FloatValue Value1 { get; set; }
        public FloatValue Value2 { get; set; }
        public LDFGEModel()
        {
            Value1 = FloatValue.Null;
            Value2 = FloatValue.Null;
        }
        public LDFGEModel(FloatValue v1, FloatValue v2)
        {
            Value1 = v1;
            Value2 = v2;
        }
        public override string GenerateCode()
        {
            return string.Format("sr_bool {0} = {1} >= {2};\r\n", ExportVaribleName, Value1.GetFloatValue(), Value2.GetFloatValue());
        }
    }
}
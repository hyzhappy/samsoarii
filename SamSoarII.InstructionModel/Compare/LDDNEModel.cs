using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class LDDNEModel : BaseModel
    {
        public DoubleWordValue Value1 { get; set; }
        public DoubleWordValue Value2 { get; set; }
        public LDDNEModel()
        {
            Value1 = DoubleWordValue.Null;
            Value2 = DoubleWordValue.Null;
        }
        public LDDNEModel(DoubleWordValue value1, DoubleWordValue value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
        public override string GenerateCode()
        {
            return string.Format("sr_bool {0} = {1} != {2};\r\n", ExportVaribleName, Value1.GetDoubleWordValue(), Value2.GetDoubleWordValue());
        }
    }
}

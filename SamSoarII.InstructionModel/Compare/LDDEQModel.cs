using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class LDDEQModel : BaseModel
    {
        public DoubleWordValue Value1 { get; set; }
        public DoubleWordValue Value2 { get; set; }
        public LDDEQModel()
        {
            Value1 = DoubleWordValue.Null;
            Value2 = DoubleWordValue.Null;
        }
        public LDDEQModel(DoubleWordValue v1, DoubleWordValue v2)
        {
            Value1 = v1;
            Value2 = v2;
        }
        public override string GenerateCode()
        {
            return string.Format("sr_bool {0} = {1} == {2};\r\n", ExportVaribleName, Value1.GetValue(), Value2.GetValue());
        }
    }
}
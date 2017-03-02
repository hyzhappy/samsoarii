using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class LDModel : BaseModel
    {
        public BitValue Value { get; set; }
        public LDModel()
        {
            Value = BitValue.Null;
        }

        public LDModel(BitValue value)
        {
            Value = value;   
        }

        public override string GenerateCode()
        {
            return string.Format("plc_bool {0} = {1};\r\n", ExportVaribleName, Value.GetBitValue());
        }
    }
}

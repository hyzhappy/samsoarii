using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class LDIModel : BaseModel
    {
        public BitValue Value { get; set; }

        public LDIModel()
        {
            Value = BitValue.Null;
        }

        public LDIModel(BitValue value)
        {
            Value = value;
        }

        public override string GenerateCode()
        {
            return string.Format("plc_bool {0} = !{1};\r\n", ExportVaribleName, Value.GetValue());
        }
        
        public override int ParaCount
        {
            get
            {
                return 1;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return Value;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: Value = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

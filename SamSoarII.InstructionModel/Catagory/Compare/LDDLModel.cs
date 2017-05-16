using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class LDDLModel : BaseModel
    {
        public DoubleWordValue Value1 { get; set; }
        public DoubleWordValue Value2 { get; set; }
        public LDDLModel()
        {
            Value1 = DoubleWordValue.Null;
            Value2 = DoubleWordValue.Null;
        }
        public LDDLModel(DoubleWordValue value1, DoubleWordValue value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
        public override string GenerateCode()
        {
            return string.Format("sr_bool {0} = {1} < {2};\r\n", ExportVaribleName, Value1.GetValue(), Value2.GetValue());
        }
        public override int ParaCount
        {
            get
            {
                return 2;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return Value1;
                case 1: return Value2;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: Value1 = (DoubleWordValue)value; break;
                case 1: Value2 = (DoubleWordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

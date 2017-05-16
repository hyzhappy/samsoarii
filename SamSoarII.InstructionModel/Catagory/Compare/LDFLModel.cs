using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class LDFLModel : BaseModel
    {
        public override string InstructionName => "LDFL";
        public FloatValue Value1 { get; set; }
        public FloatValue Value2 { get; set; }
        public LDFLModel()
        {
            Value1 = FloatValue.Null;
            Value2 = FloatValue.Null;
        }
        public LDFLModel(FloatValue v1, FloatValue v2)
        {
            Value1 = v1;
            Value2 = v2;
        }
        public override string GenerateCode()
        {
            return string.Format("sr_bool {0} = {1} == {2};\r\n", ExportVaribleName, Value1.GetValue(), Value2.GetValue());
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
                case 0: Value1 = (FloatValue)value; break;
                case 1: Value2 = (FloatValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}
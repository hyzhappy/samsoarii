using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class LDWNEModel : BaseModel
    {
        public override string InstructionName => "LDWNE";
        public WordValue Value1 { get; set; }
        public WordValue Value2 { get; set; }
        public LDWNEModel()
        {
            Value1 = WordValue.Null;
            Value2 = WordValue.Null; 
        }
        public LDWNEModel(WordValue v1, WordValue v2)
        {
            Value1 = v1;
            Value2 = v2;
        }
        public override string GenerateCode()
        {
            return string.Format("sr_bool {0} = {1} != {2};\r\n", ExportVaribleName, Value1.GetValue(), Value2.GetValue());
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
                case 0: Value1 = (WordValue)value; break;
                case 1: Value2 = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

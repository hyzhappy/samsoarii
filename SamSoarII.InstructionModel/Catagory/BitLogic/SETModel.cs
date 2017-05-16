using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class SETModel : BaseModel
    {
        public override string InstructionName => "SET";

        public BitValue Value { get; set; }
        public WordValue Count { get; set; }

        public SETModel()
        {
            Value = BitValue.Null;
            Count = WordValue.Null;
        }
        public SETModel(BitValue value, WordValue count)
        {
            Value = value;
            Count = count;
        }

        public override string GenerateCode()
        {
            return string.Empty;
            //return string.Format("if({0})\r\n{{\r\n for(int i = 0; i < {1}; i++)\r\n{{\r\n{2}=1;\r\n}}\r\n}}\r\n", ImportVaribleName, Count.GetValue(), Value.);
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
                case 0: return Value;
                case 1: return Count;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: Value = (BitValue)value; break;
                case 1: Count = (WordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

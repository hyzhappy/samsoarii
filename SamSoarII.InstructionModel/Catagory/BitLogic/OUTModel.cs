using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.ValueModel;
namespace SamSoarII.LadderInstModel
{
    public class OUTModel : BaseModel
    {
        public BitValue Value { get; set; }
        public OUTModel()
        {
            Value = BitValue.Null;
        }
        public OUTModel(BitValue value)
        {
            Value = value;
        }
        public override string GenerateCode()
        {
            return string.Format("{0} = {1};\r\n", Value.GetValue(), ImportVaribleName);
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

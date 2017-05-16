using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class XCHFModel : BaseModel
    {
        public FloatValue LeftValue { get; set; }
        public FloatValue RightValue { get; set; }

        public XCHFModel()
        {
            LeftValue = FloatValue.Null;
            RightValue = FloatValue.Null;
        }

        public XCHFModel(FloatValue _LeftValue, FloatValue _RightValue)
        {
            LeftValue = _LeftValue;
            RightValue = _RightValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
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
                case 0: return LeftValue;
                case 1: return RightValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CML'", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: LeftValue = (FloatValue)value; break;
                case 1: RightValue = (FloatValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CML'", id));
            }
        }
    }
}

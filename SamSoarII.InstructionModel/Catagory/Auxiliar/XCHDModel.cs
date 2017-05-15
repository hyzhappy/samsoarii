using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Auxiliar
{
    public class XCHDModel : BaseModel
    {
        public DoubleWordValue LeftValue { get; set; }
        public DoubleWordValue RightValue { get; set; }

        public XCHDModel()
        {
            LeftValue = DoubleWordValue.Null;
            RightValue = DoubleWordValue.Null;
        }

        public XCHDModel(DoubleWordValue _LeftValue, DoubleWordValue _RightValue)
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
                case 0: LeftValue = (DoubleWordValue)value; break;
                case 1: RightValue = (DoubleWordValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters of instruction 'CML'", id));
            }
        }
    }
}

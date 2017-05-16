using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class PLSSTOPModel : BaseModel
    {
        public override string InstructionName => "PLSSTOP";
        public BitValue OutputValue { get; set; }

        public PLSSTOPModel()
        {
            OutputValue = BitValue.Null;
        }

        public PLSSTOPModel(BitValue _OutputValue)
        {
            OutputValue = _OutputValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
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
                case 0: return OutputValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: OutputValue = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }

}
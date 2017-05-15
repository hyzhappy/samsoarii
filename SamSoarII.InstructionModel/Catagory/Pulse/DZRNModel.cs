using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class DZRNModel : BaseModel
    {
        public DoubleWordValue BackValue { get; set; }
        public DoubleWordValue CrawValue { get; set; }
        public BitValue SignalValue { get; set; }
        public BitValue OutputValue { get; set; }

        public DZRNModel()
        {
            BackValue = DoubleWordValue.Null;
            CrawValue = DoubleWordValue.Null;
            SignalValue = BitValue.Null;
            OutputValue = BitValue.Null;
        }

        public DZRNModel(DoubleWordValue _BackValue, DoubleWordValue _CrawValue, BitValue _SignalValue, BitValue _OutputValue)
        {
            BackValue = _BackValue;
            CrawValue = _CrawValue;
            SignalValue = _SignalValue;
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
                return 4;
            }
        }

        public override IValueModel GetPara(int id)
        {
            switch (id)
            {
                case 0: return BackValue;
                case 1: return CrawValue;
                case 2: return SignalValue;
                case 3: return OutputValue;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }

        public override void SetPara(int id, IValueModel value)
        {
            switch (id)
            {
                case 0: BackValue = (DoubleWordValue)value; break;
                case 1: CrawValue = (DoubleWordValue)value; break;
                case 2: SignalValue = (BitValue)value; break;
                case 3: OutputValue = (BitValue)value; break;
                default: throw new ArgumentOutOfRangeException(String.Format("Index {0:d} out of range for parameters", id));
            }
        }
    }
}

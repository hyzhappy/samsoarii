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
    }
}

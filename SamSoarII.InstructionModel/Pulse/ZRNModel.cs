using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Pulse
{
    public class ZRNModel : BaseModel
    {
        public WordValue BackValue { get; set; }
        public WordValue CrawValue { get; set; }
        public BitValue SignalValue { get; set; }
        public BitValue OutputValue { get; set; }

        public ZRNModel()
        {
            BackValue = WordValue.Null;
            CrawValue = WordValue.Null;
            SignalValue = BitValue.Null;
            OutputValue = BitValue.Null;
        }

        public ZRNModel(WordValue _BackValue, WordValue _CrawValue, BitValue _SignalValue, BitValue _OutputValue)
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

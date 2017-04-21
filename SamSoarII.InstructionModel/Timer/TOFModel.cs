using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class TOFModel : BaseModel
    {
        public WordValue TimerValue { get; set; }
        public WordValue EndValue { get; set; }

        public TOFModel()
        {
            TimerValue = WordValue.Null;
            EndValue = WordValue.Null;
        }

        public TOFModel(WordValue _TimerValue, WordValue _EndValue)
        {
            TimerValue = _TimerValue;
            EndValue = _EndValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

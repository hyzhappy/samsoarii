using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Interrupt
{
    public class DTCHModel : BaseModel
    {
        public WordValue IDValue { get; set; }

        public DTCHModel()
        {
            IDValue = WordValue.Null;
        }

        public DTCHModel(WordValue _IDValue)
        {
            IDValue = _IDValue;
        }

        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

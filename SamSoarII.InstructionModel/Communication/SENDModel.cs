using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel.Communication
{
    public class SENDModel : BaseModel
    {
        public WordValue COMPort { get; set; }
        public WordValue BaseValue { get; set; }
        public WordValue CountValue { get; set; }

        public SENDModel()
        {
            COMPort = WordValue.Null;
            BaseValue = WordValue.Null;
            CountValue = WordValue.Null;
        }

        public SENDModel(WordValue _COMPort, WordValue _Base, WordValue _Count)
        {
            COMPort = _COMPort;
            BaseValue = _Base;
            CountValue = _Count;
        }
        
        public override string GenerateCode()
        {
            return String.Empty;
        }
    }
}

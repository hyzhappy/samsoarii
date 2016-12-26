using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.InstructionModel
{
    public class LBLModel : BaseModel
    {
        public WordValue LBLIndex { get; set; } 
        public LBLModel()
        {
            LBLIndex = WordValue.Null;
        }
        public LBLModel(WordValue lblindex)
        {
            LBLIndex = lblindex;
        }

        public override string GenerateCode()
        {
            return string.Format("{0}: \r\n");
        }
    }
}

using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.LadderInstModel
{
    public class TRDModel : BaseModel
    {
        public WordValue StartValue { get; set; }
        public TRDModel()
        {
            StartValue = WordValue.Null;
        }

        public TRDModel(WordValue startValue)
        {
            StartValue = startValue;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

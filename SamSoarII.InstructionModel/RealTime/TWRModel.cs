using SamSoarII.ValueModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.InstructionModel
{
    public class TWRModel : BaseModel
    {
        public WordValue StartValue { get; set; }
        public TWRModel()
        {
            StartValue = WordValue.Null;
        }

        public TWRModel(WordValue startValue)
        {
            StartValue = startValue;
        }
        public override string GenerateCode()
        {
            throw new NotImplementedException();
        }
    }
}

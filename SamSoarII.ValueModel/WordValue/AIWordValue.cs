using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class AIWordValue : WordValue
    { 
        public AIWordValue(uint index, WordValue offset)
        {
            Index = index;
            Offset = offset == null ? WordValue.Null : offset;
        }

        public override string ValueShowString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string ValueString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string GetValue()
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class AOWordValue : WordValue
    {
        public AOWordValue(uint index, VWordValue offset = null)
        {
            Index = index;
            Offset = offset;
        }

        public override string GetWordValue()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("AO{0}{1}", Index, Offset);     
        }
    }
}

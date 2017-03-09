using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class TVWordValue : WordValue
    {
        public TVWordValue(uint index, VWordValue offset = null)
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
            return string.Format("TV{0}{1}", Index, Offset);
        }
    }
}

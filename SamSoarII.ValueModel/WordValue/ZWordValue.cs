using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class ZWordValue : WordValue
    {
        public ZWordValue(uint index)
        {
            Index = index;
        }

        public override string ValueShowString
        {
            get
            {
                return ValueString;
            }
        }

        public override string ValueString
        {
            get
            {
                return string.Format("Z{0}", Index);
            }
        }

        public override string GetValue()
        {
            throw new NotImplementedException();
        }

    }

}

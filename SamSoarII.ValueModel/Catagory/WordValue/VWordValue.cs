using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class VWordValue : WordValue
    {
        public VWordValue(uint index)
        {
            Base = string.Format("V");
            Index = index;
            Offset = Null;
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
                return string.Format("{0}{1}",Base ,Index);
            }
        }
        public override string GetValue()
        {
            throw new NotImplementedException();
        }
    }
}

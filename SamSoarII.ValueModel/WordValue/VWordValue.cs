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
            Index = index;
        }

        public override string GetWordValue()
        {
            //return string.Format("*(int16_t*)(0x{0})", Convert.ToString(AddressManager.VBaseAddress + Index, 16));
            return string.Empty;
        }
        public override string ToString()
        {
            return string.Format("V{0}", Index); ;
        }
    }
}

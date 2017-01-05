using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class CVWordValue : WordValue
    {
        public CVWordValue(uint index, VWordValue offset = null)
        {
            Index = index;
            Offset = offset;
            
        }

        public override string GetWordValue()
        {
            //if (Offset == null)
            //{
            //    return string.Format("*((int16_t*)0x{0})", Convert.ToString(AddressManager.CVBaseAddress + Index, 16));
            //}
            //else
            //{
            //    return string.Format("*((int16_t*)0x{0} + {1})", Convert.ToString(AddressManager.CVBaseAddress + Index, 16), Offset.GetWordValue());
            //}
            return string.Empty;
        }

        public override string ToString()
        {
            if (Offset == null)
            {
                return string.Format("CV{0}", Index);
            }
            else
            {
                return string.Format("CV{0}{1}", Index, Offset.ToString());
            }
        }
    }
}

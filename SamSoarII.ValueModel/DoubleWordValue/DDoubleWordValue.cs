using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class DDoubleWordValue : DoubleWordValue
    {
        public DDoubleWordValue(uint index, VWordValue offset)
        {
            Index = index;
            Offset = offset;
        }

        public override string GetDoubleWordValue()
        {
            //if (Offset == null)
            //{
            //    return string.Format("*((int32_t*)0x{0})", Convert.ToString(AddressManager.DBaseAddress + Index, 16));
            //}
            //else
            //{
            //    return string.Format("*((in32_t*)((int16_t*)0x{0} + {1}))", Convert.ToString(AddressManager.DBaseAddress + Index, 16), Offset.GetWordValue());
            //}
            return string.Empty;
        }

        public override string ToString()
        {
            if (Offset == null)
            {
                return string.Format("D{0}D{1}", Index, Index + 1);
            }
            else
            {
                return string.Format("D{0}D{1}{2}", Index, Index + 1, Offset.ToString());
            }
        }
    }
}

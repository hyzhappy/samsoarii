using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class CV32DoubleWordValue : DoubleWordValue
    {
        public CV32DoubleWordValue(uint index, VWordValue offset = null)
        {
            Index = index;
            Offset = offset;
        }
        public override string GetDoubleWordValue()
        {
            //if (Offset == null)
            //{
            //    return string.Format("*((int32_t*)0x{0})", Convert.ToString(AddressManager.CV32BaseAddress + Index, 16));
            //}
            //else
            //{
            //    return string.Format("*((in32_t*)0x{0} + {1})", Convert.ToString(AddressManager.CV32BaseAddress + Index, 16), Offset.GetWordValue());
            //}
            return string.Empty;
        }

        public override string ToString()
        {
            if (Offset == null)
            {
                return string.Format("CV{0}D{1}", Index, Index + 1);
            }
            else
            {
                return string.Format("CV{0}D{1}{2}", Index, Index + 1, Offset.ToString());
            }
        }
    }
}

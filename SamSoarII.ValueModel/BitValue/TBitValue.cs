using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class TBitValue : BitValue
    {
        public TBitValue(uint index, VWordValue offset = null)
        {
            Index = index;
            Offset = offset;
        }
        public override string GetBitValue()
        {
            //if (Offset == null)
            //{
            //    return string.Format("*((uint32_t*)0x{0})", Convert.ToString(AddressManager.TBaseAddress + Index * 4, 16));
            //}
            //else
            //{
            //    return string.Format("*((uint32_t*)0x{0} + {1})", Convert.ToString(AddressManager.TBaseAddress + Index * 4, 16), Offset.GetWordValue());
            //}
            return string.Format("TBit[{0}{1}]", Index, string.Empty);
        }

        public override string GetInputImBitAddress()
        {
            throw new NotImplementedException();
        }

        public override string GetOutputImBitAddress()
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            if (Offset == null)
            {
                return string.Format("T{0}", Index);
            }
            else
            {
                return string.Format("T{0}{1}", Index, Offset.ToString());
            }
        }
    }
}

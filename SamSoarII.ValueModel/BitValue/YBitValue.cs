using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class YBitValue : BitValue
    {
        public YBitValue(uint index, VWordValue offset = null)
        {
            Index = index;
            Offset = offset;
        }

        public override string GetBitValue()
        {
            //if(Offset == null)
            //{
            //    return string.Format("*((uint32_t*)0x{0})", Convert.ToString(AddressManager.YBaseAddress + Index * 4, 16));
            //}
            //else
            //{
            //    return string.Format("*((uint32_t*)0x{0} + {1})", Convert.ToString(AddressManager.YBaseAddress + Index * 4, 16), Offset.GetWordValue());
            //}
            return string.Format("YBit[{0}{1}]", Index, string.Empty);
        }

        public override string GetInputImBitAddress()
        {
            throw new InputImException();
        }

        public override string GetOutputImBitAddress()
        {
            throw new OutputImException();
        }
        public override string ToString()
        {
            if (Offset == null)
            {
                return string.Format("Y{0}", Index);
            }
            else
            {
                return string.Format("Y{0}{1}", Index, Offset.ToString());
            }
        }
    }
}

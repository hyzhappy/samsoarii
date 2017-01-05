using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class XBitValue : BitValue
    {
        public XBitValue(uint index, VWordValue offset = null)
        {
            Index = index;
            Offset = offset;
        }

        public override string GetBitValue()
        {
            //if(Offset == null)
            //{
            //    return string.Format("*((uint32_t*)0x{0})", Convert.ToString(AddressManager.XBaseAddress + Index * 4, 16));
            //}
            //else
            //{
            //    return string.Format("*((uint32_t*)0x{0} + {1})", Convert.ToString(AddressManager.XBaseAddress + Index * 4, 16), Offset.GetWordValue());
            //}
            return string.Empty;
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
                return string.Format("X{0}", Index);
            }
            else
            {
                return string.Format("X{0}{1}", Index, Offset.ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Utility
{
    public struct IntRange
    {
        public uint Start { get; set; }
        public uint End { get; set; }

        public IntRange(uint start, uint end)
        {
            Start = start;
            End = end;
        }

        public bool AssertValue(uint input)
        {
            return (input < End) & (input >= Start);
        }
    }
}

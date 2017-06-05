using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Extend.Utility
{
    public class BreakPointManager
    {
        static private int count = 0;

        static public void Initialize()
        {
            count = 0;
        }

        static public int Register()
        {
            return count++;
        }
    }
}

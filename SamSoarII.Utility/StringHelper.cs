using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.Utility
{
    public class StringHelper
    {
        public static int Compare(string str,int index)
        {
            int temp = 0;
            foreach (var item in str.Substring(0,index + 1))
            {
                temp += item;
            }
            return temp;
        }
        public static int Compare(string str1,string str2)
        {
            char[] chars1 = str1.ToCharArray();
            char[] chars2 = str2.ToCharArray();
            for (int i = 0; i < Math.Min(chars1.Length,chars2.Length); i++)
            {
                if (chars1[i] != chars2[i])
                {
                    return chars1[i] - chars2[i];
                }
            }
            return chars1.Length - chars2.Length;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SamSoarII.Utility
{
    public class StringHelper
    {
        public static readonly int MaxCapacity = 28;
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
        public static string Trunc(string value)
        {
            int count = value.Count();
            count += ChineseCharCount(value);
            if (count > MaxCapacity)
            {
                int TruncNum = count - MaxCapacity + 3;
                List<string> segments = value.Split(new char[] { Path.DirectorySeparatorChar },StringSplitOptions.RemoveEmptyEntries).ToList();
                string first = segments.First();
                string last = segments.Last();
                if (segments.Count == 2)
                {
                    if (last.Length > TruncNum)
                    {
                        last = "..." + last.Substring(TruncNum);
                    }
                    else
                    {
                        first = first.Substring(0, 3) + "..." + first.Substring(3 + TruncNum);
                    }
                    value = string.Format("{0}{1}{2}", first, Path.DirectorySeparatorChar, last);
                }
                else
                {
                    if (first.Length + last.Length + 5 > MaxCapacity)
                    {
                        TruncNum = first.Length + last.Length + 5 - MaxCapacity + 3;
                        if (last.Length > TruncNum)
                        {
                            last = "..." + last.Substring(TruncNum);
                        }
                        else
                        {
                            first = first.Substring(0, 3) + "..." + first.Substring(3 + TruncNum);
                        }
                    }
                    else
                    {
                        int index = segments.Count - 2;
                        while (first.Length + last.Length + 5 + segments.ElementAt(index).Length + 1 <= MaxCapacity)
                        {
                            if (index == 0)
                            {
                                break;
                            }
                            last = segments.ElementAt(index) + Path.DirectorySeparatorChar.ToString() + last;
                            index--;
                        }
                    }
                    value = string.Format("{0}{1}{3}{1}{2}", first, Path.DirectorySeparatorChar, last, "...");
                }
            }
            return value;
        }
        private static int ChineseCharCount(string value)
        {
            int cnt = 0;
            foreach (var c in value)
            {
                if (c >= 0x4e00 && c <= 0x9fbb)
                {
                    cnt++;
                }
            }
            return cnt;
        }
        public static Color ColorParse(string value)
        {
            Regex regex = new Regex("^#([0-9A-F]{2})([0-9A-F]{2})([0-9A-F]{2})([0-9A-F]{2})",RegexOptions.Compiled);
            Match match = regex.Match(value);
            if (match.Success)
            {
                Color color = new Color();
                color.A = byte.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                color.R = byte.Parse(match.Groups[2].Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                color.G = byte.Parse(match.Groups[3].Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                color.B = byte.Parse(match.Groups[4].Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                return color;
            }
            else
            {
                return Colors.Black;
            }
        }
    }
}

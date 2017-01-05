using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace SamSoarII.ValueModel
{
    public class ValueParser
    {
        
        /// <summary>
        /// Match (X|Y|M|C|T|S)[0-9]+(V[0-9]+)?
        /// </summary>
        /// <param name="valueString"></param>
        /// <returns></returns>
        public static BitValue ParseBitValue(string valueString)
        {
            if(valueString == string.Empty)
            {
                return BitValue.Null;
            }     
            Match match = Regex.Match(valueString, "^(X|Y|M|C|T|S)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
            if(match.Success)
            {
                VWordValue offset = null;
                string s = match.Value.ToUpper();
                var ss = s.Split('V');
                if(ss.Length == 2)
                {
                    offset = new VWordValue(uint.Parse(ss[1]));
                }
                uint index = uint.Parse(ss[0].Substring(1));
                switch(ss[0][0])
                {
                    case 'X':
                        return new XBitValue(index, offset);
                    case 'Y':
                        return new YBitValue(index, offset);
                    case 'M':
                        return new MBitValue(index, offset);
                    case 'C':
                        return new CBitValue(index, offset);
                    case 'T':
                        return new TBitValue(index, offset);
                    case 'S':
                        return new SBitValue(index, offset);
                    default:
                        throw new ValueParseException();
                }
            }
            else
            {
                throw new ValueParseException();
            }
        }
        /// <summary>
        /// Match (D|CV|TV|AI|AO)[0-9]+(V[0-9]+)?
        ///       (K[0-9]+)|(H[0-9A-F]+)
        ///       (V[0-9]+)
        /// </summary>
        /// <param name="valueString"></param>
        /// <returns></returns>
        public static WordValue ParseWordValue(string valueString)
        {
            if(valueString == string.Empty)
            {
                return WordValue.Null;
            }
            else
            {               
                Match match1 = Regex.Match(valueString, "^(D|CV|TV|AI|AO)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
                if(match1.Success)
                {
                    VWordValue offset = null;
                    string s = match1.Value.ToUpper();   
                    if (s.StartsWith("D"))
                    {
                        var ss = s.Split('V');
                        if (ss.Length == 2)
                        {
                            offset = new VWordValue(uint.Parse(ss[1]));
                        }
                        uint index = uint.Parse(ss[0].Substring(1));
                        return new DWordValue(index, offset);
                    }
                    if(s.StartsWith("CV"))
                    {
                        var ss = s.Split('V');
                        if (ss.Length == 3)
                        {
                            offset = new VWordValue(uint.Parse(ss[2]));
                        }
                        uint index = uint.Parse(ss[1]);
                        return new CVWordValue(index, offset);
                    }
                    if (s.StartsWith("TV"))
                    {
                        var ss = s.Split('V');
                        if (ss.Length == 3)
                        {
                            offset = new VWordValue(uint.Parse(ss[2]));
                        }
                        uint index = uint.Parse(ss[1]);
                        return new TVWordValue(index, offset);
                    }
                    if (s.StartsWith("AI"))
                    {
                        var ss = s.Split('V');
                        if (ss.Length == 2)
                        {
                            offset = new VWordValue(uint.Parse(ss[1]));
                        }
                        uint index = uint.Parse(ss[0].Substring(2));
                        return new AIWordValue(index, offset);
                    }
                    if (s.StartsWith("AO"))
                    {
                        var ss = s.Split('V');
                        if (ss.Length == 2)
                        {
                            offset = new VWordValue(uint.Parse(ss[1]));
                        }
                        uint index = uint.Parse(ss[0].Substring(2));
                        return new AOWordValue(index, offset);
                    }
                    throw new ValueParseException();
                }
                else
                {
                    Match match2 = Regex.Match(valueString, "^V[0-9]+$", RegexOptions.IgnoreCase);
                    if (match2.Success)
                    {
                        string s = match1.Value.ToUpper();
                        uint index = uint.Parse(s.Substring(1));
                        return new VWordValue(index);
                    }
                    else
                    {
                        Match match3 = Regex.Match(valueString, "^K[-+]?[0-9]+$", RegexOptions.IgnoreCase);
                        if(match3.Success)
                        {
                            string s = match3.Value.ToUpper();                        
                            short value = short.Parse(s.Substring(1));
                            return new KWordValue(value);                        
                        }
                        Match match4 = Regex.Match(valueString, "^H[0-9A-F]+$", RegexOptions.IgnoreCase);
                        if(match4.Success)
                        {
                            string s = match4.Value.ToUpper();
                            short value = short.Parse(s.Substring(1), System.Globalization.NumberStyles.HexNumber);
                            return new HWordValue(value);
                        }
                        throw new ValueParseException();
                    }
                }
                throw new ValueParseException();
            }
        }
        /// <summary>
        /// Match (D|CV)[0-9]+(V[0-9]+)?
        ///       (K[0-9]+)|(H[0-9A-F]+)
        /// </summary>
        /// <returns></returns>
        public static DoubleWordValue ParseDoubleWordValue(string valueString)
        {
            if(valueString == string.Empty)
            {
                return DoubleWordValue.Null;
            }
            else
            {
                Match match1 = Regex.Match(valueString, "^(D|CV)[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
                if(match1.Success)
                {
                    VWordValue offset = null;
                    var s = match1.Value.ToUpper();
                    var ss = s.Split('V');
                    if(s.StartsWith("D"))
                    {
                        if(ss.Length == 2)
                        {
                            offset = new VWordValue(uint.Parse(ss[1]));
                        }
                        uint index = uint.Parse(ss[0].Substring(1));
                        return new DDoubleWordValue(index, offset);
                    }
                    if(s.StartsWith("CV"))
                    {
                        if(ss.Length == 3)
                        {
                            offset = new VWordValue(uint.Parse(ss[2]));
                        }
                        uint index = uint.Parse(ss[1]);
                        return new CV32DoubleWordValue(index, offset);          
                    }
                }
                else
                {
                    Match match2 = Regex.Match(valueString, "^K[-+][0-9]+$", RegexOptions.IgnoreCase);
                    if (match2.Success)
                    {
                        var s = match1.Value.ToUpper();
                        return new KDoubleWordValue(int.Parse(s.Substring(1)));
                    }
                    Match match3 = Regex.Match(valueString, "^H[0-9A-F]+$", RegexOptions.IgnoreCase);  
                    if(match3.Success)
                    {
                        var s = match1.Value.ToUpper();
                        return new HDoubleWordValue(int.Parse(s.Substring(1), System.Globalization.NumberStyles.HexNumber));
                    }    
                }
                throw new ValueParseException();
            }
        }
        /// <summary>
        /// Match D[0-9]+(V[0-9]+)？
        ///       K[0-9]+(.[0-9]+)?
        /// </summary>
        /// <returns></returns>
        public static FloatValue ParseFloatValue(string valueString)
        {
            if(valueString == string.Empty)
            {
                return FloatValue.Null;
            }
            else
            {
                var match1 = Regex.Match(valueString, "^D[0-9]+(V[0-9]+)?$", RegexOptions.IgnoreCase);
                if (match1.Success)
                {
                    VWordValue offset = null;
                    var s = match1.Value.ToUpper();
                    var ss = s.Split('V');
                    if (ss.Length == 2)
                    {
                        offset = new VWordValue(uint.Parse(ss[1]));
                    }
                    uint index = uint.Parse(ss[0].Substring(1));
                    return new DFloatValue(index, offset);
                }
                else
                {
                    var match2 = Regex.Match(valueString, "^K[+-]?([0-9]*[.])?[0-9]+$", RegexOptions.IgnoreCase);
                    if(match2.Success)
                    {
                        var s = match2.Value.ToUpper();
                        float value = float.Parse(s.Substring(1));
                        return new KFloatValue(value);
                    }
                }
                throw new ValueParseException();
            }
        }
    }
}

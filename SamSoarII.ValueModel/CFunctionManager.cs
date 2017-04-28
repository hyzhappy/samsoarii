using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamSoarII.ValueModel
{
    public class CFunctionValueModel
    {
        public static Regex VerifyBitRegex { get; } = new Regex(@"^(Y|M|S)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex VerifyWordRegex { get; } = new Regex(@"^(D|CV|TV|AO)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex VerifyDWordRegex { get; } = new Regex(@"^(D|CV)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex VerifyFloatRegex { get; } = new Regex(@"^(D)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string Type { get; set; }
        public string Name { get; set; }
        public Regex Format { get; set; }

        public bool Success(string name)
        {
            if (!Format.Match(name).Success)
            {
                return false;
            }
            switch (Type)
            {
                case "BIT":
                    if (ValueParser.ParseBitValue(name) == null)
                        return false;
                    break;
                case "WORD":
                    if (ValueParser.ParseWordValue(name) == null)
                        return false;
                    break;
                case "DWORD":
                    if (ValueParser.ParseDoubleWordValue(name) == null)
                        return false;
                    break;
                case "FLOAT":
                    if (ValueParser.ParseFloatValue(name) == null)
                        return false;
                    break;
            }
            return true;
        }
    }

    public class CFunction
    {
        public string Name { get; set; }
        public CFunctionValueModel Arg1 { get; set; }
        public CFunctionValueModel Arg2 { get; set; }
        public CFunctionValueModel Arg3 { get; set; }
        public CFunctionValueModel Arg4 { get; set; }
    }

    public class CFunctionManager
    {
        static private Dictionary<string, CFunction> funcdict = new Dictionary<string, CFunction>();

        public IEnumerable<CFunction> Functions
        {
            get { return funcdict.Values; }
        }

        public static void Clear()
        {
            funcdict.Clear();
        }

        public static void Add(string[] msgs)
        {
            string name = msgs[0];
            CFunction func = null;
            if (funcdict.ContainsKey(name))
            {
                func = funcdict[name];
            }
            else
            {
                func = new CFunction();
                func.Name = name;
                funcdict.Add(name, func);
            }
            if (msgs.Length < 3) return;
            func.Arg1 = Decode(msgs[1], msgs[2]);
            if (msgs.Length < 5) return;
            func.Arg1 = Decode(msgs[3], msgs[4]);
            if (msgs.Length < 7) return;
            func.Arg1 = Decode(msgs[5], msgs[6]);
            if (msgs.Length < 9) return;
            func.Arg1 = Decode(msgs[7], msgs[8]);
        }

        public static CFunction Get(string name)
        {
            if (funcdict.ContainsKey(name))
            {
                return funcdict[name];
            }
            else
            {
                return null;
            }
        }
        
        public static CFunctionValueModel Decode(string type, string name)
        {
            CFunctionValueModel cfvmodel = new CFunctionValueModel();
            cfvmodel.Name = name;
            cfvmodel.Type = type;
            switch (type)
            {
                case "BIT":
                    cfvmodel.Format = CFunctionValueModel.VerifyBitRegex;
                    break;
                case "WORD":
                    cfvmodel.Format = CFunctionValueModel.VerifyWordRegex;
                    break;
                case "DWORD":
                    cfvmodel.Format = CFunctionValueModel.VerifyDWordRegex;
                    break;
                case "FLOAT":
                    cfvmodel.Format = CFunctionValueModel.VerifyFloatRegex;
                    break;
            }
            return cfvmodel;
        }
        
    }
}
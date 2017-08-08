using SamSoarII.PLCDevice;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class ValueModel : IModel
    {
        #region Resources

        public enum Types { BOOL, WORD, UWORD, DWORD, UDWORD, BCD, FLOAT, HEX, DHEX, STRING, NULL };
        public enum Bases { X, Y, S, M, C, T, D, CV, TV, AI, AO, V, Z, K, H, NULL };

        public readonly static string[] NameOfTypes = { "BOOL", "WORD", "UWORD", "DWORD", "UDWORD", "BCD", "FLOAT", "HEX", "DHEX", "STRING", "NULL" };
        public readonly static string[] NameOfBases = { "X", "Y", "S", "M", "C", "T", "D", "CV", "TV", "AI", "AO", "V", "Z", "K", "H", "NULL" };
        public readonly static Dictionary<string, Types> TypeOfNames = new Dictionary<string, Types>();
        public readonly static Dictionary<string, Bases> BaseOfNames = new Dictionary<string, Bases>();

        public readonly static ValueRegex VarRegex = new ValueRegex(
            @"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "X", "Y", "M", "C", "T", "S", "D", "V", "Z", "CV", "TV", "AI", "AO" });
        public readonly static ValueRegex BitRegex = new ValueRegex(
            @"^(X|Y|M|C|T|S)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "X", "Y", "M", "C", "T", "S" });
        public readonly static ValueRegex WordRegex = new ValueRegex(
            @"^(D|CV|TV|AI|AO)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "D", "CV", "TV", "AI", "AO" });
        public readonly static ValueRegex DoubleWordRegex = new ValueRegex(
            @"^(D|CV)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "D", "CV" });
        public readonly static ValueRegex FloatRegex = new ValueRegex(
            @"^(D)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "D" });
        public readonly static ValueRegex VarWordRegex = new ValueRegex(
            @"^(V|Z)([0-9]+)$",
            new string[] { "V", "Z" });
        public readonly static ValueRegex IntKValueRegex = new ValueRegex(
            @"^(K)([-+]?[0-9]+)$",
            new string[] { "K" });
        public readonly static ValueRegex IntHValueRegex = new ValueRegex(
            @"^(H)([0-9A-F]+)$",
            new string[] { "H" });
        public readonly static ValueRegex FloatKValueRegex = new ValueRegex(
            @"^(K)([-+]?([0-9]*[.])?[0-9]+)$",
            new string[] { "K" });
        public readonly static ValueRegex VariableRegex = new ValueRegex(
            @"^{([0-9a-zA-Z]+)}$",
            new string[] { });

        public readonly static ValueRegex VerifyBitRegex1 = new ValueRegex(
            @"^(X|Y|M|C|T|S)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "X", "Y", "M", "C", "T", "S" });
        public readonly static ValueRegex VerifyBitRegex2 = new ValueRegex(
            @"^(Y|M|C|T|S)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "Y", "M", "C", "T", "S" });
        public readonly static ValueRegex VerifyBitRegex3 = new ValueRegex(
            @"^(Y|M|S)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "Y", "M", "S" });
        public readonly static ValueRegex VerifyBitRegex4 = new ValueRegex(
            @"^(Y)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] {"Y"});
        public readonly static ValueRegex VerifyBitRegex5 = new ValueRegex(
            @"^(X|M)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "X", "M" });
        public readonly static ValueRegex VerifyBitRegex6 = new ValueRegex(
            @"^(S)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "S" });
        public readonly static ValueRegex VerifyBitRegex7 = new ValueRegex(
            @"^(Y|M)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "Y", "M" });
        public readonly static ValueRegex VerifyBitRegex8 = new ValueRegex(
            @"^(X)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "X" });


        public readonly static ValueRegex WordBitRegex = new ValueRegex(
            @"^(D|V|Z)([0-9]+)\.([0-9A-F])((V|Z)([0-9]+))?$", 
            new string[] { "Dm.n" });

        public readonly static ValueRegex VerifyWordRegex1 = new ValueRegex(
            @"^(D|CV|TV|AI|AO)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "D", "CV", "TV", "AI", "AO" });
        public readonly static ValueRegex VerifyWordRegex2 = new ValueRegex(
            @"^(D|CV|TV|AO)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "D", "CV", "TV", "AO" });
        public readonly static ValueRegex VerifyWordRegex3 = new ValueRegex(
            @"^(D)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "D" });
        public readonly static ValueRegex VerifyWordRegex4 = new ValueRegex(
            @"^(TV)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "TV" });
        public readonly static ValueRegex VerifyWordRegex5 = new ValueRegex(
            @"^(D|AI)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "AI" });
        public readonly static ValueRegex VerifyWordRegex6 = new ValueRegex(
            @"^(D|AO)([0-9]+)((V|Z)([0-9]+))?$",
            new string[] { "AO" });

        public readonly static ValueRegex BitWordRegex = new ValueRegex(
            @"(K)([0-9]+)(X|Y|M|S)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "KnMm" });
        
        public readonly static ValueRegex VerifyDoubleWordRegex1 = new ValueRegex(
            @"^(D|CV)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "D", "CV" });
        public readonly static ValueRegex VerifyDoubleWordRegex2 = new ValueRegex(
            @"^(D)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "D" });
        public readonly static ValueRegex VerifyDoubleWordRegex3 = new ValueRegex(
            @"^(CV)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "CV" });
        public readonly static ValueRegex BitDoubleWordRegex = new ValueRegex(
            @"(K)([0-9]+)(X|Y|M|S)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "KnMm" });

        public readonly static ValueRegex VerifyFloatRegex = new ValueRegex(
            @"^(D)([0-9]+)((V|Z)([0-9]+))?$", 
            new string[] { "D" });

        public readonly static ValueRegex VerifyIntKValueRegex = new ValueRegex(
            @"^(K)([-+]?[0-9]+)$", 
            new string[] { "K" });
        public readonly static ValueRegex VerifyIntHValueRegex = new ValueRegex(
            @"^(H)([0-9A-F]+)$", 
            new string[] { "H" });
        //public readonly static Regex VerifyIntKHValueRegex = new Regex(@"^(H)([0-9A-F]+)|(K)([-+]?[0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static ValueRegex VerifyFloatKValueRegex = new ValueRegex(
            @"^(K)([-+]?([0-9]*[.])?[0-9]+(e[-+]?[0-9]+)?)$", 
            new string[] { "K" });

        public readonly static ValueRegex FuncNameRegex = new ValueRegex(
            @"^([a-zA-Z_]\w*)$", 
            new string[] { });
        public readonly static ValueRegex AnyNameRegex = new ValueRegex(
            @"^[\w\t]*$", 
            new string[] { });

        public readonly static ValueModel Analyzer_Bit = new ValueModel(null, new ValueFormat("ANA", Types.BOOL, false, false, 0,
                new Regex[] { ValueModel.BitRegex, ValueModel.WordBitRegex }));
        public readonly static ValueModel Analyzer_Word = new ValueModel(null, new ValueFormat("ANA", Types.WORD, false, false, 0,
                new Regex[] { ValueModel.WordRegex, ValueModel.BitWordRegex, ValueModel.IntKValueRegex, ValueModel.IntHValueRegex }));
        public readonly static ValueModel Analyzer_DWord = new ValueModel(null, new ValueFormat("ANA", Types.DWORD, false, false, 0,
                new Regex[] { ValueModel.DoubleWordRegex, ValueModel.BitDoubleWordRegex, ValueModel.IntKValueRegex, ValueModel.IntHValueRegex}));
        public readonly static ValueModel Analyzer_Float = new ValueModel(null, new ValueFormat("ANA", Types.FLOAT, false, false, 0,
                new Regex[] { ValueModel.FloatRegex, ValueModel.FloatKValueRegex }));
        public readonly static ValueModel Analyzer_String = new ValueModel(null, new ValueFormat("ANA", Types.STRING, false, false, 0,
                new Regex[] { ValueModel.AnyNameRegex}));

        static ValueModel()
        {
            for (int i = 0; i < NameOfTypes.Length; i++)
                TypeOfNames.Add(NameOfTypes[i], (Types)i);
            for (int i = 0; i < NameOfBases.Length; i++)
                BaseOfNames.Add(NameOfBases[i], (Bases)i);
        }

        #endregion

        public ValueModel(LadderUnitModel _parent, ValueFormat _format)
        {
            format = _format;
            text = "???";
            bas = Bases.NULL;
            ofs = 0;
            ibs = Bases.NULL;
            ifs = 0;
            Parent = _parent;
        }

        public void Dispose()
        {
            Parent = null;
            format = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        #region Number

        private LadderUnitModel parent;
        public LadderUnitModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (ValueManager != null) ValueManager.Remove(this);
                this.parent = value;
                if (ValueManager != null) ValueManager.Add(this);
            }
        }
        IModel IModel.Parent { get { return Parent; } }
        public ValueManager ValueManager
        {
            get { return parent?.Parent?.Parent?.Parent?.Parent.MNGValue; }
        }

        private ValueFormat format;
        public ValueFormat Format { get { return this.format; } }
        public Types Type
        {
            get { return format != null ? format.Type : Types.NULL; }
            set { if (format != null) format = new ValueFormat(format.Name, value, format.CanRead, format.CanWrite, format.Position, format.Regexs); }
        }
        
        protected string text;
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                if (Type != Types.STRING)
                    value = value.ToUpper();
                Parse(value);
                this.text = value;
            }
        }

        public string Comment { get { return ValueManager != null ? ValueManager[this].Comment : ""; } }

        protected Bases bas;
        public Bases Base { get { return this.bas; } }

        protected int ofs;
        public int Offset { get { return this.ofs; } }

        protected int siz;
        public int Size { get { return this.siz; } }
        
        protected Bases ibs;
        public Bases Intra { get { return this.ibs; } }

        protected int ifs;
        public int IntraOffset { get { return this.ifs; } }
        
        public bool IsWordBit
        {
            get { return Type == Types.BOOL 
                    && (Base == Bases.D || Base == Bases.V || Base == Bases.Z); }
        }

        public bool IsBitWord
        {
            get { return (Type == Types.WORD || Type == Types.UWORD || Type == Types.BCD || Type == Types.HEX) 
                    && (Base == Bases.X || Base == Bases.Y || Base == Bases.M || Base == Bases.S); }
        }
        
        public bool IsBitDoubleWord
        {
            get { return (Type == Types.DWORD || Type == Types.UDWORD || Type == Types.DHEX) 
                    && (Base == Bases.X || Base == Bases.Y || Base == Bases.M || Base == Bases.S); }
        }

        protected ValueStore store;
        public ValueStore Store
        {
            get
            {
                return this.store;
            }
            set
            {
                if (store != null)
                    store.PropertyChanged -= OnStorePropertyChanged;
                this.store = value;
                if (store != null)
                    store.PropertyChanged += OnStorePropertyChanged;
            }
        }
        public object Value { get { return store?.Value != null ? store.Value : 0; } }
        public event PropertyChangedEventHandler StorePropertyChanged = delegate { };
        private void OnStorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            StorePropertyChanged(this, e);
        }

        #endregion

        #region Shell

        IViewModel IModel.View { get { return null; } set { } }

        private ProjectTreeViewItem ptvitem;
        public ProjectTreeViewItem PTVItem
        {
            get { return this.ptvitem; }
            set { this.ptvitem = value; }
        }

        #endregion

        #region Parse

        private void Parse(string text)
        {
            if (text.Equals("???"))
            {
                bas = Bases.NULL; ofs = 0;
                ibs = Bases.NULL; ifs = 0;
                return;
            }
            Match match = format.Match(text);
            Device device = PLCDeviceManager.GetPLCDeviceManager().SelectDevice;
            if (match == null)
                throw new ValueParseException("Unexpected input.", format);
            //if (ValueManager != null) ValueManager.Remove(this);
            bas = match.Groups.Count > 1 ? ParseBase(match.Groups[1].Value) : Bases.NULL;
            switch (bas)
            {
                case Bases.K:
                    switch (format.Type)
                    {
                        case Types.WORD:
                        case Types.DWORD:
                            if (match.Groups.Count > 4)
                            {
                                siz = int.Parse(match.Groups[2].Value);
                                if (siz <= 0 || siz > (format.Type == Types.WORD ? 16 : 32))
                                    throw new ValueParseException(Properties.Resources.Message_Over_Max_Len, format);
                                bas = ParseBase(match.Groups[3].Value);
                                ofs = int.Parse(match.Groups[4].Value);
                                if (ofs < 0 || ofs + siz > device.GetRange(bas).Count)
                                    throw new ValueParseException(Properties.Resources.Message_Over_Max_Len, format);
                                ibs = match.Groups.Count > 6 && match.Groups[6].Value.Length > 0 ? ParseBase(match.Groups[6].Value) : Bases.NULL;
                                ifs = match.Groups.Count > 7 && match.Groups[7].Value.Length > 0 ? int.Parse(match.Groups[7].Value) : 0;
                            }
                            else
                            {
                                Store = new ValueStore(null, Type);
                                try
                                {
                                    Store.Value = format.Type == Types.WORD
                                        ? short.Parse(match.Groups[2].Value)
                                        : int.Parse(match.Groups[2].Value);
                                }
                                catch (Exception)
                                {
                                    Store.Value = format.Type == Types.WORD
                                        ? ushort.Parse(match.Groups[2].Value)
                                        : uint.Parse(match.Groups[2].Value);
                                }
                            }
                            break;
                        case Types.FLOAT:
                            Store = new ValueStore(null, Type);
                            Store.Value = float.Parse(match.Groups[2].Value);
                            break;
                        default:
                            Store = new ValueStore(null, Type);
                            Store.Value = int.Parse(match.Groups[2].Value);
                            break;
                    }
                    break;
                case Bases.H:
                    Store = new ValueStore(null, Type);
                    Store.Value = int.Parse(match.Groups[2].Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                    break;
                case Bases.NULL:
                    break;
                default:
                    if (IsWordBit)
                    {
                        ofs = int.Parse(match.Groups[2].Value);
                        if (ofs < 0 || ofs >= device.GetRange(bas).Count)
                            throw new ValueParseException(Properties.Resources.Message_Over_Max_Len, format);
                        ofs <<= 4;
                        ofs += int.Parse(match.Groups[3].Value, System.Globalization.NumberStyles.HexNumber);
                        ibs = match.Groups.Count > 5 && match.Groups[5].Value.Length > 0 ? ParseBase(match.Groups[5].Value) : Bases.NULL;
                        ifs = match.Groups.Count > 6 && match.Groups[6].Value.Length > 0 ? int.Parse(match.Groups[6].Value) : 0;
                        break;
                    }
                    ofs = match.Groups.Count > 2 ? int.Parse(match.Groups[2].Value) : 0;
                    ibs = match.Groups.Count > 4 && match.Groups[4].Value.Length > 0 ? ParseBase(match.Groups[4].Value) : Bases.NULL;
                    ifs = match.Groups.Count > 5 && match.Groups[5].Value.Length > 0 ? int.Parse(match.Groups[5].Value) : 0;
                    if (bas == Bases.CV)
                    {
                        if (Type == Types.WORD && ofs >= 200)
                            throw new ValueParseException(Properties.Resources.Message_Over_Max_Len, format);
                        if (Type == Types.DWORD && ofs < 200)
                            throw new ValueParseException(Properties.Resources.Message_Over_Max_Len, format);
                    }
                    if (ofs < 0 || ofs >= device.GetRange(bas).Count)
                        throw new ValueParseException(Properties.Resources.Message_Over_Max_Len, format);
                    break;
            }
            //if (ValueManager != null) ValueManager.Add(this);
        }

        private Bases ParseBase(string _bas)
        {
            switch (_bas)
            {
                case "X": return Bases.X;
                case "Y": return Bases.Y;
                case "S": return Bases.S; 
                case "M": return Bases.M; 
                case "C": return Bases.C; 
                case "T": return Bases.T; 
                case "D": return Bases.D; 
                case "CV": return Bases.CV; 
                case "TV": return Bases.TV; 
                case "AI": return Bases.AI; 
                case "AO": return Bases.AO; 
                case "V": return Bases.V; 
                case "Z": return Bases.Z;
                case "K": return Bases.K;
                case "H": return Bases.H;
                default: return Bases.NULL; 
            }
        }

        #endregion
        
        #region Save & Load (Not used)

        public void Load(XElement xele)
        {
            throw new NotImplementedException();
        }

        public void Save(XElement xele)
        {
            throw new NotImplementedException();
        }

        public ValueModel Clone()
        {
            ValueModel vmodel = new ValueModel(parent, format);
            vmodel.Parse(Text);
            return vmodel;
        }

        #endregion
    }
    
    public class ValueFormat
    {
        private int position;
        public int Position { get { return this.position; } }

        private string name;
        public string Name { get { return this.name; } }

        private bool canread;
        public bool CanRead { get { return this.canread; } }

        private bool canwrite;
        public bool CanWrite { get { return this.canwrite; } }

        private ValueModel.Types type;
        public ValueModel.Types Type { get { return this.type; } }

        private IEnumerable<Regex> regexs;
        public IEnumerable<Regex> Regexs { get { return this.regexs; } }

        public Match Match(string text)
        {
            foreach (Regex regex in regexs)
            {
                Match match = regex.Match(text);
                if (match.Success)
                {
                    return match;
                }
            }
            return null;
        }

        private string supports;
        public string Supports { get { return this.supports; } }

        private string detail;
        public string Detail { get { return this.detail; } }

        public ValueFormat(string _name, ValueModel.Types _type, bool _canread, bool _canwrite, int _position, IEnumerable<Regex> _regexs, string _detail = null)
        {
            name = _name;
            type = _type;
            canread = _canread;
            canwrite = _canwrite;
            position = _position;
            regexs = _regexs;
            IEnumerable<string> _supports = new string[] { };
            foreach (Regex regex in regexs)
            {
                if (!(regex is ValueRegex)) continue;
                ValueRegex vregex = (ValueRegex)regex;
                _supports = _supports.Union(vregex.Supports);
            }
            supports = String.Join("/", _supports);
            detail = _detail != null ? _detail : String.Format("[{0:s}]{1:s}({2:s})",
                ValueModel.NameOfTypes[(int)type], name, supports);
        }
    }

    public class ValueParseException : Exception
    {
        private ValueFormat format;
        public ValueFormat Format { get { return this.format; } }
        public ValueParseException(string msg, ValueFormat _format) : base(msg)
        {
            format = _format;
        }
        
    }
}

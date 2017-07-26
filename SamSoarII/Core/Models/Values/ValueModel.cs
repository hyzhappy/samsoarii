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

        public enum Types { BOOL, WORD, UWORD, DWORD, UDWORD, BCD, FLOAT, STRING, NULL};
        public enum Bases { X, Y, S, M, C, T, D, CV, TV, AI, AO, V, Z, K, H, NULL};

        public readonly static string[] NameOfTypes = { "BOOL", "WORD", "UWORD", "DWORD", "UDWORD", "FLOAT", "BCD", "STRING", "NULL" };
        public readonly static string[] NameOfBases = { "X", "Y", "S", "M", "C", "T", "D", "CV", "TV", "AI", "AO", "V", "Z", "K", "H", "NULL" }; 
        
        public readonly static Regex VarRegex = new Regex(@"^(X|Y|M|C|T|S|D|V|Z|CV|TV|AI|AO)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex BitRegex = new Regex(@"^(X|Y|M|C|T|S)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex WordRegex = new Regex(@"^(D|CV|TV|AI|AO)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex DoubleWordRegex = new Regex(@"^(D|CV)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex FloatRegex = new Regex(@"^(D)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VarWordRegex = new Regex(@"^(V|Z)([0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex IntKValueRegex = new Regex(@"^(K)([-+]?[0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex IntHValueRegex = new Regex(@"^(H)([0-9A-F]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex FloatKValueRegex = new Regex(@"^(K)([-+]?([0-9]*[.])?[0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VariableRegex = new Regex(@"^{([0-9a-zA-Z]+)}$", RegexOptions.Compiled);

        public readonly static Regex VerifyBitRegex1 = new Regex(@"^(X|Y|M|C|T|S)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VerifyBitRegex2 = new Regex(@"^(Y|M|C|T|S)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VerifyBitRegex3 = new Regex(@"^(Y|M|S)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VerifyBitRegex4 = new Regex(@"^(Y)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VerifyBitRegex5 = new Regex(@"^(X|M)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public readonly static Regex VerifyWordRegex1 = new Regex(@"^(D|CV|TV|AI|AO)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VerifyWordRegex2 = new Regex(@"^(D|CV|TV|AO)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VerifyWordRegex3 = new Regex(@"^(D)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VerifyWordRegex4 = new Regex(@"^(TV)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public readonly static Regex VerifyDoubleWordRegex1 = new Regex(@"^(D|CV)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VerifyDoubleWordRegex2 = new Regex(@"^(D)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VerifyDoubleWordRegex3 = new Regex(@"^(CV)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public readonly static Regex VerifyFloatRegex = new Regex(@"^(D)([0-9]+)((V|Z)([0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public readonly static Regex VerifyIntKValueRegex = new Regex(@"^(K)([-+]?[0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VerifyIntHValueRegex = new Regex(@"^(H)([0-9A-F]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //public readonly static Regex VerifyIntKHValueRegex = new Regex(@"^(H)([0-9A-F]+)|(K)([-+]?[0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public readonly static Regex VerifyFloatKValueRegex = new Regex(@"^(K)([-+]?([0-9]*[.])?[0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public readonly static Regex FuncNameRegex = new Regex(@"^([a-zA-Z_]\w*)$", RegexOptions.Compiled);
        public readonly static Regex AnyNameRegex = new Regex(@"^.*$", RegexOptions.Compiled);

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
        public Types Type { get { return format != null ? format.Type : Types.NULL; } }

        protected string text;
        public string Text
        {
            get { return this.text; }
            set { value = value.ToUpper(); Parse(value); this.text = value; }
        }

        public string Comment { get { return ValueManager != null ? ValueManager[this].Comment : ""; } }

        protected Bases bas;
        public Bases Base { get { return this.bas; } }

        protected int ofs;
        public int Offset { get { return this.ofs; } }
        
        protected Bases ibs;
        public Bases Intra { get { return this.ibs; } }

        protected int ifs;
        public int IntraOffset { get { return this.ifs; } }

        protected ValueStore store;
        public ValueStore Store
        {
            get
            {
                return this.store;
            }
            set
            {
                if (store != null) store.PropertyChanged -= OnStorePropertyChanged;
                this.store = value;
                if (store != null) store.PropertyChanged += OnStorePropertyChanged;
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
            if (match == null)
                throw new ValueParseException("Unexpected input.", format);
            //if (ValueManager != null) ValueManager.Remove(this);
            bas = match.Groups.Count > 1 ? ParseBase(match.Groups[1].Value) : Bases.NULL;
            switch (bas)
            {
                case Bases.K:
                    switch (format.Type)
                    {
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
                default:
                    ofs = match.Groups.Count > 2 ? int.Parse(match.Groups[2].Value) : 0;
                    ibs = match.Groups.Count > 4 && match.Groups[4].Value.Length > 0 ? ParseBase(match.Groups[4].Value) : Bases.NULL;
                    ifs = match.Groups.Count > 5 && match.Groups[5].Value.Length > 0 ? int.Parse(match.Groups[5].Value) : 0;
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

        public ValueFormat(string _name, ValueModel.Types _type, bool _canread, bool _canwrite, int _position, IEnumerable<Regex> _regexs)
        {
            name = _name;
            type = _type;
            canread = _canread;
            canwrite = _canwrite;
            position = _position;
            regexs = _regexs;
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using SamSoarII.Core.Simulate;
using System.Diagnostics;

namespace SamSoarII.Core.Models
{
    public class ItrpModel
    {
        public ItrpModel(ProjectModel _parent)
        {

        }

        public void Dispose()
        {

        }

        #region Number

        private ProjectModel parent;
        public ProjectModel Parent { get { return this.parent; } }

        private ObservableCollection<ItrpElement> children;
        public IList<ItrpElement> Children { get { return this.children; } }

        private bool ismodified;
        public bool IsModified
        {
            get { return this.ismodified; }
            set { this.ismodified = value; }
        }

        public bool IsValid { get { return children.All(e => e.IsValid); } }

        #endregion
 
        public void Compile()
        {
            string cpath = Utility.FileHelper.AppRootPath;
            string hopath = String.Format(@"{0:s}\simug\_simuitrp.o", cpath);
            string hcpath = String.Format(@"{0:s}\simug\simuitrp.h", cpath);
            string ccpath = String.Format(@"{0:s}\simug\simuitrp.c", cpath);
            string dllpath = String.Format(@"{0:s}\simug\simuitrp.dll", cpath);
            SimulateDllModel.Encode(hopath, hcpath);
            StreamWriter sw = new StreamWriter(ccpath);
            sw.Write("#include \"{0:s}\";\n", hcpath);
            sw.Write("int8_t isfirst = 1;\n");
            for (int i = 0; i < children.Count(); i++)
            {
                if (!children[i].IsActive) continue;
                switch (children[i].Oper)
                {
                    case ItrpElement.Operators.UPEDGE:
                    case ItrpElement.Operators.DOWNEDGE:
                    case ItrpElement.Operators.CHANGED:
                        switch (children[i].Type)
                        {
                            case ValueModel.Types.BOOL: sw.Write("int8_t _last{0:d};\n", i); break;
                            case ValueModel.Types.WORD: sw.Write("int16_t _last{0:d};\n", i); break;
                            case ValueModel.Types.DWORD: sw.Write("int32_t _last{0:d};\n", i); break;
                            case ValueModel.Types.FLOAT: sw.Write("float _last{0:d};\n", i); break;
                        }
                        break;
                }
            }
            sw.Write("EXPORT int AssertItrp()\n{\n");
            sw.Write("int ret = 0;\n");
            for (int i = children.Count() - 1; i >= 0; i--)
            {
                if (!children[i].IsActive) continue;
                string lvalue = 
                    children[i].LeftCStyle();
                string rvalue = children[i].RightCStyle();
                switch (children[i].Oper)
                {
                    case ItrpElement.Operators.UPEDGE:
                        sw.Write("if (!isfirst && !_last{0:d} && {1:s}) ret = {0:d} + 1;\n", i, lvalue);
                        sw.Write("_last{0:d} = {1:s};\n", i, lvalue);
                        break;
                    case ItrpElement.Operators.DOWNEDGE:
                        sw.Write("if (!isfirst && _last{0:d} && !{1:s}) ret = {0:d} + 1;\n", i, lvalue);
                        sw.Write("_last{0:d} = {1:s};\n", i, lvalue);
                        break;
                    case ItrpElement.Operators.CHANGED:
                        sw.Write("if (!isfirst && _last{0:d} != {1:s}) ret = {0:d} + 1;\n", i, lvalue);
                        sw.Write("_last{0:d} = {1:s};\n", i, lvalue);
                        break;
                    case ItrpElement.Operators.EQUAL:
                        sw.Write("if ({1:s} == {2:s}) ret = {0:d} + 1;\n", i, lvalue, rvalue);
                        break;
                    case ItrpElement.Operators.NOTEQUAL:
                        sw.Write("if ({1:s} != {2:s}) ret = {0:d} + 1;\n", i, lvalue, rvalue);
                        break;
                    case ItrpElement.Operators.LESS:
                        sw.Write("if ({1:s} < {2:s}) ret = {0:d} + 1;\n", i, lvalue, rvalue);
                        break;
                    case ItrpElement.Operators.NOTLESS:
                        sw.Write("if ({1:s} >= {2:s}) ret = {0:d} + 1;\n", i, lvalue, rvalue);
                        break;
                    case ItrpElement.Operators.MORE:
                        sw.Write("if ({1:s} > {2:s}) ret = {0:d} + 1;\n", i, lvalue, rvalue);
                        break;
                    case ItrpElement.Operators.NOTMORE:
                        sw.Write("if ({1:s} <= {2:s}) ret = {0:d} + 1;\n", i, lvalue, rvalue);
                        break;
                }
            }
            sw.Write("isfirst = 0; return ret;\n}\n");
            sw.Close();
            Process cmd = null;
            cmd = new Process();
            cmd.StartInfo.FileName
                = String.Format(@"{0:s}\Compiler\tcc\tcc", cpath);
            cmd.StartInfo.Arguments
                = String.Format("\"{0:s}\" \"{1:s}\" -o \"{3:s}\" -shared -DBUILD_DLL",
                    hcpath, ccpath, dllpath);
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
#if DEBUG
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
#endif
            cmd.Start();
            cmd.WaitForExit();
            SimulateDllModel.SetItrpDll(dllpath);
#if RELEASE
            File.Delete(chpath);
            File.Delete(ccpath);
#endif
        }
    }

    public class ItrpElement : IDisposable, INotifyPropertyChanged
    {
        public ItrpElement(ItrpModel _parent)
        {
            parent = _parent;
            isactive = false;
            isbit = false;
            oper = Operators.NULL;
        }

        public void Dispose()
        {
            IsActive = false;
            lvalue = null;
            rvalue = null;
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ItrpModel parent;
        public ItrpModel Parent { get { return this.parent; } }

        private bool isactive;
        public bool IsActive
        {
            get { return this.isactive; }
            set { this.isactive = value; PropertyChanged(this, new PropertyChangedEventArgs("IsActive")); }
        }

        private bool isbit;
        public bool IsBit { get { return this.isbit; } }

        private ValueModel lvalue;
        public ValueModel LValue { get { return this.rvalue; } }

        private ValueModel rvalue;
        public ValueModel RValue { get { return this.rvalue; } }

        public ValueModel.Types Type
        {
            get { return lvalue != null ? lvalue.Type : ValueModel.Types.NULL; }
            set { if (lvalue != null) Parse(lvalue.Text, rvalue?.Text, "", value); }
        }

        public enum Operators
        {
            NULL, UPEDGE, DOWNEDGE, CHANGED,
            EQUAL, NOTEQUAL, LESS, MORE, NOTLESS, NOTMORE
        };
        private Operators oper;
        public Operators Oper
        {
            get { return this.oper; }
            set { this.oper = value; PropertyChanged(this, new PropertyChangedEventArgs("Oper")); }
        }

        public bool IsValid
        {
            get
            {
                if (lvalue.Text.Equals("???"))
                    return false;
                switch (oper)
                {
                    case Operators.EQUAL:
                    case Operators.NOTEQUAL:
                    case Operators.LESS:
                    case Operators.NOTLESS:
                    case Operators.MORE:
                    case Operators.NOTMORE:
                        if (rvalue.Text.Equals("???"))
                            return false;
                        break;
                }
                switch (oper)
                {
                    case Operators.UPEDGE:
                    case Operators.DOWNEDGE:
                        if (!isbit) return false;
                        break;
                    case Operators.LESS:
                    case Operators.MORE:
                    case Operators.NOTLESS:
                    case Operators.NOTMORE:
                        if (isbit) return false;
                        break;
                }
                return true;
            }
        }

#endregion

        public void Parse(string _lvalue, string _rvalue = "???", string _oper = null, ValueModel.Types _type = ValueModel.Types.WORD)
        {
            try
            {
                ValueModel.Analyzer_Bit.Text = _lvalue;
                lvalue = ValueModel.Analyzer_Bit.Clone();
                ValueModel.Analyzer_Bit.Text = _rvalue;
                rvalue = ValueModel.Analyzer_Bit.Clone();
            }
            catch (ValueParseException)
            {
                switch (_type)
                {
                    case ValueModel.Types.WORD:
                        ValueModel.Analyzer_Word.Text = _lvalue;
                        lvalue = ValueModel.Analyzer_Word.Clone();
                        ValueModel.Analyzer_Word.Text = _rvalue;
                        rvalue = ValueModel.Analyzer_Word.Clone();
                        break;
                    case ValueModel.Types.DWORD:
                        ValueModel.Analyzer_DWord.Text = _lvalue;
                        lvalue = ValueModel.Analyzer_DWord.Clone();
                        ValueModel.Analyzer_DWord.Text = _rvalue;
                        rvalue = ValueModel.Analyzer_DWord.Clone();
                        break;
                    case ValueModel.Types.FLOAT:
                        ValueModel.Analyzer_Float.Text = _lvalue;
                        lvalue = ValueModel.Analyzer_Float.Clone();
                        ValueModel.Analyzer_Float.Text = _rvalue;
                        rvalue = ValueModel.Analyzer_Float.Clone();
                        break;
                }
            }
            switch (_oper)
            {
                case "": break;
                case "上升沿": oper = Operators.UPEDGE; break;
                case "下降沿": oper = Operators.DOWNEDGE; break;
                case "变化": oper = Operators.CHANGED; break;
                case "＝": oper = Operators.EQUAL; break;
                case "≠": oper = Operators.NOTEQUAL; break;
                case "＜": oper = Operators.LESS; break;
                case "＞": oper = Operators.MORE; break;
                case "≤": oper = Operators.NOTLESS; break;
                case "≥": oper = Operators.NOTMORE; break;
                default: oper = Operators.CHANGED; break;
            }
        }

        private string ToCStyle(ValueModel vmodel)
        {
            ValueFormat vformat = vmodel.Format;
            switch (vmodel.Type)
            {
                case ValueModel.Types.BOOL:
                    switch (vmodel.Base)
                    {
                        case ValueModel.Bases.X:
                        case ValueModel.Bases.Y:
                        case ValueModel.Bases.S:
                        case ValueModel.Bases.M:
                        case ValueModel.Bases.C:
                        case ValueModel.Bases.T:
                            return String.Format("{0:s}Bit[{1:s}]",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel));
                        case ValueModel.Bases.D:
                        case ValueModel.Bases.V:
                        case ValueModel.Bases.Z:
                            return String.Format(
                                vformat.CanWrite
                                    ? "_set_wbit({0:s}Word+({1:s}>>4), {1:s}&15,"
                                    : "_get_wbit({0:s}Word+({1:s}>>4), {1:s}&15)",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel));
                    }
                    break;
                case ValueModel.Types.WORD:
                    switch (vmodel.Base)
                    {
                        case ValueModel.Bases.D:
                        case ValueModel.Bases.AI:
                        case ValueModel.Bases.AO:
                        case ValueModel.Bases.V:
                        case ValueModel.Bases.Z:
                        case ValueModel.Bases.TV:
                            return String.Format("{0:s}Word[{1:s}]",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel));
                        case ValueModel.Bases.CV:
                            return vmodel.Offset < 200
                                ? String.Format("CVWord[{0:s}]", ToCIndex(vmodel))
                                : String.Format("*((int16_t)(&CV32DoubleWord[{0:s}-200]))", ToCIndex(vmodel));
                        case ValueModel.Bases.X:
                        case ValueModel.Bases.Y:
                        case ValueModel.Bases.M:
                        case ValueModel.Bases.S:
                            return String.Format(
                                vformat.CanWrite
                                    ? "_set_bword({0:s}Bit+{1:s}, {2:d},"
                                    : "_get_bword({0:s}Bit+{1:s}, {2:d})",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel), vmodel.Size);
                        case ValueModel.Bases.K:
                        case ValueModel.Bases.H:
                            return vmodel.Store.Value.ToString();
                    }
                    break;
                case ValueModel.Types.DWORD:
                    switch (vmodel.Base)
                    {
                        case ValueModel.Bases.D:
                        case ValueModel.Bases.AI:
                        case ValueModel.Bases.AO:
                        case ValueModel.Bases.V:
                        case ValueModel.Bases.Z:
                        case ValueModel.Bases.TV:
                            return String.Format("*((int32_t)(&{0:s}Word[{1:s}]))",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel));
                        case ValueModel.Bases.CV:
                            return vmodel.Offset < 200
                                ? String.Format("*((int32_t)(&CVWord[{0:s}]))", ToCIndex(vmodel))
                                : String.Format("CV32DoubleWord[{0:s}-200]", ToCIndex(vmodel));
                        case ValueModel.Bases.X:
                        case ValueModel.Bases.Y:
                        case ValueModel.Bases.M:
                        case ValueModel.Bases.S:
                            return String.Format(
                                vformat.CanWrite
                                    ? "_set_bdword({0:s}Bit+{1:s}, {2:d},"
                                    : "_get_bdword({0:s}Bit+{1:s}, {2:d})",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel), vmodel.Size);
                        case ValueModel.Bases.K:
                        case ValueModel.Bases.H:
                            return vmodel.Store.Value.ToString();
                    }
                    break;
                case ValueModel.Types.FLOAT:
                    switch (vmodel.Base)
                    {
                        case ValueModel.Bases.D:
                        case ValueModel.Bases.AI:
                        case ValueModel.Bases.AO:
                        case ValueModel.Bases.V:
                        case ValueModel.Bases.Z:
                        case ValueModel.Bases.TV:
                            return String.Format("*((_FLOAT)(&{0:s}Word[{1:s}]))",
                                ValueModel.NameOfBases[(int)(vmodel.Base)], ToCIndex(vmodel));
                        case ValueModel.Bases.CV:
                            return vmodel.Offset < 200
                                ? String.Format("*((_FLOAT)(&CVWord[{0:s}]))", ToCIndex(vmodel))
                                : String.Format("*((_FLOAT)(&CV32DoubleWord[{0:s}-200]))", ToCIndex(vmodel));
                        case ValueModel.Bases.K:
                        case ValueModel.Bases.H:
                            return vmodel.Store.Value.ToString();
                    }
                    break;
            }
            return vmodel.Text;
        }

        private string ToCIndex(ValueModel vmodel)
        {
            int intratime = vmodel.IsWordBit ? 16 : 1;
            switch (vmodel.Intra)
            {
                case ValueModel.Bases.V: return String.Format("({0:d}+VWord[{1:d}]*{2:d})", vmodel.Offset, vmodel.IntraOffset, intratime);
                case ValueModel.Bases.Z: return String.Format("({0:d}+ZWord[{1:d}]*{2:d})", vmodel.Offset, vmodel.IntraOffset, intratime);
                default: return String.Format("({0:d})", vmodel.Offset);
            }
        }

        public string LeftCStyle()
        {
            return ToCStyle(lvalue);
        }

        public string RightCStyle()
        {
            return ToCStyle(rvalue);
        }
            


    }

}

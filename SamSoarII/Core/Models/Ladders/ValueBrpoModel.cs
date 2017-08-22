using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Xml.Linq;

using SamSoarII.Core.Simulate;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;

namespace SamSoarII.Core.Models
{
    public class ValueBrpoModel : IModel
    {
        public ValueBrpoModel(ProjectModel _parent)
        {
            parent = _parent;
            children = new ObservableCollection<ValueBrpoElement>();
            children.CollectionChanged += OnChildrenColletionChanged;
        }
        
        public void Dispose()
        {
            foreach (ValueBrpoElement ele in children)
            {
                ele.PropertyChanged -= OnChildrenPropertyChanged;
                ele.Dispose();
            }
            children.CollectionChanged -= OnChildrenColletionChanged;
            children = null;
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ProjectModel parent;
        public ProjectModel Parent { get { return this.parent; } }
        IModel IModel.Parent { get { return Parent; } }

        private ObservableCollection<ValueBrpoElement> children;
        public IList<ValueBrpoElement> Children { get { return this.children; } }
        public event NotifyCollectionChangedEventHandler ChildrenChanged = delegate { };
        private void OnChildrenColletionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ValueBrpoElement ele in e.NewItems)
                    ele.PropertyChanged += OnChildrenPropertyChanged;
            if (e.OldItems != null)
                foreach (ValueBrpoElement ele in e.OldItems)
                {
                    ele.PropertyChanged -= OnChildrenPropertyChanged;
                    ele.Dispose();
                }
            ChildrenChanged(this, e);
            IsModified = true;
        }

        private void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsModified = true;
        }

        private bool ismodified;
        public bool IsModified
        {
            get { return this.ismodified; }
            set { this.ismodified = value; }
        }
        
        #endregion
        
        #region View

        private ValueBrpoWindow view;
        public ValueBrpoWindow View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                ValueBrpoWindow _view = view;
                this.view = null;
                if (_view != null && _view.Core != null) _view.Core = null;
                this.view = value;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View
        {
            get { return View; }
            set { View = (ValueBrpoWindow)value; }
        }

        private ProjectTreeViewItem ptvitem;
        public ProjectTreeViewItem PTVItem
        {
            get { return this.ptvitem; }
            set { this.ptvitem = value; }
        }

        #endregion
        
        public void Compile()
        {
            string cpath = Utility.FileHelper.AppRootPath;
            string hopath = String.Format(@"{0:s}\simug\simuitrp.o", cpath);
            string hcpath = String.Format(@"{0:s}\simug\simuitrp.h", cpath);
            string ccpath = String.Format(@"{0:s}\simug\simuitrp.c", cpath);
            string dllpath = String.Format(@"{0:s}\simug\simuitrp.dll", cpath);
            SimulateDllModel.Encode(hopath, hcpath);
            StreamWriter sw = new StreamWriter(ccpath);
            sw.Write("#include \"simuitrp.h\"\n");
            sw.Write("int8_t isfirst = 1;\n");
            for (int i = 0; i < children.Count(); i++)
            {
                if (!children[i].IsActive) continue;
                switch (children[i].Oper)
                {
                    case ValueBrpoElement.Operators.UPEDGE:
                    case ValueBrpoElement.Operators.DOWNEDGE:
                    case ValueBrpoElement.Operators.CHANGED:
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
            sw.Write("EXPORT int AssertItrps()\n{\n");
            sw.Write("int ret = 0;\n");
            for (int i = children.Count() - 1; i >= 0; i--)
            {
                if (!children[i].IsValid || !children[i].IsActive) continue;
                string lvalue =
                    children[i].LeftCStyle();
                string rvalue = children[i].RightCStyle();
                switch (children[i].Oper)
                {
                    case ValueBrpoElement.Operators.UPEDGE:
                        sw.Write("if (!isfirst && !_last{0:d} && {1:s}) ret = {0:d} + 1;\n", i, lvalue);
                        sw.Write("_last{0:d} = {1:s};\n", i, lvalue);
                        break;
                    case ValueBrpoElement.Operators.DOWNEDGE:
                        sw.Write("if (!isfirst && _last{0:d} && !{1:s}) ret = {0:d} + 1;\n", i, lvalue);
                        sw.Write("_last{0:d} = {1:s};\n", i, lvalue);
                        break;
                    case ValueBrpoElement.Operators.CHANGED:
                        sw.Write("if (!isfirst && _last{0:d} != {1:s}) ret = {0:d} + 1;\n", i, lvalue);
                        sw.Write("_last{0:d} = {1:s};\n", i, lvalue);
                        break;
                    case ValueBrpoElement.Operators.EQUAL:
                        sw.Write("if ({1:s} == {2:s}) ret = {0:d} + 1;\n", i, lvalue, rvalue);
                        break;
                    case ValueBrpoElement.Operators.NOTEQUAL:
                        sw.Write("if ({1:s} != {2:s}) ret = {0:d} + 1;\n", i, lvalue, rvalue);
                        break;
                    case ValueBrpoElement.Operators.LESS:
                        sw.Write("if ({1:s} < {2:s}) ret = {0:d} + 1;\n", i, lvalue, rvalue);
                        break;
                    case ValueBrpoElement.Operators.NOTLESS:
                        sw.Write("if ({1:s} >= {2:s}) ret = {0:d} + 1;\n", i, lvalue, rvalue);
                        break;
                    case ValueBrpoElement.Operators.MORE:
                        sw.Write("if ({1:s} > {2:s}) ret = {0:d} + 1;\n", i, lvalue, rvalue);
                        break;
                    case ValueBrpoElement.Operators.NOTMORE:
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
                = String.Format("\"{0:s}\" -o \"{1:s}\" -shared -DBUILD_DLL",
                    ccpath, dllpath);
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
#if DEBUG
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
#endif
            cmd.Start();
            cmd.WaitForExit();
#if RELEASE
            File.Delete(chpath);
            File.Delete(ccpath);
#endif
        }

        #region Save & Load
        
        public void Save(XElement xele)
        {
            foreach (ValueBrpoElement element in children)
            {
                XElement xele_e = new XElement("Element");
                element.Save(xele_e);
                xele.Add(xele_e);
            }
        }

        public void Load(XElement xele)
        {
            foreach (XElement xele_e in xele.Elements("Element"))
            {
                ValueBrpoElement element = new ValueBrpoElement(this);
                element.Load(xele_e);
                children.Add(element);
            }
        }

        #endregion
    }

    public class ValueBrpoElement : IDisposable, INotifyPropertyChanged
    {
        public ValueBrpoElement(ValueBrpoModel _parent)
        {
            parent = _parent;
            isactive = true;
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

        private ValueBrpoModel parent;
        public ValueBrpoModel Parent { get { return this.parent; } }

        private bool isactive;
        public bool IsActive
        {
            get { return this.isactive; }
            set { this.isactive = value; PropertyChanged(this, new PropertyChangedEventArgs("IsActive")); }
        }
        
        public bool IsBit { get { return lvalue != null && lvalue.Type == ValueModel.Types.BOOL; } }

        private ValueModel lvalue;
        public ValueModel LValue { get { return this.lvalue; } }

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
        }

        public bool IsValid
        {
            get
            {
                if (lvalue == null || lvalue.Text.Equals("???"))
                    return false;
                switch (oper)
                {
                    case Operators.EQUAL:
                    case Operators.NOTEQUAL:
                    case Operators.LESS:
                    case Operators.NOTLESS:
                    case Operators.MORE:
                    case Operators.NOTMORE:
                        if (rvalue == null || rvalue.Text.Equals("???"))
                            return false;
                        break;
                }
                switch (oper)
                {
                    case Operators.UPEDGE:
                    case Operators.DOWNEDGE:
                        if (!IsBit) return false;
                        break;
                    case Operators.LESS:
                    case Operators.MORE:
                    case Operators.NOTLESS:
                    case Operators.NOTMORE:
                        if (IsBit) return false;
                        break;
                }
                return true;
            }
        }
        
        private ValueBrpoTableElement view;
        public ValueBrpoTableElement View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                ValueBrpoTableElement _view = view;
                this.view = null;
                if (_view != null && _view.Core != null) _view.Core = null;
                this.view = value;
                if (view != null && view.Core != this) view.Core = this;
            }
        }

        #endregion
        
        public void Parse(string _lvalue, string _rvalue = "???", string _oper = null, ValueModel.Types _type = ValueModel.Types.WORD)
        {
            if (lvalue != null)
            {
                lvalue.Dispose();
                lvalue = null;
            }
            if (rvalue != null)
            {
                rvalue.Dispose();
                rvalue = null;
            }
            switch (_type)
            {
                case ValueModel.Types.BOOL:
                    ValueModel.Analyzer_Bit.Text = _lvalue;
                    lvalue = ValueModel.Analyzer_Bit.Clone();
                    switch (_rvalue.ToUpper())
                    {
                        case "OFF": _rvalue = "K0"; break;
                        case "ON": _rvalue = "K1"; break;
                    }
                    ValueModel.Analyzer_Bit.Text = _rvalue;
                    rvalue = ValueModel.Analyzer_Bit.Clone();
                    break;
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
            switch (_oper)
            {
                case "": oper = Operators.CHANGED; break;
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
            PropertyChanged(this, new PropertyChangedEventArgs("@All"));
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
                        case ValueModel.Bases.K:
                        case ValueModel.Bases.H:
                            return vmodel.Store.Value.ToString();
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

        #region Save & Load

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("LeftValue", lvalue != null ? lvalue.Text : "");
            xele.SetAttributeValue("RightValue", rvalue != null ? rvalue.Text : "");
            xele.SetAttributeValue("Operation", (int)(oper));
            xele.SetAttributeValue("ValueType", (int)(Type));
        }

        public void Load(XElement xele)
        {
            string _lvalue = xele.Attribute("LeftValue").Value;
            string _rvalue = xele.Attribute("RightValue").Value;
            string _oper = xele.Attribute("Operation").Value;
            ValueModel.Types _type = (ValueModel.Types)(int.Parse(xele.Attribute("ValueType").Value));
            Parse(_lvalue, _rvalue, _oper, _type);
        }

        #endregion
    }

}

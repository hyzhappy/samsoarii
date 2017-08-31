using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class CAMModel : LadderUnitModel
    {
        public CAMModel(LadderNetworkModel _parent) : base(_parent, Types.CAM)
        {
            elements = new ObservableCollection<CAMElement>();
            elements.CollectionChanged += OnElementCollectionChanged;
            numstore = new ValueModel(this, new ValueFormat("NUM", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyWordRegex3 }, null, "圈数存放", "Number Store"));
            maxtarget = new ValueModel(this, new ValueFormat("MAX", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.IntKValueRegex }, null, "最大目标", "Maximum Target"));
            refaddr = new ValueModel(this, new ValueFormat("REF", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyWordRegex3 }, null, "映射地", "Refliction Address"));
            Parse(new string[] { "CV235", "K0"});
            numstore.Text = "D0";
            maxtarget.Text = "K0";
            refaddr.Text = "D0";
        }
        
        public CAMModel(LadderNetworkModel _parent, XElement xele) : base(_parent, xele)
        {

        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            base.Dispose();
            numstore.Dispose();
            maxtarget.Dispose();
            refaddr.Dispose();
            foreach (CAMElement element in elements)
                element.Dispose();
            elements.CollectionChanged -= OnElementCollectionChanged;
            elements = null;
        }

        #region Number

        public override bool IsPLSTable { get { return true; } }

        private ValueModel numstore;
        public ValueModel NumStore { get { return this.numstore; } }

        private ValueModel maxtarget;
        public ValueModel MaxTarget { get { return this.maxtarget; } }

        private ObservableCollection<CAMElement> elements;
        public IList<CAMElement> Elements { get { return this.elements; } }
        private bool elementchangedinvokable = true;
        private void OnElementCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!elementchangedinvokable) return;
            if (e.OldItems != null)
                foreach (CAMElement element in e.OldItems)
                    element.Dispose();
        }

        public enum ReflictModes { None, Only };
        private ReflictModes refmode;
        public ReflictModes RefMode
        {
            get { return this.refmode; }
            set { this.refmode = value; InvokePropertyChanged("RefMode"); }
        }

        private ValueModel refaddr;
        public ValueModel RefAddr
        {
            get { return this.refaddr; }
        }

        #endregion

        #region Methods
        
        public void MoveUp(int start, int end)
        {
            elementchangedinvokable = false;
            CAMElement temp = elements[start - 1];
            for (int i = start - 1; i < end; i++)
                elements[i] = elements[i + 1];
            elements[end] = temp;
            elementchangedinvokable = true;
        }

        public void MoveDown(int start, int end)
        {
            elementchangedinvokable = false;
            CAMElement temp = elements[end + 1];
            for (int i = end + 1; i > start; i--)
                elements[i] = elements[i - 1];
            elements[start] = temp;
            elementchangedinvokable = true;
        }

        #endregion

        #region Save & Load

        public void CreateToDataGrid()
        {
            foreach (CAMElement element in elements)
                element.CreateToDataGrid();
        }

        public void LoadFromDataGrid()
        {
            foreach (CAMElement element in elements)
                element.LoadFromDataGrid();
        }

        public override void Save(XElement xele)
        {
            base.Save(xele);
            xele.SetAttributeValue("NumberStore", numstore.Text);
            xele.SetAttributeValue("MaxTarget", maxtarget.Text);
            xele.SetAttributeValue("RefMode", (int)refmode);
            xele.SetAttributeValue("RefAddr", refaddr.Text);
            foreach (CAMElement element in elements)
            {
                XElement xele_e = new XElement("Element");
                element.Save(xele_e);
                xele.Add(xele_e);
            }
        }

        public override void Load(XElement xele)
        {
            base.Load(xele);
            elements = new ObservableCollection<CAMElement>();
            numstore = new ValueModel(this, new ValueFormat("NUM", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyWordRegex3 }, null, "圈数存放", "Number Store"));
            maxtarget = new ValueModel(this, new ValueFormat("MAX", ValueModel.Types.WORD, true, false, 0, new Regex[] { ValueModel.VerifyWordRegex3, ValueModel.IntKValueRegex }, null, "最大目标", "Maximum Target"));
            refaddr = new ValueModel(this, new ValueFormat("REF", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyWordRegex3 }, null, "映射地", "Refliction Address"));
            numstore.Text = xele.Attribute("NumberStore").Value;
            maxtarget.Text = xele.Attribute("MaxTarget").Value;
            refmode = (ReflictModes)(int.Parse(xele.Attribute("RefMode").Value));
            refaddr.Text = xele.Attribute("RefAddr").Value;
            foreach (XElement xele_e in xele.Elements("Element"))
            {
                CAMElement element = new CAMElement(this);
                element.Load(xele_e);
                elements.Add(element);
            }
        }

        public CAMModel Clone()
        {
            CAMModel that = new CAMModel(Parent);
            that.X = this.X;
            that.Y = this.Y;
            for (int i = 0; i < Children.Count; i++)
                that.Children[i].Text = this.Children[i].Text;
            that.NumStore.Text = this.NumStore.Text;
            that.MaxTarget.Text = this.MaxTarget.Text;
            that.RefMode = this.RefMode;
            that.RefAddr.Text = this.RefAddr.Text;
            foreach (CAMElement element in elements)
                that.Elements.Add(element.Clone(that));
            return that;
        }
        
        #endregion
    }

    public class CAMElement : IDisposable, INotifyPropertyChanged
    {
        public CAMElement() { }

        public CAMElement(CAMModel _parent)
        {
            parent = _parent;
            target = 1;
            address = new ValueModel(parent, new ValueFormat("ADDR", ValueModel.Types.BOOL, true, true, 0, new Regex[] { ValueModel.VerifyBitRegex7 }, null, "操作地址", "Operated Address"));
            address.Text = "Y0";
            mode = Modes.SET;
            CreateToDataGrid();
        }

        public void Dispose()
        {
            parent = null;
            address.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        #region Base

        private CAMModel parent;
        public CAMModel Parent { get { return this.parent; } }

        private int target;
        public int Target
        {
            get { return this.target; }
            set { this.target = value; PropertyChanged(this, new PropertyChangedEventArgs("Target")); }
        }

        private ValueModel address;
        public ValueModel Address
        {
            get { return this.address; }
        }

        public enum Modes { SET, RST };
        private Modes mode;
        public Modes Mode
        {
            get { return this.mode; }
            set { this.mode = value; PropertyChanged(this, new PropertyChangedEventArgs("Mode")); }
        }
        static private string[] NameOfModes = { "置位", "复位" };
        public IList<string> GetNameOfModes() { return NameOfModes; }

        #endregion

        #region DataGrid

        private string target_s;
        public string Target_S
        {
            get { return this.target_s; }
            set { this.target_s = value; PropertyChanged(this, new PropertyChangedEventArgs("Target_S")); }
        }

        private string address_s;
        public string Address_S
        {
            get { return this.address_s; }
            set { this.address_s = value; PropertyChanged(this, new PropertyChangedEventArgs("Address_S")); }
        }

        private string mode_s;
        public string Mode_S
        {
            get { return this.mode_s; ; }
            set { this.mode_s = value; PropertyChanged(this, new PropertyChangedEventArgs("Mode_S"));  }
        }

        #endregion

        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("Target", target);
            xele.SetAttributeValue("Address", address.Text);
            xele.SetAttributeValue("Mode", (int)mode);
        }

        public void Load(XElement xele)
        {
            target = int.Parse(xele.Attribute("Target").Value);
            address.Text = xele.Attribute("Address").Value;
            mode = (Modes)(int.Parse(xele.Attribute("Mode").Value));
        }

        public CAMElement Clone(CAMModel _parent)
        {
            CAMElement that = new CAMElement(_parent);
            this.CreateToDataGrid();
            that.Target_S = this.Target_S;
            that.Address_S = this.Address_S;
            that.Mode_S = this.Mode_S;
            that.LoadFromDataGrid();
            return that;
        }

        public void CreateToDataGrid()
        {
            Target_S = Target.ToString();
            Address_S = Address.Text;
            Mode_S = NameOfModes[(int)mode];
        }

        public void LoadFromDataGrid()
        {
            Target = int.Parse(Target_S);
            Address.Text = Address_S;
            mode = (Modes)(GetNameOfModes().IndexOf(Mode_S));
        }

        #endregion
    }

}

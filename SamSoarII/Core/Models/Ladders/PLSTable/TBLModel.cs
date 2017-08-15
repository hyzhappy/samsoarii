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
    public class TBLModel : LadderUnitModel
    {
        public TBLModel(LadderNetworkModel _parent) : base(_parent, Types.TBL)
        {
            elements = new ObservableCollection<TBLElement>();
            elements.CollectionChanged += OnElementsChanged;
            mode = Modes.Relative;
        }

        public TBLModel(LadderNetworkModel _parent, XElement xele) : base(_parent, xele)
        {
            elements = new ObservableCollection<TBLElement>();
            elements.CollectionChanged += OnElementsChanged;
            mode = Modes.Relative;
        }
    
        public override void Dispose()
        {
            base.Dispose();
            foreach (TBLElement element in elements)
                element.Dispose();
            elements.CollectionChanged -= OnElementsChanged;
            elements.Clear();
            elements = null;
        }
        
        #region Number
        
        private ObservableCollection<TBLElement> elements;
        public IList<TBLElement> Elements { get { return this.elements; } }
        private bool elementchangedinvokable = true;
        private void OnElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!elementchangedinvokable) return;
            foreach (TBLElement element in elements)
                element.InvokePropertyChanged("ID");
            if (e.OldItems != null)
                foreach (TBLElement element in e.OldItems)
                    element.Dispose();
        }

        public enum Modes { Relative, Absolute }
        private Modes mode;
        public Modes Mode
        {
            get { return this.mode; }
            set { this.mode = value; InvokePropertyChanged("Mode"); }
        }

        #endregion

        #region Save & Load

        public void CreateToDataGrid()
        {
            foreach (TBLElement element in elements)
                element.CreateToDataGrid();
        }

        public void LoadFromDataGrid()
        {
            foreach (TBLElement element in elements)
                element.LoadFromDataGrid();
        }

        public TBLModel Clone()
        {
            TBLModel that = new TBLModel(Parent);
            that.X = this.X;
            that.Y = this.Y;
            for (int i = 0; i < Children.Count; i++)
                that.Children[i].Text = this.Children[i].Text;
            that.Mode = this.Mode;
            foreach (TBLElement element in elements)
                that.Elements.Add(element.Clone(that));
            return that;
        }

        #endregion

        public void MoveUp(int start, int end)
        {
            elementchangedinvokable = false;
            TBLElement temp = elements[start - 1];
            for (int i = start - 1; i < end; i++)
                elements[i] = elements[i + 1];
            elements[end] = temp;
            for (int i = start - 1; i <= end; i++)
                elements[i].InvokePropertyChanged("ID");
            elementchangedinvokable = true;
        }

        public void MoveDown(int start, int end)
        {
            elementchangedinvokable = false;
            TBLElement temp = elements[end + 1];
            for (int i = end + 1; i > start; i--)
                elements[i] = elements[i - 1];
            elements[start] = temp;
            for (int i = start; i <= end + 1; i++)
                elements[i].InvokePropertyChanged("ID");
            elementchangedinvokable = true;
        }
    }

    public class TBLElement : INotifyPropertyChanged, IDisposable
    {
        #region Resources
        
        public enum Events { Wait, ATC, Finish }

        static private ValueFormat[] CondFormats = new ValueFormat[] {
            new ValueFormat("C", ValueModel.Types.BOOL, true, false, 0, new Regex[] {ValueModel.VerifyBitRegex9}),
            new ValueFormat("C", ValueModel.Types.DWORD, true, false, 0, new Regex[] {ValueModel.VerifyWordRegex3, ValueModel.VerifyIntKValueRegex}),
            new ValueFormat("C", ValueModel.Types.WORD, true, false, 0, new Regex[] {ValueModel.VerifyIntKValueRegex}) };

        static private string[] WaitEvents = new string[] {
            "wait信号",
            "ATC时间",
            "脉冲发送完成" };

        public IList<string> GetWaitEvents()
        {
            return WaitEvents;
        }

        #endregion

        public TBLElement()
        {
            parent = null;
        }

        public TBLElement(TBLModel _parent)
        {
            parent = _parent;
            freq = 0;
            number = 0;
            waitevent = Events.Wait;
            cond = new ValueModel(null, CondFormats[0]);
            cond.Text = "M0";
            jump = 0;
            end = new ValueModel(null, new ValueFormat("E", ValueModel.Types.BOOL, true, false, 0, new Regex[] { ValueModel.VerifyBitRegex9 }));
            end.Text = "M0";
            CreateToDataGrid();
        }

        public void Dispose()
        {
            parent = null;
            end = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void InvokePropertyChanged(string propertyname)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

        #region Number

        private TBLModel parent;
        public TBLModel Parent { get { return this.parent; } }

        #region Base
        
        private int freq;
        public int Freq { get { return this.freq; } }

        private int number;
        public int Number { get { return this.number; } }
        
        private Events waitevent;
        public Events WaitEvent { get { return this.waitevent; } }
        
        private ValueModel cond;
        public ValueModel Cond { get { return this.cond; } }

        private int jump;
        public int Jump { get { return this.jump; } }

        private ValueModel end;
        public ValueModel End { get { return this.end; } }

        #endregion

        #region DataGrid

        public int ID
        {
            get { return parent.Elements.IndexOf(this); }
        }

        private string freq_s;
        public string Freq_S
        {
            get { return this.freq_s; }
            set { this.freq_s = value; PropertyChanged(this, new PropertyChangedEventArgs("Freq_S")); }
        }

        private string number_s;
        public string Number_S
        {
            get { return this.number_s; }
            set { this.number_s = value; PropertyChanged(this, new PropertyChangedEventArgs("Number_S")); }
        }

        private string waitevent_s;
        public string WaitEvent_S
        {
            get { return this.waitevent_s; }
            set { this.waitevent_s = value; PropertyChanged(this, new PropertyChangedEventArgs("WaitEvent_S")); }
        }

        private string cond_s;
        public string Cond_S
        {
            get { return this.cond_s; }
            set { this.cond_s = value; PropertyChanged(this, new PropertyChangedEventArgs("Cond_S")); }
        }

        private string jump_s;
        public string Jump_S
        {
            get { return this.jump_s; }
            set { this.jump_s = value; PropertyChanged(this, new PropertyChangedEventArgs("Jump_S")); }
        }

        private string end_s;
        public string End_S
        {
            get { return this.end_s; }
            set { this.end_s = value; PropertyChanged(this, new PropertyChangedEventArgs("End_S")); }
        }

        #endregion

        public void LoadFromDataGrid()
        {
            try
            {
                freq = int.Parse(freq_s);
                number = int.Parse(number_s);
                waitevent = (Events)(GetWaitEvents().IndexOf(waitevent_s));
                cond = new ValueModel(null, CondFormats[(int)waitevent]);
                cond.Text = cond_s;
                jump = int.Parse(jump_s);
                end.Text = end_s;
            }
            catch (Exception e)
            {
                if (e is ValueParseException) throw e;
                throw new ValueParseException(e.Message, null);
            }
        }

        public void CreateToDataGrid()
        {
            freq_s = freq.ToString();
            number_s = number.ToString();
            waitevent_s = WaitEvents[(int)waitevent];
            cond_s = cond.Text;
            end_s = end.Text;
            jump_s = jump.ToString();
        }

        public TBLElement Clone(TBLModel _parent)
        {
            TBLElement that = new TBLElement(_parent);
            this.CreateToDataGrid();
            that.Freq_S = this.Freq_S;
            that.Number_S = this.Number_S;
            that.WaitEvent_S = this.WaitEvent_S;
            that.Cond_S = this.Cond_S;
            that.End_S = this.End_S;
            that.LoadFromDataGrid();
            return that;
        }

        #endregion
    }
}

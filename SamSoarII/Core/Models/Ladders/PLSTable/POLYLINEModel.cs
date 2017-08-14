using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public abstract class POLYLINEModel : LadderUnitModel
    {
        public POLYLINEModel(LadderNetworkModel _parent, LadderUnitModel.Types _type) : base(_parent, _type)
        {
        }

        public POLYLINEModel(LadderNetworkModel _parent, XElement xele) : base(_parent, xele)
        {
        }
        
        public override void Dispose()
        {
            base.Dispose();
        }

        #region Number
        
        public enum SystemUnits { Pls }
        private SystemUnits systemunit;
        public SystemUnits SystemUnit
        {
            get { return this.systemunit; }
            set { this.systemunit = value; InvokePropertyChanged("SystemUnit"); }
        }

        public enum ReflictModes { NoReflict, Reflict, Control }
        private ReflictModes reflictmode;
        public ReflictModes ReflictMode
        {
            get { return this.reflictmode; }
            set { this.reflictmode = value; InvokePropertyChanged("ReflictMode"); }
        }
        /*
        private ValueModel reflictlocation;
        public ValueModel ReflictLocation
        {
            get { return this.reflictlocation; }
        }
        */
        #endregion

        public virtual POLYLINEModel Clone()
        {
            POLYLINEModel that = null;
            if (this is POLYLINEIModel)
                that = new POLYLINEIModel(null);
            if (this is POLYLINEFModel)
                that = new POLYLINEFModel(null);
            if (this is LINEIModel)
                that = new LINEIModel(null);
            if (this is LINEFModel)
                that = new LINEFModel(null);
            if (this is LINEIModel)
                that = new LINEIModel(null);
            if (this is LINEFModel)
                that = new LINEFModel(null);
            that.X = this.X;
            that.Y = this.Y;
            for (int i = 0; i < Children.Count; i++)
                that.Children[i].Text = this.Children[i].Text;
            that.SystemUnit = this.SystemUnit;
            that.ReflictMode = this.ReflictMode;
            return that;
        }
    }

    public abstract class Polyline : INotifyPropertyChanged, IDisposable
    {
        protected abstract ValueModel.Types valuetype { get; }
        protected abstract IEnumerable<Regex> regexs { get; }
        public Polyline(POLYLINEModel _parent)
        {
            parent = _parent;
            x = new ValueModel(_parent, new ValueFormat("X", valuetype, true, false, 0, regexs));
            y = new ValueModel(_parent, new ValueFormat("Y", valuetype, true, false, 0, regexs));
            ac = new ValueModel(_parent, new ValueFormat("AC", valuetype, true, false, 0, regexs));
            dc = new ValueModel(_parent, new ValueFormat("DC", valuetype, true, false, 0, regexs));
            v = new ValueModel(_parent, new ValueFormat("V", valuetype, true, false, 0, regexs));
            x.Text = "K0";
            y.Text = "K0";
            ac.Text = "K0";
            dc.Text = "K0";
            v.Text = "K0";
            mode = Modes.Relative;
        }

        public void Dispose()
        {
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void InvokePropertyChanged(string propertyname)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }

        public override string ToString()
        {
            int id = 0;
            if (parent is POLYLINEIModel)
                id = ((POLYLINEIModel)parent).Polylines.IndexOf((IntPolyline)this);
            if (parent is POLYLINEFModel)
                id = ((POLYLINEFModel)parent).Polylines.IndexOf((FloatPolyline)this);
            if (this is IntLine || this is FloatLine)
                return String.Format("{0:d}.{1:s}", id, "直线");
            else
                return String.Format("{0:d}.{1:s}", id, "圆弧");
        }

        #region Number

        private POLYLINEModel parent;
        public POLYLINEModel Parent { get { return this.parent; } }
       
        private ValueModel x;
        public ValueModel X { get { return this.x; } }

        private ValueModel y;
        public ValueModel Y { get { return this.y; } }

        public enum Modes { Relative, Absulate }
        private Modes mode;
        public Modes Mode
        {
            get { return this.mode; }
            set { this.mode = value; InvokePropertyChanged("Mode"); }
        }

        private ValueModel ac;
        public ValueModel AC { get { return this.ac; } }

        private ValueModel dc;
        public ValueModel DC { get { return this.dc; } }

        private ValueModel v;
        public ValueModel V { get { return this.v; } }

        #endregion

        public virtual Polyline Clone(POLYLINEModel _parent = null)
        {
            Polyline that = null;
            if (this is IntLine)
                that = new IntLine(_parent);
            if (this is IntArch)
                that = new IntArch(_parent);
            if (this is FloatLine)
                that = new FloatLine(_parent);
            if (this is FloatArch)
                that = new FloatArch(_parent);
            that.X.Text = this.X.Text;
            that.Y.Text = this.Y.Text;
            that.Mode = this.Mode;
            that.AC.Text = this.AC.Text;
            that.DC.Text = this.DC.Text;
            that.V.Text = this.V.Text;
            return that;
        }
    }


}

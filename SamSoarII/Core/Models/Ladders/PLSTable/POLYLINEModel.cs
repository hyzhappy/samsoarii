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
            Children[0].Text = "K1";
            Children[1].Text = "D0";
            Children[2].Text = "K0";
        }

        public POLYLINEModel(LadderNetworkModel _parent, XElement xele) : base(_parent, xele)
        {
        }
        
        public override void Dispose()
        {
            base.Dispose();
        }

        #region Number
        
        public enum SystemUnits { MM, PLS }
        public abstract SystemUnits Unit { get; }

        public enum ReflictModes { NoReflict, Reflict, Control }
        private ReflictModes reflictmode;
        public ReflictModes ReflictMode
        {
            get { return this.reflictmode; }
            set { this.reflictmode = value; InvokePropertyChanged("ReflictMode"); }
        }

        #endregion

        public override void Save(XElement xele)
        {
            base.Save(xele);
            xele.SetAttributeValue("ReflictMode", (int)(reflictmode));
        }

        public override void Load(XElement xele)
        {
            base.Load(xele);
            XAttribute xatt = xele.Attribute("ReflictMode");
            reflictmode = xatt != null ? (ReflictModes)(int.Parse(xatt.Value)) : ReflictModes.NoReflict;
        }

        public virtual POLYLINEModel Clone()
        {
            POLYLINEModel that = null;
            if (this is POLYLINEIModel) that = new POLYLINEIModel(null);
            if (this is POLYLINEFModel) that = new POLYLINEFModel(null);
            if (this is LINEIModel) that = new LINEIModel(null);
            if (this is LINEFModel) that = new LINEFModel(null);
            if (this is LINEIModel) that = new LINEIModel(null);
            if (this is LINEFModel) that = new LINEFModel(null);
            that.X = this.X;
            that.Y = this.Y;
            for (int i = 0; i < Children.Count; i++)
                that.Children[i].Text = this.Children[i].Text;
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
            x = new ValueModel(_parent, new ValueFormat("X", valuetype, true, false, 0, regexs, null, "X坐标", "Axis X"));
            y = new ValueModel(_parent, new ValueFormat("Y", valuetype, true, false, 0, regexs, null, "Y坐标", "Axis Y"));
            ac = new ValueModel(_parent, new ValueFormat("AC", valuetype, true, false, 0, regexs, null, "加速时间", "Accelerate time"));
            dc = new ValueModel(_parent, new ValueFormat("DC", valuetype, true, false, 0, regexs, null, "减速时间", "Decelerate time"));
            v = new ValueModel(_parent, new ValueFormat("V", valuetype, true, false, 0, regexs, null, "速度", "Velocity"));
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
            if (this is IntLine || this is FloatLine)
                return String.Format("{0:d}.{1:s}", ID, "直线");
            else
                return String.Format("{0:d}.{1:s}", ID, "圆弧");
        }

        #region Number

        private POLYLINEModel parent;
        public POLYLINEModel Parent { get { return this.parent; } }

        public int ID
        {
            get
            {
                if (parent is POLYLINEIModel)
                    return ((POLYLINEIModel)parent).Polylines.IndexOf((IntPolyline)this);
                if (parent is POLYLINEFModel)
                    return ((POLYLINEFModel)parent).Polylines.IndexOf((FloatPolyline)this);
                return 0;
            }
        }

        private ValueModel x;
        public ValueModel X { get { return this.x; } }

        private ValueModel y;
        public ValueModel Y { get { return this.y; } }

        public abstract int Count { get; }

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

        public virtual void Save(XElement xele)
        {
            if (this is IntLine)
                xele.SetAttributeValue("Inherited", "IntLine");
            if (this is IntArch)
                xele.SetAttributeValue("Inherited", "IntArch");
            if (this is FloatLine)
                xele.SetAttributeValue("Inherited", "FloatLine");
            if (this is FloatArch)
                xele.SetAttributeValue("Inherited", "FloatArch");
            xele.SetAttributeValue("X", x.Text);
            xele.SetAttributeValue("Y", y.Text);
            xele.SetAttributeValue("Mode", (int)(mode));
            xele.SetAttributeValue("AC", ac.Text);
            xele.SetAttributeValue("DC", dc.Text);
            xele.SetAttributeValue("V", v.Text);
        }

        public virtual void Load(XElement xele)
        {
            x.Text = xele.Attribute("X").Value;
            y.Text = xele.Attribute("Y").Value;
            mode = (Modes)(int.Parse(xele.Attribute("Mode").Value));
            ac.Text = xele.Attribute("AC").Value;
            dc.Text = xele.Attribute("DC").Value;
            v.Text = xele.Attribute("V").Value;
        }
        
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

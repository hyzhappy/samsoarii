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
    public class POLYLINEFModel : POLYLINEModel
    {
        public POLYLINEFModel(LadderNetworkModel _parent)
            : base(_parent, Types.POLYLINEF)
        {
            polylines = new ObservableCollection<FloatPolyline>();
        }

        public POLYLINEFModel(LadderNetworkModel _parent, XElement xele)
            : base(_parent, xele)
        {
            //polylines = new ObservableCollection<FloatPolyline>();
        }

        public override void Dispose()
        {
            foreach (FloatPolyline pol in polylines) pol.Dispose();
            polylines.Clear();
            polylines = null;
            base.Dispose();
        }

        #region Number

        public override SystemUnits Unit { get { return SystemUnits.MM; } }

        private ObservableCollection<FloatPolyline> polylines;
        public IList<FloatPolyline> Polylines { get { return this.polylines; } }
        public void RefreshPolylines()
        {
            FloatPolyline[] _polylines = polylines.ToArray();
            polylines.Clear();
            foreach (FloatPolyline polf in _polylines)
                polylines.Add(polf);
        }

        #endregion

        public override void Load(XElement xele)
        {
            base.Load(xele);
            if (polylines == null)
                polylines = new ObservableCollection<FloatPolyline>();
            polylines.Clear();
            foreach (XElement xele_p in xele.Elements("Polyline"))
            {
                switch (xele_p.Attribute("Inherited").Value)
                {
                    case "FloatLine":
                        FloatLine linef = new FloatLine(this);
                        linef.Load(xele_p);
                        polylines.Add(linef);
                        break;
                    case "FloatArch":
                        FloatArch archf = new FloatArch(this);
                        archf.Load(xele_p);
                        polylines.Add(archf);
                        break;
                }
            }
        }

        public override void Save(XElement xele)
        {
            base.Save(xele);
            foreach (FloatPolyline polyf in polylines)
            {
                XElement xele_p = new XElement("Polyline");
                polyf.Save(xele_p);
                xele.Add(xele_p);
            }
        }

        public override POLYLINEModel Clone()
        {
            POLYLINEFModel that = (POLYLINEFModel)(base.Clone());
            foreach (FloatPolyline polf in polylines)
                that.Polylines.Add((FloatPolyline)(polf.Clone(that)));
            return that;
        }
    }

    public abstract class FloatPolyline : Polyline
    {
        public FloatPolyline(POLYLINEModel _parent) : base(_parent) { }

        protected override ValueModel.Types valuetype
        {
            get { return ValueModel.Types.FLOAT; }
        }

        protected override IEnumerable<Regex> regexs
        {
            get { return new Regex[] { ValueModel.VerifyFloatRegex, ValueModel.VerifyFloatKValueRegex }; }
        }
    }

    public class FloatLine : FloatPolyline
    {
        public FloatLine(POLYLINEModel _parent) : base(_parent) { }

        public override int Count { get { return 8; } }
    }


    public class FloatArch : FloatPolyline
    {
        public FloatArch(POLYLINEModel _parent) : base(_parent)
        {
            r = new ValueModel(_parent, new ValueFormat("R", valuetype, true, false, 0, regexs, null, "半径", "Radios"));
            r.Text = "K0";
            cx = new ValueModel(_parent, new ValueFormat("CX", valuetype, true, false, 0, regexs, null, "X轴中心点", "Center X"));
            cx.Text = "K0";
            cy = new ValueModel(_parent, new ValueFormat("CY", valuetype, true, false, 0, regexs, null, "Y轴中心点", "Center Y"));
            cy.Text = "K0";
            type = ArchTypes.TwoPoint;
            dir = Directions.Co_Clockwise;
            qua = Qualities.Inferior;
        }

        public override int Count { get { return type == ArchTypes.TwoPoint ? 11 : 13; } }

        public enum ArchTypes { TwoPoint, ThreePoint }
        private ArchTypes type;
        public ArchTypes Type
        {
            get { return this.type; }
            set { this.type = value; InvokePropertyChanged("Type"); }
        }

        private ValueModel r;
        public ValueModel R { get { return this.r; } }

        public enum Directions { Clockwise, Co_Clockwise }
        private Directions dir;
        public Directions Dir
        {
            get { return this.dir; }
            set { this.dir = value; InvokePropertyChanged("Dir"); }
        }

        public enum Qualities { Optimize, Inferior }
        private Qualities qua;
        public Qualities Qua
        {
            get { return this.qua; }
            set { this.qua = value; InvokePropertyChanged("Qua"); }
        }

        private ValueModel cx;
        public ValueModel CX { get { return this.cx; } }

        private ValueModel cy;
        public ValueModel CY { get { return this.cy; } }

        public override void Save(XElement xele)
        {
            base.Save(xele);
            xele.SetAttributeValue("Type", (int)(type));
            xele.SetAttributeValue("Dir", (int)(dir));
            xele.SetAttributeValue("Qua", (int)(qua));
            xele.SetAttributeValue("R", r.Text);
            xele.SetAttributeValue("CX", cx.Text);
            xele.SetAttributeValue("CY", cy.Text);
        }

        public override void Load(XElement xele)
        {
            base.Load(xele);
            type = (ArchTypes)(int.Parse(xele.Attribute("Type").Value));
            dir = (Directions)(int.Parse(xele.Attribute("Dir").Value));
            qua = (Qualities)(int.Parse(xele.Attribute("Qua").Value));
            r.Text = xele.Attribute("R").Value;
            cx.Text = xele.Attribute("CX").Value;
            cy.Text = xele.Attribute("CY").Value;
        }

        public override Polyline Clone(POLYLINEModel _parent = null)
        {
            FloatArch that = (FloatArch)(base.Clone(_parent));
            that.Type = this.Type;
            that.Dir = this.Dir;
            that.Qua = this.Qua;
            that.R.Text = this.R.Text;
            that.CX.Text = this.CX.Text;
            that.CY.Text = this.CY.Text;
            return that;
        }


    }
}

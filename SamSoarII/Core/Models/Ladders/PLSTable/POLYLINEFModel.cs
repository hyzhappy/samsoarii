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
            polylines = new ObservableCollection<FloatPolyline>();
        }

        public override void Dispose()
        {
            foreach (FloatPolyline pol in polylines) pol.Dispose();
            polylines.Clear();
            polylines = null;
            base.Dispose();
        }

        #region Number

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
    }


    public class FloatArch : FloatPolyline
    {
        public FloatArch(POLYLINEModel _parent) : base(_parent)
        {
            r = new ValueModel(_parent, new ValueFormat("R", valuetype, true, false, 0, regexs, null, "半径", "Radios"));
            r.Text = "K0";
            type = ArchTypes.TwoPoint;
            dir = Directions.Co_Clockwise;
            qua = Qualities.Inferior;
        }

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

        public override Polyline Clone(POLYLINEModel _parent = null)
        {
            FloatArch that = (FloatArch)(base.Clone(_parent));
            that.R.Text = this.R.Text;
            return that;
        }


    }
}

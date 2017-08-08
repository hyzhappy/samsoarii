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
    public class POLYLINEIModel : POLYLINEModel
    {
        public POLYLINEIModel(LadderNetworkModel _parent)
            : base(_parent, Types.POLYLINEI)
        {
            polylines = new ObservableCollection<IntPolyline>();
        }

        public POLYLINEIModel(LadderNetworkModel _parent, XElement xele)
            : base(_parent, xele)
        {
            polylines = new ObservableCollection<IntPolyline>();
        }
        
        public override void Dispose()
        {
            foreach (IntPolyline pol in polylines) pol.Dispose();
            polylines.Clear();
            polylines = null;
            base.Dispose();
        }

        #region Number

        private ObservableCollection<IntPolyline> polylines;
        public IList<IntPolyline> Polylines { get { return this.polylines; } }
        public void RefreshPolylines()
        {
            IntPolyline[] _polylines = polylines.ToArray();
            polylines.Clear();
            foreach (IntPolyline poli in _polylines)
                polylines.Add(poli);
        }

        #endregion

        public override POLYLINEModel Clone()
        {
            POLYLINEIModel that = (POLYLINEIModel)(base.Clone());
            foreach (IntPolyline poli in polylines)
                that.Polylines.Add((IntPolyline)(poli.Clone(that)));
            return that;
        }
    }

    public abstract class IntPolyline : Polyline
    {
        public IntPolyline(POLYLINEModel _parent) : base(_parent) { }

        protected override ValueModel.Types valuetype
        {
            get { return ValueModel.Types.DWORD; }
        }

        protected override IEnumerable<Regex> regexs
        {
            get { return new Regex[] { ValueModel.VerifyDoubleWordRegex2, ValueModel.VerifyIntKValueRegex }; }
        }
    }

    public class IntLine : IntPolyline
    {
        public IntLine(POLYLINEModel _parent) : base(_parent) { }
    }

    public class IntArch : IntPolyline
    {
        public IntArch(POLYLINEModel _parent) : base(_parent)
        {
            r = new ValueModel(_parent, new ValueFormat("R", valuetype, true, false, 0, regexs));
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
            IntArch that = (IntArch)(base.Clone(_parent));
            that.R.Text = this.R.Text;
            return that;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class PolylineAxisModel : IModel
    {
        public PolylineAxisModel(PolylineSystemModel _parent, string _pls, string _dir)
        {
            parent = _parent;
            pls = new ValueModel(null, new ValueFormat("PLS", ValueModel.Types.BOOL, false, true, 0, new Regex[] { ValueModel.VerifyBitRegex4 },
                null, "脉冲输出口", "Pulse Output"));
            dir = new ValueModel(null, new ValueFormat("DIR", ValueModel.Types.BOOL, false, true, 0, new Regex[] { ValueModel.VerifyBitRegex4 },
                null, "方向输出口", "Direction Output"));
            wei = new ValueModel(null, new ValueFormat("WEI", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatKValueRegex },
                null, "脉冲当量", "Pulse Weight"));
            lim = new ValueModel(null, new ValueFormat("LIM", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatKValueRegex },
                null, "正向极限位", "Positive Limitation"));
            clm = new ValueModel(null, new ValueFormat("CLM", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatKValueRegex },
                null, "反向极限位", "Navigate Limitation"));
            itv = new ValueModel(null, new ValueFormat("ITV", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatKValueRegex },
                null, "传动间隙", "Transformation Interval"));
            pls.Text = _pls;
            dir.Text = _dir;
            wei.Text = lim.Text = clm.Text = itv.Text = "K0";
        }

        public void Dispose()
        {
            pls.Dispose();
            dir.Dispose();
            wei.Dispose();
            lim.Dispose();
            clm.Dispose();
            itv.Dispose();
            parent = null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private PolylineSystemModel parent;
        public PolylineSystemModel Parent { get { return this.parent; } }
        IModel IModel.Parent { get { return Parent; } }

        private ValueModel pls;
        public ValueModel PLS { get { return this.pls; } }

        private ValueModel dir;
        public ValueModel DIR { get { return this.dir; } }

        private ValueModel wei;
        public ValueModel WEI { get { return this.wei; } }

        private ValueModel lim;
        public ValueModel LIM { get { return this.lim; } }

        private ValueModel clm;
        public ValueModel CLM { get { return this.clm; } }

        private ValueModel itv;
        public ValueModel ITV { get { return this.itv; } }

        #endregion

        #region View

        private PolylineAxisWidget view;
        public PolylineAxisWidget View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                PolylineAxisWidget _view = view;
                this.view = null;
                if (_view != null && _view.Core != null) _view.Core = null;
                this.view = value; ;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View
        {
            get { return View; }
            set { View = (PolylineAxisWidget)value; }
        }

        public ProjectTreeViewItem PTVItem { get; set; }

        #endregion

        #region Save & Load
        
        public void Save(XElement xele)
        {
            xele.SetAttributeValue("PLS", pls.Text);
            xele.SetAttributeValue("DIR", dir.Text);
            xele.SetAttributeValue("WEI", wei.Text);
            xele.SetAttributeValue("LIM", lim.Text);
            xele.SetAttributeValue("CLM", clm.Text);
            xele.SetAttributeValue("ITV", itv.Text);
        }

        public void Load(XElement xele)
        {
            pls.Text = xele.Attribute("PLS").Value;
            dir.Text = xele.Attribute("DIR").Value;
            wei.Text = xele.Attribute("WEI").Value;
            lim.Text = xele.Attribute("LIM").Value;
            clm.Text = xele.Attribute("CLM").Value;
            itv.Text = xele.Attribute("ITV").Value;
        }

        public PolylineAxisModel Clone()
        {
            PolylineAxisModel that = new PolylineAxisModel(parent, pls.Text, dir.Text);
            that.Load(this);
            return that;
        }

        public void Load(PolylineAxisModel that)
        {
            this.pls.Text = that.pls.Text;
            this.dir.Text = that.dir.Text;
            this.wei.Text = that.wei.Text;
            this.lim.Text = that.lim.Text;
            this.clm.Text = that.clm.Text;
            this.itv.Text = that.itv.Text;
        }

        #endregion
    }
}

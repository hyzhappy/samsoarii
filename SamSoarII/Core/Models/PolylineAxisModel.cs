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
        public PolylineAxisModel(PolylineSystemModel _parent)
        {
            parent = _parent;
            pls = new ValueModel(null, new ValueFormat("PLS", ValueModel.Types.BOOL, false, true, 0, new Regex[] { ValueModel.VerifyBitRegex4 }));
            dir = new ValueModel(null, new ValueFormat("DIR", ValueModel.Types.BOOL, false, true, 0, new Regex[] { ValueModel.VerifyBitRegex4 }));
            wei = new ValueModel(null, new ValueFormat("WEI", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatKValueRegex }));
            lim = new ValueModel(null, new ValueFormat("LIM", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatKValueRegex }));
            clm = new ValueModel(null, new ValueFormat("CLM", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatKValueRegex }));
            itv = new ValueModel(null, new ValueFormat("ITV", ValueModel.Types.FLOAT, true, false, 0, new Regex[] { ValueModel.VerifyFloatKValueRegex }));
        }

        public void Dispose()
        {
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
        public ValueModel LIM { get { return this.wei; } }

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

        }

        public void Load(XElement xele)
        {

        }

        #endregion
    }
}

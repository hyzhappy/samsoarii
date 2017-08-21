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
    public class PolylineSystemModel : IModel
    {
        public PolylineSystemModel(ProjectModel _parent)
        {
            parent = _parent;
            x = new PolylineAxisModel(this);
            y = new PolylineAxisModel(this);
            hmi = new ValueModel(null, new ValueFormat("HMI", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyWordRegex3 }));
        }

        public void Dispose()
        {
            parent = null;
            x.Dispose();
            y.Dispose();
            hmi.Dispose();
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ProjectModel parent;
        public ProjectModel Parent { get { return this.parent; } }
        IModel IModel.Parent { get { return Parent; } }

        private bool isenabled;
        public bool IsEnabled
        {
            get { return this.isenabled; }
            set { this.isenabled = value; PropertyChanged(this, new PropertyChangedEventArgs("IsEnabled")); }
        }

        public enum SystemUnits { MM, Pls }
        private SystemUnits unit;
        public SystemUnits Unit
        {
            get { return this.unit; }
            set { this.unit = value; PropertyChanged(this, new PropertyChangedEventArgs("Unit")); }
        }

        public enum OverflowHandles { None, Stop }
        private OverflowHandles overflow;
        public OverflowHandles Overflow
        {
            get { return this.overflow; }
            set { this.overflow = value; PropertyChanged(this, new PropertyChangedEventArgs("Overflow")); }
        }

        private PolylineAxisModel x;
        public PolylineAxisModel X { get { return this.x; } }

        private PolylineAxisModel y;
        public PolylineAxisModel Y { get { return this.y; } }

        private bool ishmienabled;
        public bool IsHMIEnabled
        {
            get { return this.ishmienabled; }
            set { this.ishmienabled = value; PropertyChanged(this, new PropertyChangedEventArgs("IsHMIEnabled")); }
        }

        private ValueModel hmi;
        public ValueModel HMI { get { return this.hmi; } }

        #endregion

        #region Shell

        private PolylineSystemSettingDialog view;
        public PolylineSystemSettingDialog View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                PolylineSystemSettingDialog _view = view;
                this.view = null;
                if (_view != null && _view.Core != null) _view.Core = null;
                this.view = value;
                if (view != null)
                {
                    X.View = view.WG_X;
                    Y.View = view.WG_Y;
                    if (view.Core != this) view.Core = this;
                }
                else
                {
                    X.View = null;
                    Y.View = null;
                }
            }
        }
        IViewModel IModel.View
        {
            get { return View; }
            set { View = (PolylineSystemSettingDialog)value; }
        }

        public ProjectTreeViewItem PTVItem
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IViewModel Visual
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

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

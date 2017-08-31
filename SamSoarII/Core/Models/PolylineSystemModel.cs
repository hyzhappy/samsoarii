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
        #region Resources

        private string[][] XDefaultValues = new string[][]
        {
            new string[] { "Y0", "Y12"},
            new string[] { "Y2", "Y14"},
            new string[] { "Y4", "Y16"},
            new string[] { "Y6", "Y20"},
            new string[] { "Y10", "Y22"},
        };

        private string[][] YDefaultValues = new string[][]
        {
            new string[] { "Y1", "Y13"},
            new string[] { "Y3", "Y15"},
            new string[] { "Y5", "Y17"},
            new string[] { "Y7", "Y21"},
            new string[] { "Y11", "Y23"},
        };

        #endregion

        public PolylineSystemModel(ProjectModel _parent, int _id)
        {
            id = _id;
            parent = _parent;
            isenabled = false;
            x = new PolylineAxisModel(this, XDefaultValues[id - 1][0], XDefaultValues[id - 1][1]);
            y = new PolylineAxisModel(this, YDefaultValues[id - 1][0], YDefaultValues[id - 1][1]);
            ishmienabled = false;
            hmi = new ValueModel(null, new ValueFormat("HMI", ValueModel.Types.WORD, true, true, 0, new Regex[] { ValueModel.VerifyWordRegex3 },
                null, "数据地址", "HMI Address"));
            hmi.Text = "D0";
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

        private int id;
        public int ID { get { return this.id; } }

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
                if (view != null && view.Core != this) view.Core = this;
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

        #endregion

        #region Save & Load
        
        public void Save(XElement xele)
        {
            xele.SetAttributeValue("ID", id);
            xele.SetAttributeValue("IsEnable", isenabled);
            xele.SetAttributeValue("Unit", (int)(unit));
            xele.SetAttributeValue("Overflow", (int)(overflow));
            XElement xele_x = new XElement("X");
            x.Save(xele_x);
            xele.Add(xele_x);
            XElement xele_y = new XElement("Y");
            y.Save(xele_y);
            xele.Add(xele_y);
            xele.SetAttributeValue("IsHMIEnable", ishmienabled);
            xele.SetAttributeValue("HMI", hmi.Text);
        }

        public void Load(XElement xele)
        {
            id = int.Parse(xele.Attribute("ID").Value);
            isenabled = bool.Parse(xele.Attribute("IsEnable").Value);
            unit = (SystemUnits)(int.Parse(xele.Attribute("Unit").Value));
            overflow = (OverflowHandles)(int.Parse(xele.Attribute("Overflow").Value));
            x.Load(xele.Element("X"));
            y.Load(xele.Element("Y"));
            ishmienabled = bool.Parse(xele.Attribute("IsHMIEnable").Value);
            hmi.Text = xele.Attribute("HMI").Value;
        }

        public PolylineSystemModel Clone()
        {
            PolylineSystemModel that = new PolylineSystemModel(parent, id);
            that.Load(this);
            return that;
        }
        
        public void Load(PolylineSystemModel that)
        {
            this.id = that.id;
            this.isenabled = that.isenabled;
            this.unit = that.unit;
            this.overflow = that.overflow;
            this.x.Load(that.x);
            this.y.Load(that.y);
            this.ishmienabled = that.ishmienabled;
            this.hmi.Text = that.hmi.Text;
        }

        #endregion
    }
}

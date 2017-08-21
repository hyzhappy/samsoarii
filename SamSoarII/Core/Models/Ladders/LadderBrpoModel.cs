using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SamSoarII.Core.Simulate;
using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;

namespace SamSoarII.Core.Models
{
    public class LadderBrpoModel : IModel, IBreakpoint
    {
        public LadderBrpoModel(LadderUnitModel _parent)
        {
            parent = _parent;
        }
        
        public void Dispose()
        {
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private LadderUnitModel parent;
        public LadderUnitModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent == value) return;
                LadderUnitModel _parent = parent;
                this.parent = null;
                if (_parent != null)
                {
                    if (_parent.Breakpoint != null) _parent.Breakpoint = null;
                }
                this.parent = value;
                if (parent != null)
                {
                    if (parent.Breakpoint != this) parent.Breakpoint = this;
                }
            }
        }
        IModel IModel.Parent { get { return Parent; } }
        
        private int address;
        public int Address
        {
            get { return this.address; }
            set { this.address = value; }
        }

        private bool isenable;
        public bool IsEnable
        {
            get { return this.isenable; }
            set { this.isenable = value; PropertyChanged(this, new PropertyChangedEventArgs("IsEnable")); }
        }

        private bool isactive;
        public bool IsActive
        {
            get { return this.isactive; }
            set { this.isactive = value; PropertyChanged(this, new PropertyChangedEventArgs("IsActive")); }
        }
        
        public enum Conditions { NONE, ON, OFF, UPEDGE, DOWNEDGE, EDGE};
        private Conditions condition;
        public Conditions Condition
        {
            get
            {
                return this.condition;
            }
            set
            {
                this.condition = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Condition"));
                PropertyChanged(this, new PropertyChangedEventArgs("ConditionIndex"));
            }
        }
        public int ConditionIndex
        {
            get { return (int)Condition; }
            set { Condition = (Conditions)value; }
        }

        private int skipcount;
        public int SkipCount
        {
            get { return this.skipcount; }
            set { this.skipcount = value; }
        }

        private int skiplimit;
        public int SkipLimit
        {
            get { return this.skiplimit; }
            set { this.skiplimit = value; PropertyChanged(this, new PropertyChangedEventArgs("SkipLimit")); }
        }

        private BreakpointCursor cursor;
        public BreakpointCursor Cursor
        {
            get
            {
                return this.cursor;
            }
            set
            {
                if (cursor == value) return;
                BreakpointCursor _cursor = cursor;
                this.cursor = null;
                if (_cursor != null)
                {
                    if (_cursor.Current != null) _cursor.Current = null;
                }
                this.cursor = value;
                if (cursor != null)
                {
                    if (cursor.Current != this) cursor.Current = this;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Cursor"));
            }
        }

        #endregion

        #region View
        
        ProjectTreeViewItem IModel.PTVItem { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        private LadderBrpoViewModel view;
        public LadderBrpoViewModel View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                LadderBrpoViewModel _view = view;
                this.view = null;
                if (_view != null)
                {
                    if (_view.Core != null) _view.Core = null;
                }
                this.view = value;
                if (view != null)
                {
                    if (view.Core != this) view.Core = this;
                }
            }
        }
        IViewModel IModel.View { get { return View; } set { View = (LadderBrpoViewModel)value; } }

        private LadderBrpoTableElement element;
        public LadderBrpoTableElement Element
        {
            get
            {
                return this.element;
            }
            set
            {
                if (element == value) return;
                LadderBrpoTableElement _element = element;
                this.element = null;
                if (_element != null)
                {
                    if (_element.Parent != null) _element.Parent = null;
                }
                this.element = value;
                if (element != null)
                {
                    if (element.Parent != this) element.Parent = this;
                }
            }
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
            xele.SetAttributeValue("IsActive", isactive);
            xele.SetAttributeValue("ConditionIndex", ConditionIndex);
            xele.SetAttributeValue("SkipLimit", skiplimit);
        }

        public void Load(XElement xele)
        {
            isenable = true;
            isactive = bool.Parse(xele.Attribute("IsActive").Value);
            ConditionIndex = int.Parse(xele.Attribute("ConditionIndex").Value);
            skiplimit = int.Parse(xele.Attribute("SkipLimit").Value);
        }
        
        #endregion
    }
}

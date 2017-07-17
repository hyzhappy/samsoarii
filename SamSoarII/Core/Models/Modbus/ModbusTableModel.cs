using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SamSoarII.Shell.Models;
using System.Collections.ObjectModel;
using SamSoarII.Shell.Windows;

namespace SamSoarII.Core.Models
{
    public class ModbusTableModel : IModel
    {
        public ModbusTableModel(ProjectModel _parent)
        {
            parent = _parent;
            children = new ObservableCollection<ModbusModel>();
        }
        
        public void Dispose()
        {
            foreach (ModbusModel modbus in children)
                modbus.Dispose();
            children.Clear();
            children = null;
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ProjectModel parent;
        public ProjectModel Parent { get { return this.parent; } }
        IModel IModel.Parent { get { return Parent; } }

        private ObservableCollection<ModbusModel> children;
        public IList<ModbusModel> Children { get { return children; } }

        #endregion

        #region Shell

        private ModbusTableViewModel view;
        public ModbusTableViewModel View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                ModbusTableViewModel _view = view;
                this.view = value;
                if (_view != null && _view.Core != null) _view.Core = null;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View
        {
            get { return View; }
            set { View = (ModbusTableViewModel)value; }
        }
        
        private ProjectTreeViewItem ptvitem;
        public ProjectTreeViewItem PTVItem
        {
            get { return this.ptvitem; }
            set { this.ptvitem = value; }
        }

        #endregion

        #region Save & Load

        public void Load(XElement xele)
        {
            children.Clear();
            foreach (XElement xele_m in xele.Elements("Table"))
            {
                ModbusModel mmodel = new ModbusModel(this, "# New");
                mmodel.Load(xele_m);
                children.Add(mmodel);
            }
        }

        public void Save(XElement xele)
        {
            foreach (ModbusModel mmodel in children)
            {
                XElement xele_m = new XElement("Table");
                mmodel.Save(xele_m);
                xele.Add(xele_m);
            }
        }

        #endregion
    }
}

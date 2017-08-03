using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SamSoarII.Shell.Models;
using System.Collections.ObjectModel;
using SamSoarII.Shell.Windows;
using System.Collections.Specialized;

namespace SamSoarII.Core.Models
{
    public class ModbusTableModel : IModel
    {
        public ModbusTableModel(ProjectModel _parent)
        {
            isdisposed = false;
            parent = _parent;
            children = new ObservableCollection<ModbusModel>();
            children.CollectionChanged += OnChildrenCollectionChanged;
        }

        protected bool isdisposed;
        public void Dispose()
        {
            isdisposed = true;
            foreach (ModbusModel modbus in children)
            {
                modbus.PropertyChanged -= OnChildrenPropertyChanged;
                modbus.Dispose();
            }
            children.Clear();
            children.CollectionChanged -= OnChildrenCollectionChanged;
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
        public event NotifyCollectionChangedEventHandler ChildrenChanged = delegate { };
        private bool childrenChangedInvokeable = true;
        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!childrenChangedInvokeable) return;
            if (e.OldItems != null)
                foreach (ModbusModel modbus in e.OldItems)
                {
                    modbus.PropertyChanged -= OnChildrenPropertyChanged;
                    modbus.Dispose();
                }
            if (e.NewItems != null)
                foreach (ModbusModel modbus in e.NewItems)
                {
                    modbus.PropertyChanged += OnChildrenPropertyChanged;
                }
            parent.InvokeModify(this);
            ChildrenChanged(this, e);
        }
        private void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ModbusModel mmodel = (ModbusModel)sender;
            parent.InvokeModify(mmodel);
            if (e.PropertyName.Equals("Name"))
            {
                childrenChangedInvokeable = false;
                int id = children.IndexOf(mmodel);
                children.Remove(mmodel);
                children.Insert(id, mmodel);
                childrenChangedInvokeable = true;
                ChildrenChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public void ChildrenSwap(int id1, int id2)
        {
            childrenChangedInvokeable = false;
            ModbusModel temp = children[id1];
            children[id1] = children[id2];
            children[id2] = temp;
            childrenChangedInvokeable = true;
        }

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
            foreach (ModbusModel modbus in children)
            {
                modbus.PropertyChanged -= OnChildrenPropertyChanged;
                modbus.Dispose();
            }
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

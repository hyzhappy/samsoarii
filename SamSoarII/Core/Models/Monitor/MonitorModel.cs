using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using SamSoarII.Shell.Models;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class MonitorModel : IDisposable, IModel
    {
        public MonitorModel(ProjectModel _parent)
        {
            parent = _parent;
            children = new ObservableCollection<MonitorTable>();
            children.CollectionChanged += OnChildrenChanged;
            children.Add(new MonitorTable(this, "Default"));
        }
        
        public void Dispose()
        {
            children.Clear();
            children.CollectionChanged -= OnChildrenChanged;
            children = null;
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ProjectModel parent;
        public ProjectModel Parent { get { return this.parent; } }
        public ValueManager ValueManager { get { return parent.Parent.MNGValue; } }
        IModel IModel.Parent { get { return this.Parent; } }

        private ObservableCollection<MonitorTable> children;
        public IList<MonitorTable> Children { get { return this.children; } }
        public event NotifyCollectionChangedEventHandler ChildrenChanged = delegate { };
        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (MonitorTable table in e.OldItems)
                    table.Dispose();
            ChildrenChanged(this, e);
        }
        #endregion

        #region View

        private MonitorWindow view;
        public MonitorWindow View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                MonitorWindow _view = view;
                this.view = value;
                
            }
        }
        IViewModel IModel.View { get { return this.View; } set { this.View = (MonitorWindow)value; } }

        private ProjectTreeViewItem ptvitem;
        public ProjectTreeViewItem PTVItem
        {
            get { return this.ptvitem; }
            set { this.ptvitem = value; }
        }

        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            foreach (MonitorTable table in Children)
            {
                XElement xele_t = new XElement("Table");
                table.Save(xele_t);
                xele.Add(xele_t);
            }
        }

        public void Load(XElement xele)
        {
            children.Clear();
            foreach (XElement xele_t in xele.Elements("Table"))
            {
                MonitorTable table = new MonitorTable(this, "# New");
                table.Load(xele_t);
                children.Add(table);
            }
        }

        #endregion

    }
}

using SamSoarII.Shell.Models;
using SamSoarII.Shell.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class MonitorTable : IModel
    {
        public MonitorTable(MonitorModel _parent, string _name)
        {
            parent = _parent;
            name = _name;
            children = new ObservableCollection<MonitorElement>();
            children.CollectionChanged += OnChildrenChanged;
        }
       
        public void Dispose()
        {
            foreach (MonitorElement element in children)
                element.Dispose();
            children.Clear();
            children.CollectionChanged -= OnChildrenChanged;
            children = null;
            parent = null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private MonitorModel parent;
        public MonitorModel Parent { get { return this.parent; } }
        public ValueManager ValueManager { get { return parent.ValueManager; } }
        IModel IModel.Parent { get { return this.Parent; } }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; PropertyChanged(this, new PropertyChangedEventArgs("Name")); }
        }

        private ObservableCollection<MonitorElement> children;
        public IList<MonitorElement> Children { get { return this.children; } }
        public event NotifyCollectionChangedEventHandler ChildrenChanged = delegate { };
        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (MonitorElement element in e.OldItems)
                    element.Dispose();
            ChildrenChanged(this, e);
        }
        public bool Contains(MonitorElement element)
        {
            foreach (var child in Children)
            {
                if (child.AddrType == element.AddrType && child.StartAddr == element.StartAddr
                    && child.IsIntrasegment == element.IsIntrasegment && child.IntrasegmentAddr == element.IntrasegmentAddr
                    && child.IntrasegmentType == element.IntrasegmentType) return true;
            }
            return false;
        }
        #endregion

        #region Shell

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
                this.view = null;
                if (_view != null)
                {
                    foreach (MonitorElement element in children.Where(e => e.Store != null))
                        element.Store.VisualRefNum--;
                }
                this.view = value;
                if (view != null)
                {
                    foreach (MonitorElement element in children.Where(e => e.Store != null))
                        element.Store.VisualRefNum++;
                }
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
            xele.SetAttributeValue("Name", name);
            foreach (MonitorElement element in children)
            {
                XElement xele_e = new XElement("Element");
                element.Save(xele_e);
                xele.Add(xele_e);
            }
        }

        public void Load(XElement xele)
        {
            name = xele.Attribute("Name").Value;
            foreach (MonitorElement element in children)
                element.Dispose();
            children.Clear();
            foreach (XElement xele_e in xele.Elements("Element"))
            {
                MonitorElement element = new MonitorElement(this, xele_e);
                children.Add(element);
            }
        }

        #endregion

        #region For MonitorWindow

        private bool isvisited;
        public bool IsVisited
        {
            get { return this.isvisited; }
            set { this.isvisited = value; }
        }

        #endregion

    }
}

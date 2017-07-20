using SamSoarII.Shell.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.Specialized;
using SamSoarII.Shell.Windows;

namespace SamSoarII.Core.Models
{
    public class ModbusModel : IModel
    {
        public ModbusModel(ModbusTableModel _parent, string _name)
        {
            parent = _parent;
            name = _name;
            children = new ObservableCollection<ModbusItem>();
            children.CollectionChanged += OnChildrenChanged;
        }
        
        public void Dispose()
        {
            PropertyChanged = null;
            foreach (ModbusItem item in children)
            {
                item.Dispose();
            }
            children.Clear();
            children = null;
            parent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public override string ToString()
        {
            return Name;
        }

        #region Numbers

        private ModbusTableModel parent;
        public ModbusTableModel Parent
        {
            get { return this.parent; }
        }
        IModel IModel.Parent { get { return Parent; } }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; PropertyChanged(this, new PropertyChangedEventArgs("Name"));}
        }

        private string comment;
        public string Comment
        {
            get { return this.name; }
            set { this.comment = value; PropertyChanged(this, new PropertyChangedEventArgs("Comment")); }
        }

        private ObservableCollection<ModbusItem> children;
        public IList<ModbusItem> Children
        {
            get { return this.children; }
        }
        public event NotifyCollectionChangedEventHandler ChildrenChanged = delegate { };
        private bool childrenChangedInvokable = true;
        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!childrenChangedInvokable) return;
            if (e.OldItems != null)
                foreach (ModbusItem item in e.OldItems)
                    item.Dispose();
            ChildrenChanged(this, e);
        }

        public void ChildrenSwap(int id1, int id2)
        {
            childrenChangedInvokable = false;
            ModbusItem temp = children[id1];
            children[id1] = children[id2];
            children[id2] = temp;
            childrenChangedInvokable = true;
        }

        public bool IsValid
        {
            get
            {
                foreach (ModbusItem item in children)
                    if (!item.IsValid) return false;
                return true;
            }
        }
         

        #endregion

        #region Shell

        public IViewModel View { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        
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
            xele.SetAttributeValue("Comment", comment);
            foreach (ModbusItem item in children)
            {
                XElement xele_i = new XElement("Item");
                item.Save(xele_i);
                xele.Add(xele_i);
            }
        }

        public void Load(XElement xele)
        {
            name = xele.Attribute("Name").Value;
            comment = xele.Attribute("Comment").Value;
            children.Clear();
            foreach (XElement xele_i in xele.Elements("Item"))
            {
                ModbusItem item = new ModbusItem(this);
                item.Load(xele_i);
                children.Add(item);
            }
        }

        #endregion
    }
}

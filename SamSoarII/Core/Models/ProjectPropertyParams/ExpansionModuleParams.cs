using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SamSoarII.Utility;

namespace SamSoarII.Core.Models
{
    public class ExpansionModuleParams : IParams
    {
        public ExpansionModuleParams(ProjectPropertyParams _parent)
        {
            parent = _parent;
            isexpansion = false;
            moduleindex = 0;
            module1typeindex = 0;
            module2typeindex = 0;
            module3typeindex = 0;
            module4typeindex = 0;
            module5typeindex = 0;
            module6typeindex = 0;
            module7typeindex = 0;
            module8typeindex = 0;
            module1typeisused = false;
            module2typeisused = false;
            module3typeisused = false;
            module4typeisused = false;
            module5typeisused = false;
            module6typeisused = false;
            module7typeisused = false;
            module8typeisused = false;
        }
        
        public void Dispose()
        {
            parent = null;
            PropertyChanged = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ProjectPropertyParams parent;
        public ProjectPropertyParams Parent { get { return this.parent; } }

        private bool isexpansion;
        public bool IsExpansion
        {
            get { return this.isexpansion; }
            set { this.isexpansion = value; PropertyChanged(this, new PropertyChangedEventArgs("IsExpansion")); }
        }

        private int moduleindex;
        public int ModuleIndex
        {
            get { return this.moduleindex; }
            set { this.moduleindex = value; PropertyChanged(this, new PropertyChangedEventArgs("ModuleIndex")); }
        }

        private int module1typeindex;
        public int Module1TypeIndex
        {
            get { return this.module1typeindex; }
            set { this.module1typeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("Module1TypeIndex")); }
        }

        private int module2typeindex;
        public int Module2TypeIndex
        {
            get { return this.module2typeindex; }
            set { this.module2typeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("Module2TypeIndex")); }
        }

        private int module3typeindex;
        public int Module3TypeIndex
        {
            get { return this.module3typeindex; }
            set { this.module3typeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("Module3TypeIndex")); }
        }

        private int module4typeindex;
        public int Module4TypeIndex
        {
            get { return this.module4typeindex; }
            set { this.module4typeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("Module4TypeIndex")); }
        }

        private int module5typeindex;
        public int Module5TypeIndex
        {
            get { return this.module5typeindex; }
            set { this.module5typeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("Module5TypeIndex")); }
        }

        private int module6typeindex;
        public int Module6TypeIndex
        {
            get { return this.module6typeindex; }
            set { this.module6typeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("Module6TypeIndex")); }
        }

        private int module7typeindex;
        public int Module7TypeIndex
        {
            get { return this.module7typeindex; }
            set { this.module7typeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("Module7TypeIndex")); }
        }

        private int module8typeindex;
        public int Module8TypeIndex
        {
            get { return this.module8typeindex; }
            set { this.module8typeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("Module8TypeIndex")); }
        }
        
        private bool module1typeisused;
        public bool Module1TypeIsUsed
        {
            get { return this.module1typeisused; }
            set { this.module1typeisused = value; PropertyChanged(this, new PropertyChangedEventArgs("Module1TypeIsUsed")); }
        }

        private bool module2typeisused;
        public bool Module2TypeIsUsed
        {
            get { return this.module2typeisused; }
            set { this.module2typeisused = value; PropertyChanged(this, new PropertyChangedEventArgs("Module2TypeIsUsed")); }
        }

        private bool module3typeisused;
        public bool Module3TypeIsUsed
        {
            get { return this.module3typeisused; }
            set { this.module3typeisused = value; PropertyChanged(this, new PropertyChangedEventArgs("Module3TypeIsUsed")); }
        }

        private bool module4typeisused;
        public bool Module4TypeIsUsed
        {
            get { return this.module4typeisused; }
            set { this.module4typeisused = value; PropertyChanged(this, new PropertyChangedEventArgs("Module4TypeIsUsed")); }
        }

        private bool module5typeisused;
        public bool Module5TypeIsUsed
        {
            get { return this.module5typeisused; }
            set { this.module5typeisused = value; PropertyChanged(this, new PropertyChangedEventArgs("Module5TypeIsUsed")); }
        }

        private bool module6typeisused;
        public bool Module6TypeIsUsed
        {
            get { return this.module6typeisused; }
            set { this.module6typeisused = value; PropertyChanged(this, new PropertyChangedEventArgs("Module6TypeIsUsed")); }
        }

        private bool module7typeisused;
        public bool Module7TypeIsUsed
        {
            get { return this.module7typeisused; }
            set { this.module7typeisused = value; PropertyChanged(this, new PropertyChangedEventArgs("Module7TypeIsUsed")); }
        }

        private bool module8typeisused;
        public bool Module8TypeIsUsed
        {
            get { return this.module8typeisused; }
            set { this.module8typeisused = value; PropertyChanged(this, new PropertyChangedEventArgs("Module8TypeIsUsed")); }
        }

        #endregion

        #region View

        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            xele.Add(new XElement("IsExpansion", isexpansion));
            xele.Add(new XElement("ModuleIndex", moduleindex));
            xele.Add(new XElement("Module1TypeIndex", module1typeindex));
            xele.Add(new XElement("Module2TypeIndex", module2typeindex));
            xele.Add(new XElement("Module3TypeIndex", module3typeindex));
            xele.Add(new XElement("Module4TypeIndex", module4typeindex));
            xele.Add(new XElement("Module5TypeIndex", module5typeindex));
            xele.Add(new XElement("Module6TypeIndex", module6typeindex));
            xele.Add(new XElement("Module7TypeIndex", module7typeindex));
            xele.Add(new XElement("Module8TypeIndex", module8typeindex));
            xele.Add(new XElement("Module1TypeIsUsed", module1typeisused));
            xele.Add(new XElement("Module2TypeIsUsed", module2typeisused));
            xele.Add(new XElement("Module3TypeIsUsed", module3typeisused));
            xele.Add(new XElement("Module4TypeIsUsed", module4typeisused));
            xele.Add(new XElement("Module5TypeIsUsed", module5typeisused));
            xele.Add(new XElement("Module6TypeIsUsed", module6typeisused));
            xele.Add(new XElement("Module7TypeIsUsed", module7typeisused));
            xele.Add(new XElement("Module8TypeIsUsed", module8typeisused));
        }

        public void Load(XElement xele)
        {
            isexpansion = bool.Parse(xele.Element("IsExpansion").Value);
            moduleindex = int.Parse(xele.Element("ModuleIndex").Value);
            module1typeindex = int.Parse(xele.Element("Module1TypeIndex").Value);
            module2typeindex = int.Parse(xele.Element("Module2TypeIndex").Value);
            module3typeindex = int.Parse(xele.Element("Module3TypeIndex").Value);
            module4typeindex = int.Parse(xele.Element("Module4TypeIndex").Value);
            module5typeindex = int.Parse(xele.Element("Module5TypeIndex").Value);
            module6typeindex = int.Parse(xele.Element("Module6TypeIndex").Value);
            module7typeindex = int.Parse(xele.Element("Module7TypeIndex").Value);
            module8typeindex = int.Parse(xele.Element("Module8TypeIndex").Value);
            module1typeisused = bool.Parse(xele.Element("Module1TypeIsUsed").Value);
            module2typeisused = bool.Parse(xele.Element("Module2TypeIsUsed").Value);
            module3typeisused = bool.Parse(xele.Element("Module3TypeIsUsed").Value);
            module4typeisused = bool.Parse(xele.Element("Module4TypeIsUsed").Value);
            module5typeisused = bool.Parse(xele.Element("Module5TypeIsUsed").Value);
            module6typeisused = bool.Parse(xele.Element("Module6TypeIsUsed").Value);
            module7typeisused = bool.Parse(xele.Element("Module7TypeIsUsed").Value);
            module8typeisused = bool.Parse(xele.Element("Module8TypeIsUsed").Value);
        }

        public IParams Clone()
        {
            return Clone(null);
        }

        public ExpansionModuleParams Clone(ProjectPropertyParams parent)
        {
            ExpansionModuleParams that = new ExpansionModuleParams(parent);
            that.Load(this);
            return that;
        }

        public void Load(IParams iparams)
        {
            if (iparams is ExpansionModuleParams)
            {
                ExpansionModuleParams that = (ExpansionModuleParams)iparams;
                this.IsExpansion = that.IsExpansion;
                this.ModuleIndex = that.ModuleIndex;
                this.Module1TypeIndex = that.Module1TypeIndex;
                this.Module2TypeIndex = that.Module2TypeIndex;
                this.Module3TypeIndex = that.Module3TypeIndex;
                this.Module4TypeIndex = that.Module4TypeIndex;
                this.Module5TypeIndex = that.Module5TypeIndex;
                this.Module6TypeIndex = that.Module6TypeIndex;
                this.Module7TypeIndex = that.Module7TypeIndex;
                this.Module8TypeIndex = that.Module8TypeIndex;
                this.Module1TypeIsUsed = that.Module1TypeIsUsed;
                this.Module2TypeIsUsed = that.Module2TypeIsUsed;
                this.Module3TypeIsUsed = that.Module3TypeIsUsed;
                this.Module4TypeIsUsed = that.Module4TypeIsUsed;
                this.Module5TypeIsUsed = that.Module5TypeIsUsed;
                this.Module6TypeIsUsed = that.Module6TypeIsUsed;
                this.Module7TypeIsUsed = that.Module7TypeIsUsed;
                this.Module8TypeIsUsed = that.Module8TypeIsUsed;
            }
        }

        #endregion

    }
}

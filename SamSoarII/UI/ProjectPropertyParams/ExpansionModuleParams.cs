using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI
{
    public class ExpansionModuleParams : INotifyPropertyChanged, IXEleCreateOrLoad
    {
        public ExpansionModuleParams()
        {
            InitializeProperty();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void InitializeProperty()
        {
            PropertyChanged = delegate { };
            _isExpansion = false;
            _moduleIndex = 0;
            _module1TypeIndex = 0;
            _module2TypeIndex = 0;
            _module3TypeIndex = 0;
            _module4TypeIndex = 0;
            _module5TypeIndex = 0;
            _module6TypeIndex = 0;
            _module7TypeIndex = 0;
            _module8TypeIndex = 0;
            _module1IsUsed = false;
            _module2IsUsed = false;
            _module3IsUsed = false;
            _module4IsUsed = false;
            _module5IsUsed = false;
            _module6IsUsed = false;
            _module7IsUsed = false;
            _module8IsUsed = false;
        }
        private bool _isExpansion;
        private int _moduleIndex;
        private int _module1TypeIndex;
        private int _module2TypeIndex;
        private int _module3TypeIndex;
        private int _module4TypeIndex;
        private int _module5TypeIndex;
        private int _module6TypeIndex;
        private int _module7TypeIndex;
        private int _module8TypeIndex;
        private bool _module1IsUsed;
        private bool _module2IsUsed;
        private bool _module3IsUsed;
        private bool _module4IsUsed;
        private bool _module5IsUsed;
        private bool _module6IsUsed;
        private bool _module7IsUsed;
        private bool _module8IsUsed;
        public bool IsExpansion
        {
            get
            {
                return _isExpansion;
            }
            set
            {
                _isExpansion = value;
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("IsExpansion"));
            }
        }
        public int ModuleIndex
        {
            get
            {
                return _moduleIndex;
            }
            set
            {
                _moduleIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ModuleIndex"));
            }
        }
        public int Module1TypeIndex
        {
            get
            {
                return _module1TypeIndex;
            }
            set
            {
                _module1TypeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module1TypeIndex"));
            }
        }
        public int Module2TypeIndex
        {
            get
            {
                return _module2TypeIndex;
            }
            set
            {
                _module2TypeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module2TypeIndex"));
            }
        }
        public int Module3TypeIndex
        {
            get
            {
                return _module3TypeIndex;
            }
            set
            {
                _module3TypeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module3TypeIndex"));
            }
        }
        public int Module4TypeIndex
        {
            get
            {
                return _module4TypeIndex;
            }
            set
            {
                _module4TypeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module4TypeIndex"));
            }
        }
        public int Module5TypeIndex
        {
            get
            {
                return _module5TypeIndex;
            }
            set
            {
                _module5TypeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module5TypeIndex"));
            }
        }
        public int Module6TypeIndex
        {
            get
            {
                return _module6TypeIndex;
            }
            set
            {
                _module6TypeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module6TypeIndex"));
            }
        }
        public int Module7TypeIndex
        {
            get
            {
                return _module7TypeIndex;
            }
            set
            {
                _module7TypeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module7TypeIndex"));
            }
        }
        public int Module8TypeIndex
        {
            get
            {
                return _module8TypeIndex;
            }
            set
            {
                _module8TypeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module8TypeIndex"));
            }
        }
        public bool Module1IsUsed
        {
            get
            {
                return _module1IsUsed;
            }
            set
            {
                _module1IsUsed = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module1IsUsed"));
            }
        }
        public bool Module2IsUsed
        {
            get
            {
                return _module2IsUsed;
            }
            set
            {
                _module2IsUsed = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module2IsUsed"));
            }
        }
        public bool Module3IsUsed
        {
            get
            {
                return _module3IsUsed;
            }
            set
            {
                _module3IsUsed = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module3IsUsed"));
            }
        }
        public bool Module4IsUsed
        {
            get
            {
                return _module4IsUsed;
            }
            set
            {
                _module4IsUsed = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module4IsUsed"));
            }
        }
        public bool Module5IsUsed
        {
            get
            {
                return _module5IsUsed;
            }
            set
            {
                _module5IsUsed = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module5IsUsed"));
            }
        }
        public bool Module6IsUsed
        {
            get
            {
                return _module6IsUsed;
            }
            set
            {
                _module6IsUsed = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module6IsUsed"));
            }
        }
        public bool Module7IsUsed
        {
            get
            {
                return _module7IsUsed;
            }
            set
            {
                _module7IsUsed = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module7IsUsed"));
            }
        }
        public bool Module8IsUsed
        {
            get
            {
                return _module8IsUsed;
            }
            set
            {
                _module8IsUsed = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Module8IsUsed"));
            }
        }
        public XElement CreateRootXElement()
        {
            XElement rootNode = new XElement("ExpanModuleParams");
            rootNode.Add(new XElement("IsExpansion", IsExpansion));
            rootNode.Add(new XElement("ModuleIndex", ModuleIndex));
            rootNode.Add(new XElement("Module1TypeIndex", Module1TypeIndex));
            rootNode.Add(new XElement("Module2TypeIndex", Module2TypeIndex));
            rootNode.Add(new XElement("Module3TypeIndex", Module3TypeIndex));
            rootNode.Add(new XElement("Module4TypeIndex", Module4TypeIndex));
            rootNode.Add(new XElement("Module5TypeIndex", Module5TypeIndex));
            rootNode.Add(new XElement("Module6TypeIndex", Module6TypeIndex));
            rootNode.Add(new XElement("Module7TypeIndex", Module7TypeIndex));
            rootNode.Add(new XElement("Module8TypeIndex", Module8TypeIndex));
            rootNode.Add(new XElement("Module1IsUsed", Module1IsUsed));
            rootNode.Add(new XElement("Module2IsUsed", Module2IsUsed));
            rootNode.Add(new XElement("Module3IsUsed", Module3IsUsed));
            rootNode.Add(new XElement("Module4IsUsed", Module4IsUsed));
            rootNode.Add(new XElement("Module5IsUsed", Module5IsUsed));
            rootNode.Add(new XElement("Module6IsUsed", Module6IsUsed));
            rootNode.Add(new XElement("Module7IsUsed", Module7IsUsed));
            rootNode.Add(new XElement("Module8IsUsed", Module8IsUsed));
            return rootNode;
        }
        public void LoadPropertyByXElement(XElement ele)
        {
            IsExpansion = bool.Parse(ele.Element("IsExpansion").Value);
            ModuleIndex = int.Parse(ele.Element("ModuleIndex").Value);
            Module1TypeIndex = int.Parse(ele.Element("Module1TypeIndex").Value);
            Module2TypeIndex = int.Parse(ele.Element("Module2TypeIndex").Value);
            Module3TypeIndex = int.Parse(ele.Element("Module3TypeIndex").Value);
            Module4TypeIndex = int.Parse(ele.Element("Module4TypeIndex").Value);
            Module5TypeIndex = int.Parse(ele.Element("Module5TypeIndex").Value);
            Module6TypeIndex = int.Parse(ele.Element("Module6TypeIndex").Value);
            Module7TypeIndex = int.Parse(ele.Element("Module7TypeIndex").Value);
            Module8TypeIndex = int.Parse(ele.Element("Module8TypeIndex").Value);
            Module1IsUsed = bool.Parse(ele.Element("Module1IsUsed").Value);
            Module2IsUsed = bool.Parse(ele.Element("Module2IsUsed").Value);
            Module3IsUsed = bool.Parse(ele.Element("Module3IsUsed").Value);
            Module4IsUsed = bool.Parse(ele.Element("Module4IsUsed").Value);
            Module5IsUsed = bool.Parse(ele.Element("Module5IsUsed").Value);
            Module6IsUsed = bool.Parse(ele.Element("Module6IsUsed").Value);
            Module7IsUsed = bool.Parse(ele.Element("Module7IsUsed").Value);
            Module8IsUsed = bool.Parse(ele.Element("Module8IsUsed").Value);
        }
    }
}

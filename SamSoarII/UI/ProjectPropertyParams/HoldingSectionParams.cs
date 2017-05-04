using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SamSoarII.Utility;

namespace SamSoarII.AppMain.UI
{
    public class HoldingSectionParams : INotifyPropertyChanged, IXEleCreateOrLoad
    {
        public HoldingSectionParams()
        {
            InitializeProperty();
        }
        private void InitializeProperty()
        {
            PropertyChanged = delegate { };
            _mStartAddr = 7504;
            _mLength = 496;
            _dStartAddr = 5968;
            _dLength = 2032;
            _sStartAddr = 600;
            _sLength = 400;
            _cvStartAddr = 100;
            _cvLength = 100;
            _notClear = false;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private int _mStartAddr;
        private int _mLength;
        private int _dStartAddr;
        private int _dLength;
        private int _sStartAddr;
        private int _sLength;
        private int _cvStartAddr;
        private int _cvLength;
        private bool _notClear;
        public int MStartAddr
        {
            get
            {
                return _mStartAddr;
            }
            set
            {
                _mStartAddr = value;
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("MStartAddr"));
            }
        }
        public int MLength
        {
            get
            {
                return _mLength;
            }
            set
            {
                _mLength = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("MLength"));
            }
        }
        public int DStartAddr
        {
            get
            {
                return _dStartAddr;
            }
            set
            {
                _dStartAddr = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DStartAddr"));
            }
        }
        public int DLength
        {
            get
            {
                return _dLength;
            }
            set
            {
                _dLength = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DLength"));
            }
        }
        public int SStartAddr
        {
            get
            {
                return _sStartAddr;
            }
            set
            {
                _sStartAddr = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SStartAddr"));
            }
        }
        public int SLength
        {
            get
            {
                return _sLength;
            }
            set
            {
                _sLength = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SLength"));
            }
        }
        public int CVStartAddr
        {
            get
            {
                return _cvStartAddr;
            }
            set
            {
                _cvStartAddr = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CVStartAddr"));
            }
        }
        public int CVLength
        {
            get
            {
                return _cvLength;
            }
            set
            {
                _cvLength = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CVLength"));
            }
        }
        public bool NotClear
        {
            get
            {
                return _notClear;
            }
            set
            {
                _notClear = value;
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("NotClear"));
            }
        }
        public XElement CreateRootXElement()
        {
            XElement rootNode = new XElement("HoldingSectParams");
            rootNode.Add(new XElement("MStartAddr", MStartAddr));
            rootNode.Add(new XElement("MLength", MLength));
            rootNode.Add(new XElement("DStartAddr", DStartAddr));
            rootNode.Add(new XElement("DLength", DLength));
            rootNode.Add(new XElement("SStartAddr", SStartAddr));
            rootNode.Add(new XElement("SLength", SLength));
            rootNode.Add(new XElement("CVStartAddr", CVStartAddr));
            rootNode.Add(new XElement("CVLength", CVLength));
            rootNode.Add(new XElement("NotClear", NotClear));
            return rootNode;
        }

        public void LoadPropertyByXElement(XElement ele)
        {
            MStartAddr = int.Parse(ele.Element("MStartAddr").Value);
            MLength = int.Parse(ele.Element("MLength").Value);
            DStartAddr = int.Parse(ele.Element("DStartAddr").Value);
            DLength = int.Parse(ele.Element("DLength").Value);
            SStartAddr = int.Parse(ele.Element("SStartAddr").Value);
            SLength = int.Parse(ele.Element("SLength").Value);
            CVStartAddr = int.Parse(ele.Element("CVStartAddr").Value);
            CVLength = int.Parse(ele.Element("CVLength").Value);
            NotClear = bool.Parse(ele.Element("NotClear").Value);
        }
    }
}

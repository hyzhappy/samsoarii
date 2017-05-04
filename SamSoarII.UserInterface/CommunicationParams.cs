using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.UserInterface
{
    public class CommunicationParams : INotifyPropertyChanged, IXEleCreateOrLoad
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _isCOMLinked;
        private bool _isAutoCheck;
        private int _serialPortIndex;
        private int _baudRateIndex;
        private int _dataBitIndex;
        private int _stopBitIndex;
        private int _checkCodeIndex;
        private int _timeout;
        public int SerialPortIndex
        {
            get
            {
                return _serialPortIndex;
            }
            set
            {
                _serialPortIndex = value;
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("SerialPortIndex"));
            }
        }
        public int BaudRateIndex
        {
            get
            {
                return _baudRateIndex;
            }
            set
            {
                _baudRateIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("BaudRateIndex"));
            }
        }
        public int DataBitIndex
        {
            get
            {
                return _dataBitIndex;
            }
            set
            {
                _dataBitIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DataBitIndex"));
            }
        }
        public int StopBitIndex
        {
            get
            {
                return _stopBitIndex;
            }
            set
            {
                _stopBitIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StopBitIndex"));
            }
        }
        public int CheckCodeIndex
        {
            get
            {
                return _checkCodeIndex;
            }
            set
            {
                _checkCodeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CheckCodeIndex"));
            }
        }
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Timeout"));
            }
        }
        public bool IsCOMLinked
        {
            get
            {
                return _isCOMLinked;
            }
            set
            {
                _isCOMLinked = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsCOMLinked"));
            }
        }
        public bool IsAutoCheck
        {
            get
            {
                return _isAutoCheck;
            }
            set
            {
                _isAutoCheck = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsAutoCheck"));
            }
        }
        public CommunicationParams()
        {
            InitializeProperty();
        }
        private void InitializeProperty()
        {
            PropertyChanged = delegate { };
            _serialPortIndex = 0;
            _baudRateIndex = 1;
            _dataBitIndex = 0;
            _stopBitIndex = 0;
            _checkCodeIndex = 0;
            _timeout = 20;
            _isCOMLinked = false;
            _isAutoCheck = true;
        }

        public XElement CreateRootXElement()
        {
            XElement rootNode = new XElement("CommunicationParams");
            rootNode.Add(new XElement("SerialPortIndex", SerialPortIndex));
            rootNode.Add(new XElement("BaudRateIndex", BaudRateIndex));
            rootNode.Add(new XElement("DataBitIndex", DataBitIndex));
            rootNode.Add(new XElement("StopBitIndex", StopBitIndex));
            rootNode.Add(new XElement("CheckCodeIndex", CheckCodeIndex));
            rootNode.Add(new XElement("Timeout", Timeout));
            rootNode.Add(new XElement("IsCOMLinked", IsCOMLinked));
            rootNode.Add(new XElement("IsAutoCheck", IsAutoCheck));
            return rootNode;
        }
        public void LoadPropertyByXElement(XElement ele)
        {
            SerialPortIndex = int.Parse(ele.Element("SerialPortIndex").Value);
            BaudRateIndex = int.Parse(ele.Element("BaudRateIndex").Value);
            DataBitIndex = int.Parse(ele.Element("DataBitIndex").Value);
            StopBitIndex = int.Parse(ele.Element("StopBitIndex").Value);
            CheckCodeIndex = int.Parse(ele.Element("CheckCodeIndex").Value);
            Timeout = int.Parse(ele.Element("Timeout").Value);
            IsCOMLinked = bool.Parse(ele.Element("IsCOMLinked").Value);
            IsAutoCheck = bool.Parse(ele.Element("IsAutoCheck").Value);
        }
    }
}

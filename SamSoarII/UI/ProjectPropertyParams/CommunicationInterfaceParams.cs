using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI
{
    public class CommunicationInterfaceParams : INotifyPropertyChanged, IXEleCreateOrLoad
    {
        public CommunicationInterfaceParams()
        {
            InitializeProperty();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private int _baudRateIndex;
        private int _dataBitIndex;
        private int _stopBitIndex;
        private int _checkCodeIndex;
        private int _bufferBitIndex;
        private int _stationNum;
        private int _timeout;
        public CommunicationType CommuType { get; set; }
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
        public int BufferBitIndex
        {
            get
            {
                return _bufferBitIndex;
            }
            set
            {
                _bufferBitIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("BufferBitIndex"));
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
        public int StationNum
        {
            get
            {
                return _stationNum;
            }
            set
            {
                _stationNum = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StationNum"));
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
        public void InitializeProperty()
        {
            PropertyChanged = delegate { };
            _baudRateIndex = 1;
            _dataBitIndex = 0;
            _stopBitIndex = 0;
            _checkCodeIndex = 0;
            _bufferBitIndex = 0;
            _stationNum = 2;
            _timeout = 20;
            CommuType = CommunicationType.Master;
        }
        
        private IEnumerable<XElement> CreatePropertyXElements()
        {
            List<XElement> temp = new List<XElement>();
            temp.Add(new XElement("BaudRateIndex", BaudRateIndex));
            temp.Add(new XElement("DataBitIndex", DataBitIndex));
            temp.Add(new XElement("StopBitIndex", StopBitIndex));
            temp.Add(new XElement("CheckCodeIndex", CheckCodeIndex));
            temp.Add(new XElement("BufferBitIndex", BufferBitIndex));
            temp.Add(new XElement("StationNum", StationNum));
            temp.Add(new XElement("Timeout", Timeout));
            temp.Add(new XElement("CommuType", CommuType));
            return temp;
        }
        public void LoadPropertyByXElement(XElement ele)
        {
            BaudRateIndex = int.Parse(ele.Element("BaudRateIndex").Value);
            DataBitIndex = int.Parse(ele.Element("DataBitIndex").Value);
            StopBitIndex = int.Parse(ele.Element("StopBitIndex").Value);
            CheckCodeIndex = int.Parse(ele.Element("CheckCodeIndex").Value);
            BufferBitIndex = int.Parse(ele.Element("BufferBitIndex").Value);
            StationNum = int.Parse(ele.Element("StationNum").Value);
            Timeout = int.Parse(ele.Element("Timeout").Value);
            CommuType = (CommunicationType)Enum.Parse(typeof(CommunicationType), ele.Element("CommuType").Value);
        }

        public XElement CreateRootXElement()
        {
            var rootNode = new XElement("CommuParams");
            foreach (var ele in CreatePropertyXElements())
            {
                rootNode.Add(ele);
            }
            return rootNode;
        }
    }
}

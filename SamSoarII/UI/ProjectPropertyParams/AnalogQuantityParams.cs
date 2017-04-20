using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI
{
    public class AnalogQuantityParams : INotifyPropertyChanged, IXEleCreateOrLoad
    {
        public AnalogQuantityParams()
        {
            InitializeProperty();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private int _inputPassIndex;
        private int _inputModeIndex;
        private int _samplingtimesIndex;
        private int _samplingValue;
        private int _inputStartRange;
        private int _inputEndRange;
        private int _outputPassIndex;
        private int _outputModeIndex;
        private int _outputStartRange;
        private int _outputEndRange;
        private bool _isUsed;
        public int InputPassIndex
        {
            get
            {
                return _inputPassIndex;
            }
            set
            {
                _inputPassIndex = value;
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs("InputPassIndex"));
            }
        }
        public int InputModeIndex
        {
            get
            {
                return _inputModeIndex;
            }
            set
            {
                _inputModeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("InputModeIndex"));
            }
        }
        public int SamplingtimesIndex
        {
            get
            {
                return _samplingtimesIndex;
            }
            set
            {
                _samplingtimesIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SamplingtimesIndex"));
            }
        }
        public int SamplingValue
        {
            get
            {
                return _samplingValue;
            }
            set
            {
                _samplingValue = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SamplingValue"));
            }
        }
        public int InputStartRange
        {
            get
            {
                return _inputStartRange;
            }
            set
            {
                _inputStartRange = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("InputStartRange"));
            }
        }
        public int InputEndRange
        {
            get
            {
                return _inputEndRange;
            }
            set
            {
                _inputEndRange = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("InputEndRange"));
            }
        }
        public int OutputPassIndex
        {
            get
            {
                return _outputPassIndex;
            }
            set
            {
                _outputPassIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OutputPassIndex"));
            }
        }
        public int OutputModeIndex
        {
            get
            {
                return _outputModeIndex;
            }
            set
            {
                _outputModeIndex = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OutputModeIndex"));
            }
        }
        public int OutputStartRange
        {
            get
            {
                return _outputStartRange;
            }
            set
            {
                _outputStartRange = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OutputStartRange"));
            }
        }
        public int OutputEndRange
        {
            get
            {
                return _outputEndRange;
            }
            set
            {
                _outputEndRange = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OutputEndRange"));
            }
        }
        public bool IsUsed
        {
            get
            {
                return _isUsed;
            }
            set
            {
                _isUsed = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsUsed"));
            }
        }
        private void InitializeProperty()
        {
            PropertyChanged = delegate { };
            _isUsed = false;
            _inputPassIndex = 0;
            _inputModeIndex = 0;
            _samplingtimesIndex = 0;
            _samplingValue = 1000;
            _inputStartRange = 0;
            _inputEndRange = 65535;
            _outputPassIndex = 0;
            _outputModeIndex = 0;
            _outputStartRange = 0;
            _outputEndRange = 65535;
        }
        public XElement CreateRootXElement()
        {
            XElement rootNode = new XElement("AnalogQuantityParams");
            rootNode.Add(new XElement("InputPassIndex", InputPassIndex));
            rootNode.Add(new XElement("InputModeIndex", InputModeIndex));
            rootNode.Add(new XElement("SamplingtimesIndex", SamplingtimesIndex));
            rootNode.Add(new XElement("SamplingValue", SamplingValue));
            rootNode.Add(new XElement("InputStartRange", InputStartRange));
            rootNode.Add(new XElement("InputEndRange", InputEndRange));
            rootNode.Add(new XElement("OutputPassIndex", OutputPassIndex));
            rootNode.Add(new XElement("OutputModeIndex", OutputModeIndex));
            rootNode.Add(new XElement("OutputStartRange", OutputStartRange));
            rootNode.Add(new XElement("OutputEndRange", OutputEndRange));
            rootNode.Add(new XElement("IsUsed", IsUsed));
            return rootNode;
        }
        public void LoadPropertyByXElement(XElement ele)
        {
            InputPassIndex = int.Parse(ele.Element("InputPassIndex").Value);
            InputModeIndex = int.Parse(ele.Element("InputModeIndex").Value);
            SamplingtimesIndex = int.Parse(ele.Element("SamplingtimesIndex").Value);
            SamplingValue = int.Parse(ele.Element("SamplingValue").Value);
            InputStartRange = int.Parse(ele.Element("InputStartRange").Value);
            InputEndRange = int.Parse(ele.Element("InputEndRange").Value);
            OutputPassIndex = int.Parse(ele.Element("OutputPassIndex").Value);
            OutputModeIndex = int.Parse(ele.Element("OutputModeIndex").Value);
            OutputStartRange = int.Parse(ele.Element("OutputStartRange").Value);
            OutputEndRange = int.Parse(ele.Element("OutputEndRange").Value);
            IsUsed = bool.Parse(ele.Element("IsUsed").Value);
        }
    }
}

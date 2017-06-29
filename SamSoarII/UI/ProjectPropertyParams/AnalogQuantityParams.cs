using SamSoarII.Utility;
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
            get { return _inputPassIndex; }
            set
            {
                if (_inputPassIndex != value)
                {
                    _inputPassIndex = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("InputPassIndex"));
                    ProjectPropertyManager.IsModify = true;
                }
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
                if (_inputModeIndex != value)
                {
                    _inputModeIndex = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("InputModeIndex"));
                    ProjectPropertyManager.IsModify = true;
                }
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
                if (_samplingtimesIndex != value)
                {
                    _samplingtimesIndex = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SamplingtimesIndex"));
                    ProjectPropertyManager.IsModify = true;
                }
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
                if (_samplingValue != value)
                {
                    _samplingValue = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SamplingValue"));
                    ProjectPropertyManager.IsModify = true;
                }
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

                if (_inputStartRange != value)
                {
                    _inputStartRange = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("InputStartRange"));
                    ProjectPropertyManager.IsModify = true;
                }
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
                if (_inputEndRange != value)
                {
                    _inputEndRange = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("InputEndRange"));
                    ProjectPropertyManager.IsModify = true;
                }
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
                if (_outputPassIndex != value)
                {
                    _outputPassIndex = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OutputPassIndex"));
                    ProjectPropertyManager.IsModify = true;
                }
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
                if (_outputModeIndex != value)
                {
                    _outputModeIndex = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OutputModeIndex"));
                    ProjectPropertyManager.IsModify = true;
                }
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
                if (_outputStartRange != value)
                {
                    _outputStartRange = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OutputStartRange"));
                    ProjectPropertyManager.IsModify = true;
                }
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
                if (_outputEndRange != value)
                {
                    _outputEndRange = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OutputEndRange"));
                    ProjectPropertyManager.IsModify = true;
                }
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
                if (_isUsed != value)
                {
                    _isUsed = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsUsed"));
                    ProjectPropertyManager.IsModify = true;
                }
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

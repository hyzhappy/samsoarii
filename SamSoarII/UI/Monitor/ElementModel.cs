using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI.Monitor
{
    public class ElementModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public string AddrType { get; set; }
        public uint StartAddr { get; set; }
        public string IntrasegmentType { get; set; } = string.Empty;
        public uint IntrasegmentAddr { get; set; } = 0;
        private string _currentValue = string.Format("????");
        private string _setValue = string.Empty;
        private string[] _showTypes;
        public bool IsModify { get; set; } = true;
        public bool IsIntrasegment { get; set; }
        public string ShowName
        {
            get
            {
                if (!IsIntrasegment)
                {
                    return AddrType + StartAddr;
                }
                else
                {
                    return string.Format("{0}{1}{2}{3}", AddrType, StartAddr, IntrasegmentType, IntrasegmentAddr);
                }
            }
        }
        public string CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                _currentValue = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentValue"));
            }
        }
        public string SetValue
        {
            get
            {
                return _setValue;
            }
            set
            {
                _setValue = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SetValue"));
            }
        }
        public string[] ShowTypes
        {
            get
            {
                return _showTypes;
            }
            set
            {
                _showTypes = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ShowTypes"));
            }
        }
        public int DataType { get; set; }
        public int SelectIndex
        {
            get
            {
                if (DataType == 0)
                {
                    return DataType;
                }
                else
                {
                    return DataType - 1;
                }
            }
            set
            {
                if (DataType != 0)
                {
                    if (DataType != value + 1)
                    {
                        DataType = value + 1;
                        IsModify = true;
                    }
                }
            }
        }
        public ElementModel(bool isIntrasegment, Enum dataType)
        {
            IsIntrasegment = isIntrasegment;
            if (dataType is BitType)
            {
                DataType = (int)(BitType)dataType;
            }
            else
            {
                DataType = (int)(WordType)dataType;
            }
            SetShowTypes();
        }
        public ElementModel(bool isIntrasegment, int dataType)
        {
            IsIntrasegment = isIntrasegment;
            DataType = dataType;
            SetShowTypes();
        }
        public void ShowPropertyChanged()
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ShowName"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ShowTypes"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SelectIndex"));
        }
        public ElementModel() { }
        public void SetShowTypes()
        {
            if (DataType == 0)
            {
                ShowTypes = Enum.GetNames(typeof(BitType));
            }
            else
            {
                ShowTypes = Enum.GetNames(typeof(WordType));
            }
        }
        public XElement CreateXElementBySelf()
        {
            XElement rootNode = new XElement("Element");
            rootNode.SetAttributeValue("DataType", DataType);
            rootNode.SetAttributeValue("AddrType", AddrType);
            rootNode.SetAttributeValue("StartAddr", StartAddr);
            rootNode.SetAttributeValue("IntrasegmentType", IntrasegmentType);
            rootNode.SetAttributeValue("IntrasegmentAddr", IntrasegmentAddr);
            rootNode.SetAttributeValue("IsIntrasegment", IsIntrasegment);
            return rootNode;
        }
        public void LoadSelfByXElement(XElement rootNode)
        {
            DataType = int.Parse(rootNode.Attribute("DataType").Value);
            AddrType = rootNode.Attribute("AddrType").Value;
            StartAddr = uint.Parse(rootNode.Attribute("StartAddr").Value);
            IntrasegmentAddr = uint.Parse(rootNode.Attribute("IntrasegmentAddr").Value);
            IntrasegmentType = rootNode.Attribute("IntrasegmentType").Value;
            IsIntrasegment = bool.Parse(rootNode.Attribute("IsIntrasegment").Value);
            SetShowTypes();
        }
    }
}

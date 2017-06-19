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
    public class FilterParams : INotifyPropertyChanged, IXEleCreateOrLoad
    {
        public FilterParams()
        {
            InitializeProperty();
        }
        private void InitializeProperty()
        {
            PropertyChanged = delegate { };
            _isChecked = true;
            _filterTimeIndex = 5;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
                    ProjectPropertyManager.IsModify = true;
                }
            }
        }
        private int _filterTimeIndex;
        public int FilterTimeIndex
        {
            get
            {
                return _filterTimeIndex;
            }
            set
            {
                if (_filterTimeIndex != value)
                {
                    _filterTimeIndex = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("FilterTimeIndex"));
                    ProjectPropertyManager.IsModify = true;
                }
            }
        }
        public XElement CreateRootXElement()
        {
            XElement rootNode = new XElement("FilterParams");
            rootNode.Add(new XElement("IsChecked", IsChecked));
            rootNode.Add(new XElement("FilterTimeIndex", FilterTimeIndex));
            return rootNode;
        }
        public void LoadPropertyByXElement(XElement ele)
        {
            IsChecked = bool.Parse(ele.Element("IsChecked").Value);
            FilterTimeIndex = int.Parse(ele.Element("FilterTimeIndex").Value);
        }
    }
}

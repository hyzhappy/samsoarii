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
    public class PasswordParams : INotifyPropertyChanged, IXEleCreateOrLoad
    {
        public PasswordParams()
        {
            InitializeProperty();
        }
        private void InitializeProperty()
        {
            PropertyChanged = delegate { };
            _upIsChecked = false;
            _dpIsChecked = false;
            _mpIsChecked = false;
            _uPassword = string.Empty;
            _dPassword = string.Empty;
            _mPassword = string.Empty;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _upIsChecked;
        private bool _dpIsChecked;
        private bool _mpIsChecked;
        private string _uPassword;
        private string _dPassword;
        private string _mPassword;
        public bool UPIsChecked
        {
            get { return _upIsChecked; }
            set
            {
                if (_upIsChecked != value)
                {
                    _upIsChecked = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("UPIsChecked"));
                    ProjectPropertyManager.IsModify = true;
                }
            }
        }
        public bool DPIsChecked
        {
            get { return _dpIsChecked; }
            set
            {
                if (_dpIsChecked != value)
                {
                    _dpIsChecked = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("DPIsChecked"));
                    ProjectPropertyManager.IsModify = true;
                }
            }
        }
        public bool MPIsChecked
        {
            get { return _mpIsChecked; }
            set
            {
                if (_mpIsChecked != value)
                {
                    _mpIsChecked = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("MPIsChecked"));
                    ProjectPropertyManager.IsModify = true;
                }
            }
        }
        public string UPassword
        {
            get { return _uPassword; }
            set
            {
                if (_uPassword != value)
                {
                    _uPassword = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("UPassword"));
                    ProjectPropertyManager.IsModify = true;
                }
            }
        }
        public string DPassword
        {
            get { return _dPassword; }
            set
            {
                if (_dPassword != value)
                {
                    _dPassword = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("DPassword"));
                    ProjectPropertyManager.IsModify = true;
                }
            }
        }
        public string MPassword
        {
            get { return _mPassword; }
            set
            {
                if (_mPassword != value)
                {
                    _mPassword = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("MPassword"));
                    ProjectPropertyManager.IsModify = true;
                }
            }
        }
        public XElement CreateRootXElement()
        {
            XElement rootNode = new XElement("PasswordParams");
            rootNode.Add(new XElement("UPIsChecked", UPIsChecked));
            rootNode.Add(new XElement("UPassword", StringHelper.Encryption(UPassword)));
            rootNode.Add(new XElement("DPIsChecked", DPIsChecked));
            rootNode.Add(new XElement("DPassword", StringHelper.Encryption(DPassword)));
            rootNode.Add(new XElement("MPIsChecked", MPIsChecked));
            rootNode.Add(new XElement("MPassword", StringHelper.Encryption(MPassword)));
            return rootNode;
        }

        public void LoadPropertyByXElement(XElement ele)
        {
            UPIsChecked = bool.Parse(ele.Element("UPIsChecked").Value);
            UPassword = StringHelper.Decrypt(ele.Element("UPassword").Value);
            DPIsChecked = bool.Parse(ele.Element("DPIsChecked").Value);
            DPassword = StringHelper.Decrypt(ele.Element("DPassword").Value);
            MPIsChecked = bool.Parse(ele.Element("MPIsChecked").Value);
            MPassword = StringHelper.Decrypt(ele.Element("MPassword").Value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class USBCommunicationParams : IParams
    {
        public USBCommunicationParams(ProjectPropertyParams _parent)
        {
            parent = _parent;
            timeout = 20;
        }

        private ProjectPropertyParams parent;
        public ProjectPropertyParams Parent { get { return this.parent; } }

        private int timeout;
        public int Timeout
        {
            get { return this.timeout; }
            set { this.timeout = value; PropertyChanged(this, new PropertyChangedEventArgs("Timeout")); }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public IParams Clone()
        {
            return Clone(null);
        }

        public void Dispose()
        {
            parent = null;
            PropertyChanged = null;
        }

        public void Load(IParams iparams)
        {
            if (iparams is USBCommunicationParams)
            {
                USBCommunicationParams that = (USBCommunicationParams)iparams;
                this.Timeout = that.Timeout;
            }
        }

        public void Load(XElement xele)
        {
            try
            {
                int value = int.Parse(xele.Element("Timeout").Value);
                timeout = value > 0 ? value : 20;
            }
            catch (Exception) { timeout = 20; }
        }

        public void Save(XElement xele)
        {
            xele.Add(new XElement("Timeout", timeout));
        }

        public USBCommunicationParams Clone(ProjectPropertyParams parent)
        {
            USBCommunicationParams that = new USBCommunicationParams(parent);
            that.Load(this);
            return that;
        }

        public bool CheckParams()
        {
            return true;
        }
    }
}

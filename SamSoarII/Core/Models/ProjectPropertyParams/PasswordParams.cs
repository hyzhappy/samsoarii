using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SamSoarII.Utility;

namespace SamSoarII.Core.Models
{
    public class PasswordParams : IParams
    {
        public PasswordParams(ProjectPropertyParams _parent)
        {
            parent = _parent;
            pwDownload = "";
            pwUpload = "";
            pwMonitor = "";
            pwenDownload = false;
            pwenUpload = false;
            pwenMonitor = false;
        }

        public void Dispose()
        {
            parent = null;
            PropertyChanged = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ProjectPropertyParams parent;
        public ProjectPropertyParams Parent { get { return this.parent; } }
        
        private bool pwenUpload;
        public bool PWENUpload
        {
            get { return this.pwenDownload; }
            set { this.pwenDownload = value; PropertyChanged(this, new PropertyChangedEventArgs("PWENUpload")); }
        }

        private bool pwenDownload;
        public bool PWENDownload
        {
            get { return this.pwenDownload; }
            set { this.pwenDownload = value; PropertyChanged(this, new PropertyChangedEventArgs("PWENDownload")); }
        }

        private bool pwenMonitor;
        public bool PWENMonitor
        {
            get { return this.pwenMonitor; }
            set { this.pwenMonitor = value; PropertyChanged(this, new PropertyChangedEventArgs("PWENMonitor")); }
        }

        private string pwUpload;
        public string PWUpload
        {
            get { return this.pwUpload; }
            set { this.pwUpload = value; PropertyChanged(this, new PropertyChangedEventArgs("PWUpload")); }
        }

        private string pwDownload;
        public string PWDownload
        {
            get { return this.pwDownload; }
            set { this.PWDownload = value; PropertyChanged(this, new PropertyChangedEventArgs("PWDownload")); }
        }

        private string pwMonitor;
        public string PWMonitor
        {
            get { return this.pwMonitor; }
            set { this.pwMonitor = value; PropertyChanged(this, new PropertyChangedEventArgs("PWMonitor")); }
        }

        #endregion

        #region View

        #endregion
        
        #region Save & Load
        
        public void Save(XElement xele)
        {
            xele.Add(new XElement("IsUploadProtection", pwenUpload));
            xele.Add(new XElement("UploadPassword", StringHelper.Encryption(pwUpload)));
            xele.Add(new XElement("IsDownloadProtection", pwenDownload));
            xele.Add(new XElement("DownloadPassword", StringHelper.Encryption(pwDownload)));
            xele.Add(new XElement("IsMonitorProtection", pwenMonitor));
            xele.Add(new XElement("MonitorPassword", StringHelper.Encryption(pwMonitor)));
        }

        public void Load(XElement xele)
        {
            pwenUpload = bool.Parse(xele.Element("IsUploadProtection").Value);
            pwUpload = StringHelper.Decrypt(xele.Element("UploadPassword").Value);
            pwenDownload = bool.Parse(xele.Element("IsDownloadProtection").Value);
            pwDownload = StringHelper.Decrypt(xele.Element("DownloadPassword").Value);
            pwenMonitor = bool.Parse(xele.Element("IsMonitorProtection").Value);
            pwMonitor = StringHelper.Decrypt(xele.Element("MonitorPassword").Value);
        }


        public IParams Clone()
        {
            return Clone(null);
        }

        public PasswordParams Clone(ProjectPropertyParams parent)
        {
            PasswordParams that = new PasswordParams(parent);
            that.Load(this);
            return that;
        }

        public void Load(IParams iparams)
        {
            if (iparams is PasswordParams)
            {
                PasswordParams that = (PasswordParams)iparams;
                this.PWUpload = that.PWUpload;
                this.PWENUpload = that.PWENUpload;
                this.PWDownload = that.PWDownload;
                this.PWENDownload = that.PWENDownload;
                this.PWMonitor = that.PWMonitor;
                this.PWENMonitor = that.PWENMonitor;
            }
        }

        #endregion

    }
}

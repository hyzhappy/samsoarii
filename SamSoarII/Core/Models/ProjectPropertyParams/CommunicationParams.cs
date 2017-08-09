using SamSoarII.Core.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class CommunicationParams : IParams
    {
        public CommunicationParams(ProjectPropertyParams _parent)
        {
            parent = _parent;
            serialportindex = 0;
            baudrateindex = 1;
            databitindex = 0;
            stopbitindex = 0;
            checkcodeindex = 0;
            timeout = 20;
            iscomlinked = false;
            isautocheck = true;
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

        private bool iscomlinked;
        public bool IsComLinked
        {
            get { return this.iscomlinked; }
            set { this.iscomlinked = value; PropertyChanged(this, new PropertyChangedEventArgs("IsComLinked")); }
        }

        private bool isautocheck;
        public bool IsAutoCheck
        {
            get { return this.isautocheck; }
            set { this.isautocheck = value; PropertyChanged(this, new PropertyChangedEventArgs("IsAutoCheck")); }
        }

        private int serialportindex;
        public int SerialPortIndex
        {
            get { return this.serialportindex; }
            set { this.serialportindex = value; PropertyChanged(this, new PropertyChangedEventArgs("SerialPortIndex")); }
        }

        private int baudrateindex;
        public int BaudRateIndex
        {
            get { return this.baudrateindex; }
            set { this.baudrateindex = value; PropertyChanged(this, new PropertyChangedEventArgs("BaudRateIndex")); }
        }

        private int databitindex;
        public int DataBitIndex
        {
            get { return this.databitindex; }
            set { this.databitindex = value; PropertyChanged(this, new PropertyChangedEventArgs("DataBitIndex")); }
        }

        private int stopbitindex;
        public int StopBitIndex
        {
            get { return this.stopbitindex; }
            set { this.stopbitindex = value; PropertyChanged(this, new PropertyChangedEventArgs("StopBitIndex")); }
        }

        private int checkcodeindex;
        public int CheckCodeIndex
        {
            get { return this.checkcodeindex; }
            set { this.checkcodeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("CheckCodeIndex")); }
        }

        private int timeout;
        public int Timeout
        {
            get { return this.timeout; }
            set
            {
                this.timeout = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Timeout"));
            }
        }

        private int downloadoption;
        public int DownloadOption
        {
            get { return this.downloadoption; }
            set { this.downloadoption = value; PropertyChanged(this, new PropertyChangedEventArgs("DownloadOption")); }
        }
        public bool IsDownloadProgram
        {
            get { return (downloadoption & CommunicationDataDefine.OPTION_PROGRAM) != 0; }
        }
        public bool IsDownloadComment
        {
            get { return (downloadoption & CommunicationDataDefine.OPTION_COMMENT) != 0; }
        }
        public bool IsDownloadInitialize
        {
            get { return (downloadoption & CommunicationDataDefine.OPTION_INITIALIZE) != 0; }
        }
        public bool IsDownloadSetting
        {
            get { return (downloadoption & CommunicationDataDefine.OPTION_SETTING) != 0; }
        }

        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            xele.Add(new XElement("SerialPortIndex", serialportindex));
            xele.Add(new XElement("BaudRateIndex", baudrateindex));
            xele.Add(new XElement("DataBitIndex", databitindex));
            xele.Add(new XElement("StopBitIndex", stopbitindex));
            xele.Add(new XElement("CheckCodeIndex", checkcodeindex));
            xele.Add(new XElement("Timeout", timeout));
            xele.Add(new XElement("IsCOMLinked", iscomlinked));
            xele.Add(new XElement("IsAutoCheck", isautocheck));
            xele.Add(new XElement("DownloadOption", downloadoption));
        }

        public void Load(XElement xele)
        {
            try {serialportindex = int.Parse(xele.Element("SerialPortIndex").Value);} catch (Exception) {}
            try {baudrateindex = int.Parse(xele.Element("BaudRateIndex").Value);} catch (Exception) {}
            try {databitindex = int.Parse(xele.Element("DataBitIndex").Value);} catch (Exception) {}
            try {stopbitindex = int.Parse(xele.Element("StopBitIndex").Value);} catch (Exception) {}
            try {checkcodeindex = int.Parse(xele.Element("CheckCodeIndex").Value);} catch (Exception) {}
            try
            {
                int value = int.Parse(xele.Element("Timeout").Value);
                timeout = value > 0 ? value : 20;
            } catch (Exception) {}
            try {iscomlinked = bool.Parse(xele.Element("IsCOMLinked").Value);} catch (Exception) {}
            try {isautocheck = bool.Parse(xele.Element("IsAutoCheck").Value);} catch (Exception) {}
            try {downloadoption = int.Parse(xele.Element("DownloadOption").Value);} catch (Exception) {}
        }

        public IParams Clone()
        {
            return Clone(null);
        }

        public CommunicationParams Clone(ProjectPropertyParams _parent)
        {
            CommunicationParams that = new CommunicationParams(_parent);
            that.Load(this);
            return that;
        }
        public bool LoadSuccess { get; set; }
        public void Load(IParams iparams)
        {
            if (iparams is CommunicationParams)
            {
                CommunicationParams that = (CommunicationParams)iparams;
                this.SerialPortIndex = that.SerialPortIndex;
                this.BaudRateIndex = that.BaudRateIndex;
                this.DataBitIndex = that.DataBitIndex;
                this.StopBitIndex = that.StopBitIndex;
                this.CheckCodeIndex = that.CheckCodeIndex;
                this.Timeout = that.Timeout;
                this.IsComLinked = that.IsComLinked;
                this.IsAutoCheck = that.IsAutoCheck;
                this.DownloadOption = that.DownloadOption;
            }
            LoadSuccess = true;
        }

        #endregion
    }
}

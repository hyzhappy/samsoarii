﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SamSoarII.Utility;

namespace SamSoarII.Core.Models
{
    public class CommunicationInterfaceParams : IParams
    {
        public string Name;
        public CommunicationInterfaceParams(ProjectPropertyParams _parent,string name)
        {
            Name = name;
            parent = _parent;
            baudrateindex = 1;
            databitindex = 0;
            stopbitindex = 0;
            checkcodeindex = 0;
            bufferbitindex = 0;
            stationnumber = 2;
            timeout = 20;
            comtype = ComTypes.Master;
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
        
        public enum ComTypes { Master, Slave, FreePort };
        private ComTypes comtype;
        public ComTypes ComType
        {
            get { return this.comtype; }
            set { this.comtype = value; PropertyChanged(this, new PropertyChangedEventArgs("ComType")); }
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

        private int bufferbitindex;
        public int BufferBitIndex
        {
            get { return this.bufferbitindex; }
            set { this.bufferbitindex = value; PropertyChanged(this, new PropertyChangedEventArgs("BufferBitIndex")); }
        }

        private int stationnumber;
        public int StationNumber
        {
            get { return this.stationnumber; }
            set { this.stationnumber = value; PropertyChanged(this, new PropertyChangedEventArgs("StationNumber")); }
        }

        private int timeout;
        public int Timeout
        {
            get { return this.timeout; }
            set { this.timeout = value; PropertyChanged(this, new PropertyChangedEventArgs("Timeout")); }
        }

        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            xele.Add(new XElement("BaudRateIndex", baudrateindex));
            xele.Add(new XElement("DataBitIndex", databitindex));
            xele.Add(new XElement("StopBitIndex", stopbitindex));
            xele.Add(new XElement("CheckCodeIndex", checkcodeindex));
            xele.Add(new XElement("BufferBitIndex", bufferbitindex));
            xele.Add(new XElement("StationNumber", stationnumber));
            xele.Add(new XElement("Timeout", timeout));
            xele.Add(new XElement("ComType", (int)comtype));
        }

        public void Load(XElement xele)
        {
            baudrateindex = int.Parse(xele.Element("BaudRateIndex").Value);
            databitindex = int.Parse(xele.Element("DataBitIndex").Value);
            stopbitindex = int.Parse(xele.Element("StopBitIndex").Value);
            checkcodeindex = int.Parse(xele.Element("CheckCodeIndex").Value);
            bufferbitindex = int.Parse(xele.Element("BufferBitIndex").Value);
            timeout = int.Parse(xele.Element("Timeout").Value);
            comtype = (ComTypes)(int.Parse(xele.Element("ComType").Value));
        }

        public IParams Clone()
        {
            return Clone(null);
        }

        public CommunicationInterfaceParams Clone(ProjectPropertyParams parent)
        {
            CommunicationInterfaceParams that = new CommunicationInterfaceParams(parent,this.Name);
            that.Load(this);
            return that;
        }

        public void Load(IParams iparams)
        {
            if (iparams is CommunicationInterfaceParams)
            {
                CommunicationInterfaceParams that = (CommunicationInterfaceParams)iparams;
                this.BaudRateIndex = that.BaudRateIndex;
                this.DataBitIndex = that.DataBitIndex;
                this.StopBitIndex = that.StopBitIndex;
                this.CheckCodeIndex = that.CheckCodeIndex;
                this.BufferBitIndex = that.BufferBitIndex;
                this.StationNumber = that.StationNumber;
                this.Timeout = that.Timeout;
            }
        }
        #endregion
        
    }
}
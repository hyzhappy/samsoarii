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
    public class HoldingSectionParams : IParams
    {
        public HoldingSectionParams(ProjectPropertyParams _parent)
        {
            parent = _parent;
            mStartAddr = 7504;
            mLength = 496;
            dStartAddr = 5968;
            dLength = 2032;
            sStartAddr = 600;
            sLength = 400;
            cvStartAddr = 100;
            cvLength = 100;
            notclear = false;
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

        private int mStartAddr;
        public int MStartAddr
        {
            get { return this.mStartAddr; }
            set { this.mStartAddr = value; PropertyChanged(this, new PropertyChangedEventArgs("MStartAddr")); }
        }

        private int mLength;
        public int MLength
        {
            get { return this.mLength; }
            set { this.mLength = value; PropertyChanged(this, new PropertyChangedEventArgs("MLength")); }
        }

        private int dStartAddr;
        public int DStartAddr
        {
            get { return this.dStartAddr; }
            set { this.dStartAddr = value; PropertyChanged(this, new PropertyChangedEventArgs("DStartAddr")); }
        }

        private int dLength;
        public int DLength
        {
            get { return this.dLength; }
            set { this.dLength = value; PropertyChanged(this, new PropertyChangedEventArgs("DLength")); }
        }

        private int sStartAddr;
        public int SStartAddr
        {
            get { return this.sStartAddr; }
            set { this.sStartAddr = value; PropertyChanged(this, new PropertyChangedEventArgs("SStartAddr")); }
        }

        private int sLength;
        public int SLength
        {
            get { return this.sLength; }
            set { this.sLength = value; PropertyChanged(this, new PropertyChangedEventArgs("SLength")); }
        }

        private int cvStartAddr;
        public int CVStartAddr
        {
            get { return this.cvStartAddr; }
            set { this.cvStartAddr = value; PropertyChanged(this, new PropertyChangedEventArgs("CVStartAddr")); }
        }

        private int cvLength;
        public int CVLength
        {
            get { return this.cvLength; }
            set { this.cvLength = value; PropertyChanged(this, new PropertyChangedEventArgs("CVLength")); }
        }

        private bool notclear;
        public bool NotClear
        {
            get { return this.notclear; }
            set { this.notclear = value; PropertyChanged(this, new PropertyChangedEventArgs("NotClear")); }
        }

        #endregion

        #region View

        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            xele.Add(new XElement("MStartAddr", mStartAddr));
            xele.Add(new XElement("MLength", mLength));
            xele.Add(new XElement("DStartAddr", dStartAddr));
            xele.Add(new XElement("DLength", dLength));
            xele.Add(new XElement("SStartAddr", sStartAddr));
            xele.Add(new XElement("SLength", sLength));
            xele.Add(new XElement("CVStartAddr", cvStartAddr));
            xele.Add(new XElement("CVLength", cvLength));
            xele.Add(new XElement("NotClear", notclear));
        }

        public void Load(XElement xele)
        {
            try
            {
                mStartAddr = int.Parse(xele.Element("MStartAddr").Value);
                mLength = int.Parse(xele.Element("MLength").Value);
                dStartAddr = int.Parse(xele.Element("DStartAddr").Value);
                dLength = int.Parse(xele.Element("DLength").Value);
                sStartAddr = int.Parse(xele.Element("SStartAddr").Value);
                sLength = int.Parse(xele.Element("SLength").Value);
                cvStartAddr = int.Parse(xele.Element("CVStartAddr").Value);
                cvLength = int.Parse(xele.Element("CVLength").Value);
                notclear = bool.Parse(xele.Element("NotClear").Value);
            }
            catch (Exception)
            {
            }
        }


        public IParams Clone()
        {
            return Clone(null);
        }

        public HoldingSectionParams Clone(ProjectPropertyParams parent)
        {
            HoldingSectionParams that = new HoldingSectionParams(parent);
            that.Load(this);
            return that;
        }
        public void Load(IParams iparams)
        {
            if (iparams is HoldingSectionParams)
            {
                HoldingSectionParams that = (HoldingSectionParams)iparams;
                this.MStartAddr = that.MStartAddr;
                this.MLength = that.MLength;
                this.DStartAddr = that.DStartAddr;
                this.DLength = that.DLength;
                this.SStartAddr = that.SStartAddr;
                this.SLength = that.SLength;
                this.CVStartAddr = that.CVStartAddr;
                this.CVLength = that.CVLength;
                this.NotClear = that.NotClear;
            }
        }

        public bool CheckParams()
        {
            return true;
        }

        #endregion

    }
}

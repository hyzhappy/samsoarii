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
    public class FilterParams : IParams
    {
        public FilterParams(ProjectPropertyParams _parent)
        {
            parent = _parent;
            ischecked = true;
            filtertimeindex = 5;
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

        private bool ischecked;
        public bool IsChecked
        {
            get { return this.ischecked; }
            set { this.ischecked = value; PropertyChanged(this, new PropertyChangedEventArgs("IsChecked")); }
        }

        private int filtertimeindex;
        public int FilterTimeIndex
        {
            get { return this.filtertimeindex; }
            set { this.filtertimeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("FilterTimeIndex")); }
        }

        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            xele.Add(new XElement("IsChecked", ischecked));
            xele.Add(new XElement("FilterTimeIndex", filtertimeindex));
        }

        public void Load(XElement xele)
        {
            try { ischecked = bool.Parse(xele.Element("IsChecked").Value); } catch (Exception) { }
            try { filtertimeindex = int.Parse(xele.Element("FilterTimeIndex").Value); } catch (Exception) { }
        }
            
        public IParams Clone()
        {
            return Clone(null);
        }

        public FilterParams Clone(ProjectPropertyParams parent)
        {
            FilterParams that = new FilterParams(parent);
            that.Load(this);
            return that;
        }
        public bool LoadSuccess { get; set; }
        public void Load(IParams iparams)
        {
            if (iparams is FilterParams)
            {
                FilterParams that = (FilterParams)iparams;
                this.IsChecked = that.IsChecked;
                this.FilterTimeIndex = that.FilterTimeIndex;
            }
            LoadSuccess = true;
        }

        #endregion


    }
}

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
    public class ExpansionModuleParams : IParams
    {
        public ExpansionModuleParams(ProjectPropertyParams _parent)
        {
            parent = _parent;
            _useExpansionModule = false;
            for (int i = 0; i < 8; i++)
            {
                ExpansionUnitParams.Add(new ExpansionUnitModuleParams(this,i + 1));
            }
        }
        
        public void Dispose()
        {
            parent = null;
            PropertyChanged = null;
            foreach (var param in ExpansionUnitParams)
            {
                param.Dispose();
            }
            ExpansionUnitParams.Clear();
            ExpansionUnitParams = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ProjectPropertyParams parent;
        public ProjectPropertyParams Parent { get { return this.parent; } }

        public List<ExpansionUnitModuleParams> ExpansionUnitParams = new List<ExpansionUnitModuleParams>();

        private bool _useExpansionModule;
        public bool UseExpansionModule
        {
            get
            {
                return _useExpansionModule;
            }
            set
            {
                _useExpansionModule = value;
                PropertyChanged(this,new PropertyChangedEventArgs("UseExpansionModule"));
            }
        }
        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            xele.SetAttributeValue("CanUseExpansion", _useExpansionModule);
            foreach (var param in ExpansionUnitParams)
            {
                XElement sub = new XElement(string.Format("ExpansionUnitParams{0}", param.ID));
                param.Save(sub);
                xele.Add(sub);
            }
        }

        public void Load(XElement xele)
        {
            try { UseExpansionModule = bool.Parse(xele.Attribute("CanUseExpansion").Value); } catch (Exception) { }
            foreach (var param in ExpansionUnitParams)
            {
                try { param.Load(xele.Element(string.Format("ExpansionUnitParams{0}", param.ID))); } catch (Exception) { }
            }
        }

        public IParams Clone()
        {
            return Clone(null);
        }

        public ExpansionModuleParams Clone(ProjectPropertyParams parent)
        {
            ExpansionModuleParams that = new ExpansionModuleParams(parent);
            that.Load(this);
            return that;
        }
        public void Load(IParams iparams)
        {
            if (iparams is ExpansionModuleParams)
            {
                ExpansionModuleParams that = (ExpansionModuleParams)iparams;
                this.UseExpansionModule = that.UseExpansionModule;
                for (int i = 0; i < ExpansionUnitParams.Count; i++)
                {
                    ExpansionUnitParams[i].Load(that.ExpansionUnitParams[i]);
                }
            }
        }

        public bool CheckParams()
        {
            return true;
        }
        #endregion

    }
}

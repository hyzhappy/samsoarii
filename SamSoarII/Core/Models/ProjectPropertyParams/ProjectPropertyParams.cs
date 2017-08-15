using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class ProjectPropertyParams : IParams
    {
        public ProjectPropertyParams(ProjectModel _parent)
        {
            parent = _parent;
            paraCom232 = new CommunicationInterfaceParams(this, "232");
            paraCom485 = new CommunicationInterfaceParams(this, "485");
            paraPassword = new PasswordParams(this);
            paraFilter = new FilterParams(this);
            paraHolding = new HoldingSectionParams(this);
            paraAnalog = new AnalogQuantityParams(this);
            paraExpansion = new ExpansionModuleParams(this);
            paraCom = new CommunicationParams(this);
            paraUsb = new USBCommunicationParams(this);
            paraCom232.PropertyChanged += OnChildrenPropertyChanged;
            paraCom485.PropertyChanged += OnChildrenPropertyChanged;
            paraPassword.PropertyChanged += OnChildrenPropertyChanged;
            paraFilter.PropertyChanged += OnChildrenPropertyChanged;
            paraHolding.PropertyChanged += OnChildrenPropertyChanged;
            paraAnalog.PropertyChanged += OnChildrenPropertyChanged;
            paraExpansion.PropertyChanged += OnChildrenPropertyChanged;
            paraCom.PropertyChanged += OnChildrenPropertyChanged;
            paraUsb.PropertyChanged += OnChildrenPropertyChanged;
        }
        
        public void Dispose()
        {
            parent = null;
            PropertyChanged = null;
            paraCom232.Dispose();
            paraCom485.Dispose();
            paraPassword.Dispose();
            paraFilter.Dispose();
            paraHolding.Dispose();
            paraAnalog.Dispose();
            paraExpansion.Dispose();
            paraCom.Dispose();
            paraCom232.PropertyChanged -= OnChildrenPropertyChanged;
            paraCom485.PropertyChanged -= OnChildrenPropertyChanged;
            paraPassword.PropertyChanged -= OnChildrenPropertyChanged;
            paraFilter.PropertyChanged -= OnChildrenPropertyChanged;
            paraHolding.PropertyChanged -= OnChildrenPropertyChanged;
            paraAnalog.PropertyChanged -= OnChildrenPropertyChanged;
            paraExpansion.PropertyChanged -= OnChildrenPropertyChanged;
            paraCom.PropertyChanged -= OnChildrenPropertyChanged;
            paraUsb.PropertyChanged -= OnChildrenPropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private ProjectModel parent;
        public ProjectModel Parent { get { return this.parent; } }

        private CommunicationInterfaceParams paraCom232;
        public CommunicationInterfaceParams PARACom232 { get { return this.paraCom232; } }

        private int stationnumber;
        public int StationNumber
        {
            get { return this.stationnumber; }
            set { this.stationnumber = value; PropertyChanged(this, new PropertyChangedEventArgs("StationNumber")); }
        }
        private CommunicationInterfaceParams paraCom485;
        public CommunicationInterfaceParams PARACom485 { get { return this.paraCom485; } }

        private USBCommunicationParams paraUsb;
        public USBCommunicationParams PARAUsb { get { return paraUsb; } }

        private PasswordParams paraPassword;
        public PasswordParams PARAPassword { get { return this.paraPassword; } }

        private FilterParams paraFilter;
        public FilterParams PARAFilter { get { return this.paraFilter; } }

        private HoldingSectionParams paraHolding;
        public HoldingSectionParams PARAHolding { get { return this.paraHolding; } }

        private AnalogQuantityParams paraAnalog;
        public AnalogQuantityParams PARAAnalog { get { return this.paraAnalog; } }

        private ExpansionModuleParams paraExpansion;
        public ExpansionModuleParams PARAExpansion { get { return this.paraExpansion; } }

        private CommunicationParams paraCom;
        public CommunicationParams PARACom { get { return this.paraCom; } }
        
        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            XElement xele_c = null;
            xele_c = new XElement("CommunicationInterfaceParams232");
            paraCom232.Save(xele_c);
            xele.Add(xele_c);
            xele_c = new XElement("CommunicationInterfaceParams485");
            paraCom485.Save(xele_c);
            xele.Add(xele_c);
            xele_c = new XElement("USBCommunicationParams");
            paraUsb.Save(xele_c);
            xele.Add(xele_c);
            xele_c = new XElement("PasswordParams");
            paraPassword.Save(xele_c);
            xele.Add(xele_c);
            xele_c = new XElement("FilterParams");
            paraFilter.Save(xele_c);
            xele.Add(xele_c);
            xele_c = new XElement("HoldingSectionParams");
            paraHolding.Save(xele_c);
            xele.Add(xele_c);
            xele_c = new XElement("AnalogQuantityParams");
            paraAnalog.Save(xele_c);
            xele.Add(xele_c);
            xele_c = new XElement("ExpansionModuleParams");
            paraExpansion.Save(xele_c);
            xele.Add(xele_c);
            xele_c = new XElement("CommunicationParams");
            paraCom.Save(xele_c);
            xele.Add(xele_c);
        }

        public void Load(XElement xele)
        {
            if (xele == null) return;
            XElement xeleParams;
            xeleParams = xele.Element("CommunicationInterfaceParams232");
            if(xeleParams != null)
                paraCom232.Load(xeleParams);
            xeleParams = xele.Element("CommunicationInterfaceParams485");
            if (xeleParams != null)
                paraCom232.Load(xeleParams);
            xeleParams = xele.Element("USBCommunicationParams");
            if (xeleParams != null)
                paraCom232.Load(xeleParams);
            xeleParams = xele.Element("PasswordParams");
            if (xeleParams != null)
                paraCom232.Load(xeleParams);
            xeleParams = xele.Element("FilterParams");
            if (xeleParams != null)
                paraCom232.Load(xeleParams);
            xeleParams = xele.Element("HoldingSectionParams");
            if (xeleParams != null)
                paraCom232.Load(xeleParams);
            xeleParams = xele.Element("AnalogQuantityParams");
            if (xeleParams != null)
                paraCom232.Load(xeleParams);
            xeleParams = xele.Element("ExpansionModuleParams");
            if (xeleParams != null)
                paraCom232.Load(xeleParams);
            xeleParams = xele.Element("CommunicationParams");
            if (xeleParams != null)
                paraCom232.Load(xeleParams);
        }

        public IParams Clone()
        {
            return Clone(null);
        }

        public ProjectPropertyParams Clone(ProjectModel _parent)
        {
            ProjectPropertyParams that = new ProjectPropertyParams(_parent);
            that.Load(this);
            return that;
        }
        public bool LoadSuccess
        {
            get
            {
                return PARACom232.LoadSuccess && PARACom485.LoadSuccess && 
                    PARAPassword.LoadSuccess && PARAFilter.LoadSuccess &&
                    PARAHolding.LoadSuccess && PARAAnalog.LoadSuccess &&
                    PARAExpansion.LoadSuccess && PARACom.LoadSuccess && PARAUsb.LoadSuccess;
            }
            set
            {
            }
        }
        public void Load(IParams iparams)
        {
            if (iparams is ProjectPropertyParams)
            {
                ProjectPropertyParams that = (ProjectPropertyParams)iparams;
                this.PARACom232.Load(that.PARACom232);
                this.PARACom485.Load(that.PARACom485);
                this.PARAUsb.Load(that.PARAUsb);
                this.PARAPassword.Load(that.PARAPassword);
                this.PARAFilter.Load(that.PARAFilter);
                this.PARAHolding.Load(that.PARAHolding);
                this.PARAAnalog.Load(that.PARAAnalog);
                this.PARAExpansion.Load(that.PARAExpansion);
                this.PARACom.Load(that.PARACom);
            }
        }

        #endregion

        #region Event Handler
        
        private void OnChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Parent?.InvokeModify(this);
            PropertyChanged(sender, e);
        }

        #endregion
    }
}

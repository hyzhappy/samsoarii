using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class AnalogQuantityParams : IParams
    {
        public AnalogQuantityParams(ProjectPropertyParams _parent)
        {
            parent = _parent;
            isused = false;
            inputpassindex = 0;
            inputmodeindex = 0;
            samplingtimesindex = 0;
            samplingvalue = 1000;
            inputstartrange = 0;
            inputendrange = 65535;
            outputpassindex = 0;
            outputmodeindex = 0;
            outputstartrange = 0;
            outputendrange = 65535;
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

        private int inputpassindex;
        public int InputPassIndex
        {
            get { return this.inputpassindex; }
            set { this.inputpassindex = value; PropertyChanged(this, new PropertyChangedEventArgs("InputPassIndex")); }
        }

        private int inputmodeindex;
        public int InputModeIndex
        {
            get { return this.inputmodeindex; }
            set { this.inputmodeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("InputModeIndex")); }
        }

        private int samplingtimesindex;
        public int SamplineTimesIndex
        {
            get { return this.samplingtimesindex; }
            set { this.samplingtimesindex = value; PropertyChanged(this, new PropertyChangedEventArgs("SamplingTimesIndex")); }
        }
        
        private int samplingvalue;
        public int SamplingValue
        {
            get { return this.samplingvalue; }
            set { this.samplingvalue = value; PropertyChanged(this, new PropertyChangedEventArgs("SamplingValue")); }
        }

        private int inputstartrange;
        public int InputStartRange
        {
            get { return this.inputstartrange; }
            set { this.inputstartrange = value; PropertyChanged(this, new PropertyChangedEventArgs("InputStartRange")); }
        }
        
        private int inputendrange;
        public int InputEndRange
        {
            get { return this.inputendrange; }
            set { this.inputendrange = value; PropertyChanged(this, new PropertyChangedEventArgs("InputEndRange")); }
        }
    
        private int outputpassindex;
        public int OutputPassIndex
        {
            get { return this.outputpassindex; }
            set { this.outputpassindex = value; PropertyChanged(this, new PropertyChangedEventArgs("OutputPassIndex")); }
        }

        private int outputmodeindex;
        public int OutputModeIndex
        {
            get { return this.outputmodeindex; }
            set { this.outputmodeindex = value; PropertyChanged(this, new PropertyChangedEventArgs("OutputModeIndex")); }
        }
        private int outputstartrange;
        public int OutputStartRange
        {
            get { return this.outputstartrange; }
            set { this.outputstartrange = value; PropertyChanged(this, new PropertyChangedEventArgs("OutputStartRange")); }
        }

        private int outputendrange;
        public int OutputEndRange
        {
            get { return this.outputendrange; }
            set { this.outputendrange = value; PropertyChanged(this, new PropertyChangedEventArgs("OutputEndRange")); }
        }

        private bool isused;
        public bool IsUsed
        {
            get { return this.isused; }
            set { this.isused = value; PropertyChanged(this, new PropertyChangedEventArgs("IsUsed")); }
        }

        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            xele.Add(new XElement("InputPassIndex", inputpassindex));
            xele.Add(new XElement("InputModeIndex", inputmodeindex));
            xele.Add(new XElement("SamplineTimesIndex", samplingtimesindex));
            xele.Add(new XElement("SamplingValue", samplingvalue));
            xele.Add(new XElement("InputStartRange", inputstartrange));
            xele.Add(new XElement("InputEndRange", inputendrange));
            xele.Add(new XElement("OutputPassIndex", outputpassindex));
            xele.Add(new XElement("OutputModeIndex", outputmodeindex));
            xele.Add(new XElement("OutputStartRange", outputstartrange));
            xele.Add(new XElement("OutputEndRange", outputendrange));
            xele.Add(new XElement("IsUsed", isused));
        }

        public void Load(XElement xele)
        {
            inputpassindex = int.Parse(xele.Element("InputPassIndex").Value);
            inputmodeindex = int.Parse(xele.Element("InputModeIndex").Value);
            samplingtimesindex = int.Parse(xele.Element("SamplineTimesIndex").Value);
            samplingvalue = int.Parse(xele.Element("SamplingValue").Value);
            inputstartrange = int.Parse(xele.Element("InputStartRange").Value);
            inputendrange = int.Parse(xele.Element("InputEndRange").Value);
            outputpassindex = int.Parse(xele.Element("OutputPassIndex").Value);
            outputmodeindex = int.Parse(xele.Element("OutputModeIndex").Value);
            outputstartrange = int.Parse(xele.Element("OutputStartRange").Value);
            outputendrange = int.Parse(xele.Element("OutputEndRange").Value);
            isused = bool.Parse(xele.Element("IsUsed").Value);
        }

        public IParams Clone()
        {
            return Clone(null);
        }

        public AnalogQuantityParams Clone(ProjectPropertyParams parent)
        {
            AnalogQuantityParams that = new AnalogQuantityParams(parent);
            that.Load(this);
            return that;
        }

        public void Load(IParams iparams)
        {
            if (iparams is AnalogQuantityParams)
            {
                AnalogQuantityParams that = (AnalogQuantityParams)iparams;
                this.InputPassIndex = that.InputPassIndex;
                this.InputModeIndex = that.InputModeIndex;
                this.SamplineTimesIndex = that.SamplineTimesIndex;
                this.SamplingValue = that.SamplingValue;
                this.InputStartRange = that.InputStartRange;
                this.InputEndRange = that.InputEndRange;
                this.OutputPassIndex = that.OutputPassIndex;
                this.OutputModeIndex = that.OutputModeIndex;
                this.OutputStartRange = that.OutputStartRange;
                this.OutputEndRange = that.OutputEndRange;
                this.IsUsed = that.IsUsed;
            }
        }

        #endregion

    }
}

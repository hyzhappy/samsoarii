using SamSoarII.Shell.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class ExpansionUnitModuleParams : IParams
    {
        public ExpansionUnitModuleParams(ExpansionModuleParams _parent,int id)
        {
            parent = _parent;
            ID = id;
            _moduleTypeIndex = 0;
            _IP_Channel_Index = 0;
            _OP_Channel_Index = 0;
            _useModule = false;
            IP_Channel_CB_Enabled1 = false;
            IP_Channel_CB_Enabled2 = false;
            IP_Channel_CB_Enabled3 = false;
            IP_Channel_CB_Enabled4 = false;
            OP_Channel_CB_Enabled1 = false;
            OP_Channel_CB_Enabled2 = false;
            IP_Mode_Index1 = 0;
            IP_Mode_Index2 = 0;
            IP_Mode_Index3 = 0;
            IP_Mode_Index4 = 0;
            OP_Mode_Index1 = 0;
            OP_Mode_Index2 = 0;
            IP_SampleTime_Index1 = 0;
            IP_SampleTime_Index2 = 0;
            IP_SampleTime_Index3 = 0;
            IP_SampleTime_Index4 = 0;
            SampleValue1 = "1000";
            SampleValue2 = "1000";
            SampleValue3 = "1000";
            SampleValue4 = "1000";
            IP_StartRange1 = 0;
            IP_EndRange1 = 65535;
            IP_StartRange2 = 0;
            IP_EndRange2 = 65535;
            IP_StartRange3 = 0;
            IP_EndRange3 = 65535;
            IP_StartRange4 = 0;
            IP_EndRange4 = 65535;
            OP_StartRange1 = 0;
            OP_EndRange1 = 65535;
            OP_StartRange2 = 0;
            OP_EndRange2 = 65535;
            _filterTime_Index = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number
        public int ID;
        private int _moduleTypeIndex;
        public int ModuleTypeIndex
        {
            get
            {
                return _moduleTypeIndex;
            }
            set
            {
                _moduleTypeIndex = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ModuleTypeIndex"));
                PropertyChanged(this, new PropertyChangedEventArgs("ShowName"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Channel"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Mode"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Channel_Index"));
            }
        }
        public ModuleType ModuleType
        {
            get
            {
                switch (_moduleTypeIndex)
                {
                    case 0: return ModuleType.FGs_E4AI;
                    case 1: return ModuleType.FGs_E8R;
                    case 2: return ModuleType.FGs_E8T;
                    case 3: return ModuleType.FGs_E8X;
                    case 4: return ModuleType.FGs_E8X8T;
                    case 5: return ModuleType.FGs_E16R;
                    case 6: return ModuleType.FGs_E16T;
                    case 7: return ModuleType.FGs_E2AO;
                    case 8: return ModuleType.FGs_E4AI2AO;
                    case 9: return ModuleType.FGs_E4TC;
                    case 10: return ModuleType.FGs_E8X8R;
                    case 11: return ModuleType.FGs_E16X;
                    case 12: return ModuleType.FGs_E16X16T;
                    case 13: return ModuleType.FGs_E16X16R;
                    default:
                        return ModuleType.FGs_E4AI;
                }
            }
        }
        private bool _useModule;
        public bool UseModule
        {
            get
            {
                return _useModule;
            }
            set
            {
                _useModule = value;
                PropertyChanged(this, new PropertyChangedEventArgs("UseModule"));
                PropertyChanged(this, new PropertyChangedEventArgs("ShowName"));
            }
        }
        public string ShowName
        {
            get
            {
                if (_useModule)
                {
                    return string.Format("#{0} {1}", ID, ModuleType);
                }
                return string.Format("#{0} {1}", ID, Properties.Resources.Not_Enabled);
            }
        }
        public string[] IP_Mode
        {
            get
            {
                switch (ModuleType)
                {
                    case ModuleType.FGs_E4AI:
                    case ModuleType.FGs_E4AI2AO:
                        return new string[] { "4-20mA", "0-5V", "0-10V" };
                    case ModuleType.FGs_E4TC:
                        return new string[] { Properties.Resources.Thermocouple, "PT100" };
                    case ModuleType.FGs_E8R:
                    case ModuleType.FGs_E8T:
                    case ModuleType.FGs_E8X:
                    case ModuleType.FGs_E8X8T:
                    case ModuleType.FGs_E16R:
                    case ModuleType.FGs_E16T:
                    case ModuleType.FGs_E2AO:
                    case ModuleType.FGs_E8X8R:
                    case ModuleType.FGs_E16X:
                    case ModuleType.FGs_E16X16T:
                    case ModuleType.FGs_E16X16R:
                    default:
                        return new string[] { };
                }
            }
        }
        public int IP_Mode_Index
        {
            get
            {
                switch (_IP_Channel_Index)
                {
                    case 0: return IP_Mode_Index1;
                    case 1: return IP_Mode_Index2;
                    case 2: return IP_Mode_Index3;
                    case 3: return IP_Mode_Index4;
                    default:
                        return 0;
                }
            }
            set
            {
                if (value < 0) return;
                switch (_IP_Channel_Index)
                {
                    case 0:
                        IP_Mode_Index1 = value;
                        break;
                    case 1:
                        IP_Mode_Index2 = value;
                        break;
                    case 2:
                        IP_Mode_Index3 = value;
                        break;
                    case 3:
                        IP_Mode_Index4 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Mode_Index"));
            }
        }
        private int IP_Mode_Index1;
        private int IP_Mode_Index2;
        private int IP_Mode_Index3;
        private int IP_Mode_Index4;
        public string[] IP_Channel
        {
            get
            {
                return new string[] { "AI0", "AI1", "AI2", "AI3" };
            }
        }
        private int _IP_Channel_Index;
        public int IP_Channel_Index
        {
            get
            {
                return _IP_Channel_Index;
            }
            set
            {
                if(value >= 0)
                    _IP_Channel_Index = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Channel_Index"));
                PropertyChanged(this,new PropertyChangedEventArgs("IP_Channel_CB_Enabled"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Mode_Index"));
                PropertyChanged(this, new PropertyChangedEventArgs("SampleValue"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_SampleTime_Index"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_StartRange"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_EndRange"));
            }
        }
        public bool IP_Channel_CB_Enabled
        {
            get
            {
                switch (_IP_Channel_Index)
                {
                    case 0: return IP_Channel_CB_Enabled1;
                    case 1: return IP_Channel_CB_Enabled2;
                    case 2: return IP_Channel_CB_Enabled3;
                    case 3: return IP_Channel_CB_Enabled4;
                    default:
                        return false;
                }
            }
            set
            {
                switch (_IP_Channel_Index)
                {
                    case 0:
                        IP_Channel_CB_Enabled1 = value;
                        break;
                    case 1:
                        IP_Channel_CB_Enabled2 = value;
                        break;
                    case 2:
                        IP_Channel_CB_Enabled3 = value;
                        break;
                    case 3:
                        IP_Channel_CB_Enabled4 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Channel_CB_Enabled"));
            }
        }
        private bool IP_Channel_CB_Enabled1;
        private bool IP_Channel_CB_Enabled2;
        private bool IP_Channel_CB_Enabled3;
        private bool IP_Channel_CB_Enabled4;

        public string[] IP_SampleTime
        {
            get
            {
                return new string[] {"4","8","16","32" };
            }
        }

        public int IP_SampleTime_Index
        {
            get
            {
                switch (_IP_Channel_Index)
                {
                    case 0: return IP_SampleTime_Index1;
                    case 1: return IP_SampleTime_Index2;
                    case 2: return IP_SampleTime_Index3;
                    case 3: return IP_SampleTime_Index4;
                    default:
                        return 0;
                }
            }
            set
            {
                if (value < 0) return;
                switch (_IP_Channel_Index)
                {
                    case 0:
                        IP_SampleTime_Index1 = value;
                        break;
                    case 1:
                        IP_SampleTime_Index2 = value;
                        break;
                    case 2:
                        IP_SampleTime_Index3 = value;
                        break;
                    case 3:
                        IP_SampleTime_Index4 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("IP_SampleTime_Index"));
            }
        }
        private int IP_SampleTime_Index1;
        private int IP_SampleTime_Index2;
        private int IP_SampleTime_Index3;
        private int IP_SampleTime_Index4;

        private string SampleValue1;
        private string SampleValue2;
        private string SampleValue3;
        private string SampleValue4;
        public string SampleValue
        {
            get
            {
                switch (_IP_Channel_Index)
                {
                    case 0: return SampleValue1;
                    case 1: return SampleValue2;
                    case 2: return SampleValue3;
                    case 3: return SampleValue4;
                    default:
                        return 1000.ToString();
                }
            }
            set
            {
                switch (_IP_Channel_Index)
                {
                    case 0:
                        SampleValue1 = value;
                        break;
                    case 1:
                        SampleValue2 = value;
                        break;
                    case 2:
                        SampleValue3 = value;
                        break;
                    case 3:
                        SampleValue4 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("SampleValue"));
            }
        }
        private int IP_StartRange1;
        private int IP_StartRange2;
        private int IP_StartRange3;
        private int IP_StartRange4;
        public int IP_StartRange
        {
            get
            {
                switch (_IP_Channel_Index)
                {
                    case 0: return IP_StartRange1;
                    case 1: return IP_StartRange2;
                    case 2: return IP_StartRange3;
                    case 3: return IP_StartRange4;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (_IP_Channel_Index)
                {
                    case 0:
                        IP_StartRange1 = value;
                        break;
                    case 1:
                        IP_StartRange2 = value;
                        break;
                    case 2:
                        IP_StartRange3 = value;
                        break;
                    case 3:
                        IP_StartRange4 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("IP_StartRange"));
            }
        }
        private int IP_EndRange1;
        private int IP_EndRange2;
        private int IP_EndRange3;
        private int IP_EndRange4;
        public int IP_EndRange
        {
            get
            {
                switch (_IP_Channel_Index)
                {
                    case 0: return IP_EndRange1;
                    case 1: return IP_EndRange2;
                    case 2: return IP_EndRange3;
                    case 3: return IP_EndRange4;
                    default:
                        return 65535;
                }
            }
            set
            {
                switch (_IP_Channel_Index)
                {
                    case 0:
                        IP_EndRange1 = value;
                        break;
                    case 1:
                        IP_EndRange2 = value;
                        break;
                    case 2:
                        IP_EndRange3 = value;
                        break;
                    case 3:
                        IP_EndRange4 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("IP_EndRange"));
            }
        }


        public string[] OP_Channel
        {
            get
            {
                return new string[] { "AO0","AO1"};
            }
        }

        private int _OP_Channel_Index;
        public int OP_Channel_Index
        {
            get
            {
                return _OP_Channel_Index;
            }
            set
            {
                if (value >= 0)
                    _OP_Channel_Index = value;
                PropertyChanged(this, new PropertyChangedEventArgs("OP_Channel_Index"));
                PropertyChanged(this, new PropertyChangedEventArgs("OP_Channel_CB_Enabled"));
                PropertyChanged(this, new PropertyChangedEventArgs("OP_Mode_Index"));
                PropertyChanged(this, new PropertyChangedEventArgs("OP_StartRange"));
                PropertyChanged(this, new PropertyChangedEventArgs("OP_EndRange"));
            }
        }

        public bool OP_Channel_CB_Enabled
        {
            get
            {
                switch (_OP_Channel_Index)
                {
                    case 0: return OP_Channel_CB_Enabled1;
                    case 1: return OP_Channel_CB_Enabled2;
                    default:
                        return false;
                }
            }
            set
            {
                switch (_OP_Channel_Index)
                {
                    case 0:
                        OP_Channel_CB_Enabled1 = value;
                        break;
                    case 1:
                        OP_Channel_CB_Enabled2 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("OP_Channel_CB_Enabled"));
            }
        }
        private bool OP_Channel_CB_Enabled1;
        private bool OP_Channel_CB_Enabled2;

        public string[] OP_Mode
        {
            get
            {
                return new string[] { "4-20mA", "0-5V", "0-10V" };
            }
        }

        public int OP_Mode_Index
        {
            get
            {
                switch (_OP_Channel_Index)
                {
                    case 0: return OP_Mode_Index1;
                    case 1: return OP_Mode_Index2;
                    default:
                        return 0;
                }
            }
            set
            {
                if (value < 0) return;
                switch (_OP_Channel_Index)
                {
                    case 0:
                        OP_Mode_Index1 = value;
                        break;
                    case 1:
                        OP_Mode_Index2 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("OP_Mode_Index"));
            }
        }
        private int OP_Mode_Index1;
        private int OP_Mode_Index2;

        private int OP_StartRange1;
        private int OP_StartRange2;
        public int OP_StartRange
        {
            get
            {
                switch (_OP_Channel_Index)
                {
                    case 0: return OP_StartRange1;
                    case 1: return OP_StartRange2;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (_OP_Channel_Index)
                {
                    case 0:
                        OP_StartRange1 = value;
                        break;
                    case 1:
                        OP_StartRange2 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("OP_StartRange"));
            }
        }
        private int OP_EndRange1;
        private int OP_EndRange2;
        public int OP_EndRange
        {
            get
            {
                switch (_OP_Channel_Index)
                {
                    case 0: return OP_EndRange1;
                    case 1: return OP_EndRange2;
                    default:
                        return 65535;
                }
            }
            set
            {
                switch (_OP_Channel_Index)
                {
                    case 0:
                        OP_EndRange1 = value;
                        break;
                    case 1:
                        OP_EndRange2 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("OP_EndRange"));
            }
        }

        public string[] FT_CB
        {
            get
            {
                return new string[] {"0.00","0.20","0.40","0.80","1.60","3.20","6.40","12.80","25.60","51.20" };
            }
        }
        private int _filterTime_Index;
        public int FilterTime_Index
        {
            get
            {
                return _filterTime_Index;
            }
            set
            {
                _filterTime_Index = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FilterTime_Index"));
            }
        }


        private ExpansionModuleParams parent;
        public ExpansionModuleParams Parent { get { return this.parent; } }

        public bool LoadSuccess { get; set; }
        #endregion

        public IParams Clone()
        {
            return Clone(null);
        }
        public ExpansionUnitModuleParams Clone(ExpansionModuleParams parent)
        {
            ExpansionUnitModuleParams that = new ExpansionUnitModuleParams(parent,ID);
            that.Load(this);
            return that;
        }
        public void Dispose()
        {
            parent = null;
            PropertyChanged = null;
        }

        public void Load(IParams iparams)
        {
            if (iparams is ExpansionUnitModuleParams)
            {
                ExpansionUnitModuleParams that = (ExpansionUnitModuleParams)iparams;
                ModuleTypeIndex = that.ModuleTypeIndex;
                IP_Channel_Index = that.IP_Channel_Index;
                OP_Channel_Index = that.OP_Channel_Index;
                UseModule = that.UseModule;
                IP_Channel_CB_Enabled1 = that.IP_Channel_CB_Enabled1;
                IP_Channel_CB_Enabled2 = that.IP_Channel_CB_Enabled2;
                IP_Channel_CB_Enabled3 = that.IP_Channel_CB_Enabled3;
                IP_Channel_CB_Enabled4 = that.IP_Channel_CB_Enabled4;
                OP_Channel_CB_Enabled1 = that.OP_Channel_CB_Enabled1;
                OP_Channel_CB_Enabled2 = that.OP_Channel_CB_Enabled2;
                IP_Mode_Index1 = that.IP_Mode_Index1;
                IP_Mode_Index2 = that.IP_Mode_Index2;
                IP_Mode_Index3 = that.IP_Mode_Index3;
                IP_Mode_Index4 = that.IP_Mode_Index4;
                OP_Mode_Index1 = that.OP_Mode_Index1;
                OP_Mode_Index2 = that.OP_Mode_Index2;
                IP_SampleTime_Index1 = that.IP_SampleTime_Index1;
                IP_SampleTime_Index2 = that.IP_SampleTime_Index2;
                IP_SampleTime_Index3 = that.IP_SampleTime_Index3;
                IP_SampleTime_Index4 = that.IP_SampleTime_Index4;
                SampleValue1 = that.SampleValue1;
                SampleValue2 = that.SampleValue2;
                SampleValue3 = that.SampleValue3;
                SampleValue4 = that.SampleValue4;
                if (that.IP_StartRange1 > that.IP_EndRange1 || that.IP_StartRange2 > that.IP_EndRange2
                    || that.IP_StartRange3 > that.IP_EndRange3 || that.IP_StartRange4 > that.IP_EndRange4)
                {
                    LoadSuccess = false;
                    LocalizedMessageBox.Show(Properties.Resources.Range_Error, LocalizedMessageIcon.Error);
                    return;
                }
                IP_StartRange1 = that.IP_StartRange1;
                IP_EndRange1 = that.IP_EndRange1;
                IP_StartRange2 = that.IP_StartRange2;
                IP_EndRange2 = that.IP_EndRange2;
                IP_StartRange3 = that.IP_StartRange3;
                IP_EndRange3 = that.IP_EndRange3;
                IP_StartRange4 = that.IP_StartRange4;
                IP_EndRange4 = that.IP_EndRange4;

                if (that.OP_StartRange1 > that.OP_EndRange1 || that.OP_StartRange2 > that.OP_EndRange2)
                {
                    LoadSuccess = false;
                    LocalizedMessageBox.Show(Properties.Resources.Range_Error, LocalizedMessageIcon.Error);
                    return;
                }
                OP_StartRange1 = that.OP_StartRange1;
                OP_EndRange1 = that.OP_EndRange1;
                OP_StartRange2 = that.OP_StartRange2;
                OP_EndRange2 = that.OP_EndRange2;
                FilterTime_Index = that.FilterTime_Index;
            }
            LoadSuccess = true;
        }

        public void Load(XElement xele)
        {
            try
            {
                ModuleTypeIndex = int.Parse(xele.Element("ModuleTypeIndex").Value);
                UseModule = bool.Parse(xele.Element("UseModule").Value);
                XElement inputele = xele.Element("Input");
                XElement outputele = xele.Element("Output");
                XElement filtertime = xele.Element("FilterTime");
                try
                {
                    IP_Channel_Index = int.Parse(inputele.Element("IP_Channel_Index").Value);
                    OP_Channel_Index = int.Parse(outputele.Element("OP_Channel_Index").Value);
                }
                catch (Exception)
                {
                    IP_Channel_Index = 0;
                    OP_Channel_Index = 0;
                }
                IP_Channel_CB_Enabled1 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled1").Value);
                IP_Channel_CB_Enabled2 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled2").Value);
                IP_Channel_CB_Enabled3 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled3").Value);
                IP_Channel_CB_Enabled4 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled4").Value);
                OP_Channel_CB_Enabled1 = bool.Parse(outputele.Element("OP_Channel_CB_Enabled1").Value);
                OP_Channel_CB_Enabled2 = bool.Parse(outputele.Element("OP_Channel_CB_Enabled2").Value);
                IP_Mode_Index1 = int.Parse(inputele.Element("IP_Mode_Index1").Value);
                IP_Mode_Index2 = int.Parse(inputele.Element("IP_Mode_Index2").Value);
                IP_Mode_Index3 = int.Parse(inputele.Element("IP_Mode_Index3").Value);
                IP_Mode_Index4 = int.Parse(inputele.Element("IP_Mode_Index4").Value);
                OP_Mode_Index1 = int.Parse(outputele.Element("OP_Mode_Index1").Value);
                OP_Mode_Index2 = int.Parse(outputele.Element("OP_Mode_Index2").Value);
                IP_SampleTime_Index1 = int.Parse(inputele.Element("IP_SampleTime_Index1").Value);
                IP_SampleTime_Index2 = int.Parse(inputele.Element("IP_SampleTime_Index2").Value);
                IP_SampleTime_Index3 = int.Parse(inputele.Element("IP_SampleTime_Index3").Value);
                IP_SampleTime_Index4 = int.Parse(inputele.Element("IP_SampleTime_Index4").Value);
                SampleValue1 = inputele.Element("SampleValue1").Value;
                SampleValue2 = inputele.Element("SampleValue2").Value;
                SampleValue3 = inputele.Element("SampleValue3").Value;
                SampleValue4 = inputele.Element("SampleValue4").Value;
                IP_StartRange1 = int.Parse(inputele.Element("IP_StartRange1").Value);
                IP_EndRange1 = int.Parse(inputele.Element("IP_EndRange1").Value);
                IP_StartRange2 = int.Parse(inputele.Element("IP_StartRange2").Value);
                IP_EndRange2 = int.Parse(inputele.Element("IP_EndRange2").Value);
                IP_StartRange3 = int.Parse(inputele.Element("IP_StartRange3").Value);
                IP_EndRange3 = int.Parse(inputele.Element("IP_EndRange3").Value);
                IP_StartRange4 = int.Parse(inputele.Element("IP_StartRange4").Value);
                IP_EndRange4 = int.Parse(inputele.Element("IP_EndRange4").Value);
                OP_StartRange1 = int.Parse(outputele.Element("OP_StartRange1").Value);
                OP_EndRange1 = int.Parse(outputele.Element("OP_EndRange1").Value);
                OP_StartRange2 = int.Parse(outputele.Element("OP_StartRange2").Value);
                OP_EndRange2 = int.Parse(outputele.Element("OP_EndRange2").Value);
                FilterTime_Index = int.Parse(filtertime.Element("FilterTime_Index").Value);
            }
            catch (Exception)
            {
                
            }
        }

        public void Save(XElement xele)
        {
            xele.Add(new XElement("ModuleTypeIndex", ModuleTypeIndex));
            xele.Add(new XElement("UseModule", UseModule));
            XElement inputele = new XElement("Input");
            inputele.Add(new XElement("IP_Channel_Index", IP_Channel_Index));
            inputele.Add(new XElement("IP_Channel_CB_Enabled1", IP_Channel_CB_Enabled1));
            inputele.Add(new XElement("IP_Channel_CB_Enabled2", IP_Channel_CB_Enabled2));
            inputele.Add(new XElement("IP_Channel_CB_Enabled3", IP_Channel_CB_Enabled3));
            inputele.Add(new XElement("IP_Channel_CB_Enabled4", IP_Channel_CB_Enabled4));
            inputele.Add(new XElement("IP_Mode_Index1", IP_Mode_Index1));
            inputele.Add(new XElement("IP_Mode_Index2", IP_Mode_Index2));
            inputele.Add(new XElement("IP_Mode_Index3", IP_Mode_Index3));
            inputele.Add(new XElement("IP_Mode_Index4", IP_Mode_Index4));
            inputele.Add(new XElement("IP_SampleTime_Index1", IP_SampleTime_Index1));
            inputele.Add(new XElement("IP_SampleTime_Index2", IP_SampleTime_Index2));
            inputele.Add(new XElement("IP_SampleTime_Index3", IP_SampleTime_Index3));
            inputele.Add(new XElement("IP_SampleTime_Index4", IP_SampleTime_Index4));
            inputele.Add(new XElement("SampleValue1", SampleValue1));
            inputele.Add(new XElement("SampleValue2", SampleValue2));
            inputele.Add(new XElement("SampleValue3", SampleValue3));
            inputele.Add(new XElement("SampleValue4", SampleValue4));
            inputele.Add(new XElement("IP_StartRange1", IP_StartRange1));
            inputele.Add(new XElement("IP_StartRange2", IP_StartRange2));
            inputele.Add(new XElement("IP_StartRange3", IP_StartRange3));
            inputele.Add(new XElement("IP_StartRange4", IP_StartRange4));
            inputele.Add(new XElement("IP_EndRange1", IP_EndRange1));
            inputele.Add(new XElement("IP_EndRange2", IP_EndRange2));
            inputele.Add(new XElement("IP_EndRange3", IP_EndRange3));
            inputele.Add(new XElement("IP_EndRange4", IP_EndRange4));
            XElement outputele = new XElement("Output");
            outputele.Add(new XElement("OP_Channel_Index", OP_Channel_Index));
            outputele.Add(new XElement("OP_Channel_CB_Enabled1", OP_Channel_CB_Enabled1));
            outputele.Add(new XElement("OP_Channel_CB_Enabled2", OP_Channel_CB_Enabled2));
            outputele.Add(new XElement("OP_Mode_Index1", OP_Mode_Index1));
            outputele.Add(new XElement("OP_Mode_Index2", OP_Mode_Index2));
            outputele.Add(new XElement("OP_StartRange1", OP_StartRange1));
            outputele.Add(new XElement("OP_StartRange2", OP_StartRange2));
            outputele.Add(new XElement("OP_EndRange1", OP_EndRange1));
            outputele.Add(new XElement("OP_EndRange2", OP_EndRange2));
            XElement filtertime = new XElement("FilterTime");
            filtertime.Add(new XElement("FilterTime_Index", FilterTime_Index));
            xele.Add(inputele);
            xele.Add(outputele);
            xele.Add(filtertime);
        }
    }
}

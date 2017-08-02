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
            _IP_Channel_Index = 0;
            _OP_Channel_Index = 0;
            IP_Channel_CB_Enabled1 = false;
            IP_Channel_CB_Enabled2 = false;
            IP_Channel_CB_Enabled3 = false;
            IP_Channel_CB_Enabled4 = false;
            IP_Channel_CB_Enabled5 = false;
            IP_Channel_CB_Enabled6 = false;
            IP_Channel_CB_Enabled7 = false;
            IP_Channel_CB_Enabled8 = false;
            OP_Channel_CB_Enabled1 = false;
            OP_Channel_CB_Enabled2 = false;
            OP_Channel_CB_Enabled3 = false;
            OP_Channel_CB_Enabled4 = false;
            IP_Mode_Index1 = 0;
            IP_Mode_Index2 = 0;
            IP_Mode_Index3 = 0;
            IP_Mode_Index4 = 0;
            IP_Mode_Index5 = 0;
            IP_Mode_Index6 = 0;
            IP_Mode_Index7 = 0;
            IP_Mode_Index8 = 0;
            OP_Mode_Index1 = 0;
            OP_Mode_Index2 = 0;
            OP_Mode_Index3 = 0;
            OP_Mode_Index4 = 0;
            IP_SampleTime_Index1 = 0;
            IP_SampleTime_Index2 = 0;
            IP_SampleTime_Index3 = 0;
            IP_SampleTime_Index4 = 0;
            IP_SampleTime_Index5 = 0;
            IP_SampleTime_Index6 = 0;
            IP_SampleTime_Index7 = 0;
            IP_SampleTime_Index8 = 0;
            SampleValue1 = "1000";
            SampleValue2 = "1000";
            SampleValue3 = "1000";
            SampleValue4 = "1000";
            SampleValue5 = "1000";
            SampleValue6 = "1000";
            SampleValue7 = "1000";
            SampleValue8 = "1000";
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
            OP_StartRange3 = 0;
            OP_EndRange3 = 65535;
            OP_StartRange4 = 0;
            OP_EndRange4 = 65535;
        }

        public void Dispose()
        {
            parent = null;
            PropertyChanged = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number
        private int _IP_Channel_Index;
        public int IP_Channel_Index
        {
            get
            {
                return _IP_Channel_Index;
            }
            set
            {
                if (value >= 0)
                    _IP_Channel_Index = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Channel_Index"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Channel_CB_Enabled"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Mode"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Mode_Index"));
                PropertyChanged(this, new PropertyChangedEventArgs("SampleValue"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_SampleTime_Index"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_StartRange"));
                PropertyChanged(this, new PropertyChangedEventArgs("IP_EndRange"));
            }
        }

        public string[] IP_Mode
        {
            get
            {
                switch (IP_Channel_Index)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        return new string[] {"0-5V","4-20mA" };
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        return new string[] { Properties.Resources.Thermocouple, "PT100" };
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
                    case 4: return IP_Mode_Index5;
                    case 5: return IP_Mode_Index6;
                    case 6: return IP_Mode_Index7;
                    case 7: return IP_Mode_Index8;
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
                    case 4:
                        IP_Mode_Index5 = value;
                        break;
                    case 5:
                        IP_Mode_Index6 = value;
                        break;
                    case 6:
                        IP_Mode_Index7 = value;
                        break;
                    case 7:
                        IP_Mode_Index8 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Mode_Index"));
            }
        }
        private int IP_Mode_Index1;
        private int IP_Mode_Index2;
        private int IP_Mode_Index3;
        private int IP_Mode_Index4;
        private int IP_Mode_Index5;
        private int IP_Mode_Index6;
        private int IP_Mode_Index7;
        private int IP_Mode_Index8;

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
                    case 4: return IP_Channel_CB_Enabled5;
                    case 5: return IP_Channel_CB_Enabled6;
                    case 6: return IP_Channel_CB_Enabled7;
                    case 7: return IP_Channel_CB_Enabled8;
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
                    case 4:
                        IP_Channel_CB_Enabled5 = value;
                        break;
                    case 5:
                        IP_Channel_CB_Enabled6 = value;
                        break;
                    case 6:
                        IP_Channel_CB_Enabled7 = value;
                        break;
                    case 7:
                        IP_Channel_CB_Enabled8 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("IP_Channel_CB_Enabled"));
            }
        }
        private bool IP_Channel_CB_Enabled1;
        private bool IP_Channel_CB_Enabled2;
        private bool IP_Channel_CB_Enabled3;
        private bool IP_Channel_CB_Enabled4;
        private bool IP_Channel_CB_Enabled5;
        private bool IP_Channel_CB_Enabled6;
        private bool IP_Channel_CB_Enabled7;
        private bool IP_Channel_CB_Enabled8;

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
                    case 4: return IP_SampleTime_Index5;
                    case 5: return IP_SampleTime_Index6;
                    case 6: return IP_SampleTime_Index7;
                    case 7: return IP_SampleTime_Index8;
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
                    case 4:
                        IP_SampleTime_Index5 = value;
                        break;
                    case 5:
                        IP_SampleTime_Index6 = value;
                        break;
                    case 6:
                        IP_SampleTime_Index7 = value;
                        break;
                    case 7:
                        IP_SampleTime_Index8 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("IP_SampleTime_Index"));
            }
        }
        private int IP_SampleTime_Index1;
        private int IP_SampleTime_Index2;
        private int IP_SampleTime_Index3;
        private int IP_SampleTime_Index4;
        private int IP_SampleTime_Index5;
        private int IP_SampleTime_Index6;
        private int IP_SampleTime_Index7;
        private int IP_SampleTime_Index8;

        private string SampleValue1;
        private string SampleValue2;
        private string SampleValue3;
        private string SampleValue4;
        private string SampleValue5;
        private string SampleValue6;
        private string SampleValue7;
        private string SampleValue8;
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
                    case 4: return SampleValue5;
                    case 5: return SampleValue6;
                    case 6: return SampleValue7;
                    case 7: return SampleValue8;
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
                    case 4:
                        SampleValue5 = value;
                        break;
                    case 5:
                        SampleValue6 = value;
                        break;
                    case 6:
                        SampleValue7 = value;
                        break;
                    case 7:
                        SampleValue8 = value;
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
                    case 2: return OP_Channel_CB_Enabled3;
                    case 3: return OP_Channel_CB_Enabled4;
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
                    case 2:
                        OP_Channel_CB_Enabled3 = value;
                        break;
                    case 3:
                        OP_Channel_CB_Enabled4 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("OP_Channel_CB_Enabled"));
            }
        }
        private bool OP_Channel_CB_Enabled1;
        private bool OP_Channel_CB_Enabled2;
        private bool OP_Channel_CB_Enabled3;
        private bool OP_Channel_CB_Enabled4;

        public int OP_Mode_Index
        {
            get
            {
                switch (_OP_Channel_Index)
                {
                    case 0: return OP_Mode_Index1;
                    case 1: return OP_Mode_Index2;
                    case 2: return OP_Mode_Index3;
                    case 3: return OP_Mode_Index4;
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
                    case 2:
                        OP_Mode_Index3 = value;
                        break;
                    case 3:
                        OP_Mode_Index4 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("OP_Mode_Index"));
            }
        }
        private int OP_Mode_Index1;
        private int OP_Mode_Index2;
        private int OP_Mode_Index3;
        private int OP_Mode_Index4;

        private int OP_StartRange1;
        private int OP_StartRange2;
        private int OP_StartRange3;
        private int OP_StartRange4;
        public int OP_StartRange
        {
            get
            {
                switch (_OP_Channel_Index)
                {
                    case 0: return OP_StartRange1;
                    case 1: return OP_StartRange2;
                    case 2: return OP_StartRange3;
                    case 3: return OP_StartRange4;
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
                    case 2:
                        OP_StartRange3 = value;
                        break;
                    case 3:
                        OP_StartRange4 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("OP_StartRange"));
            }
        }
        private int OP_EndRange1;
        private int OP_EndRange2;
        private int OP_EndRange3;
        private int OP_EndRange4;
        public int OP_EndRange
        {
            get
            {
                switch (_OP_Channel_Index)
                {
                    case 0: return OP_EndRange1;
                    case 1: return OP_EndRange2;
                    case 2: return OP_EndRange3;
                    case 3: return OP_EndRange4;
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
                    case 2:
                        OP_EndRange3 = value;
                        break;
                    case 3:
                        OP_EndRange4 = value;
                        break;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("OP_EndRange"));
            }
        }


        private ProjectPropertyParams parent;
        public ProjectPropertyParams Parent { get { return this.parent; } }

        

        #endregion

        #region Save & Load

        public void Save(XElement xele)
        {
            XElement inputele = new XElement("Input");
            inputele.Add(new XElement("IP_Channel_Index", IP_Channel_Index));
            inputele.Add(new XElement("IP_Channel_CB_Enabled1", IP_Channel_CB_Enabled1));
            inputele.Add(new XElement("IP_Channel_CB_Enabled2", IP_Channel_CB_Enabled2));
            inputele.Add(new XElement("IP_Channel_CB_Enabled3", IP_Channel_CB_Enabled3));
            inputele.Add(new XElement("IP_Channel_CB_Enabled4", IP_Channel_CB_Enabled4));
            inputele.Add(new XElement("IP_Channel_CB_Enabled5", IP_Channel_CB_Enabled5));
            inputele.Add(new XElement("IP_Channel_CB_Enabled6", IP_Channel_CB_Enabled6));
            inputele.Add(new XElement("IP_Channel_CB_Enabled7", IP_Channel_CB_Enabled7));
            inputele.Add(new XElement("IP_Channel_CB_Enabled8", IP_Channel_CB_Enabled8));
            inputele.Add(new XElement("IP_Mode_Index1", IP_Mode_Index1));
            inputele.Add(new XElement("IP_Mode_Index2", IP_Mode_Index2));
            inputele.Add(new XElement("IP_Mode_Index3", IP_Mode_Index3));
            inputele.Add(new XElement("IP_Mode_Index4", IP_Mode_Index4));
            inputele.Add(new XElement("IP_Mode_Index5", IP_Mode_Index5));
            inputele.Add(new XElement("IP_Mode_Index6", IP_Mode_Index6));
            inputele.Add(new XElement("IP_Mode_Index7", IP_Mode_Index7));
            inputele.Add(new XElement("IP_Mode_Index8", IP_Mode_Index8));
            inputele.Add(new XElement("IP_SampleTime_Index1", IP_SampleTime_Index1));
            inputele.Add(new XElement("IP_SampleTime_Index2", IP_SampleTime_Index2));
            inputele.Add(new XElement("IP_SampleTime_Index3", IP_SampleTime_Index3));
            inputele.Add(new XElement("IP_SampleTime_Index4", IP_SampleTime_Index4));
            inputele.Add(new XElement("IP_SampleTime_Index5", IP_SampleTime_Index5));
            inputele.Add(new XElement("IP_SampleTime_Index6", IP_SampleTime_Index6));
            inputele.Add(new XElement("IP_SampleTime_Index7", IP_SampleTime_Index7));
            inputele.Add(new XElement("IP_SampleTime_Index8", IP_SampleTime_Index8));
            inputele.Add(new XElement("SampleValue1", SampleValue1));
            inputele.Add(new XElement("SampleValue2", SampleValue2));
            inputele.Add(new XElement("SampleValue3", SampleValue3));
            inputele.Add(new XElement("SampleValue4", SampleValue4));
            inputele.Add(new XElement("SampleValue5", SampleValue5));
            inputele.Add(new XElement("SampleValue6", SampleValue6));
            inputele.Add(new XElement("SampleValue7", SampleValue7));
            inputele.Add(new XElement("SampleValue8", SampleValue8));
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
            outputele.Add(new XElement("OP_Channel_CB_Enabled3", OP_Channel_CB_Enabled3));
            outputele.Add(new XElement("OP_Channel_CB_Enabled4", OP_Channel_CB_Enabled4));
            outputele.Add(new XElement("OP_Mode_Index1", OP_Mode_Index1));
            outputele.Add(new XElement("OP_Mode_Index2", OP_Mode_Index2));
            outputele.Add(new XElement("OP_Mode_Index3", OP_Mode_Index3));
            outputele.Add(new XElement("OP_Mode_Index4", OP_Mode_Index4));
            outputele.Add(new XElement("OP_StartRange1", OP_StartRange1));
            outputele.Add(new XElement("OP_StartRange2", OP_StartRange2));
            outputele.Add(new XElement("OP_StartRange3", OP_StartRange3));
            outputele.Add(new XElement("OP_StartRange4", OP_StartRange4));
            outputele.Add(new XElement("OP_EndRange1", OP_EndRange1));
            outputele.Add(new XElement("OP_EndRange2", OP_EndRange2));
            outputele.Add(new XElement("OP_EndRange3", OP_EndRange3));
            outputele.Add(new XElement("OP_EndRange4", OP_EndRange4));
            xele.Add(inputele);
            xele.Add(outputele);
        }

        public void Load(XElement xele)
        {
            try
            {
                XElement inputele = xele.Element("Input");
                XElement outputele = xele.Element("Output");
                IP_Channel_Index = int.Parse(inputele.Element("IP_Channel_Index").Value);
                OP_Channel_Index = int.Parse(outputele.Element("OP_Channel_Index").Value);
                IP_Channel_CB_Enabled1 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled1").Value);
                IP_Channel_CB_Enabled2 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled2").Value);
                IP_Channel_CB_Enabled3 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled3").Value);
                IP_Channel_CB_Enabled4 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled4").Value);
                IP_Channel_CB_Enabled5 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled5").Value);
                IP_Channel_CB_Enabled6 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled6").Value);
                IP_Channel_CB_Enabled7 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled7").Value);
                IP_Channel_CB_Enabled8 = bool.Parse(inputele.Element("IP_Channel_CB_Enabled8").Value);
                OP_Channel_CB_Enabled1 = bool.Parse(outputele.Element("OP_Channel_CB_Enabled1").Value);
                OP_Channel_CB_Enabled2 = bool.Parse(outputele.Element("OP_Channel_CB_Enabled2").Value);
                OP_Channel_CB_Enabled3 = bool.Parse(outputele.Element("OP_Channel_CB_Enabled3").Value);
                OP_Channel_CB_Enabled4 = bool.Parse(outputele.Element("OP_Channel_CB_Enabled4").Value);
                IP_Mode_Index1 = int.Parse(inputele.Element("IP_Mode_Index1").Value);
                IP_Mode_Index2 = int.Parse(inputele.Element("IP_Mode_Index2").Value);
                IP_Mode_Index3 = int.Parse(inputele.Element("IP_Mode_Index3").Value);
                IP_Mode_Index4 = int.Parse(inputele.Element("IP_Mode_Index4").Value);
                IP_Mode_Index5 = int.Parse(inputele.Element("IP_Mode_Index5").Value);
                IP_Mode_Index6 = int.Parse(inputele.Element("IP_Mode_Index6").Value);
                IP_Mode_Index7 = int.Parse(inputele.Element("IP_Mode_Index7").Value);
                IP_Mode_Index8 = int.Parse(inputele.Element("IP_Mode_Index8").Value);
                OP_Mode_Index1 = int.Parse(outputele.Element("OP_Mode_Index1").Value);
                OP_Mode_Index2 = int.Parse(outputele.Element("OP_Mode_Index2").Value);
                OP_Mode_Index3 = int.Parse(outputele.Element("OP_Mode_Index3").Value);
                OP_Mode_Index4 = int.Parse(outputele.Element("OP_Mode_Index4").Value);
                IP_SampleTime_Index1 = int.Parse(inputele.Element("IP_SampleTime_Index1").Value);
                IP_SampleTime_Index2 = int.Parse(inputele.Element("IP_SampleTime_Index2").Value);
                IP_SampleTime_Index3 = int.Parse(inputele.Element("IP_SampleTime_Index3").Value);
                IP_SampleTime_Index4 = int.Parse(inputele.Element("IP_SampleTime_Index4").Value);
                IP_SampleTime_Index5 = int.Parse(inputele.Element("IP_SampleTime_Index5").Value);
                IP_SampleTime_Index6 = int.Parse(inputele.Element("IP_SampleTime_Index6").Value);
                IP_SampleTime_Index7 = int.Parse(inputele.Element("IP_SampleTime_Index7").Value);
                IP_SampleTime_Index8 = int.Parse(inputele.Element("IP_SampleTime_Index8").Value);
                SampleValue1 = inputele.Element("SampleValue1").Value;
                SampleValue2 = inputele.Element("SampleValue2").Value;
                SampleValue3 = inputele.Element("SampleValue3").Value;
                SampleValue4 = inputele.Element("SampleValue4").Value;
                SampleValue5 = inputele.Element("SampleValue5").Value;
                SampleValue6 = inputele.Element("SampleValue6").Value;
                SampleValue7 = inputele.Element("SampleValue7").Value;
                SampleValue8 = inputele.Element("SampleValue8").Value;
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
                OP_StartRange3 = int.Parse(outputele.Element("OP_StartRange3").Value);
                OP_EndRange3 = int.Parse(outputele.Element("OP_EndRange3").Value);
                OP_StartRange4 = int.Parse(outputele.Element("OP_StartRange4").Value);
                OP_EndRange4 = int.Parse(outputele.Element("OP_EndRange4").Value);
            }
            catch (Exception)
            {
                
            }
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
                IP_Channel_Index = that.IP_Channel_Index;
                OP_Channel_Index = that.OP_Channel_Index;
                IP_Channel_CB_Enabled1 = that.IP_Channel_CB_Enabled1;
                IP_Channel_CB_Enabled2 = that.IP_Channel_CB_Enabled2;
                IP_Channel_CB_Enabled3 = that.IP_Channel_CB_Enabled3;
                IP_Channel_CB_Enabled4 = that.IP_Channel_CB_Enabled4;
                IP_Channel_CB_Enabled5 = that.IP_Channel_CB_Enabled5;
                IP_Channel_CB_Enabled6 = that.IP_Channel_CB_Enabled6;
                IP_Channel_CB_Enabled7 = that.IP_Channel_CB_Enabled7;
                IP_Channel_CB_Enabled8 = that.IP_Channel_CB_Enabled8;
                OP_Channel_CB_Enabled1 = that.OP_Channel_CB_Enabled1;
                OP_Channel_CB_Enabled2 = that.OP_Channel_CB_Enabled2;
                OP_Channel_CB_Enabled3 = that.OP_Channel_CB_Enabled3;
                OP_Channel_CB_Enabled4 = that.OP_Channel_CB_Enabled4;
                IP_Mode_Index1 = that.IP_Mode_Index1;
                IP_Mode_Index2 = that.IP_Mode_Index2;
                IP_Mode_Index3 = that.IP_Mode_Index3;
                IP_Mode_Index4 = that.IP_Mode_Index4;
                IP_Mode_Index5 = that.IP_Mode_Index5;
                IP_Mode_Index6 = that.IP_Mode_Index6;
                IP_Mode_Index7 = that.IP_Mode_Index7;
                IP_Mode_Index8 = that.IP_Mode_Index8;
                OP_Mode_Index1 = that.OP_Mode_Index1;
                OP_Mode_Index2 = that.OP_Mode_Index2;
                OP_Mode_Index3 = that.OP_Mode_Index3;
                OP_Mode_Index4 = that.OP_Mode_Index4;
                IP_SampleTime_Index1 = that.IP_SampleTime_Index1;
                IP_SampleTime_Index2 = that.IP_SampleTime_Index2;
                IP_SampleTime_Index3 = that.IP_SampleTime_Index3;
                IP_SampleTime_Index4 = that.IP_SampleTime_Index4;
                IP_SampleTime_Index5 = that.IP_SampleTime_Index5;
                IP_SampleTime_Index6 = that.IP_SampleTime_Index6;
                IP_SampleTime_Index7 = that.IP_SampleTime_Index7;
                IP_SampleTime_Index8 = that.IP_SampleTime_Index8;
                SampleValue1 = that.SampleValue1;
                SampleValue2 = that.SampleValue2;
                SampleValue3 = that.SampleValue3;
                SampleValue4 = that.SampleValue4;
                SampleValue5 = that.SampleValue5;
                SampleValue6 = that.SampleValue6;
                SampleValue7 = that.SampleValue7;
                SampleValue8 = that.SampleValue8;
                IP_StartRange1 = that.IP_StartRange1;
                IP_EndRange1 = that.IP_EndRange1;
                IP_StartRange2 = that.IP_StartRange2;
                IP_EndRange2 = that.IP_EndRange2;
                IP_StartRange3 = that.IP_StartRange3;
                IP_EndRange3 = that.IP_EndRange3;
                IP_StartRange4 = that.IP_StartRange4;
                IP_EndRange4 = that.IP_EndRange4;
                OP_StartRange1 = that.OP_StartRange1;
                OP_EndRange1 = that.OP_EndRange1;
                OP_StartRange2 = that.OP_StartRange2;
                OP_EndRange2 = that.OP_EndRange2;
                OP_StartRange3 = that.OP_StartRange3;
                OP_EndRange3 = that.OP_EndRange3;
                OP_StartRange4 = that.OP_StartRange4;
                OP_EndRange4 = that.OP_EndRange4;
            }
        }

        #endregion

    }
}

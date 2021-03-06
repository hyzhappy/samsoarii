﻿using SamSoarII.Utility;
using SamSoarII.LadderInstViewModel.Monitor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Text.RegularExpressions;

namespace SamSoarII.AppMain.UI.Monitor
{
    public class ElementModel : INotifyPropertyChanged, IMoniValueModel
    {
        #region IMoniValueModel

        public string Value
        {
            get { return CurrentValue; }
        }

        public int RefCount { get; set; } = 1;

        public event RoutedEventHandler ValueChanged = delegate { };
        
        #endregion
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public string AddrType { get; set; }
        public uint StartAddr { get; set; }
        public string IntrasegmentType { get; set; } = string.Empty;
        public uint IntrasegmentAddr { get; set; } = 0;
        private string _currentValue = string.Format("????");
        private string _setValue = string.Empty;
        private string[] _showTypes;
        
        public bool IsIntrasegment { get; set; }
        public string ShowName
        {
            get
            {
                if (!IsIntrasegment)
                {
                    return AddrType + StartAddr;
                }
                else
                {
                    return string.Format("{0}{1}{2}{3}", AddrType, StartAddr, IntrasegmentType, IntrasegmentAddr);
                }
            }
            set
            {
                Match m1 = Regex.Match(value, @"([A-Z]{1,2})([0-9]+)");
                if (m1.Success)
                {
                    IsIntrasegment = false;
                    AddrType = m1.Groups[1].Value;
                    StartAddr = uint.Parse(m1.Groups[2].Value);
                    IntrasegmentType = String.Empty;
                    IntrasegmentAddr = 0;
                    return;
                }
                Match m2 = Regex.Match(value, @"([A-Z]{1,2})([0-9]+)([VZ])([0-9]+)");
                if (m2.Success)
                {
                    IsIntrasegment = true;
                    AddrType = m1.Groups[1].Value;
                    StartAddr = uint.Parse(m1.Groups[2].Value);
                    IntrasegmentType = m1.Groups[3].Value;
                    IntrasegmentAddr = uint.Parse(m1.Groups[4].Value);
                    return;
                }
            }
        }
        public string FlagName
        {
            get
            {
                if (!IsIntrasegment)
                {
                    return String.Format("{0}_{1}_{2}", AddrType, StartAddr, DataType);
                }
                else
                {
                    return String.Format("{0}_{1}{2}_{3}_{4}", AddrType, IntrasegmentType, IntrasegmentAddr, StartAddr, DataType);
                }
            }
        }
        public string CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                _currentValue = value;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate () { PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentValue")); });
                ValueChanged.Invoke(this, new RoutedEventArgs());
            }
        }
        public string SetValue
        {
            get
            {
                return _setValue;
            }
            set
            {
                _setValue = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SetValue"));
            }
        }
        public string[] ShowTypes
        {
            get
            {
                return _showTypes;
            }
            set
            {
                _showTypes = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ShowTypes"));
            }
        }
        public string ShowType
        {
            get
            {
                switch (DataType)
                {
                    case 0:  return "BOOL";
                    case 1:  return "WORD";
                    case 2:  return "UWORD";
                    case 3:  return "DWORD";
                    case 4:  return "UDWORD";
                    case 5:  return "BCD";
                    case 6:  return "FLOAT";
                    default: return "null";
                }
            }
            set
            {
                switch (value)
                {
                    case "BOOL":  DataType = 0; break;
                    case "WORD":  DataType = 1; break;
                    case "UWORD": DataType = 2; break;
                    case "DWORD": DataType = 3; break;
                    case "UDWORD":DataType = 4; break;
                    case "BCD":   DataType = 5; break;
                    case "FLOAT": DataType = 6; break;
                }
            }
        }
        public int DataType { get; set; }
        public int ByteCount
        {
            get
            {
                switch (DataType)
                {
                    case 1: case 2: case 5:
                        return 2;
                    case 3: case 4:
                        return 4;
                    case 6:
                        return 4;
                    default:
                        return 1;
                }
            }
        }
        public int SelectIndex
        {
            get
            {
                if (DataType == 0)
                {
                    return DataType;
                }
                else
                {
                    return DataType - 1;
                }
            }
            set
            {
                if (DataType != 0)
                {
                    if (DataType != value + 1)
                    {
                        DataType = value + 1;
                        //IsModify = true;
                    }
                }
            }
        }
        public ElementModel(bool isIntrasegment, Enum dataType)
        {
            IsIntrasegment = isIntrasegment;
            if (dataType is BitType)
            {
                DataType = (int)(BitType)dataType;
            }
            else
            {
                DataType = (int)(WordType)dataType;
            }
            SetShowTypes();
        }
        public ElementModel(bool isIntrasegment, int dataType)
        {
            IsIntrasegment = isIntrasegment;
            DataType = dataType;
            SetShowTypes();
        }
        public void ShowPropertyChanged()
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ShowName"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ShowTypes"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SelectIndex"));
        }
        public ElementModel() { }

        public ElementModel(ElementModel emodel)
        {
            AddrType = emodel.AddrType;
            StartAddr = emodel.StartAddr;
            IsIntrasegment = emodel.IsIntrasegment;
            IntrasegmentType = emodel.IntrasegmentType;
            IntrasegmentAddr = emodel.IntrasegmentAddr;
            DataType = emodel.DataType;
            RefCount = 1;
            SetShowTypes();
        }

        public void SetShowTypes()
        {
            if (DataType == 0)
            {
                ShowTypes = new string[] { "BOOL" };
                //ShowTypes = Enum.GetNames(typeof(BitType));
            }
            else
            {
                ShowTypes = new string[] { "WORD", "UWORD", "DWORD", "UDWORD", "BCD", "FLOAT" };
                //ShowTypes = Enum.GetNames(typeof(WordType));
            }
        }
        public XElement CreateXElementBySelf()
        {
            XElement rootNode = new XElement("Element");
            rootNode.SetAttributeValue("DataType", DataType);
            rootNode.SetAttributeValue("AddrType", AddrType);
            rootNode.SetAttributeValue("StartAddr", StartAddr);
            rootNode.SetAttributeValue("IntrasegmentType", IntrasegmentType);
            rootNode.SetAttributeValue("IntrasegmentAddr", IntrasegmentAddr);
            rootNode.SetAttributeValue("IsIntrasegment", IsIntrasegment);
            return rootNode;
        }
        public void LoadSelfByXElement(XElement rootNode)
        {
            DataType = int.Parse(rootNode.Attribute("DataType").Value);
            AddrType = rootNode.Attribute("AddrType").Value;
            StartAddr = uint.Parse(rootNode.Attribute("StartAddr").Value);
            IntrasegmentAddr = uint.Parse(rootNode.Attribute("IntrasegmentAddr").Value);
            IntrasegmentType = rootNode.Attribute("IntrasegmentType").Value;
            IsIntrasegment = bool.Parse(rootNode.Attribute("IsIntrasegment").Value);
            SetShowTypes();
        }
    }
}

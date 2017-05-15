using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI
{
    public class WordElementModel : IElementInitializeModel,INotifyPropertyChanged
    {
        public string Base
        {
            get;
            set;
        }
        public int DataType
        {
            get;
            set;
        }
        public uint Offset
        {
            get;
            set;
        }
        public int SelectIndex
        {
            get
            {
                return DataType - 1;
            }
            set
            {
                DataType = value + 1;
            }
        }
        public string ShowName
        {
            get
            {
                return string.Format("{0}{1}",Base,Offset);
            }
        }
        private string _showValue;
        public string ShowValue
        {
            get
            {
                return _showValue;
            }
            set
            {
                _showValue = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ShowValue"));
            }
        }
        public string[] ShowTypes
        {
            get
            {
                return Enum.GetNames(typeof(WordType));
            }
        }
        public uint Value
        {
            get;
            set;
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public string GenerateCode()
        {
            switch ((WordType)Enum.ToObject(typeof(WordType),DataType))
            {
                case WordType.INT16:
                    return string.Format("{0}Word[{1}] = (int16_t){2};/r/n", Base, Offset, Value);
                case WordType.POS_INT16:
                    return string.Format("{0}Word[{1}] = (uint16_t){2};/r/n", Base, Offset, Value);
                case WordType.INT32:
                    return string.Format("{0}DoubleWord[{1}] = (int32_t){2};/r/n", Base, Offset, Value);
                case WordType.POS_INT32:
                    return string.Format("{0}DoubleWord[{1}] = (uint32_t){2};/r/n", Base, Offset, Value);
                case WordType.BCD:
                    return string.Format("{0}Word[{1}] = (uint16_t){2};/r/n", Base, Offset, Value);
                case WordType.FLOAT:
                    return string.Format("{0}Float[{1}] = (float){2};/r/n", Base, Offset, Value);
                default:
                    return string.Empty;
            }
        }
        public XElement CreateXElementByModel()
        {
            XElement rootNode = new XElement("EleModel");
            rootNode.SetAttributeValue("Type","Word");
            rootNode.SetAttributeValue("Base", Base);
            rootNode.SetAttributeValue("Offset", Offset);
            rootNode.SetAttributeValue("DataType", DataType);
            rootNode.SetAttributeValue("ShowValue", ShowValue);
            rootNode.SetAttributeValue("Value", Value);
            return rootNode;
        }
        public void LoadByXElement(XElement rootNode)
        {
            Base = rootNode.Attribute("Base").Value;
            Offset = uint.Parse(rootNode.Attribute("Offset").Value);
            DataType = int.Parse(rootNode.Attribute("DataType").Value);
            ShowValue = rootNode.Attribute("ShowValue").Value;
            Value = uint.Parse(rootNode.Attribute("Value").Value);
        }
    }
}

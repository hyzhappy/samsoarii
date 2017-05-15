using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI
{
    public class BitElementModel : IElementInitializeModel
    {
        public string Base
        {
            get;
            set;
        }
        public int DataType
        {
            get
            {
                return 0;
            }
            set{}
        }
        public uint Offset
        {
            get;
            set;
        }
        public string ShowName
        {
            get
            {
                return string.Format("{0}{1}", Base, Offset);
            }
        }
        public int SelectIndex
        {
            get
            {
                return 0;
            }
            set { }
        }
        public string ShowValue
        {
            get;
            set;
        }
        public string[] ShowTypes
        {
            get
            {
                return Enum.GetNames(typeof(BitType));
            }
        }
        public uint Value
        {
            get;
            set;
        }
        public string GenerateCode()
        {
            return string.Format("{0}Bit[{1}] = (plc_bool){2};/r/n", Base,Offset,Value);
        }
        public XElement CreateXElementByModel()
        {
            XElement rootNode = new XElement("EleModel");
            rootNode.SetAttributeValue("Type", "Bit");
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

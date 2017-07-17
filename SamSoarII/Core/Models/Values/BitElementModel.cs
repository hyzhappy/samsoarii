using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public class BitElementModel : IElementInitializeModel
    {
        public ValueInfo parent;
        public ValueInfo Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                ValueInfo _parent = parent;
                this.parent = value;
                if (_parent != null && _parent.InitModel != null)
                    _parent.InitModel = null;
                if (parent != null && parent.InitModel != this)
                    parent.InitModel = this;
            }
        }

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
            set { }
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
        public XElement CreateXElementByModel()
        {
            XElement rootNode = new XElement("InitModel");
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

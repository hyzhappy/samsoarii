using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SamSoarII.Core.Models
{
    public interface IElementInitializeModel
    {
        ValueInfo Parent { get; set; }
        string ShowName { get; }
        string ShowValue { get; set; }
        string[] ShowTypes { get; }
        int SelectIndex { get; set; }
        string Base { get; set; }
        uint Offset { get; set; }
        int DataType { get; set; }
        uint Value { get; set; }
        XElement CreateXElementByModel();
        void LoadByXElement(XElement rootNode);
    }
}

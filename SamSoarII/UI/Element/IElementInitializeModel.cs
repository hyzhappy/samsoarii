using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI
{
    public interface IElementInitializeModel
    {
        string ShowName { get;}
        string ShowValue { get; set; }
        string[] ShowTypes { get; }
        int SelectIndex { get; set; }
        string Base { get; set; }
        uint Offset { get; set; }
        int DataType { get; set; }
        uint Value { get; set; }
        string GenerateCode();
        XElement CreateXElementByModel();
        void LoadByXElement(XElement rootNode);
    }
}

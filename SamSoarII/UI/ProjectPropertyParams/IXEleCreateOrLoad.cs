using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI
{
    public interface IXEleCreateOrLoad
    {
        XElement CreateRootXElement();
        void LoadPropertyByXElement(XElement ele);
    }
}

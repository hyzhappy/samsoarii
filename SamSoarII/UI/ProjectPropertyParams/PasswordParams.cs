using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI
{
    public class PasswordParams : INotifyPropertyChanged, IXEleCreateOrLoad
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public XElement CreateRootXElement()
        {
            XElement rootNode = new XElement("PasswordParams");
            return rootNode;
        }

        public void LoadPropertyByXElement(XElement ele)
        {
            
        }
    }
}

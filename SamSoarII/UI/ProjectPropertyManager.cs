using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI
{
    public enum CommunicationType
    {
        Master,
        Slave,
        FreePort
    }
    public class ProjectPropertyManager
    {
        public static Dictionary<string, IXEleCreateOrLoad> ParamsDic;
        private ProjectPropertyManager(){}
        static ProjectPropertyManager()
        {
            ParamsDic = new Dictionary<string, IXEleCreateOrLoad>();
            ParamsDic.Add("CommunParams232", new CommunicationInterfaceParams());
            ParamsDic.Add("CommunParams485", new CommunicationInterfaceParams());
            ParamsDic.Add("PasswordParams", new PasswordParams());
            ParamsDic.Add("FilterParams", new FilterParams());
            ParamsDic.Add("HoldingSectParams", new HoldingSectionParams());
            ParamsDic.Add("AnalogQuantityParams", new AnalogQuantityParams());
            ParamsDic.Add("ExpanModuleParams", new ExpansionModuleParams());
        }
        #region Save and Load ProjectProperty
        public static XElement CreateProjectPropertyXElement()
        {
            var rootNode = new XElement("ProjectPropertyParams");
            for (int i = 0; i < ParamsDic.Values.Count; i++)
            {
                var node = ParamsDic.Values.ElementAt(i).CreateRootXElement();
                if (i == 0)
                {
                    node.SetAttributeValue("Interface",232);
                }
                if (i == 1)
                {
                    node.SetAttributeValue("Interface", 485);
                }
                rootNode.Add(node);
            }
            return rootNode;
        }
        public static void LoadProjectPropertyByXElement(XElement rootNode)
        {
            foreach (var item in rootNode.Elements("CommuParams"))
            {
                if (item.FirstAttribute.Value == "232")
                {
                    ParamsDic["CommunParams232"].LoadPropertyByXElement(item);
                }else if (item.FirstAttribute.Value == "485")
                {
                    ParamsDic["CommunParams485"].LoadPropertyByXElement(item);
                }
            }
            ParamsDic["FilterParams"].LoadPropertyByXElement(rootNode.Element("FilterParams"));
            ParamsDic["HoldingSectParams"].LoadPropertyByXElement(rootNode.Element("HoldingSectParams"));
            ParamsDic["AnalogQuantityParams"].LoadPropertyByXElement(rootNode.Element("AnalogQuantityParams"));
            ParamsDic["ExpanModuleParams"].LoadPropertyByXElement(rootNode.Element("ExpanModuleParams"));
        }
        #endregion
    }
}

using SamSoarII.UserInterface;
using SamSoarII.Utility;
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
        public static bool IsModify { get; set; }
        public static Dictionary<string, IXEleCreateOrLoad> ProjectPropertyDic;
        private ProjectPropertyManager(){}
        static ProjectPropertyManager()
        {
            ProjectPropertyDic = new Dictionary<string, IXEleCreateOrLoad>();
            ProjectPropertyDic.Add("CommunParams232", new CommunicationInterfaceParams());
            ProjectPropertyDic.Add("CommunParams485", new CommunicationInterfaceParams());
            ProjectPropertyDic.Add("PasswordParams", new PasswordParams());
            ProjectPropertyDic.Add("FilterParams", new FilterParams());
            ProjectPropertyDic.Add("HoldingSectParams", new HoldingSectionParams());
            ProjectPropertyDic.Add("AnalogQuantityParams", new AnalogQuantityParams());
            ProjectPropertyDic.Add("ExpanModuleParams", new ExpansionModuleParams());
            ProjectPropertyDic.Add("CommunicationParams",new CommunicationParams());
            IsModify = false;
        }
        #region Save and Load ProjectProperty
        public static XElement CreateProjectPropertyXElement()
        {
            var rootNode = new XElement("ProjectPropertyParams");
            for (int i = 0; i < ProjectPropertyDic.Values.Count; i++)
            {
                var node = ProjectPropertyDic.Values.ElementAt(i).CreateRootXElement();
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
                    ProjectPropertyDic["CommunParams232"].LoadPropertyByXElement(item);
                }else if (item.FirstAttribute.Value == "485")
                {
                    ProjectPropertyDic["CommunParams485"].LoadPropertyByXElement(item);
                }
            }
            ProjectPropertyDic["PasswordParams"].LoadPropertyByXElement(rootNode.Element("PasswordParams"));
            ProjectPropertyDic["FilterParams"].LoadPropertyByXElement(rootNode.Element("FilterParams"));
            ProjectPropertyDic["HoldingSectParams"].LoadPropertyByXElement(rootNode.Element("HoldingSectParams"));
            ProjectPropertyDic["AnalogQuantityParams"].LoadPropertyByXElement(rootNode.Element("AnalogQuantityParams"));
            ProjectPropertyDic["ExpanModuleParams"].LoadPropertyByXElement(rootNode.Element("ExpanModuleParams"));
            ProjectPropertyDic["CommunicationParams"].LoadPropertyByXElement(rootNode.Element("CommunicationParams"));
        }
        #endregion
    }
}

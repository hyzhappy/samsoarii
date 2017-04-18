using SamSoarII.AppMain.Properties;
using SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI.HelpDocComponet.UserSetting
{
    public static class SettingManager
    {
        private static string dir = Directory.GetCurrentDirectory() + @"\UserSetting";
        static SettingManager()
        {
            string path = dir + @"\FavoritePages.xml";
            if (!File.Exists(path))
            {
                XDocument xDoc = new XDocument();
                var rootNode = new XElement("FavoritePages");
                xDoc.Add(rootNode);
                Directory.CreateDirectory(dir);
                xDoc.Save(File.Create(path));
            }
        }
        public static void Load()
        {
            XDocument xDoc = XDocument.Load(dir + @"\FavoritePages.xml");
            XElement rootNode = xDoc.Root;
            int pageindex;
            foreach (var ele in rootNode.Elements())
            {
                pageindex = int.Parse(ele.Attribute("PageIndex").Value);
                FavoriteManager.TabItemCollection.Add(PageManager.PageCollection[pageindex]);
            }
        }
        public static void Save()
        {
            XDocument xDoc = new XDocument();
            var rootNode = new XElement("FavoritePages");
            foreach (var item in FavoriteManager.TabItemCollection)
            {
                XElement ele = new XElement("Page");
                ele.SetAttributeValue("PageIndex",item.PageIndex);
                rootNode.Add(ele);
            }
            xDoc.Add(rootNode);
            xDoc.Save(dir + @"\FavoritePages.xml");
        }
    }
}

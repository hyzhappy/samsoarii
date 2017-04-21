﻿using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.Properties;
using SamSoarII.AppMain.UI.HelpDocComponet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.AppMain.UI
{
    public static class SettingManager
    {
        private static string dir = Directory.GetCurrentDirectory() + @"\UserSetting";
        static SettingManager()
        {
            string path = dir + @"\UserSetting.xml";
            if (!File.Exists(path))
            {
                XDocument xDoc = new XDocument();
                var rootNode = new XElement("UserSetting");
                rootNode.Add(new XElement("FavoritePages"));
                rootNode.Add(new XElement("RecentUsedProjectMessages"));
                rootNode.Add(new XElement("SystemSetting"));
                rootNode.Add(new XElement("SimulateSetting"));
                xDoc.Add(rootNode);
                Directory.CreateDirectory(dir);
                xDoc.Save(File.Create(path));
            }
        }
        public static void Load()
        {
            XDocument xDoc = XDocument.Load(dir + @"\UserSetting.xml");
            XElement rootNode = xDoc.Root;
            if (rootNode.HasElements)
            {
                FavoriteManager.LoadFavoritePagesByXElement(rootNode.Element("FavoritePages"));
                ProjectFileManager.LoadRecentUsedProjectsByXElement(rootNode.Element("RecentUsedProjectMessages"));
                GlobalSetting.LoadSystemSettingByXELement(rootNode.Element("SystemSetting"));
                Simulation.Core.Global.GlobalSetting.LoadSystemSettingByXELement(rootNode.Element("SimulateSetting"), rootNode.Element("SystemSetting"));
            }
        }
        public static void Save()
        {
            XDocument xDoc = new XDocument();
            XElement rootNode = new XElement("UserSetting");
            rootNode.Add(FavoriteManager.CreateXElementByPageIndex());
            rootNode.Add(ProjectFileManager.CreateXElementByRecentUsedProjects());
            rootNode.Add(GlobalSetting.CreateXELementBySetting());
            rootNode.Add(Simulation.Core.Global.GlobalSetting.CreateXELementBySetting());
            xDoc.Add(rootNode);
            xDoc.Save(dir + @"\UserSetting.xml");
        }
    }
}

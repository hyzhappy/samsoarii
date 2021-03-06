﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SamSoarII.Utility
{
    public static class XmlGen
    {
        public static List<Tuple<string,int>> SysRegisters;
        public static void GenSysRegisterXml()
        {
            XDocument xDoc = new XDocument();
            XElement rootNode = new XElement("SpecialRegisters");
            foreach (var item in SysRegisters)
            {
                XElement node = new XElement("Register");
                node.SetAttributeValue("Base",item.Item1);
                node.SetAttributeValue("Offset", item.Item2);
                node.SetAttributeValue("CanRead", true);
                node.SetAttributeValue("CanWrite", true);
                node.SetAttributeValue("Describe", string.Empty);
                rootNode.Add(node);
            }
            xDoc.Add(rootNode);
            xDoc.Save(Directory.GetCurrentDirectory() + "SpecialRegisters.xml");
        }
        static XmlGen()
        {
            SysRegisters = new List<Tuple<string, int>>();
            for (int i = 0; i < 8; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("AI", i));
            }
            for (int i = 26; i < 32; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("AI", i));
            }
            SysRegisters.Add(new Tuple<string, int>("AO", 30));
            SysRegisters.Add(new Tuple<string, int>("AO", 31));
            for (int i = 5952; i < 5968; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("D", i));
            }
            for (int i = 8060; i < 8080; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("D", i));
            }
            for (int i = 8092; i < 8096; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("D", i));
            }
            for (int i = 8108; i < 8112; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("D", i));
            }
            for (int i = 8124; i < 8128; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("D", i));
            }
            for (int i = 8140; i < 8148; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("D", i));
            }
            for (int i = 8173; i < 8178; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("D", i));
            }
            for (int i = 7488; i < 7504; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("M", i));
            }
            for (int i = 8035; i < 8046; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("M", i));
            }
            for (int i = 8051; i < 8056; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("M", i));
            }
            for (int i = 8070; i < 8074; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("M", i));
            }
            for (int i = 8086; i < 8090; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("M", i));
            }
            for (int i = 8102; i < 8106; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("M", i));
            }
            for (int i = 8118; i < 8122; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("M", i));
            }
            for (int i = 8134; i < 8138; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("M", i));
            }
            for (int i = 8150; i < 8184; i++)
            {
                SysRegisters.Add(new Tuple<string, int>("M", i));
            }
        }
        public static void GenUpdateXML(string dir)
        {
            XDocument xDoc = new XDocument();
            XElement rootNode = new XElement("AppUpdate");
            xDoc.Add(rootNode);
            foreach (var path in Directory.GetFileSystemEntries(@"C:\Users\yangzheyu\软件\Setup"))
            {
                if (!File.Exists(path) && (path.EndsWith("Compiler") || path.EndsWith("zh-Hans") || path.EndsWith("Update")))
                {
                    continue;
                }
                GenXElementsByDir(path, rootNode);
            }
            xDoc.Save(dir + @"\Update\AppUpdate.xml");
        }
        private static void GenXElementsByDir(string path, XElement node)
        {
            if (File.Exists(path))
            {
                FileInfo file = new FileInfo(path);
                XElement filenode = new XElement("file");
                node.Add(filenode);
                filenode.SetAttributeValue("filename",file.Name);
                filenode.SetAttributeValue("relativepath",GetRelativePath(file.DirectoryName, @"C:\Users\yangzheyu\软件\Setup"));
                filenode.SetAttributeValue("md5",FileHelper.GetMD5(path));
                filenode.SetAttributeValue("version",1);
                filenode.SetAttributeValue("size",file.Length);
            }
            else
            {
                foreach (var dir in Directory.GetFileSystemEntries(path))
                {
                    GenXElementsByDir(dir,node);
                }
            }
        }
        private static string GetRelativePath(string fullpath,string relative)
        {
            int index = fullpath.IndexOf(relative);
            if (index >= 0)
            {
                return fullpath.Substring(index + relative.Length);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Global
{
    public class LayoutSetting
    {
        #region Default dock of Anchorable
        static private Dictionary<string, string> _defaultDockWidthAnchorable = new Dictionary<string, string>();
        static private Dictionary<string, string> _defaultDockHeighAnchorable = new Dictionary<string, string>();
        static public void AddDefaultDockWidthAnchorable(string titlename, string dockwidth)
        {
            if (!_defaultDockWidthAnchorable.ContainsKey(titlename))
            {
                _defaultDockWidthAnchorable.Add(titlename, dockwidth);
            }
            else
            {
                _defaultDockWidthAnchorable[titlename] = dockwidth;
            }
        }
        static public void AddDefaultDockHeighAnchorable(string titlename, string dockwidth)
        {
            if (!_defaultDockHeighAnchorable.ContainsKey(titlename))
            {
                _defaultDockHeighAnchorable.Add(titlename, dockwidth);
            }
            else
            {
                _defaultDockHeighAnchorable[titlename] = dockwidth;
            }
        }
        static public GridLength[] GetDefaultDockAnchorable(string titlename)
        {
            string widthst = "*";
            string heighst = "*";
            if (_defaultDockWidthAnchorable.ContainsKey(titlename))
            {
                widthst = _defaultDockWidthAnchorable[titlename];
            }
            if (_defaultDockHeighAnchorable.ContainsKey(titlename))
            {
                heighst = _defaultDockHeighAnchorable[titlename];
            }
            GridLength widthgl = ParseGridLength(widthst);
            GridLength heighgl = ParseGridLength(heighst); 
            GridLength[] ret = { widthgl, heighgl};
            return ret; 
        }
        static private GridLength ParseGridLength(string st)
        {
            Match m1 = Regex.Match(st, @"^\*$");
            Match m2 = Regex.Match(st, @"^\d+$");
            Match m3 = Regex.Match(st, @"^\d+\.\d+$");
            Match m4 = Regex.Match(st, @"^\d+\*$");
            Match m5 = Regex.Match(st, @"^\d+.\d+\*$");
            Match m6 = Regex.Match(st, @"\{\*\}");
            Match m7 = Regex.Match(st, @"\{\d+\}");
            Match m8 = Regex.Match(st, @"\{\d+\.\d+\}");
            Match m9 = Regex.Match(st, @"\{\d+\*\}");
            Match m10 = Regex.Match(st, @"\{\d+.\d+\*\}");

            if (m2.Success || m3.Success)
            {
                return new GridLength(double.Parse(st), GridUnitType.Pixel);        
            }
            if (m4.Success || m5.Success)
            {
                return new GridLength(double.Parse(st.Substring(0, st.Length - 1)), GridUnitType.Star);
            }
            if (m7.Success || m8.Success)
            {
                return new GridLength(double.Parse(st.Substring(1, st.Length - 2)), GridUnitType.Pixel);
            }
            if (m9.Success || m10.Success)
            {
                return new GridLength(double.Parse(st.Substring(1, st.Length - 3)), GridUnitType.Star);
            }

            return new GridLength(1, GridUnitType.Star);
        }
        #endregion

        #region Default side of Anchorable
        static private Dictionary<string, string> _defaultSideAnchorable = new Dictionary<string, string>();
        static public void AddDefaultSideAnchorable(string titlename, string sidename)
        {
            if (_defaultSideAnchorable.ContainsKey(titlename))
            {
                _defaultSideAnchorable[titlename] = sidename.ToUpper();
            }
            else
            {
                _defaultSideAnchorable.Add(titlename, sidename);
            }
        }
        static public AnchorSide GetDefaultSideAnchorable(string titlename)
        {
            if (_defaultSideAnchorable.ContainsKey(titlename))
            {
                string sidename = _defaultSideAnchorable[titlename];
                switch (sidename)
                {
                    case "L":
                    case "LEFT":
                        return AnchorSide.Left;
                    case "R":
                    case "RIGHT":
                        return AnchorSide.Right;
                    case "U":
                    case "UP":
                    case "TOP":
                        return AnchorSide.Top;
                    case "D":
                    case "DOWN":
                    case "BOTTOM":
                        return AnchorSide.Bottom;
                    default:
                        return AnchorSide.Left;
                }
            }
            return AnchorSide.Left;
        }
        #endregion


        public const int KEYVALUEPAIR_DOCKWIDTH = 0x01;
        public const int KEYVALUEPAIR_DOCKHEIGH = 0x02;
        public const int KEYVALUEPAIR_SIDE = 0x03;

        static public void Save()
        {
            XDocument xdoc = new XDocument();
            XElement node_Root = new XElement("LayoutSetting");
            XElement node_DW = new XElement("DockWidth");
            XElement node_DH = new XElement("DockHeight");
            XElement node_S = new XElement("Side");
            node_Root.Add(node_DW);
            node_Root.Add(node_DH);
            node_Root.Add(node_S);

            XElement node_KVP = null;
            foreach (KeyValuePair<string, string> kvp in _defaultDockWidthAnchorable)
            {
                node_KVP = new XElement("KeyValuePair");
                node_KVP.SetAttributeValue("Key", kvp.Key);
                node_KVP.SetAttributeValue("Value", kvp.Value);
                node_DW.Add(node_KVP);
            }
            foreach (KeyValuePair<string, string> kvp in _defaultDockHeighAnchorable)
            {
                node_KVP = new XElement("KeyValuePair");
                node_KVP.SetAttributeValue("Key", kvp.Key);
                node_KVP.SetAttributeValue("Value", kvp.Value);
                node_DH.Add(node_KVP);
            }
            foreach (KeyValuePair<string, string> kvp in _defaultSideAnchorable)
            {
                node_KVP = new XElement("KeyValuePair");
                node_KVP.SetAttributeValue("Key", kvp.Key);
                node_KVP.SetAttributeValue("Value", kvp.Value);
                node_S.Add(node_KVP);
            }

            xdoc.Add(node_Root);
            xdoc.Save("layoutconfig.xml");
        }

        static public void Load()
        {
            XDocument xdoc = XDocument.Load("layoutconfig.xml");
            XElement node_Root = xdoc.Element("LayoutSetting");
            IEnumerable<XElement> nodes_DW = xdoc.Elements("DockWidth");
            IEnumerable<XElement> nodes_DH = xdoc.Elements("DockHeight");
            IEnumerable<XElement> nodes_S = xdoc.Elements("Side");
            foreach (XElement node_KVP in nodes_DW)
            {
                string key = node_KVP.Attribute("Key").Value as string;
                string value = node_KVP.Attribute("Value").Value as string;
                AddDefaultDockWidthAnchorable(key, value);
            }
            foreach (XElement node_KVP in nodes_DH)
            {
                string key = node_KVP.Attribute("Key").Value as string;
                string value = node_KVP.Attribute("Value").Value as string;
                AddDefaultDockHeighAnchorable(key, value);
            }
            foreach (XElement node_KVP in nodes_S)
            {
                string key = node_KVP.Attribute("Key").Value as string;
                string value = node_KVP.Attribute("Value").Value as string;
                AddDefaultSideAnchorable(key, value);
            }
        }
    }
}

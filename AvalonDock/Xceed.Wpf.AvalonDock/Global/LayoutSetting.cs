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
            if (titlename == null) return;
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
            if (titlename == null) return;
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
            GridLength[] ret = new GridLength[2];
            if (titlename == null)
            {
                ret[0] = new GridLength(1, GridUnitType.Star);
                ret[1] = new GridLength(1, GridUnitType.Star);
                return ret;
            }
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
            ret[0] = ParseGridLength(widthst);
            ret[1] = ParseGridLength(heighst); 
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

        #region Default IsDock of Anchorable
        static private Dictionary<string, bool> _defaultIsDockAnchorable = new Dictionary<string, bool>();

        static public void AddDefaultIsDockAnchorable(string titlename, bool isdock)
        {
            if (titlename == null) return;
            if (!_defaultIsDockAnchorable.ContainsKey(titlename))
            {
                _defaultIsDockAnchorable.Add(titlename, isdock);
            }
            else
            {
                _defaultIsDockAnchorable[titlename] = isdock;
            }
        }

        static public bool GetDefaultIsDockAnchorable(string titlename)
        {
            if (titlename == null) return false;
            if (!_defaultIsDockAnchorable.ContainsKey(titlename))
            {
                return false;
            }
            else
            {
                return _defaultIsDockAnchorable[titlename];
            }
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

        #region Default FloatWindow Size of Anchorable
        static private Dictionary<string, string> _defaultFloatWidthAnchorable = new Dictionary<string, string>();

        static private Dictionary<string, string> _defaultFloatHeighAnchorable = new Dictionary<string, string>();

        static public void AddDefaultFloatWidthAnchorable(string titlename, string floatwidth)
        {
            if (_defaultFloatWidthAnchorable.ContainsKey(titlename))
            {
                _defaultFloatWidthAnchorable[titlename] = floatwidth;
            }
            else
            {
                _defaultFloatWidthAnchorable.Add(titlename, floatwidth);
            }
        }

        static public void AddDefaultFloatHeighAnchorable(string titlename, string floatwidth)
        {
            if (_defaultFloatHeighAnchorable.ContainsKey(titlename))
            {
                _defaultFloatHeighAnchorable[titlename] = floatwidth;
            }
            else
            {
                _defaultFloatHeighAnchorable.Add(titlename, floatwidth);
            }
        }

        static public double[] GetDefaultFloatSizeAnchorable(string titlename)
        {
            double floatwidth = 0.0;
            double floatheigh = 0.0;
            if (_defaultFloatWidthAnchorable.ContainsKey(titlename))
            {
                floatwidth = double.Parse(_defaultFloatWidthAnchorable[titlename]);
            }
            if (_defaultFloatHeighAnchorable.ContainsKey(titlename))
            {
                floatheigh = double.Parse(_defaultFloatHeighAnchorable[titlename]);
            }
            double[] ret = { floatwidth, floatheigh };
            return ret;
        }
        #endregion
        
        #region Default IsFloat of Anchorable
        static private Dictionary<string, bool> _defaultIsFloatAnchorable = new Dictionary<string, bool>();

        static public void AddDefaultIsFloatAnchorable(string titlename, bool isdock)
        {
            if (titlename == null) return;
            if (!_defaultIsFloatAnchorable.ContainsKey(titlename))
            {
                _defaultIsFloatAnchorable.Add(titlename, isdock);
            }
            else
            {
                _defaultIsFloatAnchorable[titlename] = isdock;
            }
        }

        static public bool GetDefaultIsFloatAnchorable(string titlename)
        {
            if (titlename == null) return false;
            if (!_defaultIsFloatAnchorable.ContainsKey(titlename))
            {
                return false;
            }
            else
            {
                return _defaultIsFloatAnchorable[titlename];
            }
        }

        #endregion

        #region Default AutoHide Size of Anchorable
        static private Dictionary<string, string> _defaultAutoHideWidthAnchorable = new Dictionary<string, string>();
        static private Dictionary<string, string> _defaultAutoHideHeighAnchorable = new Dictionary<string, string>();
        static public void AddDefaultAutoHideWidthAnchorable(string titlename, string floatwidth)
        {
            if (_defaultAutoHideWidthAnchorable.ContainsKey(titlename))
            {
                _defaultAutoHideWidthAnchorable[titlename] = floatwidth;
            }
            else
            {
                _defaultAutoHideWidthAnchorable.Add(titlename, floatwidth);
            }
        }
        static public void AddDefaultAutoHideHeighAnchorable(string titlename, string floatwidth)
        {
            if (_defaultAutoHideHeighAnchorable.ContainsKey(titlename))
            {
                _defaultAutoHideHeighAnchorable[titlename] = floatwidth;
            }
            else
            {
                _defaultAutoHideHeighAnchorable.Add(titlename, floatwidth);
            }
        }
        static public double[] GetDefaultAutoHideSizeAnchorable(string titlename)
        {
            double floatwidth = 0.0;
            double floatheigh = 0.0;
            if (_defaultAutoHideWidthAnchorable.ContainsKey(titlename))
            {
                floatwidth = double.Parse(_defaultAutoHideWidthAnchorable[titlename]);
            }
            if (_defaultAutoHideHeighAnchorable.ContainsKey(titlename))
            {
                floatheigh = double.Parse(_defaultAutoHideHeighAnchorable[titlename]);
            }
            double[] ret = { floatwidth, floatheigh };
            return ret;
        }
        #endregion
        
        static public void Save()
        {
            XDocument xdoc = new XDocument();
            XElement node_Root = new XElement("LayoutSetting");
            XElement node_ID = new XElement("IsDock");
            XElement node_DW = new XElement("DockWidth");
            XElement node_DH = new XElement("DockHeight");
            XElement node_S = new XElement("Side");
            XElement node_IF = new XElement("IsFloat");
            XElement node_FW = new XElement("FloatWidth");
            XElement node_FH = new XElement("FloatHeight");
            XElement node_AW = new XElement("AutoHideWidth");
            XElement node_AH = new XElement("AutoHideHeight");
            node_Root.Add(node_ID);
            node_Root.Add(node_DW);
            node_Root.Add(node_DH);
            node_Root.Add(node_S);
            node_Root.Add(node_IF);
            node_Root.Add(node_FW);
            node_Root.Add(node_FH);
            node_Root.Add(node_AW);
            node_Root.Add(node_AH);

            XElement node_KVP = null;
            foreach (KeyValuePair<string, bool> kvp in _defaultIsDockAnchorable)
            {
                node_KVP = new XElement("KeyValuePair");
                node_KVP.SetAttributeValue("Key", kvp.Key);
                node_KVP.SetAttributeValue("Value", kvp.Value);
                node_ID.Add(node_KVP);
            }
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
            foreach (KeyValuePair<string, bool> kvp in _defaultIsFloatAnchorable)
            {
                node_KVP = new XElement("KeyValuePair");
                node_KVP.SetAttributeValue("Key", kvp.Key);
                node_KVP.SetAttributeValue("Value", kvp.Value);
                node_IF.Add(node_KVP);
            }
            foreach (KeyValuePair<string, string> kvp in _defaultFloatWidthAnchorable)
            {
                node_KVP = new XElement("KeyValuePair");
                node_KVP.SetAttributeValue("Key", kvp.Key);
                node_KVP.SetAttributeValue("Value", kvp.Value);
                node_FW.Add(node_KVP);
            }
            foreach (KeyValuePair<string, string> kvp in _defaultFloatHeighAnchorable)
            {
                node_KVP = new XElement("KeyValuePair");
                node_KVP.SetAttributeValue("Key", kvp.Key);
                node_KVP.SetAttributeValue("Value", kvp.Value);
                node_FH.Add(node_KVP);
            }

            foreach (KeyValuePair<string, string> kvp in _defaultAutoHideWidthAnchorable)
            {
                node_KVP = new XElement("KeyValuePair");
                node_KVP.SetAttributeValue("Key", kvp.Key);
                node_KVP.SetAttributeValue("Value", kvp.Value);
                node_AW.Add(node_KVP);
            }
            foreach (KeyValuePair<string, string> kvp in _defaultAutoHideHeighAnchorable)
            {
                node_KVP = new XElement("KeyValuePair");
                node_KVP.SetAttributeValue("Key", kvp.Key);
                node_KVP.SetAttributeValue("Value", kvp.Value);
                node_AH.Add(node_KVP);
            }
            xdoc.Add(node_Root);
            xdoc.Save("layoutconfig.xml");
        }

        static public void Load()
        {
            XDocument xdoc = XDocument.Load("layoutconfig.xml");
            XElement node_Root = xdoc.Element("LayoutSetting");
            XElement nodes_ID = node_Root.Element("IsDock");
            XElement nodes_DW = node_Root.Element("DockWidth");
            XElement nodes_DH = node_Root.Element("DockHeight");
            XElement nodes_S = node_Root.Element("Side");
            XElement nodes_IF = node_Root.Element("IsFloat");
            XElement nodes_FW = node_Root.Element("FloatWidth");
            XElement nodes_FH = node_Root.Element("FloatHeight");
            XElement nodes_AW = node_Root.Element("AutoHideWidth");
            XElement nodes_AH = node_Root.Element("AutoHideHeight");
            foreach (XElement node_KVP in nodes_ID.Elements("KeyValuePair"))
            {
                string key = node_KVP.Attribute("Key").Value as string;
                bool value = bool.Parse(node_KVP.Attribute("Value").Value);
                AddDefaultIsDockAnchorable(key, value);
            }
            foreach (XElement node_KVP in nodes_DW.Elements("KeyValuePair"))
            {
                string key = node_KVP.Attribute("Key").Value as string;
                string value = node_KVP.Attribute("Value").Value as string;
                AddDefaultDockWidthAnchorable(key, value);
            }
            foreach (XElement node_KVP in nodes_DH.Elements("KeyValuePair"))
            {
                string key = node_KVP.Attribute("Key").Value as string;
                string value = node_KVP.Attribute("Value").Value as string;
                AddDefaultDockHeighAnchorable(key, value);
            }
            foreach (XElement node_KVP in nodes_S.Elements("KeyValuePair"))
            {
                string key = node_KVP.Attribute("Key").Value as string;
                string value = node_KVP.Attribute("Value").Value as string;
                AddDefaultSideAnchorable(key, value);
            }
            foreach (XElement node_KVP in nodes_IF.Elements("KeyValuePair"))
            {
                string key = node_KVP.Attribute("Key").Value as string;
                bool value = bool.Parse(node_KVP.Attribute("Value").Value);
                AddDefaultIsFloatAnchorable(key, value);
            }
            foreach (XElement node_KVP in nodes_FW.Elements("KeyValuePair"))
            {
                string key = node_KVP.Attribute("Key").Value as string;
                string value = node_KVP.Attribute("Value").Value as string;
                AddDefaultFloatWidthAnchorable(key, value);
            }
            foreach (XElement node_KVP in nodes_FH.Elements("KeyValuePair"))
            {
                string key = node_KVP.Attribute("Key").Value as string;
                string value = node_KVP.Attribute("Value").Value as string;
                AddDefaultFloatHeighAnchorable(key, value);
            }
            foreach (XElement node_KVP in nodes_AW.Elements("KeyValuePair"))
            {
                string key = node_KVP.Attribute("Key").Value as string;
                string value = node_KVP.Attribute("Value").Value as string;
                AddDefaultAutoHideWidthAnchorable(key, value);
            }
            foreach (XElement node_KVP in nodes_AH.Elements("KeyValuePair"))
            {
                string key = node_KVP.Attribute("Key").Value as string;
                string value = node_KVP.Attribute("Value").Value as string;
                AddDefaultAutoHideHeighAnchorable(key, value);
            }
        }
    }
}

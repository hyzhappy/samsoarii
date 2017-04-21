using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Configuration;
using SamSoarII.LadderInstViewModel;
using System.Xml.Linq;

namespace SamSoarII.AppMain
{
    public static class GlobalSetting
    {

        private const double ScaleMin = 0.4;
        private const double ScaleMax = 2.5;

        public static int LadderWidthUnit { get { return 300; } }

        public static int LadderHeightUnit { get { return 300; } }

        public static int LadderCommentModeHeightUnit { get { return 500; } }

        public static int LadderXCapacity { get { return 12; } }

        private static double _ladderOriginScaleX;
        private static double _ladderOriginScaleY;

        public static double LadderOriginScaleX
        {
            get
            {
                return _ladderOriginScaleX;
            }
            set
            {
                _ladderOriginScaleX = value;
                LadderScaleTransform.ScaleX = _ladderOriginScaleX * _ladderScaleX;
            }
        }

        public static double LadderOriginScaleY
        {
            get
            {
                return _ladderOriginScaleY;
            }
            set
            {
                _ladderOriginScaleY = value;
                LadderScaleTransform.ScaleY = _ladderScaleY * _ladderOriginScaleY;
            }
        }

        private static double _ladderScaleX = 1.0;
        private static double _ladderScaleY = 1.0;

        public static double LadderScaleX
        {
            get
            {
                return _ladderScaleX;
            }
            set
            {
                if (value > ScaleMin && value < ScaleMax)
                {
                    _ladderScaleX = value;
                    LadderScaleTransform.ScaleX = _ladderScaleX * _ladderOriginScaleX;
                }
            }
        }
        public static double LadderScaleY
        {
            get
            {
                return _ladderScaleY;
            }
            set
            {
                if (value > ScaleMin && value < ScaleMax)
                {
                    _ladderScaleY = value;
                    LadderScaleTransform.ScaleY = _ladderOriginScaleY * _ladderScaleY;
                }
            }
        }
        private static bool _loadScaleSuccessFlag;

        public static ScaleTransform LadderScaleTransform { get; private set; }

        public static int FuncBlockFontSize { get; set; }
        public static int SelectedIndexOfFontSizeComboBox { get; set; }
        public static int SelectedIndexOfFontFamilyComboBox { get; set; }
        public static Color SelectColor { get; set; }
        private static byte _A;
        private static byte _R;
        private static byte _G;
        private static byte _B;
        static GlobalSetting()
        {
            LadderScaleTransform = new ScaleTransform();
        }
        //public static void Save()
        //{
        //    var cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    cfa.AppSettings.Settings["LadderOriginScaleX"].Value = LadderOriginScaleX.ToString();
        //    cfa.AppSettings.Settings["LadderOriginScaleY"].Value = LadderOriginScaleY.ToString();
        //    cfa.AppSettings.Settings["LadderScaleX"].Value = LadderScaleX.ToString();
        //    cfa.AppSettings.Settings["LadderScaleY"].Value = LadderScaleY.ToString();
        //    cfa.AppSettings.Settings["FuncBlockFontSize"].Value = FuncBlockFontSize.ToString();
        //    cfa.AppSettings.Settings["SelectedIndexOfFontSizeComboBox"].Value = SelectedIndexOfFontSizeComboBox.ToString();
        //    cfa.AppSettings.Settings["SelectedIndexOfFontFamilyComboBox"].Value = SelectedIndexOfFontFamilyComboBox.ToString();
        //    SaveColor();
        //    cfa.AppSettings.Settings["A"].Value = _A.ToString();
        //    cfa.AppSettings.Settings["R"].Value = _R.ToString();
        //    cfa.AppSettings.Settings["G"].Value = _G.ToString();
        //    cfa.AppSettings.Settings["B"].Value = _B.ToString();
        //    cfa.Save();
        //}
        public static XElement CreateXELementBySetting()
        {
            SaveColor();
            XElement rootNode = new XElement("SystemSetting");
            rootNode.Add(new XElement("LadderOriginScaleX", LadderOriginScaleX));
            rootNode.Add(new XElement("LadderOriginScaleY", LadderOriginScaleY));
            rootNode.Add(new XElement("LadderScaleX", LadderScaleX));
            rootNode.Add(new XElement("LadderScaleY", LadderScaleY));
            rootNode.Add(new XElement("FuncBlockFontSize", FuncBlockFontSize));
            rootNode.Add(new XElement("SelectedIndexOfFontSizeComboBox", SelectedIndexOfFontSizeComboBox));
            rootNode.Add(new XElement("SelectedIndexOfFontFamilyComboBox", SelectedIndexOfFontFamilyComboBox));
            rootNode.Add(new XElement("_A", _A));
            rootNode.Add(new XElement("_R", _R));
            rootNode.Add(new XElement("_G", _G));
            rootNode.Add(new XElement("_B", _B));
            return rootNode;
        }
        public static void LoadSystemSettingByXELement(XElement rootNode)
        {
            try
            {
                LadderOriginScaleX = double.Parse(rootNode.Element("LadderOriginScaleX").Value);
                LadderOriginScaleY = double.Parse(rootNode.Element("LadderOriginScaleY").Value);
                _loadScaleSuccessFlag = true;
            }
            catch (Exception exception)
            {
                _loadScaleSuccessFlag = false;
            }
            try
            {
                LadderScaleX = double.Parse(rootNode.Element("LadderScaleX").Value);
                LadderScaleY = double.Parse(rootNode.Element("LadderScaleY").Value);
            }
            catch (Exception exception)
            {
                LadderScaleX = 1;
                LadderScaleY = 1;
            }
            try
            {
                FuncBlockFontSize = int.Parse(rootNode.Element("FuncBlockFontSize").Value);
            }
            catch (Exception exception)
            {
                FuncBlockFontSize = 16;
            }
            try
            {
                SelectedIndexOfFontFamilyComboBox = int.Parse(rootNode.Element("SelectedIndexOfFontFamilyComboBox").Value);
            }
            catch (Exception)
            {
                SelectedIndexOfFontFamilyComboBox = 0;
            }
            try
            {
                SelectedIndexOfFontSizeComboBox = int.Parse(rootNode.Element("SelectedIndexOfFontSizeComboBox").Value);
            }
            catch (Exception)
            {
                SelectedIndexOfFontSizeComboBox = 10;
            }
            try
            {
                _A = byte.Parse(rootNode.Element("_A").Value);
                _R = byte.Parse(rootNode.Element("_R").Value);
                _G = byte.Parse(rootNode.Element("_G").Value);
                _B = byte.Parse(rootNode.Element("_B").Value);
                Color color = new Color();
                color.A = _A;
                color.R = _R;
                color.G = _G;
                color.B = _B;
                SelectColor = color;
            }
            catch (Exception)
            {
                SelectColor = Colors.Black;
            }
            FontManager.GetFontDataProvider().SelectFontSizeIndex = SelectedIndexOfFontSizeComboBox;
            FontManager.GetFontDataProvider().SelectFontFamilyIndex = SelectedIndexOfFontFamilyComboBox;
            FontManager.GetFontDataProvider().Initialize();
            ColorManager.GetColorDataProvider().SelectColor = new SolidColorBrush(SelectColor);
        }
        private static void SaveColor()
        {
            _A = SelectColor.A;
            _R = SelectColor.R;
            _G = SelectColor.G;
            _B = SelectColor.B;
        }
        public static bool LoadLadderScaleSuccess()
        {
            return _loadScaleSuccessFlag;
        }

        //public static void Load()
        //{
        //    try
        //    {
        //        LadderOriginScaleX = double.Parse(ConfigurationManager.AppSettings["LadderOriginScaleX"]);
        //        LadderOriginScaleY = double.Parse(ConfigurationManager.AppSettings["LadderOriginScaleY"]);
        //        _loadScaleSuccessFlag = true;
        //    }
        //    catch (Exception exception)
        //    {
        //        _loadScaleSuccessFlag = false;
        //    }
        //    try
        //    {
        //        LadderScaleX = double.Parse(ConfigurationManager.AppSettings["LadderScaleX"]);
        //        LadderScaleY = double.Parse(ConfigurationManager.AppSettings["LadderScaleY"]);
        //    }
        //    catch (Exception exception)
        //    {
        //        LadderScaleX = 1;
        //        LadderScaleY = 1;
        //    }
        //    try
        //    {
        //        FuncBlockFontSize = int.Parse(ConfigurationManager.AppSettings["FuncBlockFontSize"]);
        //    }
        //    catch (Exception exception)
        //    {
        //        FuncBlockFontSize = 16;
        //    }
        //    try
        //    {
        //        SelectedIndexOfFontFamilyComboBox = int.Parse(ConfigurationManager.AppSettings["SelectedIndexOfFontFamilyComboBox"]);
        //    }
        //    catch (Exception)
        //    {
        //        SelectedIndexOfFontFamilyComboBox = 0;
        //    }
        //    try
        //    {
        //        SelectedIndexOfFontSizeComboBox = int.Parse(ConfigurationManager.AppSettings["SelectedIndexOfFontSizeComboBox"]);
        //    }
        //    catch (Exception)
        //    {
        //        SelectedIndexOfFontSizeComboBox = 10;
        //    }
        //    try
        //    {
        //        _A = byte.Parse(ConfigurationManager.AppSettings["A"]);
        //        _R = byte.Parse(ConfigurationManager.AppSettings["R"]);
        //        _G = byte.Parse(ConfigurationManager.AppSettings["G"]);
        //        _B = byte.Parse(ConfigurationManager.AppSettings["B"]);
        //        Color color = new Color();
        //        color.A = _A;
        //        color.R = _R;
        //        color.G = _G;
        //        color.B = _B;
        //        SelectColor = color;
        //    }
        //    catch (Exception)
        //    {
        //        SelectColor = Colors.Black;
        //    }
        //    FontManager.GetFontDataProvider().SelectFontSizeIndex = SelectedIndexOfFontSizeComboBox;
        //    FontManager.GetFontDataProvider().SelectFontFamilyIndex = SelectedIndexOfFontFamilyComboBox;
        //    FontManager.GetFontDataProvider().Initialize();
        //    ColorManager.GetColorDataProvider().SelectColor = new SolidColorBrush(SelectColor);
        //}
    }
}


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
        public static int WholeWidth
        {
            get
            {
                return LadderWidthUnit * LadderXCapacity;
            }
        }
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
        public static bool IsSavedByTime { get; set; }
        public static int SaveTimeSpan { get; set; }
        
        public static XElement CreateXELementBySetting()
        {
            SaveColor();
            XElement rootNode = new XElement("SystemSetting");
            rootNode.Add(new XElement("LadderOriginScaleX", LadderOriginScaleX));
            rootNode.Add(new XElement("LadderOriginScaleY", LadderOriginScaleY));
            rootNode.Add(new XElement("LadderScaleX", LadderScaleX));
            rootNode.Add(new XElement("LadderScaleY", LadderScaleY));
            rootNode.Add(new XElement("FuncBlockFontSize", FuncBlockFontSize));
            rootNode.Add(new XElement("_A", _A));
            rootNode.Add(new XElement("_R", _R));
            rootNode.Add(new XElement("_G", _G));
            rootNode.Add(new XElement("_B", _B));
            rootNode.Add(new XElement("IsSavedByTime", IsSavedByTime));
            rootNode.Add(new XElement("SaveTimeSpan", SaveTimeSpan));
            XElement xele_font = new XElement("Font");
            rootNode.Add(xele_font);
            XElement xele = null;
            xele = new XElement("Comment");
            FontManager.SaveFontDataToXElement(
                FontManager.GetComment(), xele);
            xele_font.Add(xele);
            xele = new XElement("Func");
            FontManager.SaveFontDataToXElement(
                FontManager.GetFunc(), xele);
            xele_font.Add(xele);
            xele = new XElement("Inst");
            FontManager.SaveFontDataToXElement(
                FontManager.GetInst(), xele);
            xele_font.Add(xele);
            xele = new XElement("Ladder");
            FontManager.SaveFontDataToXElement(
                FontManager.GetLadder(), xele);
            xele_font.Add(xele);
            xele = new XElement("Title");
            FontManager.SaveFontDataToXElement(
                FontManager.GetTitle(), xele);
            xele_font.Add(xele);
            XElement xele_color = new XElement("Color");
            rootNode.Add(xele_color);
            xele = new XElement("Comment");
            ColorManager.SaveColorDataToXElement(
                ColorManager.GetComment(), xele);
            xele_color.Add(xele);
            xele = new XElement("DiagramTitle");
            ColorManager.SaveColorDataToXElement(
                ColorManager.GetDiagramTitle(), xele);
            xele_color.Add(xele);
            xele = new XElement("FuncScreen");
            ColorManager.SaveColorDataToXElement(
                ColorManager.GetFuncScreen(), xele);
            xele_color.Add(xele);
            xele = new XElement("Inst");
            ColorManager.SaveColorDataToXElement(
                ColorManager.GetInst(), xele);
            xele_color.Add(xele);
            xele = new XElement("InstScreen");
            ColorManager.SaveColorDataToXElement(
                ColorManager.GetInstScreen(), xele);
            xele_color.Add(xele);
            xele = new XElement("Ladder");
            ColorManager.SaveColorDataToXElement(
                ColorManager.GetLadder(), xele);
            xele_color.Add(xele);
            xele = new XElement("LadderScreen");
            ColorManager.SaveColorDataToXElement(
                ColorManager.GetLadderScreen(), xele);
            xele_color.Add(xele);
            xele = new XElement("NetworkTitle");
            ColorManager.SaveColorDataToXElement(
                ColorManager.GetNetworkTitle(), xele);
            xele_color.Add(xele);
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
            catch (Exception)
            {
                _loadScaleSuccessFlag = false;
            }
            try
            {
                LadderScaleX = double.Parse(rootNode.Element("LadderScaleX").Value);
                LadderScaleY = double.Parse(rootNode.Element("LadderScaleY").Value);
            }
            catch (Exception)
            {
                LadderScaleX = 1;
                LadderScaleY = 1;
            }
            try
            {
                FuncBlockFontSize = int.Parse(rootNode.Element("FuncBlockFontSize").Value);
            }
            catch (Exception)
            {
                FuncBlockFontSize = 16;
            }
            try
            {
                XElement xele_font = rootNode.Element("Font");
                FontManager.LoadFontDataByXElement(
                    FontManager.GetTitle(), 
                    xele_font.Element("Title"));
                FontManager.LoadFontDataByXElement(
                    FontManager.GetLadder(),
                    xele_font.Element("Ladder"));
                FontManager.LoadFontDataByXElement(
                    FontManager.GetComment(),
                    xele_font.Element("Comment"));
                FontManager.LoadFontDataByXElement(
                    FontManager.GetFunc(),
                    xele_font.Element("Func"));
                FontManager.LoadFontDataByXElement(
                    FontManager.GetInst(),
                    xele_font.Element("Inst"));
                XElement xele_color = rootNode.Element("Color");
                ColorManager.LoadColorDataByXElement(
                    ColorManager.GetLadderScreen(),
                    xele_color.Element("LadderScreen"));
                ColorManager.LoadColorDataByXElement(
                    ColorManager.GetLadder(),
                    xele_color.Element("Ladder"));
                ColorManager.LoadColorDataByXElement(
                    ColorManager.GetComment(),
                    xele_color.Element("Comment"));
                ColorManager.LoadColorDataByXElement(
                    ColorManager.GetDiagramTitle(),
                    xele_color.Element("DiagramTitle"));
                ColorManager.LoadColorDataByXElement(
                    ColorManager.GetNetworkTitle(),
                    xele_color.Element("NetworkTitle"));
                ColorManager.LoadColorDataByXElement(
                    ColorManager.GetFuncScreen(),
                    xele_color.Element("FuncScreen"));
                ColorManager.LoadColorDataByXElement(
                    ColorManager.GetInstScreen(),
                    xele_color.Element("InstScreen"));
                ColorManager.LoadColorDataByXElement(
                    ColorManager.GetInst(),
                    xele_color.Element("Inst"));
            }
            catch (Exception)
            {
                FontManager.GetTitle().FontSize = 32;
                FontManager.GetTitle().FontFamily = new FontFamily("微软雅黑");
                FontManager.GetTitle().FontColor = ColorManager.Parse("255 0 0 0");
                FontManager.GetLadder().FontSize = 24;
                FontManager.GetLadder().FontFamily = new FontFamily("Courier New");
                FontManager.GetLadder().FontColor = ColorManager.Parse("255 0 0 0");
                FontManager.GetComment().FontSize = 16;
                FontManager.GetComment().FontFamily = new FontFamily("微软雅黑");
                FontManager.GetComment().FontColor = ColorManager.Parse("255 0 0 0");
                FontManager.GetFunc().FontSize = 16;
                FontManager.GetFunc().FontFamily = new FontFamily("Courier New");
                FontManager.GetFunc().FontColor = ColorManager.Parse("255 0 0 0");
                FontManager.GetInst().FontSize = 20;
                FontManager.GetInst().FontFamily = new FontFamily("Courier New");
                FontManager.GetInst().FontColor = ColorManager.Parse("255 0 0 0");
                ColorManager.GetLadder().Background = ColorManager.Parse("255 255 255 255");
                ColorManager.GetLadder().Foreground = ColorManager.Parse("255 0 0 0");
                ColorManager.GetLadderScreen().Background = ColorManager.Parse("255 255 255 255");
                ColorManager.GetLadderScreen().Foreground = ColorManager.Parse("255 0 0 0");
                ColorManager.GetDiagramTitle().Background = ColorManager.Parse("255 210 32 32");
                ColorManager.GetDiagramTitle().Foreground = ColorManager.Parse("255 0 0 0");
                ColorManager.GetNetworkTitle().Background = ColorManager.Parse("255 32 210 32");
                ColorManager.GetNetworkTitle().Foreground = ColorManager.Parse("255 0 0 0");
                ColorManager.GetComment().Background = ColorManager.Parse("255 210 250 250");
                ColorManager.GetComment().Foreground = ColorManager.Parse("255 0 0 0");
                ColorManager.GetFuncScreen().Background = ColorManager.Parse("255 255 255 255");
                ColorManager.GetFuncScreen().Foreground = ColorManager.Parse("255 0 0 0");
                ColorManager.GetInst().Background = ColorManager.Parse("255 255 255 255");
                ColorManager.GetInst().Foreground = ColorManager.Parse("255 0 0 0");
                ColorManager.GetInstScreen().Background = ColorManager.Parse("255 255 255 255");
                ColorManager.GetInstScreen().Foreground = ColorManager.Parse("255 255 255 255");
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
            try
            {
                IsSavedByTime = bool.Parse(rootNode.Element("IsSavedByTime").Value);
                SaveTimeSpan = int.Parse(rootNode.Element("SaveTimeSpan").Value);
            }
            catch (Exception)
            {
                IsSavedByTime = false;
                SaveTimeSpan = 1;
            }
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
        
    }
}


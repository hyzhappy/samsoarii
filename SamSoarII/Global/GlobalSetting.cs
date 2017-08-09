using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Configuration;
using System.Xml.Linq;
using SamSoarII.Shell.Managers;
using System.Reflection;
using System.IO;

namespace SamSoarII.Global
{
    public class GlobalSetting
    {
        static GlobalSetting()
        {
            LadderScaleTransform = new ScaleTransform();
            Color color = new Color();
            color.A = 255;
            color.R = 60;
            color.G = 58;
            color.B = 58;
            FoldingBrush = new SolidColorBrush(color);
            //#FF4CB2FA
            color = new Color();
            color.A = 255;
            color.R = 0x4c;
            color.G = 0xb2;
            color.B = 0xfa;
            MonitorBrush = new SolidColorBrush(color);
            //#FFF46622
            color = new Color();
            color.A = 255;
            color.R = 0xf4;
            color.G = 0x66;
            color.B = 0x22;
            SimulateBrush = new SolidColorBrush(color);
            //#FFA92AD7
            color = new Color();
            color.A = 255;
            color.R = 0xa9;
            color.G = 0x2a;
            color.B = 0xd7;
            LoadingBrush = new SolidColorBrush(color);
        }

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
        public static bool IsSavedByTime { get; set; }
        public static int SaveTimeSpan { get; set; }
        public static bool IsInstByTime { get; set; }
        public static int InstTimeSpan { get; set; }
        public static bool IsOpenLSetting { get; set; }
        public static bool IsCheckCoil { get; set; }
        public static bool IsCheckTimer { get; set; }
        public static bool IsCheckCounter { get; set; }
        public static string LanagArea { get; set; }
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
            rootNode.Add(new XElement("IsInstByTime", IsInstByTime));
            rootNode.Add(new XElement("InstTimeSpan", InstTimeSpan));
            rootNode.Add(new XElement("IsCheckCoil", IsCheckCoil));
            rootNode.Add(new XElement("IsCheckTimer", IsCheckTimer));
            rootNode.Add(new XElement("IsCheckCounter", IsCheckCounter));
            rootNode.Add(new XElement("IsOpenLSetting", IsOpenLSetting));
            rootNode.Add(new XElement("LanagArea", LanagArea));
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
            xele = new XElement("Ladder");
            FontManager.SaveFontDataToXElement(
                FontManager.GetLadder(), xele);
            xele_font.Add(xele);
            xele = new XElement("Title");
            FontManager.SaveFontDataToXElement(
                FontManager.GetTitle(), xele);
            xele_font.Add(xele);
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
                XElement xele_color = rootNode.Element("Color");
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
            try
            {
                IsInstByTime = bool.Parse(rootNode.Element("IsInstByTime").Value);
                InstTimeSpan = int.Parse(rootNode.Element("InstTimeSpan").Value);

            }
            catch (Exception)
            {
                IsInstByTime = true;
                InstTimeSpan = 10;
            }
            try
            {
                IsOpenLSetting = bool.Parse(rootNode.Element("IsOpenLSetting").Value);
            }
            catch (Exception)
            {
                IsOpenLSetting = false;
            }
            try
            {
                IsCheckCoil = bool.Parse(rootNode.Element("IsCheckCoil").Value);
                IsCheckTimer = bool.Parse(rootNode.Element("IsCheckTimer").Value);
                IsCheckCounter = bool.Parse(rootNode.Element("IsCheckCounter").Value);
            }
            catch (Exception)
            {
                IsCheckCoil = false;
                IsCheckTimer = false;
                IsCheckCounter = false;
            }
            try
            {
                LanagArea = rootNode.Element("LanagArea").Value;
            }
            catch (Exception)
            {
                LanagArea = string.Empty;
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

        public static Dictionary<string, List<string>> InstrutionNameAndToolTips { get; private set; }
        public static void LoadInstrutionNameAndToolTips()
        {
            InstrutionNameAndToolTips = new Dictionary<string, List<string>>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("SamSoarII.Resources.InstructionPopup.xml");
            Dictionary<string, List<string>> tempDic = new Dictionary<string, List<string>>();
            XDocument xDoc = XDocument.Load(stream);
            XElement rootNode = xDoc.Root;
            List<XElement> nodes = rootNode.Elements().ToList();
            foreach (var node in nodes)
            {
                if (node.Name != "HLine" && node.Name != "VLine")
                {
                    List<string> tempList = new List<string>();
                    tempList.Add(node.Attribute("Describe").Value);
                    tempList.Add(node.Attribute("Text_1").Value);
                    tempList.Add(node.Attribute("Text_2").Value);
                    tempList.Add(node.Attribute("Text_3").Value);
                    tempList.Add(node.Attribute("Text_4").Value);
                    tempList.Add(node.Attribute("Text_5").Value);
                    tempList.Add(node.Attribute("Detail").Value);
                    InstrutionNameAndToolTips.Add(node.Name.ToString(), tempList);
                }
            }
        }
        
        public static SolidColorBrush FoldingBrush { get; private set; }
        public static SolidColorBrush SimulateBrush { get; private set; }
        public static SolidColorBrush MonitorBrush { get; private set; }
        public static SolidColorBrush LoadingBrush { get; private set; }
    }
}

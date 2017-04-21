using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Configuration;
using System.Xml.Linq;

namespace SamSoarII.Simulation.Core.Global
{
    public static class GlobalSetting
    {

        private const double ScaleMin = 0.4;
        private const double ScaleMax = 2.5;

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

        public static ScaleTransform RulerScaleTransform { get; private set; }

        public static double RulerScaleX
        {
            get { return RulerScaleTransform.ScaleX; }
            set { RulerScaleTransform.ScaleX = value; }
        }
        
        public static double RulerScaleY
        {
            get { return RulerScaleTransform.ScaleY; }
            set { RulerScaleTransform.ScaleY = value; }
        }

        public static int FuncBlockFontSize { get; set; }
        
        public const int SCANPERIOD_CONST = 0x00;
        public const int SCANPERIOD_RATE = 0x01;
        public static int ScanPeriodType { get; set; }
        public static double ScanPeriodConst { get; set; }
        public static double ScanPeriodRate { get; set; }
        public static int ScanDataMaximum { get; set; }
        public static int ScanLockMaximum { get; set; }
        public static int ScanViewMaximum { get; set; }

        public static int TimeRulerDivideNumber { get; set; }
        public static int TimeRulerSubDivideNumber { get; set; }
        public static int TimeRulerStart { get; set; }
        public static int TimeRulerEnd { get; set; }
        
        public static int DrawAccurate { get; set; }
        public static int DrawMaximum { get; set; }
        public static Brush[] DrawBrushes { get; set; }
        public static int DrawValueDivide { get; set; }
        public static int DrawValueSubDivide { get; set; }
        public static int DrawValueStart { get; set; }
        public static int DrawValueEnd { get; set; }
        
        static GlobalSetting()
        {
            LadderScaleTransform = new ScaleTransform();
            RulerScaleTransform = new ScaleTransform();
            DrawBrushes = new Brush[8];
            DrawBrushes[0] = Brushes.Black;
            DrawBrushes[1] = Brushes.DarkOrange;
            DrawBrushes[2] = Brushes.DarkBlue;
            DrawBrushes[3] = Brushes.BlueViolet;
            DrawBrushes[4] = Brushes.DarkCyan;
            DrawBrushes[5] = Brushes.DarkGray;
            DrawBrushes[6] = Brushes.DarkGoldenrod;
            DrawBrushes[7] = Brushes.DarkTurquoise;
            //double scale = 1024.0 * 749 / (267 + 749) / 3000 * 1.4;
            //LadderOriginScaleX = scale;
            //LadderOriginScaleY = scale;
        }
        
        //public static void Save()
        //{
        //    var cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    //cfa.AppSettings.Settings["LadderOriginScaleX"].Value = LadderOriginScaleX.ToString();
        //    //cfa.AppSettings.Settings["LadderOriginScaleY"].Value = LadderOriginScaleY.ToString();
        //    //cfa.AppSettings.Settings["LadderScaleX"].Value = LadderScaleX.ToString();
        //    //cfa.AppSettings.Settings["LadderScaleY"].Value = LadderScaleY.ToString();
        //    //cfa.AppSettings.Settings["FuncBlockFontSize"].Value = FuncBlockFontSize.ToString();
        //    cfa.AppSettings.Settings["ScanPeriodType"].Value = ScanPeriodType.ToString();
        //    cfa.AppSettings.Settings["ScanPeriodConst"].Value = ScanPeriodConst.ToString();
        //    cfa.AppSettings.Settings["ScanPeriodRate"].Value = ScanPeriodRate.ToString();
        //    cfa.AppSettings.Settings["ScanDataMaximum"].Value = ScanDataMaximum.ToString();
        //    cfa.AppSettings.Settings["ScanLockMaximum"].Value = ScanLockMaximum.ToString();
        //    cfa.AppSettings.Settings["ScanViewMaximum"].Value = ScanViewMaximum.ToString();
        //    cfa.AppSettings.Settings["TimeRulerDivideNumber"].Value = TimeRulerDivideNumber.ToString();
        //    cfa.AppSettings.Settings["TimeRulerSubDivideNumber"].Value = TimeRulerSubDivideNumber.ToString();
        //    cfa.AppSettings.Settings["TimeRulerStart"].Value = TimeRulerStart.ToString();
        //    cfa.AppSettings.Settings["TimeRulerEnd"].Value = TimeRulerEnd.ToString();
        //    cfa.AppSettings.Settings["DrawAccurate"].Value = DrawAccurate.ToString();
        //    cfa.AppSettings.Settings["DrawMaximum"].Value = DrawMaximum.ToString();
        //    cfa.AppSettings.Settings["DrawValueStart"].Value = DrawValueStart.ToString();
        //    cfa.AppSettings.Settings["DrawValueEnd"].Value = DrawValueEnd.ToString();
        //    cfa.AppSettings.Settings["DrawValueDivide"].Value = DrawValueDivide.ToString();
        //    cfa.AppSettings.Settings["DrawValueSubDivide"].Value = DrawValueSubDivide.ToString();
        //    for (int i = 0; i < 8; i++)
        //    {
        //        cfa.AppSettings.Settings[String.Format("Brush{0:d}", i+1)].Value = DrawBrushes[i].ToString();
        //    }
        //    cfa.Save();
        //}
        public static XElement CreateXELementBySetting()
        {
            XElement rootNode = new XElement("SimulateSetting");
            rootNode.Add(new XElement("ScanPeriodType", ScanPeriodType));
            rootNode.Add(new XElement("ScanPeriodConst", ScanPeriodConst));
            rootNode.Add(new XElement("ScanPeriodRate", ScanPeriodRate));
            rootNode.Add(new XElement("ScanDataMaximum", ScanDataMaximum));
            rootNode.Add(new XElement("ScanLockMaximum", ScanLockMaximum));
            rootNode.Add(new XElement("ScanViewMaximum", ScanViewMaximum));
            rootNode.Add(new XElement("TimeRulerDivideNumber", TimeRulerDivideNumber));
            rootNode.Add(new XElement("TimeRulerSubDivideNumber", TimeRulerSubDivideNumber));
            rootNode.Add(new XElement("TimeRulerStart", TimeRulerStart));
            rootNode.Add(new XElement("TimeRulerEnd", TimeRulerEnd));
            rootNode.Add(new XElement("DrawAccurate", DrawAccurate));
            rootNode.Add(new XElement("DrawMaximum", DrawMaximum));
            rootNode.Add(new XElement("DrawValueStart", DrawValueStart));
            rootNode.Add(new XElement("DrawValueEnd", DrawValueEnd));
            rootNode.Add(new XElement("DrawValueDivide", DrawValueDivide));
            rootNode.Add(new XElement("DrawValueSubDivide", DrawValueSubDivide));
            for (int i = 0; i < 8; i++)
            {
                rootNode.Add(new XElement(string.Format("Brush{0:d}", i + 1), DrawBrushes[i]));
            }
            return rootNode;
        }
        public static void LoadSystemSettingByXELement(XElement selfNode,XElement mainNode)
        {
            try
            {
                LadderOriginScaleX = double.Parse(mainNode.Element("LadderOriginScaleX").Value);
                LadderOriginScaleY = double.Parse(mainNode.Element("LadderOriginScaleY").Value);
                _loadScaleSuccessFlag = true;
            }
            catch (Exception exception)
            {
                _loadScaleSuccessFlag = false;
            }

            try
            {
                LadderScaleX = double.Parse(mainNode.Element("LadderScaleX").Value);
                LadderScaleY = double.Parse(mainNode.Element("LadderScaleY").Value);
            }
            catch (Exception exception)
            {
                LadderScaleX = 1;
                LadderScaleY = 1;
            }

            try
            {
                FuncBlockFontSize = int.Parse(mainNode.Element("FuncBlockFontSize").Value);
            }
            catch (Exception exception)
            {
                FuncBlockFontSize = 16;
            }

            try
            {
                ScanPeriodType = int.Parse(selfNode.Element("ScanPeriodType").Value);
                ScanPeriodConst = double.Parse(selfNode.Element("ScanPeriodConst").Value);
                ScanPeriodRate = double.Parse(selfNode.Element("ScanPeriodRate").Value);
            }
            catch (Exception e)
            {
                ScanPeriodType = SCANPERIOD_CONST;
                ScanPeriodConst = 1;
                ScanPeriodRate = 400;
            }

            try
            {
                ScanDataMaximum = int.Parse(selfNode.Element("ScanDataMaximum").Value);
                ScanLockMaximum = int.Parse(selfNode.Element("ScanLockMaximum").Value);
                ScanViewMaximum = int.Parse(selfNode.Element("ScanViewMaximum").Value);
            }
            catch (Exception e)
            {
                ScanDataMaximum = 65536;
                ScanLockMaximum = 8;
                ScanViewMaximum = 8;
            }

            try
            {
                TimeRulerDivideNumber = int.Parse(selfNode.Element("TimeRulerDivideNumber").Value);
                TimeRulerSubDivideNumber = int.Parse(selfNode.Element("TimeRulerSubDivideNumber").Value);

            }
            catch (Exception e)
            {
                TimeRulerDivideNumber = 20;
                TimeRulerSubDivideNumber = 5;
            }

            try
            {
                TimeRulerStart = int.Parse(selfNode.Element("TimeRulerStart").Value);
                TimeRulerEnd = int.Parse(selfNode.Element("TimeRulerEnd").Value);
            }
            catch (Exception e)
            {
                TimeRulerStart = 0;
                TimeRulerEnd = 800;
            }

            try
            {
                DrawValueStart = int.Parse(selfNode.Element("DrawValueStart").Value);
                DrawValueEnd = int.Parse(selfNode.Element("DrawValueEnd").Value);
                DrawValueDivide = int.Parse(selfNode.Element("DrawValueDivide").Value);
                DrawValueSubDivide = int.Parse(selfNode.Element("DrawValueSubDivide").Value);
            }
            catch (Exception e)
            {
                DrawValueStart = 0;
                DrawValueEnd = 1000;
                DrawValueDivide = 50;
                DrawValueSubDivide = 2;
            }

            try
            {
                DrawAccurate = int.Parse(selfNode.Element("DrawAccurate").Value);
            }
            catch (Exception e)
            {
                DrawAccurate = 1024;
            }

            try
            {
                DrawMaximum = int.Parse(selfNode.Element("DrawMaximum").Value);
            }
            catch (Exception e)
            {
                DrawMaximum = 8;
            }
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
        //    catch(Exception exception)
        //    {
        //        _loadScaleSuccessFlag = false;
        //    }

        //    try
        //    {
        //        LadderScaleX = double.Parse(ConfigurationManager.AppSettings["LadderScaleX"]);
        //        LadderScaleY = double.Parse(ConfigurationManager.AppSettings["LadderScaleY"]);
        //    }
        //    catch(Exception exception)
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
        //        ScanPeriodType = int.Parse(ConfigurationManager.AppSettings["ScanPeriodType"]);
        //        ScanPeriodConst = double.Parse(ConfigurationManager.AppSettings["ScanPeriodConst"]);
        //        ScanPeriodRate = double.Parse(ConfigurationManager.AppSettings["ScanPeriodRate"]);
        //    }
        //    catch (Exception e)
        //    {
        //        ScanPeriodType = SCANPERIOD_CONST;
        //        ScanPeriodConst = 1;
        //        ScanPeriodRate = 400;
        //    }

        //    try
        //    {
        //        ScanDataMaximum = int.Parse(ConfigurationManager.AppSettings["ScanDataMaximum"]);
        //        ScanLockMaximum = int.Parse(ConfigurationManager.AppSettings["ScanLockMaximum"]);
        //        ScanViewMaximum = int.Parse(ConfigurationManager.AppSettings["ScanViewMaximum"]);
        //    }
        //    catch (Exception e)
        //    {
        //        ScanDataMaximum = 65536;
        //        ScanLockMaximum = 8;
        //        ScanViewMaximum = 8;
        //    }

        //    try
        //    {
        //        TimeRulerDivideNumber = int.Parse(ConfigurationManager.AppSettings["TimeRulerDivideNumber"]);
        //        TimeRulerSubDivideNumber = int.Parse(ConfigurationManager.AppSettings["TimeRulerSubDivideNumber"]);

        //    }
        //    catch (Exception e)
        //    {
        //        TimeRulerDivideNumber = 20;
        //        TimeRulerSubDivideNumber = 5;
        //    }

        //    try
        //    {
        //        TimeRulerStart = int.Parse(ConfigurationManager.AppSettings["TimeRulerStart"]);
        //        TimeRulerEnd = int.Parse(ConfigurationManager.AppSettings["TimeRulerEnd"]);
        //    }
        //    catch (Exception e)
        //    {
        //        TimeRulerStart = 0;
        //        TimeRulerEnd = 800;
        //    }

        //    try
        //    {
        //        DrawValueStart = int.Parse(ConfigurationManager.AppSettings["DrawValueStart"]);
        //        DrawValueEnd = int.Parse(ConfigurationManager.AppSettings["DrawValueEnd"]);
        //        DrawValueDivide =int.Parse(ConfigurationManager.AppSettings["DrawValueDivide"]);
        //        DrawValueSubDivide = int.Parse(ConfigurationManager.AppSettings["DrawValueSubDivide"]);
        //    }
        //    catch (Exception e)
        //    {
        //        DrawValueStart = 0;
        //        DrawValueEnd = 1000;
        //        DrawValueDivide = 50;
        //        DrawValueSubDivide = 2;
        //    }

        //    try
        //    {
        //        DrawAccurate = int.Parse(ConfigurationManager.AppSettings["DrawAccurate"]);
        //    }
        //    catch (Exception e)
        //    {
        //        DrawAccurate = 1024;
        //    }

        //    try
        //    {
        //        DrawMaximum = int.Parse(ConfigurationManager.AppSettings["DrawMaximum"]);
        //    }
        //    catch (Exception e)
        //    {
        //        DrawMaximum = 8;
        //    }
            
        //}
    }
}

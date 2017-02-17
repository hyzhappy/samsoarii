using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Configuration;
namespace SamSoarII.AppMain
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
                if(value > ScaleMin && value < ScaleMax)
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
                if(value > ScaleMin && value < ScaleMax)
                {
                    _ladderScaleY = value;
                    LadderScaleTransform.ScaleY = _ladderOriginScaleY * _ladderScaleY;
                }
            }
        }
        private static bool _loadScaleSuccessFlag;

        public static ScaleTransform LadderScaleTransform { get; private set; }

        public static int FuncBlockFontSize { get; set; }

        static GlobalSetting()
        {
            LadderScaleTransform = new ScaleTransform();
        }
     

        public static void Save()
        {
            var cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["LadderOriginScaleX"].Value = LadderOriginScaleX.ToString();
            cfa.AppSettings.Settings["LadderOriginScaleY"].Value = LadderOriginScaleY.ToString();
            cfa.AppSettings.Settings["LadderScaleX"].Value = LadderScaleX.ToString();
            cfa.AppSettings.Settings["LadderScaleY"].Value = LadderScaleY.ToString();
            cfa.AppSettings.Settings["FuncBlockFontSize"].Value = FuncBlockFontSize.ToString();
            cfa.Save();
        }

        public static bool LoadLadderScaleSuccess()
        {
            return _loadScaleSuccessFlag;
        }

        public static void Load()
        {
            try
            {
                LadderOriginScaleX = double.Parse(ConfigurationManager.AppSettings["LadderOriginScaleX"]);
                LadderOriginScaleY = double.Parse(ConfigurationManager.AppSettings["LadderOriginScaleY"]);       
                _loadScaleSuccessFlag = true;
            }
            catch(Exception exception)
            {
                _loadScaleSuccessFlag = false;
            }

            try
            {
                LadderScaleX = double.Parse(ConfigurationManager.AppSettings["LadderScaleX"]);
                LadderScaleY = double.Parse(ConfigurationManager.AppSettings["LadderScaleY"]);
            }
            catch(Exception exception)
            {
                LadderScaleX = 1;
                LadderScaleY = 1;
            }

            try
            {
                FuncBlockFontSize = int.Parse(ConfigurationManager.AppSettings["FuncBlockFontSize"]);
            }
            catch (Exception exception)
            {
                FuncBlockFontSize = 16;
            }              
        }
    }
}

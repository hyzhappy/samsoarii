using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SamSoarII.AppMain.UI.Style
{
    public class IconManager
    {
        public static string RoutineImage;
        public static string FuncImage;
        public static string ModbusImage;
        static IconManager()
        {
            RoutineImage = "pack://application:,,,/SamSoarII;component/Resources/Image/MainStyle/Routines.png";
            FuncImage = "pack://application:,,,/SamSoarII;component/Resources/Image/MainStyle/FUNC.png";
            ModbusImage = "pack://application:,,,/SamSoarII;component/Resources/Image/MainStyle/ModBusTable.png";
        }
    }
}

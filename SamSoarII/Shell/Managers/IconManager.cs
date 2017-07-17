using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Shell.Managers
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

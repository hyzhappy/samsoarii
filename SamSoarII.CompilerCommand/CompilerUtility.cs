using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.CompilerCommand
{
    static class CompilerUtility
    {
        public static string CrossARM_CC = "arm-none-eabi-gcc";
        public static string CrossARM_CXX = "arm-none-eabi-g++";
        public static string CrossARM_Objcopy = "arm-none-eabi-objcopy";
        public static string CrossARM_Dir = string.Format(@"{0}\Toolkit\CrossCompiler\bin", FileHelper.AppRootPath);

        public static string PC_CC = "gcc";
        public static string PC_CXX = "g++";
        public static string PC_Dir = string.Format(@"{0}\Toolkit\LocalCompiler\bin", FileHelper.AppRootPath);

        public static string FGs16MR_Base_Dir = string.Format(@"{0}\Toolkit\Firmware\FGs16MR", FileHelper.AppRootPath);
        public static string FGs16MT_Base_Dir = string.Format(@"{0}\Toolkit\Firmware\FGs16MT", FileHelper.AppRootPath);
    }
}

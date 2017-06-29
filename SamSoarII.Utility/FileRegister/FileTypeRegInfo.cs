using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Utility.FileRegister
{
    public class FileTypeRegInfo
    {
        public string ExtendName;

        public string Description;

        public string IconPath;

        public string ExePath;

        public FileTypeRegInfo() { }

        public FileTypeRegInfo(string extendName)
        {
            ExtendName = extendName;
        }
    }
}

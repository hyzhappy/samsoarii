using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSoarII.AppMain.Project
{
    public class FuncBlockModel
    {
        public string Name { get; set; }

        public string Code
        {
            get;
            set;
        }
        public FuncBlockModel(string name)
        {
            Name = name;
            Code = string.Format("void {0}()\r\n{{\r\n\r\n}}\r\n", name);
        }      
    }
}

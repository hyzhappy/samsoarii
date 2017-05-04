using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SamSoarII.Simulation.UI.Base;

namespace SamSoarII.Simulation.UI
{
    public class FuncHeaderModel
    {
        public string Name { get; set; }

        public string ReturnType { get; set; }

        private string[] argtypes = new string[0];

        private string[] argnames = new string[0];

        public int ArgCount
        {
            get
            {
                return argnames.Length;
            }
            set
            {
                argnames = new string[value];
                argtypes = new string[value];
            }
        }
        
        public string GetArgType(int id)
        {
            return argtypes[id];
        }

        public void SetArgType(int id, string value)
        {
            argtypes[id] = value;
        }

        public string GetArgName(int id)
        {
            return argnames[id];
        }

        public void SetArgName(int id, string value)
        {
            argnames[id] = value;
        }
    }

    /// <summary>
    /// SimuViewFuncBlockModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewFuncBlockModel : SimuViewTabModel
    {
        public SimuViewFuncBlockModel(string name)
        {
            InitializeComponent();
            FuncBlockName = name;
            CodeTextBox.DataContext = this;
        }

        public string FuncBlockName { get; set; }

        public List<FuncHeaderModel> Headers { get; private set; } 
            = new List<FuncHeaderModel>();

        public string Code
        {
            get
            {
                return CodeTextBox.Text;
            }
            set
            {
                CodeTextBox.Text = value;
            }

        }

        public void GenerateCHeader(StreamWriter sw)
        {
            sw.Write("#include<stdint.h>\r\n");
            sw.Write("typedef int32_t _BIT;\r\n");
            sw.Write("typedef int32_t _WORD;\r\n");
            sw.Write("typedef int64_t D_WORD;\r\n");
            sw.Write("typedef double  _FLOAT;\r\n");
            
            foreach (FuncHeaderModel header in Headers)
            {
                if (header.ArgCount == 0)
                {
                    sw.Write("{0:s} {1:s}();",
                        header.ReturnType, header.Name);
                }
                else
                {
                    sw.Write("{0:s} {1:s}({2:s} {3:s}",
                        header.ReturnType, header.Name,
                        header.GetArgType(0), header.GetArgName(0));
                    for (int i = 1; i < header.ArgCount; i++)
                    {
                        sw.Write(",{0:s} {1:s}",
                            header.GetArgType(i),
                            header.GetArgName(i));
                    }
                    sw.Write(");\r\n");
                }
            }
        }

        public void GenerateCCode(StreamWriter sw)
        {
            sw.Write("#include \"simuf.h\"\r\n");
            sw.Write(Code);
        }
        
    }

}

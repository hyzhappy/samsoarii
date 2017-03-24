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

namespace SamSoarII.Simulation.UI
{
    /// <summary>
    /// SimuViewFuncBlockModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewFuncBlockModel : UserControl
    {
        public SimuViewFuncBlockModel(string name)
        {
            InitializeComponent();
            FuncBlockName = name;
            CodeTextBox.DataContext = this;
        }

        public string FuncBlockName { get; set; }

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
            sw.Write("#include<minwindef.h>\r\n");
            string pattern = @"void \w+(WORD W, BIT B)";
            foreach (Match match in Regex.Matches(Code, pattern))
            {
                sw.Write("{0:s};\r\n", match.Value);
            }
        }

        public void GenerateCCode(StreamWriter sw)
        {
            sw.Write("#include \"simuf.h\"\r\n");
            sw.Write(Code);
        }
        
    }
}

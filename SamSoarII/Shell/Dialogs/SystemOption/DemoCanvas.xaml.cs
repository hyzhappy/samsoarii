using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// DemoCanvas.xaml 的交互逻辑
    /// </summary>
    public partial class DemoCanvas : UserControl
    {
        public DemoCanvas()
        {
            InitializeComponent();
            diagram = new DemoLadderViewModel();
            func = new DemoFuncViewModel();
            func.CodeTextBox.Text = "void func1(WORD* w1, BIT* b2)\n{\tw1[2] = w1[0] + w1[1];\n}\n";
            ShowDiagram();
        }

        private DemoLadderViewModel diagram;
        private DemoFuncViewModel func;

        public void ShowDiagram()
        {
            GD_Main.Children.Clear();
            GD_Main.Children.Add(diagram);
        }
        public void ShowFuncBlock()
        {
            GD_Main.Children.Clear();
            GD_Main.Children.Add(func);
        }
    }
}

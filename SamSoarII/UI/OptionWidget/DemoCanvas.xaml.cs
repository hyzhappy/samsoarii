using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using SamSoarII.LadderInstViewModel.Auxiliar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// DemoCanvas.xaml 的交互逻辑
    /// </summary>
    public partial class DemoCanvas : UserControl
    {
        private ProjectModel pmodel;
        private LadderDiagramViewModel ldvmodel;
        private LadderNetworkViewModel lnvmodel;
        private FuncBlockViewModel fbvmodel;
        private DemoInstructionViewModel divmodel;

        public DemoCanvas()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            pmodel = new ProjectModel("效果展示");
            ldvmodel = new LadderDiagramViewModel("效果展示", pmodel);
            lnvmodel = ldvmodel.GetNetworks().First();
            fbvmodel = new FuncBlockViewModel("效果展示");
            divmodel = new DemoInstructionViewModel();
            ldvmodel.LadderMode = LadderMode.Demo;
            ldvmodel.Visibility = Visibility.Hidden;
            fbvmodel.Visibility = Visibility.Hidden;
            divmodel.Visibility = Visibility.Hidden;
            
            //ldvmodel.AddNetwork(lnvmodel, 0);
            GD_Main.Children.Add(ldvmodel);
            GD_Main.Children.Add(fbvmodel);
            GD_Main.Children.Add(divmodel);

            LDViewModel vmodel1 = new LDViewModel();
            vmodel1.X = 0;
            vmodel1.Y = 0;
            vmodel1.ParseValue(new string[]{"X0"});
            vmodel1.IsCommentMode = true;
            lnvmodel.ReplaceElement(vmodel1);

            LDWEQViewModel vmodel2 = new LDWEQViewModel();
            vmodel2.X = 1;
            vmodel2.Y = 0;
            vmodel2.ParseValue(new string[] { "D0", "D1" });
            vmodel2.IsCommentMode = true;
            lnvmodel.ReplaceElement(vmodel2);

            SMOVViewModel vmodel3 = new SMOVViewModel();
            vmodel3.X = 2;
            vmodel3.Y = 0;
            vmodel3.ParseValue(new string[] { "D0", "K1", "K2", "D1", "K3" });
            vmodel3.IsCommentMode = true;
            lnvmodel.ReplaceElement(vmodel3);

            fbvmodel.Code = "void Func(WORD* w1, WORD* w2, WORD* w3)\n{\n\tw3[0] = w1[0] + w2[0];\n}\n";
        }

        public void ShowDiagram()
        {
            ldvmodel.Visibility = Visibility.Visible;
            fbvmodel.Visibility = Visibility.Hidden;
            divmodel.Visibility = Visibility.Hidden;
        }

        public void ShowFuncBlock()
        {
            ldvmodel.Visibility = Visibility.Hidden;
            fbvmodel.Visibility = Visibility.Visible;
            divmodel.Visibility = Visibility.Hidden;
        }

        public void ShowInstruction()
        {
            ldvmodel.Visibility = Visibility.Hidden;
            fbvmodel.Visibility = Visibility.Hidden;
            divmodel.Visibility = Visibility.Visible;
        }
    }
}

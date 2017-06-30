using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI.OptionWidget.DemoViewModel;
using SamSoarII.LadderInstViewModel;
using SamSoarII.LadderInstViewModel.Auxiliar;
using SamSoarII.Utility;
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
            
            ldvmodel.LadderMode = LadderMode.Demo;
            ldvmodel.Visibility = Visibility.Hidden;
            GD_Main.Children.Add(ldvmodel);

            LDDemoViewModel ldmodel = new LDDemoViewModel();
            LDWEQDemoViewModel ldweqmodel = new LDWEQDemoViewModel();
            SMOVDemoViewModel smovmodel = new SMOVDemoViewModel();
            lnvmodel.LadderCanvas.Children.Add(ldmodel);
            lnvmodel.LadderCanvas.Children.Add(ldweqmodel);
            lnvmodel.LadderCanvas.Children.Add(smovmodel);

            lnvmodel.CommentAreaExpander.IsEnabled = false;
            lnvmodel.ladderExpander.IsEnabled = false;
            lnvmodel.LadderCanvas.IsEnabled = false;
            ldvmodel.CommentAreaExpander.IsEnabled = false;
            ldvmodel.ladderExpander.IsEnabled = false;
            ldvmodel.IsCommentMode = true;

            fbvmodel = new FuncBlockViewModel("效果展示", null);
            fbvmodel.Code = "void Func(WORD* w1, WORD* w2, WORD* w3)\n{\n\tw3[0] = w1[0] + w2[0];\n}\n";
            fbvmodel.CodeTextBox.IsReadOnly = true;
            GD_Main.Children.Add(fbvmodel);
            fbvmodel.Visibility = Visibility.Hidden;
        }
        public void SetFontColor(Brush brush,int selectIndex)
        {
            switch (selectIndex)
            {
                case 1:
                    ldvmodel.LadderTitleTextBlock.Foreground = brush;
                    lnvmodel.NetworkNumberLabel.Foreground = brush;
                    lnvmodel.NetworkStepCountLabel.Foreground = brush;
                    break;
                case 3:
                    fbvmodel.CodeTextBox.Foreground = brush;
                    break;
                default:
                    break;
            }
        }
        public void SetFontSize(int size,int selectIndex)
        {
            switch (selectIndex)
            {
                case 1:
                    ldvmodel.LadderTitleTextBlock.FontSize = size;
                    lnvmodel.NetworkNumberLabel.FontSize = size;
                    lnvmodel.NetworkStepCountLabel.FontSize = size;
                    break;
                case 3:
                    fbvmodel.CodeTextBox.FontSize = size;
                    break;
                default:
                    break;
            }
        }
        public void SetFontFamily(string FamilyName,int selectIndex)
        {
            switch (selectIndex)
            {
                case 1:
                    ldvmodel.LadderTitleTextBlock.FontFamily = new FontFamily(FamilyName);
                    lnvmodel.NetworkNumberLabel.FontFamily = new FontFamily(FamilyName);
                    lnvmodel.NetworkStepCountLabel.FontFamily = new FontFamily(FamilyName);
                    break;
                case 3:
                    fbvmodel.CodeTextBox.FontFamily = new FontFamily(FamilyName);
                    break;
                default:
                    break;
            }
        }
        public void ShowDiagram()
        {
            ldvmodel.Visibility = Visibility.Visible;
            fbvmodel.Visibility = Visibility.Hidden;
        }
        public void ShowFuncBlock()
        {
            ldvmodel.Visibility = Visibility.Hidden;
            fbvmodel.Visibility = Visibility.Visible;
        }
    }
}

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

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// InstructionDiagramViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class InstructionDiagramViewModel : UserControl
    {
        List<InstructionNetworkViewModel> invmodels;

        public InstructionDiagramViewModel()
        {
            InitializeComponent();
            invmodels = new List<InstructionNetworkViewModel>();
        }
        
        public void Setup(LadderDiagramViewModel ldvmodel)
        {
            invmodels.Clear();
            MainStack.Children.Clear();
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                InstructionNetworkViewModel invmodel = new InstructionNetworkViewModel();
                invmodel.Setup(lnvmodel);
                MainStack.Children.Add(invmodel);
                invmodels.Add(invmodel);
            }
        }

        public void Setup(LadderNetworkViewModel lnvmodel)
        {
            invmodels.Clear();
            InstructionNetworkViewModel invmodel = new InstructionNetworkViewModel();
            invmodel.Setup(lnvmodel);
            MainStack.Children.Add(invmodel);
            invmodels.Add(invmodel);
        }
    }
}

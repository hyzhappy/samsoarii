using SamSoarII.LadderInstViewModel;
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
        protected LadderDiagramViewModel ldvmodel;

        protected List<InstructionNetworkViewModel> invmodels;

        protected Dictionary<LadderNetworkViewModel, InstructionNetworkViewModel> invmodeldict;
        
        public InstructionDiagramViewModel()
        {
            InitializeComponent();
            ldvmodel = null;
            invmodels = new List<InstructionNetworkViewModel>();
            invmodeldict = new Dictionary<LadderNetworkViewModel, InstructionNetworkViewModel>();
        }
        
        public void Setup(LadderDiagramViewModel _ldvmodel)
        {
            if (ldvmodel != _ldvmodel)
            {
                invmodeldict.Clear();
                if (ldvmodel != null)
                {
                    ldvmodel.SelectionRect.NetworkParentChanged -= OnLadderCursorChanged;
                    ldvmodel.SelectionRect.XChanged -= OnLadderCursorChanged;
                    ldvmodel.SelectionRect.YChanged -= OnLadderCursorChanged;
                }
                if (_ldvmodel != null)
                {
                    _ldvmodel.SelectionRect.NetworkParentChanged += OnLadderCursorChanged;
                    _ldvmodel.SelectionRect.XChanged += OnLadderCursorChanged;
                    _ldvmodel.SelectionRect.YChanged += OnLadderCursorChanged;
                }
            }
            this.ldvmodel = _ldvmodel;
            invmodels.Clear();
            MainStack.Children.Clear();
            foreach (LadderNetworkViewModel lnvmodel in ldvmodel.GetNetworks())
            {
                InstructionNetworkViewModel invmodel = null;
                if (invmodeldict.ContainsKey(lnvmodel))
                {
                    invmodel = invmodeldict[lnvmodel];
                }
                else
                {
                    invmodel = new InstructionNetworkViewModel();
                    invmodel.Setup(lnvmodel);
                    invmodeldict.Add(lnvmodel, invmodel);
                }
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

        #region Event Handler

        private void OnLadderCursorChanged(object sender, RoutedEventArgs e)
        {
            if (sender is SelectRect)
            {
                SelectRect sr = (SelectRect)(sender);
                if (sr == null) return;
                BaseViewModel bvmodel = sr.CurrentElement;
                if (bvmodel == null) return;
                double currenty = 0; 
                foreach (InstructionNetworkViewModel invmodel in invmodels)
                {
                    if (invmodel.CatchCursor(bvmodel))
                    {
                        double cursory = 20 + Grid.GetRow(invmodel.Cursor) * 20;
                        Scroll.ScrollToVerticalOffset(Math.Max(0, currenty + cursory - Scroll.ViewportHeight / 2));
                    }
                    currenty += invmodel.ActualHeight;
                }
            }
        }
        #endregion
    }
}

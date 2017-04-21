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

        protected InstructionNetworkViewModel invmodelcursor;
        
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
                    invmodel.CursorChanged += OnNetworkCursorChanged;
                    invmodel.CursorEdit += OnNetworkCursorEdit;
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

        public event RoutedEventHandler CursorChanged = delegate { };
        private void OnNetworkCursorChanged(object sender, RoutedEventArgs e)
        {
            if (sender is InstructionNetworkViewModel)
            {
                if (invmodelcursor != null && invmodelcursor != sender)
                {
                    invmodelcursor.Cursor.Visibility = Visibility.Hidden;
                }
                invmodelcursor = (InstructionNetworkViewModel)(sender);
                CursorChanged(sender, e);
            }
        }

        public event RoutedEventHandler CursorEdit = delegate { };
        private void OnNetworkCursorEdit(object sender, RoutedEventArgs e)
        {
            if (sender is InstructionNetworkViewModel)
            {
                CursorEdit(sender, e);
            }
        }
        
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && invmodelcursor != null)
            {
                if (invmodelcursor.CursorUp())
                {
                    CursorChanged(invmodelcursor, new RoutedEventArgs());
                    return;
                }
                int cursorpos = invmodels.IndexOf(invmodelcursor);
                while (cursorpos > 0 && !invmodels[cursorpos - 1].CatchCursorBottom())
                {
                    cursorpos--;
                }
                if (cursorpos > 0)
                {
                    invmodelcursor.Cursor.Visibility = Visibility.Hidden;
                    invmodelcursor = invmodels[cursorpos - 1];
                    CursorChanged(invmodelcursor, new RoutedEventArgs());
                }
            }
            if (e.Key == Key.Down && invmodelcursor != null)
            {
                if (invmodelcursor.CursorDown())
                {
                    CursorChanged(invmodelcursor, new RoutedEventArgs());
                    return;
                }
                int cursorpos = invmodels.IndexOf(invmodelcursor);
                while (cursorpos < invmodels.Count()-1 && !invmodels[cursorpos + 1].CatchCursorTop())
                {
                    cursorpos++;
                }
                if (cursorpos < invmodels.Count()-1)
                {
                    invmodelcursor.Cursor.Visibility = Visibility.Hidden;
                    invmodelcursor = invmodels[cursorpos + 1];
                    CursorChanged(invmodelcursor, new RoutedEventArgs());
                }
            }
        }
        #endregion

    }
}

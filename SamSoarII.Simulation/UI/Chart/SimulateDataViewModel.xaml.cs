using SamSoarII.Simulation.Core.DataModel;
using System;
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

namespace SamSoarII.Simulation.UI.Chart
{
    /// <summary>
    /// SimulateDataViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimulateDataViewModel : UserControl
    {
        private SimulateDataModel sdmodel;
        
        public SimulateDataModel SDModel
        {
            get { return this.sdmodel; }
            set { Setup(value); }
        }

        public SimulateDataViewModel()
        {
            InitializeComponent();
            sdmodel = null;
            MainCanva.Children.Remove(NameTextBlock);
            LockFlag.Visibility = Visibility.Hidden;
            ViewFlag.Visibility = Visibility.Hidden;
            NameTextInput.KeyUp += OnInputKeyUp;
        }
        
        public SimulateDataViewModel(SimulateDataModel _sdmodel)
        {
            InitializeComponent();
            Setup(_sdmodel);
        }
        
        public void Setup(SimulateDataModel _sdmodel)
        {
            if (MainCanva.Children.Contains(NameTextInput))
            {
                MainCanva.Children.Remove(NameTextInput);
            }
            if (!MainCanva.Children.Contains(NameTextBlock))
            {
                MainCanva.Children.Add(NameTextBlock);
            }
            sdmodel = _sdmodel;
            NameTextBlock.Text = String.Format("{0:s}({1:s})", sdmodel.Name, sdmodel.Var);
            if (sdmodel.IsLock)
            {
                LockFlag.Visibility = Visibility.Visible;
            }
            else
            {
                LockFlag.Visibility = Visibility.Hidden;
            }
            if (sdmodel.IsView)
            {
                ViewFlag.Visibility = Visibility.Visible;
            }
            else
            {
                ViewFlag.Visibility = Visibility.Hidden;
            }
        }

        #region Event Handler
        public event SimulateDataModelEventHandler SDModelSetup;
        private void OnInputKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string name = NameTextInput.Text;
                Match m1 = Regex.Match(name, @"^\w+\d+$");
                //Match m2 = Regex.Match(name, @"^\w+\[\d+\.\.\d+\]$");
                //Match m3 = Regex.Match(name, @"^\w+\b\w+\d+$");
                //Match m4 = Regex.Match(name, @"^\w+\b\w+\[\d+\.\.\d+\]$");
                if (m1.Success)
                {
                    SimulateDataModel _sdmodel = SimulateDataModel.Create(name);
                    if (_sdmodel != null && SDModelSetup != null)
                    {
                        SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
                        _e.SDModel_new = _sdmodel;
                        //_e.SDVModel = this;
                        Setup(_sdmodel);
                        SDModelSetup(this, _e);
                    }
                }
            }
        }

        private void OnCloseButtonEnter(object sender, MouseEventArgs e)
        {
            CloseButtonCursor.Opacity = 0.25;
        }

        private void OnCloseButtonLeave(object sender, MouseEventArgs e)
        {
            CloseButtonCursor.Opacity = 0.0;
        }

        public event RoutedEventHandler Closed;
        private void OnCloseButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, e);
            }
        }

        public event SimulateDataModelEventHandler SDModelLock;
        public event SimulateDataModelEventHandler SDModelUnlock;
        private void MI_LockButton_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.SDModel_old = _e.SDModel_new = SDModel;
            if (!SDModel.IsLock)
            {
                if (SDModelLock != null)
                {
                    SDModelLock(this, _e);
                }
            }
            else
            {
                if (SDModelUnlock != null)
                {
                    SDModelUnlock(this, _e);
                }    
            }
        }

        public event SimulateDataModelEventHandler SDModelView;
        public event SimulateDataModelEventHandler SDModelUnview;
        private void MI_ViewButton_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.SDModel_old = _e.SDModel_new = SDModel;
            if (!SDModel.IsView)
            {
                if (SDModelView != null)
                {
                    SDModelView(this, _e);
                }
            }
            else
            {
                if (SDModelUnview != null)
                {
                    SDModelUnview(this, _e);
                }
            }
        }
        #endregion
        
    }
}

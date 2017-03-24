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

using SamSoarII.Simulation.Core.DataModel;

namespace SamSoarII.Simulation.UI.Chart
{
    /// <summary>
    /// VariableList.xaml 的交互逻辑
    /// </summary>
    public partial class VariableList : UserControl
    {
        private LinkedList<SimulateDataViewModel> sdvmodels;
        public LinkedList<SimulateDataViewModel> SDVModels
        {
            get
            {
                return this.sdvmodels;
            }
            set
            {
                this.sdvmodels = value;
                Update();
            }
        }

        public VariableList()
        {
            InitializeComponent();
        }

        public void Update()
        {
            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
            RowDefinition rdef = null;
            int i = 0;
            foreach (SimulateDataViewModel sdvmodel in sdvmodels)
            {
                rdef = new RowDefinition();
                rdef.Height = new GridLength(40);
                MainGrid.RowDefinitions.Add(rdef);
                Grid.SetRow(sdvmodel, i++);
                Grid.SetColumn(sdvmodel, 0);
                MainGrid.Children.Add(sdvmodel);
            }
            rdef = new RowDefinition();
            rdef.Height = new GridLength(40);
            MainGrid.RowDefinitions.Add(rdef);
            Grid.SetRow(NewButton, i);
            Grid.SetColumn(NewButton, 0);
            MainGrid.Children.Add(NewButton);
        }

        public void AddLast(SimulateDataViewModel sdvmodel)
        {
            RowDefinition rdef = new RowDefinition();
            rdef.Height = new GridLength(40);
            MainGrid.RowDefinitions.Add(rdef);
            MainGrid.Children.Remove(NewButton);
            int count = SDVModels.Count();
            Grid.SetRow(sdvmodel, count);
            Grid.SetColumn(sdvmodel, 0);
            Grid.SetRow(NewButton, count+1);
            Grid.SetColumn(NewButton, 0);
            SDVModels.AddLast(sdvmodel);
            MainGrid.Children.Add(sdvmodel);
            MainGrid.Children.Add(NewButton);
        }

        public void Remove(SimulateDataViewModel sdvmodel)
        {
            MainGrid.RowDefinitions.RemoveAt(0);
            LinkedListNode<SimulateDataViewModel> node = SDVModels.Find(sdvmodel);
            node = node.Next;
            while (node != null)
            {
                SimulateDataViewModel _sdvmodel = node.Value;
                int row = Grid.GetRow(_sdvmodel);
                Grid.SetRow(_sdvmodel, row - 1);
                node = node.Next;
            }
            Grid.SetRow(NewButton, SDVModels.Count() - 1);
            SDVModels.Remove(sdvmodel);
            MainGrid.Children.Remove(sdvmodel);
        }

        #region Event Handler
        private void OnNewButtonClicked(object sender, MouseEventArgs e)
        {
            SimulateDataViewModel sdimodel = new SimulateDataViewModel();
            sdimodel.SDModelSetup += OnSDModelSetup;
            sdimodel.Closed += OnSDModelClose;
            AddLast(sdimodel);
        }

        public event SimulateDataModelEventHandler SDModelSetup;
        private void OnSDModelSetup(object sender, SimulateDataModelEventArgs e)
        {
            if (SDModelSetup != null)
            {
                SDModelSetup(sender, e);
            }
        }

        public event SimulateDataModelEventHandler SDModelClose;
        private void OnSDModelClose(object sender, RoutedEventArgs e)
        {
            if (sender is SimulateDataViewModel)
            {
                SimulateDataViewModel sdvmodel = (SimulateDataViewModel)(sender);
                if (SDModelClose != null)
                {
                    SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
                    _e.SDModel = sdvmodel.SDModel;
                    _e.SDVModel = sdvmodel;
                    SDModelClose(sender, _e);
                }
            }
        }
        #endregion

    }
}

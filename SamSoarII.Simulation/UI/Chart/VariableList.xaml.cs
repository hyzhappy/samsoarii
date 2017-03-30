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

        public void BuildRouted(SimuViewChartModel svcmodel)
        {
            svcmodel.SDModelClose += OnSDModelClose;
            svcmodel.SDModelSetup += OnSDModelSetup;
            svcmodel.SDModelLock += OnSDModelLock;
            svcmodel.SDModelView += OnSDModelView;
            svcmodel.SDModelUnlock += OnSDModelUnlock;
            svcmodel.SDModelUnview += OnSDModelUnview;
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

        #region SimulateDataModel Manipulation

        public void Add(SimulateDataModel sdmodel, int id)
        {
            SimulateDataViewModel sdvmodel = new SimulateDataViewModel(sdmodel);
            Add(sdvmodel, id);
        }

        public void Add(SimulateDataViewModel sdvmodel, int id)
        {
            if (id == sdvmodels.Count())
            {
                AddLast(sdvmodel);
                return;
            }
            RowDefinition rdef = new RowDefinition();
            rdef.Height = new GridLength(40);
            MainGrid.RowDefinitions.Add(rdef);
            SimulateDataViewModel _sdvmodel = sdvmodels.ElementAt(id);
            LinkedListNode<SimulateDataViewModel> nodestart = sdvmodels.Find(_sdvmodel);
            LinkedListNode<SimulateDataViewModel> node = nodestart;
            while (node != null)
            {
                _sdvmodel = node.Value;
                int row = Grid.GetRow(_sdvmodel);
                Grid.SetRow(_sdvmodel, row + 1);
                node = node.Next;
            }
            Grid.SetRow(sdvmodel, id);
            sdvmodels.AddBefore(nodestart, sdvmodel);
            MainGrid.Children.Add(sdvmodel);
            sdvmodel.Closed += OnSDModelClose;
            sdvmodel.SDModelLock += OnSDModelLock;
            sdvmodel.SDModelView += OnSDModelView;
            sdvmodel.SDModelUnlock += OnSDModelUnlock;
            sdvmodel.SDModelUnview += OnSDModelUnview;
        }

        public void AddLast(SimulateDataModel sdmodel)
        {
            SimulateDataViewModel sdvmodel = new SimulateDataViewModel(sdmodel);
            AddLast(sdvmodel);
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
            sdvmodel.Closed += OnSDModelClose;
            sdvmodel.SDModelLock += OnSDModelLock;
            sdvmodel.SDModelView += OnSDModelView;
            sdvmodel.SDModelUnlock += OnSDModelUnlock;
            sdvmodel.SDModelUnview += OnSDModelUnview;
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
            sdvmodel.Closed -= OnSDModelClose;
            sdvmodel.SDModelLock -= OnSDModelLock;
            sdvmodel.SDModelView -= OnSDModelView;
            sdvmodel.SDModelUnlock -= OnSDModelUnlock;
            sdvmodel.SDModelUnview -= OnSDModelUnview;
        }

        #endregion

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
            if (e.SDVModel != null)
            {
                return;
            }
            if (sender is SimulateDataViewModel)
            {
                SimulateDataViewModel sdvmodel = (SimulateDataViewModel)(sender);
                e.SDVModel = sdvmodel;
                e.ID = 0;
                foreach (SimulateDataViewModel _sdvmodel in SDVModels)
                {
                    if (sdvmodel == _sdvmodel)
                    {
                        break;
                    }
                    e.ID++;
                }
                if (SDModelSetup != null)
                {
                    SDModelSetup(sender, e);
                }
            }
            else if (sender is SimuViewChartModel)
            {
                foreach (SimulateDataViewModel sdvmodel in SDVModels)
                {
                    if (sdvmodel.SDModel == e.SDModel_old)
                    {
                        sdvmodel.SDModel = e.SDModel_new;
                        e.SDVModel = sdvmodel;
                        break;
                    }
                }
                if (e.SDVModel == null)
                {
                    Add(e.SDModel_new, e.ID);
                }
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
                    _e.SDModel_old = sdvmodel.SDModel;
                    _e.SDVModel = sdvmodel;
                    SDModelClose(sender, _e);
                }
            }
        }
        private void OnSDModelClose(object sender, SimulateDataModelEventArgs e)
        {
            if (e.SDVModel != null)
            {
                return;
            }
            if (sender is SimuViewChartModel)
            {
                foreach (SimulateDataViewModel sdvmodel in SDVModels)
                {
                    if (sdvmodel.SDModel == e.SDModel_old)
                    {
                        e.SDVModel = sdvmodel;
                        Remove(sdvmodel);
                        break;
                    }
                }
                if (e.SDVModel == null)
                {
                    throw new ArgumentException();
                }
            }
        }

        public event SimulateDataModelEventHandler SDModelLock;
        private void OnSDModelLock(object sender, SimulateDataModelEventArgs e)
        {
            if (e.SDVModel != null)
            {
                return;
            }
            if (sender is SimulateDataViewModel)
            {
                e.SDVModel = (SimulateDataViewModel)(sender);
                SDModelLock(this, e);
            }
            if (sender is SimuViewChartModel)
            {
                if (e.SDVModel == null)
                {
                    foreach (SimulateDataViewModel sdvmodel in SDVModels)
                    {
                        if (sdvmodel.SDModel == e.SDModel_old)
                        {
                            e.SDVModel = sdvmodel;
                        }
                    }
                }
                if (e.SDVModel == null)
                {
                    throw new KeyNotFoundException();
                }
                //e.SDVModel.ShowLockFlag();
                e.SDVModel.LockFlag.Visibility = Visibility.Visible;
            }
        }

        public event SimulateDataModelEventHandler SDModelView;
        private void OnSDModelView(object sender, SimulateDataModelEventArgs e)
        {
            if (sender is SimulateDataViewModel)
            {
                e.SDVModel = (SimulateDataViewModel)(sender);
                SDModelView(this, e);
            }
            if (sender is SimuViewChartModel)
            {
                if (e.SDVModel == null)
                {
                    foreach (SimulateDataViewModel sdvmodel in SDVModels)
                    {
                        if (sdvmodel.SDModel == e.SDModel_old)
                        {
                            e.SDVModel = sdvmodel;
                        }
                    }
                }
                if (e.SDVModel == null)
                {
                    throw new KeyNotFoundException();
                }
                //e.SDVModel.ShowViewFlag();
                e.SDVModel.ViewFlag.Visibility = Visibility.Visible;
            }
        }

        public event SimulateDataModelEventHandler SDModelUnlock;
        private void OnSDModelUnlock(object sender, SimulateDataModelEventArgs e)
        {
            if (sender is SimulateDataViewModel)
            {
                e.SDVModel = (SimulateDataViewModel)(sender);
                SDModelUnlock(this, e);
            }
            if (sender is SimuViewChartModel)
            {
                if (e.SDVModel == null)
                {
                    foreach (SimulateDataViewModel sdvmodel in SDVModels)
                    {
                        if (sdvmodel.SDModel == e.SDModel_old)
                        {
                            e.SDVModel = sdvmodel;
                        }
                    }
                }
                if (e.SDVModel == null)
                {
                    throw new KeyNotFoundException();
                }
                //e.SDVModel.HideLockFlag();
                e.SDVModel.LockFlag.Visibility = Visibility.Hidden;
            }
        }

        public event SimulateDataModelEventHandler SDModelUnview;
        private void OnSDModelUnview(object sender, SimulateDataModelEventArgs e)
        {
            if (sender is SimulateDataViewModel)
            {
                e.SDVModel = (SimulateDataViewModel)(sender);
                SDModelUnview(this, e);
            }
            if (sender is SimuViewChartModel)
            {
                if (e.SDVModel == null)
                {
                    foreach (SimulateDataViewModel sdvmodel in SDVModels)
                    {
                        if (sdvmodel.SDModel == e.SDModel_old)
                        {
                            e.SDVModel = sdvmodel;
                        }
                    }
                }
                if (e.SDVModel == null)
                {
                    throw new KeyNotFoundException();
                }
                //e.SDVModel.HideViewFlag();
                e.SDVModel.ViewFlag.Visibility = Visibility.Hidden;
            }
        }
        #endregion

    }
}

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
using SamSoarII.Project;
using SamSoarII.InstructionViewModel;
using SamSoarII.UserInterface;
namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ProjectTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectTreeView : UserControl
    {
        private ProjectModel _currentProject;

        private LadderTabControl _ldTabControl;

        private ProjectTreeView()
        {
            InitializeComponent();
        }

        private void LoadInstruction()
        {
            foreach (var ldmodel in InstructionViewModelPrototype.GetElementViewModels())
            {
                var treeitem = new InstrucionTreeItem(ldmodel.GetCatalogID());
                treeitem.Header = ldmodel.GetCatalogID();
                treeitem.MouseDoubleClick += InstructionTreeViewItemDoubleClick;
                InstructionRootItem.Items.Add(treeitem);
            }
        }

        private void InstructionTreeViewItemDoubleClick(object sender, RoutedEventArgs e)
        {
            // Add a new ViewModel when Cureent TabItem is LadderTabItem
            var tabItem = _ldTabControl.SelectedItem as LadderTabItem;
            if(tabItem != null)
            {
                var treeItem = sender as InstrucionTreeItem;
                tabItem.AddElement(treeItem.Index);
            }
        }

        public ProjectTreeView(ProjectModel projectModel, LadderTabControl ldTabControl) : this()
        {
            _currentProject = projectModel;
            _ldTabControl = ldTabControl;
            ProjectRootItem.Header = string.Format("工程-{0}", projectModel.Name);
            SetMainRoutineTreeViewItem(projectModel.MainRoutine);
            foreach(var subroutine in projectModel.SubRoutines)
            {
                AddSubRoutineTreeViewItem(subroutine);
            }
            foreach (var fbmodel in projectModel.FuncBlocks)
            {
                AddFuncBlockTreeViewItem(fbmodel);
            }
            LoadInstruction();          
        }

        private void SetMainRoutineTreeViewItem(LadderDiagramModel ldmodel)
        {
            MainRoutineRootTreeViewItem.Items.Clear();
            TreeViewItem mitem = new TreeViewItem();
            MainRoutineRootTreeViewItem.Items.Add(mitem);
            mitem.ContextMenu = Resources["ContextMenu4"] as ContextMenu;
            BindingOperations.SetBinding(mitem, HeaderedItemsControl.HeaderProperty, new Binding() { Source = ldmodel, Path = new PropertyPath("Name"), Mode = BindingMode.TwoWay });
            mitem.MouseDoubleClick += Item_MouseDoubleClick;
        }

        public void AddSubRoutineTreeViewItem(LadderDiagramModel ldmodel)
        {
            TreeViewItem sitem = new TreeViewItem();
            sitem.Background = Brushes.Transparent;
            SubRoutineRootTreeViewItem.Items.Add(sitem);
            sitem.ContextMenu = Resources["ContextMenu4"] as ContextMenu;
            BindingOperations.SetBinding(sitem, HeaderedItemsControl.HeaderProperty, new Binding() { Source = ldmodel, Path = new PropertyPath("Name"), Mode = BindingMode.TwoWay });
            sitem.MouseDoubleClick += Item_MouseDoubleClick;
        }

        public void AddFuncBlockTreeViewItem(FuncBlockModel fdmodel)
        {
            TreeViewItem sitem = new TreeViewItem();
            sitem.Background = Brushes.Transparent;
            FuncBlockRootTreeViewItem.Items.Add(sitem);
            sitem.ContextMenu = Resources["ContextMenu4"] as ContextMenu;
            BindingOperations.SetBinding(sitem, HeaderedItemsControl.HeaderProperty, new Binding() { Source = fdmodel, Path = new PropertyPath("Name"), Mode = BindingMode.TwoWay });
            sitem.MouseDoubleClick += Item_MouseDoubleClick;
        }



        private void RemoveSubTreeViewItem(TreeViewItem sitem)
        {
            SubRoutineRootTreeViewItem.Items.Remove(sitem);
            FuncBlockRootTreeViewItem.Items.Remove(sitem);
        }

        private void Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            _ldTabControl.ShowItem(item.Header.ToString());
        }

        private void OnOpenRoutineMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if(menuItem != null)
            {
                var treeViewItem = ((ContextMenu)menuItem.Parent).PlacementTarget as TreeViewItem;
                _ldTabControl.ShowItem(treeViewItem.Header.ToString());
            }
        }

        private void OnRenameRoutineMenuItemClick(object sender, RoutedEventArgs e)
        {
            //TODO: 
        }

        private void OnRemoveRoutineMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                var treeViewItem = ((ContextMenu)menuItem.Parent).PlacementTarget as TreeViewItem;
                RemoveSubTreeViewItem(treeViewItem);
                _currentProject.RemoveFuncBlockByName(treeViewItem.Header.ToString());
                _currentProject.RemoveSubRoutineByName(treeViewItem.Header.ToString());
                _ldTabControl.RemoveTabItem(treeViewItem.Header.ToString());
            }
        }
    }
}

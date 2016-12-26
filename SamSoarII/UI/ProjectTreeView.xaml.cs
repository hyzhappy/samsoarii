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
            LoadInstruction();          
        }

        private void SetMainRoutineTreeViewItem(LadderDiagramModel ldmodel)
        {
            MainRoutineTreeViewItem.Items.Clear();
            TreeViewItem mitem = new TreeViewItem();
            MainRoutineTreeViewItem.Items.Add(mitem);
            mitem.ContextMenu = Resources["ContextMenu4"] as ContextMenu;
            BindingOperations.SetBinding(mitem, HeaderedItemsControl.HeaderProperty, new Binding() { Source = ldmodel, Path = new PropertyPath("Name"), Mode = BindingMode.TwoWay });
            mitem.MouseDoubleClick += Item_MouseDoubleClick;
        }

        public void AddSubRoutineTreeViewItem(LadderDiagramModel ldmodel)
        {
            TreeViewItem sitem = new TreeViewItem();
            sitem.Background = Brushes.Transparent;
            SubRoutineTreeViewItem.Items.Add(sitem);
            sitem.ContextMenu = Resources["ContextMenu4"] as ContextMenu;
            BindingOperations.SetBinding(sitem, HeaderedItemsControl.HeaderProperty, new Binding() { Source = ldmodel, Path = new PropertyPath("Name"), Mode = BindingMode.TwoWay });
            sitem.MouseDoubleClick += Item_MouseDoubleClick;
        }

        private void RemoveSubRoutineTreeViewItem(LadderDiagramModel ldmodel)
        {

        }

        public void AddSubRoutine(object sender, RoutedEventArgs e)
        {
            AddNewRoutineWindow window = new AddNewRoutineWindow();
            window.EnsureButtonClick += (sender1, e1) =>
            {
                if (window.NameContent == string.Empty)
                {
                    MessageBox.Show("程序名不能为空");
                    return;
                }
                if (_currentProject.Contain(window.NameContent))
                {
                    MessageBox.Show("已存在同名子程序");
                    return;
                }
                LadderDiagramModel ldmodel = new LadderDiagramModel(window.NameContent);
                _currentProject.AddSubRoutine(ldmodel);
                AddSubRoutineTreeViewItem(ldmodel);
                _ldTabControl.ShowItem(ldmodel);
                window.Close();
            };
            window.ShowDialog();
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
                //RemoveSubRoutineTreeViewItem(treeViewItem.Header.ToString());
            }
        }
    }
}

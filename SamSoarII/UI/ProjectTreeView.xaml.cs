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
using SamSoarII.AppMain.Project;
using SamSoarII.InstructionViewModel;
using SamSoarII.UserInterface;
namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ProjectTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectTreeView : UserControl
    {

        public event MouseButtonEventHandler InstructionTreeItemDoubleClick;

        public event ShowTabItemEventHandler TabItemOpened;
        public ProjectTreeView()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            MainRoutineRootTreeViewItem.Items.Clear();
            SubRoutineRootTreeViewItem.Items.Clear();
            FuncBlockRootTreeViewItem.Items.Clear();
        }

        /// <summary>
        /// Load ProjectModel, add it's MainRoutine, SubRoutines and FuncBlocks to TreeView
        /// </summary>
        /// <param name="project"></param>
        public void LoadProject(ProjectModel project)
        {
            Reset();
            TreeViewItem mainRoutineItem = new TreeViewItem();
            mainRoutineItem.Header = project.MainRoutine.LadderName;
            MainRoutineRootTreeViewItem.Items.Add(mainRoutineItem);
            foreach(LadderDiagramViewModel ldmodel in project.SubRoutines)
            {
                AddSubRoutine(ldmodel.LadderName);
            }
            foreach(FuncBlockViewModel fbmodel in project.FuncBlocks)
            {
                AddFuncBlock(fbmodel.FuncBlockName);
            }
        }

        public void AddSubRoutine(string name)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = name;
            SubRoutineRootTreeViewItem.Items.Add(item);
        }

        public void AddFuncBlock(string name)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = name;
            FuncBlockRootTreeViewItem.Items.Add(item);
        }

        #region Event handler
        // 打开子程序处理事件，直接调用双击事件
        void OnOpenRoutine(object sender, RoutedEventArgs e)
        {
            if (this.TabItemOpened != null)
            {
                var menuitem = sender as MenuItem;
                if(menuitem != null)
                {
                    var contextmenu = menuitem.Parent as ContextMenu;
                    if(contextmenu != null)
                    {
                        var treeviewitem = contextmenu.PlacementTarget as TreeViewItem;
                        if(treeviewitem != null)
                        {
                            TabItemOpened.Invoke(sender, new ShowTabItemEventArgs(treeviewitem.Header.ToString()));
                        }
                    }

                }

            }
        }


        private void OnRoutineTreeItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(this.TabItemOpened != null)
            {
                var treeItem = sender as TreeViewItem;
                if(treeItem != null)
                {
                    TabItemOpened.Invoke(sender, new ShowTabItemEventArgs(treeItem.Header.ToString()));
                }
            }
        }

        void OnInstructionTreeItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(this.InstructionTreeItemDoubleClick != null)
            {
                InstructionTreeItemDoubleClick.Invoke(sender, e);
            }
        }
        #endregion


    }
}

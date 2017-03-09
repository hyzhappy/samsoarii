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
using SamSoarII.LadderInstViewModel;
using SamSoarII.UserInterface;
namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ProjectTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectTreeView : UserControl
    {

        public event MouseButtonEventHandler InstructionTreeItemDoubleClick = delegate { }; 

        public event ShowTabItemEventHandler TabItemOpened = delegate { };

        public event RoutineRenamedEventHandler RoutineRenamed = delegate { };

        public event RoutineChangedEventHandler RoutineRemoved = delegate { };
        public ProjectTreeView(string projectname)
        {
            InitializeComponent();
            ProjectRootItem.Header = string.Format("Project - {0}", projectname);
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

        public TreeViewItem GetSubRoutineTreeViewItemByName(string name)
        {
            foreach(TreeViewItem item in SubRoutineRootTreeViewItem.Items)
            {
                if(item.Header.ToString() == name)
                {
                    return item;
                }
            }
            return null;
        }
        public TreeViewItem GetFuncBlockTreeViewItemByName(string name)
        {
            foreach (TreeViewItem item in FuncBlockRootTreeViewItem.Items)
            {
                if (item.Header.ToString() == name)
                {
                    return item;
                }
            }
            return null;
        }

        public void RemoveRoutine(string name)
        {
            var item = GetSubRoutineTreeViewItemByName(name);
            if(item != null)
            {
                SubRoutineRootTreeViewItem.Items.Remove(item);
            }
            else
            {
                item = GetFuncBlockTreeViewItemByName(name);
                if(item != null)
                {
                    FuncBlockRootTreeViewItem.Items.Remove(item);
                }
            }
        }


        #region Event handler
        // 从ContextMenu打开子程序处理事件，直接调用双击事件
        void OnOpenRoutine(object sender, RoutedEventArgs e)
        {
            var menuitem = sender as MenuItem;
            if (menuitem != null)
            {
                var contextmenu = menuitem.Parent as ContextMenu;
                if (contextmenu != null)
                {
                    var treeviewitem = contextmenu.PlacementTarget as TreeViewItem;
                    if (treeviewitem != null)
                    {
                        TabItemOpened.Invoke(sender, new ShowTabItemEventArgs(treeviewitem.Header.ToString(), TabType.Program));
                    }
                }
            }         
        }
        private void OnRoutineTreeItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeItem = sender as TreeViewItem;
            if(treeItem != null)
            {
                TabItemOpened.Invoke(sender, new ShowTabItemEventArgs(treeItem.Header.ToString(), TabType.Program));
            }        
        }
        void OnInstructionTreeItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.InstructionTreeItemDoubleClick != null)
            {
                InstructionTreeItemDoubleClick.Invoke(sender, e);
            }
        }
        private void OnShowCommentList(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnShowUsageList(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnShowVariableList(object sender, MouseButtonEventArgs e)
        {
            TabItemOpened.Invoke(sender, new ShowTabItemEventArgs("变量表", TabType.VariableList));
        }

        private void OnRenameRoutine(object sender, RoutedEventArgs e)
        {
            var menuitem = sender as MenuItem;
            if (menuitem != null)
            {
                var contextmenu = menuitem.Parent as ContextMenu;
                if (contextmenu != null)
                {
                    var treeviewitem = contextmenu.PlacementTarget as TreeViewItem;
                    if (treeviewitem != null)
                    {
                        var oldname = treeviewitem.Header.ToString();
                        var textBox = new TextBox();                 
                        treeviewitem.Header = textBox;
                        textBox.Loaded += (sender3, e3) =>
                        {
                            textBox.Text = oldname;
                            textBox.SelectAll();
                            textBox.Focusable = true;
                            textBox.Focus();
                            Keyboard.Focus(textBox);
                            textBox.TextAlignment = TextAlignment.Right;
                            textBox.MinWidth = 100;
                        };
                        textBox.KeyDown += (sender2, e2) =>
                        {
                            if (e2.Key == Key.Enter)
                            {
                                textBox.Focusable = false;
                                //textBox.RaiseEvent(new RoutedEventArgs(TextBox.LostFocusEvent));       
                            }
                        };
                        textBox.LostFocus += (sender1, e1) =>
                        {
                            var newname = textBox.Text;
                            if (oldname != newname)
                            {
                                RoutineRenamed.Invoke(treeviewitem, new RoutineRenamedEventArgs(oldname, newname));
                            }
                            else
                            {
                                treeviewitem.Header = newname;
                            }
                        };
                    }
                }
            }
        }
        private void OnRemoveRoutine(object sender, RoutedEventArgs e)
        {
            var menuitem = sender as MenuItem;
            if (menuitem != null)
            {
                var contextmenu = menuitem.Parent as ContextMenu;
                if (contextmenu != null)
                {
                    var treeviewitem = contextmenu.PlacementTarget as TreeViewItem;
                    if (treeviewitem != null)
                    {
                        var name = treeviewitem.Header.ToString();
                        var result = MessageBox.Show("删除后不能恢复，是否确定", "重要", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if(result == MessageBoxResult.Yes)
                        {
                            RoutineRemoved.Invoke(treeviewitem, new RoutineChangedEventArgs(treeviewitem.Header.ToString()));
                        }
                    }
                }
            }
        }

        #endregion


    }
}

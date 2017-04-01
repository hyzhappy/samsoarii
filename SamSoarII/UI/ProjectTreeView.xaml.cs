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
using System.ComponentModel;
using System.Collections.ObjectModel;
namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// ProjectTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectTreeView : UserControl, INotifyPropertyChanged
    {

        private ProjectModel _projectModel;
        public ProjectModel Project
        {
            get { return _projectModel; }
        }

        private string _oldname;

        private object _renamedItem;

        private bool _isInEditMode;
        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set
            {
                _isInEditMode = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsInEditMode"));
            }
        }
        ElementList _elementList;
        public event MouseButtonEventHandler InstructionTreeItemDoubleClick = delegate { }; 

        public event ShowTabItemEventHandler TabItemOpened = delegate { };

        public event RoutineRenamedEventHandler RoutineRenamed = delegate { };

        public event RoutedEventHandler RoutineRemoved = delegate { };

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ProjectTreeView(ProjectModel project)
        {
            InitializeComponent();
            _projectModel = project;
            this.DataContext = Project;
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
                        TabItemOpened.Invoke(treeviewitem.Header, new ShowTabItemEventArgs(TabType.Program));
                    }
                }
            }         
        }

        // 每次双击会触发两次这个事件 ???
        private void OnRoutineTreeItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeItem = sender as TreeViewItem;
            if(treeItem != null)
            {
                IProgram prog = treeItem.Header as IProgram;
                if(prog != null)
                {
                    TabItemOpened.Invoke(prog, new ShowTabItemEventArgs(TabType.Program));
                }           
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
            TabItemOpened.Invoke(sender, new ShowTabItemEventArgs(TabType.CommentList));
        }

        private void OnShowCommentList(object sender, RoutedEventArgs e)
        {
            TabItemOpened.Invoke(sender, new ShowTabItemEventArgs(TabType.CommentList));
        }

        private void OnShowUsageList(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnShowUsageList(object sender, RoutedEventArgs e)
        {

        }

        private void OnShowVariableList(object sender, MouseButtonEventArgs e)
        {
            TabItemOpened.Invoke(sender, new ShowTabItemEventArgs(TabType.VariableList));
        }

        private void OnShowVariableList(object sender, RoutedEventArgs e)
        {
            TabItemOpened.Invoke(sender, new ShowTabItemEventArgs(TabType.VariableList));
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
                        var prog = treeviewitem.Header as IProgram;
                        if(prog != null)
                        {
                            _oldname = prog.ProgramName;
                            IsInEditMode = true;
                        }

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
                        var result = MessageBox.Show("删除后不能恢复，是否确定", "重要", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if(result == MessageBoxResult.Yes)
                        {
                            RoutineRemoved.Invoke(treeviewitem.Header, new RoutedEventArgs());
                        }
                    }
                }
            }
        }

        private void OnEditTextBoxVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(textBox.IsVisible)
            {
                textBox.Focus();
                textBox.SelectAll();
            }
        }

        private void OnEditTextBoxKeyDown(object sender, KeyEventArgs e )
        {
            if(e.Key == Key.Enter)
            {
                IsInEditMode = false;
            }
            if(e.Key == Key.Escape)
            {
                var textBox = sender as TextBox;
                textBox.Text = _oldname;
                IsInEditMode = false;
            }
        }

        private void OnEditTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            IsInEditMode = false;
            var textBox = sender as TextBox;
            TreeViewItem item = FindTreeItem(sender as TextBox);
            RoutineRenamed.Invoke(item.Header, new RoutineRenamedEventArgs(textBox.Text));
        }

        static TreeViewItem FindTreeItem(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);
            return source as TreeViewItem;
        }
        private void OnElementListOpenDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_elementList == null)
            {
                _elementList = new ElementList();
                _elementList.Show();
            }
            else
            {
                double workHeight = SystemParameters.WorkArea.Height;
                double workWidth = SystemParameters.WorkArea.Width;
                _elementList.Left = (workWidth - _elementList.Width) / 2;
                _elementList.Top = (workHeight - _elementList.Height) / 2;
                _elementList.Show();
            }
        }
        private void OnElementListOpen(object sender, RoutedEventArgs e)
        {
            if (_elementList == null)
            {
                _elementList = new ElementList();
                _elementList.Show();
            }
            else
            {
                double workHeight = SystemParameters.WorkArea.Height;
                double workWidth = SystemParameters.WorkArea.Width;
                _elementList.Left = (workWidth - _elementList.Width) / 2;
                _elementList.Top = (workHeight - _elementList.Height) / 2;
                _elementList.Show();
            }
        }
        #endregion
    }
}

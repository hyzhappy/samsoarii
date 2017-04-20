﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SamSoarII.AppMain.Project;
using SamSoarII.UserInterface;
using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.Configuration;
using SamSoarII.LadderInstViewModel;
using SamSoarII.AppMain.UI.HelpDocComponet;
using System.Windows.Media.Animation;
using SamSoarII.PLCDevice;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private InteractionFacade _interactionFacade;
        private CanAnimationScroll MainScroll;
        public MainWindow()
        {
            InitializeComponent();
            _interactionFacade = new InteractionFacade(this);
            this.Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            RecentFileMenu.DataContext = ProjectFileManager.projectShowMessage;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Name == "elementList")
                {
                    window.Closing -= ((ElementList)window).OnClosing;
                    window.Close();
                    break;
                }
            }
        }

        public void SetProjectTreeView(ProjectTreeView treeview)
        {
            TreeViewGrid.Children.Clear();
            TreeViewGrid.Children.Add(treeview);
        }
        public void ClearProjectTreeView()
        {
            TreeViewGrid.Children.Clear();
        }
        #region Event handler
        private void OnCommentModeToggle(object sender, RoutedEventArgs e)
        {
            _interactionFacade.IsCommentMode = !_interactionFacade.IsCommentMode;
        }

        private void OnShowAboutDialog(object sender, RoutedEventArgs e)
        {

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(!GlobalSetting.LoadLadderScaleSuccess())
            {
                GlobalSetting.LadderOriginScaleX = MainTab.ActualWidth / 3700;
                GlobalSetting.LadderOriginScaleY = MainTab.ActualWidth / 3700;
            }
            MainScroll = GetMainScroll();
        }
        private CanAnimationScroll GetMainScroll()
        {
            return (CanAnimationScroll)MainTab.Template.FindName("MainTabScroll", MainTab);
        }

        private void OnTabItemHeaderCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if(button != null)
            {
                TabItem tabitem = button.TemplatedParent as TabItem;
                if(tabitem != null)
                {
                    ITabItem tab = tabitem.Content as ITabItem;
                    if (tab != null)
                    {
                        _interactionFacade.CloseTabItem(tab);
                    }
                }
            }
        }
        private void OnTabItemHeaderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                Grid grid = sender as Grid;
                if (grid != null)
                {
                    TabItem tabitem = grid.TemplatedParent as TabItem;
                    if (tabitem != null)
                    {
                        ITabItem tab = tabitem.Content as ITabItem;
                        if (tab != null)
                        {
                            _interactionFacade.CloseTabItem(tab);
                        }
                    }
                }
            }
        }
        private void OnRecentProjectOpen(object sender, RoutedEventArgs e)
        {
            int index = GetMenuItemIndex((sender as MenuItem).Header as string);
            var projectMessage = ProjectFileManager.RecentUsedProjectMessages.ElementAt(index);
            if (!_interactionFacade.ProjectLoaded)
            {
                if (!File.Exists(projectMessage.Value.Item2))
                {
                    MessageBox.Show(string.Format("file has been removed or deleted"));
                    ProjectFileManager.Delete(index);
                }
                else
                {
                    _interactionFacade.LoadProject(projectMessage.Value.Item2);
                }
            }
            else
            {
                if (projectMessage.Value.Item1 == _interactionFacade.ProjectModel.ProjectName)
                {
                    MessageBox.Show(string.Format("the opening project is current project"));
                }
                else
                {
                    if (!File.Exists(projectMessage.Value.Item2))
                    {
                        MessageBox.Show(string.Format("file has been removed or deleted"));
                        ProjectFileManager.Delete(index);
                    }
                    else
                    {
                        _interactionFacade.LoadProject(projectMessage.Value.Item2);
                    }
                }
            }
            e.Handled = true;
        }
        private int GetMenuItemIndex(string item)
        {
            return RecentFileMenu.Items.IndexOf(item);
        }
        #endregion

        private void CreateMainRoutine(string name)
        {

        }

        private void CreateProject(string name, string fullFileName)
        {
            _interactionFacade.CreateProject(name, fullFileName);
            _interactionFacade.SaveProject();
        }
        private bool OpenProject(string fullFileName)
        {
            return _interactionFacade.LoadProject(fullFileName);
        }
        #region Command can Execute
        private void ClosePageCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainTab.SelectedItem != null;
        }
        private void ScrollToRightCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MainScroll == null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = MainScroll.ScrollableWidth != 0;
            }
        }
        private void ScrollToLeftCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MainScroll == null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = MainScroll.HorizontalOffset != 0;
            }
        }
        private void SaveCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void AddNewSubRouteinCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void AddNewFuncBlockCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ZoomInCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ZoomOutCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void CompileCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ShowPropertyDialogCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void ShowOptionDialogCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void DownloadCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void SimulateCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        #endregion


        #region Command Execute
        private void ClosePageExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MainTab.CloseItem(MainTab.SelectedItem as ITabItem);
        }
        private void ScrollToLeftCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ScrollExecute(ScrollDirection.Left);
        }
        private void ScrollToRightCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ScrollExecute(ScrollDirection.Right);
        }
        private void OnZoomInCommandExecute(object sender, RoutedEventArgs e)
        {
            GlobalSetting.LadderScaleX += 0.1;
            GlobalSetting.LadderScaleY += 0.1;
        }

        private void OnZoomOutCommandExecute(object sender, RoutedEventArgs e)
        {
            GlobalSetting.LadderScaleX -= 0.1;
            GlobalSetting.LadderScaleY -= 0.1;
        }

        private void OnAddNewSubRoutineCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            AddNewRoutineWindow window = new AddNewRoutineWindow();
            window.EnsureButtonClick += (sender1, e1) =>
            {
                if(!_interactionFacade.AddNewSubRoutine(window.NameContent))
                {
                    MessageBox.Show("已存在同名的子程序或函数功能块");
                }
                window.Close();
            };
            window.ShowDialog();
        }

        private void OnAddNewFuncBlockCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            AddNewRoutineWindow window = new AddNewRoutineWindow();
            window.EnsureButtonClick += (sender1, e1) =>
            {
                if (!_interactionFacade.AddNewFuncBlock(window.NameContent))
                {
                    MessageBox.Show("已存在同名的子程序或函数功能块");
                }
                window.Close();
            };
            window.ShowDialog();
        }

        private void OnNewProjectExecute(object sender, RoutedEventArgs e)
        {
            NewProjectDialog newProjectDialog;
            using (newProjectDialog = new NewProjectDialog())
            {
                newProjectDialog.EnsureButtonClick += (sender1, e1) =>
                {
                    string name = newProjectDialog.NameContent;
                    string dir = newProjectDialog.PathContent;
                    if (!Directory.Exists(dir))
                    {
                        MessageBox.Show("指定路径不存在");
                        return;
                    }
                    if (name == string.Empty)
                    {
                        MessageBox.Show("文件名不能为空");
                        return;
                    }
                    string fullFileName = string.Format(@"{0}\{1}.ssp", dir, name);
                    if (File.Exists(fullFileName))
                    {
                        MessageBox.Show("指定路径已存在同名文件");
                        return;
                    }
                    PLCDeviceManager.GetPLCDeviceManager().SetSelectDeviceType(newProjectDialog.Type);
                    CreateProject(name, fullFileName);
                    newProjectDialog.Close();
                };
                newProjectDialog.ShowDialog();
            }
        }

        private void OnOpenProjectExecute(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ssp文件|*.ssp";
            if (openFileDialog.ShowDialog() == true)
            {            
                if(!OpenProject(openFileDialog.FileName))
                {
                    MessageBox.Show("不正确的工程文件，工程文件已损坏!");
                }
            }
        }

        private void OnSaveProjectExecute(object sender, RoutedEventArgs e)
        {
            _interactionFacade.SaveProject();
        }
        private void OnCloseProjectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _interactionFacade.CloseCurrentProject();
        }
        private void OnSaveAsProjectExecute(object sender, RoutedEventArgs e)
        {

        }

        private void OnCompileCommandExecute(object sender, RoutedEventArgs e)
        {
            _interactionFacade.CompileProject();
        }

        private void OnDownloadCommandExecute(object sender, RoutedEventArgs e)
        {

        }

        private void OnSimulateCommandExecute(object sender, RoutedEventArgs e)
        {
            _interactionFacade.SimulateProject();
        }

        private void OnShowPropertyDialogCommandExecute(object sender, RoutedEventArgs e)
        {
            ProjectPropertyDialog dialog = new ProjectPropertyDialog(_interactionFacade.ProjectModel);
            dialog.EnsureButtonClick += (sender1, e1) =>
            {
                dialog.Save();
                dialog.Close();
            };
            dialog.ShowDialog();
        }
        private void OnShowHelpDocWindow(object sender, RoutedEventArgs e)
        {
            HelpDocWindow helpDocWindow = new HelpDocWindow();
            helpDocWindow.Show();
        }
        private void OnShowOptionDialogCommandExecute(object sender, RoutedEventArgs e)
        {
            OptionDialog dialog = new OptionDialog();
            dialog.ShowDialog();
        }

        private void OnProcessExitExecute(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        private void OnCommentModeToggle(object sender, ExecutedRoutedEventArgs e)
        {
            if(CommentModeToggleButton.IsChecked.HasValue)
            {
                _interactionFacade.IsCommentMode = CommentModeToggleButton.IsChecked.Value;
            }   
        }
        private void OnCloseProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void CommentModeCanToggle(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void OnCheckNetworkErrorCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_interactionFacade != null)
            {
                e.CanExecute = _interactionFacade.ProjectLoaded;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void OnCheckNetworkErrorCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (!_interactionFacade.CurrentLadder.CheckProgramControlInstructions())
            {
                MessageBox.Show(string.Format("程序控制指令配对失败！"));
            }
            else
            {
                var tempQueue = new Queue<LadderNetworkViewModel>(_interactionFacade.CurrentLadder.GetNetworks());
                while (tempQueue.Count > 0)
                {
                    var ladderNetworkViewModel = tempQueue.Dequeue();
                    if (ladderNetworkViewModel.IsNetworkError())
                    {
                        MessageBox.Show(string.Format("网络{0}错误", ladderNetworkViewModel.NetworkNumber));
                    }
                    else
                    {
                        MessageBox.Show(string.Format("网络{0}正常，可以编译!", ladderNetworkViewModel.NetworkNumber));
                    }
                }
            }
        }
        private void ScrollToLeftAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = MainScroll.CanChangeHorizontalOffset;
            animation.To = Math.Max(0, MainScroll.CanChangeHorizontalOffset - 70);
            animation.Duration = new Duration(new TimeSpan(1500000));
            MainScroll.BeginAnimation(CanAnimationScroll.CanChangeHorizontalOffsetProperty, animation);
        }
        private void ScrollToRightAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = MainScroll.CanChangeHorizontalOffset;
            animation.To = MainScroll.CanChangeHorizontalOffset + Math.Min(70, MainScroll.ScrollableWidth);
            animation.Duration = new Duration(new TimeSpan(1500000));
            MainScroll.BeginAnimation(CanAnimationScroll.CanChangeHorizontalOffsetProperty, animation);
        }
        private void ScrollExecute(ScrollDirection direction)
        {
            switch (direction)
            {
                case ScrollDirection.Left:
                    ScrollToLeftAnimation();
                    break;
                case ScrollDirection.Right:
                    ScrollToRightAnimation();
                    break;
                default:
                    break;
            }
        }
    }
}

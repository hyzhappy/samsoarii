using SamSoarII.AppMain.Project;
using SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages;
using SamSoarII.AppMain.UI.HelpDocComponet;
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
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// HelpDocWindow.xaml 的交互逻辑
    /// </summary>
    public enum ScrollDirection
    {
        Left,
        Right
    }
    public class ViewChangedEventArgs : EventArgs
    {
        private int _viewNum;
        public int ViewNum
        {
            get
            {
                return _viewNum;
            }
            set
            {
                _viewNum = value;
            }
        }
        public ViewChangedEventArgs(int viewNum)
        {
            ViewNum = viewNum;
        }
    }
    public delegate void ViewChangedEventHandler(ViewChangedEventArgs e);
    public partial class HelpDocWindow : Window
    {
        private PageManager _pageManager;
        private bool _isHide = false;
        public List<UserControl> FuncUserControls = new List<UserControl>();
        private event ViewChangedEventHandler ViewChanged = delegate { };
        private CanAnimationScroll MainScroll;
        public HelpDocWindow()
        {
            InitializeComponent();
            FuncUserControls.Add(new HelpDocTreeView());
            FuncUserControls.Add(new HelpDocFavorite());
            FuncUserControls.Add(new HelpDocSearch());
            _pageManager = new PageManager(this);
            ViewChanged += HelpDocWindow_ViewChanged;
            Loaded += HelpDocWindow_Loaded;
        }
        
        private void HelpDocWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainScroll = GetMainScroll();
        }
        private CanAnimationScroll GetMainScroll()
        {
            return (CanAnimationScroll)MainTab.Template.FindName("MainTabScroll", MainTab);
        }
        private void HelpDocWindow_ViewChanged(ViewChangedEventArgs e)
        {
            ShowView(e.ViewNum);
        }

        public void ShowView(int viewnum)
        {
            if (!FuncGrid.Children.Contains(FuncUserControls[viewnum]))
            {
                FuncGrid.Children.Clear();
                FuncGrid.Children.Add(FuncUserControls[viewnum]);
            }
        }
        public UserControl GetView(int viewnum)
        {
            return FuncUserControls[viewnum];
        }
        private void OnTabItemHeaderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Grid grid = sender as Grid;
                if (grid != null)
                {
                    TabItem tabitem = grid.TemplatedParent as TabItem;
                    if (tabitem != null)
                    {
                        HelpDocFrame tab = tabitem.Content as HelpDocFrame;
                        if (tab != null)
                        {
                            MainTab.CloseItem(tab);
                        }
                    }
                }
            }
        }
        private void OnTabItemHeaderCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                TabItem tabitem = button.TemplatedParent as TabItem;
                if (tabitem != null)
                {
                    HelpDocFrame tab = tabitem.Content as HelpDocFrame;
                    if (tab != null)
                    {
                        MainTab.CloseItem(tab);
                    }
                }
            }
        }
        private void OnClick(object sender, RoutedEventArgs e)
        {
            int viewnum;
            if (sender is Button)
            {
                Button button = sender as Button;
                viewnum = int.Parse((string)button.Tag);
            }
            else
            {
                MenuItem item = sender as MenuItem;
                viewnum = int.Parse((string)item.Tag);
            }
            ViewChanged.Invoke(new ViewChangedEventArgs(viewnum));
        }
        #region Command
        private void OnHideOrShowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _isHide = !_isHide;
            if (_isHide)
            {
                ShowOrHideItem.HeaderTemplate = (DataTemplate)FindResource("ShowTemplate");
                HideOrShowItem.Header = string.Format("显示选项卡(_T)");
                HideOrShowColumn.Width = new GridLength(0);
            }
            else
            {
                ShowOrHideItem.HeaderTemplate = (DataTemplate)FindResource("HideTemplate");
                HideOrShowItem.Header = string.Format("隐藏选项卡(_T)");
                HideOrShowColumn.Width = new GridLength(ActualWidth / 5);
            }
        }
        private void ScrollToLeftCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ScrollExecute(ScrollDirection.Left);
        }

        private void ScrollToRightCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ScrollExecute(ScrollDirection.Right);
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
        private void ClosePageExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MainTab.CloseItem(MainTab.SelectedItem as HelpDocFrame);
        }

        private void ClosePageCanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainTab.SelectedItem != null;
        }
        #endregion
        
        private void ScrollToLeftAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = MainScroll.CanChangeHorizontalOffset;
            animation.To = Math.Max(0, MainScroll.CanChangeHorizontalOffset - 70);
            animation.Duration = new Duration(new TimeSpan(1500000));
            MainScroll.BeginAnimation(CanAnimationScroll.CanChangeHorizontalOffsetProperty,animation);
        }
        private void ScrollToRightAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = MainScroll.CanChangeHorizontalOffset;
            animation.To = MainScroll.CanChangeHorizontalOffset + Math.Min(70,MainScroll.ScrollableWidth);
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

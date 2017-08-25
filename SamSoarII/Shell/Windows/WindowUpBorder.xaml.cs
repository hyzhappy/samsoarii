using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// WindowUpBorder.xaml 的交互逻辑
    /// </summary>
    public partial class WindowUpBorder : UserControl
    {
        private MainWindow window;
        public WindowUpBorder()
        {
            InitializeComponent();
            Loaded += WindowUpBorder_Loaded;
        }

        private void WindowUpBorder_Loaded(object sender, RoutedEventArgs e)
        {
            window = (MainWindow)Application.Current.MainWindow;
            window.StateChanged += Window_StateChanged;
            window.IFParent.UDManager.InformationsCountChanged += UDManager_InformationsCountChanged;
        }

        private void UDManager_InformationsCountChanged(object sender, EventArgs e)
        {
            if (window.IFParent.UDManager.View.Informations.Count == 0) InformButton.Background = Brushes.Transparent;
            else InformButton.Background = Brushes.OrangeRed;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (window.WindowState)
            {
                case WindowState.Normal:
                    var root = (Image)ResizeButton.Template.FindName("Image", ResizeButton);
                    root.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Image/Maximized.png"));
                    ResizeButton.ToolTip = Properties.Resources.Maximize;
                    break;
                case WindowState.Maximized:
                    root = (Image)ResizeButton.Template.FindName("Image", ResizeButton);
                    root.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Image/Custom.png"));
                    ResizeButton.ToolTip = Properties.Resources.Restore;
                    break;
                default:
                    break;
            }
        }

        int timestamp = 0;
        private void OnDragWindow(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (window.WindowState == WindowState.Maximized && timestamp > 0 && e.Timestamp - timestamp < 150) window.WindowState = WindowState.Normal;
                window.DragMove();
            }
        }
        

        private void OnCloseWindow(object sender, RoutedEventArgs e)
        {
            window.Close();
        }

        private void OnMinimizedWindow(object sender, RoutedEventArgs e)
        {
            window.WindowState = WindowState.Minimized;
        }
        private void ChangeWindowState()
        {
            switch (window.WindowState)
            {
                case WindowState.Normal:
                    window.WindowState = WindowState.Maximized;
                    break;
                case WindowState.Maximized:
                    window.WindowState = WindowState.Normal;
                    break;
                default:
                    break;
            }
        }

        private void OnResizeWindow(object sender, RoutedEventArgs e)
        {
            ChangeWindowState();
        }
        private void OnInformWindow(object sender, RoutedEventArgs e)
        {
            var model = (LayoutAnchorable)window.LACInform.Model;
            if (model.IsActive)
            {
                window.LACInform.Hide();
                if (model.IsDock || model.IsFloat)
                    model.Hide();
            }
            else
            {
                window.LACInform.Show();
            }
            if(!model.IsHidden && !model.IsAutoHidden && !model.IsActive)
            {
                window.LACInform.Hide();
                model.Hide();
            }
        }
        
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ChangeWindowState();
            }
            timestamp = e.Timestamp;
        }
    }
}

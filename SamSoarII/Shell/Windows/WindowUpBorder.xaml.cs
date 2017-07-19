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
            window = (MainWindow)Application.Current.MainWindow;
            DataContext = window;
            window.StateChanged += Window_StateChanged;
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

        private void OnDragWindow(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (window.WindowState == WindowState.Maximized) window.WindowState = WindowState.Normal;
                window.DragMove();
            }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ChangeWindowState();
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
    }
}

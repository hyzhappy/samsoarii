using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static SamSoarII.Utility.Delegates;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// LoadingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public static readonly DependencyProperty MessageProperty;
        public static readonly DependencyProperty OffsetProperty;
        static LoadingWindow()
        {
            PropertyMetadata metadata = new PropertyMetadata();
            metadata.PropertyChangedCallback += OnOffsetPropertyChanged;
            MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(LoadingWindow));
            OffsetProperty = DependencyProperty.Register("Offset", typeof(double), typeof(LoadingWindow), metadata);
        }
        public static void OnOffsetPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            Canvas.SetLeft(((LoadingWindow)source)?.rect, ((LoadingWindow)source).Offset);
        }

        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }
        public double Offset
        {
            get
            {
                return (double)GetValue(OffsetProperty);
            }
            set
            {
                SetValue(OffsetProperty, value);
            }
        }
        private Rectangle rect;
        public LoadingWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += LoadingWindow_Loaded;
            rect = new Rectangle();
            rect.Width = 100;
            rect.Height = 18;
            Color color = new Color();
            color.A = 0xff;
            color.R = 0x47;
            color.G = 0x6f;
            color.B = 0xf7;
            rect.Fill = new SolidColorBrush(color);
        }
        private void LoadingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            canvas.Children.Add(rect);
            Panel.SetZIndex(rect, 1);
            StartAnimation();
        }
        public void StartAnimation()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = -80;
            animation.To = 598;
            animation.Duration = TimeSpan.FromSeconds(3.5);
            animation.RepeatBehavior = new RepeatBehavior(2000);
            BeginAnimation(OffsetProperty, animation);
        }
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    }


    public class LoadingWindowHandle
    {
        /// <summary>
        /// 表示打开窗口的线程已启动
        /// </summary>
        private bool started = false;
        protected Thread uiThread;
        private LoadingWindow loadWin;
        private string _message;
        private bool _completed = false;
        /// <summary>
        /// params="Completed"
        /// 表示整个耗时操作的完成!
        /// </summary>
        public bool Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;
            }
        }
        public LoadingWindowHandle(string message)
        {
            _message = message;
        }
        public void Start()
        {
            started = true;
            uiThread = new Thread(ThreadStartingPoint);
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.IsBackground = true;
            uiThread.Start();
        }
        public void Abort()
        {
            //Completed = true;窗口关闭不代表耗时操作已完成，比如可能被Messagebox阻塞

            while (started && loadWin == null)
            {
                Thread.Sleep(10);//线程已启动，但还未执行ThreadStartingPoint方法时，需等待
            }
            if (started)
            {
                loadWin.Dispatcher.Invoke(new Execute(() =>
                {
                    loadWin.Close();
                    loadWin = null;
                    started = false;
                }));
            }
        }

        private void ThreadStartingPoint()
        {
            loadWin = new LoadingWindow();
            loadWin.Message = _message;
            loadWin.ShowDialog();
            Dispatcher.Run();
        }
    }
}

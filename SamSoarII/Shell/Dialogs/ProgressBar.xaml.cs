using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// ProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBar : Window
    {
        private string message;
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }
        public ProgressBar()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += ProgressBar_Loaded;
        }

        private void ProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }

    public class ProgressBarHandle
    {
        /// <summary>
        /// 表示打开窗口的线程已启动
        /// </summary>
        private bool started = false;
        protected Thread uiThread;
        private ProgressBar pg_Bar;
        public ProgressBar PG_Bar { get { return pg_Bar; } }
        private string _message;
        private bool _completed = false;
        private int minimum;
        private int maximum;
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
        public ProgressBarHandle(string message,int Minimum,int Maximum)
        {
            _message = message;
            minimum = Minimum;
            maximum = Maximum;
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

            while (started && pg_Bar == null)
            {
                Thread.Sleep(10);//线程已启动，但还未执行ThreadStartingPoint方法时，需等待
            }
            if (started)
            {
                pg_Bar.Dispatcher.Invoke(new Execute(() =>
                {
                    pg_Bar.Close();
                    pg_Bar = null;
                    started = false;
                }));
            }
        }

        private void ThreadStartingPoint()
        {
            pg_Bar = new ProgressBar();
            pg_Bar.PG_Bar.Minimum = minimum;
            pg_Bar.PG_Bar.Maximum = maximum;
            pg_Bar.Message = _message;
            pg_Bar.ShowDialog();
            Dispatcher.Run();
        }
        public void StartAnimation(double from, double to,double duration)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = from;
            animation.To = to;
            animation.Duration = TimeSpan.FromSeconds(duration);
            pg_Bar.PG_Bar.BeginAnimation(RangeBase.ValueProperty, animation);
        }
    }
}

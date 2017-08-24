using SamSoarII.Core.Communication;
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
    public partial class ProgressBar : Window ,INotifyPropertyChanged
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
                PropertyChanged(this,new PropertyChangedEventArgs("Message"));
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }

    public class ProgressBarHandle : IDisposable
    {
        /// <summary>
        /// 表示打开窗口的线程已启动
        /// </summary>
        private bool started = false;
        //区别于主UI的UI线程
        Thread uiThread;
        private ProgressBar pg_Bar;
        //用于执行后台耗时操作的工作对象
        private BackgroundWorker worker;
        //用于执行的后台方法
        Action<ProgressBarHandle> action;
        //调用后台方法的Dispatcher
        Dispatcher dispatcher;
        public Dispatcher Dispatcher { get { return dispatcher; } }
        private string _message;
        //进度条的最小最大值
        private int minimum;
        private int maximum;
        
        public ProgressBarHandle(string message,int Minimum,int Maximum,Action<ProgressBarHandle> handle, BackgroundWorker worker,Dispatcher dispatcher)
        {
            _message = message;
            minimum = Minimum;
            maximum = Maximum;
            action = handle;
            
            this.worker = worker;
            this.dispatcher = dispatcher;
            worker.WorkerReportsProgress = true;
            worker.DoWork += DoWork;
        }

        #region Progress
        public void ReportProgress(int percentProgress,double timeSpan)
        {
            pg_Bar?.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                StartAnimation(pg_Bar.PG_Bar.Value, percentProgress, timeSpan);
            });
        }
        
        private void StartAnimation(double from, double to, double duration)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = from;
            animation.To = to;
            animation.Duration = TimeSpan.FromSeconds(duration);
            pg_Bar.PG_Bar.BeginAnimation(RangeBase.ValueProperty, animation);
        }
        #endregion

        #region 耗时操作
        public void StartWork()
        {
            worker.RunWorkerAsync();
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            Start();
            dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                action.Invoke(this);
            });
        }
        #endregion

        #region ui thread (start/end)
        private void ThreadStartingPoint()
        {
            pg_Bar = new ProgressBar();
            pg_Bar.PG_Bar.Minimum = minimum;
            pg_Bar.PG_Bar.Maximum = maximum;
            pg_Bar.Message = _message;
            pg_Bar.ShowDialog();
            Dispatcher.Run();
        }

        private void Start()
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
        #endregion
        public void UpdateMessage(string message)
        {
            pg_Bar?.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                pg_Bar.Message = message;
            });
        }

        public void Dispose()
        {
            worker = null;
            dispatcher = null;
            action = null;
        }
    }
}

using SamSoarII.Simulation.Core.Global;
using SamSoarII.Simulation.UI.Chart.Window;
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

namespace SamSoarII.Simulation.UI.Chart
{
    /// <summary>
    /// TimeRuler.xaml 的交互逻辑
    /// </summary>
    public partial class TimeRuler : UserControl
    {
        private const double DesignWidth = 800;
        private const double DesignHeight = 32;

        private double actualwidth;
        public new double ActualWidth
        {
            get { return this.actualwidth; }
            set
            {
                this.actualwidth = value;
                GlobalSetting.RulerScaleX = actualwidth / DesignWidth;
                GlobalSetting.RulerScaleY = 1.0;
            }
        }
        
        private double timestart;
        private double timescale;

        private int divnum;
        private int subdivnum;

        public double TimeStart
        {
            get { return this.timestart; }
            set { this.timestart = value; }
        }

        public double TimeScale
        {
            get { return this.timescale; }
            set { this.timescale = value; }
        }

        public double IntevalWidth
        {
            get { return DesignWidth / (DivideNumber * SubDivideNumber); }
        }

        public double TimeEnd
        {
            get { return this.timestart + DesignWidth * TimeScale; }
            set { TimeScale = (value - TimeStart) / DesignWidth; }
        }
        
        public int DivideNumber
        {
            get { return this.divnum; }
            set { this.divnum = value; }
        }

        public int SubDivideNumber
        {
            get { return this.subdivnum; }
            set { this.subdivnum = value; }
        }
        
        public TimeRuler()
        {
            InitializeComponent();
            LayoutTransform = GlobalSetting.RulerScaleTransform;
            this.timestart = 0.0;
            this.timescale = 1.0;
            this.divnum = 20;
            this.subdivnum = 5;
            StartPointLine = new Line();
            StartPointLine.Y1 = 0;
            StartPointLine.Y2 = DesignHeight;
            StartPointLine.StrokeThickness = 3;
            StartPointLine.Stroke = Brushes.LightPink;
            StartPointLine.Opacity = 0.0;
            EndPointLine = new Line();
            EndPointLine.Y1 = 0;
            EndPointLine.Y2 = DesignHeight;
            EndPointLine.StrokeThickness = 3;
            EndPointLine.Stroke = Brushes.LightGray;
            EndPointLine.Opacity = 0.0;
            Update();
        }

        private void Update()
        {
            MainCanva.Children.Clear();
            double x = 0;
            double itv = IntevalWidth;
            for (int i = 0; i < DivideNumber; i++)
                for (int j = 0; j < SubDivideNumber; j++)
                {
                    Line line = new Line();
                    line.X1 = line.X2 = x;
                    line.Y1 = 0;
                    line.Stroke = Brushes.AntiqueWhite;
                    if (j == 0)
                    {
                        line.Y2 = DesignHeight;
                        line.StrokeThickness = 2;
                        TextBlock tblock = new TextBlock();
                        tblock.Text = String.Format("{0} ms", timestart + x * timescale);
                        tblock.FontSize = 10;
                        tblock.Foreground = Brushes.AntiqueWhite;
                        Canvas.SetTop(tblock, 18);
                        Canvas.SetLeft(tblock, x);
                        MainCanva.Children.Add(tblock);
                    }
                    else
                    {
                        line.Y2 = DesignHeight/3;
                        line.StrokeThickness = 1;
                    }
                    MainCanva.Children.Add(line);
                    x += itv;
                }
            MainCanva.Children.Add(StartPointLine);
            MainCanva.Children.Add(EndPointLine);
            MainCanva.Width = x;
            this.Width = MainCanva.Width;
        }

        #region StartPoint & EndPoint
        private const int POINT_FREE = 0x00;
        private const int POINT_STARTSETTING = 0x01;
        private const int POINT_STARTLOCK = 0x02;
        private const int POINT_ENDSETTING = 0x04;
        private const int POINT_ENDLOCK = 0x08;

        private int pstatus = 0;
        private int PStatus
        {
            get
            {
                return this.pstatus;
            }
            set
            {
                this.pstatus = value;
                if ((pstatus & POINT_STARTSETTING) != 0 ||
                    (pstatus & POINT_STARTLOCK) != 0)
                {
                    StartPointLine.Opacity = 1.0;
                }
                else
                {
                    StartPointLine.Opacity = 0.0;
                    PointStartDisable(this, new RoutedEventArgs());
                }
                if ((pstatus & POINT_ENDSETTING) != 0 ||
                    (pstatus & POINT_ENDLOCK) != 0)
                {
                    EndPointLine.Opacity = 1.0;
                }
                else
                {
                    EndPointLine.Opacity = 0.0;
                    PointEndDisable(this, new RoutedEventArgs());
                }
                if ((pstatus & POINT_STARTSETTING) != 0)
                {
                    if (TPW_StartPoint == null)
                    {
                        TPW_StartPoint = new TimePointWindow();
                        TPW_StartPoint.Title = "设置起点";
                        TPW_StartPoint.ValueChanged += TPW_StartPoint_ValueChanged;
                        TPW_StartPoint.B_ok.Click += TPW_StartPoint_B_ok_Click;
                        TPW_StartPoint.Closed += TPW_StartPoint_Closed;
                        TPW_StartPoint.Show();
                    }
                }
                else
                {
                    if (TPW_StartPoint != null)
                    {
                        PointStart = TPW_StartPoint.Value;
                        PointStartEnable(this, new RoutedEventArgs());
                        TPW_StartPoint.Close();
                        TPW_StartPoint = null;
                    }
                }
                if ((pstatus & POINT_ENDSETTING) != 0)
                {
                    if (TPW_EndPoint == null)
                    {
                        TPW_EndPoint = new TimePointWindow();
                        TPW_EndPoint.Title = "设置终点";
                        TPW_EndPoint.ValueChanged += TPW_EndPoint_ValueChanged;
                        TPW_EndPoint.B_ok.Click += TPW_EndPoint_B_ok_Click;
                        TPW_EndPoint.Closed += TPW_EndPoint_Closed;
                        TPW_EndPoint.Show();
                    }
                }
                else
                {
                    if (TPW_EndPoint != null)
                    {
                        PointEnd = TPW_EndPoint.Value;
                        PointEndEnable(this, new RoutedEventArgs());
                        TPW_EndPoint.Close();
                        TPW_EndPoint = null;
                    }
                }
                if ((pstatus & POINT_STARTLOCK) != 0)
                {
                    MI_StartPoint.Header = "取消起点";
                }
                else
                {
                    MI_StartPoint.Header = "设置起点";
                }

                if ((pstatus & POINT_ENDLOCK) != 0)
                {
                    MI_EndPoint.Header = "取消终点";
                }
                else
                {
                    MI_EndPoint.Header = "设置终点";
                }
                StartLineChanged(StartPointLine, new RoutedEventArgs());
                EndLineChanged(EndPointLine, new RoutedEventArgs());
            }
        }
        
        private double pointstart;
        private double pointend;
        public double PointStart
        {
            get
            {
                return this.pointstart;
            }
            set
            {
                this.pointstart = value;
            }
        }
        public double PointEnd
        {
            get
            {
                return this.pointend;
            }
            set
            {
                this.pointend = value;
            }
        }
        
        private Line StartPointLine;

        private Line EndPointLine;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point p = e.GetPosition(this);
            int ix = (int)((p.X / DesignWidth) * (DivideNumber * SubDivideNumber));
            double px = (ix * DesignWidth) / (DivideNumber * SubDivideNumber);
            double tx = TimeStart + px * TimeScale;
            if ((PStatus & POINT_STARTSETTING) != 0)
            {
                TPW_StartPoint.Value = tx;
            }
            if ((PStatus & POINT_ENDSETTING) != 0)
            {
                TPW_EndPoint.Value = tx;
            }
            
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if ((PStatus & POINT_STARTSETTING) != 0)
            {
                try
                {
                    PointStart = TPW_StartPoint.Value;
                    PStatus &= ~(POINT_STARTSETTING);
                    PStatus |= POINT_STARTLOCK;
                }
                catch (FormatException)
                {
                }
            }
            if ((PStatus & POINT_ENDSETTING) != 0)
            {
                try
                {
                    PointStart = TPW_EndPoint.Value;
                    PStatus &= ~(POINT_ENDSETTING);
                    PStatus |= POINT_ENDLOCK;
                }
                catch (FormatException)
                {
                }
            }
        }
        #endregion

        #region Event Handler

        public event RoutedEventHandler StartLineChanged = delegate { };

        public event RoutedEventHandler EndLineChanged = delegate { };

        public event RoutedEventHandler StartTimeChanged = delegate { };

        public event RoutedEventHandler EndTimeChanged = delegate { };

        public event RoutedEventHandler PointStartEnable = delegate { };

        public event RoutedEventHandler PointEndEnable = delegate { };

        public event RoutedEventHandler PointStartDisable = delegate { };

        public event RoutedEventHandler PointEndDisable = delegate { };

        private TimePointWindow TPW_StartPoint = null;

        private void MI_StartPoint_Click(object sender, RoutedEventArgs e)
        {
            if ((PStatus & POINT_STARTLOCK) != 0)
            {
                PStatus &= ~(POINT_STARTLOCK);
            }
            else if ((PStatus & POINT_STARTSETTING) == 0)
            {
                PStatus |= POINT_STARTSETTING;
            }
        }

        private void TPW_StartPoint_B_ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //PointStart = TPW_StartPoint.Value;
                PStatus &= ~(POINT_STARTSETTING);
                PStatus |= POINT_STARTLOCK;
            }
            catch (FormatException)
            {
            }
        }

        private void TPW_StartPoint_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TimePointWindow)
            {
                TimePointWindow tpwin = (TimePointWindow)(sender);
                try
                {
                    double px = (tpwin.Value - TimeStart) / TimeScale;
                    px = Math.Max(px, 0);
                    px = Math.Min(px, DesignWidth);
                    StartPointLine.Opacity = 1.0;
                    StartPointLine.X1 = StartPointLine.X2 = px;
                }
                catch (FormatException)
                {
                    StartPointLine.Opacity = 0.0;
                }
                StartLineChanged(StartPointLine, new RoutedEventArgs());
            }
        }

        private void TPW_StartPoint_Closed(object sender, EventArgs e)
        {
            if ((PStatus & POINT_STARTSETTING) != 0)
            {
                PStatus &= ~(POINT_STARTSETTING);
            }
        }

        private TimePointWindow TPW_EndPoint = null;

        private void MI_EndPoint_Click(object sender, RoutedEventArgs e)
        {
            if ((PStatus & POINT_ENDLOCK) != 0)
            {
                PStatus &= ~(POINT_ENDLOCK);
            }
            else if ((PStatus & POINT_ENDSETTING) == 0)
            {
                PStatus |= POINT_ENDSETTING;
            }
        }

        private void TPW_EndPoint_B_ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //PointEnd = TPW_EndPoint.Value;
                PStatus &= ~(POINT_ENDSETTING);
                PStatus |= POINT_ENDLOCK;
            }
            catch (FormatException)
            {
            }
        }

        private void TPW_EndPoint_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TimePointWindow)
            {
                TimePointWindow tpwin = (TimePointWindow)(sender);
                try
                {
                    double px = (tpwin.Value - TimeStart) / TimeScale;
                    px = Math.Max(px, 0);
                    px = Math.Min(px, DesignWidth);
                    EndPointLine.Opacity = 1.0;
                    EndPointLine.X1 = EndPointLine.X2 = px;
                }
                catch (FormatException)
                {
                    EndPointLine.Opacity = 0.0;
                }
                EndLineChanged(EndPointLine, new RoutedEventArgs());
            }
        }

        private void TPW_EndPoint_Closed(object sender, EventArgs e)
        {
            if ((PStatus & POINT_ENDSETTING) != 0)
            {
                PStatus &= ~(POINT_ENDSETTING);
            }
        }

        private void MI_Setting_Click(object sender, RoutedEventArgs e)
        {
            GlobalSetting.TimeRulerStart = (int)(TimeStart);
            GlobalSetting.TimeRulerEnd = (int)(TimeEnd);
            GlobalSetting.TimeRulerDivideNumber = DivideNumber;
            GlobalSetting.TimeRulerSubDivideNumber = SubDivideNumber;
            SettingWindow swin = new SettingWindow();
            swin.EnsureButtonClick += W_Setting_Ensure_Click;
            swin.Show();
        }

        private void W_Setting_Ensure_Click(object sender, RoutedEventArgs e)
        {
            DivideNumber = GlobalSetting.TimeRulerDivideNumber;
            SubDivideNumber = GlobalSetting.TimeRulerSubDivideNumber;
            TimeStart = (double)(GlobalSetting.TimeRulerStart);
            TimeEnd = (double)(GlobalSetting.TimeRulerEnd);
            Update();
            StartTimeChanged(this, new RoutedEventArgs());
            EndTimeChanged(this, new RoutedEventArgs());
        }
        #endregion
    }
}

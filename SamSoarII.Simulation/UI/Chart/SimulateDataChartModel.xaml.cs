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

using SamSoarII.Simulation.Core.DataModel;
using SamSoarII.Simulation.Core.Global;

namespace SamSoarII.Simulation.UI.Chart
{
    /// <summary>
    /// SimulateDataChartModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimulateDataChartModel : UserControl
    {
        private SimulateDataModel sdmodel;
        private TimeRuler truler;

        private const double DesignWidth = 800;
        private const double DesignHeight = 40;


        private double actualwidth;
        public new double ActualWidth
        {
            get { return this.actualwidth; }
            set
            {
                if (this.actualwidth != value)
                {
                    this.actualwidth = value;
                    Update();
                }
            }
        }

        public SimulateDataModel SDModel
        {
            get { return this.sdmodel; }
            set { Setup(value); }
        }

        public TimeRuler TRuler
        {
            get { return this.truler; }
            set
            {
                if (this.truler != null)
                {
                    this.truler.StartTimeChanged -= OnTimeRulerChanged;
                    this.truler.EndTimeChanged -= OnTimeRulerChanged;
                }
            }
        }

        private void OnTimeRulerChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public SimulateDataChartModel()
        {
            InitializeComponent();
            LayoutTransform = GlobalSetting.RulerScaleTransform;
        }

        public SimulateDataChartModel(SimulateDataModel _sdmodel, TimeRuler _truler)
        {
            InitializeComponent();
            truler = _truler;
            Setup(_sdmodel);
        }
        
        public void Setup(SimulateDataModel _sdmodel)
        {
            this.sdmodel = _sdmodel;
            Update();
        }

        public void Update()
        {
            if (truler == null)
            {
                return;
            }
            double rts = truler.TimeStart;
            double rte = truler.TimeEnd;
            Line hline, vline;
            Rectangle rect;
            TextBlock vtext;
            int pbit = 0;

            MainCanva.Children.Clear();

            if (SDModel.IsLock)
            {
                rect = new Rectangle();
                Canvas.SetTop(rect, 0);
                Canvas.SetLeft(rect, 0);
                rect.Width = ActualWidth;
                rect.Height = DesignHeight;
                rect.Fill = Brushes.LightGreen;
                MainCanva.Children.Add(rect);
            }

            if (SDModel.IsView)
            {
                rect = new Rectangle();
                Canvas.SetTop(rect, 0);
                Canvas.SetLeft(rect, 0);
                rect.Width = ActualWidth;
                rect.Height = DesignHeight;
                rect.Fill = Brushes.LightCoral;
                MainCanva.Children.Add(rect);
            }

            foreach (ValueSegment vs in sdmodel.Values)
            {
                double vts = vs.TimeStart;
                double vte = vs.TimeEnd;
                if (vts < rts) vts = rts;
                if (vte > rte) vte = rte;
                if (vts > vte) continue;
                if (vs is IntSegment && sdmodel.Type.Equals("BIT"))
                {
                    hline = new Line();
                    hline.X1 = ActualWidth * (vts - rts) / (rte - rts);
                    hline.X2 = ActualWidth * (vte - rts) / (rte - rts);
                    hline.Stroke = Brushes.Black;
                    hline.StrokeThickness = 2;

                    vline = new Line();
                    vline.X1 = vline.X2 = hline.X1;
                    vline.Y1 = 2;
                    vline.Y2 = DesignHeight - 2;
                    vline.Stroke = Brushes.Black;
                    vline.StrokeThickness = 2;

                    switch ((int)(vs.Value))
                    {
                        case 0:
                            hline.Y1 = hline.Y2 = DesignHeight - 2;
                            if (pbit == 1)
                            {
                                MainCanva.Children.Add(vline);
                            }
                            pbit = 0;
                            break;
                        case 1:
                            hline.Y1 = hline.Y2 = 2;
                            if (pbit == 0)
                            {
                                MainCanva.Children.Add(vline);
                            }
                            pbit = 1;
                            break;
                        default:
                            break;
                     }
                     MainCanva.Children.Add(hline);
                }
                else
                {
                    vline = new Line();
                    vline.X1 = vline.X2 = ActualWidth * (vts - rts) / (rte - rts);
                    vline.Y1 = 2;
                    vline.Y2 = DesignHeight - 2;
                    vline.Stroke = Brushes.Black;
                    vline.StrokeThickness = 2;
                    MainCanva.Children.Add(vline);

                    vtext = new TextBlock();
                    Canvas.SetTop(vtext, 10);
                    Canvas.SetLeft(vtext, vline.X1);
                    vtext.Text = vs.Value.ToString();
                    MainCanva.Children.Add(vtext);

                    vline = new Line();
                    vline.X1 = vline.X2 = ActualWidth * (vte - rts) / (rte - rts);
                    vline.Y1 = 2;
                    vline.Y2 = DesignHeight - 2;
                    vline.Stroke = Brushes.Black;
                    vline.StrokeThickness = 2;
                    MainCanva.Children.Add(vline);
                }
            }
        }
    }
}

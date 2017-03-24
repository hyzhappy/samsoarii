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
            this.timestart = 0.0;
            this.timescale = 1.0;
            this.divnum = 20;
            this.subdivnum = 5;
            Update();
        }

        private void Update()
        {
            MainCanva.Children.Clear();
            int x = 0;
            for (int i = 0; i < DivideNumber; i++)
                for (int j = 0; j < SubDivideNumber; j++)
                {
                    Line line = new Line();
                    line.X1 = line.X2 = x;
                    line.Y1 = 0;
                    line.Stroke = Brushes.AntiqueWhite;
                    if (j == 0)
                    {
                        line.Y2 = 30;
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
                        line.Y2 = 10;
                        line.StrokeThickness = 1;
                    }
                    MainCanva.Children.Add(line);
                    x += 10;
                }
            MainCanva.Width = x;
            this.Width = MainCanva.Width;
        }
        

    }
}

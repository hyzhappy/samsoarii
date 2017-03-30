using SamSoarII.Simulation.Core.Global;
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
    /// ValueRuler.xaml 的交互逻辑
    /// </summary>
    public partial class ValueRuler : UserControl
    {
        private const double DesignWidth = 32;
        private const double DesignHeight = 600;

        private double actualheight;
        public new double ActualHeight
        {
            get { return this.actualheight; }
            set
            {
                this.actualheight = value;
                GlobalSetting.RulerScaleX = actualheight / DesignHeight;
                GlobalSetting.RulerScaleY = 1.0;
            }
        }

        private double valuestart;
        private double valuescale;

        private int divnum;
        private int subdivnum;

        public double ValueStart
        {
            get { return this.valuestart; }
            set { this.valuestart = value; }
        }

        public double ValueScale
        {
            get { return this.valuescale; }
            set { this.valuescale = value; }
        }

        public double IntevalHeight
        {
            get { return DesignHeight / (DivideNumber * SubDivideNumber); }
        }

        public double ValueEnd
        {
            get { return this.valuestart + DesignHeight * ValueScale; }
            set { ValueScale = (value - ValueStart) / DesignHeight; }
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

        public ValueRuler()
        {
            InitializeComponent();
            LayoutTransform = GlobalSetting.RulerScaleTransform;
            this.valuestart = 0.0;
            this.valuescale = 1.0;
            this.divnum = 20;
            this.subdivnum = 5;
            Update();
        }

        private void Update()
        {
            MainCanva.Children.Clear();
            double y = 0;
            double itv = IntevalHeight;
            for (int i = 0; i < DivideNumber; i++)
                for (int j = 0; j < SubDivideNumber; j++)
                {
                    Line line = new Line();
                    line.Y1 = line.Y2 = y;
                    line.X1 = 0;
                    line.Stroke = Brushes.AntiqueWhite;
                    if (j == 0)
                    {
                        line.X2 = DesignWidth;
                        line.StrokeThickness = 2;
                        TextBlock tblock = new TextBlock();
                        tblock.Text = String.Format("{0} ms", valuestart + x * valuescale);
                        tblock.FontSize = 10;
                        tblock.Foreground = Brushes.AntiqueWhite;
                        Canvas.SetLeft(tblock, 0);
                        Canvas.SetTop(tblock, y);
                        MainCanva.Children.Add(tblock);
                    }
                    else
                    {
                        line.X2 = DesignWidth / 3;
                        line.StrokeThickness = 1;
                    }
                    MainCanva.Children.Add(line);
                    y += itv;
                }
            MainCanva.Height = DesignHeight;
            this.Height = MainCanva.Height;
        }
    }
}

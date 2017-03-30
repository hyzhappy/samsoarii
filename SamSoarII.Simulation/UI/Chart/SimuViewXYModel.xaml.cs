using SamSoarII.Simulation.Core.DataModel;
using SamSoarII.Simulation.UI.Base;
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
    /// SimuViewXYModel.xaml 的交互逻辑
    /// </summary>
    public partial class SimuViewXYModel : SimuViewTabModel
    {
        public const int CanvaWidth = 800;
        public const int CanvaHeight = 600;

        private SimulateDataModel sdmodel;

        public SimuViewXYModel()
        {
            InitializeComponent();
        }

        public SimuViewXYModel(SimulateDataModel _sdmodel, int timestart, int timeend)
        {
            Setup(sdmodel, timestart, timeend);
        }

        public void Setup(SimulateDataModel _sdmodel, int timestart, int timeend)
        {
            this.sdmodel = _sdmodel;
            sdmodel.SortByTime();
            double min = 1e50;
            double max = -1e50;
            foreach (ValueSegment vs in sdmodel.Values)
            {
                int vts = vs.TimeStart;
                int vte = vs.TimeEnd;
                if (vts < timestart) vts = timestart;
                if (vte > timeend) vte = timeend;
                if (vts >= vte) continue;
                if (vs is IntSegment)
                {
                    min = Math.Min(min, (double)((int)(vs.Value)));
                    max = Math.Max(max, (double)((int)(vs.Value)));
                }
                if (vs is FloatSegment)
                {
                    min = Math.Min(min, (double)((float)(vs.Value)));
                    max = Math.Max(max, (double)((float)(vs.Value)));
                }
                if (vs is DoubleSegment)
                {
                    min = Math.Min(min, (double)(vs.Value));
                    max = Math.Max(max, (double)(vs.Value));
                }
            }
            XRuler.TimeStart = timestart;
            XRuler.TimeEnd = timeend;
            YRuler.ValueStart = min;
            YRuler.ValueEnd = max;
        }

        public void Update()
        {
            double vts, vte;
            double v = 0, vp = YRuler.ValueStart;
            Line lineh, linev;
            foreach (ValueSegment vs in sdmodel.Values)
            {
                vts = vs.TimeStart;
                vte = vs.TimeEnd;
                if (vts < XRuler.TimeStart) vts = XRuler.TimeStart;
                if (vte > XRuler.TimeEnd) vte = XRuler.TimeEnd;
                if (vts >= vte) continue;
                if (vs is IntSegment)
                {
                    v = (double)((int)(vs.Value));
                }
                if (vs is FloatSegment)
                {
                    v = (double)((float)(vs.Value));
                }
                if (vs is DoubleSegment)
                {
                    v = (double)(vs.Value);
                }
                linev = new Line();
                linev.Y1 = CanvaHeight * (vp - YRuler.ValueStart) / (YRuler.ValueEnd - YRuler.ValueStart);
                linev.Y2 = CanvaHeight * (v - YRuler.ValueStart) / (YRuler.ValueEnd - YRuler.ValueStart);
                linev.X1 = linev.X2 = CanvaWidth * (vts - XRuler.TimeStart) / (XRuler.TimeEnd - XRuler.TimeStart);
                linev.Stroke = Brushes.Black;
                linev.StrokeThickness = 1;
                MainCanva.Children.Add(linev);
                lineh = new Line();
                lineh.X1 = linev.X1;
                lineh.X2 = CanvaWidth * (vts - XRuler.TimeStart) / (XRuler.TimeEnd - XRuler.TimeStart);
                lineh.Y1 = lineh.Y2 = linev.Y2;
                lineh.Stroke = Brushes.Black;
                lineh.StrokeThickness = 1;
                MainCanva.Children.Add(lineh);
            }
        }


        #region Actual Size
        protected override void OnActualWidthChanged()
        {
            base.OnActualWidthChanged();

            XRuler.ActualWidth = ActualWidth - 32;
        }

        protected override void OnActualHeightChanged()
        {
            base.OnActualHeightChanged();

            YRuler.ActualHeight = ActualHeight - 32;
        }
        #endregion

    }
}

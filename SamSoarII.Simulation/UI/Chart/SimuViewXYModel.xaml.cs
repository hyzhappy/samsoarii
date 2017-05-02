using SamSoarII.Simulation.Core.DataModel;
using SamSoarII.Simulation.Core.Global;
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

        private IEnumerable<SimulateDataModel> sdmodels;
        private int timestart;
        private int timeend;

        private Brush[] brushes;
        
        public SimuViewXYModel()
        {
            InitializeComponent();
            brushes = GlobalSetting.DrawBrushes;
        }

        public SimuViewXYModel(SimulateDataModel _sdmodel, int timestart, int timeend)
        {
            Setup(_sdmodel, timestart, timeend);
        }
        
        public SimuViewXYModel(IEnumerable<SimulateDataModel> _sdmodels, int timestart, int timeend)
        {
            Setup(_sdmodels, timestart, timeend);
        }

        public void Setup(IEnumerable<SimulateDataModel> _sdmodels, int timestart, int timeend)
        {
            this.sdmodels = _sdmodels;
            this.timestart = timestart;
            this.timeend = timeend;
            _Setup();
        }

        public void Setup(SimulateDataModel _sdmodel, int timestart, int timeend)
        {
            List<SimulateDataModel> _sdmodels = new List<SimulateDataModel>();
            _sdmodels.Add(_sdmodel);
            this.sdmodels = _sdmodels;
            this.timestart = timestart;
            this.timeend = timeend;
            _Setup();
        }

        private void _Setup()
        {
            double min = 1e50;
            double max = -1e50;
            foreach (SimulateDataModel sdmodel in sdmodels)
            {
                sdmodel.SortByTime();
                foreach (ValueSegment vs in sdmodel.Values)
                {
                    int vts = vs.TimeStart;
                    int vte = vs.TimeEnd;
                    if (vts < timestart) vts = timestart;
                    if (vte > timeend) vte = timeend;
                    if (vts >= vte) continue;
                    if (vs is BitSegment || vs is WordSegment)
                    {
                        min = Math.Min(min, (double)((Int32)(vs.Value)));
                        max = Math.Max(max, (double)((Int32)(vs.Value)));
                    }
                    if (vs is DWordSegment)
                    {
                        min = Math.Min(min, (double)((Int64)(vs.Value)));
                        max = Math.Max(max, (double)((Int64)(vs.Value)));
                    }
                    if (vs is FloatSegment)
                    {
                        min = Math.Min(min, (double)(vs.Value));
                        max = Math.Max(max, (double)(vs.Value));
                    }
                }
            }
            XRuler.TimeStart = timestart;
            XRuler.TimeEnd = timeend;
            YRuler.ValueStart = min;
            YRuler.ValueEnd = max;
            Update();
        }

        public void Update()
        {
            double vts, vte;
            double v = 0, vp = YRuler.ValueStart;
            Line lineh, linev;
            int i = 0;
            foreach (SimulateDataModel sdmodel in sdmodels)
            {
                foreach (ValueSegment vs in sdmodel.Values)
                {
                    vts = vs.TimeStart;
                    vte = vs.TimeEnd;
                    if (vts < XRuler.TimeStart) vts = XRuler.TimeStart;
                    if (vte > XRuler.TimeEnd) vte = XRuler.TimeEnd;
                    if (vts >= vte) continue;
                    if (vs is BitSegment || vs is WordSegment)
                    {
                        v = (double)((Int32)vs.Value);
                    }
                    if (vs is DWordSegment)
                    {
                        v = (double)((Int64)vs.Value);
                    }
                    if (vs is FloatSegment)
                    {
                        v = (double)vs.Value;
                    }
                    linev = new Line();
                    linev.Y1 = CanvaHeight * (vp - YRuler.ValueStart) / (YRuler.ValueEnd - YRuler.ValueStart);
                    linev.Y2 = CanvaHeight * (v - YRuler.ValueStart) / (YRuler.ValueEnd - YRuler.ValueStart);
                    linev.X1 = linev.X2 = CanvaWidth * (vts - XRuler.TimeStart) / (XRuler.TimeEnd - XRuler.TimeStart);
                    linev.Stroke = brushes[i];
                    linev.StrokeThickness = 1;
                    MainCanva.Children.Add(linev);
                    lineh = new Line();
                    lineh.X1 = linev.X1;
                    lineh.X2 = CanvaWidth * (vts - XRuler.TimeStart) / (XRuler.TimeEnd - XRuler.TimeStart);
                    lineh.Y1 = lineh.Y2 = linev.Y2;
                    lineh.Stroke = brushes[i];
                    lineh.StrokeThickness = 1;
                    MainCanva.Children.Add(lineh);
                }
                i++;
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

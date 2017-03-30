using SamSoarII.Simulation.Core.DataModel;
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
    /// ValueChart.xaml 的交互逻辑
    /// </summary>
    public partial class ValueChart : UserControl
    {
        private LinkedList<SimulateDataChartModel> sdcmodels;
        public LinkedList<SimulateDataChartModel> SDCModels
        {
            get
            {
                return this.sdcmodels;
            }
            set
            {
                this.sdcmodels = value;
                Update();
            }
        }
        private double _actualWidth;
        public new double ActualWidth
        {
            get { return this._actualWidth; }
            set
            {
                this._actualWidth = value;
                foreach (SimulateDataChartModel sdcmodel in SDCModels)
                {
                    sdcmodel.ActualWidth = value;
                }
            }
        }

        private TimeRuler truler;

        public ValueChart()
        {
            InitializeComponent();
        }
        
        public void BuildRouted(SimuViewChartModel svcmodel)
        {
            svcmodel.SDModelSetup += OnSDModelSetup;
            svcmodel.SDModelClose += OnSDModelClose;
            svcmodel.SDModelLock += OnSDModelLock;
            svcmodel.SDModelView += OnSDModelView;
            svcmodel.SDModelUnlock += OnSDModelUnlock;
            svcmodel.SDModelUnview += OnSDModelUnview;
            truler = svcmodel.TRuler;
        }

        public void Update()
        {
            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
            RowDefinition rdef = null;
            int i = 0;
            foreach (SimulateDataChartModel sdcmodel in sdcmodels)
            {
                rdef = new RowDefinition();
                rdef.Height = new GridLength(40);
                MainGrid.RowDefinitions.Add(rdef);
                Grid.SetRow(sdcmodel, i++);
                Grid.SetColumn(sdcmodel, 0);
                MainGrid.Children.Add(sdcmodel);
            }
        }

        #region SimulateDataModel Manipulation
        public void Add(SimulateDataModel sdmodel, int id)
        {
            SimulateDataChartModel sdcmodel = new SimulateDataChartModel(sdmodel, truler);
            Add(sdcmodel, id);
        }

        public void Add(SimulateDataChartModel sdcmodel, int id)
        {
            if (id == sdcmodels.Count())
            {
                AddLast(sdcmodel);
                return;
            }
            RowDefinition rdef = new RowDefinition();
            rdef.Height = new GridLength(40);
            MainGrid.RowDefinitions.Add(rdef);
            SimulateDataChartModel _sdcmodel = sdcmodels.ElementAt(id);
            LinkedListNode<SimulateDataChartModel> nodestart = sdcmodels.Find(_sdcmodel);
            LinkedListNode<SimulateDataChartModel> node = nodestart;
            while (node != null)
            {
                _sdcmodel = node.Value;
                int row = Grid.GetRow(_sdcmodel);
                Grid.SetRow(_sdcmodel, row + 1);
                node = node.Next;
            }
            Grid.SetRow(sdcmodel, id);
            sdcmodels.AddBefore(nodestart, sdcmodel);
            MainGrid.Children.Add(sdcmodel);
        }

        public void AddLast(SimulateDataModel sdmodel)
        {
            SimulateDataChartModel sdcmodel = new SimulateDataChartModel(sdmodel, truler);
            AddLast(sdcmodel);
        }

        public void AddLast(SimulateDataChartModel sdcmodel)
        {
            RowDefinition rdef = new RowDefinition();
            rdef.Height = new GridLength(40);
            MainGrid.RowDefinitions.Add(rdef);
            int count = sdcmodels.Count();
            Grid.SetRow(sdcmodel, count);
            Grid.SetColumn(sdcmodel, 0);
            SDCModels.AddLast(sdcmodel);
            MainGrid.Children.Add(sdcmodel);
        }

        public void Remove(SimulateDataChartModel sdcmodel)
        {
            MainGrid.RowDefinitions.RemoveAt(0);
            LinkedListNode<SimulateDataChartModel> node = sdcmodels.Find(sdcmodel);
            node = node.Next;
            while (node != null)
            {
                SimulateDataChartModel _sdcmodel = node.Value;
                int row = Grid.GetRow(_sdcmodel);
                Grid.SetRow(_sdcmodel, row - 1);
                node = node.Next;
            }
            sdcmodels.Remove(sdcmodel);
            MainGrid.Children.Remove(sdcmodel);
        }

        #endregion

        #region Event Handler
        public event SimulateDataModelEventHandler SDModelClose;
        private void OnSDModelClose(object sender, SimulateDataModelEventArgs e)
        {
            if (e.SDCModel != null)
            {
                return;
            }
            if (sender is SimuViewChartModel)
            {
                foreach (SimulateDataChartModel sdcmodel in SDCModels)
                {
                    if (sdcmodel.SDModel == e.SDModel_old)
                    {
                        e.SDCModel = sdcmodel;
                        Remove(sdcmodel);
                        break;
                    }
                }
            }
        }

        public event SimulateDataModelEventHandler SDModelSetup;
        private void OnSDModelSetup(object sender, SimulateDataModelEventArgs e)
        {
            if (e.SDCModel != null)
            {
                return;
            }
            if (sender is SimuViewChartModel)
            {
                foreach (SimulateDataChartModel sdcmodel in SDCModels)
                {
                    if (sdcmodel.SDModel == e.SDModel_old)
                    {
                        sdcmodel.SDModel = e.SDModel_new;
                        e.SDCModel = sdcmodel;
                        break;
                    }
                }
                if (e.SDCModel == null)
                {
                    Add(e.SDModel_new, e.ID);
                }
            }
        }

        private void OnSDModelLock(object sender, SimulateDataModelEventArgs e)
        {
            if (e.SDCModel != null)
            {
                return;
            }
            if (sender is SimuViewChartModel)
            {
                foreach (SimulateDataChartModel sdcmodel in SDCModels)
                {
                    if (sdcmodel.SDModel == e.SDModel_old)
                    {
                        e.SDCModel = sdcmodel;
                        break;
                    }
                }
                if (e.SDCModel == null)
                {
                    throw new KeyNotFoundException();
                }
                e.SDCModel.MainCanva.Background = Brushes.Green;
            }
        }

        private void OnSDModelView(object sender, SimulateDataModelEventArgs e)
        {
            if (e.SDCModel != null)
            {
                return;
            }
            if (sender is SimuViewChartModel)
            {
                foreach (SimulateDataChartModel sdcmodel in SDCModels)
                {
                    if (sdcmodel.SDModel == e.SDModel_old)
                    {
                        e.SDCModel = sdcmodel;
                        break;
                    }
                }
                if (e.SDCModel == null)
                {
                    throw new KeyNotFoundException();
                }
                e.SDCModel.MainCanva.Background = Brushes.Beige;
            }
        }

        private void OnSDModelUnlock(object sender, SimulateDataModelEventArgs e)
        {
            if (e.SDCModel != null)
            {
                return;
            }
            if (sender is SimuViewChartModel)
            {
                foreach (SimulateDataChartModel sdcmodel in SDCModels)
                {
                    if (sdcmodel.SDModel == e.SDModel_old)
                    {
                        e.SDCModel = sdcmodel;
                        break;
                    }
                }
                if (e.SDCModel == null)
                {
                    throw new KeyNotFoundException();
                }
                e.SDCModel.MainCanva.Background = Brushes.Transparent;
            }
        }

        private void OnSDModelUnview(object sender, SimulateDataModelEventArgs e)
        {
            if (e.SDCModel != null)
            {
                return;
            }
            if (sender is SimuViewChartModel)
            {
                foreach (SimulateDataChartModel sdcmodel in SDCModels)
                {
                    if (sdcmodel.SDModel == e.SDModel_old)
                    {
                        e.SDCModel = sdcmodel;
                        break;
                    }
                }
                if (e.SDCModel == null)
                {
                    throw new KeyNotFoundException();
                }
                e.SDCModel.MainCanva.Background = Brushes.Transparent;
            }
        }

        #endregion

        #region Cursor

        private const int CURSOR_FREE = 0x00;
        private const int CURSOR_MOUSEDOWN = 0x01;
        private const int CURSOR_LOCK = 0x02;
        private int cstatus = CURSOR_FREE;
        private double cssx, cssy, csex, csey;
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            cstatus = CURSOR_MOUSEDOWN;

            Point p = e.GetPosition(this);
            cssx = p.X;
            cssy = p.Y;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            
            Point p = e.GetPosition(this);
            csex = p.X;
            csey = p.Y;

            if (csex == cssx && csey == cssy)
            {
                cstatus = CURSOR_FREE;
                Cursor.Visibility = Visibility.Hidden;
            }
            else
            {
                cstatus = CURSOR_LOCK;
                this.Focus();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            switch (cstatus)
            {
                case CURSOR_MOUSEDOWN:
                    Cursor.Visibility = Visibility.Visible;
                    Point p = e.GetPosition(this);
                    csex = p.X;
                    csey = p.Y;
                    double x1 = Math.Min(cssx, csex);
                    double x2 = Math.Max(cssx, csex);
                    double y1 = Math.Min(cssy, csey);
                    double y2 = Math.Max(cssy, csey);
                    y1 = ((int)(y1)) / 40 * 40;
                    y2 = ((int)(y2+40)) / 40 * 40;
                    Canvas.SetTop(Cursor, y1);
                    Canvas.SetLeft(Cursor, x1);
                    Cursor.Width = x2 - x1;
                    Cursor.Height = y2 - y1;
                    break;
                case CURSOR_LOCK:
                    Cursor.Visibility = Visibility.Visible;
                    break;
                case CURSOR_FREE:
                    Cursor.Visibility = Visibility.Hidden;
                    break;
            }
        }

        #region Cursor Write

        private void OnValueChartKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);

            LinkedListNode<SimulateDataChartModel> nodeb;
            LinkedListNode<SimulateDataChartModel> nodee;
            LinkedListNode<SimulateDataChartModel> node;
            List<SimulateDataChartModel> bitsdcs;
            SimulateDataChartModel sdcmodel;
            SimulateDataModel sdmodel;

            switch (cstatus)
            {
                case CURSOR_LOCK:
                    nodeb = CursorNodeBegin();
                    nodee = CursorNodeEnd();
                    if (nodeb == null || nodee == null)
                        break;
                    bitsdcs = new List<SimulateDataChartModel>();
                    for (node = nodeb; ; node = node.Next)
                    {
                        sdcmodel = node.Value;
                        sdmodel = sdcmodel.SDModel;
                        if (sdmodel.Type.Equals("BIT"))
                        {
                            bitsdcs.Add(sdcmodel);
                        }
                        if (node == nodee) break;
                    }
                    if (bitsdcs.Count() > 0)
                    {
                        foreach (SimulateDataChartModel _sdcmodel in bitsdcs)
                        {
                            sdmodel = _sdcmodel.SDModel;
                            switch (e.Key)
                            {
                                case Key.D0:
                                    CursorSetValue(sdmodel, 0);
                                    break;
                                case Key.D1:
                                    CursorSetValue(sdmodel, 1);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        for (node = nodeb; ; node = node.Next)
                        {
                            sdcmodel = node.Value;
                            sdmodel = sdcmodel.SDModel;
                            switch (e.Key)
                            {
                                case Key.Enter:
                                    break;
                            }
                            if (node == nodee) break;
                        }
                    }
                    foreach (SimulateDataChartModel _sdcmodel in SDCModels)
                    {
                        _sdcmodel.Update();
                    }
                    break;
            }
        }

        private LinkedListNode<SimulateDataChartModel> CursorNodeBegin()
        {
            int id = (int)(Math.Min(cssy, csey) / 40);
            if (id >= SDCModels.Count()) return null;
            SimulateDataChartModel sdcmodel = SDCModels.ElementAt(id);
            LinkedListNode<SimulateDataChartModel> node = SDCModels.Find(sdcmodel);
            return node;
        }

        private LinkedListNode<SimulateDataChartModel> CursorNodeEnd()
        {

            int id = (int)(Math.Max(cssy, csey) / 40);
            if (id >= SDCModels.Count()) return null;
            SimulateDataChartModel sdcmodel = SDCModels.ElementAt(id);
            LinkedListNode<SimulateDataChartModel> node = SDCModels.Find(sdcmodel);
            return node;
        }

        private double CursorTimeStart()
        {
            double rts = truler.TimeStart;
            double rte = truler.TimeEnd;
            double cts = Math.Min(cssx, csex);
            return rts + (rte - rts) * cts / ActualWidth;
        }
        
        private double CursorTimeEnd()
        {
            double rts = truler.TimeStart;
            double rte = truler.TimeEnd;
            double cte = Math.Max(cssx, csex);
            return rts + (rte - rts) * cte / ActualWidth;
        }

        private void CursorSetValue(SimulateDataModel sdmodel, object value)
        {
            ValueSegment vseg;
            switch (sdmodel.Type)
            {
                case "BIT": case "WORD": case "DWORD":
                    IntSegment iseg = new IntSegment();
                    iseg.TimeStart = (int)(CursorTimeStart());
                    iseg.TimeEnd = (int)(CursorTimeEnd());
                    iseg.Value = value;
                    vseg = iseg;
                    break;
                case "FLOAT":
                    FloatSegment fseg = new FloatSegment();
                    fseg.TimeStart = (int)(CursorTimeStart());
                    fseg.TimeEnd = (int)(CursorTimeEnd());
                    fseg.Value = value;
                    vseg = fseg;
                    break;
                case "DOUBLE":
                    DoubleSegment dseg = new DoubleSegment();
                    dseg.TimeStart = (int)(CursorTimeStart());
                    dseg.TimeEnd = (int)(CursorTimeEnd());
                    dseg.Value = value;
                    vseg = dseg;
                    break;
                default:
                    throw new ArgumentException();
            }
            List<ValueSegment> delvss = new List<ValueSegment>();
            List<ValueSegment> addvss = new List<ValueSegment>();
            ValueSegment vs1, vs2;
            foreach (ValueSegment vs in sdmodel.Values)
            {
                // [()]
                if (vs.TimeStart <= vseg.TimeStart && vseg.TimeEnd <= vs.TimeEnd)
                {
                    vs1 = vs.Clone();
                    vs2 = vs.Clone();
                    vs1.TimeEnd = vseg.TimeStart;
                    vs2.TimeStart = vseg.TimeEnd;
                    delvss.Add(vs);
                    if (vs1.TimeStart < vs1.TimeEnd)
                        addvss.Add(vs1);
                    if (vs2.TimeStart < vs2.TimeEnd)
                        addvss.Add(vs2);
                }
                else
                // ([])
                if (vseg.TimeStart <= vs.TimeStart && vs.TimeEnd <= vseg.TimeEnd)
                {
                    delvss.Add(vs);
                }
                else
                // ([)]
                if (vseg.TimeStart <= vs.TimeStart && vs.TimeStart <= vseg.TimeEnd && vseg.TimeEnd <= vs.TimeEnd)
                {
                    vs.TimeStart = vseg.TimeEnd;
                    if (vs.TimeStart >= vs.TimeEnd)
                        delvss.Add(vs);
                }
                else
                // [(])
                if (vs.TimeStart <= vseg.TimeStart && vseg.TimeStart <= vs.TimeEnd && vs.TimeEnd <= vseg.TimeEnd)
                {
                    vs.TimeEnd = vseg.TimeStart;
                    if (vs.TimeStart >= vs.TimeEnd)
                        delvss.Add(vs);
                }
                // []() or ()[]
            }
            foreach (ValueSegment vs in delvss)
            {
                sdmodel.Remove(vs);
            }
            foreach (ValueSegment vs in addvss)
            {
                sdmodel.Add(vs);
            }
            sdmodel.Add(vseg);
            sdmodel.SortByTime();
        }

        #endregion

        #endregion

    }
}

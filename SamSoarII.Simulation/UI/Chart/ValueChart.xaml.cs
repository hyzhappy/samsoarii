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
        private const int DesignWidth = 800;
        private const int DesignHeight = 600;

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
        private double _actualHeight;
        public new double ActualHeight
        {
            get { return this._actualHeight; }
            set
            {
                this._actualHeight = value;
                StartLine.Y2 = value;
                EndLine.Y2 = value;
            }
        }

        private TimeRuler truler;
        public TimeRuler TRuler
        {
            get { return this.truler; }
            set { this.truler = value; }
        }

        public ValueChart()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            SDCModels.Clear();
            Update();
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
            truler.StartLineChanged += OnStartLineChanged;
            truler.EndLineChanged += OnEndLineChanged;
        }

        public void Update()
        {
            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
            RowDefinition rdef = null;
            int i = 0;
            foreach (SimulateDataChartModel sdcmodel in SDCModels)
            {
                rdef = new RowDefinition();
                rdef.Height = new GridLength(40);
                MainGrid.RowDefinitions.Add(rdef);
                Grid.SetRow(sdcmodel, i++);
                Grid.SetColumn(sdcmodel, 0);
                MainGrid.Children.Add(sdcmodel);
            }
        }

        public void UpdateChart()
        {
            foreach (SimulateDataChartModel sdcmodel in SDCModels)
            {
                sdcmodel.Update();
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
            sdcmodel.ActualWidth = ActualWidth;
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
            sdcmodel.ActualWidth = ActualWidth;
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
            LinkedListNode<SimulateDataChartModel> node = sdcmodels.Find(sdcmodel);
            if (node == null) return;
            MainGrid.RowDefinitions.RemoveAt(0);
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
        //public event SimulateDataModelEventHandler SDModelClose;
        private void OnSDModelClose(object sender, SimulateDataModelEventArgs e)
        {
            if (sender is SimuViewChartModel)
            {
                if (e.SDCModel == null)
                {
                    foreach (SimulateDataChartModel sdcmodel in SDCModels)
                    {
                        if (sdcmodel.SDModel == e.SDModel_old)
                        {
                            e.SDCModel = sdcmodel;
                            break;
                        }
                    }
                }
                if (e.SDCModel == null)
                {
                    throw new ArgumentException();
                }
                Remove(e.SDCModel);
            }
        }

        //public event SimulateDataModelEventHandler SDModelSetup;
        private void OnSDModelSetup(object sender, SimulateDataModelEventArgs e)
        {
            if (sender is SimuViewChartModel)
            {
                if (e.SDCModel == null)
                {
                    foreach (SimulateDataChartModel sdcmodel in SDCModels)
                    {
                        if (sdcmodel.SDModel == e.SDModel_old)
                        {
                            e.SDCModel = sdcmodel;
                            break;
                        }
                    }
                }
                if (e.SDCModel == null)
                {
                    Add(e.SDModel_new, e.ID);
                }
                else
                {
                    e.SDCModel.Setup(e.SDModel_new);
                }
            }
        }

        private void OnSDModelLock(object sender, SimulateDataModelEventArgs e)
        {
            if (sender is SimuViewChartModel)
            {
                if (e.SDCModel == null)
                {
                    foreach (SimulateDataChartModel sdcmodel in SDCModels)
                    {
                        if (sdcmodel.SDModel == e.SDModel_old)
                        {
                            e.SDCModel = sdcmodel;
                            break;
                        }
                    }
                }
                if (e.SDCModel == null)
                {
                    throw new KeyNotFoundException();
                }
                e.SDCModel.Update();
            }
        }

        private void OnSDModelView(object sender, SimulateDataModelEventArgs e)
        {
            if (sender is SimuViewChartModel)
            {
                if (e.SDCModel == null)
                {
                    foreach (SimulateDataChartModel sdcmodel in SDCModels)
                    {
                        if (sdcmodel.SDModel == e.SDModel_old)
                        {
                            e.SDCModel = sdcmodel;
                            break;
                        }
                    }
                }
                if (e.SDCModel == null)
                {
                    throw new KeyNotFoundException();
                }
                e.SDCModel.Update();
            }
        }

        private void OnSDModelUnlock(object sender, SimulateDataModelEventArgs e)
        {
            if (sender is SimuViewChartModel)
            {
                if (e.SDCModel == null)
                {
                    foreach (SimulateDataChartModel sdcmodel in SDCModels)
                    {
                        if (sdcmodel.SDModel == e.SDModel_old)
                        {
                            e.SDCModel = sdcmodel;
                            break;
                        }
                    }
                }
                if (e.SDCModel == null)
                {
                    throw new KeyNotFoundException();
                }
                e.SDCModel.Update();
            }
        }

        private void OnSDModelUnview(object sender, SimulateDataModelEventArgs e)
        {
            if (sender is SimuViewChartModel)
            {
                if (e.SDCModel == null)
                {
                    foreach (SimulateDataChartModel sdcmodel in SDCModels)
                    {
                        if (sdcmodel.SDModel == e.SDModel_old)
                        {
                            e.SDCModel = sdcmodel;
                            break;
                        }
                    }
                }
                if (e.SDCModel == null)
                {
                    throw new KeyNotFoundException();
                }
                e.SDCModel.Update();
            }
        }

        private void OnRunDataFinished(object sender, SimulateDataModelEventArgs e)
        {
            foreach (SimulateDataChartModel sdcmodel in SDCModels)
            {
                SimulateDataModel sdmodel = sdcmodel.SDModel;
                if (sdmodel.IsView)
                    sdcmodel.Update();
            }
        }

        public event SimulateDataModelEventHandler XYModelCreate;

        private void OnRunDrawFinished(object sender, SimulateDataModelEventArgs e)
        {
            List<SimulateDataModel> views = new List<SimulateDataModel>();
            foreach (SimulateDataChartModel sdcmodel in SDCModels)
            {
                SimulateDataModel sdmodel = sdcmodel.SDModel;
                if (sdmodel.IsView)
                {
                    views.Add(sdmodel);
                }
            }
            e.SDModels = views;
            if (XYModelCreate != null)
            {
                XYModelCreate(this, e);
            }
        }
      
        private void OnStartLineChanged(object sender, RoutedEventArgs e)
        {
            if (sender is Line)
            {
                Line line = (Line)(sender);
                StartLine.X1 = StartLine.X2 = line.X1 * ActualWidth / DesignWidth;
                StartLine.Opacity = line.Opacity;
            }
        }
        
        private void OnEndLineChanged(object sender, RoutedEventArgs e)
        {
            if (sender is Line)
            {
                Line line = (Line)(sender);
                EndLine.X1 = EndLine.X2 = line.X1 * ActualWidth / DesignWidth;
                EndLine.Opacity = line.Opacity;
            }
        }

        #region ContextMenu Event Handler

        public event SimulateDataModelEventHandler SDModelRun;
        private void MI_Run_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.TimeStart = CursorTimeStart();
            _e.TimeEnd = CursorTimeEnd();
            if (SDModelRun != null)
            {
                SDModelRun(this, _e);
            }
        }

        public event SimulateDataModelEventHandler SDModelSelect;
        private void MI_Select_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            if (SDModelSelect != null)
            {
                SDModelSelect(this, _e);
            }
        }

        public event SimulateDataModelEventHandler SDModelCut;
        private void MI_Cut_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.TimeStart = CursorTimeStart();
            _e.TimeEnd = CursorTimeEnd();
            if (SDModelCut != null)
            {
                SDModelCut(this, _e);
            }
            UpdateChart();
        }

        public event SimulateDataModelEventHandler SDModelCopy;
        private void MI_Copy_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.TimeStart = CursorTimeStart();
            _e.TimeEnd = CursorTimeEnd();
            if (SDModelCopy != null)
            {
                SDModelCopy(this, _e);
            }
        }

        public event SimulateDataModelEventHandler SDModelPaste;
        private void MI_Paste_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.TimeStart = CursorTimeStart();
            _e.TimeEnd = CursorTimeEnd();
            _e.ID = (int)(Math.Min(cssy, csey) / 40);
            if (SDModelPaste != null)
            {
                SDModelPaste(this, _e);
            }
            UpdateChart();
        }

        public event SimulateDataModelEventHandler SDModelDelete;
        private void MI_Delete_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.TimeStart = CursorTimeStart();
            _e.TimeEnd = CursorTimeEnd();
            if (SDModelDelete != null)
            {
                SDModelDelete(this, _e);
            }
            UpdateChart();
        }


        public event SimulateDataModelEventHandler SDModelDraw;
        private void MI_Draw_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.TimeStart = CursorTimeStart();
            _e.TimeEnd = CursorTimeEnd();
            if (SDModelDraw != null)
            {
                SDModelDraw(this, _e);
            }
        }

        public event SimulateDataModelEventHandler SDModelSave;
        private void MI_Save_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.TimeStart = CursorTimeStart();
            _e.TimeEnd = CursorTimeEnd();
            if (SDModelSave != null)
            {
                SDModelSave(this, _e);
            }
        }

        public event SimulateDataModelEventHandler SDModelLoad;
        private void MI_Load_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.TimeStart = CursorTimeStart();
            _e.TimeEnd = CursorTimeEnd();
            if (SDModelLoad != null)
            {
                SDModelLoad(this, _e);
            }
        }

        public event SimulateDataModelEventHandler SDModelSaveAll;
        private void MI_SaveAll_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.TimeStart = CursorTimeStart();
            _e.TimeEnd = CursorTimeEnd();
            if (SDModelSaveAll != null)
            {
                SDModelSaveAll(this, _e);
            }
        }

        public event SimulateDataModelEventHandler SDModelLoadAll;
        private void MI_LoadAll_Click(object sender, RoutedEventArgs e)
        {
            SimulateDataModelEventArgs _e = new SimulateDataModelEventArgs();
            _e.TimeStart = CursorTimeStart();
            _e.TimeEnd = CursorTimeEnd();
            if (SDModelLoadAll != null)
            {
                SDModelLoadAll(this, _e);
            }
        }
        #endregion

        #endregion

        #region Cursor

        private const int CURSOR_FREE = 0x00;
        private const int CURSOR_MOUSEDOWN = 0x01;
        private const int CURSOR_LOCK = 0x02;
        private int cstatus = CURSOR_FREE;
        private double cssx, cssy, csex, csey;
        private bool isdoubleclick = false;
        
        #region Cursor Handle
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (isdoubleclick)
            {
                isdoubleclick = false;
                return;
            }

            base.OnMouseDown(e);
            
            cstatus = CURSOR_MOUSEDOWN;

            Point p = e.GetPosition(this);
            cssx = p.X;
            cssy = p.Y;
            if (StartLine.Opacity > 0.0)
            {
                cssx = StartLine.X1;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (cstatus == CURSOR_LOCK)
            {
                this.Focus();
                return;
            }
            
            Point p = e.GetPosition(this);
            csex = p.X;
            csey = p.Y;
            if (EndLine.Opacity > 0.0)
            {
                csex = EndLine.X1;
            }

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

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            isdoubleclick = true;
            cstatus = CURSOR_LOCK;
            Cursor.Visibility = Visibility.Visible;
            Point p = e.GetPosition(this);
            cssx = 0;
            cssy = p.Y;
            csex = ActualWidth;
            csey = p.Y;
            if (StartLine.Opacity > 0.0)
            {
                cssx = StartLine.X1;
            }
            if (EndLine.Opacity > 0.0)
            {
                csex = EndLine.X1;
            }
            double x1 = Math.Min(cssx, csex);
            double x2 = Math.Max(cssx, csex);
            double y1 = Math.Min(cssy, csey);
            double y2 = Math.Max(cssy, csey);
            y1 = Math.Max(((int)(y1)) / 40, 0) * 40;
            y2 = Math.Min(((int)(y2 + 40)) / 40, SDCModels.Count()) * 40;
            if (x2 - x1 > 0 && y2 - y1 > 0)
            {
                Canvas.SetTop(Cursor, y1);
                Canvas.SetLeft(Cursor, x1);
                Cursor.Width = x2 - x1;
                Cursor.Height = y2 - y1;
            }
            this.Focus();
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
                    if (EndLine.Opacity > 0.0)
                    {
                        csex = EndLine.X1;
                    }
                    double x1 = Math.Min(cssx, csex);
                    double x2 = Math.Max(cssx, csex);
                    double y1 = Math.Min(cssy, csey);
                    double y2 = Math.Max(cssy, csey);
                    y1 = Math.Max(((int)(y1)) / 40, 0) * 40;
                    y2 = Math.Min(((int)(y2 + 40)) / 40, SDCModels.Count()) * 40;
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

        #endregion

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
                                    sdmodel.SortByTime();
                                    break;
                                case Key.D1:
                                    CursorSetValue(sdmodel, 1);
                                    sdmodel.SortByTime();
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
            if (id >= SDCModels.Count()) id = SDCModels.Count() - 1;
            SimulateDataChartModel sdcmodel = SDCModels.ElementAt(id);
            LinkedListNode<SimulateDataChartModel> node = SDCModels.Find(sdcmodel);
            return node;
        }

        private LinkedListNode<SimulateDataChartModel> CursorNodeEnd()
        {

            int id = (int)(Math.Max(cssy, csey) / 40);
            if (id >= SDCModels.Count()) id = SDCModels.Count() - 1;
            SimulateDataChartModel sdcmodel = SDCModels.ElementAt(id);
            LinkedListNode<SimulateDataChartModel> node = SDCModels.Find(sdcmodel);
            return node;
        }

        private List<SimulateDataChartModel> extraSelectSDModels = new List<SimulateDataChartModel>();
        public IEnumerable<SimulateDataChartModel> CursorCollection()
        {
            List<SimulateDataChartModel> col = new List<SimulateDataChartModel>();
            LinkedListNode<SimulateDataChartModel> nodeb = CursorNodeBegin();
            LinkedListNode<SimulateDataChartModel> nodee = CursorNodeEnd();
            LinkedListNode<SimulateDataChartModel> node = nodeb;
            for (;; node = node.Next)
            {
                col.Add(node.Value);
                if (node == nodee) break;
            }
            return col.Union(extraSelectSDModels);
        }

        public double CursorTimeStart()
        {
            double rts = truler.TimeStart;
            double rte = truler.TimeEnd;
            double cts = Math.Min(cssx, csex);
            return rts + (rte - rts) * cts / ActualWidth;
        }
        
        public double CursorTimeEnd()
        {
            double rts = truler.TimeStart;
            double rte = truler.TimeEnd;
            double cte = Math.Max(cssx, csex);
            return rts + (rte - rts) * cte / ActualWidth;
        }

        public void CursorSetValue(SimulateDataModel sdmodel, object value)
        {
            int timestart = (int)(CursorTimeStart());
            int timeend = (int)(CursorTimeEnd());
            SetValue(sdmodel, value, timestart, timeend);
        }

        public void CursorRemoveValue(SimulateDataModel sdmodel)
        {
            int timestart = (int)(CursorTimeStart());
            int timeend = (int)(CursorTimeEnd());
            RemoveValue(sdmodel, timestart, timeend);
        }

        public void SetValue(SimulateDataModel sdmodel, object value, int timestart, int timeend)
        {
            RemoveValue(sdmodel, timestart, timeend);

            ValueSegment vseg;
            switch (sdmodel.Type)
            {
                case "BIT":
                    BitSegment bseg = new BitSegment();
                    bseg.Value = value;
                    vseg = bseg;
                    break;
                case "WORD":
                    WordSegment wseg = new WordSegment();
                    wseg.Value = value;
                    vseg = wseg;
                    break;
                case "DWORD":
                    DWordSegment dseg = new DWordSegment();
                    dseg.Value = value;
                    vseg = dseg;
                    break;
                case "FLOAT":
                    FloatSegment fseg = new FloatSegment();
                    fseg.Value = value;
                    vseg = fseg;
                    break;
                default:
                    throw new ArgumentException();
            }
            vseg.TimeStart = timestart;
            vseg.TimeEnd = timeend;
            sdmodel.Add(vseg);
        }

        public void SetValue(SimulateDataModel sour, SimulateDataModel dest, int sourstart, int sourend, int deststart, int destend)
        {
            sour.SortByTime();
            destend = deststart + (sourend - sourstart);
            IEnumerator<ValueSegment> souriter = sour.Values.GetEnumerator();
            souriter.MoveNext();
            ValueSegment sourvseg = souriter.Current;
            while (sourvseg != null && sourvseg.TimeEnd < sourstart)
            {
                if (!souriter.MoveNext()) break;
                sourvseg = souriter.Current;
            }
            RemoveValue(dest, deststart, destend);
            sourvseg = souriter.Current;
            while (sourvseg != null && sourvseg.TimeStart < sourend)
            {
                int vsegstart = sourvseg.TimeStart;
                int vsegend = sourvseg.TimeEnd;
                if (vsegstart < sourstart)
                    vsegstart = sourstart;
                if (vsegend > sourend)
                    vsegend = sourend;
                if (vsegstart > vsegend)
                    continue;
                ValueSegment vseg = sourvseg.Clone();
                vseg.TimeStart = vsegstart + (deststart - sourstart);
                vseg.TimeEnd = vsegend + (deststart - sourstart);
                dest.Add(vseg);

                if (!souriter.MoveNext()) break;
                sourvseg = souriter.Current;
            }
        }

        public void RemoveValue(SimulateDataModel sdmodel, int timestart, int timeend)
        {
            List<ValueSegment> delvss = new List<ValueSegment>();
            List<ValueSegment> addvss = new List<ValueSegment>();
            ValueSegment vs1, vs2;
            foreach (ValueSegment vs in sdmodel.Values)
            {
                // [()]
                if (vs.TimeStart <= timestart && timeend <= vs.TimeEnd)
                {
                    vs1 = vs.Clone();
                    vs2 = vs.Clone();
                    vs1.TimeEnd = timestart;
                    vs2.TimeStart = timeend;
                    delvss.Add(vs);
                    if (vs1.TimeStart < vs1.TimeEnd)
                        addvss.Add(vs1);
                    if (vs2.TimeStart < vs2.TimeEnd)
                        addvss.Add(vs2);
                }
                else
                // ([])
                if (timestart <= vs.TimeStart && vs.TimeEnd <= timeend)
                {
                    delvss.Add(vs);
                }
                else
                // ([)]
                if (timestart <= vs.TimeStart && vs.TimeStart <= timeend && timeend <= vs.TimeEnd)
                {
                    vs.TimeStart = timeend;
                    if (vs.TimeStart >= vs.TimeEnd)
                        delvss.Add(vs);
                }
                else
                // [(])
                if (vs.TimeStart <= timestart && timestart <= vs.TimeEnd && vs.TimeEnd <= timeend)
                {
                    vs.TimeEnd = timestart;
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
            //sdmodel.SortByTime();
        }

        #endregion

        #endregion
    }
}

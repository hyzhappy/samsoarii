using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SamSoarII.Core.Models;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Controls;
using SamSoarII.Global;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using SamSoarII.Threads;
using System.Runtime.InteropServices;
using SamSoarII.Utility;
using System.Windows.Threading;
using System.Threading;

namespace SamSoarII.Shell.Models
{
    public enum SelectStatus
    {
        Idle,
        SingleSelecting,
        SingleSelected,
        MultiSelecting,
        MultiSelected,
        NetworkDraging,
    }

    public enum CrossNetworkState
    {
        CrossUp,
        CrossDown,
        NoCross
    }
    /// <summary>
    /// LadderDiagramViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class LadderDiagramViewModel : UserControl, IViewModel, IProgram
    {
        public LadderDiagramViewModel(LadderDiagramModel _core)
        {
            //if (GlobalSetting.InstrutionNameAndToolTips == null)
            //    GlobalSetting.LoadInstrutionNameAndToolTips();
            InitializeComponent();
            DataContext = this;
            Core = _core;
            _selectRect = new SelectRect();
            _selectRect.IsCommentMode = core.IsCommentMode;
            MainCanvas.Children.Add(_selectRect);
            _selectArea = new SelectArea(Core);
            _selectArea.IsCommentMode = core.IsCommentMode;
            MainCanvas.Children.Add(_selectArea);
            outline = new NetworkOutlineViewModel();
            cmEdit = new LadderEditMenu();
            cmMoni = new LadderMonitorMenu();
            ladderExpander.MouseEnter += OnExpanderMouseEnter;
            ladderExpander.MouseLeave += OnExpanderMouseLeave;
            ladderExpander.line.Visibility = Visibility.Hidden;
            ladderExpander.line1.Visibility = Visibility.Hidden;
            ladderExpander.expandButton.IsExpandChanged += OnExpandChanged;
            ladderExpander.IsExpand = IsExpand;
            TitleStackPanel.Children.Remove(ThumbnailButton);
            Update();
        }
        
        public void Dispose()
        {
            _selectRect.Dispose();
            outline.Dispose();
            cmEdit.Dispose();
            cmMoni.Dispose();
            Core = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Core

        private LadderDiagramModel core;
        public LadderDiagramModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                LadderDiagramModel _core = core;
                this.core = null;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    _core.ViewPropertyChanged -= OnCorePropertyChanged;
                    _core.ChildrenChanged -= OnCoreChildrenChanged;
                    if (_core.View != null) _core.View = null;
                }
                this.core = value;
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    core.ViewPropertyChanged += OnCorePropertyChanged;
                    core.ChildrenChanged += OnCoreChildrenChanged;
                    if (core.View != this) core.View = this;
                    Update();
                }
            }
        }
        public InteractionFacade IFParent { get { return core?.Parent?.Parent; } }
        
        IModel IViewModel.Core
        {
            get { return core; }
            set { Core = (LadderDiagramModel)value; }
        }

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    PropertyChanged(this, new PropertyChangedEventArgs("ProgramName"));
                    //PropertyChanged(this, new PropertyChangedEventArgs("TabHeader"));
                    break;
                case "Brief":
                    PropertyChanged(this, new PropertyChangedEventArgs("LadderComment"));
                    break;
                case "IsExpand":
                    Update();
                    ladderExpander.IsExpand = IsExpand;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExpand"));
                    break;
                case "IsCommentMode":
                    _selectRect.IsCommentMode = core.IsCommentMode;
                    _selectArea.IsCommentMode = core.IsCommentMode;
                    break;
                case "CanvasHeight":
                    MainCanvas.Height = core.CanvasHeight;
                    foreach (UIElement uiele in MainCanvas.Children)
                    {
                        if (uiele is LadderUnitViewModel)
                        {
                            LadderUnitViewModel unit = (LadderUnitViewModel)uiele;
                            if (unit.Core != null)
                                unit.Update(LadderUnitViewModel.UPDATE_TOP);
                        }
                    }
                    break;
            }
        }
        
        private void OnCoreChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (!IsFullLoaded) return;
            LadderNetworkModel net = null;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    net = (LadderNetworkModel)(e.NewItems[0]);
                    if (net.View == null)
                        net.View = new LadderNetworkViewModel(net);
                    MainCanvas.Children.Insert(e.NewStartingIndex, net.View);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    net = (LadderNetworkModel)(e.OldItems[0]);
                    if (SelectRectOwner == net) SelectRectOwner = null;
                    MainCanvas.Children.Remove(net.View);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    net = (LadderNetworkModel)(e.NewItems[0]);
                    MainCanvas.Children[e.NewStartingIndex] = net.View;
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    Update();
                    break;
            }
        }
        
        public void QuickInsertElement(LadderUnitModel.Types type)
        {
            switch (SelectionStatus)
            {
                case SelectStatus.Idle: break;
                case SelectStatus.SingleSelected:
                    isnavigatable = false;
                    if (type == LadderUnitModel.Types.VLINE)
                    {
                        LadderUnitModel vline = SelectRectOwner.VLines[_selectRect.X - 1, _selectRect.Y];
                        if (_selectRect.X > 0)
                        {
                            if (vline == null)
                                Core.QuickInsertElement(type, SelectRectOwner, _selectRect.X - 1, _selectRect.Y);
                            SelectRectDown();
                        }
                    }
                    else if (LadderUnitModel.Formats[(int)type].Formats.Count == 0)
                    {
                        Core.QuickInsertElement(type, _selectRect.Core);
                        SelectRectRight();
                    }
                    else
                    {
                        //core.QuickInsertElement(type, _selectRect.Core);
                        if (IFParent.ShowElementPropertyDialog(type, _selectRect.Core))
                            SelectRectRight();
                    }
                    isnavigatable = true;
                    break;
                case SelectStatus.MultiSelecting: break;
                case SelectStatus.MultiSelected:
                    LadderNetworkModel net = SelectStartNetwork;    
                    int x = _selectArea.Core.XOrigin;
                    int y = _selectArea.Core.YOrigin;
                    Core.QuickInsertElement(type, net, x, y);
                    break;
            }
        }

        public void QuickRemoveElement(LadderUnitModel.Types type)
        {
            switch (SelectionStatus)
            {
                case SelectStatus.Idle: break;
                case SelectStatus.SingleSelected:
                    isnavigatable = false;
                    switch (type)
                    {
                        case LadderUnitModel.Types.HLINE:
                            LadderUnitModel hline = SelectRectOwner.Children[_selectRect.X, _selectRect.Y];
                            if (hline != null && hline.Type == LadderUnitModel.Types.HLINE)
                                Core.RemoveU(SelectRectOwner, new LadderUnitModel[] { hline });
                            SelectRectRight();
                            break;
                        case LadderUnitModel.Types.VLINE:
                            LadderUnitModel vline = SelectRectOwner.VLines[_selectRect.X - 1, _selectRect.Y];
                            if (vline != null)
                            {
                                Core.RemoveU(SelectRectOwner, new LadderUnitModel[] { vline });
                                SelectRectDown();
                            }
                            break;
                    }
                    isnavigatable = true;
                    break;
                case SelectStatus.MultiSelecting: break;
                case SelectStatus.MultiSelected:
                    switch (type)
                    {
                        case LadderUnitModel.Types.HLINE:
                            IEnumerable<LadderUnitModel> hlines = _selectArea.Core.SelectUnits.Where(u => u.Shape == LadderUnitModel.Shapes.HLine);
                            Core.RemoveU(SelectStartNetwork, hlines);
                            break;
                        case LadderUnitModel.Types.VLINE:
                            IEnumerable<LadderUnitModel> vlines = _selectArea.Core.SelectUnits.Where(u => u.Shape == LadderUnitModel.Shapes.VLine);
                            Core.RemoveU(SelectStartNetwork, vlines);
                            break;
                    }
                    break;
            }
        }

        public void InsertRow()
        {
            switch (SelectionStatus)
            {
                case SelectStatus.SingleSelected:
                    Core.AddR(SelectRectOwner, _selectRect.Y + 1);
                    break;
                case SelectStatus.MultiSelected: 
                    if (SelectStartNetwork != null && _selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                        Core.AddR(SelectStartNetwork, _selectArea.Core.YEnd + 1);
                    break;
            }
        }

        public void RemoveRow()
        {
            switch (SelectionStatus)
            {
                case SelectStatus.SingleSelected:
                    Core.RemoveR(SelectRectOwner, _selectRect.Y);
                    break;
                case SelectStatus.MultiSelected:
                    if (SelectStartNetwork != null && _selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                        Core.RemoveR(SelectStartNetwork, _selectArea.Core.YStart, _selectArea.Core.YEnd);
                    break;
            }
        }

        #endregion

        #region Shell

        public ProjectViewModel ViewParent { get { return core?.Parent.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }
        
        public ScrollViewer Scroll { get { return this.MainScrollViewer; } }

        private NetworkOutlineViewModel outline;
        public NetworkOutlineViewModel Outline { get { return this.outline; } }

        #region Binding

        public string TabHeader
        {
            get { return ProgramName; }
            set { ProgramName = value; }
        }

        public string ProgramName
        {
            get { return core != null ? core.Name : ""; }
            set { if (core != null) core.Name = value; }
        }
        
        public string LadderComment
        {
            get { return core != null ? core.Brief : ""; }
            set { if (core != null) core.Brief = value; }
        }
        
        #endregion
        
        #region Floating

        public bool IsFloat { get; set; }
        public LayoutFloatingWindow FloatWindow { get; set; }
        private LayoutFloatingWindowControl floatcontrol;
        public LayoutFloatingWindowControl FloatControl
        {
            get { return this.floatcontrol; }
            set
            {
                this.floatcontrol = value;
                floatcontrol.Closed += OnFloatClosed;
            }
        }
        public event RoutedEventHandler FloatClosed = delegate { };
        private void OnFloatClosed(object sender, EventArgs e)
        {
            FloatClosed(this, new RoutedEventArgs());
        }
        #endregion

        #region Select

        private SelectRect _selectRect;
        public SelectRect SelectionRect
        {
            get { return _selectRect; }
        }
        public LadderNetworkModel SelectRectOwner
        {
            get { return _selectRect != null ? _selectRect.Core.Parent : null; }
            private set { if (_selectRect != null) _selectRect.Core.Parent = value; }
        }

        private SelectArea _selectArea;
        public SelectArea SelectionArea
        {
            get { return _selectArea; }
        }
        public LadderNetworkModel SelectStartNetwork
        {
            get { return _selectArea.Core.State != SelectAreaCore.Status.NotSelected ? core.Children[_selectArea.Core.NetOrigin] : null; }
        }
        public IEnumerable<LadderNetworkViewModel> SelectAllNetworks
        {
            get { return _selectArea.Core.SelectNetworks.Select(n => n.View); }
        }
    
        private SelectStatus _selectStatus = SelectStatus.Idle;
        public SelectStatus SelectionStatus
        {
            get { return this._selectStatus; }
            set { this._selectStatus = value; }
        }
        
        private CrossNetworkState _crossNetState;
        public CrossNetworkState CrossNetState
        {
            get { return this._crossNetState; }
            set { this._crossNetState = value; SelectionChanged(this, new RoutedEventArgs()); }
        }

        #region Selection area change

        public void ReleaseSelect()
        {
            _selectArea.Core.Release();
            _selectRect.Core.Parent = null;
            _selectStatus = SelectStatus.Idle;
        }

        private void SelectionAreaChanged(Key key)
        {
            if (_selectStatus == SelectStatus.SingleSelected)
            {
                if (SingleSelectionAreaCanChange(key))
                {
                    ChangeSingleSelectionArea(key);
                }
            }
            else if (_selectStatus == SelectStatus.MultiSelected)
            {
                if (MutiSelectionAreaCanChange(key))
                {
                    ChangeMutiSelectionArea(key);
                }
            }
        }
        
        private bool MutiSelectionAreaCanChange(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    return _selectArea.Core.CanMoveLeft();
                case Key.Right:
                    return _selectArea.Core.CanMoveRight();
                case Key.Up:
                    return _selectArea.Core.CanMoveUp();
                case Key.Down:
                    return _selectArea.Core.CanMoveDown();
                default:
                    return false;
            }
        }

        private bool SingleSelectionAreaCanChange(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    return SelectionRect.X > 0;
                case Key.Right:
                    return SelectionRect.X < GlobalSetting.LadderXCapacity - 1;
                case Key.Up:
                    return SelectRectOwner.ID > 0 || SelectionRect.Y > 0;
                case Key.Down:
                    return SelectRectOwner.ID < Core.NetworkCount - 1 || SelectionRect.Y < SelectRectOwner.RowCount - 1;
                default:
                    return false;
            }
        }
        private void ChangeMutiSelectionArea(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    ChangeViewport(BoundaryDirection.Left);
                    _selectArea.Core.MoveLeft();
                    break;
                case Key.Right:
                    ChangeViewport(BoundaryDirection.Right);
                    _selectArea.Core.MoveRight();
                    break;
                case Key.Up:
                    ChangeViewport(BoundaryDirection.Up);
                    _selectArea.Core.MoveUp();
                    break;
                case Key.Down:
                    ChangeViewport(BoundaryDirection.Bottom);
                    _selectArea.Core.MoveDown();
                    break;
                default:
                    break;
            }
        }
        private void ChangeSingleSelectionArea(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    ChangeViewport(BoundaryDirection.Left);
                    _selectArea.Core.Select(SelectRectOwner.ID,
                        _selectRect.X, _selectRect.Y, _selectRect.X - 1, _selectRect.Y);
                    break;
                case Key.Right:
                    ChangeViewport(BoundaryDirection.Right);
                    _selectArea.Core.Select(SelectRectOwner.ID,
                        _selectRect.X, _selectRect.Y, _selectRect.X + 1, _selectRect.Y);
                    break;
                case Key.Up:
                    ChangeViewport(BoundaryDirection.Up);
                    if (_selectRect.Y == 0)
                        _selectArea.Core.Select(SelectRectOwner.ID, SelectRectOwner.ID - 1);
                    else
                        _selectArea.Core.Select(SelectRectOwner.ID,
                            _selectRect.X, _selectRect.Y, _selectRect.X, _selectRect.Y - 1);
                    break;
                case Key.Down:
                    ChangeViewport(BoundaryDirection.Bottom);
                    if (_selectRect.Y == SelectRectOwner.RowCount - 1)
                        _selectArea.Core.Select(SelectRectOwner.ID, SelectRectOwner.ID + 1);
                    else
                        _selectArea.Core.Select(SelectRectOwner.ID,
                            _selectRect.X, _selectRect.Y, _selectRect.X, _selectRect.Y + 1);
                    break;
                default:
                    break;
            }
        }
        
        #endregion
        
        #region SelectRect Move & Modify

        private bool isnavigatable = true;
        public bool IsNavigatable
        {
            get { return this.isnavigatable; }
        }

        private void SelectRectUp()
        {
            if (SelectRectOwner != null)
            {
                ChangeViewport(BoundaryDirection.Up);
                if (_selectRect.Y > 0)
                {
                    _selectRect.Y--;
                }
                else
                {
                    int id = SelectRectOwner.ID - 1;
                    LadderNetworkModel lnmodel = null;
                    while (id >= 0)
                    {
                        lnmodel = Core.Children[id];
                        if (lnmodel.View != null && !lnmodel.IsMasked && lnmodel.View.IsExpand)
                        {
                            break;
                        }
                        id--;
                    }
                    if (id >= 0)
                    {
                        SelectRectOwner = lnmodel;
                        _selectRect.Y = lnmodel.RowCount - 1;
                    }
                }
                //VScrollToRect(SelectRectOwner.ID, _selectRect.Y);
            }
        }
        private void SelectRectDown()
        {
            if (SelectRectOwner != null)
            {
                ChangeViewport(BoundaryDirection.Bottom);
                if (_selectRect.Y + 1 < SelectRectOwner.RowCount)
                {
                    _selectRect.Y++;
                }
                else
                {
                    int id = SelectRectOwner.ID + 1;
                    LadderNetworkModel lnmodel = null;
                    while (id < Core.NetworkCount)
                    {
                        lnmodel = Core.Children[id];
                        if (lnmodel.View != null && !lnmodel.IsMasked && lnmodel.View.IsExpand)
                        {
                            break;
                        }
                        id++;
                    }
                    if (id < Core.NetworkCount)
                    {
                        SelectRectOwner = lnmodel;
                        _selectRect.Y = 0;
                    }
                }
                //VScrollToRect(SelectRectOwner.ID, _selectRect.Y);
            }
        }
        private void SelectRectLeft()
        {
            if (SelectRectOwner != null)
            {
                if (_selectRect.X > 0)
                {
                    ChangeViewport(BoundaryDirection.Left);
                    _selectRect.X--;
                    //HScrollToRect(_selectRect.X);
                }
            }
        }
        public void SelectRectRight()
        {
            if (SelectRectOwner != null)
            {
                if (_selectRect.X < GlobalSetting.LadderXCapacity - 1)
                {
                    ChangeViewport(BoundaryDirection.Right);
                    _selectRect.X++;
                    //HScrollToRect(_selectRect.X);
                }
            }
        }
        private void SelectRectLeftWithLine(bool expand = false)
        {
            if (LadderMode != LadderModes.Edit)
            {
                SelectRectLeft();
                return;
            }
            if (SelectRectOwner != null)
            {
                isnavigatable = false;
                SelectRectLeft();
                if (expand)
                {
                    PushLeft(_selectRect.X, _selectRect.Y);
                    isnavigatable = true;
                    return;
                }
                if (_selectRect.Current != null)
                {
                    if (_selectRect.Current.Type == LadderUnitModel.Types.HLINE)
                        Core.RemoveSingleUnit(_selectRect.Current);
                }
                else
                {
                    Core.QuickInsertElement(LadderUnitModel.Types.HLINE, _selectRect.Core);
                }
                isnavigatable = true;
            }
        }
        private void SelectRectRightWithLine(bool expand = false)
        {
            if (LadderMode != LadderModes.Edit)
            {
                SelectRectRight();
                return;
            }
            int x = _selectRect.X;
            int y = _selectRect.Y;
            if (SelectRectOwner != null)
            {
                isnavigatable = false;
                if (expand)
                {
                    SelectRectRight();
                    PushRight(_selectRect.X, _selectRect.Y);
                    isnavigatable = true;
                    return;
                }
                if (_selectRect.Current != null)
                {
                    if (_selectRect.Current.Type == LadderUnitModel.Types.HLINE)
                        Core.RemoveSingleUnit(_selectRect.Current);
                }
                else
                {
                    Core.QuickInsertElement(LadderUnitModel.Types.HLINE, _selectRect.Core);
                }
                SelectRectRight();
                isnavigatable = true;
            }
        }
        private void SelectRectUpWithLine(bool expand = false)
        {
            if (LadderMode != LadderModes.Edit)
            {
                SelectRectUp();
                return;
            }
            int x = _selectRect.X - 1;
            int y = _selectRect.Y - 1;
            if (SelectRectOwner != null)
            {
                isnavigatable = false;
                if (expand)
                {
                    PushUp(_selectRect.X, _selectRect.Y);
                    SelectRectUp();
                    isnavigatable = true;
                    return;
                }
                if (y >= 0)
                {
                    if (x >= 0)
                    {
                        SelectRectUp();
                        var vline = SelectRectOwner.VLines[x, y];
                        if (vline != null)
                            Core.RemoveSingleUnit(vline);
                        else
                            Core.QuickInsertElement(LadderUnitModel.Types.VLINE, SelectRectOwner, x, y);
                    }
                }
                isnavigatable = true;
            }
        }
        private void SelectRectDownWithLine(bool expand = false)
        {
            if (LadderMode != LadderModes.Edit)
            {
                SelectRectDown();
                return;
            }
            int x = _selectRect.X - 1;
            int y = _selectRect.Y;
            if (SelectRectOwner != null)
            {
                isnavigatable = false;
                if (expand)
                {
                    SelectRectDown();
                    PushDown(_selectRect.X, _selectRect.Y);
                    isnavigatable = true;
                    return;
                }
                if (x >= 0)
                {
                    var vline = SelectRectOwner.VLines[x, y];
                    if (vline != null)
                        Core.RemoveSingleUnit(vline);
                    else
                        Core.QuickInsertElement(LadderUnitModel.Types.VLINE, SelectRectOwner, x, y);
                    SelectRectDown();
                }
                isnavigatable = true;
            }
        }
        
        private List<LadderUnitModel> push_removes = new List<LadderUnitModel>();
        private List<LadderUnitModel> push_moves = new List<LadderUnitModel>();
        private List<LadderUnitModel> push_adds = new List<LadderUnitModel>();
        
        public bool PushUp(int x, int y, bool addline = false)
        {
            if (x == 0 || y == 0) return false;
            IEnumerable<LadderUnitModel> units_t = SelectRectOwner.Children.SelectRange(0, 11, y - 1, y - 1);
            if (units_t.Count() > 0) return false;
            push_removes = SelectRectOwner.VLines.SelectRange(0, 11, y - 1, y - 1).ToList();
            push_moves = SelectRectOwner.Children.SelectRange(0, 11, y, SelectRectOwner.RowCount - 1).ToList();
            push_moves.AddRange(SelectRectOwner.VLines.SelectRange(0, 11, y, SelectRectOwner.RowCount - 1));
            push_adds.Clear();
            Core.ReplaceMoveU(SelectRectOwner, push_removes, push_adds, push_moves, 0, -1, -1);
            return true;
        }
        public bool PushDown(int x, int y, bool addline = false)
        {
            push_removes.Clear();
            push_moves.Clear();
            push_adds.Clear();
            bool addrow = false;
            bool success = _PushDown(x, y, ref addrow);
            if (success) Core.ReplaceMoveU(SelectRectOwner, push_removes, push_adds, push_moves, 0, 1, addrow ? 1 : 0);
            return success;
        }
        public bool PushLeft(int x, int y, bool addline = false)
        {
            push_removes.Clear();
            push_moves.Clear();
            push_adds.Clear();
            bool success = _PushLeft(x, y);
            if (success) Core.ReplaceMoveU(SelectRectOwner, push_removes, push_adds, push_moves, -1, 0);
            return success;
        }
        public bool PushRight(int x, int y, bool addline = false)
        {
            push_removes.Clear();
            push_moves.Clear();
            push_adds.Clear();
            bool success = _PushRight(x, y);
            if (success) Core.ReplaceMoveU(SelectRectOwner, push_removes, push_adds, push_moves, 1, 0);
            return success;
        }
        private bool _PushLeft(int x, int y)
        {
            if (x == 0) return false;
            int y1 = y, y2 = y;
            LadderUnitModel unit = SelectRectOwner.Children[x, y];
            LadderUnitModel unit_r = null;
            if (push_moves.Contains(unit)) return true;
            LadderUnitModel vline = null;
            LadderUnitModel vline_u = SelectRectOwner.VLines[x, y - 1];
            LadderUnitModel vline_d = SelectRectOwner.VLines[x, y];
            while (vline_u != null)
                vline_u = SelectRectOwner.VLines[x, --y1 - 1];
            while (vline_d != null)
                vline_d = SelectRectOwner.VLines[x, ++y2];
            for (int _y = y1; _y <= y2; _y++)
            {
                unit = SelectRectOwner.Children[x, _y];
                unit_r = SelectRectOwner.Children[x + 1, _y];
                bool rightlink = (unit_r != null && !push_moves.Contains(unit_r));
                if (unit != null)
                {
                    if (push_moves.Contains(unit)) return true;
                    if (unit.Type == LadderUnitModel.Types.HLINE)
                    {
                        if (!rightlink && !push_removes.Contains(unit)) push_removes.Add(unit);
                    }
                    else
                    {
                        push_moves.Add(unit);
                        if (rightlink && push_adds.Where(u => u.X == x && u.Y == _y).Count() == 0)
                            push_adds.Add(new LadderUnitModel(null, LadderUnitModel.Types.HLINE) { X = x, Y = _y });
                    }
                }
                else if (rightlink && push_adds.Where(u => u.X == x && u.Y == _y).Count() == 0)
                {
                    push_adds.Add(new LadderUnitModel(null, LadderUnitModel.Types.HLINE) { X = x, Y = _y });
                }
                if (_y < y2)
                {
                    vline = SelectRectOwner.VLines[x, _y];
                    if (!push_moves.Contains(vline))
                        push_moves.Add(vline);
                }
            }
            for (int _y = y1; _y <= y2; _y++)
            {
                unit = SelectRectOwner.Children[x, _y];
                if (unit != null && unit.Type != LadderUnitModel.Types.HLINE)
                    if (!_PushLeft(x - 1, _y)) return false;
            }
            return true;
        }
        private bool _PushRight(int x, int y)
        {
            if (x == GlobalSetting.LadderXCapacity - 1) return false;
            int y1 = y, y2 = y;
            LadderUnitModel unit = SelectRectOwner.Children[x, y];
            LadderUnitModel unit_l = null;
            if (unit != null && push_moves.Contains(unit)) return true;
            LadderUnitModel vline = null;
            LadderUnitModel vline_u = SelectRectOwner.VLines[x - 1, y - 1];
            LadderUnitModel vline_d = SelectRectOwner.VLines[x - 1, y];
            while (vline_u != null)
                vline_u = SelectRectOwner.VLines[x - 1, --y1 - 1];
            while (vline_d != null)
                vline_d = SelectRectOwner.VLines[x - 1, ++y2];
            for (int _y = y1; _y <= y2; _y++)
            {
                unit = SelectRectOwner.Children[x, _y];
                unit_l = SelectRectOwner.Children[x - 1, _y];
                bool leftlink = (unit_l != null && !push_moves.Contains(unit_l));
                if (unit != null)
                {
                    if (push_moves.Contains(unit)) return true;
                    if (unit.Type == LadderUnitModel.Types.HLINE)
                    {
                        if (!leftlink && !push_removes.Contains(unit)) push_removes.Add(unit);
                    }
                    else
                    {
                        push_moves.Add(unit);
                        if (leftlink && push_adds.Where(u => u.X == x && u.Y == _y).Count() == 0)
                            push_adds.Add(new LadderUnitModel(null, LadderUnitModel.Types.HLINE) { X = x, Y = _y });
                    }
                }
                else if (leftlink && push_adds.Where(u => u.X == x && u.Y == _y).Count() == 0)
                {
                    push_adds.Add(new LadderUnitModel(null, LadderUnitModel.Types.HLINE) { X = x, Y = _y });
                }
                if (_y < y2)
                {
                    vline = SelectRectOwner.VLines[x - 1, _y];
                    if (!push_moves.Contains(vline))
                        push_moves.Add(vline);
                }
            }
            for (int _y = y1; _y <= y2; _y++)
            {
                unit = SelectRectOwner.Children[x, _y];
                if (unit != null && unit.Type != LadderUnitModel.Types.HLINE)
                    if (!_PushRight(x + 1, _y)) return false;
            }
            return true;
        }
        private bool _PushDown(int x, int y, ref bool addrow)
        {
            LadderUnitModel unit = SelectRectOwner.Children[x, y];
            LadderUnitModel unit_l = SelectRectOwner.Children[x - 1, y];
            LadderUnitModel unit_r = SelectRectOwner.Children[x + 1, y];
            LadderUnitModel vline = null;
            LadderUnitModel vline_u = null;
            LadderUnitModel vline_d = null;
            if ((unit == null || push_moves.Contains(unit))
             && (unit_l == null || push_moves.Contains(unit_l))
             && (unit_r == null || push_moves.Contains(unit_r)))
            {
                return true;
            }
            if (y == SelectRectOwner.RowCount - 1)
            {
                addrow = true;
            }
            int x1 = x, x2 = x;
            while (unit_l != null)
            {
                unit_l = SelectRectOwner.Children[--x1 - 1, y];
            }
            while (unit_r != null)
            {
                unit_r = SelectRectOwner.Children[++x2 + 1, y];
            }
            vline_u = SelectRectOwner.VLines[x1 - 1, y - 1];
            vline_d = SelectRectOwner.VLines[x1 - 1, y];
            if (vline_u != null && !push_moves.Contains(vline_u))
            {
                push_moves.Add(vline_u);
                vline = new LadderUnitModel(null, LadderUnitModel.Types.VLINE) { X = x1 - 1, Y = y - 1 };
                push_adds.Add(vline);
            }
            if (vline_d != null && !push_moves.Contains(vline_d))
            {
                push_moves.Add(vline_d);
            }
            for (int _x = x1; _x <= x2; _x++)
            {
                unit = SelectRectOwner.Children[_x, y];
                if (unit != null && !push_moves.Contains(unit))
                    push_moves.Add(unit);
                vline_u = SelectRectOwner.VLines[_x, y - 1];
                vline_d = SelectRectOwner.VLines[_x, y];
                if (vline_u != null && !push_moves.Contains(vline_u))
                {
                    push_moves.Add(vline_u);
                    vline = new LadderUnitModel(null, LadderUnitModel.Types.VLINE) { X = _x, Y = y - 1 };
                    push_adds.Add(vline);
                }
                if (vline_d != null && !push_moves.Contains(vline_d))
                {
                    push_moves.Add(vline_d);
                }
            }
            for (int _x = x1; _x <= x2; _x++)
            {
                if (!_PushDown(_x, y + 1, ref addrow))
                    return false;
            }
            return true;
        }

        #endregion

        #endregion

        #region Navigate & Select

        private enum BoundaryDirection
        {
            Up,
            Right,
            Left,
            Bottom,
            None
        }
        private double _offset = 0;
        /// <summary>
        /// 当光标跨网络时，计算横跨的距离
        /// </summary>
        /// <param name="networkNum">起始网络号</param>
        /// <param name="_up">是否向上收集</param>
        /// <param name="_add">true 代表直到收集到第一个非屏蔽的网路，false 代表收集连续的屏蔽网络直到碰到非屏蔽的网络终止</param>
        /// <returns></returns>
        private double GetNextNetworkOffset(int networkNum, bool _up, bool _add)
        {
            double offset = 0;
            if (_up)
            {
                if (_add)
                {
                    if (networkNum == 0) return 0;
                    int i = 1;
                    var network = Core.Children[networkNum - i++].View;
                    offset += network.ActualHeight;
                    while (network.IsMasked && i <= networkNum)
                    {
                        network = Core.Children[networkNum - i++].View;
                        offset += network.ActualHeight;
                    }
                    if (network.IsMasked) return 0;
                    else return offset;
                }
                else
                {
                    if (networkNum == 0) return 0;
                    int i = 1;
                    var network = Core.Children[networkNum - i++].View;
                    if (network.IsMasked)
                        offset += network.ActualHeight;
                    while (network.IsMasked && i <= networkNum)
                    {
                        network = Core.Children[networkNum - i++].View;
                        if (network.IsMasked)
                            offset += network.ActualHeight;
                        else break;
                    }
                    if (network.IsMasked && network.Core.ID == Core.NetworkCount - 1) return 0;
                    return offset;
                }
            }
            else
            {
                if (_add)
                {
                    if (Core.NetworkCount == networkNum + 1) return 0;
                    int i = 1;
                    var network = Core.Children[networkNum + i++].View;
                    offset += network.ActualHeight;
                    while (network.IsMasked && i + networkNum < Core.NetworkCount)
                    {
                        network = Core.Children[networkNum + i++].View;
                        offset += network.ActualHeight;
                    }
                    if (network.IsMasked) return 0;
                    else return offset;
                }
                else
                {
                    if (Core.NetworkCount == networkNum + 1) return 0;
                    int i = 1;
                    var network = Core.Children[networkNum + i++].View;
                    if (network.IsMasked)
                        offset += network.ActualHeight;
                    while (network.IsMasked && i + networkNum < Core.NetworkCount)
                    {
                        network = Core.Children[networkNum + i++].View;
                        if (network.IsMasked)
                            offset += network.ActualHeight;
                        else break;
                    }
                    if (network.IsMasked && network.Core.ID == 0) return 0;
                    return offset;
                }
            }
        }
        /// <summary>
        /// 计算光标移动时距离可视界面的边距
        /// </summary>
        /// <param name="direction">代表光标移动的方向</param>
        /// <param name="_isSingleSelected">是否是单选</param>
        /// <param name="_isCrossed">网络的横跨状态</param>
        /// <returns></returns>
        private double ComputeOffset(BoundaryDirection direction, bool _isSingleSelected, bool _isCrossed = false)
        {
            double scaleX = GlobalSetting.LadderScaleTransform.ScaleX;
            double scaleY = GlobalSetting.LadderScaleTransform.ScaleY;
            Point point;
            switch (direction)
            {
                case BoundaryDirection.Up:
                    if (_isSingleSelected)
                    {
                        point = _selectRect.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (_selectRect.Y == 0)
                        {
                            var value = GetNextNetworkOffset(SelectRectOwner.ID, true, false);
                            if (point.Y < (100 + _selectRect.ActualHeight + value) * scaleY)
                            {
                                return point.Y - (100 + _selectRect.ActualHeight + value) * scaleY;
                            }
                            if (point.Y < 100 * scaleY)
                            {
                                return point.Y - 100 * scaleY;
                            }
                            if (point.Y + _selectRect.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                            {
                                return point.Y + _selectRect.ActualHeight * scaleY - MainScrollViewer.ViewportHeight;
                            }
                        }
                        if (_selectRect.Y != 0)
                        {
                            if (point.Y < _selectRect.ActualHeight * scaleY)
                            {
                                return point.Y - _selectRect.ActualHeight * scaleY;
                            }
                            if (point.Y + _selectRect.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                            {
                                return point.Y + _selectRect.ActualHeight * scaleY - MainScrollViewer.ViewportHeight;
                            }
                        }
                    }
                    else if (!_isCrossed)
                    {
                        point = _selectArea.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (_selectArea.Core.YStart == 0 && _selectArea.Core.YOrigin == _selectArea.Core.YEnd)
                        {
                            if (point.Y < 100 * scaleY || SelectStartNetwork.View.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                            {
                                return point.Y - 100 * scaleY;
                            }
                            if (point.Y + SelectStartNetwork.View.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                            {
                                return point.Y + SelectStartNetwork.View.ActualHeight * scaleY - MainScrollViewer.ViewportHeight;
                            }
                        }
                        else
                        {
                            if (_selectArea.Core.YOrigin == _selectArea.Core.YEnd)
                            {
                                if (point.Y < (_selectRect.ActualHeight + 30) * scaleY || (_selectArea.ActualHeight + _selectRect.ActualHeight) * scaleY > MainScrollViewer.ViewportHeight)
                                {
                                    return point.Y - (_selectRect.ActualHeight + 30) * scaleY;
                                }
                                if (point.Y + 30 * scaleY > MainScrollViewer.ViewportHeight)
                                {
                                    return point.Y + 30 * scaleY - MainScrollViewer.ViewportHeight;
                                }
                            }
                            if (_selectArea.Core.YOrigin == _selectArea.Core.YStart)
                            {
                                if (point.Y + (_selectArea.ActualHeight - _selectRect.ActualHeight) * scaleY > MainScrollViewer.ViewportHeight)
                                {
                                    return point.Y + (_selectArea.ActualHeight - _selectRect.ActualHeight) * scaleY - MainScrollViewer.ViewportHeight;
                                }
                                if (point.Y + (_selectArea.ActualHeight - 2 * _selectRect.ActualHeight) * scaleY < 0)
                                {
                                    return point.Y + (_selectArea.ActualHeight - 2 * _selectRect.ActualHeight) * scaleY;
                                }
                            }
                        }
                    }
                    else
                    {
                        switch (CrossNetState)
                        {
                            case CrossNetworkState.CrossUp:
                                double value;
                                if (SelectAllNetworks.Count() == 0)
                                {
                                    point = SelectStartNetwork.View.TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    value = GetNextNetworkOffset(SelectStartNetwork.ID, true, true);
                                }
                                else
                                {
                                    point = SelectAllNetworks.First().TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    value = GetNextNetworkOffset(SelectAllNetworks.First().Core.ID, true, true);
                                }
                                if (point.Y < value * scaleY)
                                    return point.Y - value * scaleY;
                                break;
                            case CrossNetworkState.CrossDown:
                                if (SelectAllNetworks.Count() == 0)
                                {
                                    point = SelectStartNetwork.View.TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    return point.Y + SelectStartNetwork.View.ActualHeight * scaleY - MainScrollViewer.ViewportHeight;
                                }
                                else
                                {
                                    point = SelectAllNetworks.Last().TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    value = GetNextNetworkOffset(SelectAllNetworks.Last().Core.ID, false, false);
                                }
                                return point.Y - value * scaleY - MainScrollViewer.ViewportHeight;
                        }
                    }
                    break;
                case BoundaryDirection.Right:
                    if (_isSingleSelected)
                    {
                        point = _selectRect.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (point.X < 0) return point.X;
                        if (point.X + 2 * _selectRect.ActualWidth * scaleX > MainScrollViewer.ViewportWidth)
                        {
                            return point.X + 2 * _selectRect.ActualWidth * scaleX - MainScrollViewer.ViewportWidth;
                        }
                    }
                    else if (!_isCrossed)
                    {
                        point = _selectArea.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (point.X < 0) return point.X;
                        if (point.X + (_selectRect.ActualWidth + _selectArea.ActualWidth) * scaleX > MainScrollViewer.ViewportWidth)
                        {
                            return point.X + (_selectRect.ActualWidth + _selectArea.ActualWidth) * scaleX - MainScrollViewer.ViewportWidth;
                        }
                    }
                    break;
                case BoundaryDirection.Left:
                    if (_isSingleSelected)
                    {
                        point = _selectRect.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (point.X + _selectRect.ActualWidth * scaleX > MainScrollViewer.ViewportWidth)
                        {
                            return point.X + _selectRect.ActualWidth * scaleX - MainScrollViewer.ViewportWidth;
                        }
                        if (point.X - _selectRect.ActualWidth * scaleX < 0)
                        {
                            return point.X - _selectRect.ActualWidth * scaleX;
                        }
                    }
                    else if (!_isCrossed)
                    {
                        point = _selectArea.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (point.X + _selectArea.ActualWidth * scaleX > MainScrollViewer.ViewportWidth)
                        {
                            return point.X + _selectArea.ActualWidth * scaleX - MainScrollViewer.ViewportWidth;
                        }
                        if (point.X - _selectRect.ActualWidth * scaleX < 0)
                        {
                            return point.X - _selectRect.ActualWidth * scaleX;
                        }
                    }
                    break;
                case BoundaryDirection.Bottom:
                    if (_isSingleSelected)
                    {
                        point = _selectRect.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (_selectRect.Y == SelectRectOwner.RowCount - 1)
                        {
                            var value = GetNextNetworkOffset(SelectRectOwner.ID, false, false);
                            if (point.Y + (2 * _selectRect.ActualHeight + 100 + value) * scaleY > MainScrollViewer.ViewportHeight)
                            {
                                return point.Y + (2 * _selectRect.ActualHeight + 100 + value) * scaleY - MainScrollViewer.ViewportHeight;
                            }
                            if (point.Y + _selectRect.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                            {
                                return point.Y + _selectRect.ActualHeight * scaleY - MainScrollViewer.ViewportHeight;
                            }
                            if (point.Y < 0) return point.Y;
                        }
                        if (_selectRect.Y != SelectRectOwner.RowCount - 1)
                        {
                            if (point.Y + 2 * _selectRect.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                            {
                                return point.Y + 2 * _selectRect.ActualHeight * scaleY - MainScrollViewer.ViewportHeight;
                            }
                            if (point.Y < 0) return point.Y;
                        }
                    }
                    else if (!_isCrossed)
                    {
                        point = _selectArea.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (_selectArea.Core.YEnd == SelectStartNetwork.RowCount - 1)
                        {
                            if (point.Y + _selectArea.ActualHeight * scaleY > MainScrollViewer.ViewportHeight || SelectStartNetwork.View.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                            {
                                return point.Y + _selectArea.ActualHeight * scaleY - MainScrollViewer.ViewportHeight;
                            }
                            if (point.Y + _selectArea.ActualHeight * scaleY < SelectStartNetwork.RowCount * _selectRect.ActualHeight * scaleY + 100 * scaleY)
                            {
                                return point.Y + _selectArea.ActualHeight * scaleY - SelectStartNetwork.RowCount * _selectRect.ActualHeight * scaleY - 100 * scaleY;
                            }
                        }
                        else
                        {
                            if (_selectArea.Core.YOrigin == _selectArea.Core.YEnd)
                            {
                                if (point.Y + _selectRect.ActualHeight * scaleY < 0)
                                {
                                    return point.Y + _selectRect.ActualHeight * scaleY;
                                }
                                if (point.Y + 2 * _selectRect.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                                {
                                    return point.Y + 2 * _selectRect.ActualHeight * scaleY - MainScrollViewer.ViewportHeight;
                                }
                            }
                            if (_selectArea.Core.YOrigin == _selectArea.Core.YStart)
                            {
                                if (point.Y + (_selectArea.ActualHeight + _selectRect.ActualHeight) * scaleY > MainScrollViewer.ViewportHeight)
                                {
                                    return point.Y + (_selectArea.ActualHeight + _selectRect.ActualHeight) * scaleY - MainScrollViewer.ViewportHeight;
                                }
                                if (point.Y + _selectArea.ActualHeight * scaleY < 0)
                                {
                                    return point.Y + _selectArea.ActualHeight * scaleY;
                                }
                            }
                        }
                    }
                    else
                    {
                        switch (CrossNetState)
                        {
                            case CrossNetworkState.CrossUp:
                                double value;
                                if (SelectAllNetworks.Count() == 0)
                                {
                                    point = SelectStartNetwork.View.TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    return point.Y;
                                }
                                else
                                {
                                    point = SelectAllNetworks.First().TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    value = GetNextNetworkOffset(SelectAllNetworks.First().Core.ID, true, false);
                                }
                                return point.Y + (value + SelectAllNetworks.First().ActualHeight) * scaleY;
                            case CrossNetworkState.CrossDown:
                                LadderNetworkViewModel net;
                                if (SelectAllNetworks.Count() == 0)
                                {
                                    point = SelectStartNetwork.View.TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    value = GetNextNetworkOffset(SelectStartNetwork.ID, false, true);
                                    net = SelectStartNetwork.View;
                                }
                                else
                                {
                                    net = SelectAllNetworks.Last();
                                    point = net.TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    value = GetNextNetworkOffset(net.Core.ID, false, true);
                                }
                                return point.Y + (value + net.ActualHeight) * scaleY - MainScrollViewer.ViewportHeight;
                        }
                    }
                    break;
            }
            return 0;
        }
        /// <summary>
        /// 表示在方向direction上，是否需要移动光标
        /// </summary>
        /// <param name="direction">移动的方向</param>
        /// <returns></returns>
        private bool AssertSelectionArea(BoundaryDirection direction)
        {
            var tempoffset = 0.0;
            switch (CrossNetState)
            {
                case CrossNetworkState.CrossUp:
                case CrossNetworkState.CrossDown:
                    tempoffset = ComputeOffset(direction, false, true);
                    break;
                case CrossNetworkState.NoCross:
                    tempoffset = ComputeOffset(direction, SelectRectOwner != null);
                    break;
            }
            if (tempoffset != 0)
            {
                _offset = tempoffset;
                return false;
            }
            else
            {
                _offset = 0;
                return true;
            }
        }
        /// <summary>
        /// 在对应方向上改变可视界面
        /// </summary>
        /// <param name="direction">需要改变的方向</param>
        private void ChangeViewport(BoundaryDirection direction)
        {
            if (!AssertSelectionArea(direction))
            {
                switch (direction)
                {
                    case BoundaryDirection.Up:
                    case BoundaryDirection.Bottom:
                        MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + _offset);
                        break;
                    case BoundaryDirection.Left:
                    case BoundaryDirection.Right:
                        MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + _offset);
                        break;
                    case BoundaryDirection.None:
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 保证指令输入框保持在视野内
        /// </summary>
        public void NavigateByInstructionInputDialog()
        {
            double scaleX = GlobalSetting.LadderScaleTransform.ScaleX;
            double scaleY = GlobalSetting.LadderScaleTransform.ScaleY;
            Point point = _selectRect.TranslatePoint(new Point(0, 0), MainScrollViewer);
            if (point.X < 0)
            {
                MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + point.X);
            }
            if (point.X + 2 * _selectRect.ActualWidth * scaleX > MainScrollViewer.ViewportWidth)
            {
                MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + point.X + 2 * _selectRect.ActualWidth * scaleX - MainScrollViewer.ViewportWidth);
            }
            if (point.Y < 0)
            {
                MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + point.Y);
            }
            if (point.Y + _selectRect.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
            {
                MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + point.Y + _selectRect.ActualHeight * scaleY - MainScrollViewer.ViewportHeight);
            }
            if (_selectRect.X == GlobalSetting.LadderXCapacity - 1)
            {
                MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + MainScrollViewer.ScrollableWidth);
            }
            if (_selectRect.X == 0)
            {
                MainScrollViewer.ScrollToHorizontalOffset(0);
            }
        }
        /// <summary>
        /// 精确定位到指定网络号的网络
        /// </summary>
        /// <param name="num">网络号</param>
        public void NavigateToNetworkByNum(int num)
        {
            VScrollToRect(num, _selectRect.Y);
            HScrollToRect(_selectRect.X);
        }
        /// <summary>
        /// 纵向精确定位到指定网络的行
        /// </summary>
        /// <param name="networkNumber">网络号</param>
        /// <param name="row">行号</param>
        public void VScrollToRect(int networkNumber, int row)
        {
            double scale = GlobalSetting.LadderScaleTransform.ScaleY;
            double offset = scale * MainBorder.ActualHeight;
            foreach (var network in Core.Children.Where(x => { return x.ID < networkNumber; }))
                offset += scale * network.View.ActualHeight;
            offset += scale * _selectRect.ActualHeight * row;
            offset = Math.Max(0, offset);
            MainScrollViewer.ScrollToVerticalOffset(offset);
        }
        /// <summary>
        /// 横向精确定位到指定网络的列
        /// </summary>
        /// <param name="XIndex"></param>
        public void HScrollToRect(int XIndex)
        {
            double scale = GlobalSetting.LadderScaleTransform.ScaleX;
            double offset = 0;
            offset += scale * GlobalSetting.LadderWidthUnit * (XIndex + 1);
            offset -= MainScrollViewer.ViewportWidth / 1.3;
            if (MainScrollViewer.ViewportWidth == 0) offset = 0;
            MainScrollViewer.ScrollToHorizontalOffset(offset);
        }

        public void Select(LadderNetworkModel network, int x1, int x2, int y1, int y2)
        {
            _selectRect.Core.Parent = null;
            _selectArea.Core.Select(network.ID, x1, y1, x2, y2);
            _selectStatus = SelectStatus.MultiSelected;
            NavigateToNetworkByNum(network.ID);
        }

        public void Select(int start, int end)
        {
            _selectRect.Core.Parent = null;
            _selectArea.Core.Select(start, end);
            _selectStatus = SelectStatus.MultiSelected;
            NavigateToNetworkByNum(start);
        }

        #endregion
        
        #region Expand

        //private bool isexpand;
        public bool IsExpand
        {
            get
            {
                return Core.IsExpand;
            }
            set
            {
                if (Core.IsExpand == value) return;
                Core.IsExpand = value;
            }
        }
        
        private void RemoveToolTipByLadder(ToolTip tooltip)
        {
            if (tooltip != null)
            {
                StackPanel stackpanel = (StackPanel)((ScrollViewer)tooltip.Content).Content;
                stackpanel.LayoutTransform = null;
                stackpanel.Children.Clear();
            }
        }

        private ToolTip GenerateToolTipByLadder()
        {
            ToolTip tooltip = new ToolTip();
            ScrollViewer scroll = new ScrollViewer();
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            StackPanel stackpanel = new StackPanel();
            scroll.MaxHeight = 385;
            stackpanel.Background = Brushes.White;
            stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            ScaleTransform transform = new ScaleTransform(GlobalSetting.LadderOriginScaleX / 1.7, GlobalSetting.LadderOriginScaleY / 1.7);
            foreach (LadderNetworkModel net in Core.Children)
            {
                if (net.View != null) stackpanel.Children.Add(net.View);
            }
            stackpanel.LayoutTransform = transform;
            scroll.Content = stackpanel;
            tooltip.Content = scroll;
            return tooltip;
        }

        #endregion

        #region Dynamic

        private bool isviewmodified;
        public bool IsViewModified
        {
            get { return this.isviewmodified; }
            set { this.isviewmodified = value; }
        }

        #endregion

        #region network drag(only not IsExpand)

        private LadderNetworkViewModel dragItem;
        private LadderNetworkViewModel currentItem;
        private void OnLadderDiagramMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _selectStatus = SelectStatus.Idle;
            }
            if (LadderMode != LadderModes.Edit)
            {
                return;
            }
            var network = GetNetworkByMouse();
            if (network != null)
            {
                if (!network.ladderExpander.IsExpand)
                {
                    dragItem = network;
                }
                else
                {
                    dragItem = null;
                }
            }
        }
        private void OnDrop(object sender, DragEventArgs e)
        {
            var sourcenet = (LadderNetworkViewModel)e.Data.GetData(typeof(LadderNetworkViewModel));
            var desnetwork = (LadderNetworkViewModel)e.Source;
            if (sourcenet == null) return;
            if (sourcenet != desnetwork)
            {
                desnetwork.Opacity = 0.3;
                desnetwork.ladderExpander.IsExpand = false;
                Core.ExchangeN(sourcenet.Core, desnetwork.Core);
            }
            sourcenet.CommentAreaBorder.BorderBrush = Brushes.Brown;
            sourcenet.CommentAreaBorder.BorderThickness = new Thickness(4);
            desnetwork.Opacity = 1;
            dragItem = null;
            currentItem = null;
        }
        private void OnDragOver(object sender, DragEventArgs e)
        {
            var sourcenet = (LadderNetworkViewModel)e.Data.GetData(typeof(LadderNetworkViewModel));
            var desnetwork = (LadderNetworkViewModel)e.Source;
            if (sourcenet == null) return;
            if (sourcenet != desnetwork)
            {
                sourcenet.CommentAreaBorder.BorderBrush = GlobalSetting.MonitorBrush;
                sourcenet.CommentAreaBorder.BorderThickness = new Thickness(6);
                desnetwork.Opacity = 0.3;
                desnetwork.ladderExpander.IsExpand = false;
            }
        }
        private void OnDragLeave(object sender, DragEventArgs e)
        {
            ((LadderNetworkViewModel)e.Source).Opacity = 1;
            if (dragItem == null) return;
            dragItem.CommentAreaBorder.BorderBrush = Brushes.Brown;
            dragItem.CommentAreaBorder.BorderThickness = new Thickness(4);
        }

        #endregion

        #region ContextMenu

        private LadderEditMenu cmEdit;
        public LadderEditMenu CMEdit { get { return this.cmEdit; } }

        private LadderMonitorMenu cmMoni;
        public LadderMonitorMenu CMMoni { get { return this.cmMoni; } }

        #endregion

        public void Update()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("TabHeader"));
            PropertyChanged(this, new PropertyChangedEventArgs("ProgramName"));
            PropertyChanged(this, new PropertyChangedEventArgs("LadderComment"));
            if (!IsExpand)
            {
                MainCanvas.Children.Clear();
                if (ThumbnailButton.ToolTip == null)
                {
                    ThumbnailButton.ToolTip = GenerateToolTipByLadder();
                    TitleStackPanel.Children.Add(ThumbnailButton);
                }
            }
            else
            { 
                if (ThumbnailButton.ToolTip != null)
                {
                    RemoveToolTipByLadder((ToolTip)ThumbnailButton.ToolTip);
                    ThumbnailButton.ToolTip = null;
                    TitleStackPanel.Children.Remove(ThumbnailButton);
                }
                MainCanvas.Children.Clear();
                if (_selectRect != null)
                    MainCanvas.Children.Add(_selectRect);
                if (_selectArea != null)
                    MainCanvas.Children.Add(_selectArea);
                foreach (LadderNetworkModel net in core.Children)
                {
                    if (net.View == null)
                        net.View = new LadderNetworkViewModel(net);
                    MainCanvas.Children.Add(net.View);
                }
                core.UpdateCanvasTop();
            }
        }

        public InstructionDiagramViewModel Inst
        {
            get { return core != null && core.Inst != null ? core.Inst.View : null; }
        }
        
        public LadderModes LadderMode { get { return core.LadderMode; } }
        public bool IsCommentMode { get { return core.IsCommentMode; } }
        
        private int WidthUnit { get { return GlobalSetting.LadderWidthUnit; } }
        private int HeightUnit { get { return IsCommentMode ? GlobalSetting.LadderCommentModeHeightUnit : GlobalSetting.LadderHeightUnit; } }

        #endregion

        #region Event Handler

        public event RoutedEventHandler SelectionChanged = delegate { };
        
        private void MainScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            isviewmodified = true;
        }

        private void MainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            isviewmodified = true;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            isviewmodified = true;
        }

        #region Expander

        private void OnExpanderMouseEnter(object sender, MouseEventArgs e)
        {
            Rect.Fill = GlobalSetting.FoldingBrush;
            Rect.Opacity = 0.08;
            ladderExpander.Rect.Fill = GlobalSetting.FoldingBrush;
            ladderExpander.Rect.Opacity = 0.2;
        }

        private void OnExpanderMouseLeave(object sender, MouseEventArgs e)
        {
            Rect.Fill = Brushes.Transparent;
            Rect.Opacity = 1;
            ladderExpander.Rect.Fill = Brushes.Transparent;
            ladderExpander.Rect.Opacity = 1;
        }

        private void OnExpandChanged(object sender, RoutedEventArgs e)
        {
            IsExpand = ladderExpander.IsExpand;
        }
        
        private void ThumbnailButton_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            _canScrollToolTip = false;
        }
        private void ThumbnailButton_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            _canScrollToolTip = true;
        }
        
        private void OnThumbnailButtonMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IsExpand = !IsExpand;
        }

        private bool _canScrollToolTip = false;
        private void OnThumbnailButtonMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_canScrollToolTip)
            {
                ScrollViewer scroll = (ScrollViewer)((ToolTip)ThumbnailButton.ToolTip).Content;
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta / 10);
            }
        }

        #endregion

        #region Keyboard & Mouse

        [DllImport("user32.dll", EntryPoint = "GetKeyboardState")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        public static bool CapsLockStatus
        {
            get
            {
                byte[] bs = new byte[256];
                GetKeyboardState(bs);
                return (bs[0x14] == 1);
            }
        }

        private bool ispressingctrl;
        private void OnLadderDiagramKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                ispressingctrl = false;
            }
            if (_selectStatus == SelectStatus.MultiSelecting)
            {
                _selectStatus = SelectStatus.MultiSelected;
            }
        }
        private void OnLadderDiagramKeyDown(object sender, KeyEventArgs e)
        {
            if (IFParent.IsWaitForKey)
            {
                return;
            }
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                ispressingctrl = true;
            }
            if (e.Key == Key.Left)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    SelectRectLeftWithLine((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
                }
                else if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    SelectionAreaChanged(e.Key);
                }
                else
                {
                    SelectRectLeft();
                }
                e.Handled = true;
            }
            if (e.Key == Key.Right)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    SelectRectRightWithLine((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
                }
                else if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    SelectionAreaChanged(e.Key);
                }
                else
                {
                    SelectRectRight();
                }
                e.Handled = true;
            }
            if (e.Key == Key.Down)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    SelectRectDownWithLine((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
                }
                else if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    SelectionAreaChanged(e.Key);
                }
                else
                {
                    SelectRectDown();
                }
                e.Handled = true;
            }
            if (e.Key == Key.Up)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    SelectRectUpWithLine((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
                }
                else if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    SelectionAreaChanged(e.Key);
                }
                else
                {
                    SelectRectUp();
                }
                e.Handled = true;
            }
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                if (LadderMode != LadderModes.Edit) return;
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
                {
                    char c;
                    bool isupper = CapsLockStatus;
                    isupper ^= ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
                    c = (char)((int)e.Key + (isupper ? 21 : 53));
                    string s = new string(c, 1);
                    IFParent.ShowInstructionInputDialog(s, _selectRect.Core);
                }
            }
            if (LadderMode == LadderModes.Edit)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    switch (e.Key)
                    {
                        case Key.OemPlus: IFParent.ShowInstructionInputDialog("ADD", _selectRect.Core); break;
                        case Key.OemMinus: IFParent.ShowInstructionInputDialog("SUB", _selectRect.Core); break;
                        case Key.D8: IFParent.ShowInstructionInputDialog("MUL", _selectRect.Core); break;
                        case Key.Oem5: IFParent.ShowInstructionInputDialog("DIV", _selectRect.Core); break;

                        case Key.F2: IFParent.ShowInstructionInputDialog("LDIM ", _selectRect.Core); break;
                        case Key.F3: IFParent.ShowInstructionInputDialog("LDIIM ", _selectRect.Core); break;
                        case Key.F5: Core.QuickInsertElement(LadderUnitModel.Types.MEP, _selectRect.Core); break;
                        case Key.F6: Core.QuickInsertElement(LadderUnitModel.Types.MEF, _selectRect.Core); break;
                        case Key.F8: IFParent.ShowInstructionInputDialog("OUTIM ", _selectRect.Core); break;
                        case Key.F9:
                            if (_selectRect.Current.Type == LadderUnitModel.Types.HLINE)
                                Core.RemoveSingleUnit(_selectRect.Current);
                            SelectRectRight();
                            break;
                        case Key.F10:
                            if (_selectRect.Current.Type == LadderUnitModel.Types.VLINE)
                                Core.RemoveSingleUnit(_selectRect.Current);
                            SelectRectDown();
                            break;
                    }
                }
                else if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
                {
                    switch (e.Key)
                    {
                        case Key.F2: IFParent.ShowInstructionInputDialog("LD ", _selectRect.Core); break;
                        case Key.F3: IFParent.ShowInstructionInputDialog("LDI ", _selectRect.Core); break;
                        case Key.F5: IFParent.ShowInstructionInputDialog("LDP ", _selectRect.Core); break;
                        case Key.F6: IFParent.ShowInstructionInputDialog("LDF ", _selectRect.Core); break;
                        case Key.F7: Core.QuickInsertElement(LadderUnitModel.Types.INV, _selectRect.Core); break;
                        case Key.F8: IFParent.ShowInstructionInputDialog("OUT ", _selectRect.Core); break;
                        case Key.F9: Core.QuickInsertElement(LadderUnitModel.Types.HLINE, _selectRect.Core); break;
                        case Key.F10: Core.QuickInsertElement(LadderUnitModel.Types.VLINE, _selectRect.Core); break;
                    }
                }
            }
            if (e.Key == Key.Enter)
            {
                if (LadderMode != LadderModes.Edit) return;
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    if (_selectRect.ViewParent != null)
                    {
                        isnavigatable = false;
                        Core.AddR(SelectRectOwner, SelectRectOwner.RowCount);
                        _selectRect.X = 0;
                        _selectRect.Y = SelectRectOwner.RowCount - 1;
                        NavigateByInstructionInputDialog();
                        isnavigatable = true;
                        return;
                    }
                }
                if (_selectRect.Current != null && _selectRect.Current.Type != LadderUnitModel.Types.HLINE)
                    IFParent.ShowElementPropertyDialog(_selectRect.Current);
                else
                    IFParent.ShowInstructionInputDialog(string.Empty, _selectRect.Core);
                e.Handled = true;
            }
            if (e.Key == Key.Delete)
            {
                if(SelectionStatus == SelectStatus.SingleSelected && (e.KeyboardDevice.Modifiers ^ ModifierKeys.Control) == ModifierKeys.None)
                {
                    Core.RemoveR(SelectRectOwner, _selectRect.Y);
                }
                else Delete();
            }
        }
        private void OnLadderDiagramMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                int scaleX = e.Delta / 100;
                int scaleY = scaleX;
                GlobalSetting.LadderScaleX += scaleX * 0.03;
                GlobalSetting.LadderScaleY += scaleY * 0.03;
                // 不继续冒泡传递事件
                e.Handled = true;
            }
        }
        /*
        private void OnLadderDiagramMouseMove(object sender, MouseEventArgs e)
        {
            Point _p = e.GetPosition(this);
            if (_selectStatus == SelectStatus.SingleSelected)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (SelectRectOwner != null)
                    {
                        var p = e.GetPosition(SelectRectOwner.View.LadderCanvas);
                        var pp = IntPoint.GetIntpointByDouble(p.X, p.Y, WidthUnit, HeightUnit);
                        if (p.Y < 0 || (pp.X != _selectRect.X) || (pp.Y != _selectRect.Y))
                        {
                            _selectStartNetwork = SelectRectOwner.View;
                            _selectStartNetwork.SelectAreaOriginFX = _selectRect.X;
                            _selectStartNetwork.SelectAreaOriginFY = _selectRect.Y;
                            _selectStartNetwork.SelectAreaOriginSX = pp.X;
                            _selectStartNetwork.SelectAreaFirstX = _selectRect.X;
                            _selectStartNetwork.SelectAreaFirstY = _selectRect.Y;
                            _selectStartNetwork.SelectAreaSecondX = _selectRect.X;
                            _selectStartNetwork.SelectAreaSecondY = _selectRect.Y;
                            _selectStatus = SelectStatus.MultiSelecting;
                        }
                    }
                }
            }
            if (_selectStatus == SelectStatus.MultiSelecting)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    CrossNetState = GetSelectionCrossNetworkState();
                    switch (CrossNetState)
                    {
                        case CrossNetworkState.NoCross:
                            foreach (var net in _selectAllNetworkCache)
                            {
                                net.IsSelectAreaMode = false;
                                net.IsSelectAllMode = false;
                            }
                            var p = e.GetPosition(_selectStartNetwork.LadderCanvas);
                            var pp = IntPoint.GetIntpointByDouble(p.X, p.Y, WidthUnit, HeightUnit);
                            _selectStartNetwork.SelectAreaOriginSX = pp.X;
                            _selectStartNetwork.IsSelectAllMode = false;
                            if (pp.X == _selectStartNetwork.SelectAreaOriginFX
                             && pp.Y == _selectStartNetwork.SelectAreaOriginFY)
                            {
                                SelectionRect.X = pp.X;
                                SelectionRect.Y = pp.Y;
                                _selectStartNetwork.AcquireSelectRect();
                            }
                            else
                            {
                                _selectStartNetwork.SelectAreaFirstX = _selectStartNetwork.SelectAreaOriginFX;
                                _selectStartNetwork.SelectAreaFirstY = _selectStartNetwork.SelectAreaOriginFY;
                                _selectStartNetwork.SelectAreaSecondX = pp.X;
                                _selectStartNetwork.SelectAreaSecondY = pp.Y;
                            }
                            break;
                        case CrossNetworkState.CrossUp:
                            CollectSelectAllNetworkUp();
                            _selectStartNetwork.IsSelectAllMode = true;
                            foreach (var net in _selectAllNetworkCache)
                            {
                                net.IsSelectAreaMode = false;
                                net.IsSelectAllMode = false;
                            }
                            foreach (var net in _selectAllNetworks)
                            {
                                net.IsSelectAllMode = true;
                            }
                            break;
                        case CrossNetworkState.CrossDown:
                            CollectSelectAllNetworkDown();
                            _selectStartNetwork.IsSelectAllMode = true;
                            foreach (var net in _selectAllNetworkCache)
                            {
                                net.IsSelectAreaMode = false;
                                net.IsSelectAllMode = false;
                            }
                            foreach (var net in _selectAllNetworks)
                            {
                                net.IsSelectAllMode = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void OnLadderDiagramMouseUp(object sender, MouseButtonEventArgs e)
        {
            // 如果处于选择模式则关闭
            if (_selectStatus == SelectStatus.MultiSelecting)
            {
                _selectStatus = SelectStatus.MultiSelected;
            }
        }
        */
        private void OnCommentAreaMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IFParent.ShowEditDiagramCommentDialog(Core);
        }
        private void OnEditComment(object sender, RoutedEventArgs e)
        {
            IFParent.ShowEditDiagramCommentDialog(Core);
        }

        #endregion

        #region Command
        
        public void Delete()
        {
            if (SelectionStatus == SelectStatus.SingleSelected)
            {
                if (_selectRect.Current != null)
                    Core.RemoveSingleUnit(_selectRect.Current);
            }
            else
            {
                if (SelectionStatus == SelectStatus.MultiSelected)
                {
                    // 多网络复制
                    if (_selectArea.Core.State == SelectAreaCore.Status.SelectCross)
                    {
                        List<LadderNetworkModel> removes = _selectArea.Core.SelectNetworks.ToList();
                        Core.ReplaceN(removes, new LadderNetworkModel[] { });
                        _selectArea.Core.Release();
                        _selectStatus = SelectStatus.Idle;
                    }
                    else if (_selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                    {
                        Core.RemoveU(SelectStartNetwork, _selectArea.Core.SelectUnits);
                        _selectArea.Core.Release();
                        _selectStatus = SelectStatus.Idle;
                    }
                }
            }
        }

        private void CutAndCopy(bool cut)
        {
            XElement xele = new XElement("Root");
            xele.SetAttributeValue("SelectionStatus", (int)SelectionStatus);
            xele.SetAttributeValue("CrossNetworkState", (int)CrossNetState);
            if (SelectionStatus == SelectStatus.SingleSelected)
            {
                // 单元素复制
                if (_selectRect.Current != null)
                {
                    XElement xele_u = new XElement("Current");
                    _selectRect.Current.Save(xele_u);
                    xele.Add(xele_u);
                    Clipboard.SetData("LadderContent", xele.ToString());
                    if (cut) Core.RemoveSingleUnit(_selectRect.Current);
                }
            }
            else
            {
                if (SelectionStatus == SelectStatus.MultiSelected)
                {
                    // 多网络复制
                    if (_selectArea.Core.State == SelectAreaCore.Status.SelectCross)
                    {
                        XElement xele_ns = new XElement("Networks");
                        xele.Add(xele_ns);
                        List<LadderNetworkModel> removes = new List<LadderNetworkModel>();
                        foreach (LadderNetworkModel lnmodel in _selectArea.Core.SelectNetworks)
                        {
                            XElement xele_n = new XElement("Network");
                            lnmodel.Save(xele_n);
                            xele_ns.Add(xele_n);
                            removes.Add(lnmodel);
                        }
                        if (cut) Core.ReplaceN(removes, new LadderNetworkModel[] { });
                        Clipboard.SetData("LadderContent", xele.ToString());
                        _selectStatus = SelectStatus.Idle;
                    }
                    else if (_selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                    {
                        // 单网络多图元复制
                        xele.SetAttributeValue("XBegin", _selectArea.Core.XStart);
                        xele.SetAttributeValue("YBegin", _selectArea.Core.YStart);
                        xele.SetAttributeValue("XEnd", _selectArea.Core.XEnd);
                        xele.SetAttributeValue("YEnd", _selectArea.Core.YEnd);
                        XElement xele_us = new XElement("Units");
                        xele.Add(xele_us);
                        List<LadderUnitModel> units = _selectArea.Core.SelectUnits.ToList();
                        foreach (LadderUnitModel unit in units)
                        {
                            XElement xele_u = new XElement("Unit");
                            unit.Save(xele_u);
                            xele_us.Add(xele_u);
                        }
                        Clipboard.SetData("LadderContent", xele.ToString());
                        if (cut) Core.RemoveU(SelectStartNetwork, units);
                        _selectStatus = SelectStatus.Idle;
                    }
                }
            }
        }

        private void OnCutCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CutAndCopy(true);
        }
        private void OnCopyCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CutAndCopy(false);
        }
        private void OnPasteCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                string xmltext = Clipboard.GetData("LadderContent") as string;
                if (xmltext != null)
                {
                    XElement xele = XElement.Parse(xmltext);
                    XElement xele_c = xele.Element("Current");
                    XElement xele_us = xele.Element("Units");
                    XElement xele_ns = xele.Element("Networks");
                    if (xele_c != null)
                    {
                        LadderUnitModel unit = LadderUnitModel.Create(SelectRectOwner, xele_c);
                        unit.X = _selectRect.X;
                        unit.Y = _selectRect.Y;
                        Core.AddSingleUnit(unit, SelectRectOwner, false);
                    }
                    if (xele_us != null)
                    { 
                        List<LadderUnitModel> units = new List<LadderUnitModel>();
                        int _xBegin = int.Parse(xele.Attribute("XBegin").Value);
                        int _yBegin = int.Parse(xele.Attribute("YBegin").Value);
                        int _xEnd = int.Parse(xele.Attribute("XEnd").Value);
                        int _yEnd = int.Parse(xele.Attribute("YEnd").Value);
                        int _xWidth = _xEnd - _xBegin + 1;
                        int _yHeight = _yEnd - _yBegin + 1;
                        foreach (XElement xele_u in xele_us.Elements("Unit"))
                        {
                            LadderUnitModel unit = LadderUnitModel.Create(SelectRectOwner, xele_u);
                            units.Add(unit);
                        }
                        bool containoutput = units.Where(u => u.Shape == LadderUnitModel.Shapes.Output || u.Shape == LadderUnitModel.Shapes.OutputRect).Count() > 0;
                        int xBegin = containoutput
                            ? GlobalSetting.LadderXCapacity - _xWidth : SelectRectOwner != null
                            ? _selectRect.X : _selectArea.Core.XStart;
                        int yBegin = SelectRectOwner != null
                            ? _selectRect.Y : _selectArea.Core.YStart;
                        int xEnd = Math.Min(xBegin + _xWidth - 1, GlobalSetting.LadderXCapacity - 1 - (containoutput ? 0 : 1));
                        int yEnd = yBegin + _yHeight - 1;
                        int xWidth = xEnd - xBegin + 1;
                        int yHeight = yEnd - yBegin + 1;
                        units = units.Where(u => u.X >= _xBegin && u.X < _xBegin + xWidth && u.Y >= _yBegin && u.Y < _yBegin + yHeight).ToList();
                        foreach (LadderUnitModel unit in units)
                        {
                            unit.X += xBegin - _xBegin;
                            unit.Y += yBegin - _yBegin;
                        }
                        IEnumerable<LadderUnitModel> oldunits = SelectRectOwner.Children.SelectRange(xBegin, xEnd, yBegin, yEnd);
                        oldunits = oldunits.Concat(SelectRectOwner.VLines.SelectRange(xBegin, xEnd, yBegin, yEnd));
                        Core.ReplaceU(SelectRectOwner, oldunits, units);
                    }
                    if (xele_ns != null)
                    {
                        List<LadderNetworkModel> nets = new List<LadderNetworkModel>();
                        int nStart = SelectRectOwner != null
                            ? SelectRectOwner.ID : CrossNetState == CrossNetworkState.CrossUp && _selectArea.Core.SelectNetworks.Count() > 0
                            ? _selectArea.Core.SelectNetworks.Select(n => n.ID).Min()
                            : SelectStartNetwork.ID;
                        int nEnd = nStart - 1;
                        foreach (XElement xele_n in xele_ns.Elements("Network"))
                        {
                            LadderNetworkModel net = new LadderNetworkModel(Core, 0);
                            net.Load(xele_n);
                            net.ID = ++nEnd;
                            nets.Add(net);
                        }
                        IEnumerable<LadderNetworkModel> oldnets = SelectRectOwner != null
                            ? new LadderNetworkModel[] { SelectRectOwner }
                            : new LadderNetworkModel[] { SelectStartNetwork }.Concat(_selectArea.Core.SelectNetworks);
                        Core.ReplaceN(oldnets, nets);
                    }
                }
            }
            catch (Exception exc)
            {

            }
        }
        private void OnUndoCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Core.Undo();
        }
        private void OnRedoCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Core.Redo();
        }
        private void OnSelectAllCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            _selectRect.Core.Parent = null;
            _selectArea.Core.Select(0, core.NetworkCount - 1);
            _selectStatus = SelectStatus.MultiSelected;
        }
        private void OnFindCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            IFParent.WNDMain.LACFind.Show();
        }
        private void OnReplaceCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            IFParent.WNDMain.LACReplace.Show();
        }
        
        private void CutCopyCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IFParent == null || LadderMode != LadderModes.Edit || IFParent.IsWaitForKey)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = SelectionStatus == SelectStatus.SingleSelected;
            e.CanExecute &= _selectRect.Current != null;
            e.CanExecute |= SelectionStatus == SelectStatus.MultiSelected;
            
        }
        private void PasteCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IFParent == null || LadderMode != LadderModes.Edit || IFParent.IsWaitForKey)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = SelectionStatus == SelectStatus.MultiSelected;
            e.CanExecute |= SelectionStatus == SelectStatus.SingleSelected;
            e.CanExecute &= Clipboard.ContainsData("LadderContent");
        }
        private void UndoCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IFParent == null || LadderMode != LadderModes.Edit || IFParent.IsWaitForKey)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = Core.CanUndo;
        }
        private void RedoCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IFParent == null || LadderMode != LadderModes.Edit || IFParent.IsWaitForKey)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = Core.CanRedo;
        }
        private void FindCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IFParent == null || LadderMode != LadderModes.Edit || IFParent.IsWaitForKey)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = true;
        }
        private void ReplaceCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IFParent == null || LadderMode != LadderModes.Edit || IFParent.IsWaitForKey)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = true;
        }
        private void SelectAllCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IFParent == null || IFParent.IsWaitForKey)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = true;
        }

        #endregion

        #region MainCanvas
        
        private void OnMainCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                object obj = GetObjectByMouse(e);
                switch (SelectionStatus)
                {
                    case SelectStatus.SingleSelecting:
                        if (obj is LadderUnitModel)
                        {
                            LadderUnitModel unit = (LadderUnitModel)obj;
                            if (unit.Parent == SelectRectOwner && (unit.X != _selectRect.X || unit.Y != _selectRect.Y))
                                _selectArea.Core.Select(unit.Parent.ID, _selectRect.X, _selectRect.Y, unit.X, unit.Y);
                            else if (unit.Parent != SelectRectOwner)
                                _selectArea.Core.Select(SelectRectOwner.ID, unit.Parent.ID);
                        }
                        else if (obj is LadderNetworkPositionModel)
                        {
                            LadderNetworkPositionModel pos = (LadderNetworkPositionModel)obj;
                            if (pos.Network == SelectRectOwner && (pos.X != _selectRect.X || pos.Y != _selectRect.Y))
                                _selectArea.Core.Select(pos.Network.ID, _selectRect.X, _selectRect.Y, pos.X, pos.Y);
                            else if (pos.Network != SelectRectOwner)
                                _selectArea.Core.Select(SelectRectOwner.ID, pos.Network.ID);
                        }
                        else if (obj is LadderNetworkModel)
                        {
                            LadderNetworkModel net = (LadderNetworkModel)obj;
                            _selectArea.Core.Select(SelectRectOwner.ID, net.ID);
                        }
                        if (_selectArea.Core.State != SelectAreaCore.Status.NotSelected)
                        {
                            _selectRect.Core.Parent = null;
                            _selectStatus = SelectStatus.MultiSelecting;
                        }
                        break;
                    case SelectStatus.MultiSelecting:
                        if (obj is LadderUnitModel)
                        {
                            LadderUnitModel unit = (LadderUnitModel)obj;
                            _selectArea.Core.Move(unit);
                        }
                        else if (obj is LadderNetworkPositionModel)
                        {
                            LadderNetworkPositionModel pos = (LadderNetworkPositionModel)obj;
                            _selectArea.Core.Move(pos);
                        }
                        else if (obj is LadderNetworkModel)
                        {
                            LadderNetworkModel net = (LadderNetworkModel)obj;
                            _selectArea.Core.Move(net);
                        }
                        if (_selectArea.Core.XStart == _selectArea.Core.XEnd
                         && _selectArea.Core.YStart == _selectArea.Core.YEnd)
                        {
                            _selectRect.Core.Parent = core.Children[_selectArea.Core.NetStart];
                            _selectRect.Core.X = _selectArea.Core.XStart;
                            _selectRect.Core.Y = _selectArea.Core.YStart;
                            _selectArea.Core.Release();
                            _selectStatus = SelectStatus.SingleSelecting;
                        }
                        break;
                }
                var p1 = e.GetPosition(MainScrollViewer);
                if (MainScrollViewer.ViewportHeight < p1.Y)
                {
                    MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + (p1.Y - MainScrollViewer.ViewportHeight) * GlobalSetting.LadderScaleY * 0.2);
                }
                else if (p1.Y < 0)
                {
                    MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + p1.Y * GlobalSetting.LadderScaleY * 0.2);
                }
                else if (p1.X < 0)
                {
                    MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + p1.X * GlobalSetting.LadderScaleX * 0.8);
                }
                else if (MainScrollViewer.ViewportWidth < p1.X)
                {
                    MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + (p1.X - MainScrollViewer.ViewportWidth) * GlobalSetting.LadderScaleX * 0.8);
                }
            }
            /*
            currentItem = GetNetworkByMouse();
            if (currentItem == null && dragItem != null)
            {
                dragItem.Opacity = 1;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (currentItem != null && dragItem != null)
                {
                    DragDrop.DoDragDrop(this, dragItem, DragDropEffects.Move);
                }
            }
            */
        }
        
        private void OnMainCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.CaptureMouse();
            object obj = GetObjectByMouse(e);
            if (obj is LadderUnitModel)
            {
                LadderUnitModel unit = (LadderUnitModel)obj;
                _selectArea.Core.Release();
                _selectRect.Core.Current = unit;
                _selectStatus = SelectStatus.SingleSelecting;
            }
            else if (obj is LadderNetworkPositionModel)
            {
                LadderNetworkPositionModel pos = (LadderNetworkPositionModel)obj;
                _selectArea.Core.Release();
                _selectRect.Core.Parent = pos.Network;
                _selectRect.Core.X = pos.X;
                _selectRect.Core.Y = pos.Y;
                _selectStatus = SelectStatus.SingleSelecting;
            }
            else if (obj is LadderNetworkModel)
            {

            }
            else
            {
                _selectArea.Core.Release();
                _selectRect.Core.Parent = null;
                _selectStatus = SelectStatus.Idle;
            }
            if (e.ClickCount == 2 && _selectRect.Core.Parent != null)
            { 
                LadderUnitModel unit = _selectRect.Current;
                if (LadderMode == LadderModes.Edit)
                {
                    if (unit == null || unit.Shape == LadderUnitModel.Shapes.HLine || unit.Shape == LadderUnitModel.Shapes.Special)
                        IFParent.ShowInstructionInputDialog("", _selectRect.Core);
                    else
                        IFParent.ShowElementPropertyDialog(unit);
                }
                else if (unit != null)
                {
                    IFParent.ShowValueModifyDialog(unit.UniqueChildren);
                }
            }
        }
        
        private void OnMainCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.ReleaseMouseCapture();
            switch (SelectionStatus)
            {
                case SelectStatus.SingleSelecting:
                    _selectStatus = SelectStatus.SingleSelected;
                    break;
                case SelectStatus.MultiSelecting:
                    _selectStatus = SelectStatus.MultiSelected;
                    break;
            }
        }
        
        private object GetObjectByMouse(MouseEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);
            int x = (int)(p.X / WidthUnit);
            foreach (LadderNetworkModel net in Core.Children)
            {
                if (p.Y >= net.CanvasTop && p.Y < net.UnitBaseTop) return net;
                if (!net.IsExpand || net.IsMasked) continue;
                int y = (int)((p.Y - net.UnitBaseTop) / HeightUnit);
                if (x >= 0 && x < GlobalSetting.LadderXCapacity
                 && y >= 0 && y < net.RowCount)
                {
                    LadderUnitModel unit = net.Children[x, y];
                    if (unit != null) return unit;
                    return new LadderNetworkPositionModel(net, x, y);
                }
            }
            return null;
        }

        private LadderNetworkViewModel GetNetworkByMouse()
        {
            foreach (LadderNetworkModel net in Core.Children)
            {
                if (net.View != null && net.View.IsMouseOver)
                    return net.View;
            }
            return null;
        }

        #endregion

        #endregion

    }
    
    public class LadderNetworkPositionModel
    {
        public LadderNetworkModel Network { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public LadderNetworkPositionModel(LadderNetworkModel _network, int _x, int _y)
        {
            Network = _network;
            X = _x; Y = _y;
        }
    }

}

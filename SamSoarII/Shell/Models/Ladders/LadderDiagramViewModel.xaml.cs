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
using SamSoarII.Shell.Windows;
using SamSoarII.Shell.Dialogs;
using System.Windows.Controls.Primitives;

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
        UnitDraging,
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
            cmEdit.Post += OnLadderEdit;
            ladderExpander.MouseEnter += OnExpanderMouseEnter;
            ladderExpander.MouseLeave += OnExpanderMouseLeave;
            ladderExpander.line.Visibility = Visibility.Hidden;
            ladderExpander.line1.Visibility = Visibility.Hidden;
            ladderExpander.expandButton.IsExpandChanged += OnExpandChanged;
            ladderExpander.IsExpand = IsExpand;
            TitleStackPanel.Children.Remove(ThumbnailButton);
            loadedrowstart = 0;
            loadedrowend = -1;
            Update();
        }
        
        public void Dispose()
        {
            _selectRect.Dispose();
            _selectArea.Dispose();
            outline.Dispose();
            cmEdit.Post -= OnLadderEdit;
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
                    ladderExpander.IsExpand = IsExpand;
                    if (!IsExpand)
                    {
                        DynamicDispose();
                        MainCanvas.Visibility = Visibility.Hidden;
                        LadderCanvas.Visibility = Visibility.Hidden;
                        MainCanvas.Height = 0;
                        //LadderCanvas.Height = 0;
                        if (ThumbnailButton.ToolTip == null)
                        {
                            ThumbnailButton.ToolTip = GenerateToolTipByLadder();
                            TitleStackPanel.Children.Add(ThumbnailButton);
                        }
                    }
                    else
                    {
                        MainCanvas.Visibility = Visibility.Visible;
                        LadderCanvas.Visibility = Visibility.Visible;
                        core.UpdateCanvasTop();
                        if (ThumbnailButton.ToolTip != null)
                        {
                            RemoveToolTipByLadder((ToolTip)ThumbnailButton.ToolTip);
                            ThumbnailButton.ToolTip = null;
                            TitleStackPanel.Children.Remove(ThumbnailButton);
                        }
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExpand"));
                    break;
                case "IsCommentMode":
                    _selectRect.IsCommentMode = core.IsCommentMode;
                    _selectArea.IsCommentMode = core.IsCommentMode;
                    break;
                case "CanvasHeight":
                    MainCanvas.Height = core.CanvasHeight;
                    LadderCanvas.Height = core.CanvasHeight;
                    if (_selectRect != null) _selectRect.Update();
                    if (_selectArea != null) _selectArea.Update();
                    foreach (LadderUnitViewModel unit in LadderCanvas.Units)
                        if (unit.Core != null) unit.Update();
                    isviewmodified = true;
                    break;
                case "IsExecuting":
                    if (!core.IsExecuting) isnavigatable = true;
                    break;
            }
        }
        
        private void OnCoreChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (loadedrowstart <= loadedrowend)
            {
                if (e.NewItems != null)
                {
                    if (e.NewStartingIndex <= loadedrowstart)
                    {
                        loadedrowstart += e.NewItems.Count;
                        loadedrowend += e.NewItems.Count;
                    }
                    else if (e.NewStartingIndex <= loadedrowend)
                    {
                        loadedrowend += e.NewItems.Count;
                    }
                }
                if (e.OldItems != null)
                {
                    if (e.OldStartingIndex + e.OldItems.Count - 1 < loadedrowstart)
                    {
                        loadedrowstart -= e.OldItems.Count;
                        loadedrowend -= e.OldItems.Count;
                    }
                    else if (e.OldStartingIndex <= loadedrowend)
                    {
                        DynamicDispose();
                    }
                }
            }
            if (e.OldItems != null)
                foreach (LadderNetworkModel lnmodel in e.OldItems)
                {
                    if (SelectRectOwner == lnmodel)
                        ReleaseSelect();
                    if (lnmodel.View != null)
                    {
                        lnmodel.View.DynamicDispose();
                        lnmodel.View.Visibility = Visibility.Hidden;
                        lnmodel.View.Dispose();
                    }
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
            get
            {
                return this.floatcontrol;
            }
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
            get { return _selectArea.Core.State != SelectAreaCore.Status.NotSelected ? _selectArea.NetOrigin : null; }
        }
        public IEnumerable<LadderNetworkModel> SelectAllNetworks
        {
            get { return _selectArea.Core.SelectNetworks; }
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
                    return true;
                case Key.Down:
                    return true;
                default:
                    return false;
            }
        }
        private void ChangeMutiSelectionArea(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    _selectArea.Core.MoveLeft();
                    if (IsSelectAreaOutOfViewpoint(Directions.Left))
                        NavigateToSelectArea(Directions.Left, Directions.None);
                    break;
                case Key.Right:
                    _selectArea.Core.MoveRight();
                    if (IsSelectAreaOutOfViewpoint(Directions.Right))
                        NavigateToSelectArea(Directions.Right, Directions.None);
                    break;
                case Key.Up:
                    _selectArea.Core.MoveUp();
                    if (IsSelectAreaOutOfViewpoint(Directions.Up))
                    {
                        if (_selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                            NavigateToSelectArea(Directions.None, Directions.Up);
                        else if (_selectArea.Core.NetOrigin == _selectArea.Core.NetEnd
                         && _selectArea.Core.NetStart < _selectArea.Core.NetEnd)
                            NavigateToSelectArea(Directions.None, Directions.Up);
                        else
                            NavigateToSelectArea(Directions.None, Directions.Bottom);
                    }
                    break;
                case Key.Down:
                    _selectArea.Core.MoveDown();
                    if (IsSelectAreaOutOfViewpoint(Directions.Bottom))
                    {
                        if (_selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                            NavigateToSelectArea(Directions.None, Directions.Bottom);
                        else if (_selectArea.Core.NetOrigin == _selectArea.Core.NetStart
                         && _selectArea.Core.NetStart < _selectArea.Core.NetEnd)
                            NavigateToSelectArea(Directions.None, Directions.Bottom);
                        else
                            NavigateToSelectArea(Directions.None, Directions.Up);
                    }
                    break;
                default:
                    break;
            }
            if (_selectArea.Core.State == SelectAreaCore.Status.SelectRange
             && _selectArea.Core.XStart == _selectArea.Core.XEnd
             && _selectArea.Core.YStart == _selectArea.Core.YEnd)
            {
                _selectRect.Core.Parent = core.Children[_selectArea.Core.NetOrigin];
                _selectRect.Core.X = _selectArea.Core.XStart;
                _selectRect.Core.Y = _selectArea.Core.YStart;
                _selectArea.Core.Release();
                _selectStatus = SelectStatus.SingleSelected;
            }
        }
        private void ChangeSingleSelectionArea(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    _selectArea.Core.Select(SelectRectOwner.ID,
                        _selectRect.X, _selectRect.Y, _selectRect.X - 1, _selectRect.Y);
                    if (IsSelectAreaOutOfViewpoint(Directions.Left))
                        NavigateToSelectArea(Directions.Left, Directions.None);
                    break;
                case Key.Right:
                    _selectArea.Core.Select(SelectRectOwner.ID,
                        _selectRect.X, _selectRect.Y, _selectRect.X + 1, _selectRect.Y);
                    if (IsSelectAreaOutOfViewpoint(Directions.Right))
                        NavigateToSelectArea(Directions.Right, Directions.None);
                    break;
                case Key.Up:
                    if (_selectRect.Y == 0)
                    {
                        _selectArea.Core.Select(SelectRectOwner.ID, _selectRect.X, _selectRect.Y, _selectRect.X, _selectRect.Y);
                        _selectArea.Core.Select(SelectRectOwner.ID, SelectRectOwner.ID, Direction.Up);
                    }
                    else
                        _selectArea.Core.Select(SelectRectOwner.ID,
                            _selectRect.X, _selectRect.Y, _selectRect.X, _selectRect.Y - 1);
                    if (IsSelectAreaOutOfViewpoint(Directions.Up))
                        NavigateToSelectArea(Directions.None, Directions.Up);
                    break;
                case Key.Down:
                    if (_selectRect.Y == SelectRectOwner.RowCount - 1)
                    {
                        _selectArea.Core.Select(SelectRectOwner.ID, _selectRect.X, _selectRect.Y, _selectRect.X, _selectRect.Y);
                        _selectArea.Core.Select(SelectRectOwner.ID, SelectRectOwner.ID, Direction.Down);
                    }
                    else
                        _selectArea.Core.Select(SelectRectOwner.ID,
                            _selectRect.X, _selectRect.Y, _selectRect.X, _selectRect.Y + 1);
                    if (IsSelectAreaOutOfViewpoint(Directions.Bottom))
                        NavigateToSelectArea(Directions.None, Directions.Bottom);
                    break;
                default:
                    break;
            }
            _selectRect.Core.Parent = null;
            _selectStatus = SelectStatus.MultiSelected;
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
                        if (!lnmodel.IsMasked && lnmodel.IsExpand) break;
                        id--;
                    }
                    if (id >= 0)
                    {
                        SelectRectOwner = lnmodel;
                        _selectRect.Y = lnmodel.RowCount - 1;
                    }
                }
                if (IsSelectRectOutOfViewpoint(Directions.Up))
                    NavigateToSelectRect(Directions.None, Directions.Up);
            }
        }
        private void SelectRectDown()
        {
            if (SelectRectOwner != null)
            {
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
                        if (!lnmodel.IsMasked && lnmodel.IsExpand) break;
                        id++;
                    }
                    if (id < Core.NetworkCount)
                    {
                        SelectRectOwner = lnmodel;
                        _selectRect.Y = 0;
                    }
                }
                if (IsSelectRectOutOfViewpoint(Directions.Bottom))
                    NavigateToSelectRect(Directions.None, Directions.Bottom);
            }
        }
        private void SelectRectLeft()
        {
            if (SelectRectOwner != null)
            {
                if (_selectRect.X > 0)
                {
                    _selectRect.X--;
                    if (IsSelectRectOutOfViewpoint(Directions.Left))
                        NavigateToSelectRect(Directions.Left, Directions.None);
                }
            }
        }
        public void SelectRectRight()
        {
            if (SelectRectOwner != null)
            {
                if (_selectRect.X < GlobalSetting.LadderXCapacity - 1)
                {
                    _selectRect.X++;
                    if (IsSelectRectOutOfViewpoint(Directions.Right))
                        NavigateToSelectRect(Directions.Right, Directions.None);
                }
            }
        }
        private void SelectRectLeftWithLine(bool expand = false)
        {
            if (core.IsExecuting) return;
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
            }
        }
        private void SelectRectRightWithLine(bool expand = false)
        {
            if (core.IsExecuting) return;
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
            }
        }
        private void SelectRectUpWithLine(bool expand = false)
        {
            if (core.IsExecuting) return;
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
            }
        }
        private void SelectRectDownWithLine(bool expand = false)
        {
            if (core.IsExecuting) return;
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

        public enum Directions
        {
            Up,
            Right,
            Left,
            Bottom,
            Center,
            None
        }
        /// <summary>
        /// 保证指令输入框保持在视野内
        /// </summary>
        public void NavigateByInstructionInputDialog()
        {
            double scaleX = GlobalSetting.LadderScaleTransform.ScaleX;
            double scaleY = GlobalSetting.LadderScaleTransform.ScaleY;
            if (SelectRectOwner == null) return;
            double pointX = (LeftBorder + _selectRect.X * WidthUnit) * scaleX - MainScrollViewer.HorizontalOffset;
            double pointY = (TopBorder + SelectRectOwner.UnitBaseTop + _selectRect.Y * HeightUnit) * scaleY - MainScrollViewer.VerticalOffset;
            if (pointX < 0)
                MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + pointX);
            if (pointX + 2 * _selectRect.ActualWidth * scaleX > MainScrollViewer.ViewportWidth)
                MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + pointX + 2 * _selectRect.ActualWidth * scaleX - MainScrollViewer.ViewportWidth);
            if (pointY < 0)
                MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + pointY);
            if (pointY + _selectRect.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + pointY + _selectRect.ActualHeight * scaleY - MainScrollViewer.ViewportHeight);
            if (_selectRect.X == GlobalSetting.LadderXCapacity - 1)
                MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + MainScrollViewer.ScrollableWidth);
            if (_selectRect.X == 0)
                MainScrollViewer.ScrollToHorizontalOffset(0);
        }
        
        private bool IsSelectRectOutOfViewpoint(Directions dir)
        {
            double scalex = GlobalSetting.LadderScaleTransform.ScaleX;
            double scaley = GlobalSetting.LadderScaleTransform.ScaleY;
            if (SelectRectOwner == null) return false;
            double rectx = scalex * (LeftBorder + WidthUnit * _selectRect.X);
            double recty = scaley * (TopBorder + SelectRectOwner.UnitBaseTop + HeightUnit * _selectRect.Y);
            Point point = _selectRect.TranslatePoint(new Point(0, 0), Scroll);
            switch (dir)
            {
                case Directions.Left: return rectx - Scroll.HorizontalOffset < 0;
                case Directions.Right: return rectx + _selectRect.ActualWidth * scalex - Scroll.HorizontalOffset > Scroll.ViewportWidth;
                case Directions.Up: return recty - Scroll.VerticalOffset < 0;
                case Directions.Bottom: return recty + _selectRect.ActualHeight * scaley - Scroll.VerticalOffset > Scroll.ViewportHeight;
                case Directions.Center:
                    return IsSelectRectOutOfViewpoint(Directions.Left)
                        && IsSelectRectOutOfViewpoint(Directions.Right)
                        && IsSelectRectOutOfViewpoint(Directions.Up)
                        && IsSelectRectOutOfViewpoint(Directions.Bottom);
                default: return false;
            }
        }

        private bool IsSelectAreaOutOfViewpoint(Directions dir)
        {
            double scalex = GlobalSetting.LadderScaleTransform.ScaleX;
            double scaley = GlobalSetting.LadderScaleTransform.ScaleY;
            double areax1 = 0.0;
            double areax2 = 0.0;
            double areay1 = 0.0;
            double areay2 = 0.0;
            switch (_selectArea.Core.State)
            {
                case SelectAreaCore.Status.SelectRange:
                    areax1 = scalex * (LeftBorder + WidthUnit * _selectArea.Core.XStart);
                    areay1 = scaley * (TopBorder + SelectStartNetwork.UnitBaseTop + HeightUnit * _selectArea.Core.YStart);
                    areax2 = areax1 + scalex * (_selectArea.Core.XEnd - _selectArea.Core.XStart + 1) * WidthUnit;
                    areay2 = areay1 + scaley * (_selectArea.Core.YEnd - _selectArea.Core.YStart + 1) * HeightUnit;
                    break;
                case SelectAreaCore.Status.SelectCross:
                    areax1 = scalex * LeftBorder;
                    areax2 = areax1 + scalex * GlobalSetting.LadderXCapacity;
                    areay1 = scaley * core.Children[_selectArea.Core.NetStart].UnitBaseTop;
                    areay2 = scaley * (core.Children[_selectArea.Core.NetEnd].UnitBaseTop + core.Children[_selectArea.Core.NetEnd].ViewHeight);
                    break;
                default:
                    return false;
            }
            switch (dir)
            {
                case Directions.Left: return areax1 - Scroll.HorizontalOffset < 0;
                case Directions.Right: return areax2 - Scroll.HorizontalOffset > Scroll.ViewportWidth;
                case Directions.Up: return areay1 - Scroll.VerticalOffset < 0;
                case Directions.Bottom: return areay2 - Scroll.VerticalOffset > Scroll.ViewportHeight;
                case Directions.Center:
                    return IsSelectAreaOutOfViewpoint(Directions.Left)
                        && IsSelectAreaOutOfViewpoint(Directions.Right)
                        && IsSelectAreaOutOfViewpoint(Directions.Up)
                        && IsSelectAreaOutOfViewpoint(Directions.Bottom);
                default: return false;
            }
        }

        /// <summary>
        /// 精确定位到当前光标
        /// </summary>
        /// <param name="hdir">横向停靠方向</param>
        /// <param name="vdir">纵向停靠方向</param>
        public void NavigateToSelectRect(Directions hdir = Directions.Center, Directions vdir = Directions.Center)
        {
            if (vdir != Directions.None) VScrollToRect(SelectRectOwner.ID, _selectRect.Y, vdir);
            if (hdir != Directions.None) HScrollToRect(_selectRect.X, hdir);
        }
        /// <summary>
        /// 精确定位到当前选择区域
        /// </summary>
        /// <param name="hdir">横向停靠方向</param>
        /// <param name="vdir">纵向停靠方向</param>
        public void NavigateToSelectArea(Directions hdir = Directions.Center, Directions vdir = Directions.Center)
        {
            switch (_selectArea.Core.State)
            {
                case SelectAreaCore.Status.SelectCross:
                    switch (vdir)
                    {
                        case Directions.Up:
                        case Directions.Center: VScrollToNetwork(_selectArea.Core.NetStart, Directions.Up); break;
                        case Directions.Bottom:
                            double scale = GlobalSetting.LadderScaleTransform.ScaleY;
                            LadderNetworkModel net = core.Children[_selectArea.Core.NetEnd];
                            if (scale * net.ViewHeight < Scroll.ViewportHeight)
                                VScrollToNetwork(_selectArea.Core.NetEnd, Directions.Bottom);
                            else
                                VScrollToNetwork(_selectArea.Core.NetEnd, Directions.Up);
                            break;
                    }
                    switch (hdir)
                    {
                        case Directions.Left:
                        case Directions.Center: HScrollToRect(0, Directions.Left); break;
                        case Directions.Right: HScrollToRect(GlobalSetting.LadderXCapacity - 1, Directions.Right); break;
                    }
                    break;
                case SelectAreaCore.Status.SelectRange:
                    switch (vdir)
                    {
                        case Directions.Up: VScrollToRect(_selectArea.Core.NetOrigin, _selectArea.Core.YStart, Directions.Up); break;
                        case Directions.Center: VScrollToRect(_selectArea.Core.NetOrigin, (_selectArea.Core.YStart + _selectArea.Core.YEnd) / 2, Directions.Center); break;
                        case Directions.Bottom: VScrollToRect(_selectArea.Core.NetOrigin, _selectArea.Core.YEnd, Directions.Bottom); break;
                    }
                    switch (hdir)
                    {
                        case Directions.Left: HScrollToRect(_selectArea.Core.XStart, Directions.Left); break;
                        case Directions.Center: HScrollToRect((_selectArea.Core.XStart + _selectArea.Core.XEnd) / 2, Directions.Center); break;
                        case Directions.Right: HScrollToRect(_selectArea.Core.XEnd, Directions.Right); break;
                    }
                    break;
            }
        }
        /// <summary>
        /// 纵向精确定位到指定网络
        /// </summary>
        /// <param name="net">网络号</param>
        /// <param name="row">行号</param>
        /// <param name="dir">停靠方向</param>
        public void VScrollToNetwork(int net, Directions dir = Directions.Up)
        {
            LadderNetworkModel lnmodel = core.Children[net]; 
            double scale = GlobalSetting.LadderScaleTransform.ScaleY;
            double offset = scale * (TopBorder + lnmodel.CanvasTop);
            switch (dir)
            {
                case Directions.Center: offset -= (Scroll.ViewportHeight - scale * lnmodel.ViewHeight) / 2; break;
                case Directions.Bottom: offset -= Scroll.ViewportHeight - scale * lnmodel.ViewHeight; break;
            }
            MainScrollViewer.ScrollToVerticalOffset(offset);
        }
        /// <summary>
        /// 纵向精确定位到指定网络的行
        /// </summary>
        /// <param name="net">网络号</param>
        /// <param name="row">行号</param>
        /// <param name="dir">停靠方向</param>
        public void VScrollToRect(int net, int row, Directions dir = Directions.Center)
        {
            double scale = GlobalSetting.LadderScaleTransform.ScaleY;
            double offset = scale * (TopBorder + core.Children[net].UnitBaseTop);
            offset += scale * HeightUnit * row;
            switch (dir)
            {
                case Directions.Center: offset -= (Scroll.ViewportHeight - scale * HeightUnit) / 2; break;
                case Directions.Bottom: offset -= Scroll.ViewportHeight - scale * HeightUnit; break;
            }
            MainScrollViewer.ScrollToVerticalOffset(offset);
        }
        /// <summary>
        /// 横向精确定位到指定网络的列
        /// </summary>
        /// <param name="col">列号</param>
        /// <param name="dir">停靠方向</param>
        public void HScrollToRect(int col, Directions dir = Directions.Center)
        {
            double scale = GlobalSetting.LadderScaleTransform.ScaleX;
            double offset = scale * LeftBorder;
            offset += scale * WidthUnit * col;
            MainScrollViewer.ScrollToHorizontalOffset(offset);
        }

        public void Select(LadderNetworkModel network, int x1, int x2, int y1, int y2)
        {
            _selectRect.Core.Parent = null;
            _selectArea.Core.Select(network.ID, x1, y1, x2, y2);
            _selectStatus = SelectStatus.MultiSelected;
            NavigateToSelectArea();
        }

        public void Select(int start, int end)
        {
            _selectRect.Core.Parent = null;
            _selectArea.Core.Select(start, end);
            _selectStatus = SelectStatus.MultiSelected;
            NavigateToSelectArea();
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
            /*
            foreach (LadderNetworkModel net in Core.Children)
            {
                if (net.View != null) stackpanel.Children.Add(net.View);
            }
            */
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

        private int loadedrowstart;
        public int LoadedRowStart { get { return this.loadedrowstart; } }

        private int loadedrowend;
        public int LoadedRowEnd { get { return this.loadedrowend; } }

        private double oldscrolloffset;
        private double newscrolloffset;
        public void DynamicUpdate()
        {
            double scaleY = 0;
            double scrollheight = 0;
            double titleheight = 0;
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                scaleY = GlobalSetting.LadderScaleTransform.ScaleY;
                newscrolloffset = Scroll.VerticalOffset;
                scrollheight = Scroll.ViewportHeight;
                titleheight = TopBorder;
            });
            if (!IsExpand)
            {
                if (loadedrowstart <= loadedrowend)
                {
                    DisposeRange(loadedrowstart, loadedrowend);
                    loadedrowstart = 0;
                    loadedrowend = -1;
                }
            }
            else
            {
                int _loadedrowstart = 0;
                int _loadedrowend = core.NetworkCount - 1;
                LadderNetworkModel net = core.Children[_loadedrowstart];
                while (net != null && (net.CanvasTop + net.ViewHeight) * scaleY - newscrolloffset < -20)
                {
                    _loadedrowstart++;
                    net = _loadedrowstart < core.NetworkCount ? core.Children[_loadedrowstart] : null;
                }
                net = core.Children[_loadedrowend];
                while (net != null && net.CanvasTop * scaleY - newscrolloffset > scrollheight + 20)
                {
                    _loadedrowend--;
                    net = _loadedrowend >= 0 ? core.Children[_loadedrowend] : null;
                }
                if (_loadedrowstart > _loadedrowend)
                {
                    if (loadedrowstart <= loadedrowend)
                    {
                        if (newscrolloffset > oldscrolloffset)
                            DisposeRange(loadedrowstart, loadedrowend);
                        else
                            DisposeRange(loadedrowend, loadedrowstart);
                    }
                }
                else if (loadedrowstart > loadedrowend)
                {
                    if (newscrolloffset > oldscrolloffset)
                        CreateRange(_loadedrowstart, _loadedrowend);
                    else
                        CreateRange(_loadedrowend, _loadedrowstart);
                }
                else
                {
                    if (newscrolloffset > oldscrolloffset)
                    {
                        if (_loadedrowstart > loadedrowstart)
                            DisposeRange(loadedrowstart, _loadedrowstart - 1);
                        if (loadedrowend > _loadedrowend)
                            DisposeRange(_loadedrowend + 1, loadedrowend);
                        //Thread.Sleep(10);
                        //if (Scroll.VerticalOffset == newscrolloffset)
                        if (_loadedrowstart < loadedrowstart)
                            CreateRange(_loadedrowstart, Math.Min(_loadedrowend, loadedrowstart - 1));
                        //if (Scroll.VerticalOffset == newscrolloffset)
                        if (loadedrowend < _loadedrowend)
                            CreateRange(Math.Max(_loadedrowstart, loadedrowend + 1), _loadedrowend);
                        //if (Scroll.VerticalOffset == newscrolloffset)
                        if (!(_loadedrowstart > loadedrowend) && !(_loadedrowend < loadedrowstart))
                            CreateRange(_loadedrowstart, _loadedrowend);
                    }
                    else
                    {
                        if (_loadedrowstart > loadedrowstart)
                            DisposeRange(_loadedrowstart - 1, loadedrowstart);
                        if (loadedrowend > _loadedrowend)
                            DisposeRange(loadedrowend, _loadedrowend + 1);
                        //Thread.Sleep(10);
                        //if (Scroll.VerticalOffset == newscrolloffset)
                        if (_loadedrowstart < loadedrowstart)
                            CreateRange(Math.Min(_loadedrowend, loadedrowstart - 1), _loadedrowstart);
                        //if (Scroll.VerticalOffset == newscrolloffset)
                        if (loadedrowend < _loadedrowend)
                            CreateRange(_loadedrowend, Math.Max(_loadedrowstart, loadedrowend + 1));
                        //if (Scroll.VerticalOffset == newscrolloffset)
                        if (!(_loadedrowstart > loadedrowend) && !(_loadedrowend < loadedrowstart))
                            CreateRange(_loadedrowend, _loadedrowstart);
                    }
                }
                loadedrowstart = _loadedrowstart;
                loadedrowend = _loadedrowend;
            }
            oldscrolloffset = newscrolloffset;
            foreach (LadderNetworkModel net in core.Children)
                if (net.View != null) net.View.DynamicUpdate();
        }

        public void DynamicDispose()
        {
            if (loadedrowstart <= loadedrowend)
            {
                DisposeRange(loadedrowstart, loadedrowend);
                loadedrowstart = 0;
                loadedrowend = -1;
            }
        }

        private void DisposeRange(int rowstart, int rowend)
        {
            int dir = (rowstart < rowend ? 1 : -1);
            for (int y = rowstart; y != rowend + dir; y += dir)
            {
                if (y >= core.Children.Count) continue;
                LadderNetworkModel net = core.Children[y];
                if (net.View != null)
                {
                    net.View.DynamicDispose();
                    Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        net.View.Visibility = Visibility.Hidden;
                        net.View.Dispose();
                    });
                }
            }
        }

        private void CreateRange(int rowstart, int rowend)
        {
            int dir = (rowstart < rowend ? 1 : -1);
            for (int y = rowstart; y != rowend + dir; y += dir)
            {
                //if (Scroll.VerticalOffset != newscrolloffset) return;
                LadderNetworkModel net = core.Children[y];
                if (net.View == null)
                {
                    Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        (ThreadStart)delegate ()
                    {
                        net.View = AllResourceManager.CreateNet(net);
                        net.View.Visibility = Visibility.Visible;
                        if (net.View.Parent != MainCanvas)
                        {
                            if (net.View.Parent is Canvas)
                                ((Canvas)(net.View.Parent)).Children.Remove(net.View);
                            MainCanvas.Children.Add(net.View);
                        }
                    });
                }
            }
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
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("IsExpand"));
        }

        public InstructionDiagramViewModel Inst
        {
            get { return core != null && core.Inst != null ? core.Inst.View : null; }
        }
        
        public LadderModes LadderMode { get { return core.LadderMode; } }
        public bool IsCommentMode { get { return core.IsCommentMode; } }
        
        private int WidthUnit { get { return GlobalSetting.LadderWidthUnit; } }
        private int HeightUnit { get { return IsCommentMode ? GlobalSetting.LadderCommentModeHeightUnit : GlobalSetting.LadderHeightUnit; } }
        public double LeftBorder { get { return LeftStackPanel.ActualWidth; } }
        public double TopBorder { get { return TitleStackPanel.ActualHeight; } }

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

        private bool ispressingctrl = false;
        private bool ispressingshift = false;
        private void OnLadderDiagramKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                ispressingctrl = false;
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                ispressingshift = false;
            if (_selectStatus == SelectStatus.MultiSelecting)
                _selectStatus = SelectStatus.MultiSelected;
        }
        private void OnLadderDiagramKeyDown(object sender, KeyEventArgs e)
        {
            if (IFParent.IsWaitForKey) return;
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                ispressingctrl = true;
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                ispressingshift = true;
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
                    isnavigatable = false;
                    IFParent.ShowInstructionInputDialog(s, _selectRect.Core);
                    isnavigatable = true;
                }
            }
            if (LadderMode == LadderModes.Edit)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    isnavigatable = false;
                    switch (e.Key)
                    {
                        case Key.OemPlus: IFParent.ShowInstructionInputDialog("ADD", _selectRect.Core); break;
                        case Key.OemMinus: IFParent.ShowInstructionInputDialog("SUB", _selectRect.Core); break;
                        case Key.D8: IFParent.ShowInstructionInputDialog("MUL", _selectRect.Core); break;
                        case Key.Oem5: IFParent.ShowInstructionInputDialog("DIV", _selectRect.Core); break;

                        case Key.F2: IFParent.ShowInstructionInputDialog("LDIM ", _selectRect.Core); break;
                        case Key.F3: IFParent.ShowInstructionInputDialog("LDIIM ", _selectRect.Core); break;
                        case Key.F5: QuickInsertElement(LadderUnitModel.Types.MEP); break;
                        case Key.F6: QuickInsertElement(LadderUnitModel.Types.MEF); break;
                        case Key.F8: IFParent.ShowInstructionInputDialog("OUTIM ", _selectRect.Core); break;
                        case Key.F9: QuickRemoveElement(LadderUnitModel.Types.HLINE); break;
                        case Key.F10: QuickRemoveElement(LadderUnitModel.Types.VLINE); break;
                    }
                    isnavigatable = true;
                }
                else if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.None)
                {
                    isnavigatable = false;
                    switch (e.Key)
                    {
                        case Key.F2: IFParent.ShowInstructionInputDialog("LD ", _selectRect.Core); break;
                        case Key.F3: IFParent.ShowInstructionInputDialog("LDI ", _selectRect.Core); break;
                        case Key.F5: IFParent.ShowInstructionInputDialog("LDP ", _selectRect.Core); break;
                        case Key.F6: IFParent.ShowInstructionInputDialog("LDF ", _selectRect.Core); break;
                        case Key.F7: QuickInsertElement(LadderUnitModel.Types.INV); break;
                        case Key.F8: IFParent.ShowInstructionInputDialog("OUT ", _selectRect.Core); break;
                        case Key.F9: QuickInsertElement(LadderUnitModel.Types.HLINE); break;
                        case Key.F10: QuickInsertElement(LadderUnitModel.Types.VLINE); break;
                    }
                    isnavigatable = true;
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
                isnavigatable = false;
                if (_selectRect.Current != null && _selectRect.Current.Type != LadderUnitModel.Types.HLINE)
                    IFParent.ShowElementPropertyDialog(_selectRect.Current);
                else
                    IFParent.ShowInstructionInputDialog(string.Empty, _selectRect.Core);
                isnavigatable = true;
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
                        ReleaseSelect();
                        Core.ReplaceN(removes, new LadderNetworkModel[] { });
                    }
                    else if (_selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                    {
                        Core.RemoveU(SelectStartNetwork, _selectArea.Core.SelectUnits);
                        ReleaseSelect();
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
                        ReleaseSelect();
                        if (cut) Core.ReplaceN(removes, new LadderNetworkModel[] { });
                        Clipboard.SetData("LadderContent", xele.ToString());
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
                        ReleaseSelect();
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
                            : _selectArea.Core.SelectNetworks;
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
            if (core.IsExecuting) return;
            Core.Undo();
        }
        private void OnRedoCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (core.IsExecuting) return;
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
                            _selectArea.Core.Select(SelectRectOwner.ID, _selectRect.X, _selectRect.Y, _selectRect.X, _selectRect.Y);
                            _selectArea.Core.Select(SelectRectOwner.ID, net.ID);
                        }
                        else if (_selectArea.Core.State != SelectAreaCore.Status.SelectCross)
                        {
                            _selectArea.Core.Select(SelectRectOwner.ID, _selectRect.X, _selectRect.Y, _selectRect.X, _selectRect.Y);
                            _selectArea.Core.Select(SelectRectOwner.ID, SelectRectOwner.ID);
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
                        else if (_selectArea.Core.State != SelectAreaCore.Status.SelectCross)
                        {
                            _selectArea.Core.Select(SelectStartNetwork.ID, SelectStartNetwork.ID);
                        }
                        if (_selectArea.Core.State == SelectAreaCore.Status.SelectRange
                         && _selectArea.Core.XStart == _selectArea.Core.XEnd
                         && _selectArea.Core.YStart == _selectArea.Core.YEnd)
                        {
                            _selectRect.Core.Parent = SelectStartNetwork;
                            _selectRect.Core.X = _selectArea.Core.XStart;
                            _selectRect.Core.Y = _selectArea.Core.YStart;
                            _selectArea.Core.Release();
                            _selectStatus = SelectStatus.SingleSelecting;
                        }
                        break;
                }
                var p1 = e.GetPosition(MainScrollViewer);
                if (MainScrollViewer.ViewportHeight < p1.Y)
                    MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + (p1.Y - MainScrollViewer.ViewportHeight) * GlobalSetting.LadderScaleY * 0.2);
                else if (p1.Y < 0)
                    MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + p1.Y * GlobalSetting.LadderScaleY * 0.2);
                else if (p1.X < 0)
                    MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + p1.X * GlobalSetting.LadderScaleX * 0.8);
                else if (MainScrollViewer.ViewportWidth < p1.X)
                    MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + (p1.X - MainScrollViewer.ViewportWidth) * GlobalSetting.LadderScaleX * 0.8);
            }
        }
        
        private void OnMainCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            CanvasGrid.ContextMenu = LadderMode == LadderModes.Edit
                ? (ContextMenu)cmEdit : (ContextMenu)cmMoni;
            CanvasGrid.CaptureMouse();
            object obj = GetObjectByMouse(e);
            if (obj is LadderUnitModel)
            {
                LadderUnitModel unit = (LadderUnitModel)obj;
                cmEdit.Core = unit.Parent;
                if (unit.IsUsed && (LadderMode != LadderModes.Edit || e.ChangedButton == MouseButton.Left))
                {
                    cmMoni.Core = unit;
                    if (!ispressingshift || _selectStatus == SelectStatus.Idle)
                    {
                        _selectArea.Core.Release();
                        _selectRect.Core.Current = unit;
                        _selectStatus = SelectStatus.SingleSelecting;
                    }
                    else
                    {
                        switch (_selectStatus)
                        {
                            case SelectStatus.SingleSelecting:
                            case SelectStatus.SingleSelected:
                                if (unit != _selectRect.Core.Current)
                                {
                                    if (unit.Parent == SelectRectOwner)
                                    {
                                        _selectArea.Core.Select(SelectRectOwner.ID,
                                            _selectRect.X, _selectRect.Y, unit.X, unit.Y);
                                    }
                                    else
                                    {
                                        _selectArea.Core.Select(SelectRectOwner.ID,
                                            _selectRect.X, _selectRect.Y, _selectRect.X, _selectRect.Y);
                                        _selectArea.Core.Select(SelectRectOwner.ID, unit.Parent.ID);
                                    }
                                    _selectRect.Core.Parent = null;
                                    _selectStatus = SelectStatus.MultiSelecting;
                                }
                                break;
                            case SelectStatus.MultiSelecting:
                            case SelectStatus.MultiSelected:
                                _selectArea.Core.Move(unit);
                                if (_selectArea.Core.State == SelectAreaCore.Status.SelectRange
                                 && _selectArea.Core.XStart == _selectArea.Core.XEnd
                                 && _selectArea.Core.YStart == _selectArea.Core.YEnd)
                                {

                                    _selectRect.Core.Parent = core.Children[_selectArea.Core.NetOrigin];
                                    _selectRect.Core.X = _selectArea.Core.XStart;
                                    _selectRect.Core.Y = _selectArea.Core.YStart;
                                    _selectArea.Core.Release();
                                    _selectStatus = SelectStatus.SingleSelecting;
                                }
                                break;
                        }
                    }
                }
            }
            else if (obj is LadderNetworkPositionModel)
            {
                LadderNetworkPositionModel pos = (LadderNetworkPositionModel)obj;
                cmEdit.Core = pos.Network;
                cmMoni.Core = null;
                if (!pos.Network.IsMasked && (LadderMode != LadderModes.Edit || e.ChangedButton == MouseButton.Left))
                {
                    if (!ispressingshift || _selectStatus == SelectStatus.Idle)
                    {
                        _selectArea.Core.Release();
                        _selectRect.Core.Parent = pos.Network;
                        _selectRect.Core.X = pos.X;
                        _selectRect.Core.Y = pos.Y;
                        _selectStatus = SelectStatus.SingleSelecting;
                    }
                    else
                    {
                        switch (_selectStatus)
                        {
                            case SelectStatus.SingleSelecting:
                            case SelectStatus.SingleSelected:
                                if (pos.Network != SelectRectOwner || pos.X != _selectRect.X || pos.Y != _selectRect.Y)
                                {
                                    if (pos.Network == SelectRectOwner)
                                    {
                                        _selectArea.Core.Select(SelectRectOwner.ID,
                                            _selectRect.X, _selectRect.Y, pos.X, pos.Y);
                                    }
                                    else
                                    {
                                        _selectArea.Core.Select(SelectRectOwner.ID,
                                            _selectRect.X, _selectRect.Y, _selectRect.X, _selectRect.Y);
                                        _selectArea.Core.Select(SelectRectOwner.ID, pos.Network.ID);
                                    }
                                    _selectRect.Core.Parent = null;
                                    _selectStatus = SelectStatus.MultiSelecting;
                                }
                                break;
                            case SelectStatus.MultiSelecting:
                            case SelectStatus.MultiSelected:
                                _selectArea.Core.Move(pos);
                                if (_selectArea.Core.State == SelectAreaCore.Status.SelectRange
                                 && _selectArea.Core.XStart == _selectArea.Core.XEnd
                                 && _selectArea.Core.YStart == _selectArea.Core.YEnd)
                                {
                                    _selectRect.Core.Parent = core.Children[_selectArea.Core.NetOrigin];
                                    _selectRect.Core.X = _selectArea.Core.XStart;
                                    _selectRect.Core.Y = _selectArea.Core.YStart;
                                    _selectArea.Core.Release();
                                    _selectStatus = SelectStatus.SingleSelecting;
                                }

                                break;
                        }
                    }
                }
            }
            else if (obj is LadderNetworkModel)
            {
                LadderNetworkModel net = (LadderNetworkModel)obj;
                cmMoni.Core = null;
                cmEdit.Core = net;
                if (LadderMode != LadderModes.Edit || e.ChangedButton == MouseButton.Left)
                    ReleaseSelect();
            }
            else
            {
                ReleaseSelect();
                cmMoni.Core = null;
                cmEdit.Core = null;
                MainCanvas.ContextMenu = null;
            }
            if (MainCanvas.ContextMenu == cmMoni && cmMoni.Core == null)
                MainCanvas.ContextMenu = null;
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
            CanvasGrid.ReleaseMouseCapture();
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
        
        private void OnMainCanvasDrop(object sender, DragEventArgs e)
        {
            switch (_selectStatus)
            {
                case SelectStatus.NetworkDraging:
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
                    break;
                case SelectStatus.UnitDraging:
                    ProjectTreeViewItem ptvitem = new ProjectTreeViewItem(null);
                    if (_selectRect.Core.Parent == null) return;
                    object obj = GetObjectByMouse(e);
                    LadderNetworkModel net;
                    if (obj is LadderUnitModel) net = ((LadderUnitModel)obj).Parent;
                    else if(obj is LadderNetworkModel) net = (LadderNetworkModel)obj;
                    else net = ((LadderNetworkPositionModel)obj).Network;
                    if (net == null || net.IsMasked) return;
                    if (e.Data.GetDataPresent(ptvitem.GetType()))
                    {
                        ptvitem = (ProjectTreeViewItem)(e.Data.GetData(ptvitem.GetType()));
                        if (ptvitem.RelativeObject is LadderUnitModel.Types)
                        {
                            LadderUnitModel.Types type = (LadderUnitModel.Types)(ptvitem.RelativeObject);
                            //Core.Parent.QuickInsertElement(type, ViewParent.SelectionRect.Core, false);
                            if (IFParent.ShowElementPropertyDialog(type, _selectRect.Core, false))
                                SelectRectRight();
                        }
                        else if (ptvitem.RelativeObject is FuncModel)
                        {
                            FuncModel fmodel = (FuncModel)(ptvitem.RelativeObject);
                            if (!fmodel.CanCALLM())
                            {
                                LocalizedMessageBox.Show(String.Format("{0:s}{1}", fmodel.Name, Properties.Resources.Message_Can_Not_CALL), LocalizedMessageIcon.Error);
                                return;
                            }
                            IFParent.ShowElementPropertyDialog(fmodel, _selectRect.Core);
                        }
                        else if (ptvitem.RelativeObject is LadderDiagramModel)
                        {
                            LadderDiagramModel ldmodel = (LadderDiagramModel)(ptvitem.RelativeObject);
                            LadderUnitModel unit = new LadderUnitModel(null, LadderUnitModel.Types.CALL);
                            unit.InstArgs = new string[] { ldmodel.Name };
                            unit.X = _selectRect.X;
                            unit.Y = _selectRect.Y;
                            Core.AddSingleUnit(unit, SelectRectOwner, false);
                        }
                        else if (ptvitem.RelativeObject is ModbusModel)
                        {
                            ModbusModel mmodel = (ModbusModel)(ptvitem.RelativeObject);
                            if (!mmodel.IsValid)
                            {
                                LocalizedMessageBox.Show(String.Format("{0:s}{1}", mmodel.Name, Properties.Resources.Message_Modbus_Table_Error), LocalizedMessageIcon.Error);
                                return;
                            }
                            IFParent.ShowElementPropertyDialog(mmodel, _selectRect.Core);
                        }
                    }
                    break;
            }
        }

        private void OnMainCanvasDragOver(object sender, DragEventArgs e)
        {
            switch (_selectStatus)
            {
                case SelectStatus.NetworkDraging:
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
                    break;
                default:
                    _selectStatus = SelectStatus.UnitDraging;
                    ProjectTreeViewItem ptvitem = new ProjectTreeViewItem(null);
                    if (e.Data.GetDataPresent(ptvitem.GetType()))
                    {
                        ptvitem = (ProjectTreeViewItem)(e.Data.GetData(ptvitem.GetType()));
                        if (ptvitem.RelativeObject is LadderUnitModel.Types
                         || ptvitem.RelativeObject is FuncModel
                         || ptvitem.RelativeObject is LadderDiagramModel
                         || ptvitem.RelativeObject is ModbusModel)
                        {
                            object obj = GetObjectByMouse(e);
                            if (obj is LadderUnitModel)
                            {
                                LadderUnitModel unit = (LadderUnitModel)obj;
                                if (unit.Parent.IsMasked) return;
                                _selectArea.Core.Release();
                                _selectRect.Core.Parent = unit.Parent;
                                _selectRect.X = unit.X;
                                _selectRect.Y = unit.Y;
                            }
                            else if (obj is LadderNetworkPositionModel)
                            {
                                LadderNetworkPositionModel pos = (LadderNetworkPositionModel)obj;
                                if (pos.Network.IsMasked) return;
                                _selectArea.Core.Release();
                                _selectRect.Core.Parent = pos.Network;
                                _selectRect.X = pos.X;
                                _selectRect.Y = pos.Y;
                            }
                        }
                    }
                    double scaleX = GlobalSetting.LadderScaleTransform.ScaleX;
                    double scaleY = GlobalSetting.LadderScaleTransform.ScaleY;
                    var p1 = e.GetPosition(MainScrollViewer);
                    if (MainScrollViewer.ViewportHeight < p1.Y)
                        MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + (p1.Y - MainScrollViewer.ViewportHeight) * GlobalSetting.LadderScaleY * 0.2);
                    else if (p1.Y < 0)
                        MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset + p1.Y * GlobalSetting.LadderScaleY * 0.2);
                    else if (p1.X < 0)
                        MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + p1.X * GlobalSetting.LadderScaleX * 0.8);
                    else if (MainScrollViewer.ViewportWidth < p1.X)
                        MainScrollViewer.ScrollToHorizontalOffset(MainScrollViewer.HorizontalOffset + (p1.X - MainScrollViewer.ViewportWidth) * GlobalSetting.LadderScaleX * 0.8);
                    break;
            }
        }

        private void OnMainCanvasDragLeave(object sender, DragEventArgs e)
        {
            switch (_selectStatus)
            {
                case SelectStatus.NetworkDraging:
                    ((LadderNetworkViewModel)e.Source).Opacity = 1;
                    if (dragItem == null) return;
                    dragItem.CommentAreaBorder.BorderBrush = Brushes.Brown;
                    dragItem.CommentAreaBorder.BorderThickness = new Thickness(4);
                    break;
            }
        }

        private object GetObjectByMouse(MouseEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);
            return GetObjectByMouse(p);
        }

        private object GetObjectByMouse(DragEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);
            return GetObjectByMouse(p);
        }

        private object GetObjectByMouse(Point p)
        {
            if (p.X < 0) return null;
            int x = (int)(p.X / WidthUnit);
            foreach (LadderNetworkModel net in Core.Children)
            {
                if (p.Y >= net.CanvasTop && p.Y < net.UnitBaseTop) return net;
                if (p.Y < net.UnitBaseTop || !net.IsExpand) continue;
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

        #region Ladder Edit

        private void OnLadderEdit(object sender, LadderEditEventArgs e)
        {
            if (core.IsExecuting) return;
            switch (e.Type)
            {
                case LadderEditEventArgs.Types.Delete:
                    Delete();
                    break;
                case LadderEditEventArgs.Types.RowInsertBefore:
                    if (_selectStatus == SelectStatus.SingleSelected)
                        Core.AddR(SelectRectOwner, _selectRect.Y);
                    else if (_selectStatus == SelectStatus.MultiSelected
                     && _selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                        Core.AddR(SelectStartNetwork, _selectArea.Core.YStart);
                    break;
                case LadderEditEventArgs.Types.RowInsertAfter:
                    if (_selectStatus == SelectStatus.SingleSelected)
                        Core.AddR(SelectRectOwner, _selectRect.Y + 1);
                    else if (_selectStatus == SelectStatus.MultiSelected
                     && _selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                        Core.AddR(SelectStartNetwork, _selectArea.Core.YEnd + 1);
                    break;
                case LadderEditEventArgs.Types.RowInsertEnd:
                    if (_selectStatus == SelectStatus.SingleSelected)
                        Core.AddR(SelectRectOwner, SelectRectOwner.RowCount);
                    else if (_selectStatus == SelectStatus.MultiSelected)
                        Core.AddR(_selectArea.NetEnd, _selectArea.NetEnd.RowCount);
                    break;
                case LadderEditEventArgs.Types.RowDelete:
                    if (_selectStatus == SelectStatus.SingleSelected)
                        Core.RemoveR(SelectRectOwner, _selectRect.Y);
                    else if (_selectStatus == SelectStatus.MultiSelected
                     && _selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                        Core.RemoveR(SelectStartNetwork, _selectArea.Core.YStart, _selectArea.Core.YEnd);
                    else
                        Delete();
                    break;
                case LadderEditEventArgs.Types.NetInsertBefore:
                    Core.AddN(cmEdit.Core.ID);
                    break;
                case LadderEditEventArgs.Types.NetInsertAfter:
                    Core.AddN(cmEdit.Core.ID + 1);
                    break;
                case LadderEditEventArgs.Types.NetInsertEnd:
                    Core.AddN(Core.NetworkCount);
                    break;
                case LadderEditEventArgs.Types.NetDelete:
                    if (_selectStatus == SelectStatus.MultiSelected
                     && _selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                        Core.RemoveN(SelectStartNetwork.ID, SelectStartNetwork);
                    else if (cmEdit.Core != null)
                        Core.RemoveN(cmEdit.Core.ID, cmEdit.Core);
                    break;
                case LadderEditEventArgs.Types.NetCopy:
                    if (_selectStatus == SelectStatus.MultiSelected
                     && _selectArea.Core.State == SelectAreaCore.Status.SelectCross)
                        CutAndCopy(false);
                    else if (cmEdit.Core != null)
                        cmEdit.Core.CopyToClipboard();
                    break;
                case LadderEditEventArgs.Types.NetCut:
                    if (_selectStatus == SelectStatus.MultiSelected
                     && _selectArea.Core.State == SelectAreaCore.Status.SelectCross)
                        CutAndCopy(true);
                    else if (cmEdit.Core != null)
                    {
                        cmEdit.Core.CopyToClipboard();
                        Core.RemoveN(cmEdit.Core.ID, cmEdit.Core);
                    }
                    break;
                case LadderEditEventArgs.Types.NetShield:
                    if (_selectStatus == SelectStatus.MultiSelected)
                    {
                        bool ismasked = !cmEdit.Core.IsMasked;
                        for (int i = _selectArea.Core.NetStart; i <= _selectArea.Core.NetEnd; i++)
                            core.Children[i].IsMasked = ismasked;
                    }
                    else if (cmEdit.Core != null)
                        cmEdit.Core.IsMasked = !cmEdit.Core.IsMasked;
                    break;
                case LadderEditEventArgs.Types.NetExpand:
                    if (_selectStatus == SelectStatus.MultiSelected)
                    {
                        for (int i = _selectArea.Core.NetStart; i <= _selectArea.Core.NetEnd; i++)
                            core.Children[i].IsExpand = true;
                        NavigateToSelectArea(Directions.None, Directions.Up);
                    }
                    else if (cmEdit.Core != null)
                        cmEdit.Core.IsExpand = true;
                    break;
                case LadderEditEventArgs.Types.NetExpandAll:
                    foreach (LadderNetworkModel net in core.Children)
                        net.IsExpand = true;
                    break;
                case LadderEditEventArgs.Types.NetCollapsed:
                    if (_selectStatus == SelectStatus.MultiSelected)
                    {
                        for (int i = _selectArea.Core.NetStart; i <= _selectArea.Core.NetEnd; i++)
                            core.Children[i].IsExpand = false;
                        NavigateToSelectArea(Directions.None, Directions.Up);
                    }
                    else if (cmEdit.Core != null)
                        cmEdit.Core.IsExpand = false;
                    break;
                case LadderEditEventArgs.Types.NetCollapsedAll:
                    foreach (LadderNetworkModel net in core.Children)
                        net.IsExpand = false;
                    break;
                case LadderEditEventArgs.Types.NetMerge:
                    if (_selectArea.Core.State == SelectAreaCore.Status.SelectCross
                     && SelectAllNetworks.Count() > 0)
                    {
                        core.MergeNetworks(SelectAllNetworks, SelectAllNetworks.Select(n => n.ID).Min());
                    }
                    break;
                case LadderEditEventArgs.Types.NetSplit:
                    if (_selectStatus == SelectStatus.SingleSelected)
                        core.SplitNetwork(SelectRectOwner, 0, _selectRect.Y);
                    else if (_selectStatus == SelectStatus.MultiSelected
                      && _selectArea.Core.State == SelectAreaCore.Status.SelectRange)
                    {
                        core.SplitNetwork(_selectArea.NetOrigin, _selectArea.Core.YStart, _selectArea.Core.YEnd);
                    }
                    break;
            }
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

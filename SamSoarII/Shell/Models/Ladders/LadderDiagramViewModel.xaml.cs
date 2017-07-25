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
        SingleSelected,
        MultiSelecting,
        MultiSelected,
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
            if (GlobalSetting.InstrutionNameAndToolTips == null)
                GlobalSetting.LoadInstrutionNameAndToolTips();
            InitializeComponent();
            DataContext = this;
            Core = _core;
            if (Core.Parent.View != null)
                IsCommentMode = Core.Parent.View.IsCommentMode;
            _selectRect = new SelectRect();
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
                    _core.ChildrenChanged -= OnCoreChildrenChanged;
                    if (_core.View != null) _core.View = null;
                }
                this.core = value;
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
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
                    LadderNetworkStackPanel.Children.Insert(e.NewStartingIndex, net.View);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    net = (LadderNetworkModel)(e.OldItems[0]);
                    if (SelectRectOwner == net) SelectRectOwner = null;
                    LadderNetworkStackPanel.Children.RemoveAt(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    net = (LadderNetworkModel)(e.NewItems[0]);
                    LadderNetworkStackPanel.Children[e.NewStartingIndex] = net.View;
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
                    else if (LadderUnitModel.Formats[(int)type].Length == 0)
                    {
                        Core.QuickInsertElement(type, _selectRect.Core);
                        SelectRectRight();
                    }
                    else
                    {
                        if(IFParent.ShowElementPropertyDialog(type, _selectRect.Core))
                            SelectRectRight();
                    }
                    isnavigatable = true;
                    break;
                case SelectStatus.MultiSelecting: break;
                case SelectStatus.MultiSelected:
                    LadderNetworkModel net = SelectStartNetwork.Core;    
                    int x = SelectStartNetwork.SelectAreaFirstX;
                    int y = SelectStartNetwork.SelectAreaFirstY;
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
                            IEnumerable<LadderUnitModel> hlines = SelectStartNetwork.GetSelectedHLines();
                            foreach (LadderNetworkViewModel netview in Core.Children.Select(c => c.View))
                                hlines = hlines.Concat(netview.GetSelectedHLines());
                            Core.RemoveU(SelectStartNetwork.Core, hlines);
                            break;
                        case LadderUnitModel.Types.VLINE:
                            IEnumerable<LadderUnitModel> vlines = SelectStartNetwork.GetSelectedVLines();
                            foreach (LadderNetworkViewModel netview in Core.Children.Select(c => c.View))
                                vlines = vlines.Concat(netview.GetSelectedVLines());
                            Core.RemoveU(SelectStartNetwork.Core, vlines);
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
                    if (SelectStartNetwork != null && SelectStartNetwork.IsSelectAreaMode)
                        Core.AddR(SelectStartNetwork.Core, 
                            Math.Max(SelectStartNetwork.SelectAreaFirstY, SelectStartNetwork.SelectAreaSecondY) + 1);
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
                    if (SelectStartNetwork != null && SelectStartNetwork.IsSelectAreaMode)
                        Core.RemoveR(SelectStartNetwork.Core, 
                            Math.Min(SelectStartNetwork.SelectAreaFirstY, SelectStartNetwork.SelectAreaSecondY), 
                            Math.Max(SelectStartNetwork.SelectAreaFirstY, SelectStartNetwork.SelectAreaSecondY));
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

        private LadderNetworkViewModel _selectStartNetwork;
        public LadderNetworkViewModel SelectStartNetwork
        {
            get { return _selectStartNetwork; }
        }

        private SortedSet<LadderNetworkViewModel> _selectAllNetworks
            = new SortedSet<LadderNetworkViewModel>();
        public SortedSet<LadderNetworkViewModel> SelectAllNetworks
        {
            get { return _selectAllNetworks; }
        }

        private SortedSet<LadderNetworkViewModel> _selectAllNetworkCache
            = new SortedSet<LadderNetworkViewModel>();
        public SortedSet<LadderNetworkViewModel> SelectAllNetworkCache
        {
            get { return _selectAllNetworkCache; }
        }
        
        private SelectStatus _selectStatus = SelectStatus.Idle;
        public SelectStatus SelectionStatus
        {
            get
            {
                return _selectStatus;
            }
            set
            {
                _selectStatus = value;
                switch (_selectStatus)
                {
                    case SelectStatus.Idle:
                        EnterIdleState();
                        break;
                    case SelectStatus.SingleSelected:
                        EnterSingleSelectedState();
                        break;
                    case SelectStatus.MultiSelecting:
                        EnterMultiSelectingState();
                        break;
                    case SelectStatus.MultiSelected:
                        EnterMultiSelectedState();
                        break;
                    default:
                        break;
                }
                SelectionChanged(this, new RoutedEventArgs());
            }
        }
        
        private CrossNetworkState _crossNetState;
        public CrossNetworkState CrossNetState
        {
            get { return this._crossNetState; }
            set { this._crossNetState = value; SelectionChanged(this, new RoutedEventArgs()); }
        }

        #region Selection area change

        private void SelectionAreaChanged(Key key)
        {
            if (_selectStatus == SelectStatus.SingleSelected)
            {
                if (SelectRectOwner != null)
                {
                    _selectStartNetwork = SelectRectOwner.View;
                }
                if (SingleSelectionAreaCanChange(key))
                {
                    ChangeSingleSelectionArea(key);
                }
            }
            else if (_selectStatus == SelectStatus.MultiSelected)
            {
                _selectStatus = SelectStatus.MultiSelecting;
                if (MutiSelectionAreaCanChange(key))
                {
                    ChangeMutiSelectionArea(key);
                }
            }
            else if (_selectStatus == SelectStatus.MultiSelecting)
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
                    return _selectStartNetwork.SelectAreaSecondX > 0;
                case Key.Right:
                    return _selectStartNetwork.SelectAreaSecondX < GlobalSetting.LadderXCapacity - 1;
                case Key.Up:
                    if (CrossNetState == CrossNetworkState.NoCross)
                    {
                        return _selectStartNetwork.Core.ID > 0 || _selectStartNetwork.SelectAreaSecondY > 0 || !_selectStartNetwork.IsSelectAllMode;
                    }
                    else if (CrossNetState == CrossNetworkState.CrossDown)
                    {
                        return true;
                    }
                    else
                    {
                        if (_selectAllNetworks.ToList().Exists(x => { return x.Core.ID == 0; }))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                case Key.Down:
                    if (CrossNetState == CrossNetworkState.NoCross)
                    {
                        return _selectStartNetwork.Core.ID < Core.NetworkCount - 1 || _selectStartNetwork.SelectAreaSecondY < _selectStartNetwork.RowCount - 1 || !_selectStartNetwork.IsSelectAllMode;
                    }
                    else if (CrossNetState == CrossNetworkState.CrossUp)
                    {
                        return true;
                    }
                    else
                    {
                        if (_selectAllNetworks.ToList().Exists(x => { return x.Core.ID == Core.NetworkCount - 1; }))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                default:
                    return false;
            }
        }
        private bool SingleSelectionAreaCanChange(Key key)
        {
            if (_selectStartNetwork != null && _selectStartNetwork.ladderExpander.IsExpand)
            {
                switch (key)
                {
                    case Key.Left:
                        return SelectionRect.X > 0;
                    case Key.Right:
                        return SelectionRect.X < GlobalSetting.LadderXCapacity - 1;
                    case Key.Up:
                        return SelectStartNetwork.Core.ID > 0 || SelectionRect.Y > 0 || !SelectStartNetwork.IsSelectAllMode;
                    case Key.Down:
                        return SelectStartNetwork.Core.ID < Core.NetworkCount - 1 || SelectionRect.Y < SelectStartNetwork.RowCount - 1 || !SelectStartNetwork.IsSelectAllMode;
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }
        private void ChangeMutiSelectionArea(Key key)
        {
            switch (key)
            {
                case Key.Left:
                    if (CrossNetState == CrossNetworkState.NoCross)
                    {
                        if (_selectStartNetwork.SelectAreaSecondX > 0)
                        {
                            ChangeViewport(BoundaryDirection.Left);
                            _selectStartNetwork.SelectAreaSecondX--;
                            _selectStartNetwork.SelectAreaOriginSX = _selectStartNetwork.SelectAreaSecondX;
                            //HScrollToRect(_selectStartNetwork.SelectAreaSecondX);
                            if (_selectStartNetwork.SelectAreaSecondX == _selectStartNetwork.SelectAreaFirstX && _selectStartNetwork.SelectArea.Height == GlobalSetting.LadderHeightUnit)
                            {
                                SelectionRect.X = _selectStartNetwork.SelectAreaFirstX;
                                SelectionRect.Y = _selectStartNetwork.SelectAreaFirstY;
                                _selectStartNetwork.AcquireSelectRect();
                            }
                        }
                    }
                    break;
                case Key.Right:
                    if (CrossNetState == CrossNetworkState.NoCross)
                    {
                        if (_selectStartNetwork.SelectAreaSecondX < GlobalSetting.LadderXCapacity - 1)
                        {
                            ChangeViewport(BoundaryDirection.Right);
                            _selectStartNetwork.SelectAreaSecondX++;
                            _selectStartNetwork.SelectAreaOriginSX = _selectStartNetwork.SelectAreaSecondX;
                            //HScrollToRect(_selectStartNetwork.SelectAreaSecondX);
                            if (_selectStartNetwork.SelectAreaSecondX == _selectStartNetwork.SelectAreaFirstX && _selectStartNetwork.SelectArea.Height == GlobalSetting.LadderHeightUnit)
                            {
                                SelectionRect.X = _selectStartNetwork.SelectAreaFirstX;
                                SelectionRect.Y = _selectStartNetwork.SelectAreaFirstY;
                                _selectStartNetwork.AcquireSelectRect();
                            }
                        }
                    }
                    break;
                case Key.Up:
                    ChangeViewport(BoundaryDirection.Up);
                    if (CrossNetState == CrossNetworkState.CrossUp)
                    {
                        CollectSelectAllNetworkUpByCount(_selectAllNetworks.Count + 1);
                        foreach (var net in _selectAllNetworkCache)
                        {
                            net.IsSelectAreaMode = false;
                            net.IsSelectAllMode = false;
                        }
                        foreach (var net in _selectAllNetworks)
                        {
                            net.IsSelectAllMode = true;
                        }
                    }
                    else if (CrossNetState == CrossNetworkState.CrossDown)
                    {
                        if (_selectAllNetworks.Count > 0)
                        {
                            CollectSelectAllNetworkDownByCount(_selectAllNetworks.Count - 1);
                            foreach (var net in _selectAllNetworkCache)
                            {
                                net.IsSelectAreaMode = false;
                                net.IsSelectAllMode = false;
                            }
                            foreach (var net in _selectAllNetworks)
                            {
                                net.IsSelectAllMode = true;
                            }
                        }
                        else
                        {
                            _selectStartNetwork.IsSelectAllMode = false;
                            _selectStartNetwork.EnterOriginSelectArea(true);
                            CrossNetState = CrossNetworkState.NoCross;
                        }
                    }
                    else
                    {
                        if (_selectStartNetwork.SelectAreaSecondY == 0)
                        {
                            _selectStartNetwork.IsSelectAllMode = true;
                            CrossNetState = CrossNetworkState.CrossUp;
                        }
                        else
                        {
                            _selectStartNetwork.SelectAreaSecondY--;
                            //VScrollToRect(_selectStartNetwork.NetworkNumber, _selectStartNetwork.SelectAreaSecondY);
                            if (_selectStartNetwork.SelectArea.Height == GlobalSetting.LadderHeightUnit && _selectStartNetwork.SelectArea.Width == GlobalSetting.LadderWidthUnit)
                            {
                                _selectRect.X = _selectStartNetwork.SelectAreaFirstX;
                                _selectRect.Y = _selectStartNetwork.SelectAreaFirstY;
                                _selectStartNetwork.AcquireSelectRect();
                            }
                        }
                    }
                    break;
                case Key.Down:
                    ChangeViewport(BoundaryDirection.Bottom);
                    if (CrossNetState == CrossNetworkState.CrossDown)
                    {
                        CollectSelectAllNetworkDownByCount(_selectAllNetworks.Count + 1);
                        foreach (var net in _selectAllNetworkCache)
                        {
                            net.IsSelectAreaMode = false;
                            net.IsSelectAllMode = false;
                        }
                        foreach (var net in _selectAllNetworks)
                        {
                            net.IsSelectAllMode = true;
                        }
                    }
                    else if (CrossNetState == CrossNetworkState.CrossUp)
                    {
                        if (_selectAllNetworks.Count > 0)
                        {
                            CollectSelectAllNetworkUpByCount(_selectAllNetworks.Count - 1);
                            foreach (var net in _selectAllNetworkCache)
                            {
                                net.IsSelectAreaMode = false;
                                net.IsSelectAllMode = false;
                            }
                            foreach (var net in _selectAllNetworks)
                            {
                                net.IsSelectAllMode = true;
                            }
                        }
                        else
                        {
                            _selectStartNetwork.IsSelectAllMode = false;
                            _selectStartNetwork.EnterOriginSelectArea(false);
                            CrossNetState = CrossNetworkState.NoCross;
                        }
                    }
                    else
                    {
                        if (!_selectStartNetwork.ladderExpander.IsExpand || _selectStartNetwork.SelectAreaSecondY == _selectStartNetwork.RowCount - 1)
                        {
                            _selectStartNetwork.IsSelectAllMode = true;
                            CrossNetState = CrossNetworkState.CrossDown;
                        }
                        else
                        {
                            _selectStartNetwork.SelectAreaSecondY++;
                            //VScrollToRect(_selectStartNetwork.NetworkNumber, _selectStartNetwork.SelectAreaSecondY);
                            if (_selectStartNetwork.SelectArea.Height == GlobalSetting.LadderHeightUnit && _selectStartNetwork.SelectArea.Width == GlobalSetting.LadderWidthUnit)
                            {
                                _selectRect.X = _selectStartNetwork.SelectAreaFirstX;
                                _selectRect.Y = _selectStartNetwork.SelectAreaFirstY;
                                _selectStartNetwork.AcquireSelectRect();
                            }
                        }
                    }
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
                    _selectStartNetwork.SelectAreaOriginFX = _selectRect.X;
                    _selectStartNetwork.SelectAreaOriginFY = _selectRect.Y;
                    _selectStartNetwork.SelectAreaFirstX = _selectRect.X;
                    _selectStartNetwork.SelectAreaFirstY = _selectRect.Y;
                    _selectStartNetwork.SelectAreaSecondX = _selectRect.X - 1;
                    _selectStartNetwork.SelectAreaSecondY = _selectRect.Y;
                    _selectStartNetwork.SelectAreaOriginSX = _selectRect.X - 1;
                    //HScrollToRect(_selectStartNetwork.SelectAreaSecondX);
                    CrossNetState = CrossNetworkState.NoCross;
                    SelectionStatus = SelectStatus.MultiSelecting;
                    break;
                case Key.Right:
                    ChangeViewport(BoundaryDirection.Right);
                    _selectStartNetwork.SelectAreaOriginFX = _selectRect.X;
                    _selectStartNetwork.SelectAreaOriginFY = _selectRect.Y;
                    _selectStartNetwork.SelectAreaFirstX = _selectRect.X;
                    _selectStartNetwork.SelectAreaFirstY = _selectRect.Y;
                    _selectStartNetwork.SelectAreaSecondX = _selectRect.X + 1;
                    _selectStartNetwork.SelectAreaSecondY = _selectRect.Y;
                    _selectStartNetwork.SelectAreaOriginSX = _selectRect.X + 1;
                    //HScrollToRect(_selectStartNetwork.SelectAreaSecondX);
                    CrossNetState = CrossNetworkState.NoCross;
                    SelectionStatus = SelectStatus.MultiSelecting;
                    break;
                case Key.Up:
                    ChangeViewport(BoundaryDirection.Up);
                    if (_selectRect.Y == 0)
                    {
                        _selectStartNetwork.IsSelectAllMode = true;
                        _selectStartNetwork.SelectAreaOriginFY = 0;
                        CrossNetState = CrossNetworkState.CrossUp;
                    }
                    else
                    {
                        _selectStartNetwork.SelectAreaOriginFX = _selectRect.X;
                        _selectStartNetwork.SelectAreaOriginFY = _selectRect.Y;
                        _selectStartNetwork.SelectAreaOriginSX = _selectRect.X;
                        _selectStartNetwork.SelectAreaFirstX = _selectRect.X;
                        _selectStartNetwork.SelectAreaFirstY = _selectRect.Y;
                        _selectStartNetwork.SelectAreaSecondX = _selectRect.X;
                        _selectStartNetwork.SelectAreaSecondY = _selectRect.Y - 1;
                        CrossNetState = CrossNetworkState.NoCross;
                        //VScrollToRect(_selectStartNetwork.NetworkNumber, _selectStartNetwork.SelectAreaSecondY);
                    }
                    SelectionStatus = SelectStatus.MultiSelecting;
                    break;
                case Key.Down:
                    ChangeViewport(BoundaryDirection.Bottom);
                    if (_selectRect.Y == _selectStartNetwork.RowCount - 1)
                    {
                        _selectStartNetwork.IsSelectAllMode = true;
                        _selectStartNetwork.SelectAreaOriginFY = _selectRect.Y;
                        CrossNetState = CrossNetworkState.CrossDown;
                    }
                    else
                    {
                        _selectStartNetwork.SelectAreaOriginFX = _selectRect.X;
                        _selectStartNetwork.SelectAreaOriginFY = _selectRect.Y;
                        _selectStartNetwork.SelectAreaOriginSX = _selectRect.X;
                        _selectStartNetwork.SelectAreaFirstX = _selectRect.X;
                        _selectStartNetwork.SelectAreaFirstY = _selectRect.Y;
                        _selectStartNetwork.SelectAreaSecondX = _selectRect.X;
                        _selectStartNetwork.SelectAreaSecondY = _selectRect.Y + 1;
                        CrossNetState = CrossNetworkState.NoCross;
                        //VScrollToRect(_selectStartNetwork.NetworkNumber, _selectStartNetwork.SelectAreaSecondY);
                    }
                    SelectionStatus = SelectStatus.MultiSelecting;
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// 获取鼠标跨网络状态
        /// </summary>
        /// <returns></returns>
        private CrossNetworkState GetSelectionCrossNetworkState()
        {
            if (SelectionStatus == SelectStatus.MultiSelecting)
            {
                var p = Mouse.GetPosition(_selectStartNetwork.LadderCanvas);
                if (p.Y < 0 || p.X < 0 || p.X > _selectStartNetwork.LadderCanvas.ActualWidth)
                {
                    return CrossNetworkState.CrossUp;
                }
                else
                {
                    if (p.Y > _selectStartNetwork.LadderCanvas.ActualHeight)
                    {
                        return CrossNetworkState.CrossDown;
                    }
                    else
                    {
                        return CrossNetworkState.NoCross;
                    }
                }
            }
            return CrossNetworkState.NoCross;
        }

        /// <summary>
        /// 收集从起始网络(_selectStartNetwork)开始，鼠标向下掠过的网络，加入到_selectAllNetworks中（不包括_selectStartNetwork）
        /// </summary>
        private void CollectSelectAllNetworkDown()
        {
            _selectAllNetworks.Clear();
            int id = _selectStartNetwork.Core.ID + 1;
            while (id < Core.NetworkCount)
            {
                LadderNetworkModel net = Core.Children[id];
                if (Mouse.GetPosition(net.View.LadderCanvas).Y < 0)
                {
                    break;
                }
                else
                {
                    if (!net.View.IsMasked)
                    {
                        _selectAllNetworks.Add(net.View);
                        _selectAllNetworkCache.Add(net.View);
                    }
                    id++;
                }
            }
        }
        private void CollectSelectAllNetworkDownByCount(int count)
        {
            _selectAllNetworks.Clear();
            int id = _selectStartNetwork.Core.ID + 1;
            while (id < Core.NetworkCount && count > 0)
            {
                LadderNetworkModel net = Core.Children[id];
                if (!net.View.IsMasked)
                {
                    _selectAllNetworks.Add(net.View);
                    _selectAllNetworkCache.Add(net.View);
                    count--;
                }
                id++;
            }
        }

        /// <summary>
        /// 收集从起始网络(_selectStartNetwork)开始，鼠标向上掠过的网络，加入到_selectAllNetworks中（不包括_selectStartNetwork）
        /// </summary>
        private void CollectSelectAllNetworkUp()
        {
            _selectAllNetworks.Clear();
            int id = _selectStartNetwork.Core.ID - 1;
            while (id >= 0)
            {
                LadderNetworkModel net = Core.Children[id];
                if (Mouse.GetPosition(net.View.LadderCanvas).Y > net.View.LadderCanvas.ActualHeight)
                {
                    break;
                }
                else
                {
                    if (!net.View.IsMasked)
                    {
                        _selectAllNetworks.Add(net.View);
                        _selectAllNetworkCache.Add(net.View);
                    }
                    id--;
                }
            }
        }
        private void CollectSelectAllNetworkUpByCount(int count)
        {
            _selectAllNetworks.Clear();
            int id = _selectStartNetwork.Core.ID - 1;
            while (id >= 0 && count > 0)
            {
                LadderNetworkModel net = Core.Children[id];
                if (!net.View.IsMasked)
                {
                    _selectAllNetworks.Add(net.View);
                    _selectAllNetworkCache.Add(net.View);
                    count--;
                }
                id--;
            }
        }

        #endregion

        #region Selection state transfers

        private void EnterIdleState()
        {
            if (_selectStartNetwork != null)
            {
                _selectStartNetwork.IsSelectAreaMode = false;
                _selectStartNetwork.IsSelectAllMode = false;
                _selectStartNetwork = null;
            }
            SelectRectOwner = null;
            _selectAllNetworks.Clear();
            foreach (var net in _selectAllNetworkCache)
            {
                net.IsSelectAreaMode = false;
                net.IsSelectAllMode = false;
            }
            _selectAllNetworkCache.Clear();
            CrossNetState = CrossNetworkState.NoCross;
        }

        private void EnterSingleSelectedState()
        {
            if (_selectStartNetwork != null)
            {
                _selectStartNetwork.IsSelectAreaMode = false;
                _selectStartNetwork.IsSelectAllMode = false;
                _selectStartNetwork = null;
            }
            _selectAllNetworks.Clear();
            foreach (var net in _selectAllNetworkCache)
            {
                net.IsSelectAreaMode = false;
                net.IsSelectAllMode = false;
            }
            _selectAllNetworkCache.Clear();
            CrossNetState = CrossNetworkState.NoCross;
        }

        private void EnterMultiSelectingState()
        {
            if (_selectStartNetwork != null)
            {
                _selectStartNetwork.IsSelectAreaMode = true;
            }
            SelectRectOwner = null;
        }

        private void EnterMultiSelectedState()
        {
            SelectRectOwner = null;
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
                        point = _selectStartNetwork.SelectArea.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (_selectStartNetwork.SelectAreaSecondY == 0)
                        {
                            if (point.Y < 100 * scaleY || _selectStartNetwork.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                            {
                                return point.Y - 100 * scaleY;
                            }
                            if (point.Y + _selectStartNetwork.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                            {
                                return point.Y + _selectStartNetwork.ActualHeight * scaleY - MainScrollViewer.ViewportHeight;
                            }
                        }
                        else
                        {
                            if (_selectStartNetwork.SelectAreaFirstY > _selectStartNetwork.SelectAreaSecondY)
                            {
                                if (point.Y < (_selectRect.ActualHeight + 30) * scaleY || (_selectStartNetwork.SelectArea.ActualHeight + _selectRect.ActualHeight) * scaleY > MainScrollViewer.ViewportHeight)
                                {
                                    return point.Y - (_selectRect.ActualHeight + 30) * scaleY;
                                }
                                if (point.Y + 30 * scaleY > MainScrollViewer.ViewportHeight)
                                {
                                    return point.Y + 30 * scaleY - MainScrollViewer.ViewportHeight;
                                }
                            }
                            if (_selectStartNetwork.SelectAreaFirstY < _selectStartNetwork.SelectAreaSecondY)
                            {
                                if (point.Y + (_selectStartNetwork.SelectArea.ActualHeight - _selectRect.ActualHeight) * scaleY > MainScrollViewer.ViewportHeight)
                                {
                                    return point.Y + (_selectStartNetwork.SelectArea.ActualHeight - _selectRect.ActualHeight) * scaleY - MainScrollViewer.ViewportHeight;
                                }
                                if (point.Y + (_selectStartNetwork.SelectArea.ActualHeight - 2 * _selectRect.ActualHeight) * scaleY < 0)
                                {
                                    return point.Y + (_selectStartNetwork.SelectArea.ActualHeight - 2 * _selectRect.ActualHeight) * scaleY;
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
                                if (_selectAllNetworks.Count == 0)
                                {
                                    point = _selectStartNetwork.TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    value = GetNextNetworkOffset(_selectStartNetwork.Core.ID, true, true);
                                }
                                else
                                {
                                    point = _selectAllNetworks.First().TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    value = GetNextNetworkOffset(_selectAllNetworks.First().Core.ID, true, true);
                                }
                                if (point.Y < value * scaleY)
                                    return point.Y - value * scaleY;
                                break;
                            case CrossNetworkState.CrossDown:
                                if (_selectAllNetworks.Count == 0)
                                {
                                    point = _selectStartNetwork.TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    return point.Y + _selectStartNetwork.ActualHeight * scaleY - MainScrollViewer.ViewportHeight;
                                }
                                else
                                {
                                    point = _selectAllNetworks.Last().TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    value = GetNextNetworkOffset(_selectAllNetworks.Last().Core.ID, false, false);
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
                        point = _selectStartNetwork.SelectArea.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (point.X < 0) return point.X;
                        if (point.X + (_selectRect.ActualWidth + _selectStartNetwork.SelectArea.ActualWidth) * scaleX > MainScrollViewer.ViewportWidth)
                        {
                            return point.X + (_selectRect.ActualWidth + _selectStartNetwork.SelectArea.ActualWidth) * scaleX - MainScrollViewer.ViewportWidth;
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
                        point = _selectStartNetwork.SelectArea.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (point.X + _selectStartNetwork.SelectArea.ActualWidth * scaleX > MainScrollViewer.ViewportWidth)
                        {
                            return point.X + _selectStartNetwork.SelectArea.ActualWidth * scaleX - MainScrollViewer.ViewportWidth;
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
                        point = _selectStartNetwork.SelectArea.TranslatePoint(new Point(0, 0), MainScrollViewer);
                        if (_selectStartNetwork.SelectAreaSecondY == _selectStartNetwork.RowCount - 1)
                        {
                            if (point.Y + _selectStartNetwork.SelectArea.ActualHeight * scaleY > MainScrollViewer.ViewportHeight || _selectStartNetwork.ActualHeight * scaleY > MainScrollViewer.ViewportHeight)
                            {
                                return point.Y + _selectStartNetwork.SelectArea.ActualHeight * scaleY - MainScrollViewer.ViewportHeight;
                            }
                            if (point.Y + _selectStartNetwork.SelectArea.ActualHeight * scaleY < _selectStartNetwork.RowCount * _selectRect.ActualHeight * scaleY + 100 * scaleY)
                            {
                                return point.Y + _selectStartNetwork.SelectArea.ActualHeight * scaleY - _selectStartNetwork.RowCount * _selectRect.ActualHeight * scaleY - 100 * scaleY;
                            }
                        }
                        else
                        {
                            if (_selectStartNetwork.SelectAreaFirstY > _selectStartNetwork.SelectAreaSecondY)
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
                            if (_selectStartNetwork.SelectAreaFirstY < _selectStartNetwork.SelectAreaSecondY)
                            {
                                if (point.Y + (_selectStartNetwork.SelectArea.ActualHeight + _selectRect.ActualHeight) * scaleY > MainScrollViewer.ViewportHeight)
                                {
                                    return point.Y + (_selectStartNetwork.SelectArea.ActualHeight + _selectRect.ActualHeight) * scaleY - MainScrollViewer.ViewportHeight;
                                }
                                if (point.Y + _selectStartNetwork.SelectArea.ActualHeight * scaleY < 0)
                                {
                                    return point.Y + _selectStartNetwork.SelectArea.ActualHeight * scaleY;
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
                                if (_selectAllNetworks.Count == 0)
                                {
                                    point = _selectStartNetwork.TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    return point.Y;
                                }
                                else
                                {
                                    point = _selectAllNetworks.First().TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    value = GetNextNetworkOffset(_selectAllNetworks.First().Core.ID, true, false);
                                }
                                return point.Y + (value + _selectAllNetworks.First().ActualHeight) * scaleY;
                            case CrossNetworkState.CrossDown:
                                LadderNetworkViewModel net;
                                if (_selectAllNetworks.Count == 0)
                                {
                                    point = _selectStartNetwork.TranslatePoint(new Point(0, 0), MainScrollViewer);
                                    value = GetNextNetworkOffset(_selectStartNetwork.Core.ID, false, true);
                                    net = _selectStartNetwork;
                                }
                                else
                                {
                                    net = _selectAllNetworks.Last();
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
            if (SelectRectOwner != null) SelectRectOwner = null;
            SelectionStatus = SelectStatus.Idle;
            CrossNetState = CrossNetworkState.NoCross;
            _selectAllNetworks.Clear();
            _selectAllNetworkCache.Clear();
            _selectStartNetwork = network.View;
            _selectStartNetwork.IsSelectAreaMode = true;
            _selectStartNetwork.SelectAreaFirstX = x1;
            _selectStartNetwork.SelectAreaFirstY = y1;
            _selectStartNetwork.SelectAreaSecondX = x2;
            _selectStartNetwork.SelectAreaSecondY = y2;
            SelectionStatus = SelectStatus.MultiSelected;
            CrossNetState = CrossNetworkState.NoCross;
            NavigateToNetworkByNum(network.ID);
        }

        public void Select(int start, int end)
        {
            if (SelectRectOwner != null) SelectRectOwner = null;
            SelectionStatus = SelectStatus.Idle;
            CrossNetState = CrossNetworkState.NoCross;
            _selectStartNetwork = null;
            _selectAllNetworks.Clear();
            _selectAllNetworkCache.Clear();
            for (int nn = start; nn <= end; nn++)
            {
                LadderNetworkModel network = Core.Children[nn];
                network.View.IsSelectAllMode = true;
                if (network.IsMasked) continue;
                if (_selectStartNetwork == null)
                    _selectStartNetwork = network.View;
                else
                {
                    _selectAllNetworks.Add(network.View);
                    _selectAllNetworkCache.Add(network.View);
                }
            }
            if (_selectStartNetwork != null)
            {
                SelectionStatus = SelectStatus.MultiSelected;
                CrossNetState = CrossNetworkState.CrossDown;
                NavigateToNetworkByNum(_selectStartNetwork.Core.ID);
            }
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
        
        #region network drag(only not IsExpand)

        private LadderNetworkViewModel dragItem;
        private LadderNetworkViewModel currentItem;
        private void OnLadderDiagramMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);

            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectionStatus = SelectStatus.Idle;
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
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
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
                LadderNetworkStackPanel.Children.Clear();
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
                LadderNetworkStackPanel.Children.Clear();
                foreach (LadderNetworkModel net in core.Children)
                {
                    if (net.View == null) net.View = new LadderNetworkViewModel(net);
                    LadderNetworkStackPanel.Children.Add(net.View);
                }
            }
        }

        public InstructionDiagramViewModel Inst
        {
            get { return core != null && core.Inst != null ? core.Inst.View : null; }
        }
        
        public LadderModes LadderMode { get { return core.LadderMode; } }
        
        private bool iscommentmode;
        public bool IsCommentMode
        {
            get
            {
                return this.iscommentmode;
            }
            set
            {
                this.iscommentmode = value;
                if (_selectRect != null) _selectRect.IsCommentMode = iscommentmode;
                if (Inst != null) Inst.IsCommentMode = value;
                foreach (LadderNetworkModel net in Core.Children)
                {
                    if (net.View != null) net.View.IsCommentMode = iscommentmode;
                }
            }
        }
        
        private int WidthUnit { get { return GlobalSetting.LadderWidthUnit; } }
        private int HeightUnit { get { return IsCommentMode ? GlobalSetting.LadderCommentModeHeightUnit : GlobalSetting.LadderHeightUnit; } }

        #endregion

        #region Event Handler

        public event RoutedEventHandler SelectionChanged = delegate { };

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
                SelectionStatus = SelectStatus.MultiSelected;
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
                            SelectionStatus = SelectStatus.MultiSelecting;
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
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
        }
        private void OnLadderDiagramMouseUp(object sender, MouseButtonEventArgs e)
        {
            // 如果处于选择模式则关闭
            if (_selectStatus == SelectStatus.MultiSelecting)
            {
                SelectionStatus = SelectStatus.MultiSelected;
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
                    if (CrossNetState == CrossNetworkState.CrossDown || CrossNetState == CrossNetworkState.CrossUp)
                    {
                        if (_selectStartNetwork != null)
                            _selectAllNetworks.Add(_selectStartNetwork);
                        List<LadderNetworkModel> removes = _selectAllNetworks.Select(nv => nv.Core).ToList();
                        Core.ReplaceN(removes, new LadderNetworkModel[] { });
                        SelectionStatus = SelectStatus.Idle;
                    }
                    else
                    {
                        int xBegin = Math.Min(_selectStartNetwork.SelectAreaFirstX, _selectStartNetwork.SelectAreaSecondX);
                        int yBegin = Math.Min(_selectStartNetwork.SelectAreaFirstY, _selectStartNetwork.SelectAreaSecondY);
                        int xEnd = Math.Max(_selectStartNetwork.SelectAreaFirstX, _selectStartNetwork.SelectAreaSecondX);
                        int yEnd = Math.Max(_selectStartNetwork.SelectAreaFirstY, _selectStartNetwork.SelectAreaSecondY);
                        IEnumerable<LadderUnitModel> units = _selectStartNetwork.Core.Children.SelectRange(xBegin, xEnd, yBegin, yEnd);
                        units = units.Concat(_selectStartNetwork.Core.VLines.SelectRange(xBegin, xEnd, yBegin, yEnd));
                        Core.RemoveU(_selectStartNetwork.Core, units);
                        SelectionStatus = SelectStatus.Idle;
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
                    if (CrossNetState == CrossNetworkState.CrossDown || CrossNetState == CrossNetworkState.CrossUp)
                    {
                        XElement xele_ns = new XElement("Networks");
                        xele.Add(xele_ns);
                        if (_selectStartNetwork != null)
                            _selectAllNetworks.Add(_selectStartNetwork);
                        List<LadderNetworkModel> removes = new List<LadderNetworkModel>();
                        foreach (LadderNetworkViewModel lnvmodel in _selectAllNetworks)
                        {
                            XElement xele_n = new XElement("Network");
                            lnvmodel.Core.Save(xele_n);
                            xele_ns.Add(xele_n);
                            removes.Add(lnvmodel.Core);
                        }
                        if (cut) Core.ReplaceN(removes, new LadderNetworkModel[] { });
                        Clipboard.SetData("LadderContent", xele.ToString());
                        SelectionStatus = SelectStatus.Idle;
                    }
                    else
                    {
                        // 单网络多图元复制
                        int xBegin = Math.Min(_selectStartNetwork.SelectAreaFirstX, _selectStartNetwork.SelectAreaSecondX);
                        int yBegin = Math.Min(_selectStartNetwork.SelectAreaFirstY, _selectStartNetwork.SelectAreaSecondY);
                        int xEnd = Math.Max(_selectStartNetwork.SelectAreaFirstX, _selectStartNetwork.SelectAreaSecondX);
                        int yEnd = Math.Max(_selectStartNetwork.SelectAreaFirstY, _selectStartNetwork.SelectAreaSecondY);
                        xele.SetAttributeValue("XBegin", xBegin);
                        xele.SetAttributeValue("YBegin", yBegin);
                        xele.SetAttributeValue("XEnd", xEnd);
                        xele.SetAttributeValue("YEnd", yEnd);
                        XElement xele_us = new XElement("Units");
                        xele.Add(xele_us);
                        IEnumerable<LadderUnitModel> units = _selectStartNetwork.Core.Children.SelectRange(xBegin, xEnd, yBegin, yEnd);
                        units = units.Concat(_selectStartNetwork.Core.VLines.SelectRange(xBegin, xEnd, yBegin, yEnd));
                        foreach (LadderUnitModel unit in units)
                        {
                            XElement xele_u = new XElement("Unit");
                            unit.Save(xele_u);
                            xele_us.Add(xele_u);
                        }
                        Clipboard.SetData("LadderContent", xele.ToString());
                        if (cut) Core.RemoveU(_selectStartNetwork.Core, units);
                        SelectionStatus = SelectStatus.Idle;
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
                    //xele = xele.Element("Root");
                    //SelectStatus selectstatus = (SelectStatus)int.Parse(xele.Attribute("SelectionStatus").Value);
                    //CrossNetworkState crossstate = (CrossNetworkState)int.Parse(xele.Attribute("CrossNetworkState").Value);
                    XElement xele_c = xele.Element("Current");
                    XElement xele_us = xele.Element("Units");
                    XElement xele_ns = xele.Element("Networks");
                    if (xele_c != null)
                    {
                        LadderUnitModel unit = new LadderUnitModel(null, xele_c);
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
                            LadderUnitModel unit = new LadderUnitModel(null, xele_u);
                            units.Add(unit);
                        }
                        bool containoutput = units.Where(u => u.Shape == LadderUnitModel.Shapes.Output || u.Shape == LadderUnitModel.Shapes.OutputRect).Count() > 0;
                        int xBegin = containoutput
                            ? GlobalSetting.LadderXCapacity - _xWidth : SelectRectOwner != null
                            ? _selectRect.X : Math.Min(_selectStartNetwork.SelectAreaFirstX, _selectStartNetwork.SelectAreaSecondX);
                        int yBegin = SelectRectOwner != null
                            ? _selectRect.Y : Math.Min(_selectStartNetwork.SelectAreaFirstY, _selectStartNetwork.SelectAreaSecondY);
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
                            ? SelectRectOwner.ID : CrossNetState == CrossNetworkState.CrossUp && _selectAllNetworks.Count() > 0
                            ? _selectAllNetworks.Select(nv => nv.Core.ID).Min()
                            : _selectStartNetwork.Core.ID;
                        int nEnd = nStart - 1;
                        foreach (XElement xele_n in xele_ns.Elements("Network"))
                        {
                            LadderNetworkModel net = new LadderNetworkModel(null, 0);
                            net.Load(xele_n);
                            net.ID = ++nEnd;
                            nets.Add(net);
                        }
                        IEnumerable<LadderNetworkModel> oldnets = SelectRectOwner != null
                            ? new LadderNetworkModel[] { SelectRectOwner }
                            : new LadderNetworkModel[] { _selectStartNetwork.Core }.Concat(_selectAllNetworks.Select(nv => nv.Core));
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
            SelectionStatus = SelectStatus.MultiSelected;
            CrossNetState = CrossNetworkState.CrossDown;
            int id = 0;
            LadderNetworkModel lnmodel = Core.Children[id];
            while (lnmodel.IsMasked && id < Core.NetworkCount - 1)
                lnmodel = Core.Children[++id];
            _selectStartNetwork = lnmodel.View;
            _selectStartNetwork.IsSelectAllMode = true;
            foreach (var net in Core.Children)
            { 
                if (net.IsMasked || net == _selectStartNetwork.Core) continue;
                _selectAllNetworkCache.Add(net.View);
                _selectAllNetworks.Add(net.View);
                net.View.IsSelectAllMode = true;
            }
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

        #endregion

    }
}

using SamSoarII.Core.Models;
using SamSoarII.Global;
using SamSoarII.Shell.Dialogs;
using SamSoarII.Shell.Windows;
using SamSoarII.Threads;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SamSoarII.Shell.Models
{
    enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
    enum DirectionStatus
    {
        Up_Dec,
        Up_Inc,
        Down_Dec,
        Down_Inc,
        None
    }
    /// <summary>
    /// LadderNetworkViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class LadderNetworkViewModel : UserControl, IViewModel, INotifyPropertyChanged, IComparable<LadderNetworkViewModel>
    {
        public LadderNetworkViewModel(LadderNetworkModel _core)
        {
            InitializeComponent();
            DataContext = this;
            Core = _core;
            SelectArea.Fill = new SolidColorBrush(Colors.DarkBlue);
            SelectArea.Opacity = 0.2;
            Canvas.SetZIndex(SelectArea, -1);
            ladderExpander.MouseEnter += OnExpanderMouseEnter;
            ladderExpander.MouseLeave += OnExpanderMouseLeave;
            ladderExpander.expandButton.IsExpandChanged += OnExpandChanged;
            ladderExpander.IsExpand = IsExpand;
            CommentAreaGrid.Children.Remove(ThumbnailButton);
            if (ViewParent != null)
                IsCommentMode = ViewParent.IsCommentMode;
            loadedrowstart = 0;
            loadedrowend = -1;
            Update();
        }

        public void Dispose()
        {
            Core = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public int CompareTo(LadderNetworkViewModel that)
        {
            return this.Core.ID.CompareTo(that.Core.ID);
        }
        
        #region Core

        private LadderNetworkModel core;
        public LadderNetworkModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                LadderNetworkModel _core = core;
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
                }
            }
        }
        
        IModel IViewModel.Core
        {
            get { return core; }
            set { Core = (LadderNetworkModel)value; }
        }
        public InteractionFacade IFParent { get { return core?.Parent?.Parent?.Parent; } }

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RowCount":
                    if (IsSingleSelected())
                        if (ViewParent.SelectionRect.Y >= RowCount) ViewParent.SelectionRect.Y = RowCount - 1;
                    if (IsSelectAreaMode || IsSelectAllMode)
                    {
                        if (SelectAreaFirstY >= RowCount) SelectAreaFirstY = RowCount - 1;
                        if (SelectAreaSecondY >= RowCount) SelectAreaSecondY = RowCount - 1;
                    }
                    LadderCanvas.Height = RowCount * HeightUnit;
                    DynamicUpdate();
                    PropertyChanged(this, new PropertyChangedEventArgs("RowCount"));
                    break;
                case "IsExpand":
                    ladderExpander.IsExpand = IsExpand;
                    Update();
                    PropertyChanged(this, new PropertyChangedEventArgs("RowCount"));
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExpand"));
                    break;
                case "IsMasked":
                    if (IsMasked)
                    {
                        ViewParent.SelectionRect.Core.Parent = null;
                        IsSelectAreaMode = false;
                        IsSelectAllMode = false;
                        CommentAreaExpander.Background = Brushes.LightGray;
                        LadderCanvas.Background = Brushes.LightGray;
                        CommentAreaExpander.Opacity = 0.4;
                        LadderCanvas.Opacity = 0.4;
                    }
                    else
                    {
                        CommentAreaExpander.Opacity = 1.0;
                        LadderCanvas.Opacity = 1.0;
                        CommentAreaExpander.Background = Brushes.LightCyan;
                        LadderCanvas.Background = Brushes.Transparent;
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("IsMasked"));
                    break;
                case "ID":
                    NetworkNumberLabel.Content = NetworkNumber;
                    //PropertyChanged(this, new PropertyChangedEventArgs("NetworkNumber"));
                    break;
                case "Brief":
                    NetworkBriefLabel.Content = NetworkBrief;
                    //PropertyChanged(this, new PropertyChangedEventArgs("NetworkBrief"));
                    break;
                case "Description":
                    NetworkDescriptionTextBlock.Text = NetworkDescription;
                    //PropertyChanged(this, new PropertyChangedEventArgs("NetworkDescription"));
                    break;
            }
        }
        
        private void OnCoreChildrenChanged(LadderUnitModel sender, LadderUnitChangedEventArgs e)
        {
            //if (!IsFullLoaded) return;
            if (!IsExpand) IsExpand = true;
            switch (e.Action)
            {
                case LadderUnitAction.ADD:
                    if (sender.Y < loadedrowstart || sender.Y > loadedrowend)
                        break;
                    if (sender.View == null)
                        sender.View = LadderUnitViewModel.Create(sender);
                    //if (sender.View.Parent is Canvas)
                    //    ((Canvas)(sender.View.Parent)).Children.Remove(sender.View);
                    LadderCanvas.Children.Add(sender.View);
                    break;
                case LadderUnitAction.REMOVE:
                    if (sender.View == null) break;
                    LadderCanvas.Children.Remove(sender.View);
                    //AllResourceManager.Dispose(sender.View);
                    //sender.View = null;
                    sender.View.Dispose();
                    break;
                case LadderUnitAction.MOVE:
                    break;
                case LadderUnitAction.UPDATE:
                    break;
            }
        }

        #endregion

        #region Shell

        public LadderDiagramViewModel ViewParent { get { return core?.Parent?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        #region Binding
        
        public int RowCount
        {
            get { return IsExpand ? core.RowCount : 0; }
            set { core.RowCount = value; }
        }

        public string NetworkNumber
        {
            get { return String.Format("Network {0:d}", core != null ? core.ID : 0); }
        }

        public string NetworkBrief
        {
            get { return Core.Brief; }
        }

        public string NetworkDescription
        {
            get { return Core.Description; }
        }


        #endregion

        #region Canvas System

        #endregion

        #region Select

        private int WidthUnit { get { return GlobalSetting.LadderWidthUnit; } }
        private int HeightUnit { get { return IsCommentMode ? GlobalSetting.LadderCommentModeHeightUnit : GlobalSetting.LadderHeightUnit; } }

        public int SelectAreaOriginFX
        {
            get; set;
        }
        public int SelectAreaOriginFY
        {
            get; set;
        }
        private int _selectAreaOriginSX;
        public int SelectAreaOriginSX
        {
            get
            {
                return _selectAreaOriginSX;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > GlobalSetting.LadderXCapacity - 1)
                {
                    value = GlobalSetting.LadderXCapacity - 1;
                }
                _selectAreaOriginSX = value;
            }
        }
        private int _selectAreaFirstX;
        private int _selectAreaFirstY;
        private int _selectAreaSecondX;
        private int _selectAreaSecondY;
        public int SelectAreaFirstX
        {
            get
            {
                return _selectAreaFirstX;
            }
            set
            {
                _selectAreaFirstX = value;
                var left = Math.Min(_selectAreaFirstX, _selectAreaSecondX) * WidthUnit;
                var width = (Math.Abs(_selectAreaFirstX - _selectAreaSecondX) + 1) * WidthUnit;
                SelectArea.Width = width;
                Canvas.SetLeft(SelectArea, left);
            }
        }
        public int SelectAreaFirstY
        {
            get
            {
                return _selectAreaFirstY;
            }
            set
            {
                _selectAreaFirstY = value;
                var top = Math.Min(_selectAreaFirstY, _selectAreaSecondY) * HeightUnit;
                int height = !IsExpand ? 0 : (Math.Abs(_selectAreaFirstY - _selectAreaSecondY) + 1) * HeightUnit;
                SelectArea.Height = height;
                Canvas.SetTop(SelectArea, top);
            }
        }
        public int SelectAreaSecondX
        {
            get
            {
                return _selectAreaSecondX;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > GlobalSetting.LadderXCapacity - 1)
                {
                    value = GlobalSetting.LadderXCapacity - 1;
                }
                _selectAreaSecondX = value;
                var left = Math.Min(_selectAreaFirstX, _selectAreaSecondX) * WidthUnit;
                var width = (Math.Abs(_selectAreaFirstX - _selectAreaSecondX) + 1) * WidthUnit;
                SelectArea.Width = width;
                Canvas.SetLeft(SelectArea, left);
            }
        }
        public int SelectAreaSecondY
        {
            get
            {
                return _selectAreaSecondY;
            }
            set
            {
                if (value < 0) value = 0;
                if (value > RowCount - 1)
                    value = RowCount - 1;
                _selectAreaSecondY = value;
                var top = Math.Min(_selectAreaFirstY, _selectAreaSecondY) * HeightUnit;
                int height = !IsExpand ? 0 : (Math.Abs(_selectAreaFirstY - _selectAreaSecondY) + 1) * HeightUnit;
                SelectArea.Height = height;
                Canvas.SetTop(SelectArea, top);
            }
        }
        private bool _isSelectAreaMode;
        public bool IsSelectAreaMode
        {
            get
            {
                return _isSelectAreaMode;
            }
            set
            {
                if (!_isSelectAreaMode && value
                 && !LadderCanvas.Children.Contains(SelectArea))
                {
                    LadderCanvas.Children.Add(SelectArea);
                }
                if (!value && _isSelectAreaMode
                 && LadderCanvas.Children.Contains(SelectArea))
                {
                    LadderCanvas.Children.Remove(SelectArea);
                }
                _isSelectAreaMode = value;
            }
        }
        private bool _isSelectAllMode;
        public bool IsSelectAllMode
        {
            get
            {
                return _isSelectAllMode;
            }
            set
            {
                _isSelectAllMode = value;
                if (_isSelectAllMode)
                {
                    IsSelectAreaMode = true;
                    SelectAreaFirstY = 0;
                    SelectAreaFirstX = 0;
                    SelectAreaSecondX = GlobalSetting.LadderXCapacity - 1;
                    SelectAreaSecondY = !IsExpand ? 0 : RowCount - 1;
                    CommentAreaGrid.Background = new SolidColorBrush(Colors.DarkBlue);
                    CommentAreaGrid.Background.Opacity = 0.3;
                }
                else
                {
                    CommentAreaGrid.Background = Brushes.Transparent;
                }
            }
        }
        public Rectangle SelectArea { get; set; } = new Rectangle();
        
        public bool IsSingleSelected()
        {
            return LadderCanvas.Children.Contains(ViewParent.SelectionRect);
        }
        
        public void ReleaseSelectRect()
        {
            if (ViewParent.SelectionRect == null) return;
            if (ViewParent.SelectionRect.Core.Parent == Core)
                ViewParent.SelectionRect.Core.Parent = null;
        }
        public void AcquireSelectRect()
        {
            if (ViewParent.SelectionRect == null) return;
            if (ViewParent.SelectionRect.Core.Parent != Core)
                ViewParent.SelectionRect.Core.Parent = Core;
        }

        public void EnterOriginSelectArea(bool isUp)
        {
            if (!isUp)
            {
                if (IsExpand && SelectAreaOriginFY == 0 && SelectAreaOriginFX == SelectAreaOriginSX)
                {
                    AcquireSelectRect();
                }
                else
                {
                    SelectAreaSecondY = 0;
                    SelectAreaFirstX = SelectAreaOriginFX;
                    SelectAreaFirstY = SelectAreaOriginFY;
                    SelectAreaSecondX = SelectAreaOriginSX;
                    IsSelectAreaMode = true;
                }
            }
            else
            {
                if (IsExpand && SelectAreaOriginFY == RowCount - 1 && SelectAreaOriginFX == SelectAreaOriginSX)
                {
                    AcquireSelectRect();
                }
                else
                {
                    SelectAreaSecondY = RowCount > 0 ? RowCount - 1 : 0;
                    SelectAreaFirstX = SelectAreaOriginFX;
                    SelectAreaFirstY = SelectAreaOriginFY;
                    SelectAreaSecondX = SelectAreaOriginSX;
                    IsSelectAreaMode = true;
                }
            }
        }

        public IEnumerable<LadderUnitModel> GetSelectedElements()
        {
            int xBegin = Math.Min(_selectAreaFirstX, _selectAreaSecondX);
            int xEnd = Math.Max(_selectAreaFirstX, _selectAreaSecondX);
            int yBegin = Math.Min(_selectAreaFirstY, _selectAreaSecondY);
            int yEnd = Math.Max(_selectAreaFirstY, _selectAreaSecondY);
            return Core.Children.SelectRange(xBegin, xEnd, yBegin, yEnd);
        }
        
        public IEnumerable<LadderUnitModel> GetSelectedHLines()
        {
            return GetSelectedElements().Where(
                (ele) => { return ele.Shape == LadderUnitModel.Shapes.HLine; });
        }

        public IEnumerable<LadderUnitModel> GetSelectedVLines()
        {
            int xBegin = Math.Min(_selectAreaFirstX, _selectAreaSecondX);
            int xEnd = Math.Max(_selectAreaFirstX, _selectAreaSecondX);
            int yBegin = Math.Min(_selectAreaFirstY, _selectAreaSecondY);
            int yEnd = Math.Max(_selectAreaFirstY, _selectAreaSecondY);
            return Core.VLines.SelectRange(xBegin, xEnd, yBegin, yEnd);
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
                Core.IsExpand = value;
            }
        }

        private Canvas tipcanvas = null;
        private ToolTip GenerateToolTipByLadder()
        {
            ToolTip tooltip = new ToolTip();
            ScrollViewer scroll = new ScrollViewer();
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            tipcanvas = new Canvas();
            scroll.MaxHeight = 385;
            tipcanvas.Background = Brushes.White;
            tipcanvas.HorizontalAlignment = HorizontalAlignment.Left;
            ScaleTransform transform = new ScaleTransform(GlobalSetting.LadderOriginScaleX / 1.7, GlobalSetting.LadderOriginScaleY / 1.7);
            tipcanvas.Height = Core.RowCount * HeightUnit;
            tipcanvas.Width = LadderCanvas.Width;
            tipcanvas.LayoutTransform = transform;
            scroll.Content = tipcanvas;
            tooltip.Content = scroll;
            return tooltip;
        }

        private void RemoveToolTipByLadder(ToolTip tooltip)
        {
            if (tooltip != null)
            {
                //Canvas canvas = (Canvas)((ScrollViewer)tooltip.Content).Content;
                tipcanvas.LayoutTransform = null;
                tipcanvas.Children.Clear();
                tipcanvas = null;
            }
        }

        #endregion

        #region Acquire
        
        public bool AcquireSelectRect(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsMasked)
            {
                var pos = e.GetPosition(LadderCanvas);
                return AcquireSelectRect(pos);
            }
            return false;
        }
        public bool AcquireSelectRect(DragEventArgs e)
        {
            var pos = e.GetPosition(LadderCanvas);
            return AcquireSelectRect(pos);
        }
        public bool AcquireSelectRect(Point pos)
        {
            var intPoint = IntPoint.GetIntpointByDouble(pos.X, pos.Y, WidthUnit, HeightUnit);
            if (intPoint.X < 0 || intPoint.X >= GlobalSetting.LadderXCapacity
             || intPoint.Y < 0 || intPoint.Y >= RowCount)
            {
                return false;
            }
            ViewParent.SelectionRect.X = intPoint.X;
            ViewParent.SelectionRect.Y = intPoint.Y;
            if (!IsSingleSelected())
                AcquireSelectRect();
            return true;
        }

        #endregion

        #region Dynamic

        private int loadedrowstart;
        public int LoadedRowStart { get { return this.loadedrowstart; } }

        private int loadedrowend;
        public int LoadedRowEnd { get { return this.loadedrowend; } }

        private double oldscrolloffset;
        public void DynamicUpdate()
        {
            //double scaleX = GlobalSetting.LadderScaleTransform.ScaleX;
            double scaleY = 0;
            ScrollViewer scroll = null;
            Point p = new Point();
            double newscrolloffset = 0;
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                scaleY = GlobalSetting.LadderScaleTransform.ScaleY;
                scroll = ViewParent?.Scroll;
                if (scroll == null) return;
                p = LadderCanvas.TranslatePoint(new Point(0, 0), scroll);
                newscrolloffset = scroll.VerticalOffset;
            });
            if (scroll == null) return;
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
                int _loadedrowend = RowCount - 1;
                _loadedrowstart = Math.Max(_loadedrowstart, (int)(-p.Y / (HeightUnit * scaleY)) - 3);
                _loadedrowend = Math.Min(_loadedrowend, (int)((-p.Y + scroll.ViewportHeight) / (HeightUnit * scaleY)) + 3);
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
                else if (loadedrowstart > _loadedrowend)
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
                        if (_loadedrowstart < loadedrowstart)
                            CreateRange(_loadedrowstart, Math.Min(_loadedrowend, loadedrowstart - 1));
                        if (_loadedrowstart > loadedrowstart)
                            DisposeRange(loadedrowstart, _loadedrowstart - 1);
                        if (loadedrowend < _loadedrowend)
                            CreateRange(Math.Max(_loadedrowstart, loadedrowend + 1), _loadedrowend);
                        if (loadedrowend > _loadedrowend)
                            DisposeRange(_loadedrowend + 1, loadedrowend);
                    }
                    else
                    {
                        if (_loadedrowstart < loadedrowstart)
                            CreateRange(Math.Min(_loadedrowend, loadedrowstart - 1), _loadedrowstart);
                        if (_loadedrowstart > loadedrowstart)
                            DisposeRange(_loadedrowstart - 1, loadedrowstart);
                        if (loadedrowend < _loadedrowend)
                            CreateRange(_loadedrowend, Math.Max(_loadedrowstart, loadedrowend + 1));
                        if (loadedrowend > _loadedrowend)
                            DisposeRange(loadedrowend, _loadedrowend + 1);
                    }
                }
                loadedrowstart = _loadedrowstart;
                loadedrowend = _loadedrowend;
            }
            oldscrolloffset = newscrolloffset;
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

        private void CreateRange(int rowstart, int rowend)
        {
            int dir = (rowstart < rowend ? 1 : -1);
            for (int y = rowstart; y != rowend + dir; y += dir)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    IEnumerable<LadderUnitModel> units = Core.Children.SelectRange(0, GlobalSetting.LadderXCapacity - 1, y, y);
                    units = units.Concat(Core.VLines.SelectRange(0, GlobalSetting.LadderXCapacity - 1, y, y));
                    foreach (LadderUnitModel unit in units)
                    {
                        if (unit.View == null)
                        {
                            unit.View = LadderUnitViewModel.Create(unit);
                            LadderCanvas.Children.Add(unit.View);
                        }
                    }
                });
            }
        }

        private void DisposeRange(int rowstart, int rowend)
        {
            int dir = (rowstart < rowend ? 1 : -1);
            for (int y = rowstart; y != rowend + dir; y += dir)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    IEnumerable<LadderUnitModel> units = Core.Children.SelectRange(0, GlobalSetting.LadderXCapacity - 1, y, y);
                    units = units.Concat(Core.VLines.SelectRange(0, GlobalSetting.LadderXCapacity - 1, y, y));
                    foreach (LadderUnitModel unit in units)
                    {
                        if (unit.View != null)
                        {
                            LadderCanvas.Children.Remove(unit.View);
                            //AllResourceManager.Dispose(unit.View);
                            //unit.View = null;
                            unit.View.Dispose();
                        }
                    }
                });
            }
        }

        #endregion

        public void Update()
        {
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("ID"));
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("Brief"));
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("Description"));
            if (!IsExpand)
            {
                ReleaseSelectRect();
                if (IsSelectAreaMode && !IsSelectAllMode) IsSelectAreaMode = false;
                LadderCanvas.Height = 0;
                LadderCanvas.Children.Clear();
                if (ThumbnailButton.ToolTip == null)
                {
                    ThumbnailButton.ToolTip = GenerateToolTipByLadder();
                    if (ThumbnailButton.Parent is Grid)
                        ((Grid)(ThumbnailButton.Parent)).Children.Remove(ThumbnailButton);
                    CommentAreaGrid.Children.Add(ThumbnailButton);
                }
            }
            else
            {
                if (ThumbnailButton.ToolTip != null)
                {
                    RemoveToolTipByLadder((ToolTip)ThumbnailButton.ToolTip);
                    ThumbnailButton.ToolTip = null;
                    CommentAreaGrid.Children.Remove(ThumbnailButton);
                }
                LadderCanvas.Height = RowCount * HeightUnit;
                if (IsSelectAreaMode && !LadderCanvas.Children.Contains(SelectArea))
                    LadderCanvas.Children.Add(SelectArea);
            }
        }
        
        public LadderModes LadderMode { get { return core.LadderMode; } }

        public LadderEditMenu CMEdit
        {
            get
            {
                return LadderCanvas.ContextMenu is LadderEditMenu
                    ? (LadderEditMenu)(LadderCanvas.ContextMenu) : null;
            }
            set
            {
                if (CMEdit == value) return;
                CMMoni = null;
                LadderEditMenu _CMEdit = CMEdit;
                LadderCanvas.ContextMenu = null;
                if (_CMEdit != null)
                {
                    _CMEdit.Post -= OnLadderNetworkEdit;
                    if (_CMEdit.Parent != null) _CMEdit.Parent = null;
                }
                LadderCanvas.ContextMenu = value;
                _CMEdit = CMEdit;
                if (_CMEdit != null)
                {
                    _CMEdit.Post += OnLadderNetworkEdit;
                    if (_CMEdit.Parent != this) _CMEdit.Parent = this;
                }
            }
        }

        public LadderMonitorMenu CMMoni
        {
            get
            {
                return LadderCanvas.ContextMenu is LadderMonitorMenu
                    ? (LadderMonitorMenu)(LadderCanvas.ContextMenu) : null;
            }
            set
            {
                if (CMMoni == value) return;
                CMEdit = null;
                LadderMonitorMenu _CMMoni = CMMoni;
                LadderCanvas.ContextMenu = null;
                if (_CMMoni != null && _CMMoni.Parent != null) _CMMoni.Parent = null;
                LadderCanvas.ContextMenu = value;
                _CMMoni = CMMoni;
                if (_CMMoni != null && _CMMoni.Parent != this) _CMMoni.Parent = this;
            }
        }
        
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
                foreach (LadderUnitModel unit in Core.Children)
                    if (unit.View != null) unit.View.IsCommentMode = iscommentmode;
                LadderCanvas.Height = HeightUnit * RowCount;
                if (IsSelectAreaMode || IsSelectAllMode)
                {
                    SelectAreaFirstX = SelectAreaFirstX;
                    SelectAreaFirstY = SelectAreaFirstY;
                    SelectAreaSecondX = SelectAreaSecondX;
                    SelectAreaSecondY = SelectAreaSecondY;
                }
            }
        }

        //private bool ismasked;
        public bool IsMasked
        {
            get { return Core.IsMasked; }
            set { Core.IsMasked = value; }
        }
        
        #endregion

        #region Event Handler
        
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
        
        private void OnLadderNetworkEdit(object sender, LadderEditEventArgs e)
        {
            switch (e.Type)
            {
                case LadderEditEventArgs.Types.Delete:
                    ViewParent.Delete();
                    break;
                case LadderEditEventArgs.Types.RowInsertBefore:
                    if (IsSingleSelected())
                        Core.Parent.AddR(Core, ViewParent.SelectionRect.Y);
                    else if (IsSelectAreaMode)
                        Core.Parent.AddR(Core, Math.Min(SelectAreaFirstY, SelectAreaSecondY));
                    break;
                case LadderEditEventArgs.Types.RowInsertAfter:
                    if (IsSingleSelected())
                        Core.Parent.AddR(Core, ViewParent.SelectionRect.Y + 1);
                    else if (IsSelectAreaMode)
                        Core.Parent.AddR(Core, Math.Max(SelectAreaFirstY, SelectAreaSecondY) + 1);
                    break;
                case LadderEditEventArgs.Types.RowInsertEnd:
                    Core.Parent.AddR(Core, RowCount);
                    break;
                case LadderEditEventArgs.Types.RowDelete:
                    if (IsSingleSelected())
                        Core.Parent.RemoveR(Core, ViewParent.SelectionRect.Y);
                    else if (IsSelectAreaMode)
                        Core.Parent.RemoveR(Core, Math.Min(SelectAreaFirstY, SelectAreaSecondY), Math.Max(SelectAreaFirstY, SelectAreaSecondY));
                    break;
                case LadderEditEventArgs.Types.NetInsertBefore:
                    Core.Parent.AddN(Core.ID);
                    break;
                case LadderEditEventArgs.Types.NetInsertAfter:
                    Core.Parent.AddN(Core.ID + 1);
                    break;
                case LadderEditEventArgs.Types.NetInsertEnd:
                    Core.Parent.AddN(Core.Parent.NetworkCount);
                    break;
                case LadderEditEventArgs.Types.NetDelete:
                    Core.Parent.RemoveN(Core.ID, Core);
                    break;
                case LadderEditEventArgs.Types.NetCopy:
                    Core.CopyToClipboard();
                    break;
                case LadderEditEventArgs.Types.NetCut:
                    Core.CopyToClipboard();
                    Core.Parent.RemoveN(Core.ID, Core);
                    break;
                case LadderEditEventArgs.Types.NetShield:
                    Core.IsMasked = !Core.IsMasked;
                    break;
            }
        }

        private void OnEditComment(object sender, RoutedEventArgs e)
        {
            IFParent.ShowEditNetworkCommentDialog(Core);
        }
        
        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (LadderMode)
            {
                case LadderModes.Edit:
                    CMEdit = ViewParent.CMEdit;
                    break;
                default:
                    CMMoni = ViewParent.CMMoni;
                    CMMoni.Core = ViewParent.SelectionRect.Current;
                    break;
            }
            LadderCanvas.CaptureMouse();
            AcquireSelectRect(e);
            if (e.ClickCount == 2)
            {
                LadderUnitModel unit = ViewParent.SelectionRect.Current;
                if (LadderMode == LadderModes.Edit)
                {
                    if (unit == null || unit.Shape == LadderUnitModel.Shapes.HLine || unit.Shape == LadderUnitModel.Shapes.Special)
                        IFParent.ShowInstructionInputDialog("", ViewParent.SelectionRect.Core);
                    else
                        IFParent.ShowElementPropertyDialog(unit);
                }
                else if (unit != null)
                {
                    IFParent.ShowValueModifyDialog(unit.UniqueChildren);
                }
            }
        }
        private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            LadderCanvas.ReleaseMouseCapture();
        }
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            ProjectTreeViewItem ptvitem = new ProjectTreeViewItem(null);
            if (e.Data.GetDataPresent(ptvitem.GetType()))
            {
                ptvitem = (ProjectTreeViewItem)(e.Data.GetData(ptvitem.GetType()));
                if (ptvitem.RelativeObject is LadderUnitModel.Types
                 || ptvitem.RelativeObject is FuncModel
                 || ptvitem.RelativeObject is LadderDiagramModel
                 || ptvitem.RelativeObject is ModbusModel)
                {
                    if (ladderExpander.IsExpand && !IsMasked)
                        AcquireSelectRect(e);
                }
            }
            double scaleX = GlobalSetting.LadderScaleTransform.ScaleX;
            double scaleY = GlobalSetting.LadderScaleTransform.ScaleY;
            var point = e.GetPosition(ViewParent.MainScrollViewer);
            if (ViewParent.MainScrollViewer.ViewportHeight - point.Y < 100 * scaleY)
                ViewParent.MainScrollViewer.ScrollToVerticalOffset(ViewParent.MainScrollViewer.VerticalOffset + 80 * scaleX);
            else if (point.Y < 100 * scaleY)
                ViewParent.MainScrollViewer.ScrollToVerticalOffset(ViewParent.MainScrollViewer.VerticalOffset - 80 * scaleY);
            else if (point.X < 100 * scaleX)
                ViewParent.MainScrollViewer.ScrollToHorizontalOffset(ViewParent.MainScrollViewer.HorizontalOffset - 80 * scaleY);
            else if (ViewParent.MainScrollViewer.ViewportWidth - point.X < 100 * scaleX)
                ViewParent.MainScrollViewer.ScrollToHorizontalOffset(ViewParent.MainScrollViewer.HorizontalOffset + 80 * scaleX);
            //e.Handled = true;
        }
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (!ladderExpander.IsExpand || IsMasked) return;
            ProjectTreeViewItem ptvitem = new ProjectTreeViewItem(null);
            bool isacquired = AcquireSelectRect(e);
            if (e.Data.GetDataPresent(typeof(LadderNetworkViewModel)))
            {
                isacquired = false;
                ReleaseSelectRect();
            }
            if (!isacquired) return;
            if (e.Data.GetDataPresent(ptvitem.GetType()))
            {
                ptvitem = (ProjectTreeViewItem)(e.Data.GetData(ptvitem.GetType()));
                if (ptvitem.RelativeObject is LadderUnitModel.Types)
                {
                    LadderUnitModel.Types type = (LadderUnitModel.Types)(ptvitem.RelativeObject);
                    Core.Parent.QuickInsertElement(type, ViewParent.SelectionRect.Core, false);
                }
                else if (ptvitem.RelativeObject is FuncModel)
                {
                    FuncModel fmodel = (FuncModel)(ptvitem.RelativeObject);
                    if (!fmodel.CanCALLM())
                    {
                        LocalizedMessageBox.Show(String.Format("{0:s}{1}", fmodel.Name, Properties.Resources.Message_Can_Not_CALL), LocalizedMessageIcon.Error);
                        return;
                    }
                    LadderUnitModel unit = new LadderUnitModel(null, fmodel);
                    unit.X = ViewParent.SelectionRect.X;
                    unit.Y = ViewParent.SelectionRect.Y;
                    Core.Parent.AddSingleUnit(unit, Core, false);
                }
                else if (ptvitem.RelativeObject is LadderDiagramModel)
                {
                    LadderDiagramModel ldmodel = (LadderDiagramModel)(ptvitem.RelativeObject);
                    LadderUnitModel unit = new LadderUnitModel(null, LadderUnitModel.Types.CALL);
                    unit.InstArgs = new string[] { ldmodel.Name };
                    unit.X = ViewParent.SelectionRect.X;
                    unit.Y = ViewParent.SelectionRect.Y;
                    Core.Parent.AddSingleUnit(unit, Core, false);
                }
                else if (ptvitem.RelativeObject is ModbusModel)
                {
                    ModbusModel mmodel = (ModbusModel)(ptvitem.RelativeObject);
                    LadderUnitModel unit = new LadderUnitModel(null, LadderUnitModel.Types.MBUS);
                    unit.InstArgs = new string[] { "K285", mmodel.Name, "D0" };
                    unit.X = ViewParent.SelectionRect.X;
                    unit.Y = ViewParent.SelectionRect.Y;
                    Core.Parent.AddSingleUnit(unit, Core, false);
                }
            }
        }

        #endregion
        
    }
}

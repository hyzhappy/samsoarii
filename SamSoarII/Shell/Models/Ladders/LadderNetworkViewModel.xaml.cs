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
    public partial class LadderNetworkViewModel : UserControl, ILoadModel, INotifyPropertyChanged, IComparable<LadderNetworkViewModel>
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
            {
                LadderMode = ViewParent.LadderMode;
                IsCommentMode = ViewParent.IsCommentMode;
            }
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
                this.core = value;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    _core.ChildrenChanged -= OnCoreChildrenChanged;
                    if (_core.View != null) _core.View = null;
                }
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
        public InteractionFacade IFParent { get { return core.Parent.Parent.Parent; } }

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RowCount":
                    if (IsSingleSelected())
                        ViewParent.SelectionRect.Y = Math.Min(ViewParent.SelectionRect.Y, RowCount - 1);
                    LadderCanvas.Height = RowCount * HeightUnit;
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
                    if (sender.View == null)
                        sender.View = LadderUnitViewModel.Create(sender);
                    if (sender.View.Parent is Canvas)
                        ((Canvas)(sender.View.Parent)).Children.Remove(sender.View);
                    LadderCanvas.Children.Add(sender.View);
                    break;
                case LadderUnitAction.REMOVE:
                    LadderCanvas.Children.Remove(sender.View);
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

        public LadderDiagramViewModel ViewParent { get { return core?.Parent.View; } }
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
            if (ViewParent.SelectionRect.Core.Parent == Core)
                ViewParent.SelectionRect.Core.Parent = null;
        }
        public void AcquireSelectRect()
        {
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

        #endregion

        #region Load
        
        public bool IsFullLoaded
        {
            get
            {
                foreach (LadderUnitModel unit in core.Children)
                    if (unit.View == null) return false;
                return true;
            }
        }

        public ViewThreadManager ViewThread { get { return Core.Parent.Parent.Parent.ThMNGView; } }

        public IEnumerable<ILoadModel> LoadChildren { get { return new ILoadModel[] { }; } }
        
        public void FullLoad()
        {
            for (int y = 0; y < core.RowCount; y++)
            { 
                Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    foreach (LadderUnitModel unit in core.Children.SelectRange(0, GlobalSetting.LadderXCapacity, y, y))
                    {
                        if (unit.View == null) unit.View = LadderUnitViewModel.Create(unit);
                        if (!ViewThread.ThAlive || !ViewThread.ThActive) break;
                    }
                });
                if (!ViewThread.ThAlive || !ViewThread.ThActive) break;
                Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                {
                    foreach (LadderUnitModel unit in core.VLines.SelectRange(0, GlobalSetting.LadderXCapacity, y, y))
                    {
                        if (unit.View == null) unit.View = LadderUnitViewModel.Create(unit);
                        if (!ViewThread.ThAlive || !ViewThread.ThActive) break;
                    }
                });
                if (!ViewThread.ThAlive || !ViewThread.ThActive) break;
            }
        }

        public void UpdateFullLoadProgress()
        {
            NetworkNumberLabel.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                NetworkNumberLabel.Content = String.Format("{0:s}({1:d}%)", NetworkNumber, Core.Children.Where(u => u.View != null).Count() * 100 / Core.Children.Count());
            });
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
        
        private ToolTip GenerateToolTipByLadder()
        {
            ToolTip tooltip = new ToolTip();
            ScrollViewer scroll = new ScrollViewer();
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Canvas canvas = new Canvas();
            scroll.MaxHeight = 385;
            canvas.Background = Brushes.White;
            canvas.HorizontalAlignment = HorizontalAlignment.Left;
            ScaleTransform transform = new ScaleTransform(GlobalSetting.LadderOriginScaleX / 1.7, GlobalSetting.LadderOriginScaleY / 1.7);
            canvas.Height = Core.RowCount * HeightUnit;
            canvas.Width = LadderCanvas.Width;
            foreach (var ele in Core.Children)
            {
                canvas.Children.Add(ele.View);
            }
            foreach (var ele in Core.VLines)
            {
                canvas.Children.Add(ele.View);
            }
            canvas.LayoutTransform = transform;
            scroll.Content = canvas;
            tooltip.Content = scroll;
            return tooltip;
        }

        private void RemoveToolTipByLadder(ToolTip tooltip)
        {
            if (tooltip != null)
            {
                Canvas canvas = (Canvas)((ScrollViewer)tooltip.Content).Content;
                canvas.LayoutTransform = null;
                canvas.Children.Clear();
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
            {
                AcquireSelectRect();
            }
            return true;
        }

        #endregion

        public void Update()
        {
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("ID"));
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("Brief"));
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("Description"));
            if (!IsFullLoaded)
            {
                CommentAreaExpander.Background = Brushes.Gray;
                CommentAreaExpander.Opacity = 0.4;
                ladderExpander.Visibility = Visibility.Collapsed;
                ladderExpander.IsEnabled = false;
                ThumbnailButton.Visibility = Visibility.Hidden;
                ThumbnailButton.IsEnabled = false;
            }
            else
            {
                OnCorePropertyChanged(this, new PropertyChangedEventArgs("IsMasked"));
                OnCorePropertyChanged(this, new PropertyChangedEventArgs("RowCount"));
                ladderExpander.Visibility = Visibility.Visible;
                ladderExpander.IsEnabled = true;
                ThumbnailButton.Visibility = Visibility.Visible;
                ThumbnailButton.IsEnabled = true;
            }
            if (!IsFullLoaded || !IsExpand)
            {
                ReleaseSelectRect();
                LadderCanvas.Height = 0;
                LadderCanvas.Children.Clear();
                if (IsFullLoaded && !IsExpand && ThumbnailButton.ToolTip == null)
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
                LadderCanvas.Children.Clear();
                foreach (LadderUnitModel unit in Core.Children.Concat(Core.VLines))
                    LadderCanvas.Children.Add(unit.View);
                if (IsSelectAreaMode)
                    LadderCanvas.Children.Add(SelectArea);
            }
        }
        
        private LadderModes laddermode;
        public LadderModes LadderMode
        {
            get
            {
                return this.laddermode;
            }
            set
            {
                this.laddermode = value;
                foreach (LadderUnitModel unit in Core.Children)
                    if (unit.View != null) unit.View.LadderMode = laddermode;
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
            }
        }

        //private bool ismasked;
        public bool IsMasked
        {
            get
            {
                return Core.IsMasked;
            }
            set
            {
                Core.IsMasked = value;
            }
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
        
        private void OnDeleteElement(object sender, RoutedEventArgs e)
        {
            ViewParent.Delete();
        }
        private void OnAddNewRowBefore(object sender, RoutedEventArgs e)
        {
            if (IsSingleSelected())
                Core.Parent.AddR(Core, ViewParent.SelectionRect.Y);
            if (IsSelectAreaMode)
                Core.Parent.AddR(Core, Math.Min(SelectAreaFirstY, SelectAreaSecondY));
        }
        private void OnAddNewRowAfter(object sender, RoutedEventArgs e)
        {
            if (IsSingleSelected())
                Core.Parent.AddR(Core, ViewParent.SelectionRect.Y + 1);
            if (IsSelectAreaMode)
                Core.Parent.AddR(Core, Math.Max(SelectAreaFirstY, SelectAreaSecondY) + 1);
        }
        private void OnDeleteRow(object sender, RoutedEventArgs e)
        {
            if (IsSingleSelected())
                Core.Parent.RemoveR(Core, ViewParent.SelectionRect.Y);
            if (IsSelectAreaMode)
                Core.Parent.RemoveR(Core, Math.Min(SelectAreaFirstY, SelectAreaSecondY), Math.Max(SelectAreaFirstY, SelectAreaSecondY));
        }
        private void OnAppendNewRow(object sender, RoutedEventArgs e)
        {
            Core.Parent.AddR(Core, RowCount);
        }
        private void OnAppendNewNetwork(object sender, RoutedEventArgs e)
        {
            Core.Parent.AddN(Core.Parent.NetworkCount);
        }
        private void OnAddNewNetworkBefore(object sender, RoutedEventArgs e)
        {
            Core.Parent.AddN(Core.ID);
        }
        private void OnAddNewNetworkAfter(object sender, RoutedEventArgs e)
        {
            Core.Parent.AddN(Core.ID + 1);
        }
        private void OnRemoveNetwork(object sender, RoutedEventArgs e)
        {
            Core.Parent.RemoveN(Core.ID, Core);
        }
        private void OnEditComment(object sender, RoutedEventArgs e)
        {
            IFParent.ShowEditNetworkCommentDialog(Core);
        }
        
        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
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
            e.Handled = true;
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
                    Core.Parent.QuickInsertElement(type, ViewParent.SelectionRect.Core);
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

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
            ladderExpander.MouseEnter += OnExpanderMouseEnter;
            ladderExpander.MouseLeave += OnExpanderMouseLeave;
            ladderExpander.expandButton.IsExpandChanged += OnExpandChanged;
            ladderExpander.IsExpand = IsExpand;
            CommentAreaGrid.Children.Remove(ThumbnailButton);
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
                    if (IsMultiSelected())
                    {
                        if (ViewParent.SelectionArea.Core.YStart >= RowCount)
                            ReleaseSelectRect();
                        else
                        {
                            while (ViewParent.SelectionArea.Core.YEnd >= RowCount)
                                ViewParent.SelectionArea.Core.MoveUp();
                        }
                    }
                    LadderCanvas.Height = RowCount * HeightUnit;
                    DynamicUpdate();
                    PropertyChanged(this, new PropertyChangedEventArgs("RowCount"));
                    break;
                case "IsExpand":
                    ladderExpander.IsExpand = IsExpand;
                    if (!IsExpand)
                    {
                        ReleaseSelectRect();
                        LadderCanvas.Height = 0;
                        DynamicDispose(true);
                        if (ThumbnailButton.ToolTip == null)
                        {
                            ThumbnailButton.ToolTip = new ToolTip();
                            if (ThumbnailButton.Parent is Grid)
                                ((Grid)(ThumbnailButton.Parent)).Children.Remove(ThumbnailButton);
                            CommentAreaGrid.Children.Add(ThumbnailButton);
                        }
                    }
                    else
                    {
                        if (ThumbnailButton.ToolTip != null)
                        {
                            ThumbnailButton.ToolTip = null;
                            CommentAreaGrid.Children.Remove(ThumbnailButton);
                        }
                        LadderCanvas.Height = RowCount * HeightUnit;
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("RowCount"));
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExpand"));
                    ViewParent.IsViewModified = true;
                    break;
                case "IsBriefExpand":
                    CommentAreaExpander.IsExpanded = core.IsBriefExpand;
                    break;
                case "IsMasked":
                    if (IsMasked)
                    {
                        ReleaseSelectRect();
                        CommentAreaExpander.Background = Brushes.LightGray;
                        LadderCanvas.Background = Brushes.LightGray;
                        CommentAreaExpander.Opacity = 0.4;
                        LadderCanvas.Opacity = 0.4;
                        Canvas.SetZIndex(this, -2);
                    }
                    else
                    {
                        CommentAreaExpander.Opacity = 1.0;
                        LadderCanvas.Opacity = 1.0;
                        CommentAreaExpander.Background = Brushes.LightCyan;
                        LadderCanvas.Background = Brushes.Transparent;
                        Canvas.SetZIndex(this, 0);
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("IsMasked"));
                    break;
                case "IsCommentMode":
                    LadderCanvas.Height = HeightUnit * RowCount;
                    break;
                case "CanvasTop":
                    Canvas.SetTop(this, core.CanvasTop);
                    break;
                case "ID":
                    NetworkNumberLabel.Content = NetworkNumber;
                    break;
                case "Brief":
                    NetworkBriefLabel.Content = NetworkBrief;
                    break;
                case "Description":
                    NetworkDescriptionTextBlock.Text = NetworkDescription;
                    break;
            }
        }
        
        private void OnCoreChildrenChanged(LadderUnitModel sender, LadderUnitChangedEventArgs e)
        {
            if (!IsExpand) IsExpand = true;
            switch (e.Action)
            {
                case LadderUnitAction.ADD:
                    if (sender.Y < loadedrowstart || sender.Y > loadedrowend)
                        break;
                    if (sender.View == null)
                        sender.View = LadderUnitViewModel.Create(sender);
                    if (sender.View.Parent != ViewParent.MainCanvas)
                    {
                        if (sender.View.Parent is Canvas)
                            ((Canvas)(sender.View.Parent)).Children.Remove(sender.View);
                        ViewParent.MainCanvas.Children.Add(sender.View);
                    }
                    break;
                case LadderUnitAction.REMOVE:
                    if (sender.View == null) break;
                    sender.View.Visibility = Visibility.Hidden;
                    sender.View.Dispose();
                    break;
                case LadderUnitAction.MOVE:
                    if (sender.Y >= loadedrowstart && sender.Y <= loadedrowend)
                    {
                        if (sender.View == null)
                            sender.View = LadderUnitViewModel.Create(sender);
                        if (!LadderCanvas.Children.Contains(sender.View))
                            LadderCanvas.Children.Add(sender.View);
                    }
                    else
                    {
                        if (sender.View != null)
                        {
                            LadderCanvas.Children.Remove(sender.View);
                            sender.View.Dispose();
                        }
                    }
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
        
        #region Select

        public int WidthUnit { get { return GlobalSetting.LadderWidthUnit; } }
        public int HeightUnit { get { return IsCommentMode ? GlobalSetting.LadderCommentModeHeightUnit : GlobalSetting.LadderHeightUnit; } }
        
        public bool IsSingleSelected()
        {
            return ViewParent?.SelectionRect != null && ViewParent.SelectionRect.Core.Parent == Core;
        }

        public bool IsMultiSelected()
        {
            if (ViewParent?.SelectionArea == null)
                return false;
            switch (ViewParent.SelectionArea.Core.State)
            {
                case SelectAreaCore.Status.SelectRange:
                    return ViewParent.SelectionArea.Core.NetOrigin == core.ID;
                default:
                    return false;
            }
        }
        
        public void ReleaseSelectRect()
        {
            if (ViewParent?.SelectionRect == null
             || ViewParent?.SelectionArea == null)
            {
                return;
            }
            if (ViewParent.SelectionRect.Core.Parent == Core
             || ViewParent.SelectionArea.Core.State == SelectAreaCore.Status.SelectRange
             && ViewParent.SelectionArea.Core.NetOrigin == core.ID)
            {
                ViewParent.ReleaseSelect();
            }
        }

        public void AcquireSelectRect()
        {
            if (ViewParent?.SelectionRect == null
             || ViewParent?.SelectionArea == null)
            {
                return;
            }
            ViewParent.SelectionArea.Core.Release();
            if (ViewParent.SelectionRect.Core.Parent != Core)
                ViewParent.SelectionRect.Core.Parent = Core;
            ViewParent.SelectionStatus = SelectStatus.SingleSelected;
        }
        
        #endregion
        
        #region Expand
        
        //private bool isexpand;
        public bool IsExpand
        {
            get { return Core.IsExpand; }
            set { Core.IsExpand = value; }
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
            double newscrolloffset = 0;
            double titleheight = 0;
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                if (ViewParent == null) return;
                scaleY = GlobalSetting.LadderScaleTransform.ScaleY;
                scroll = ViewParent?.Scroll;
                if (scroll == null) return;
                newscrolloffset = scroll.VerticalOffset;
                titleheight = ViewParent.TopBorder;
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
                _loadedrowstart = Math.Max(_loadedrowstart, 
                    (int)(((newscrolloffset - titleheight) / scaleY - core.UnitBaseTop) / HeightUnit) 
                    - (newscrolloffset < oldscrolloffset ? 3 : 1));
                _loadedrowend = Math.Min(_loadedrowend, 
                    (int)(((newscrolloffset - titleheight + scroll.ViewportHeight) / scaleY - core.UnitBaseTop) / HeightUnit) 
                    + (newscrolloffset > oldscrolloffset ? 3 : 1));
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

        public void DynamicDispose(bool hide = true)
        {
            if (loadedrowstart <= loadedrowend)
            {
                DisposeRange(loadedrowstart, loadedrowend, hide);
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
                            if (unit.View.Parent != ViewParent.MainCanvas)
                            {
                                if (unit.View.Parent is Canvas)
                                    ((Canvas)(unit.View.Parent)).Children.Remove(unit.View);
                                ViewParent.MainCanvas.Children.Add(unit.View);
                            }
                        }
                    }
                });
            }
        }

        private void DisposeRange(int rowstart, int rowend, bool hide = true)
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
                            if (hide) unit.View.Visibility = Visibility.Hidden;
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
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("RowCount"));
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("IsExpand"));
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("IsBriefExpand"));
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("IsMasked"));
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("IsCommentMode"));
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("CanvasTop"));
            OnCorePropertyChanged(this, new PropertyChangedEventArgs("UnitBaseTop"));
        }
        
        public LadderModes LadderMode { get { return core.LadderMode; } }
        public bool IsCommentMode { get { return core.IsCommentMode; } }
        
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
            ViewParent.Outline.Core = null;
            ((ToolTip)(ThumbnailButton.ToolTip)).Content = null;

        }
        private void ThumbnailButton_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            _canScrollToolTip = true;
            ViewParent.Outline.Core = Core;
            ((ToolTip)(ThumbnailButton.ToolTip)).Content = ViewParent.Outline;
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
                ScrollViewer scroll = ((NetworkOutlineViewModel)(((ToolTip)ThumbnailButton.ToolTip).Content)).Scroll;
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset - e.Delta / 10);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (_canScrollToolTip) e.Handled = true;
            base.OnMouseWheel(e);
        }

        #endregion

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (ViewParent != null) ViewParent.IsViewModified = true;
        }

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
                    else if (IsMultiSelected())
                        Core.Parent.AddR(Core, ViewParent.SelectionArea.Core.YStart);
                    break;
                case LadderEditEventArgs.Types.RowInsertAfter:
                    if (IsSingleSelected())
                        Core.Parent.AddR(Core, ViewParent.SelectionRect.Y + 1);
                    else if (IsMultiSelected())
                        Core.Parent.AddR(Core, ViewParent.SelectionArea.Core.YEnd + 1);
                    break;
                case LadderEditEventArgs.Types.RowInsertEnd:
                    Core.Parent.AddR(Core, RowCount);
                    break;
                case LadderEditEventArgs.Types.RowDelete:
                    if (IsSingleSelected())
                        Core.Parent.RemoveR(Core, ViewParent.SelectionRect.Y);
                    else if (IsMultiSelected())
                        Core.Parent.RemoveR(Core, ViewParent.SelectionArea.Core.YStart, ViewParent.SelectionArea.Core.YEnd);
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
        /*
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
                    //if (ladderExpander.IsExpand && !IsMasked)
                    //    AcquireSelectRect(e);
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
                    //Core.Parent.QuickInsertElement(type, ViewParent.SelectionRect.Core, false);
                    if (IFParent.ShowElementPropertyDialog(type, ViewParent.SelectionRect.Core, false))
                        core.Parent.View.SelectRectRight();
                }
                else if (ptvitem.RelativeObject is FuncModel)
                {
                    FuncModel fmodel = (FuncModel)(ptvitem.RelativeObject);
                    if (!fmodel.CanCALLM())
                    {
                        LocalizedMessageBox.Show(String.Format("{0:s}{1}", fmodel.Name, Properties.Resources.Message_Can_Not_CALL), LocalizedMessageIcon.Error);
                        return;
                    }
                    IFParent.ShowElementPropertyDialog(fmodel, ViewParent.SelectionRect.Core);
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
                    IFParent.ShowElementPropertyDialog(mmodel, ViewParent.SelectionRect.Core);
                }
            }
        }
        */
        private void CommentAreaExpander_Expanded(object sender, RoutedEventArgs e)
        {
            core.IsBriefExpand = true;
        }

        private void CommentAreaExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            core.IsBriefExpand = false;
        }

        #endregion
        
    }
}

using SamSoarII.Core.Generate;
using SamSoarII.Core.Models;
using SamSoarII.Threads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Threading;
using SamSoarII.Utility;
using SamSoarII.Global;

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// InstructionNetworkViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class InstructionNetworkViewModel : UserControl, IViewModel, IResource, INotifyPropertyChanged
    {
        #region IResource

        private int resourceid;
        public int ResourceID
        {
            get { return this.resourceid; }
            set { this.resourceid = value; }
        }

        public IResource Create(params object[] args)
        {
            return new InstructionNetworkViewModel((InstructionNetworkModel)args[0]);
        }

        public virtual void Recreate(params object[] args)
        {
            Visibility = Visibility.Visible;
            Core = (InstructionNetworkModel)args[0];
            loadedrowstart = 0;
            loadedrowend = -1;
            oldscrolloffset = 0;
            if (Core != null)
            {
                SetPosition(tberr, 0);
                tberr.Visibility = Visibility.Visible;
                if (tberr.Parent != ViewParent.MainCanvas)
                {
                    if (tberr.Parent is Canvas)
                        ((Canvas)(tberr.Parent)).Children.Remove(tberr);
                    ViewParent.MainCanvas.Children.Add(tberr);
                }
                Expander.IsExpand = core.IsExpand;
                BaseUpdate();
            }
        }

        #endregion

        public InstructionNetworkViewModel(InstructionNetworkModel _core)
        {
            InitializeComponent();
            DataContext = this;
            Expander.MouseEnter += OnExpanderMouseEnter;
            Expander.MouseLeave += OnExpanderMouseLeave;
            Expander.expandButton.IsExpandChanged += OnExpandChanged;
            children = new ObservableCollection<InstructionRowViewModel>();
            children.CollectionChanged += OnChildrenCollectionChanged;
            tberr = new TextBlock();
            tberr.Background = Brushes.Red;
            Recreate(_core);
        }
        
        public void Dispose()
        {
            Core = null;
            foreach (InstructionRowViewModel row in children.ToArray())
                children.Remove(row);
            tberr.Visibility = Visibility.Hidden;
            AllResourceManager.Dispose(this);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Core

        private InstructionNetworkModel core;
        public InstructionNetworkModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                InstructionNetworkModel _core = core;
                this.core = value;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    _core.ViewPropertyChanged -= OnCorePropertyChanged;
                    if (_core.View != null) _core.View = null;
                }
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    core.ViewPropertyChanged += OnCorePropertyChanged;
                    if (core.View != this) core.View = this;
                }
            }
        }
        IModel IViewModel.Core
        {
            get { return core; }
            set { Core = (InstructionNetworkModel)value; }
        }

        public ValueManager ValueManager { get { return Core != null ? Core.Parent.Parent.Parent.Parent.MNGValue : null; } }

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ID": PropertyChanged(this, new PropertyChangedEventArgs("Header")); break;
                case "IsMasked": BaseUpdate(); break;
                case "IsCommentMode":
                    foreach (InstructionRowViewModel row in children)
                        row.TextBlocks[7].Visibility = IsCommentMode
                            ? Visibility.Visible : Visibility.Hidden;
                    break;
                case "CanvasTop":
                    Canvas.SetTop(this, core.CanvasTop);
                    foreach (InstructionRowViewModel row in children)
                    {
                        for (int i = 0; i < row.TextBlocks.Count; i++)
                            SetPosition(row.TextBlocks[i], row.ID, i);
                        row.TextBlocks[7].Visibility = IsCommentMode
                            ? Visibility.Visible : Visibility.Hidden;
                    }
                    break;
                case "ViewHeight":
                    Expander.Height = core.ViewHeight / 0.4;
                    RN_Wait.Height = core.ViewHeight - 24;
                    if (Expander.IsMouseOver)
                        ViewParent.Rect.Height = core.ViewHeight;
                    break;
                case "IsExpand": BaseUpdate(); break;
                case "IsModified":
                    Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)(delegate ()
                    {
                        TB_Wait.Visibility = RN_Wait.Visibility = core.IsModified 
                            ? Visibility.Visible : Visibility.Hidden;
                        if (core.IsModified)
                        {
                            tberr.Visibility = Visibility.Hidden;
                            InstSelectRectCore cursor = core.Parent?.Parent?.Inst?.View?.Cursor?.Core;
                            if (cursor != null && cursor.Parent == core)
                                cursor.Parent = null;
                        }
                    }));
                    break;
            }
        }
        
        #endregion

        #region Shell

        public InstructionDiagramViewModel ViewParent { get { return core?.Parent?.Parent?.Inst?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        private TextBlock tberr;

        #region Binding

        public string Header { get { return String.Format("Network {0:d}", core != null ? core.Parent.ID : 0); } }

        public int RowCount { get { return !core.IsExpand ? 0 : core.Invalid ? 1 : core.Insts.Count; } }

        public bool Invalid { get { return core == null || Core.IsMasked || Core.IsOpenCircuit || Core.IsShortCircuit || Core.IsFusionCircuit; } }

        #endregion
        
        private void SetPosition(FrameworkElement ctrl, int row, int column = -1)
        {
            //ctrl.Margin = new Thickness(2, 2, 0, 0);
            Canvas.SetTop(ctrl, core.CanvasTop + 26 + row * 20 + 1);
            Canvas.SetZIndex(ctrl, -1);
            ctrl.Height = 18;
            switch (column)
            {
                case -1: Canvas.SetLeft(ctrl,24+1); ctrl.Width = 520; break;
                case 0: Canvas.SetLeft(ctrl, 24+1); ctrl.Width = 38; break;
                case 1: Canvas.SetLeft(ctrl, 24+41); ctrl.Width = 78; break;
                case 2: Canvas.SetLeft(ctrl, 24+121); ctrl.Width = 78; break;
                case 3: Canvas.SetLeft(ctrl, 24+201); ctrl.Width = 78; break;
                case 4: Canvas.SetLeft(ctrl, 24+281); ctrl.Width = 78; break;
                case 5: Canvas.SetLeft(ctrl, 24+361); ctrl.Width = 78; break;
                case 6: Canvas.SetLeft(ctrl, 24+441); ctrl.Width = 78; break;
                case 7: Canvas.SetLeft(ctrl, 24+521); ctrl.Width = 238; break;
            }
        }

        #region Dynamic

        private int loadedrowstart;
        public int LoadedRowStart { get { return this.loadedrowstart; } }

        private int loadedrowend;
        public int LoadedRowEnd { get { return this.loadedrowend; } }

        private double oldscrolloffset;

        private ObservableCollection<InstructionRowViewModel> children;
        public IList<InstructionRowViewModel> Children { get { return this.children; } }
        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (InstructionRowViewModel row in e.NewItems)
                {
                    for (int i = 0; i < row.TextBlocks.Count; i++)
                    {
                        SetPosition(row.TextBlocks[i], row.ID, i);
                        row.TextBlocks[i].Visibility = Visibility.Visible;
                        if (row.TextBlocks[i].Parent != ViewParent.MainCanvas)
                        {
                            if (row.TextBlocks[i].Parent is Canvas)
                                ((Canvas)(row.TextBlocks[i].Parent)).Children.Remove(row.TextBlocks[i]);
                            ViewParent.MainCanvas.Children.Add(row.TextBlocks[i]);
                        }
                    }
                    row.TextBlocks[7].Visibility = IsCommentMode
                        ? Visibility.Visible : Visibility.Hidden;
                }
            if (e.OldItems != null)
                foreach (InstructionRowViewModel row in e.OldItems)
                {
                    for (int i = 0; i < row.TextBlocks.Count; i++)
                        row.TextBlocks[i].Visibility = Visibility.Hidden;
                    row.Dispose();
                }
        }

        public void BaseUpdate()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                if (core == null) return;
                tberr.Visibility = core.IsExpand && core.Invalid ? Visibility.Visible : Visibility.Hidden;
                tberr.Background = Core.IsMasked ? Brushes.Gray : Brushes.Red;
                if (!core.IsExpand && ViewParent.Cursor.Core.Parent == Core)
                    ViewParent.Cursor.Core.Parent = null;
                if (Core.IsMasked)
                {
                    tberr.Text = String.Format(
                        App.CultureIsZH_CH() ? "Network {0:d} 已被屏蔽！" : "Network {0:d} has been masked!",
                        Core.ID);
                }
                else if (Core.IsOpenCircuit)
                {
                    tberr.Text = String.Format(
                        App.CultureIsZH_CH() ? "Network {0:d} 的梯形图存在断路错误！" : "There have broken circuit in ladder of Network {0:d}.",
                        Core.ID);
                }
                else if (Core.IsShortCircuit)
                {
                    tberr.Text = String.Format(
                        App.CultureIsZH_CH() ? "Network {0:d} 的梯形图存在短路错误！" : "There have short circuit in ladder of Network {0:d}.",
                        Core.ID);
                }
                else if (Core.IsFusionCircuit)
                {
                    tberr.Text = String.Format(
                        App.CultureIsZH_CH() ? "Network {0:d} 的梯形图存在混连错误！" : "There have fusion circuit in ladder of Network {0:d}.",
                        Core.ID);
                }
                else
                {
                    tberr.Text = "";
                }
            });
            DynamicDispose();
            DynamicUpdate();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)(delegate ()
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Header"));
                OnCorePropertyChanged(this, new PropertyChangedEventArgs("CanvasTop"));
                OnCorePropertyChanged(this, new PropertyChangedEventArgs("ViewHeight"));
                OnCorePropertyChanged(this, new PropertyChangedEventArgs("IsModified"));
            }));
            ViewParent.IsViewModified = true;
        }

        public void DynamicUpdate()
        {
            if (core.IsExpand && !Invalid)
            {
                ScrollViewer scroll = null;
                double newscrolloffset = 0;
                Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    scroll = ViewParent?.Scroll;
                    if (scroll == null) return;
                    newscrolloffset = scroll.VerticalOffset;
                });
                if (scroll == null) return;
                int _loadedrowstart = 0;
                int _loadedrowend = Core.Insts.Count - 1;
                _loadedrowstart = Math.Max(_loadedrowstart, (int)((newscrolloffset - (core.CanvasTop + 28)) / 20) - 3);
                _loadedrowend = Math.Min(_loadedrowend, (int)((newscrolloffset - (core.CanvasTop + 28) + scroll.ViewportHeight) / 20) + 3);
                if (_loadedrowstart > _loadedrowend)
                {
                    if (loadedrowstart <= loadedrowend)
                        DisposeRange(loadedrowstart, loadedrowend);
                }
                else if (loadedrowstart > _loadedrowend)
                {
                    CreateRange(_loadedrowstart, _loadedrowend);
                }
                else
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
                loadedrowstart = _loadedrowstart;
                loadedrowend = _loadedrowend;
                oldscrolloffset = newscrolloffset;
            }
            else
            {
                DynamicDispose();
            }
        }

        public void DynamicDispose()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                foreach (InstructionRowViewModel row in children.ToArray())
                    children.Remove(row);
                loadedrowstart = 0;
                loadedrowend = -1;
            });
        }

        private void CreateRange(int rowstart, int rowend)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                for (int i = rowstart; i <= rowend; i++)
                    children.Add(AllResourceManager.CreateInstRow(Core.Insts[i], i));
            });
        }

        private void DisposeRange(int rowstart, int rowend)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                foreach (InstructionRowViewModel row in children.Where(r => r.ID >= rowstart && r.ID <= rowend).ToArray())
                    children.Remove(row);
            });
        }

        #endregion
        
        public LadderModes LadderMode { get { return core.LadderMode; } }
        public bool IsCommentMode { get { return core.IsCommentMode; } }

        #endregion

        #region Event Handler
        
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            ViewParent.IsViewModified = true;
        }
        
        #region Expander

        private void OnExpanderMouseEnter(object sender, MouseEventArgs e)
        {
            Expander.Rect.Fill = GlobalSetting.FoldingBrush;
            Expander.Rect.Opacity = 0.2;
            Canvas.SetTop(ViewParent.Rect, core.CanvasTop);
            ViewParent.Rect.Height = core.ViewHeight;
            ViewParent.Rect.Visibility = Visibility.Visible;
        }

        private void OnExpanderMouseLeave(object sender, MouseEventArgs e)
        {
            Expander.Rect.Fill = Brushes.Transparent;
            Expander.Rect.Opacity = 1;
            ViewParent.Rect.Visibility = Visibility.Hidden;
        }

        private void OnExpandChanged(object sender, RoutedEventArgs e)
        {
            if (core != null)
                core.IsExpand = Expander.IsExpand;
        }

        #endregion

        #endregion
    }
}

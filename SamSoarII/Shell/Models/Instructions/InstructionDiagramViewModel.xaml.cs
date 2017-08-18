using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
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
using SamSoarII.Threads;
using System.Windows.Threading;
using System.Threading;

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// InstructionDiagramViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class InstructionDiagramViewModel : UserControl, IViewModel
    {
        public InstructionDiagramViewModel(InstructionDiagramModel _core)
        {
            InitializeComponent();
            DataContext = this;
            Core = _core;
            cursor = new InstSelectRect(new InstSelectRectCore(null));
            if (core.Parent?.View?.SelectionRect != null)
                cursor.Core.Ladder = core.Parent?.View?.SelectionRect.Core;
            MainCanvas.Children.Add(cursor);
            loadedstart = 0;
            loadedend = -1;
            Update();
        }

        public void Dispose()
        {
            Core = null;
            cursor.Dispose();
        }
        
        #region Core

        private InstructionDiagramModel core;
        public InstructionDiagramModel Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                InstructionDiagramModel _core = core;
                this.core = value;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    _core.ViewPropertyChanged -= OnCorePropertyChanged;
                    _core.ChildrenChanged -= OnCoreChildrenChanged;
                    if (_core.View != null) _core.View = null;
                }
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    core.ViewPropertyChanged += OnCorePropertyChanged;
                    core.ChildrenChanged += OnCoreChildrenChanged;
                    if (core.View != this) core.View = this;
                }
            }
        }

        InteractionFacade IFParent { get { return core.Parent?.Parent?.Parent; } }

        IModel IViewModel.Core
        {
            get { return core; }
            set { Core = (InstructionDiagramModel)value; }
        }

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ViewHeight":
                    MainCanvas.Height = core.ViewHeight;
                    LN_RV.Y2 = core.ViewHeight;
                    Canvas.SetTop(LN_LB, core.ViewHeight);
                    Canvas.SetTop(LN_RB, core.ViewHeight);
                    isviewmodified = true;
                    break;
            }
        }
        
        private void OnCoreChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (LadderNetworkModel lnmodel in e.OldItems)
                {
                    if (cursor.Core.Parent == lnmodel.Inst)
                        cursor.Core.Parent = null;
                    if (lnmodel.Inst.View != null)
                    {
                        lnmodel.Inst.View.Visibility = Visibility.Hidden;
                        lnmodel.Inst.View.DynamicDispose();
                        lnmodel.Inst.View.Dispose();
                    }
                }
            if (!Core.Parent.IsExecuting)
            {
                loadedstart = 0;
                loadedend = core.Children.Count() - 1;
                DynamicDispose();
                isviewmodified = true;
            }
        }

        #endregion

        #region Shell

        public ProjectViewModel ViewParent { get { return core?.Parent.Parent.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        #region Dynamic

        private int loadedstart;
        public int LoadedStart { get { return this.loadedstart; } }

        private int loadedend;
        public int LoadedEnd { get { return this.loadedend; } }

        private double oldscrolloffset;
        private InstructionNetworkModel[] inets;
        public void DynamicUpdate()
        {
            double newscrolloffset = 0;
            double scrollheight = 0;
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                newscrolloffset = Scroll.VerticalOffset;
                scrollheight = Scroll.ViewportHeight;
            });
            inets = core.Children.ToArray();
            int _loadedstart = 0;
            int _loadedend = inets.Length - 1;
            InstructionNetworkModel inet = inets[_loadedstart];
            while (inet != null && inet.CanvasTop + inet.ViewHeight - newscrolloffset < -20)
            {
                _loadedstart++;
                inet = _loadedstart < inets.Length ? inets[_loadedstart] : null;
            }
            inet = inets[_loadedend];
            while (inet != null && inet.CanvasTop - newscrolloffset > scrollheight + 20)
            {
                _loadedend--;
                inet = _loadedend >= 0 ? inets[_loadedend] : null;
            }
            if (_loadedstart > _loadedend)
            {
                if (loadedstart <= loadedend)
                {
                    if (newscrolloffset > oldscrolloffset)
                        DisposeRange(loadedstart, loadedend);
                    else
                        DisposeRange(loadedend, loadedstart);
                }
            }
            else if (loadedstart > loadedend)
            {
                if (newscrolloffset > oldscrolloffset)
                    CreateRange(_loadedstart, _loadedend);
                else
                    CreateRange(_loadedend, _loadedstart);
            }
            else
            {
                if (newscrolloffset > oldscrolloffset)
                {
                    if (_loadedstart < loadedstart)
                        CreateRange(_loadedstart, Math.Min(_loadedend, loadedstart - 1));
                    if (_loadedstart > loadedstart)
                        DisposeRange(loadedstart, _loadedstart - 1);
                    if (loadedend < _loadedend)
                        CreateRange(Math.Max(_loadedstart, loadedend + 1), _loadedend);
                    if (loadedend > _loadedend)
                        DisposeRange(_loadedend + 1, loadedend);
                }
                else
                {
                    if (_loadedstart < loadedstart)
                        CreateRange(Math.Min(_loadedend, loadedstart - 1), _loadedstart);
                    if (_loadedstart > loadedstart)
                        DisposeRange(_loadedstart - 1, loadedstart);
                    if (loadedend < _loadedend)
                        CreateRange(_loadedend, Math.Max(_loadedstart, loadedend + 1));
                    if (loadedend > _loadedend)
                        DisposeRange(loadedend, _loadedend + 1);
                }
            }
            loadedstart = _loadedstart;
            loadedend = _loadedend;
            oldscrolloffset = newscrolloffset;
            foreach (InstructionNetworkModel _inet in inets)
                if (_inet.View != null) _inet.View.DynamicUpdate();
        }

        public void DynamicDispose()
        {
            if (loadedstart <= loadedend)
            {
                inets = core.Children.ToArray();
                DisposeRange(loadedstart, loadedend);
                loadedstart = 0;
                loadedend = -1;
            }
        }

        public void CreateRange(int start, int end)
        {
            int dir = (start < end ? 1 : -1);
            for (int y = start; y != end + dir; y += dir)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    InstructionNetworkModel inet = inets[y];
                    if (inet.View == null)
                        inet.View = AllResourceManager.CreateINet(inet);
                    inet.View.Visibility = Visibility.Visible;
                    if (inet.View.Parent != MainCanvas)
                    {
                        if (inet.View.Parent is Canvas)
                            ((Canvas)(inet.View.Parent)).Children.Remove(inet.View);
                        MainCanvas.Children.Add(inet.View);
                    }
                });
            }
        }

        public void DisposeRange(int start, int end)
        {
            int dir = (start < end ? 1 : -1);
            for (int y = start; y != end + dir; y += dir)
            {
                if (y > inets.Length) continue;
                InstructionNetworkModel inet = inets[y];
                if (inet.View != null)
                {
                    inet.View.DynamicDispose();
                    Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        if (inet?.View == null) return;
                        inet.View.Visibility = Visibility.Hidden;
                        inet.View.Dispose();
                    });
                }
            }
        }
        
        #endregion

        public void Update()
        {
            core.UpdateCanvasTop();
        }
        
        public LadderModes LadderMode { get { return core.LadderMode; } }
        public bool IsCommentMode { get { return core.IsCommentMode; } }

        private bool isviewmodified;
        public bool IsViewModified
        {
            get { return this.isviewmodified; }
            set { this.isviewmodified = value; }
        }

        #region Select

        private InstSelectRect cursor;
        public new InstSelectRect Cursor { get { return this.cursor; } }
        
        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);
            foreach (InstructionNetworkModel instnet in core.Children)
            {
                if (p.Y < instnet.CanvasTop || p.Y > instnet.CanvasTop + instnet.ViewHeight) continue;
                if (p.X < 24 || p.X > 24 + 522) continue;
                if (instnet.IsModified)
                {
                    instnet.Update();
                    continue;
                }
                if (!instnet.IsExpand || instnet.Invalid) continue;
                cursor.IsNavigatable = false;
                cursor.Core.Parent = instnet;
                cursor.Core.Row = (int)((p.Y - instnet.CanvasTop - 26) / 20);
                cursor.IsNavigatable = true;
            }
            if (e.ChangedButton == MouseButton.Left && e.ClickCount >= 2
             && cursor.Core.Current?.Inst?.ProtoType != null
             && cursor.Core.Current.Inst.ProtoType.Children.Count > 0)
            {
                IFParent.ShowElementPropertyDialog(cursor.Core.Current.Inst.ProtoType);
            }
        }
        
        private void MainCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            InstructionNetworkModel instnet = null;
            if (cursor.Core.Parent == null) return;
            switch (e.Key)
            {
                case Key.Up:
                    e.Handled = true;
                    if (cursor.Core.Row <= 0)
                    {
                        int id = cursor.Core.Parent.Parent.ID - 1;
                        for ( ; id >= 0 ; id--)
                        {
                            instnet = Core.Parent.Children[id].Inst;
                            if (!instnet.IsExpand) continue;
                            if (instnet.Invalid) continue;
                            break;
                        }
                        if (id >= 0)
                        {
                            cursor.IsNavigatable = false;
                            cursor.Core.Parent = instnet;
                            cursor.Core.Row = instnet.Insts.Count - 1;
                            cursor.IsNavigatable = true;
                            cursor.Navigate();
                        }
                    }
                    else
                    {
                        cursor.Core.Row--;
                    }
                    break;
                case Key.Down:
                    e.Handled = true;
                    if (cursor.Core.Row >= cursor.Core.Parent.Insts.Count - 1)
                    {
                        int id = cursor.Core.Parent.Parent.ID + 1;
                        for (; id < Core.Parent.Children.Count; id++)
                        {
                            instnet = Core.Parent.Children[id].Inst;
                            if (!instnet.IsExpand) continue;
                            if (instnet.Invalid) continue;
                            break;
                        }
                        if (id < Core.Parent.Children.Count)
                        {
                            cursor.IsNavigatable = false;
                            cursor.Core.Parent = instnet;
                            cursor.Core.Row = 0;
                            cursor.IsNavigatable = true;
                            cursor.Navigate();
                        }
                    }
                    else
                    {
                        cursor.Core.Row++;
                    }
                    break;
                case Key.Enter:
                    e.Handled = true;
                    if (cursor.Core.Current?.Inst?.ProtoType != null
                     && cursor.Core.Current.Inst.ProtoType.Children.Count > 0)
                    {
                        IFParent.ShowElementPropertyDialog(cursor.Core.Current.Inst.ProtoType);
                    }
                    break;
            }
        }

        #endregion

        #endregion

        #region Event Handler
        
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            isviewmodified = true;
        }

        private void Scroll_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            isviewmodified = true;
        }
        
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Undo)
                e.CanExecute = Core?.Parent?.CanUndo == true;
            if (e.Command == ApplicationCommands.Redo)
                e.CanExecute = Core?.Parent?.CanRedo == true;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Undo)
                Core?.Parent?.Undo();
            if (e.Command == ApplicationCommands.Redo)
                Core?.Parent?.Redo();
        }

        #endregion

    }
}

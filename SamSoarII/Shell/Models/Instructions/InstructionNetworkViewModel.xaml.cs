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

namespace SamSoarII.Shell.Models
{
    /// <summary>
    /// InstructionNetworkViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class InstructionNetworkViewModel : UserControl, IViewModel
    {
        public InstructionNetworkViewModel(InstructionNetworkModel _core)
        {
            InitializeComponent();
            DataContext = this;
            Core = _core;

            children = new ObservableCollection<InstructionRowViewModel>();
            children.CollectionChanged += OnChildrenCollectionChanged;
            loadedrowstart = 0;
            loadedrowend = -1;
            oldscrolloffset = 0;

            tberr = new TextBlock();
            tberr.Background = Brushes.Red;
            SetPosition(tberr, 0);
            CV_Inst.Children.Add(tberr);

            BaseUpdate();
        }
        
        public void Dispose()
        {
            Core = null;

            foreach (InstructionRowViewModel row in children.ToArray())
                children.Remove(row);
            children.CollectionChanged -= OnChildrenCollectionChanged;
            children = null;

            CV_Inst.Children.Remove(tberr);
            tberr = null;

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
                    if (_core.View != null) _core.View = null;
                }
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
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
            }
        }
        
        #endregion

        #region Shell

        public InstructionDiagramViewModel ViewParent { get { return core?.Parent?.Parent?.Inst?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        private TextBlock tberr;

        #region Binding

        public string Header { get { return String.Format("Network {0:d}", core != null ? core.Parent.ID : 0); } }

        public int RowCount { get { return (int)(CV_Inst.Height / 20); } }

        public bool Invalid { get { return core == null || Core.IsMasked || Core.IsOpenCircuit || Core.IsShortCircuit || Core.IsFusionCircuit; } }

        #endregion
        
        private void SetPosition(FrameworkElement ctrl, int row, int column = -1)
        {
            Canvas.SetTop(ctrl, row * 20 + 1);
            ctrl.Height = 18;
            switch (column)
            {
                case -1: Canvas.SetLeft(ctrl, 1); ctrl.Width = 600; break;
                case 0: Canvas.SetLeft(ctrl, 1); ctrl.Width = 38; break;
                case 1: Canvas.SetLeft(ctrl, 41); ctrl.Width = 78; break;
                case 2: Canvas.SetLeft(ctrl, 121); ctrl.Width = 78; break;
                case 3: Canvas.SetLeft(ctrl, 201); ctrl.Width = 78; break;
                case 4: Canvas.SetLeft(ctrl, 281); ctrl.Width = 78; break;
                case 5: Canvas.SetLeft(ctrl, 361); ctrl.Width = 78; break;
                case 6: Canvas.SetLeft(ctrl, 441); ctrl.Width = 78; break;
                case 7: Canvas.SetLeft(ctrl, 521); ctrl.Width = 238; break;
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
                        CV_Inst.Children.Add(row.TextBlocks[i]);
                    }
                    row.TextBlocks[7].Visibility = iscommentmode
                        ? Visibility.Visible : Visibility.Hidden;
                }
            if (e.OldItems != null)
                foreach (InstructionRowViewModel row in e.OldItems)
                {
                    for (int i = 0; i < row.TextBlocks.Count; i++)
                        CV_Inst.Children.Remove(row.TextBlocks[i]);
                    row.Dispose();
                }
        }

        public void BaseUpdate()
        {
            tberr.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                CV_Inst.Height = Invalid ? 20 : Core.Insts.Count * 20;
                tberr.Visibility = Invalid ? Visibility.Visible : Visibility.Hidden;
                tberr.Background = Core.IsMasked ? Brushes.Gray : Brushes.Red;
                if (ViewParent.Cursor.Core.Parent == Core)
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
        }

        public void DynamicUpdate()
        {
            if (!Invalid)
            {
                ScrollViewer scroll = null;
                Point p = new Point();
                double newscrolloffset = 0;
                CV_Inst.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    scroll = ViewParent?.Scroll;
                    if (scroll == null) return;
                    p = CV_Inst.TranslatePoint(new Point(0, 0), scroll);
                    newscrolloffset = scroll.VerticalOffset;
                });
                if (scroll == null) return;
                int _loadedrowstart = 0;
                int _loadedrowend = Core.Insts.Count - 1;
                _loadedrowstart = Math.Max(_loadedrowstart, (int)(-p.Y / 20) - 3);
                _loadedrowend = Math.Min(_loadedrowend, (int)((-p.Y + scroll.ViewportHeight) / 20) + 3);
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
                //children.Clear();
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

                foreach (InstructionRowViewModel row in children)
                    row.TextBlocks[7].Visibility = iscommentmode
                        ? Visibility.Visible : Visibility.Hidden;
            }
        }

        #endregion

        #region Event Handler

        private void CV_Inst_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(CV_Inst);
            if (p.Y >= 0 && p.Y < 20 * core.Insts.Count)
            {
                ViewParent.Cursor.Core.Parent = Core;
                ViewParent.Cursor.Core.Row = (int)(p.Y / 20);
            }
        }

        #endregion
    }
}

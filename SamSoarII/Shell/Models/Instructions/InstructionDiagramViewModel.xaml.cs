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
    public partial class InstructionDiagramViewModel : UserControl, ILoadModel
    {
        public InstructionDiagramViewModel(InstructionDiagramModel _core)
        {
            InitializeComponent();
            DataContext = this;
            Core = _core;
        }

        public void Dispose()
        {
            Core = null;
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
            set { Core = (InstructionDiagramModel)value; }
        }

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
            }
        }
        
        private void OnCoreChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LadderNetworkModel net = null;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    net = (LadderNetworkModel)(e.NewItems[0]);
                    if (net.Inst.View == null)
                    {
                        net.Inst.View = new InstructionNetworkViewModel(net.Inst);
                        ViewThread.Add(net.Inst.View);
                    }
                    MainStack.Children.Insert(e.NewStartingIndex, net.Inst.View);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    net = (LadderNetworkModel)(e.OldItems[0]);
                    MainStack.Children.RemoveAt(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    net = (LadderNetworkModel)(e.NewItems[0]);
                    MainStack.Children[e.NewStartingIndex] = net.Inst.View;
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    Update();
                    break;
            }
            LadderMode = laddermode;
            IsCommentMode = iscommentmode;
        }

        #endregion

        #region Shell

        public ProjectViewModel ViewParent { get { return core?.Parent.Parent.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        #region Load
        
        public bool IsFullLoaded
        {
            get
            {
                foreach (LadderNetworkModel net in core.Parent.Children)
                    if (net.Inst.View == null) return false;
                return true;
            }
        }
        public ViewThreadManager ViewThread
        {
            get
            {
                return Core.Parent.Parent.Parent.ThMNGView;
            }
        }
        public IEnumerable<ILoadModel> LoadChildren
        {
            get
            {
                foreach (LadderNetworkModel net in core.Parent.Children)
                    if (net.Inst.View != null && !net.Inst.View.IsFullLoaded)
                        yield return net.Inst.View;
            }
        }

        public void FullLoad()
        {
            foreach (LadderNetworkModel net in core.Parent.Children)
            {
                if (net.Inst.View == null)
                {
                    Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        net.Inst.View = new InstructionNetworkViewModel(net.Inst);
                    });
                }
            }
        }   

        public void UpdateFullLoadProgress() { }
        
        #endregion

        public void Update()
        {
            MainStack.Children.Clear();
            foreach (LadderNetworkModel net in core.Parent.Children)
                MainStack.Children.Add(net.Inst.View);
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
                foreach (InstructionNetworkViewModel invmodel in MainStack.Children)
                    invmodel.LadderMode = value;
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
                foreach (InstructionNetworkViewModel invmodel in MainStack.Children)
                {
                    invmodel.IsCommentMode = value;
                }
            }
        }

        #endregion

    }
}

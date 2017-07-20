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
                        net.Inst.View = new InstructionNetworkViewModel(net.Inst);
                    MainStack.Children.Insert(e.NewStartingIndex, net.Inst.View);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    net = (LadderNetworkModel)(e.OldItems[0]);
                    MainStack.Children.Remove(net.Inst.View);
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
        
        public void Update()
        {
            MainStack.Children.Clear();
            foreach (LadderNetworkModel net in core.Parent.Children)
            {
                if (net.Inst.View == null)
                    net.Inst.View = new InstructionNetworkViewModel(net.Inst);
                MainStack.Children.Add(net.Inst.View);
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

        #region Select

        private InstSelectRect cursor;
        public new InstSelectRect Cursor { get { return this.cursor; } }

        #endregion

        #endregion

        #region Event Handler
        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (cursor.Core.Parent == null) return;
            switch (e.Key)
            {
                case Key.Down:
                    if (cursor.Core.Row <= 0)
                    {
                        int id = cursor.Core.Parent.Parent.ID;
                        if (id > 0)
                        {
                            cursor.Core.Parent = Core.Parent.Children[id - 1].Inst;
                            cursor.Core.Row = cursor.Core.Parent.Insts.Count - 1;
                        }
                    }
                    else
                    {
                        cursor.Core.Row--;
                    }
                    break;
                case Key.Up:
                    if (cursor.Core.Row >= cursor.Core.Parent.Insts.Count - 1)
                    {
                        int id = cursor.Core.Parent.Parent.ID;
                        if (id < Core.Parent.Children.Count - 1)
                        {
                            cursor.Core.Parent = Core.Parent.Children[id + 1].Inst;
                            cursor.Core.Row = cursor.Core.Parent.Insts.Count - 1;
                        }
                    }
                    else
                    {
                        cursor.Core.Row++;
                    }
                    break;
            }
        }

        #endregion
    }
}

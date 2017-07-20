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
using SamSoarII.Shell.Windows;
using System.Xml.Linq;
using SamSoarII.Core.Generate;

namespace SamSoarII.Shell.Models
{
    public class InstSelectRectCore : IModel
    {
        public InstSelectRectCore(InstructionNetworkModel _parent)
        {
            Parent = _parent;
        }

        public void Dispose()
        {
            Parent = null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private InstructionNetworkModel parent;
        public InstructionNetworkModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent == value) return;
                InstructionNetworkModel _parent = parent;
                this.parent = value;
                if (_parent?.View != null) _parent?.View.CV_Inst.Children.Remove(view);
                if (parent?.View != null) parent?.View.CV_Inst.Children.Add(view);
            }
        }
        IModel IModel.Parent { get { return Parent; } }

        private SelectRectCore ladder;
        public SelectRectCore Ladder
        {
            get
            {
                return this.ladder;
            }
            set
            {
                if (ladder == value) return;
                SelectRectCore _ladder = ladder;
                ladder = null;
                if (_ladder != null)
                {
                    _ladder.PropertyChanged -= OnLadderPropertyChanged;
                    if (_ladder.Inst != null) _ladder.Inst = null;
                }
                this.ladder = value;
                if (ladder != null)
                {
                    ladder.PropertyChanged += OnLadderPropertyChanged;
                    if (ladder.Inst != this) ladder.Inst = this;
                }
            }
        }

        private bool isnavigatable = true;
        public bool SelfIsNavigatable { get { return isnavigatable; } }
        public bool IsNavigatable { get { return isnavigatable && (ladder == null || ladder.SelfIsNavigatable); } }

        private void OnLadderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Current":
                    if (!IsNavigatable) break;
                    LadderUnitModel _current = ladder?.Current;
                    if (_current?.Inst?.Origin != null)
                    {
                        isnavigatable = false;
                        Parent = _current.Parent.Inst;
                        Row = Parent.Insts.IndexOf(_current.Inst.Origin);
                        isnavigatable = true;
                    }
                    break;
            }
        }

        private InstSelectRect view;
        public InstSelectRect View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (view == value) return;
                InstSelectRect _view = view;
                view = null;
                if (_view != null && _view.Core != null) _view.Core = null;
                this.view = value;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View { get { return View; } set { View = (InstSelectRect)value; } }

        private int row;
        public int Row
        {
            get
            {
                return this.row;
            }
            set
            {
                if (row == value) return;
                this.row = value;
                InstructionDiagramViewModel idvmodel = parent?.Parent?.Parent?.Inst?.View;
                InstructionNetworkViewModel invmodel = parent?.Parent?.Inst?.View;
                if (idvmodel != null)
                {
                    ScrollViewer scroll = idvmodel.Scroll;
                    Point p = invmodel.CV_Inst.TranslatePoint(new Point(0, 0), scroll);
                    if (p.Y + row * 20 < 0)
                        scroll.ScrollToVerticalOffset(scroll.VerticalOffset - (p.Y + row * 20));
                    if (p.Y + (row + 1) * 20 > scroll.Height)
                        scroll.ScrollToVerticalOffset(scroll.VerticalOffset - (p.Y + (row + 1) * 20));
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Row"));
                PropertyChanged(this, new PropertyChangedEventArgs("Current"));
            }
        }
        public PLCOriginInst Current { get { return parent != null && row >= 0 && row < parent.Insts.Count ? parent.Insts[row] : null; } }

        #endregion

        public ProjectTreeViewItem PTVItem { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public void Save(XElement xele)
        {
            throw new NotImplementedException();
        }

        public void Load(XElement xele)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// InstSelectRect.xaml 的交互逻辑
    /// </summary>
    public partial class InstSelectRect : UserControl, IViewModel
    {
        public InstSelectRect(InstSelectRectCore _core)
        {
            InitializeComponent();
            Core = _core;
            Canvas.SetZIndex(this, 1);
        }

        public void Dispose()
        {
            Core = null;
        }

        #region Core

        private InstSelectRectCore core;
        public InstSelectRectCore Core
        {
            get
            {
                return this.core;
            }
            set
            {
                if (core == value) return;
                InstSelectRectCore _core = core;
                core = null;
                if (_core != null)
                {
                    _core.PropertyChanged -= OnCorePropertyChanged;
                    if (_core.View != null) _core.View = null;
                }
                this.core = value;
                if (core != null)
                {
                    core.PropertyChanged += OnCorePropertyChanged;
                    if (core.View != this) core.View = this;
                }
            }
        }
        IModel IViewModel.Core { get { return Core; } set { Core = (InstSelectRectCore)value; } }

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Row":
                    Canvas.SetTop(this, 20 * core.Row);
                    break;
                case "Current":
                    Visibility = core.Current != null 
                        ? Visibility.Visible : Visibility.Hidden;
                    break;
            }
        }

        #endregion

        #region Shell
        
        public InstructionNetworkViewModel ViewParent { get { return core?.Parent?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        #endregion
    }
}

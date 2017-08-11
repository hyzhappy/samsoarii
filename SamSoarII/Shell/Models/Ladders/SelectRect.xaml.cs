using SamSoarII.Core.Models;
using SamSoarII.Shell.Windows;
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
using System.Xml.Linq;

namespace SamSoarII.Shell.Models
{
    public class SelectRectCore : IModel
    {
        public SelectRectCore(LadderNetworkModel _parent)
        {
            Parent = _parent;
        }

        public void Dispose()
        {
            Parent = null;
        }

        public void Save(XElement xele)
        {
            throw new NotImplementedException();
        }

        public void Load(XElement xele)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Number

        private LadderNetworkModel parent;
        public LadderNetworkModel Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (parent == value) return;
                LadderNetworkModel _parent = parent;
                this.parent = null;
                if (_parent != null)
                {
                    if (_parent.Parent.View.SelectionStatus == SelectStatus.SingleSelected)
                        _parent.Parent.View.SelectionStatus = SelectStatus.Idle;
                }
                this.parent = value;
                if (parent != null)
                {
                    if (!parent.IsExpand) parent.IsExpand = true;
                    if (parent.Parent.View.SelectionStatus != SelectStatus.SingleSelected)
                        parent.Parent.View.SelectionStatus = SelectStatus.SingleSelected;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Parent"));
                PropertyChanged(this, new PropertyChangedEventArgs("Current"));
            }
        }
        IModel IModel.Parent { get { return Parent; } }

        private int x;
        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
                PropertyChanged(this, new PropertyChangedEventArgs("X"));
                PropertyChanged(this, new PropertyChangedEventArgs("Current"));
            }
        }

        private int y;
        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Y"));
                PropertyChanged(this, new PropertyChangedEventArgs("Current"));
            }
        }

        public LadderUnitModel Current
        {
            get
            {
                return parent == null ? null : parent.Children[x, y];
            }
            set
            {
                Parent = value.Parent;
                X = value.X;
                Y = value.Y;
            }
        }

        private InstSelectRectCore inst;
        public InstSelectRectCore Inst
        {
            get
            {
                return this.inst;
            }
            set
            {
                if (inst == value) return;
                InstSelectRectCore _inst = inst;
                inst = null;
                if (_inst != null)
                {
                    _inst.PropertyChanged -= OnInstPropertyChanged;
                    if (_inst.Ladder != null) _inst.Ladder = null;
                }
                this.inst = value;
                if (inst != null)
                {
                    inst.PropertyChanged += OnInstPropertyChanged;
                    if (inst.Ladder != this) inst.Ladder = this;
                }
            }
        }

        private bool isnavigatable = true;
        public bool SelfIsNavigatable { get { return isnavigatable; } }
        public bool IsNavigatable { get { return isnavigatable && (inst == null || inst.SelfIsNavigatable); } }

        private void OnInstPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Current":
                    if (!IsNavigatable) break;
                    LadderUnitModel _current = Inst.Current?.Inst?.ProtoType;
                    if (_current?.Parent != null && _current != Current)
                    {
                        isnavigatable = false;
                        Parent = _current.Parent;
                        X = _current.X;
                        Y = _current.Y;
                        Parent.Parent.View.IFParent.Navigate(_current);
                        isnavigatable = true;
                    }
                    break;
            }
        }

        #endregion

        #region View

        private SelectRect view;
        public SelectRect View
        {
            get
            {
                return this.view;
            }
            set
            {
                SelectRect _view = view;
                this.view = value;
                if (_view != null && _view.Core != null) _view.Core = null;
                if (view != null && view.Core != this) view.Core = this;
            }
        }
        IViewModel IModel.View { get { return View; } set { View = (SelectRect)value; } }

        public ProjectTreeViewItem PTVItem { get { return null; } set { } }

        #endregion
    }
    /// <summary>
    /// SelectRect.xaml 的交互逻辑
    /// </summary>
    public partial class SelectRect : UserControl, IViewModel, INotifyPropertyChanged
    {
        public SelectRect()
        {
            InitializeComponent();
            Core = new SelectRectCore(null);
            Width = Global.GlobalSetting.LadderWidthUnit;
            IsCommentMode = false;
            Canvas.SetZIndex(this, 1);
            Visibility = Visibility.Hidden;
        }

        public void Dispose()
        {
            Core.Dispose();
            Core = null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Core

        private SelectRectCore core;
        public SelectRectCore Core
        {
            get
            {
                return this.core;
            }
            set
            {
                SelectRectCore _core = core;
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
        IModel IViewModel.Core { get { return Core; } set { Core = (SelectRectCore)value; } }
        public int X { get { return Core.X; } set { Core.X = value; } }
        public int Y { get { return Core.Y; } set { Core.Y = value; } }
        public LadderUnitModel Current { get { return Core.Current; } }

        private void OnCorePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Parent": case "X": case "Y":
                    if (Core?.Parent != null)
                    {
                        Visibility = Visibility.Visible;
                        Canvas.SetLeft(this, Global.GlobalSetting.LadderWidthUnit * Core.X);
                        Canvas.SetTop(this, Core.Parent.UnitBaseTop + (isCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit) * Core.Y);
                    }
                    else
                        Visibility = Visibility.Hidden;
                    break;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        #endregion

        #region Shell

        public LadderNetworkViewModel ViewParent { get { return core?.Parent?.View; } }
        IViewModel IViewModel.ViewParent { get { return ViewParent; } }

        private bool isCommentMode;
        public bool IsCommentMode
        {
            get
            {
                return isCommentMode;
            }
            set
            {
                isCommentMode = value;
                Canvas.SetTop(this, (isCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit) * Core.Y);
                Height = isCommentMode ? Global.GlobalSetting.LadderCommentModeHeightUnit : Global.GlobalSetting.LadderHeightUnit;
            }
        }

        #endregion
        
    }
}

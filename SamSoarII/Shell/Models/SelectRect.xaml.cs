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
                this.parent = value;
                if (_parent?.View != null)
                {
                    _parent.View.LadderCanvas.Children.Remove(View);
                    if (_parent.Parent.View.SelectionStatus == SelectStatus.SingleSelected)
                        _parent.Parent.View.SelectionStatus = SelectStatus.Idle;
                }
                if (parent?.View != null)
                {
                    parent.View.LadderCanvas.Children.Add(View);
                    parent.View.SelectAreaOriginFX = X;
                    parent.View.SelectAreaOriginFY = Y;
                    parent.View.SelectAreaOriginSX = X;
                    if (parent.Parent.View.SelectionStatus != SelectStatus.SingleSelected)
                        parent.Parent.View.SelectionStatus = SelectStatus.SingleSelected;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Parent"));
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
            get { return parent == null ? null : parent.Children[x, y]; }
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
            Width = 300;
            IsCommentMode = false;
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
                case "X": Canvas.SetLeft(this, 300 * Core.X); break;
                case "Y": Canvas.SetTop(this, (isCommentMode ? 500 : 300) * Core.Y); break;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(e.PropertyName));
        }

        #endregion

        #region Shell

        public LadderNetworkViewModel ViewParent { get { return core.Parent.View; } }
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
                Canvas.SetTop(this, (isCommentMode ? 500 : 300) * Core.Y);
                Height = isCommentMode ? 500 : 300;
            }
        }

        public void Move(LadderNetworkViewModel net, int x = 0, int y = 0)
        {
            Core.Parent = (net != null ? net.Core : null);
            Core.X = x;
            Core.Y = y;
        }

        #endregion
        
    }
}

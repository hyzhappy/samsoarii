﻿using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// SelectRect.xaml 的交互逻辑
    /// </summary>
    public partial class SelectRect : UserControl
    {
        protected LadderNetworkViewModel parent;
        public event RoutedEventHandler NetworkParentChanged = delegate { };
        public LadderNetworkViewModel NetworkParent
        {
            get { return this.parent; }
            set
            {
                this.parent = value;
                NetworkParentChanged(this, new RoutedEventArgs());
            }
        }
        public BaseViewModel CurrentElement
        {
            get
            {
                if (parent == null) return null;
                return parent.GetElementByPosition(X, Y);
            }
        }
        
        private bool _isCommentMode;
        public bool IsCommentMode
        {
            get
            {
                return _isCommentMode;
            }
            set
            {
                _isCommentMode = value;
                UpdateHeightProperty();
                UpdateTopProperty();
            }
        }
        int _x;

        public event RoutedEventHandler XChanged = delegate { };
        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                UpdateLeftProperty();
                XChanged(this, new RoutedEventArgs());
            }
        }
        int _y;

        public event RoutedEventHandler YChanged = delegate { };
        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                UpdateTopProperty();
                YChanged(this, new RoutedEventArgs());
            }
        }
        public SelectRect()
        {
            InitializeComponent();
            IsCommentMode = false;
        }

        private void UpdateTopProperty()
        {
            Canvas.SetTop(this, (_isCommentMode ? 500 : 300) * _y);
        }

        private void UpdateLeftProperty()
        {
            Canvas.SetLeft(this, _x * 300);
        }

        private void UpdateHeightProperty()
        {
            Height = _isCommentMode ? 500 : 300;
        }
    }
}

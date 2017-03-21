using System;
using System.Collections.Generic;
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
using SamSoarII.LadderInstModel;
using SamSoarII.PLCDevice;

namespace SamSoarII.LadderInstViewModel
{
    /// <summary>
    /// InputBaseViewModel.xaml 的交互逻辑
    /// </summary>
    public abstract partial class InputBaseViewModel : BaseViewModel
    {
        public override ElementType Type
        {
            get
            {
                return ElementType.Input;
            }
        }

        private int _x;
        private int _y;
        private bool _isCommentMode;
        private bool _isMonitorMode;

        public override int X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
                UpdateLeftProperty();
            }
        }

        public override int Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
                UpdateTopProperty();
            }
        }

        public override bool IsCommentMode
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
                UpdateCommentAreaVisibility();
            }
        }

        public override bool IsMonitorMode
        {
            get
            {
                return _isMonitorMode;
            }

            set
            {
                _isMonitorMode = value;
            }
        }

        public InputBaseViewModel()
        {
            InitializeComponent();
            IsCommentMode = false;
        }

        private void UpdateHeightProperty()
        {
            Height = _isCommentMode ? 500 : 300;
        }

        private void UpdateTopProperty()
        {
            if (_isCommentMode)
            {
                Canvas.SetTop(this, _y * 500);
            }
            else
            {
                Canvas.SetTop(this, _y * 300);
            }
        }

        private void UpdateLeftProperty()
        {
            Canvas.SetLeft(this, _x * 300);
        }
        public override void UpdateCommentContent()
        {
            // nothing to do
        }
        private void UpdateCommentAreaVisibility()
        {
            CommentArea.Visibility = _isCommentMode ? Visibility.Visible : Visibility.Hidden;
        }
        public override bool Assert()
        {
            return NextElemnets.All(x => { return (x.Type == ElementType.Input) | (x.Type == ElementType.Special) | (x.Type == ElementType.Null); }) & NextElemnets.Count > 0;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {

        }
    }
}

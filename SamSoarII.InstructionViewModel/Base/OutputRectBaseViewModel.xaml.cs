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
using SamSoarII.UserInterface;
using SamSoarII.PLCDevice;

namespace SamSoarII.LadderInstViewModel
{
    /// <summary>
    /// OutputNoValueBaseViewModel.xaml 的交互逻辑
    /// </summary>
    public abstract partial class OutputRectBaseViewModel : BaseViewModel
    {
        public override ElementType Type
        {
            get
            {
                return ElementType.Output;
            }
        }

        private int _x;
        private int _y;
        private bool _isCommentMode;

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
        
        public OutputRectBaseViewModel()
        {
            InitializeComponent();
            IsCommentMode = false;
            DataContext = this;
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

        private void UpdateCommentAreaVisibility()
        {
            CommentArea.Visibility = _isCommentMode ? Visibility.Visible : Visibility.Hidden;
        }
        public override void UpdateCommentContent()
        {
            // nothing to do
        }
        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {

        }
        public override bool Assert()
        {
            return NextElements.All(x => { return (x.Type == ElementType.Input) | (x.Type == ElementType.Special); }) & NextElements.Count > 0;
        }

        #region Monitor

        private string middletextblock1_oldtext
            = String.Empty;
        private string middletextblock2_oldtext
            = String.Empty;
        private string middletextblock3_oldtext
            = String.Empty;
        private string middletextblock4_oldtext
            = String.Empty;
        private string middletextblock5_oldtext
            = String.Empty;
        private bool ismonitormode;
        public override bool IsMonitorMode
        {
            get
            {
                return this.ismonitormode;
            }
            set
            {
                this.ismonitormode = value;
                switch (value)
                {
                    case true:
                        middletextblock1_oldtext = MiddleTextBlock1.Text;
                        middletextblock2_oldtext = MiddleTextBlock2.Text;
                        middletextblock3_oldtext = MiddleTextBlock3.Text;
                        middletextblock4_oldtext = MiddleTextBlock4.Text;
                        middletextblock5_oldtext = MiddleTextBlock5.Text;
                        UpdateMonitor();
                        break;
                    case false:
                        MiddleTextBlock1.Text = middletextblock1_oldtext;
                        MiddleTextBlock2.Text = middletextblock2_oldtext;
                        MiddleTextBlock3.Text = middletextblock3_oldtext;
                        MiddleTextBlock4.Text = middletextblock4_oldtext;
                        MiddleTextBlock5.Text = middletextblock5_oldtext;
                        break;
                }
            }
        }

        private void UpdateMonitor()
        {
            UpdateMonitor_MiddleTextBlock1();
            UpdateMonitor_MiddleTextBlock2();
            UpdateMonitor_MiddleTextBlock3();
            UpdateMonitor_MiddleTextBlock4();
            UpdateMonitor_MiddleTextBlock5();
        }

        private void UpdateMonitor_MiddleTextBlock1()
        {
            Dispatcher.Invoke(() =>
            {
                MiddleTextBlock1.Text = Model.ParaCount > 0 && _values[0] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(0).ValueString,
                        IsRunning ? _values[0].Value : "???")
                    : String.Empty;
            });
        }

        private void UpdateMonitor_MiddleTextBlock2()
        {
            Dispatcher.Invoke(() =>
            {
                MiddleTextBlock2.Text = Model.ParaCount > 1 && _values[1] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(1).ValueString,
                        IsRunning ? _values[1].Value : "???")
                    : String.Empty;
            });
        }

        private void UpdateMonitor_MiddleTextBlock3()
        {
            Dispatcher.Invoke(() =>
            {
                MiddleTextBlock3.Text = Model.ParaCount > 2 && _values[2] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(2).ValueString,
                        IsRunning ? _values[2].Value : "???")
                    : String.Empty;
            });
        }

        private void UpdateMonitor_MiddleTextBlock4()
        {
            Dispatcher.Invoke(() =>
            {
                MiddleTextBlock4.Text = Model.ParaCount > 3 && _values[3] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(3).ValueString,
                        IsRunning ? _values[3].Value : "???")
                    : String.Empty;
            });
        }

        private void UpdateMonitor_MiddleTextBlock5()
        {
            Dispatcher.Invoke(() =>
            {
                MiddleTextBlock5.Text = Model.ParaCount > 4 && _values[4] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(4).ValueString,
                        IsRunning ? _values[4].Value : "???")
                    : String.Empty;
            });
        }

        protected override void OnStart(object sender, RoutedEventArgs e)
        {
            base.OnStart(sender, e);
            UpdateMonitor();
        }

        protected override void OnAbort(object sender, RoutedEventArgs e)
        {
            base.OnAbort(sender, e);
            UpdateMonitor();
        }

        protected override void OnValueChanged(object sender, RoutedEventArgs e)
        {
            base.OnValueChanged(sender, e);
            if (sender == _values[0])
            {
                UpdateMonitor_MiddleTextBlock1();
            }
            if (sender == _values[1])
            {
                UpdateMonitor_MiddleTextBlock2();
            }
            if (sender == _values[2])
            {
                UpdateMonitor_MiddleTextBlock3();
            }
            if (sender == _values[3])
            {
                UpdateMonitor_MiddleTextBlock4();
            }
            if (sender == _values[4])
            {
                UpdateMonitor_MiddleTextBlock5();
            }
        }

        #endregion
    }
}

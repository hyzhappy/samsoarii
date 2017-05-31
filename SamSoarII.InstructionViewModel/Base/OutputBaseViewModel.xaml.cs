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
    /// OutputBaseViewModel.xaml 的交互逻辑
    /// </summary>
    public abstract partial class OutputBaseViewModel : BaseViewModel
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
                UpdateTopProperty();
                UpdateHeightProperty();
                UpdateCommentAreaVisibility();
            }
        }
        
        public OutputBaseViewModel()
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


        public override bool Assert()
        {
            return NextElements.All(x => { return (x.Type == ElementType.Input) | (x.Type == ElementType.Special); }) & NextElements.Count > 0;
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

        #region Monitor

        private string valuetextblock_oldtext
            = String.Empty;
        private string counttextblock_oldtext
            = String.Empty;
        private bool ismonitormode = false;
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
                        valuetextblock_oldtext = ValueTextBlock.Text;
                        counttextblock_oldtext = CountTextBlock.Text;
                        UpdateMonitor();
                        break;
                    case false:
                        ValueTextBlock.Text = valuetextblock_oldtext;
                        CountTextBlock.Text = counttextblock_oldtext;
                        break;
                }
            }
        }

        private void UpdateMonitor()
        {
            UpdateMonitor_ValueTextBlock();
            UpdateMonitor_CountTextBlock();
            UpdateMonitor_CenterCanvas();
        }

        private void UpdateMonitor_ValueTextBlock()
        {
            Dispatcher.Invoke(() =>
            {
                ValueTextBlock.Text = Model.ParaCount > 0 && _values[0] != null
                        ? String.Format("{0:s} = {1:s}",
                            Model.GetPara(0).ValueString,
                            IsRunning ? _values[0].Value : "???")
                        : String.Empty;
            });
        }

        private void UpdateMonitor_CountTextBlock()
        {
            Dispatcher.Invoke(() =>
            {
                CountTextBlock.Text = Model.ParaCount > 1 && _values[1] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(1).ValueString,
                        IsRunning ? _values[1].Value : "???")
                    : String.Empty;
            });
        }
        
        static string[] BIT_0_SHOWS = { "0", "OFF", "FALSE" };
        static string[] BIT_1_SHOWS = { "1", "ON", "TRUE" };
        private void UpdateMonitor_CenterCanvas()
        {
            Dispatcher.Invoke(() =>
            {
                if (!IsRunning)
                {
                    CenterCanvas.Background = Brushes.Transparent;
                    return;
                }
                bool value = false;
                try
                {
                    if (_values[0] == null)
                        throw new FormatException("Lack of arguments.");
                    if (!BIT_0_SHOWS.Contains(_values[0].Value)
                     && !BIT_1_SHOWS.Contains(_values[0].Value))
                        throw new FormatException("value0 is not a BIT.");
                    value = BIT_1_SHOWS.Contains(_values[0].Value);
                }
                catch (FormatException)
                {
                    CenterCanvas.Background = Brushes.Red;
                    return;
                }
                CenterCanvas.Background = value ? Brushes.Green : Brushes.Transparent;
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
            UpdateMonitor();
        }
        #endregion

    }
}

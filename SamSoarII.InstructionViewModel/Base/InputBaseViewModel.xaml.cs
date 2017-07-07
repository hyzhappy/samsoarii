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
using SamSoarII.Utility;
using static SamSoarII.Utility.Delegates;

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
        private IntPoint intPos = new IntPoint();
        public override IntPoint IntPos
        {
            get { return intPos; }
            set { intPos = value; }
        }
        public override int X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
                intPos.X = value;
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
                intPos.Y = value;
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

        public InputBaseViewModel()
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
                if (BPRect != null)
                    Canvas.SetTop(BPRect, _y * 500);
            }
            else
            {
                Canvas.SetTop(this, _y * 300);
                if (BPRect != null)
                    Canvas.SetTop(BPRect, _y * 300);
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
            return NextElements.All(x => { return (x.Type == ElementType.Input) | (x.Type == ElementType.Special) | (x.Type == ElementType.Null); }) & NextElements.Count > 0;
        }

        public override void AcceptNewValues(IList<string> valueStrings, Device contextDevice)
        {

        }

        #region Monitor

        private string valuetextblock_oldtext
            = String.Empty;
        private string value2textblock_oldtext
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
                        valuetextblock_oldtext = ValueTextBlock.Text;
                        value2textblock_oldtext = Value2TextBlock.Text;
                        UpdateMonitor_ValueTextBlock();
                        UpdateMonitor_Value2TextBlock();
                        break;
                    case false:
                        ValueTextBlock.Text = valuetextblock_oldtext;
                        Value2TextBlock.Text = value2textblock_oldtext;
                        break;
                }
            }
        }

        private void UpdateMonitor()
        {
            UpdateMonitor_ValueTextBlock();
            UpdateMonitor_Value2TextBlock();
            UpdateMonitor_CenterCanvas();
        }

        private void UpdateMonitor_ValueTextBlock()
        {
            Dispatcher.Invoke(new Execute(() =>
            {
                ValueTextBlock.Text = Model.ParaCount > 0 && _values[0] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(0).ValueString,
                        IsRunning ? _values[0].Value : "???")
                    : Model.ParaCount > 0 ? String.Format("{0:s}", Model.GetPara(0).ValueString)
                    : String.Empty;

            }));
        }

        private void UpdateMonitor_Value2TextBlock()
        {
            Dispatcher.Invoke(new Execute(() =>
            {
                Value2TextBlock.Text = Model.ParaCount > 1 && _values[1] != null
                    ? String.Format("{0:s} = {1:s}",
                        Model.GetPara(1).ValueString,
                        IsRunning ? _values[1].Value : "???")
                    : Model.ParaCount > 1 ? String.Format("{0:s}", Model.GetPara(1).ValueString)
                    : String.Empty;
            }));
        }

        static string[] BIT_0_SHOWS = { "0", "OFF", "FALSE" };
        static string[] BIT_1_SHOWS = { "1", "ON", "TRUE" };
        private void UpdateMonitor_CenterCanvas()
        {
            Dispatcher.Invoke(new Execute(() =>
            {
                if (!IsRunning)
                {
                    CenterCanvas.Background = Brushes.Transparent;
                    return;
                }
                bool value = false;
                Int32 w1 = 0, w2 = 0;
                Int64 d1 = 0, d2 = 0;
                double f1 = 0, f2 = 0;
                try
                {
                    switch (InstructionName)
                    {
                        case "LD":
                        case "LDI":
                        case "LDIM":
                        case "LDIIM":
                        case "LDP":
                        case "LDF":
                            if (_values[0] == null)
                                throw new FormatException("Lack of Arguments.");
                            if (!BIT_0_SHOWS.Contains(_values[0].Value)
                             && !BIT_1_SHOWS.Contains(_values[0].Value))
                                throw new FormatException("value0 is not a BIT.");
                            break;
                        case "LDWEQ":
                        case "LDWNE":
                        case "LDWLE":
                        case "LDWGE":
                        case "LDWL":
                        case "LDWG":
                        case "LDDEQ":
                        case "LDDNE":
                        case "LDDLE":
                        case "LDDGE":
                        case "LDDL":
                        case "LDDG":
                        case "LDFEQ":
                        case "LDFNE":
                        case "LDFLE":
                        case "LDFGE":
                        case "LDFL":
                        case "LDFG":
                            if (_values[0] == null || _values[1] == null)
                                throw new FormatException("Lack of arguments.");
                            break;
                    }
                    switch (InstructionName)
                    {
                        case "LD":
                        case "LDIM":
                            value = BIT_1_SHOWS.Contains(_values[0].Value);
                            break;
                        case "LDI":
                        case "LDIIM":
                            value = BIT_0_SHOWS.Contains(_values[0].Value);
                            break;
                        case "LDWEQ":
                        case "LDWNE":
                        case "LDWLE":
                        case "LDWGE":
                        case "LDWL":
                        case "LDWG":
                            w1 = Int32.Parse(_values[0].Value);
                            w2 = Int32.Parse(_values[1].Value);
                            break;
                        case "LDDEQ":
                        case "LDDNE":
                        case "LDDLE":
                        case "LDDGE":
                        case "LDDL":
                        case "LDDG":
                            d1 = Int64.Parse(_values[0].Value);
                            d2 = Int64.Parse(_values[1].Value);
                            break;
                        case "LDFEQ":
                        case "LDFNE":
                        case "LDFLE":
                        case "LDFGE":
                        case "LDFL":
                        case "LDFG":
                            f1 = double.Parse(_values[0].Value);
                            f2 = double.Parse(_values[1].Value);
                            break;
                    }
                    switch (InstructionName)
                    {
                        case "LDWEQ": value = (w1 == w2); break;
                        case "LDWNE": value = (w1 != w2); break;
                        case "LDWLE": value = (w1 <= w2); break;
                        case "LDWGE": value = (w1 >= w2); break;
                        case "LDWL": value = (w1 < w2); break;
                        case "LDWG": value = (w1 > w2); break;
                        case "LDDEQ": value = (d1 == d2); break;
                        case "LDDNE": value = (d1 != d2); break;
                        case "LDDLE": value = (d1 <= d2); break;
                        case "LDDGE": value = (d1 >= d2); break;
                        case "LDDL": value = (d1 < d2); break;
                        case "LDDG": value = (d1 > d2); break;
                        case "LDFEQ": value = (f1 == f2); break;
                        case "LDFNE": value = (f1 != f2); break;
                        case "LDFLE": value = (f1 <= f2); break;
                        case "LDFGE": value = (f1 >= f2); break;
                        case "LDFL": value = (f1 < f2); break;
                        case "LDFG": value = (f1 > f2); break;
                    }
                }
                catch (FormatException)
                {
                    CenterCanvas.Background = Brushes.Red;
                    return;
                }
                CenterCanvas.Background = value ? Brushes.Green : Brushes.Transparent;
            }));
        }

        protected override void OnStart(object sender, RoutedEventArgs e)
        {
            base.OnStart(sender, e);
            Dispatcher.Invoke(new Execute(() => { UpdateMonitor(); }));
        }

        protected override void OnAbort(object sender, RoutedEventArgs e)
        {
            base.OnAbort(sender, e);
            Dispatcher.Invoke(new Execute(() => { UpdateMonitor(); }));
        }

        protected override void OnValueChanged(object sender, RoutedEventArgs e)
        {
            base.OnValueChanged(sender, e);
            Dispatcher.Invoke(new Execute(() => { UpdateMonitor(); }));
        }


        #endregion
    }
}

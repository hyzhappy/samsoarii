using System;
using System.Collections.Generic;
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

using SamSoarII.Simulation.Core.VariableModel;

/// <summary>
/// Namespace : SamSoarII.Simulation.UI.Monitor
/// ClassName : MonitorTextBox
/// Version   : 1.0
/// Date      : 2017/4/11
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 监视变量列表中行元素的值开关交替按钮
/// 当开关为OFF时按一下变为ON
/// 当开关为ON时按一下变为OFF
/// </remarks>

namespace SamSoarII.Simulation.UI.Monitor
{
    /// <summary>
    /// MonitorBitButton.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorBitButton : Button
    {
        #region Numbers & Numbers Interface

        #region Simulate Variable Unit

        /// <summary>
        /// 对应的变量单元
        /// </summary>
        private SimulateVariableUnit svunit;
        /// <summary>
        /// 对应的变量单元
        /// </summary>
        public SimulateVariableUnit SVUnit
        {
            set
            {
                if (svunit != null)
                {
                    svunit.ValueChanged -= OnValueChanged;
                    svunit.LockChanged -= OnLockChanged;
                }
                this.svunit = value;
                if (svunit != null)
                {
                    svunit.ValueChanged += OnValueChanged;
                    svunit.LockChanged += OnLockChanged;
                }
                SetText();
            }
            protected get
            {
                return this.svunit;
            }
        }

        #endregion

        #region Status

        /// <summary> 按钮状态标志常量：打开 </summary>
        public const int STATUS_ON = 0x01;
        /// <summary> 按钮状态标志常量：关闭 </summary>
        public const int STATUS_OFF = 0x00;
        /// <summary> 按钮状态标志常量：出错 </summary>
        public const int STATUS_ERROR = 0x02;
        /// <summary> 按钮状态标志常量：显示变量组 </summary>
        public const int STATUS_SERIES = 0x03;
        /// <summary>
        /// 按钮状态
        /// </summary>
        private int status;
        /// <summary>
        /// 按钮状态
        /// </summary>
        public int Status
        {
            get
            {
                return this.status;
            }
            protected set
            {
                // 根据状态来显示文本
                this.Dispatcher.Invoke(new Utility.Delegates.Execute(() =>
                {
                    this.status = value;
                    switch (this.status)
                    {
                        case STATUS_ON:
                            Title.Text = "ON";
                            Background = Brushes.Transparent;
                            break;
                        case STATUS_OFF:
                            Title.Text = "OFF";
                            Background = Brushes.Transparent;
                            break;
                        case STATUS_ERROR:
                            Title.Text = "ERROR";
                            Background = Brushes.Red;
                            break;
                        case STATUS_SERIES:
                            Title.Text = "...";
                            Background = Brushes.Transparent;
                            break;
                    }
                }));
            }
        }

        #endregion

        /// <summary>
        /// 是否为只读（与文本框对应）
        /// </summary>
        public bool IsReadOnly { get; set; }

        #endregion

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public MonitorBitButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置文本（状态）
        /// </summary>
        public void SetText()
        {
            // 没有变量则返回
            if (svunit == null)
            {
                return;
            }
            if (svunit is SimulateUnitSeries)
            {
                Status = STATUS_SERIES;
                return;
            }
            if (svunit.Type.Equals("BIT"))
            {
                // 根据值来设置文本
                switch ((int)(svunit.Value))
                {
                    case 0:
                        Status = STATUS_OFF;
                        break;
                    case 1:
                        Status = STATUS_ON;
                        break;
                    default:
                        Status = STATUS_ERROR;
                        break;
                }
            }
        }
        
        private MonitorDetailDialog dialog;

        public void ShowDetailDialog()
        {
            if (!(SVUnit is SimulateUnitSeries) ||
                dialog != null)
            {
                return;
            }
            dialog = new MonitorDetailDialog(
                (SimulateUnitSeries)SVUnit);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dialog.Closed += OnDialogClosed;
            dialog.Show();
        }

        private void OnDialogClosed(object sender, EventArgs e)
        {
            dialog = null;
        }
        
        #region Event Handler

        #region KeyBoard Control

        /// <summary>
        /// 当焦点上移时，触发这个代理
        /// </summary>
        public event RoutedEventHandler FocusUp = delegate { };
        /// <summary>
        /// 当焦点下移时，触发这个代理
        /// </summary>
        public event RoutedEventHandler FocusDown = delegate { };
        /// <summary>
        /// 当焦点左移时，触发这个代理
        /// </summary>
        public event RoutedEventHandler FocusLeft = delegate { };
        /// <summary>
        /// 当焦点右移时，触发这个代理
        /// </summary>
        public event RoutedEventHandler FocusRight = delegate { };
        /// <summary>
        /// 当按下键盘时发生
        /// </summary>
        /// <param name="e">键盘事件</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            // 根据按键执行相应操作
            switch (e.Key)
            {
                // 键入Enter时向后新建
                case Key.Enter:
                    //OnClick();
                    break;
                // 键入Up时焦点上移
                case Key.Up:
                    FocusUp(this, new RoutedEventArgs());
                    break;
                // 键入Down时焦点下移
                case Key.Down:
                    FocusDown(this, new RoutedEventArgs());
                    break;
                // 键入Left时焦点左移
                case Key.Left:
                    FocusLeft(this, new RoutedEventArgs());
                    break;
                // 键入Right时焦点右移
                case Key.Right:
                    FocusRight(this, new RoutedEventArgs());
                    break;
            }
        }

        #endregion

        /// <summary>
        /// 当文本（状态）发生合法更改时，触发这个代理
        /// </summary>
        public event RoutedEventHandler TextLegalChanged = delegate { };

        /// <summary>
        /// 当点击按钮时发生
        /// </summary>
        protected override void OnClick()
        {
            if (Status == STATUS_SERIES)
            {
                ShowDetailDialog();
                return;
            }
            // 非只读状态下才能点击按钮
            if (!IsReadOnly)
            {
                base.OnClick();
                // 交替修改当前状态
                switch (Status)
                {
                    case STATUS_ON:
                        Status = STATUS_OFF;
                        TextLegalChanged(this, new RoutedEventArgs());
                        break;
                    case STATUS_OFF:
                        Status = STATUS_ON;
                        TextLegalChanged(this, new RoutedEventArgs());
                        break;
                }
            }
        }

        /// <summary>
        /// 当变量数值修改时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnValueChanged(object sender, RoutedEventArgs e)
        {
            SetText();
        }

        /// <summary>
        /// 当变量锁定修改时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnLockChanged(object sender, RoutedEventArgs e)
        {
            IsReadOnly = !SVUnit.Islocked;
            OnValueChanged(sender, e);
        }
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

using SamSoarII.Simulation.Core.VariableModel;

/// <summary>
/// Namespace : SamSoarII.Simulation.UI.Monitor
/// ClassName : MonitorTextBox
/// Version   : 1.0
/// Date      : 2017/4/11
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 监视变量列表中行元素的文本输入组件
/// 可以显示和输入变量的名称，别名和数值
/// </remarks>

namespace SamSoarII.Simulation.UI.Monitor
{
    public class MonitorTextBox : TextBox
    {
        #region Numbers & Numbers Interface
        
        #region Simulate Variable Unit

        /// <summary>
        /// 对应的变量单元
        /// </summary>
        private SimulateVariableUnit svunit;
        /// <summary>
        /// 设置变量单元
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
                    SetText();
                }
            }
            get
            {
                return this.svunit;
            }
        }

        private bool settingup = false;

        #endregion

        #region Type
        /// <summary> 类型标志：名称</summary>
        public const int TYPE_NAME = 0x01;
        /// <summary> 类型标志：变量类型</summary>
        public const int TYPE_TYPE = 0x02;
        /// <summary> 类型标志：别名</summary>
        public const int TYPE_VAR = 0x03;
        /// <summary> 类型标志：数值</summary>
        public const int TYPE_VALUE = 0x04;
        /// <summary> 显示标志：10进制显示</summary>
        public const int BASE_10 = 0x00;
        /// <summary> 显示标志：16进制显示</summary>
        public const int BASE_16 = 0x10;

        /// <summary>
        /// 类型
        /// </summary>
        private int type;
        /// <summary>
        /// 类型
        /// </summary>
        public int Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
                OnValueChanged(this, new RoutedEventArgs());
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="_type">类型</param>
        public MonitorTextBox(int _type)
        {
            Type = _type;
        }
        
        /// <summary>
        /// 根据变量和类型设置文本
        /// </summary>
        public void SetText()
        {
            settingup = true;
            // 如果没有变量则返回
            if (SVUnit == null)
            {
                settingup = false;
                return;
            }
            // 根据变量和类型设置文本
            this.Dispatcher.Invoke(() =>
            {
                switch (Type & 0x0f)
                {
                    case TYPE_NAME:
                        if (Text == null || !Text.Equals(svunit.Name))
                            Text = svunit.Name;
                        break;
                    case TYPE_TYPE:
                        if (Text == null || !Text.Equals(svunit.Type))
                            Text = svunit.Type;
                        break;
                    case TYPE_VAR:
                        if (Text == null || !Text.Equals(svunit.Var))
                            Text = svunit.Var;
                        break;
                    case TYPE_VALUE:
                        if (Text == null || !Text.Equals(svunit.Value.ToString()))
                        {
                            switch (Type & 0xf0)
                            {
                                case BASE_10:
                                    Text = SVUnit.Value.ToString();
                                    break;
                                case BASE_16:
                                    switch (SVUnit.Type)
                                    {
                                        case "WORD":
                                            Text = String.Format("{0:X8}", Int32.Parse(SVUnit.Value.ToString()));
                                            break;
                                        case "DWORD":
                                            Text = String.Format("{0:X16}", Int64.Parse(SVUnit.Value.ToString()));
                                            break;
                                        default:
                                            Text = SVUnit.Value.ToString();
                                            break;
                                    }
                                    break;
                            }
                        }
                        break;
                    default:
                        Text = String.Empty;
                        break;
                }
            });
            settingup = false;
        }

        #region Event Handler

        #region Text

        /// <summary>
        /// 当文本发生合法修改时，触发这个代理
        /// </summary>
        public event RoutedEventHandler TextLegalChanged = delegate { };

        /// <summary>
        /// 当文本修改时发生
        /// </summary>
        /// <param name="e">事件</param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (settingup)
            {
                return;
            }
            // 判断文本是否合法
            try
            {
                // 预设不合法的背景颜色（红色）
                Background = Brushes.Red;
                // 单个变量的名称（D0）
                Match m1 = Regex.Match(Text, @"^\w+\d+$");
                // 多个变量，即变量组的名称（D[0..100]）
                Match m2 = Regex.Match(Text, @"^\w+\[\d+\.\.\d+\]$");
                // 根据类型来判断
                switch (type&0X0f)
                {
                    // 检查名称是否合法
                    case TYPE_NAME:
                        // 不合法就退出
                        if (!m1.Success && !m2.Success)
                        {
                            return;
                        }
                        //svunit.Name = Text;
                        break;
                    // 检查别名是否合法
                    case TYPE_VAR:
                        // 输入即合法
                        svunit.Var = Text;
                        break;
                    // 检查数值是否合法
                    case TYPE_VALUE:
                        // 根据变量类型来判断
                        switch (svunit.Type)
                        {
                            // 类型为位（BIT）
                            case "BIT":
                                // 只有0和1才合法
                                if (!Regex.Match(Text, @"^[01]$").Success)
                                    return;
                                // 合法时修改值
                                svunit.Value = Int32.Parse(Text);
                                break;
                            // 类型为字（WORD）
                            case "WORD":
                                // 只有整数才合法
                                if (!Regex.Match(Text, @"^\d+$").Success)
                                    return;
                                svunit.Value = Int32.Parse(Text);
                                break;
                            // 类型为双字（DWORD）
                            case "DWORD":
                                // 只有整数才合法
                                if (!Regex.Match(Text, @"^\d+$").Success)
                                    return;
                                svunit.Value = Int64.Parse(Text);
                                // 双字在int范围内，所以无需检查范围
                                break;
                            case "FLOAT":
                                // 只有整数和浮点才合法
                                if (!Regex.Match(Text, @"^\d+(\.\d+)?$").Success)
                                    return;
                                // 能转换即合法
                                svunit.Value = double.Parse(Text);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                // 设置为合法的背景颜色（白色）
                Background = Brushes.White;
            }
            catch (FormatException)
            {
                return;
            }
            // 发送合法文本修改的事件
            // 数值需要等待Enter键键入才能发送
            if (Type != TYPE_VALUE)
            {
                TextLegalChanged(this, new RoutedEventArgs());
            }
            // 当然是原谅她
            else
            {
                Background = Brushes.LightGreen;
            }
        }

        #endregion

        #region KeyBoard Control

        /// <summary>
        /// 当需要往前新建一个变量时，触发这个代理
        /// </summary>
        public event RoutedEventHandler InsertRowElementBeforeHere = delegate { };
        /// <summary>
        /// 当需要往后新建一个变量时，触发这个代理
        /// </summary>
        public event RoutedEventHandler InsertRowElementAfterHere = delegate { };
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
                // 键入Enter时
                case Key.Enter:
                    switch (Type&0x0f)
                    {
                        // 名称框向后新建新的变量
                        case TYPE_NAME:
                            InsertRowElementAfterHere(this, new RoutedEventArgs());
                            break;
                        // 数值框确认数值是否正确并发送
                        case TYPE_VALUE:
                            if (Background == Brushes.LightGreen)
                            {
                                TextLegalChanged(this, new RoutedEventArgs());
                                Background = Brushes.White;
                            }
                            break;
                    }
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
        /// 当变量值更改时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnValueChanged(object sender, RoutedEventArgs e)
        {
            switch (Type&0x0f)
            {
                case TYPE_VALUE:
                    SetText();
                    break;
            }
        }

        /// <summary>
        /// 当变量锁定更改时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnLockChanged(object sender, RoutedEventArgs e)
        {
            switch (Type&0x0f)
            {
                case TYPE_VALUE:
                    IsReadOnly = (!SVUnit.Islocked);
                    OnValueChanged(sender, e);
                    break;
            }
        }

        #endregion

    }
}

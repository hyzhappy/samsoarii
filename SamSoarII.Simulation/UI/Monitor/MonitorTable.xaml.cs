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
using System.Xml.Linq;
using System.Threading;

using SamSoarII.Simulation.Core.Event;
using SamSoarII.Simulation.Core.VariableModel;

/// <summary>
/// Namespace : SamSoarII.Simulation.UI.Monitor
/// ClassName : MonitorTable
/// Version   : 1.0
/// Date      : 2017/4/12
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 监控变量的管理表格
/// 支持单独插入变量和批量插入变量（设为变量组）
/// 支持变量的实时修改，锁定，变量组的展开
/// </remarks>

namespace SamSoarII.Simulation.UI.Monitor
{
    /// <summary>
    /// MonitorTable.xaml 的交互逻辑
    /// </summary> 
    public partial class MonitorTable : UserControl, IDisposable
    {
        /// <summary>
        /// 监视表格中的行元素
        /// </summary>
        private class RowElement
        {
            #region Numbers

            #region Private
            /// <summary>
            /// 作为父亲的监视表格
            /// </summary>
            private MonitorTable parent;
            /// <summary>
            /// 对应的变量单元
            /// </summary>
            private SimulateVariableUnit svunit;
            /// <summary>
            /// 对应的表格内的行分割块
            /// </summary>
            private RowDefinition currentRowDefinition;
            /// <summary>
            /// 元素ID，对应行号
            /// </summary>
            private int id;
            /// <summary>
            /// 元素是否正在设置，屏蔽掉修改事件
            /// </summary>
            private bool settingup;
            /// <summary>
            /// 元素组件聚焦的位置
            /// </summary>
            private int focus;
            #endregion

            #region Public
            /// <summary>
            /// 元素组件：显示和修改变量名称的文本编辑框
            /// </summary>
            public MonitorTextBox TextBox_Name;
            /// <summary>
            /// 元素组件：显示和修改变量数据类型的选择框
            /// </summary>
            public MonitorComboBox ComboBox_Type;
            /// <summary>
            /// 元素组件：显示和修改变量别名的文本编辑框
            /// </summary>
            public MonitorTextBox TextBox_Var;
            /// <summary>
            /// 元素组件：显示和修改变量值的文本编辑框
            /// </summary>
            public MonitorTextBox TextBox_Value;
            /// <summary>
            /// 元素组件：打开（设为１）和关闭（设为０）位变量的交替按钮
            /// </summary>
            public MonitorBitButton Button_Value;
            /// <summary>
            /// 元素组件：对变量锁定和取消锁定的图标按钮
            /// </summary>
            public MonitorLockButton Button_Lock;
            /// <summary>
            /// 元素组件：对变量锁定和取消锁定的图标按钮
            /// </summary>
            public MonitorExpandButton Button_Expand;
            /// <summary>
            /// 元素组件：删除元素的图标按钮
            /// </summary>
            public MonitorCloseButton Button_Close;
            #endregion

            #endregion

            #region Numbers Interface

            /// <summary>
            /// 元素ID，对应行号
            /// </summary>
            public int ID
            {
                get
                {
                    return this.id;
                }
                set
                {
                    // 设置新的ID
                    this.id = value;
                    // 更改行分割块的位置
                    parent.MainGrid.RowDefinitions.Remove(currentRowDefinition);
                    parent.MainGrid.RowDefinitions.Insert(id-1, currentRowDefinition);
                    // 根据ID设置组件的行位置
                    Grid.SetRow(TextBox_Name, id);
                    Grid.SetRow(ComboBox_Type, id);
                    Grid.SetRow(TextBox_Var, id);
                    Grid.SetRow(TextBox_Value, id);
                    Grid.SetRow(Button_Value, id);
                    Grid.SetRow(Button_Lock, id);
                    Grid.SetRow(Button_Close, id);
                    Grid.SetRow(Button_Expand, id);
                }
            }

            #region Focus
            /// <summary> 聚焦状态的标志常量：未被聚焦，处于自由状态</summary>
            public const int FOCUS_NULL = 0x00;
            /// <summary> 聚焦状态的标志常量：聚焦于名称组件上</summary>
            public const int FOCUS_NAME = 0x01;
            /// <summary> 聚焦状态的标志常量：聚焦于类型组件上</summary>
            public const int FOCUS_TYPE = 0x02;
            /// <summary> 聚焦状态的标志常量：聚焦于别名组件上</summary>
            public const int FOCUS_VAR = 0x03;
            /// <summary> 聚焦状态的标志常量：聚焦于值组件上</summary>
            public const int FOCUS_VALUE = 0x04;
            /// <summary>
            /// 当前聚焦的状态
            /// </summary>
            public int Focus
            {
                get
                {
                    return this.focus;
                }
                set
                {
                    this.focus = value;
                    switch (focus)
                    {
                        case FOCUS_NAME:
                            TextBox_Name.Focus();
                            break;
                        case FOCUS_TYPE:
                            ComboBox_Type.Focus();
                            break;
                        case FOCUS_VAR:
                            TextBox_Var.Focus();
                            break;
                        case FOCUS_VALUE:
                            // 哪个显示聚焦哪个
                            if (TextBox_Value.Visibility == Visibility.Visible)
                            {
                                TextBox_Value.Focus();
                            }
                            if (Button_Value.Visibility == Visibility.Visible)
                            {
                                Button_Value.Focus();
                            }
                            break;
                    }
                }
            }
            #endregion

            #endregion

            /// <summary>
            /// 初始化构造函数
            /// </summary>
            /// <param name="_parent">表格</param>
            /// <param name="_id">ID</param>
            /// <param name="svunit">变量</param>
            public RowElement(MonitorTable _parent, int _id, SimulateVariableUnit svunit = null)
            {
                settingup = false;
                parent = _parent;
                id = _id;
                Install();
                /// 存在变量则安装
                if (svunit != null)
                {
                    Setup(svunit);
                }
            }

            /// <summary>
            /// 安装变量
            /// </summary>
            /// <param name="_svunit">变量</param>
            public void Setup(SimulateVariableUnit _svunit)
            {
                // 正在安装，请勿打扰
                settingup = true;
                this.svunit = _svunit;
                // 对所有组件安装变量
                TextBox_Name.SVUnit = svunit;
                ComboBox_Type.SVUnit = svunit;
                TextBox_Var.SVUnit = svunit;
                TextBox_Value.SVUnit = svunit;
                Button_Value.SVUnit = svunit;
                Button_Lock.SVUnit = svunit;
                Button_Expand.SVUnit = svunit;
                // 如果变量为空则隐藏，否则显示
                if (svunit == null)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
                // 请继续你的修改监听
                settingup = false;
            }
            
            /// <summary>
            /// 将元素安装到表格中
            /// </summary>
            public void Install()
            {
                // 新建所有组件
                TextBox_Name = new MonitorTextBox(MonitorTextBox.TYPE_NAME);
                ComboBox_Type = new MonitorComboBox(MonitorComboBox.TYPE_DATATYPE);
                TextBox_Var = new MonitorTextBox(MonitorTextBox.TYPE_VAR);
                TextBox_Value = new MonitorTextBox(MonitorTextBox.TYPE_VALUE);
                Button_Value = new MonitorBitButton();
                Button_Close = new MonitorCloseButton();
                Button_Lock = new MonitorLockButton();
                Button_Expand = new MonitorExpandButton();
                // 统一右键菜单
                TextBox_Name.ContextMenu = parent.RightClickMenu;
                ComboBox_Type.ContextMenu = parent.RightClickMenu;
                TextBox_Var.ContextMenu = parent.RightClickMenu;
                TextBox_Value.ContextMenu = parent.RightClickMenu;
                TextBox_Value.IsReadOnly = true;
                Button_Value.ContextMenu = parent.RightClickMenu;
                Button_Value.IsReadOnly = true;
                // 订购事件
                TextBox_Name.TextLegalChanged += OnTextLegalChanged;
                TextBox_Name.InsertRowElementBeforeHere += OnInsertRowBeforeHere;
                TextBox_Name.InsertRowElementAfterHere += OnInsertRowAfterHere;
                TextBox_Name.GotFocus += OnChildrenGotFocus;
                TextBox_Name.LostFocus += OnChildrenLostFocus;
                TextBox_Name.FocusUp += OnFocusUp;
                TextBox_Name.FocusDown += OnFocusDown;
                //TextBox_Name.FocusLeft += OnFocusLeft;
                //TextBox_Name.FocusRight += OnFocusRight;
                ComboBox_Type.TextLegalChanged += OnTextLegalChanged;
                ComboBox_Type.GotFocus += OnChildrenGotFocus;
                ComboBox_Type.LostFocus += OnChildrenLostFocus;
                ComboBox_Type.FocusUp += OnFocusUp;
                ComboBox_Type.FocusDown += OnFocusDown;
                //ComboBox_Type.FocusLeft += OnFocusLeft;
                //ComboBox_Type.FocusRight += OnFocusRight;
                TextBox_Var.TextLegalChanged += OnTextLegalChanged;
                TextBox_Var.GotFocus += OnChildrenGotFocus;
                TextBox_Var.LostFocus += OnChildrenLostFocus;
                TextBox_Var.FocusUp += OnFocusUp;
                TextBox_Var.FocusDown += OnFocusDown;
                //TextBox_Var.FocusLeft += OnFocusLeft;
                //TextBox_Var.FocusRight += OnFocusRight;
                TextBox_Value.TextLegalChanged += OnTextLegalChanged;
                TextBox_Value.GotFocus += OnChildrenGotFocus;
                TextBox_Value.LostFocus += OnChildrenLostFocus;
                TextBox_Value.FocusUp += OnFocusUp;
                TextBox_Value.FocusDown += OnFocusDown;
                //TextBox_Value.FocusLeft += OnFocusLeft;
                //TextBox_Value.FocusRight += OnFocusRight;
                Button_Value.TextLegalChanged += OnTextLegalChanged;
                Button_Expand.MouseDown += OnExpandButtonClicked;
                Button_Lock.VariableUnitLocked += OnVariableUnitLocked;
                Button_Lock.VariableUnitUnlocked += OnVariableUnitUnlocked;
                Button_Close.MouseUp += OnCloseButtonClicked;
                VariableUnitChanged += parent.OnVariableUnitChanged;
                VariableUnitClosed += parent.OnVariableUnitDeleted;
                VariableUnitLocked += parent.OnVariableUnitLocked;
                VariableUnitUnlocked += parent.OnVariableUnitUnlocked;
                VariableUnitExpanded += parent.OnVariableUnitExpanded;
                InsertRowElementBeforeHere += parent.OnInsertRowBeforeHere;
                InsertRowElementAfterHere += parent.OnInsertRowAfterHere;
                FocusUp += parent.OnFocusUp;
                FocusDown += parent.OnFocusDown;
                // 设置表格行列
                Grid.SetRow(TextBox_Name, id);
                Grid.SetRow(ComboBox_Type, id);
                Grid.SetRow(TextBox_Var, id);
                Grid.SetRow(TextBox_Value, id);
                Grid.SetRow(Button_Value, id);
                Grid.SetRow(Button_Lock, id);
                Grid.SetRow(Button_Close, id);
                Grid.SetRow(Button_Expand, id);
                Grid.SetColumn(TextBox_Name, 0);
                Grid.SetColumn(ComboBox_Type, 1);
                Grid.SetColumn(TextBox_Var, 2);
                Grid.SetColumn(TextBox_Value, 3);
                Grid.SetColumn(Button_Value, 3);
                Grid.SetColumn(Button_Lock, 4);
                Grid.SetColumn(Button_Expand, 4);
                Grid.SetColumn(Button_Close, 5);
                // 设置行分割块
                currentRowDefinition = new RowDefinition();
                currentRowDefinition.Height = new GridLength(24);
                // 加入到表格的组件集合中
                //parent.MainGrid.RowDefinitions.Add(currentRowDefinition);
                parent.MainGrid.RowDefinitions.Insert(id, currentRowDefinition);
                parent.MainGrid.Children.Add(TextBox_Name);
                parent.MainGrid.Children.Add(ComboBox_Type);
                parent.MainGrid.Children.Add(TextBox_Var);
                parent.MainGrid.Children.Add(TextBox_Value);
                parent.MainGrid.Children.Add(Button_Value);
                parent.MainGrid.Children.Add(Button_Lock);
                parent.MainGrid.Children.Add(Button_Close);
                parent.MainGrid.Children.Add(Button_Expand);
            }
            
            /// <summary>
            /// 卸载元素
            /// </summary>
            public void Uninstall()
            {
                // 取消事件
                TextBox_Name.TextLegalChanged -= OnTextLegalChanged;
                TextBox_Name.InsertRowElementBeforeHere -= OnInsertRowBeforeHere;
                TextBox_Name.InsertRowElementAfterHere -= OnInsertRowAfterHere;
                TextBox_Name.GotFocus -= OnChildrenGotFocus;
                TextBox_Name.LostFocus -= OnChildrenLostFocus;
                TextBox_Name.FocusUp -= OnFocusUp;
                TextBox_Name.FocusDown -= OnFocusDown;
                TextBox_Name.FocusLeft -= OnFocusLeft;
                TextBox_Name.FocusRight -= OnFocusRight;
                ComboBox_Type.TextLegalChanged -= OnTextLegalChanged;
                ComboBox_Type.GotFocus -= OnChildrenGotFocus;
                ComboBox_Type.LostFocus -= OnChildrenLostFocus;
                ComboBox_Type.FocusUp -= OnFocusUp;
                ComboBox_Type.FocusDown -= OnFocusDown;
                ComboBox_Type.FocusLeft -= OnFocusLeft;
                ComboBox_Type.FocusRight -= OnFocusRight;
                TextBox_Var.TextLegalChanged -= OnTextLegalChanged;
                TextBox_Var.GotFocus -= OnChildrenGotFocus;
                TextBox_Var.LostFocus -= OnChildrenLostFocus;
                TextBox_Var.FocusUp -= OnFocusUp;
                TextBox_Var.FocusDown -= OnFocusDown;
                TextBox_Var.FocusLeft -= OnFocusLeft;
                TextBox_Var.FocusRight -= OnFocusRight;
                TextBox_Value.TextLegalChanged -= OnTextLegalChanged;
                TextBox_Value.GotFocus -= OnChildrenGotFocus;
                TextBox_Value.LostFocus -= OnChildrenLostFocus;
                TextBox_Value.FocusUp -= OnFocusUp;
                TextBox_Value.FocusDown -= OnFocusDown;
                TextBox_Value.FocusLeft -= OnFocusLeft;
                TextBox_Value.FocusRight -= OnFocusRight;
                Button_Value.TextLegalChanged -= OnTextLegalChanged;
                Button_Expand.MouseDown -= OnExpandButtonClicked;
                Button_Close.MouseUp -= OnCloseButtonClicked;
                Button_Lock.VariableUnitLocked -= OnVariableUnitLocked;
                Button_Lock.VariableUnitUnlocked -= OnVariableUnitUnlocked;
                VariableUnitChanged -= parent.OnVariableUnitChanged;
                VariableUnitClosed -= parent.OnVariableUnitDeleted;
                VariableUnitLocked -= parent.OnVariableUnitLocked;
                VariableUnitUnlocked -= parent.OnVariableUnitUnlocked;
                VariableUnitExpanded -= parent.OnVariableUnitExpanded;
                InsertRowElementBeforeHere -= parent.OnInsertRowBeforeHere;
                InsertRowElementAfterHere -= parent.OnInsertRowAfterHere;
                FocusUp -= parent.OnFocusUp;
                FocusDown -= parent.OnFocusDown;
                // 从表格的组件集合中去除
                parent.MainGrid.Children.Remove(TextBox_Name);
                parent.MainGrid.Children.Remove(ComboBox_Type);
                parent.MainGrid.Children.Remove(TextBox_Var);
                parent.MainGrid.Children.Remove(TextBox_Value);
                parent.MainGrid.Children.Remove(Button_Value);
                parent.MainGrid.Children.Remove(Button_Lock);
                parent.MainGrid.Children.Remove(Button_Close);
                parent.MainGrid.RowDefinitions.Remove(currentRowDefinition);
            }
            
            /// <summary>
            /// 显示这个行元素
            /// </summary>
            public void Show()
            {
                // 显示所有必要组件
                TextBox_Name.Visibility = Visibility.Visible;
                TextBox_Var.Visibility = Visibility.Visible;
                ComboBox_Type.Visibility = Visibility.Visible;
                currentRowDefinition.Height = new GridLength(24);
                // 如果是变量组，只能展开不能锁定
                if (svunit is SimulateUnitSeries)
                {
                    SimulateUnitSeries ssunit = (SimulateUnitSeries)(svunit);
                    Button_Expand.IsExpanded = ssunit.IsExpand;
                    Button_Lock.Visibility = Visibility.Hidden;
                    Button_Expand.Visibility = Visibility.Visible;
                }
                // 如果是输入变量，不能锁定和展开
                else if (svunit is SimulateVInputUnit)
                {
                    Button_Lock.Visibility = Visibility.Hidden;
                    Button_Expand.Visibility = Visibility.Hidden;
                }
                // 如果是其他变量，只能锁定不能展开
                else
                {
                    Button_Lock.IsLocked = svunit.Islocked;
                    Button_Lock.Visibility = Visibility.Visible;
                    Button_Expand.Visibility = Visibility.Hidden;
                    // 如果是位变量，显示值开关按钮
                    if (svunit is SimulateBitUnit)
                    {
                        TextBox_Value.Visibility = Visibility.Hidden;
                        Button_Value.Visibility = Visibility.Visible;
                    }
                    // 如果是其他变量，显示值输入框
                    else
                    {
                        TextBox_Value.Visibility = Visibility.Visible;
                        Button_Value.Visibility = Visibility.Hidden;
                    }
                }
            }

            /// <summary>
            /// 隐藏这个行元素
            /// </summary>
            public void Hide()
            {
                // 隐藏所有组件
                TextBox_Name.Visibility = Visibility.Collapsed;
                TextBox_Var.Visibility = Visibility.Collapsed;
                TextBox_Value.Visibility = Visibility.Collapsed;
                ComboBox_Type.Visibility = Visibility.Collapsed;
                Button_Close.Visibility = Visibility.Collapsed;
                Button_Expand.Visibility = Visibility.Collapsed;
                Button_Lock.Visibility = Visibility.Collapsed;
                Button_Value.Visibility = Visibility.Collapsed;
                currentRowDefinition.Height = new GridLength(0);
            }

            #region Event Handler

            #region Variable Unit
            /// <summary>
            /// 当旧的变量更改成一个新的变量时，触发这个代理
            /// </summary>
            public event VariableUnitChangeEventHandler VariableUnitChanged = delegate { };
            
            /// <summary> 
            /// 当旧的变量关闭时，触发这个代理
            /// </summary>
            public event VariableUnitChangeEventHandler VariableUnitClosed = delegate { };

            /// <summary>
            /// 当要锁定当前变量时，触发这个代理
            /// </summary>
            public event VariableUnitChangeEventHandler VariableUnitLocked = delegate { };

            /// <summary>
            /// 当要解锁当前变量时，触发这个代理
            /// </summary>
            public event VariableUnitChangeEventHandler VariableUnitUnlocked = delegate { };

            /// <summary>
            /// 当要扩展当前变量时，触发这个代理
            /// </summary>
            public event RoutedEventHandler VariableUnitExpanded = delegate { };
            
            /// <summary>
            /// 当要在这个元素前添加一个新元素时，触发这个代理
            /// </summary>
            public event RoutedEventHandler InsertRowElementBeforeHere = delegate { };

            /// <summary>
            /// 当要在这个元素后添加一个新元素时，触发这个代理
            /// </summary>
            public event RoutedEventHandler InsertRowElementAfterHere = delegate { };
            
            /// <summary>
            /// 当输入框的文本更改时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnTextLegalChanged(object sender, RoutedEventArgs e)
            {
                // 安装和更新状态下，屏蔽掉这个事件
                if (settingup)
                {
                    return;
                }
                // 新建变量更改的事件
                VariableUnitChangeEventArgs _e = new VariableUnitChangeEventArgs();
                _e.Old = svunit;
                // 如果变量是个变量组
                if (svunit is SimulateUnitSeries)
                {
                    SimulateUnitSeries ssunit = (SimulateUnitSeries)(svunit);
                    // 发送源是类型选择框，修改的是类型
                    if (sender == ComboBox_Type)
                    {
                        // 获取选定的新类型的变量并安装替换
                        _e.New = ssunit.ChangeDataType(ComboBox_Type.Text);
                        Setup(_e.New);
                        VariableUnitChanged(this, _e);
                        return;
                    }
                }
                // 如果在锁定状态下按了值开关的按钮
                if (sender == Button_Value && Button_Lock.IsLocked)
                {
                    // 只是修改值不替换变量
                    switch (Button_Value.Status)
                    {
                        case MonitorBitButton.STATUS_ON:
                            svunit.Value = 1;
                            // 需要重新锁定
                            _e.New = svunit;
                            VariableUnitLocked(this, _e);
                            break;
                        case MonitorBitButton.STATUS_OFF:
                            svunit.Value = 0;
                            _e.New = svunit;
                            VariableUnitLocked(this, _e);
                            break;
                        case MonitorBitButton.STATUS_ERROR:
                            break;
                    }
                    return;
                }
                // 如果在锁定状态下输入更改了变量的值
                if (sender == TextBox_Value && Button_Lock.IsLocked)
                {
                    // 修改值
                    if (svunit is SimulateFloatUnit)
                    {
                        svunit.Value = float.Parse(TextBox_Value.Text);
                        _e.New = svunit;
                        VariableUnitLocked(this, _e);
                    }
                    else
                    {
                        svunit.Value = int.Parse(TextBox_Value.Text);
                        _e.New = svunit;
                        VariableUnitLocked(this, _e);
                    }
                    return;
                }
                // 新的变量
                SimulateVariableUnit _svunit = null;
                // 修改的是名称
                if (sender == TextBox_Name)
                {
                    // 进行第二层检查，预设为非法颜色
                    TextBox_Name.Background = Brushes.Red;
                    // 若和原变量相等则退出（视为合法）
                    if (svunit != null && TextBox_Name.Text.Equals(svunit.Name))
                    {
                        TextBox_Name.Background = Brushes.White;
                        return;
                    }
                    // 若存在同名变量则退出
                    IEnumerable<SimulateVariableUnit> nequ = parent.SVUnits.Where(
                        (SimulateVariableUnit svunit) =>
                            {
                                return svunit.Name.Equals(TextBox_Name.Text);
                            }
                    );
                    if (nequ.Count() > 0) return;
                    // 不能创建新的变量则退出
                    _svunit = SimulateVariableUnit.Create(TextBox_Name.Text);
                    if (_svunit == null) return;
                    // 设为合法颜色
                    TextBox_Name.Background = Brushes.White;
                }
                // 剩下的发送源默认是类型选择框，如果不是也可替换为现类型
                else
                {
                    switch (ComboBox_Type.Text)
                    {
                        case "BIT":
                            _svunit = new SimulateBitUnit();
                            break;
                        case "WORD":
                            _svunit = new SimulateWordUnit();
                            break;
                        case "DWORD":
                            _svunit = new SimulateDWordUnit();
                            break;
                        case "FLOAT":
                            _svunit = new SimulateFloatUnit();
                            break;
                        default:
                            return;
                    }
                }
                // 对新变量设置属性，安装并发送事件
                _svunit.Name = TextBox_Name.Text;
                //_svunit.Type = ComboBox_Type;
                _svunit.Var = TextBox_Var.Text;
                //_svunit.Value = TextBox_Value.Text;
                _e.New = _svunit;
                Setup(_svunit); 
                VariableUnitChanged(this, _e);
            }

            /// <summary>
            /// 当按下关闭按钮时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
            {
                // 正在锁定时先解锁
                if (Button_Lock.IsLocked)
                {
                    Button_Lock.IsLocked = false;
                }
                // 设置并发送变量更改事件，新变量为空表示删除
                VariableUnitChangeEventArgs _e = new VariableUnitChangeEventArgs();
                _e.Old = svunit;
                _e.New = null;
                VariableUnitClosed(this, _e);
                // 卸载该元素
                //Uninstall();
            }
            
            /// <summary>
            /// 当按下扩展按钮时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnExpandButtonClicked(object sender, RoutedEventArgs e)
            {
                VariableUnitExpanded(this, e);
            }
            
            /// <summary>
            /// 当要锁定当前变量时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnVariableUnitLocked(object sender, VariableUnitChangeEventArgs e)
            {
                Button_Value.IsReadOnly = false;
                TextBox_Value.IsReadOnly = false;
                VariableUnitLocked(this, e);
            }

            /// <summary>
            /// 当要解锁当前变量时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnVariableUnitUnlocked(object sender, VariableUnitChangeEventArgs e)
            {
                Button_Value.IsReadOnly = true;
                TextBox_Value.IsReadOnly = true;
                VariableUnitUnlocked(this, e);
            }

            /// <summary>
            /// 当要在这个元素前添加一个新元素时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnInsertRowBeforeHere(object sender, RoutedEventArgs e)
            {
                InsertRowElementBeforeHere(this, e);
            }
            
            /// <summary>
            /// 当要在这个元素后添加一个新元素时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnInsertRowAfterHere(object sender, RoutedEventArgs e)
            {
                InsertRowElementAfterHere(this, e);
            }
            
            #endregion

            #region Focus

            /// <summary>
            /// 当子组件获得焦点时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnChildrenGotFocus(object sender, RoutedEventArgs e)
            {
                // 根据发送源确定聚焦的组件
                if (sender == TextBox_Name)
                {
                    this.focus = FOCUS_NAME;
                }
                if (sender == ComboBox_Type)
                {
                    this.focus = FOCUS_TYPE;
                }
                if (sender == TextBox_Var)
                {
                    this.focus = FOCUS_VAR;
                }
                if (sender == TextBox_Value)
                {
                    this.focus = FOCUS_VALUE;
                }
            }

            /// <summary>
            /// 当子组件失去焦点时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnChildrenLostFocus(object sender, RoutedEventArgs e)
            {
                this.focus = FOCUS_NULL;
            }

            /// <summary>
            /// 当焦点上移时，触发这个代理
            /// </summary>
            public event RoutedEventHandler FocusUp = delegate { };
            /// <summary>
            /// 当焦点上移时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnFocusUp(object sender, RoutedEventArgs e)
            {
                FocusUp(this, e);
            }
            
            /// <summary>
            /// 当焦点下移时，触发这个代理
            /// </summary>
            public event RoutedEventHandler FocusDown = delegate { };
            /// <summary>
            /// 当焦点下移时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnFocusDown(object sender, RoutedEventArgs e)
            {
                FocusDown(this, e);
            }

            /// <summary>
            /// 当焦点左移时，触发这个代理
            /// </summary>
            public event RoutedEventHandler FocusLeft = delegate { };
            /// <summary>
            /// 当焦点左移时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnFocusLeft(object sender, RoutedEventArgs e)
            {
                // 如果焦点在最左边（名字）则返回到最右边
                if (this.Focus == FOCUS_NAME)
                {
                    this.Focus = FOCUS_VALUE;
                }
                // 否则直接左移
                else
                {
                    this.Focus--;
                }
                // 触发代理
                FocusLeft(this, e);
            }

            /// <summary>
            /// 当焦点右移时，触发这个代理
            /// </summary>
            public event RoutedEventHandler FocusRight = delegate { };
            /// <summary>
            /// 当焦点右移时发生
            /// </summary>
            /// <param name="sender">发送源</param>
            /// <param name="e">事件</param>
            private void OnFocusRight(object sender, RoutedEventArgs e)
            {
                // 如果焦点在最右边（名字）则返回到最左边
                if (this.Focus == FOCUS_VALUE)
                {
                    this.Focus = FOCUS_NAME;
                }
                // 否则直接右移
                else
                { 
                    this.Focus++;
                }
                // 触发代理
                FocusRight(this, e);
            }

            #endregion

            #endregion

            public override string ToString()
            {
                return String.Format("Row{0:d}:{SVUnit={1:s}}", ID, svunit);
            }
        }

        #region Numbers & Numbers Interface

        #region SVUnit List

        /// <summary>
        /// 所有变量单元的列表
        /// </summary>
        private List<SimulateVariableUnit> svunits;
        /// <summary>
        /// 所有变量单元的列表
        /// </summary>
        public List<SimulateVariableUnit> SVUnits
        {
            set
            {
                this.svunits = value;
                // 设置后更新界面
                Update();
            }
            // 获取变量列表时要包括变量值展开后的所有变量
            get
            {
                // 返回的结果
                List<SimulateVariableUnit> ret = new List<SimulateVariableUnit>();
                // 对于原列表中的所有变量
                foreach (SimulateVariableUnit svunit in svunits)
                {
                    // 加入到新列表中
                    ret.Add(svunit);
                    // 如果是变量组
                    if (svunit is SimulateUnitSeries)
                    {
                        SimulateUnitSeries sus = svunit as SimulateUnitSeries;
                        // 如果已经展开
                        if (sus.IsExpand)
                        {
                            // 加入展开后的所有变量
                            foreach (SimulateVariableUnit ssvunit in (SimulateVariableUnit[])(sus.Value))
                            {
                                ret.Add(ssvunit);
                            }
                        }
                    }
                }
                return ret;
            }
        }

        #endregion

        #region RowElements List

        /// <summary>
        /// 所有行元素的列表
        /// </summary>
        private LinkedList<RowElement> reles;
        /// <summary>
        /// 所有行元素的列表
        /// </summary>
        private LinkedList<RowElement> RowElements
        {
            get { return this.reles; }
            set { this.reles = value; }
        }

        #endregion
        
        #endregion

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public MonitorTable()
        {
            InitializeComponent();
            RowElements = new LinkedList<RowElement>();
            SVUnits = new List<SimulateVariableUnit>();
        }
        
        /// <summary>
        /// 析构函数
        /// </summary>
        public void Dispose()
        {
            // 卸载所有的行元素
            foreach (RowElement rele in reles)
            {
                rele.Uninstall();
            }
        }
        
        /// <summary>
        /// 更新界面，根据现有变量的列表创建行元素的列表
        /// </summary>
        public void Update()
        {
            // 变量和行元素各建一个迭代器
            IEnumerator<SimulateVariableUnit> iter1 = SVUnits.GetEnumerator();
            IEnumerator<RowElement> iter2 = reles.GetEnumerator();
            // 当前变量和当前行元素
            SimulateVariableUnit svunit = null;
            RowElement rele = null;
            // 迭代器的状态位，第一位为iter1是否到达结尾，第二位为iter2是否到达结尾
            int eoi = 0;

            // 两个迭代器都进入到第一个元素中
            if (!iter1.MoveNext()) eoi |= 1;
            if (!iter2.MoveNext()) eoi |= 2;
            // 两个迭代器都没结尾
            while (eoi == 0)
            { 
                // 取得迭代器值并设置变量
                svunit = iter1.Current;
                rele = iter2.Current;
                rele.Setup(svunit);
                // 两个迭代器前进
                if (!iter1.MoveNext()) eoi |= 1;
                if (!iter2.MoveNext()) eoi |= 2;
            }
            // iter2结尾，但iter1没到达结尾
            while ((eoi & 1) == 0)
            {
                // 取得变量，建立新的行元素并设置
                svunit = iter1.Current;
                rele = new RowElement(this, reles.Count() + 1, svunit);
                reles.AddLast(rele);
                // iter1前进
                if (!iter1.MoveNext()) eoi |= 1;
            }
            // iter1结尾，但iter2没到达结尾
            while ((eoi & 2) == 0)
            {
                // 取得行元素，并变量单元为空
                rele = iter2.Current;
                rele.Setup(null);
                if (!iter2.MoveNext()) eoi |= 2;
            }
        }

        #region Save & Load
        
        /// <summary>
        /// 保存当前监视列表，以XML文档的形式
        /// </summary>
        /// <param name="fileName">保存文件路径</param>
        /// <returns>结果标号（0成功，其他为各种错误）</returns>
        public int Save(string fileName)
        {
            // 建立XML里的各种节点
            XDocument xdoc = new XDocument();
            XElement node_Root = new XElement("Monitor");
            XElement node_SVUnit = null;
            XElement node_SSVUnit = null;
            // 保存所有的变量
            foreach (SimulateVariableUnit svunit in svunits)
            {
                node_SVUnit = new XElement("SimulateVariableUnit");
                // 保存变量组
                if (svunit is SimulateUnitSeries)
                {
                    SimulateUnitSeries ssunit = (SimulateUnitSeries)(svunit);
                    node_SVUnit.SetAttributeValue("Inherited", "SimulateUnitSeries");
                    node_SVUnit.SetAttributeValue("Name", ssunit.Name);
                    node_SVUnit.SetAttributeValue("DataType", ssunit.DataType);
                    foreach (SimulateVariableUnit ssvunit in (SimulateVariableUnit[])(ssunit.Value))
                    {
                        node_SSVUnit = new XElement("SimulateVariableUnit");
                        SaveSVUnit(node_SSVUnit, ssvunit);
                        node_SVUnit.Add(node_SSVUnit);
                    }
                }
                else
                {
                    // 保存一般变量
                    SaveSVUnit(node_SVUnit, svunit);
                }

                node_Root.Add(node_SVUnit);
            }
            xdoc.Add(node_Root);
            // 保存文件
            xdoc.Save(fileName);
            // 返回成功
            return 0;
        }

        /// <summary>
        /// 保存一般变量到XML节点中
        /// </summary>
        /// <param name="node">XML文档节点</param>
        /// <param name="svunit">变量</param>
        private void SaveSVUnit(XElement node, SimulateVariableUnit svunit)
        {
            // 保存类型
            if (svunit is SimulateBitUnit)
            {
                node.SetAttributeValue("Inherited", "SimulateBitUnit");
            }
            if (svunit is SimulateWordUnit)
            {
                node.SetAttributeValue("Inherited", "SimulateWordUnit");
            }
            if (svunit is SimulateDWordUnit)
            {
                node.SetAttributeValue("Inherited", "SimulateDWordUnit");
            }
            if (svunit is SimulateFloatUnit)
            {
                node.SetAttributeValue("Inherited", "SimulateFloatUnit");
            }
            if (svunit is SimulateVInputUnit)
            {
                node.SetAttributeValue("Inherited", "SimulateVInputUnit");
                return;
            }
            // 保存属性
            node.SetAttributeValue("Name", svunit.Name);
            node.SetAttributeValue("Var", svunit.Var);
            node.SetAttributeValue("Value", svunit.Value);
        }

        /// <summary>
        /// 读取XML文档的信息，建立新的监视列表
        /// </summary>
        /// <param name="fileName">要读取的文件路径</param>
        /// <returns>是否成功</returns>
        public int Load(string fileName)
        {
            // 打开文档
            XDocument xdoc = XDocument.Load(fileName);
            XElement node_Root = xdoc.Element("Monitor");
            IEnumerable<XElement> nodes_SVUnit = node_Root.Elements("SimulateVariableUnit");
            // 初始化列表
            Clear();
            // 依次加入读取的变量
            foreach (XElement node_SVUnit in nodes_SVUnit)
            {
                SimulateVariableUnit svunit = LoadSVUnit(node_SVUnit);
                Add(svunit);
            }
            // 更新界面
            Update();
            // 返回成功
            return 0;
        }
        
        /// <summary>
        /// 从XML节点中读取变量
        /// </summary>
        /// <param name="node">XML文档节点</param>
        /// <param name="svunit">变量</param>
        private SimulateVariableUnit LoadSVUnit(XElement node)
        {
            SimulateVariableUnit ret = null;
            IEnumerable<XElement> snodes = null;
            List<SimulateVariableUnit> ssvunits = null;
            string inherited = node.Attribute("Inherited").Value as string;
            // 根据读取的类型建立对应的类型对象
            switch (inherited)
            {
                case "SimulateBitUnit":
                    ret = new SimulateBitUnit();
                    break;
                case "SimulateWordUnit":
                    ret = new SimulateWordUnit();
                    break;
                case "SimulateDWordUnit":
                    ret = new SimulateDWordUnit();
                    break;
                case "SimulateFloatUnit":
                    ret = new SimulateFloatUnit();
                    break;
                // 输入类型直接返回
                case "SimulateVInputUnit":
                    ret = new SimulateVInputUnit();
                    return ret;
                // 变量组需要读取子变量
                case "SimulateUnitSeries":
                    snodes = node.Elements("SimulateVariableUnit");
                    ssvunits = new List<SimulateVariableUnit>();
                    foreach (XElement snode in snodes)
                    {
                        ssvunits.Add(LoadSVUnit(snode));
                    }
                    ret = new SimulateUnitSeries(SimulateVariableModel.Create(ssvunits));
                    //((SimulateUnitSeries)(ret)).DataType = node.Attribute("DataType").Value;
                    ret.Name = node.Attribute("Name").Value;
                    return ret;
            }
            // 读取属性并返回
            ret.Name = node.Attribute("Name").Value;
            ret.Var = node.Attribute("Var").Value;
            return ret;
        }

        #endregion

        #region SVUnit Change

        /// <summary>
        /// 添加新的变量
        /// </summary>
        /// <param name="svunit"></param>
        public void Add(SimulateVariableUnit svunit)
        {
            // 建立添加事件并触发
            VariableUnitChangeEventArgs e = new VariableUnitChangeEventArgs();
            e.Old = null;
            e.New = svunit;
            OnVariableUnitCreated(this, e);
        }

        /// <summary>
        /// 删除变量
        /// </summary>
        /// <param name="svunit"></param>
        public void Remove(SimulateVariableUnit svunit)
        {
            // 建立删除事件并触发
            VariableUnitChangeEventArgs e = new VariableUnitChangeEventArgs();
            e.Old = svunit;
            e.New = null;
            OnVariableUnitDeleted(this, e);
        }

        /// <summary>
        /// 清除所有变量
        /// </summary>
        public void Clear()
        {
            // 对当前所有变量触发一次删除事件
            VariableUnitChangeEventArgs e = new VariableUnitChangeEventArgs();   
            foreach (SimulateVariableUnit svunit in svunits)
            {
                e.Old = svunit;
                e.New = null;
                VariableUnitChanged(this, e);
            }
            // 清空列表
            svunits.Clear();
        }

        /// <summary>
        /// 替换变量
        /// </summary>
        /// <param name="svunit_old">旧变量</param>
        /// <param name="svunit_new">新变量</param>
        public void Replace(SimulateVariableUnit svunit_old, SimulateVariableUnit svunit_new)
        {
            // 建立替换变量的事件
            VariableUnitChangeEventArgs e = new VariableUnitChangeEventArgs();
            e.Old = svunit_old;
            e.New = svunit_new;
            OnVariableUnitChanged(this, e);
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// 当变量更改时，触发这个代理
        /// </summary>
        public event VariableUnitChangeEventHandler VariableUnitChanged = delegate { };

        /// <summary>
        /// 当变量锁定时，触发这个代理
        /// </summary>
        public event VariableUnitChangeEventHandler VariableUnitLocked = delegate { };

        /// <summary>
        /// 当变量解锁时，触发这个代理
        /// </summary>
        public event VariableUnitChangeEventHandler VariableUnitUnlocked = delegate { };

        /// <summary>
        /// 当变量组扩展时，触发这个代理
        /// </summary>
        public event RoutedEventHandler VariableUnitExpanded;

        /// <summary>
        /// 当替换变量时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnVariableUnitChanged(object sender, VariableUnitChangeEventArgs e)
        {
            // 获得旧变量的标号，修改标号中的相应列表
            int id = svunits.IndexOf(e.Old);
            svunits[id] = e.New;
            // 触发相应的代理
            VariableUnitChanged(sender, e);
        }

        /// <summary>
        /// 当新建变量时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnVariableUnitCreated(object sender, VariableUnitChangeEventArgs e)
        {
            // 加入新的变量
            svunits.Add(e.New);
            // 更新界面
            Update();
            // 触发相应的代理
            VariableUnitChanged(sender, e);
        }

        /// <summary>
        /// 当删除变量时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnVariableUnitDeleted(object sender, VariableUnitChangeEventArgs e)
        {
            // 删除旧的变量
            svunits.Remove(e.Old);
            // 更新界面
            Update();
            // 触发相应的代理
            VariableUnitChanged(sender, e);
        }
        
        /// <summary>
        /// 当锁定变量时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnVariableUnitLocked(object sender, VariableUnitChangeEventArgs e)
        {
            // 触发代理
            VariableUnitLocked(sender, e);
        }
        
        /// <summary>
        /// 当解锁变量时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnVariableUnitUnlocked(object sender, VariableUnitChangeEventArgs e)
        {
            // 触发代理
            VariableUnitUnlocked(sender, e);
        }
        
        /// <summary>
        /// 当变量组扩展时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnVariableUnitExpanded(object sender, RoutedEventArgs e)
        {
            // 更新界面
            Update();
            // 触发相应的代理
            VariableUnitExpanded(sender, e);
        }

        /// <summary>
        /// 在当前变量前输入变量时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnInsertRowBeforeHere(object sender, RoutedEventArgs e)
        {
            // 发送源必须是行元素，要在行元素后加入
            if (sender is RowElement)
            {
                RowElement rele = (RowElement)(sender);
                // 建立新的输入变量并加入
                SimulateVariableUnit _svunit = new SimulateVInputUnit();
                svunits.Insert(rele.ID - 1, _svunit);
                // 更新界面
                Update();
                // 焦点上移到新建的行元素
                OnFocusUp(sender, e);
            }
        }
        
        /// <summary>
        /// 在当前变量后输入变量时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnInsertRowAfterHere(object sender, RoutedEventArgs e)
        {
            // 发送源必须是行元素，要在行元素后加入
            if (sender is RowElement)
            {
                RowElement rele = (RowElement)(sender);
                // 建立新的输入变量并加入
                SimulateVariableUnit _svunit = new SimulateVInputUnit();
                svunits.Insert(rele.ID, _svunit);
                // 更新界面
                Update();
                // 焦点下移到新建的行元素
                OnFocusDown(sender, e);
            }
        }

        /// <summary>
        /// 当焦点上移时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnFocusUp(object sender, RoutedEventArgs e)
        {
            // 发送源必须是行元素
            if (sender is RowElement)
            {
                RowElement rele = (RowElement)(sender);
                LinkedListNode<RowElement> node = reles.Find(rele);
                LinkedListNode<RowElement> nodep = node.Previous;
                if (nodep != null)
                {
                    RowElement relep = nodep.Value;
                    relep.Focus = rele.Focus;
                }
            }
        }

        /// <summary>
        /// 当焦点下移时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void OnFocusDown(object sender, RoutedEventArgs e)
        {
            if (sender is RowElement)
            {
                RowElement rele = (RowElement)(sender);
                LinkedListNode<RowElement> node = reles.Find(rele);
                LinkedListNode<RowElement> noden = node.Next;
                if (noden != null)
                {
                    RowElement relen = noden.Value;
                    relen.Focus = rele.Focus;
                }
            }
        }
        
        #endregion
    }
}

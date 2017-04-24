using SamSoarII.Simulation.Core.Event;
using SamSoarII.Simulation.Core.VariableModel;
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

/// <summary>
/// Namespace : SamSoarII.Simulation.UI.Monitor
/// ClassName : MonitorTextBox
/// Version   : 1.0
/// Date      : 2017/4/11
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 监视变量列表中行元素的值的锁定交替按钮
/// 当锁定时按一下解锁
/// 当未锁定时按一下锁定
/// </remarks>

namespace SamSoarII.Simulation.UI.Monitor
{
    /// <summary>
    /// MonitorLockButton.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorLockButton : UserControl
    {
        #region Numbers & Numbers Interface

        #region Simulate Variable Unit

        /// <summary>
        /// 变量单元
        /// </summary>
        private SimulateVariableUnit svunit;

        /// <summary>
        /// 变量单元
        /// </summary>
        public SimulateVariableUnit SVUnit
        {
            get { return this.svunit; }
            set
            {
                if (svunit != null)
                {
                    svunit.LockChanged -= OnLockChanged;
                }
                this.svunit = value;
                if (svunit != null)
                {
                    //IsLocked = svunit.Islocked;
                    svunit.LockChanged += OnLockChanged;
                }
            }

        }

        #endregion

        #region Lock
        
        /// <summary> 状态常量：未锁定</summary>
        public const int STATUS_LOCKOFF = 0x00;
        /// <summary> 状态常量：正在锁定</summary>
        public const int STATUS_LOCKON = 0x01;
        /// <summary>
        /// 按钮状态
        /// </summary>
        private int status;
        /// <summary>
        /// 当前变量是否锁定（和按钮状态相关）
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return (status == 1);
            }
            set
            {
                // 有可能触发变量事件，建立事件类
                VariableUnitChangeEventArgs e = new VariableUnitChangeEventArgs();
                e.Old = e.New = SVUnit;
                // 根据是否锁定来设置状态值
                status = value ? STATUS_LOCKON : STATUS_LOCKOFF;
                // 判断当前状态
                switch (status)
                {
                    // 未锁定时
                    case STATUS_LOCKOFF:
                        // 显示未锁定的图标
                        Image_LockOff.Opacity = 0.3;
                        Image_LockOn.Opacity = 0.0;
                        // 变量存在且锁定时解锁
                        if (SVUnit != null && SVUnit.Islocked)
                        {
                            SVUnit.Islocked = false;
                            VariableUnitUnlocked(this, e);
                        }
                        break;
                    // 锁定时
                    case STATUS_LOCKON:
                        // 显示锁定的图标
                        Image_LockOff.Opacity = 0.0;
                        Image_LockOn.Opacity = 0.8;
                        // 变量存在且未锁定时则锁定
                        if (SVUnit != null && !SVUnit.Islocked)
                        {
                            SVUnit.Islocked = true;
                            VariableUnitLocked(this, e);
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
        public MonitorLockButton()
        {
            InitializeComponent();
            IsLocked = false;
        }

        #region Event Handler

        #region Lock

        /// <summary>
        /// 变量锁定时，触发这个代理
        /// </summary>
        public event VariableUnitChangeEventHandler VariableUnitLocked = delegate { };

        /// <summary>
        /// 变量解锁时，触发这个代理
        /// </summary>
        public event VariableUnitChangeEventHandler VariableUnitUnlocked = delegate { };

        private void OnLockChanged(object sender, RoutedEventArgs e)
        {
            IsLocked = svunit.Islocked;
        }

        #endregion

        #region Mouse Control

        /// <summary>
        /// 当鼠标进入按钮时发生
        /// </summary>
        /// <param name="e">鼠标事件</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            switch (status)
            {
                case STATUS_LOCKOFF:
                    Image_LockOff.Opacity = 0.8;
                    break;
                case STATUS_LOCKON:
                    Image_LockOn.Opacity = 0.3;
                    break;
            }
        }

        /// <summary>
        /// 当鼠标离开按钮时发生
        /// </summary>
        /// <param name="e">事件</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            switch (status)
            {
                case STATUS_LOCKOFF:
                    Image_LockOff.Opacity = 0.3;
                    break;
                case STATUS_LOCKON:
                    Image_LockOn.Opacity = 0.8;
                    break;
            }
        }

        /// <summary>
        /// 当鼠标左键点击时发生
        /// </summary>
        /// <param name="e">鼠标事件</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            switch (status)
            {
                case STATUS_LOCKOFF:
                    IsLocked = true;
                    break;
                case STATUS_LOCKON:
                    IsLocked = false;
                    break;
            }
        }

        #endregion

        #endregion

    }
}

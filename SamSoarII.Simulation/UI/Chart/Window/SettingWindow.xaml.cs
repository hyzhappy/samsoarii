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

using SamSoarII.Simulation.Core.Global;

namespace SamSoarII.Simulation.UI.Chart.Window
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : System.Windows.Window
    {
        public bool? CB_ScanPeriod_IsChecked
        {
            get
            {
                return CB_ScanPeriod.IsChecked;
            }
            set
            {
                CB_ScanPeriod.IsChecked = value;
                switch (value)
                {
                    case true:
                        TB_ScanPeriod.IsReadOnly = false;
                        CB_ScanRate.IsChecked = false;
                        TB_ScanRate.IsReadOnly = true;
                        break;
                    case false:
                        TB_ScanPeriod.IsReadOnly = true;
                        CB_ScanRate.IsChecked = true;
                        TB_ScanRate.IsReadOnly = false;
                        break;
                }
            }
        }

        public bool? CB_ScanRate_IsChecked
        {
            get
            {
                return CB_ScanRate.IsChecked;
            }
            set
            {
                CB_ScanRate.IsChecked = value;
                switch (value)
                {
                    case true:
                        TB_ScanRate.IsReadOnly = false;
                        CB_ScanPeriod.IsChecked = false;
                        TB_ScanPeriod.IsReadOnly = true;
                        break;
                    case false:
                        TB_ScanRate.IsReadOnly = true;
                        CB_ScanPeriod.IsChecked = true;
                        TB_ScanPeriod.IsReadOnly = false;
                        break;
                }
            }
        }

        public SettingWindow()
        {
            InitializeComponent();
            TB_DataLimit.Text = GlobalSetting.ScanDataMaximum.ToString();
            TB_Divide.Text = GlobalSetting.TimeRulerDivideNumber.ToString();
            TB_LockLimit.Text = GlobalSetting.ScanLockMaximum.ToString();
            TB_ScanPeriod.Text = GlobalSetting.ScanPeriodConst.ToString();
            TB_ScanRate.Text = GlobalSetting.ScanPeriodRate.ToString();
            TB_SubDivide.Text = GlobalSetting.TimeRulerSubDivideNumber.ToString();
            TB_TimeEnd.Text = GlobalSetting.TimeRulerEnd.ToString();
            TB_TimeStart.Text = GlobalSetting.TimeRulerStart.ToString();
            TB_ValueDivide.Text = GlobalSetting.DrawValueDivide.ToString();
            TB_ValueSubDivide.Text = GlobalSetting.DrawValueSubDivide.ToString();
            TB_ViewLimit.Text = GlobalSetting.ScanViewMaximum.ToString();
            TB_DrawLimit.Text = GlobalSetting.DrawMaximum.ToString();
            TB_DrawAccurate.Text = GlobalSetting.DrawAccurate.ToString();
            Brush1.Fill = GlobalSetting.DrawBrushes[0];
            Brush2.Fill = GlobalSetting.DrawBrushes[1];
            Brush3.Fill = GlobalSetting.DrawBrushes[2];
            Brush4.Fill = GlobalSetting.DrawBrushes[3];
            Brush5.Fill = GlobalSetting.DrawBrushes[4];
            Brush6.Fill = GlobalSetting.DrawBrushes[5];
            Brush7.Fill = GlobalSetting.DrawBrushes[6];
            Brush8.Fill = GlobalSetting.DrawBrushes[7];
            //CB_TimeStartUnit.Text = "ms";
            //CB_TimeEndUnit.Text = "ms";
            int _ScanPeriodType = GlobalSetting.ScanPeriodType;
            switch (_ScanPeriodType)
            {
                case GlobalSetting.SCANPERIOD_CONST:
                    CB_ScanPeriod_IsChecked = true;
                    CB_ScanRate_IsChecked = false;
                    break;
                case GlobalSetting.SCANPERIOD_RATE:
                    CB_ScanPeriod_IsChecked = false;
                    CB_ScanRate_IsChecked = true;
                    break;
                default:
                    CB_ScanPeriod_IsChecked = false;
                    CB_ScanRate_IsChecked = false;
                    break;
            }
        }

        #region Event Handler
        public event RoutedEventHandler EnsureButtonClick;
        private void B_ok_Click(object sender, RoutedEventArgs e)
        {
            int _ScanDataMaximum = 0;
            try
            {
                if (CB_DataLimit.IsChecked == true)
                {
                    _ScanDataMaximum = int.Parse(TB_DataLimit.Text);
                    if (_ScanDataMaximum < 1 || _ScanDataMaximum > 65536)
                        throw new ArgumentOutOfRangeException();
                }
                else
                {
                    _ScanDataMaximum = 0x3fffffff;
                }
            }
            catch (FormatException fe)
            {
                MessageBox.Show(String.Format("最大数据数的格式非法！"));
                return;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                MessageBox.Show(String.Format("最大数据数的数值超出范围！合法范围为(1024-65536)"));
                return;
            }
            int _TimeRulerDivideNumber = 0;
            try
            {
                _TimeRulerDivideNumber = int.Parse(TB_Divide.Text);
                if (_TimeRulerDivideNumber < 2 || _TimeRulerDivideNumber > 40)
                    throw new ArgumentOutOfRangeException();
            }
            catch (FormatException fe)
            {
                MessageBox.Show(String.Format("时间轴主划分数量的格式非法！"));
                return;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                MessageBox.Show(String.Format("时间轴主划分数量的数值超出范围！合法范围为(2-40)"));
                return;
            }
            int _ScanLockMaximum = 0;
            try
            {
                if (CB_LockLimit.IsChecked == true)
                {
                    _ScanLockMaximum = int.Parse(TB_LockLimit.Text);
                    if (_ScanLockMaximum < 1 || _ScanLockMaximum > 32)
                        throw new ArgumentOutOfRangeException();
                }
                else
                {
                    _ScanLockMaximum = 0x3fffffff;
                }
            }
            catch (FormatException fe)
            {
                MessageBox.Show(String.Format("最大锁定数的格式非法！"));
                return;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                MessageBox.Show(String.Format("最大锁定数的数值超出范围！合法范围为(1-32)"));
                return;
            }
            int _ScanPariodType = 0;
            if (CB_ScanPeriod_IsChecked == true)
            {
                _ScanPariodType = GlobalSetting.SCANPERIOD_CONST;
            }
            else if (CB_ScanRate_IsChecked == true)
            {
                _ScanPariodType = GlobalSetting.SCANPERIOD_RATE;
            }
            else
            {
                MessageBox.Show(String.Format(""));
                return;
            }
            int _ScanPeriodConst = 0;
            try
            {
                _ScanPeriodConst = int.Parse(TB_ScanPeriod.Text);
                if (_ScanPeriodConst < 1 || _ScanPeriodConst > 100)
                    throw new ArgumentOutOfRangeException();
            }
            catch (FormatException fe)
            {
                if (_ScanPariodType == GlobalSetting.SCANPERIOD_CONST)
                {
                    MessageBox.Show(String.Format("扫描周期（常量）的格式非法！"));
                    return;
                }
            }
            catch (ArgumentOutOfRangeException ae)
            {
                if (_ScanPariodType == GlobalSetting.SCANPERIOD_CONST)
                {
                    MessageBox.Show(String.Format("扫描周期（常量）的数值超出范围！合法范围为(1-100)"));
                    return;
                }
            }
            int _ScanPeriodRate = 0;
            try
            {
                _ScanPeriodRate = int.Parse(TB_ScanRate.Text);
                if (_ScanPeriodRate < 32 || _ScanPeriodRate > 1024)
                    throw new ArgumentOutOfRangeException();
            }
            catch (FormatException fe)
            {
                if (_ScanPariodType == GlobalSetting.SCANPERIOD_RATE)
                {
                    MessageBox.Show(String.Format("扫描周期（比例）的格式非法！"));
                    return;
                }
            }
            catch (ArgumentOutOfRangeException ae)
            {
                if (_ScanPariodType == GlobalSetting.SCANPERIOD_RATE)
                {
                    MessageBox.Show(String.Format("扫描周期（比例）的数值超出范围！合法范围为(32-1024)"));
                    return;
                }
            }
            int _TimeRulerSubDivideNumber = 0;
            try
            {
                _TimeRulerSubDivideNumber = int.Parse(TB_SubDivide.Text);
                if (_TimeRulerSubDivideNumber < 2 || _TimeRulerSubDivideNumber > 10)
                    throw new ArgumentOutOfRangeException();
            }
            catch (FormatException fe)
            {
                MessageBox.Show(String.Format("时间轴副划分数量的格式非法！"));
                return;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                MessageBox.Show(String.Format("时间轴副划分数量的数值超出范围！合法范围为(2-10)"));
                return;
            }
            int _TimeRulerStart = 0;
            try
            {
                _TimeRulerStart = int.Parse(TB_TimeStart.Text);
                if (CB_TimeStartUnit.Text.Equals("s"))
                    _TimeRulerStart *= 1000;
                if (_TimeRulerStart < 0 || _TimeRulerStart > 600000)
                    throw new ArgumentOutOfRangeException();
            }
            catch (FormatException fe)
            {
                MessageBox.Show(String.Format("时间轴起点的格式非法！"));
                return;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                MessageBox.Show(String.Format("时间轴起点的数值超出范围！合法范围为(0-600s)"));
                return;
            }
            int _TimeRulerEnd = 0;
            try
            {
                _TimeRulerEnd = int.Parse(TB_TimeEnd.Text);
                if (CB_TimeEndUnit.Text.Equals("s"))
                    _TimeRulerEnd *= 1000;
                if (_TimeRulerEnd < 0 || _TimeRulerEnd > 600000)
                    throw new ArgumentOutOfRangeException();
            }
            catch (FormatException fe)
            {
                MessageBox.Show(String.Format("时间轴终点的格式非法！"));
                return;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                MessageBox.Show(String.Format("时间轴终点的数值超出范围！合法范围为(0-600s)"));
                return;
            }
            int _DrawValueDivide = 0;
            try
            {
                _DrawValueDivide = int.Parse(TB_ValueDivide.Text);
                if (_DrawValueDivide < 2 || _DrawValueDivide > 100)
                    throw new ArgumentOutOfRangeException();
            }
            catch (FormatException fe)
            {
                MessageBox.Show(String.Format("数值主划分数量的格式非法！"));
                return;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                MessageBox.Show(String.Format("数值主划分数量超出范围！合法范围为(2-100)"));
                return;
            }
            int _DrawValueSubDivide = 0;
            try
            {
                _DrawValueSubDivide = int.Parse(TB_ValueSubDivide.Text);
                if (_DrawValueSubDivide < 2 || _DrawValueSubDivide > 10)
                    throw new ArgumentOutOfRangeException();
            }
            catch (FormatException fe)
            {
                MessageBox.Show(String.Format("数值副划分数量的格式非法！"));
                return;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                MessageBox.Show(String.Format("数值副划分数量超出范围！合法范围为(2-10)"));
                return;
            }
            int _ScanViewMaximum = 0;
            try
            {
                if (CB_ViewLimit.IsChecked == true)
                {
                    _ScanViewMaximum = int.Parse(TB_ViewLimit.Text);
                    if (_ScanViewMaximum < 1 || _ScanViewMaximum > 256)
                        throw new ArgumentOutOfRangeException();
                }
                else
                {
                    _ScanViewMaximum = 0x3fffffff;
                }
            }
            catch (FormatException fe)
            {
                MessageBox.Show(String.Format("最大监视数的格式非法！"));
                return;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                MessageBox.Show(String.Format("最大监视数的数值超出范围！合法范围为(1-256)"));
                return;
            }
            int _DrawMaximum = 0;
            try
            {
                _DrawMaximum = int.Parse(TB_DrawLimit.Text);
                if (_DrawMaximum < 1 || _DrawMaximum > 32)
                    throw new ArgumentOutOfRangeException();
            }
            catch (FormatException fe)
            {
                MessageBox.Show(String.Format("最大曲线数的格式非法！"));
                return;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                MessageBox.Show(String.Format("最大曲线数的数值超出范围！合法范围为(1-32)"));
                return;
            }
            int _DrawAccurate = 0;
            try
            {
                _DrawAccurate = int.Parse(TB_DrawAccurate.Text);
                if (_DrawAccurate < 32 || _DrawAccurate > 2048)
                    throw new ArgumentOutOfRangeException();
            }
            catch (FormatException fe)
            {
                MessageBox.Show(String.Format("曲线绘制精度的格式非法！"));
                return;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                MessageBox.Show(String.Format("曲线绘制精度的数值超出范围！合法范围为(32-2048)"));
                return;
            }

            GlobalSetting.ScanDataMaximum = _ScanDataMaximum;
            GlobalSetting.TimeRulerDivideNumber = _TimeRulerDivideNumber;
            GlobalSetting.ScanLockMaximum = _ScanLockMaximum;
            GlobalSetting.ScanPeriodType = _ScanPariodType;
            GlobalSetting.ScanPeriodConst = _ScanPeriodConst;
            GlobalSetting.ScanPeriodRate = _ScanPeriodRate;
            GlobalSetting.TimeRulerSubDivideNumber = _TimeRulerSubDivideNumber;
            GlobalSetting.TimeRulerEnd = _TimeRulerEnd;
            GlobalSetting.TimeRulerStart = _TimeRulerStart;
            GlobalSetting.DrawValueDivide = _DrawValueDivide;
            GlobalSetting.DrawValueSubDivide = _DrawValueSubDivide;
            GlobalSetting.ScanViewMaximum = _ScanViewMaximum;
            GlobalSetting.DrawMaximum = _DrawMaximum;
            GlobalSetting.DrawAccurate = _DrawAccurate;
            GlobalSetting.DrawBrushes[0] = Brush1.Fill;
            GlobalSetting.DrawBrushes[1] = Brush2.Fill;
            GlobalSetting.DrawBrushes[2] = Brush3.Fill;
            GlobalSetting.DrawBrushes[3] = Brush4.Fill;
            GlobalSetting.DrawBrushes[4] = Brush5.Fill;
            GlobalSetting.DrawBrushes[5] = Brush6.Fill;
            GlobalSetting.DrawBrushes[6] = Brush7.Fill;
            GlobalSetting.DrawBrushes[7] = Brush8.Fill;
            Close();
            if (EnsureButtonClick != null)
            {
                EnsureButtonClick(this, e);
            }
        }

        public event RoutedEventHandler CancelButtonClick;
        private void B_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
            if (CancelButtonClick != null)
            {
                CancelButtonClick(this, e);
            }
        }

        private void CB_ScanPeriod_Click(object sender, RoutedEventArgs e)
        {
            switch (CB_ScanPeriod_IsChecked)
            {
                case true:
                    TB_ScanPeriod.IsReadOnly = false;
                    CB_ScanRate.IsChecked = false;
                    TB_ScanRate.IsReadOnly = true;
                    break;
                case false:
                    TB_ScanPeriod.IsReadOnly = true;
                    CB_ScanRate.IsChecked = true;
                    TB_ScanRate.IsReadOnly = false;
                    break;
            }
        }

        private void B_ScanPeriodInc_Click(object sender, RoutedEventArgs e)
        {
            int TB_ScanPeriod_Value = 0;
            try
            {
                TB_ScanPeriod_Value = int.Parse(TB_ScanPeriod.Text);
            }
            catch (FormatException)
            {
            }
            TB_ScanPeriod_Value = Math.Min(TB_ScanPeriod_Value + 1, 100);
            TB_ScanPeriod.Text = String.Format("{0:d}", TB_ScanPeriod_Value);
        }

        private void B_ScanPeriodDec_Click(object sender, RoutedEventArgs e)
        {
            int TB_ScanPeriod_Value = 0;
            try
            {
                TB_ScanPeriod_Value = int.Parse(TB_ScanPeriod.Text);
            }
            catch (FormatException)
            {
            }
            TB_ScanPeriod_Value = Math.Max(TB_ScanPeriod_Value - 1, 1);
            TB_ScanPeriod.Text = String.Format("{0:d}", TB_ScanPeriod_Value - 1);
        }

        private void CB_ScanRate_Click(object sender, RoutedEventArgs e)
        {
            switch (CB_ScanRate_IsChecked)
            {
                case true:
                    TB_ScanRate.IsReadOnly = false;
                    CB_ScanPeriod.IsChecked = false;
                    TB_ScanPeriod.IsReadOnly = true;
                    break;
                case false:
                    TB_ScanRate.IsReadOnly = true;
                    CB_ScanPeriod.IsChecked = true;
                    TB_ScanPeriod.IsReadOnly = false;
                    break;
            }
        }

        private void B_ScanRateInc_Click(object sender, RoutedEventArgs e)
        {
            int TB_ScanRate_Value = 0;
            try
            {
                TB_ScanRate_Value = int.Parse(TB_ScanRate.Text);
            }
            catch (FormatException)
            {
            }
            TB_ScanRate_Value = Math.Min(TB_ScanRate_Value + 1, 1024);
            TB_ScanRate.Text = String.Format("{0:d}", TB_ScanRate_Value);
        }

        private void B_ScanRateDec_Click(object sender, RoutedEventArgs e)
        {
            int TB_ScanRate_Value = 0;
            try
            {
                TB_ScanRate_Value = int.Parse(TB_ScanRate.Text);
            }
            catch (FormatException)
            {
            }
            TB_ScanRate_Value = Math.Max(TB_ScanRate_Value - 1, 32);
            TB_ScanRate.Text = String.Format("{0:d}", TB_ScanRate_Value);
        }

        private void CB_DataLimit_Click(object sender, RoutedEventArgs e)
        {
            switch (CB_DataLimit.IsChecked)
            {
                case true:
                    TB_DataLimit.IsReadOnly = false;
                    break;
                case false:
                    TB_DataLimit.IsReadOnly = true;
                    break;
            }
        }

        private void CB_LockLimit_Click(object sender, RoutedEventArgs e)
        {
            switch (CB_LockLimit.IsChecked)
            {
                case true:
                    TB_LockLimit.IsReadOnly = false;
                    break;
                case false:
                    TB_LockLimit.IsReadOnly = true;
                    break;
            }
        }

        private void B_LockLimitInc_Click(object sender, RoutedEventArgs e)
        {
            int TB_LockLimit_Value = 0;
            try
            {
                TB_LockLimit_Value = int.Parse(TB_LockLimit.Text);
            }
            catch (FormatException)
            {
            }
            TB_LockLimit_Value = Math.Min(TB_LockLimit_Value + 1, 32);
            TB_LockLimit.Text = String.Format("{0:d}", TB_LockLimit_Value);
        }

        private void B_LockLimitDec_Click(object sender, RoutedEventArgs e)
        {
            int TB_LockLimit_Value = 0;
            try
            {
                TB_LockLimit_Value = int.Parse(TB_LockLimit.Text);
            }
            catch (FormatException)
            {
            }
            TB_LockLimit_Value = Math.Max(TB_LockLimit_Value - 1, 1);
            TB_LockLimit.Text = String.Format("{0:d}", TB_LockLimit_Value);
        }

        private void CB_ViewLimit_Click(object sender, RoutedEventArgs e)
        {
            switch (CB_ViewLimit.IsChecked)
            {
                case true:
                    TB_ViewLimit.IsReadOnly = false;
                    break;
                case false:
                    TB_ViewLimit.IsReadOnly = true;
                    break;
            }
        }

        private void B_ViewLimitInc_Click(object sender, RoutedEventArgs e)
        {
            int TB_ViewLimit_Value = 0;
            try
            {
                TB_ViewLimit_Value = int.Parse(TB_ViewLimit.Text);
            }
            catch (FormatException)
            {
            }
            TB_ViewLimit_Value = Math.Min(TB_ViewLimit_Value + 1, 256);
            TB_ViewLimit.Text = String.Format("{0:d}", TB_ViewLimit_Value);
        }

        private void B_ViewLimitDec_Click(object sender, RoutedEventArgs e)
        {
            int TB_ViewLimit_Value = 0;
            try
            {
                TB_ViewLimit_Value = int.Parse(TB_ViewLimit.Text);
            }
            catch (FormatException)
            {
            }
            TB_ViewLimit_Value = Math.Max(TB_ViewLimit_Value - 1, 1);
            TB_ViewLimit.Text = String.Format("{0:d}", TB_ViewLimit_Value);
        }

        private void B_DivideInc_Click(object sender, RoutedEventArgs e)
        {
            int TB_Divide_Value = 0;
            try
            {
                TB_Divide_Value = int.Parse(TB_Divide.Text);
            }
            catch (FormatException)
            {
            }
            TB_Divide_Value = Math.Min(TB_Divide_Value + 1, 40);
            TB_Divide.Text = String.Format("{0:d}", TB_Divide_Value);
        }

        private void B_DivideDec_Click(object sender, RoutedEventArgs e)
        {
            int TB_Divide_Value = 0;
            try
            {
                TB_Divide_Value = int.Parse(TB_Divide.Text);
            }
            catch (FormatException)
            {
            }
            TB_Divide_Value = Math.Min(TB_Divide_Value - 1, 20);
            TB_Divide.Text = String.Format("{0:d}", TB_Divide_Value);
        }

        private void B_SubDivideInc_Click(object sender, RoutedEventArgs e)
        {
            int TB_SubDivide_Value = 0;
            try
            {
                TB_SubDivide_Value = int.Parse(TB_SubDivide.Text);
            }
            catch (FormatException)
            {
            }
            TB_SubDivide_Value = Math.Min(TB_SubDivide_Value + 1, 10);
            TB_SubDivide.Text = String.Format("{0:d}", TB_SubDivide_Value);
        }

        private void B_SubDivideDec_Click(object sender, RoutedEventArgs e)
        {
            int TB_SubDivide_Value = 0;
            try
            {
                TB_SubDivide_Value = int.Parse(TB_SubDivide.Text);
            }
            catch (FormatException)
            {
            }
            TB_SubDivide_Value = Math.Min(TB_SubDivide_Value - 1, 2);
            TB_SubDivide.Text = String.Format("{0:d}", TB_SubDivide_Value);
        }

        private void B_DrawLimitInc_Click(object sender, RoutedEventArgs e)
        {
            int TB_DrawLimit_Value = 0;
            try
            {
                TB_DrawLimit_Value = int.Parse(TB_DrawLimit.Text);
            }
            catch (FormatException)
            {
            }
            TB_DrawLimit_Value = Math.Min(TB_DrawLimit_Value + 1, 8);
            TB_DrawLimit.Text = String.Format("{0:d}", TB_DrawLimit_Value);
        }

        private void B_DrawLimitDec_Click(object sender, RoutedEventArgs e)
        {
            int TB_DrawLimit_Value = 0;
            try
            {
                TB_DrawLimit_Value = int.Parse(TB_DrawLimit.Text);
            }
            catch (FormatException)
            {
            }
            TB_DrawLimit_Value = Math.Min(TB_DrawLimit_Value - 1, 1);
            TB_DrawLimit.Text = String.Format("{0:d}", TB_DrawLimit_Value);
        }

        private void B_ValueDivideInc_Click(object sender, RoutedEventArgs e)
        {
            int TB_ValueDivide_Value = 0;
            try
            {
                TB_ValueDivide_Value = int.Parse(TB_ValueDivide.Text);
            }
            catch (FormatException)
            {
            }
            TB_ValueDivide_Value = Math.Min(TB_ValueDivide_Value + 1, 100);
            TB_ValueDivide.Text = String.Format("{0:d}", TB_ValueDivide_Value);
        }

        private void B_ValueDivideDec_Click(object sender, RoutedEventArgs e)
        {
            int TB_ValueDivide_Value = 0;
            try
            {
                TB_ValueDivide_Value = int.Parse(TB_ValueDivide.Text);
            }
            catch (FormatException)
            {
            }
            TB_ValueDivide_Value = Math.Max(TB_ValueDivide_Value - 1, 2);
            TB_ValueDivide.Text = String.Format("{0:d}", TB_ValueDivide_Value);
        }

        private void B_ValueSubDivideInc_Click(object sender, RoutedEventArgs e)
        {
            int TB_ValueSubDivide_Value = 0;
            try
            {
                TB_ValueSubDivide_Value = int.Parse(TB_ValueSubDivide.Text);
            }
            catch (FormatException)
            {
            }
            TB_ValueSubDivide_Value = Math.Min(TB_ValueSubDivide_Value + 1, 10);
            TB_ValueSubDivide.Text = String.Format("{0:d}", TB_ValueSubDivide_Value + 1);
        }

        private void B_ValueSubDivideDec_Click(object sender, RoutedEventArgs e)
        {

            int TB_ValueSubDivide_Value = 0;
            try
            {
                TB_ValueSubDivide_Value = int.Parse(TB_ValueSubDivide.Text);
            }
            catch (FormatException)
            {
            }
            TB_ValueSubDivide_Value = Math.Max(TB_ValueSubDivide_Value - 1, 2);
            TB_ValueSubDivide.Text = String.Format("{0:d}", TB_ValueSubDivide_Value - 1);
        }
        #endregion
    }
}

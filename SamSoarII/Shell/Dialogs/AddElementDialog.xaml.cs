using System;
using SamSoarII.PLCDevice;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SamSoarII.Core.Helpers;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// AddElement.xaml 的交互逻辑
    /// </summary>
    public partial class AddElementDialog : Window, INotifyPropertyChanged, IDisposable
    {
        private bool isTypeChanged = false;
        public string AddrType { get; set; }
        public uint StartAddr { get; set; }
        public int DataType { get; set; }
        public string IntrasegmentType { get; set; } = string.Empty;
        public uint IntrasegmentAddr { get; set; } = 0;
        public int AddNums { get; set; }
        public int Flag { get; set; }
        private bool flaglegal;
        public event RoutedEventHandler EnsureButtonClick;
        public string[] DataTypes
        {
            get
            {
                switch (comboBox.SelectedIndex)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        return new string[] { "BOOL", "WORD", "DWORD" };
                    case 4:
                    case 5:
                        return new string[] { "BOOL" };
                    case 6:
                    case 7:
                    case 8:
                        return new string[] { "WORD", "UWORD", "DWORD", "UDWORD", "BCD", "FLOAT", "HEX", "DHEX", "BOOL" };
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        return new string[] { "WORD", "UWORD", "DWORD", "UDWORD", "BCD", "FLOAT", "HEX", "DHEX"};
                    default:
                        return null;
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public AddElementDialog()
        {
            InitializeComponent();
            KeyDown += AddElementDialog_KeyDown;
            EnsureButton.Click += EnsureButton_Click;
            CancelButton.Click += CancelButton_Click;
            DataContext = this;
        }
        private void AddElementDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnsureButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                if (e.Key == Key.Escape)
                {
                    CancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void EnsureButton_Click(object sender, RoutedEventArgs e)
        {
            if (!flaglegal)
            {
                LocalizedMessageBox.Show(String.Format("{0:s}{1:s}", TBL_Flag.Text, Properties.Resources.is_illegal), LocalizedMessageIcon.Warning);
                return;
            }
            ElementAddressType Type = (ElementAddressType)Enum.ToObject(typeof(ElementAddressType), comboBox.SelectedIndex);
            Device device = PLCDeviceManager.GetPLCDeviceManager().SelectDevice;
            if (Type == ElementAddressType.H || Type == ElementAddressType.K)
            {
                LocalizedMessageBox.Show(Properties.Resources.Constant_Monitor, LocalizedMessageIcon.Warning);
            }
            else if (ElementAddressHelper.AssertAddrRange(Type, uint.Parse(textBox.Text), device))
            {
                if ((bool)checkbox1.IsChecked && !ElementAddressHelper.AssertAddrRange(ElementAddressHelper.GetIntrasegmentAddrType(comboBox1.SelectedIndex), uint.Parse(textBox1.Text), device))
                {
                    LocalizedMessageBox.Show(Properties.Resources.Intra_Cross, LocalizedMessageIcon.Warning);
                }
                else if ((bool)checkbox.IsChecked && !ElementAddressHelper.AssertAddrRange(Type, uint.Parse(textBox.Text) + uint.Parse(rangeTextBox.GetTextBox().Text) - 1, device))
                {
                    LocalizedMessageBox.Show(Properties.Resources.Exceed_Adddress, LocalizedMessageIcon.Warning);
                }
                else
                {
                    if (EnsureButtonClick != null)
                    {
                        AddrType = Type.ToString();
                        StartAddr = uint.Parse(textBox.Text);
                        if ((bool)checkbox1.IsChecked)
                        {
                            IntrasegmentType = ElementAddressHelper.GetIntrasegmentAddrType(comboBox1.SelectedIndex).ToString();
                            IntrasegmentAddr = uint.Parse(textBox1.Text);
                        }
                        if (ElementAddressHelper.IsBitAddr(Type))
                        {
                            DataType = 0;
                        }
                        else
                        {
                            DataType = DataTypeCombox.SelectedIndex + 1;
                        }
                        if ((bool)checkbox.IsChecked)
                        {
                            AddNums = int.Parse(rangeTextBox.GetTextBox().Text);
                        }
                        else
                        {
                            AddNums = 1;
                        }
                        EnsureButtonClick.Invoke(this, new RoutedEventArgs());
                    }
                }
            }
            else
            {
                LocalizedMessageBox.Show(Properties.Resources.Address_Cross, LocalizedMessageIcon.Warning);
            }
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DataTypes"));
            if (textBox != null)
            {
                textBox.Text = string.Empty;
                isTypeChanged = true;
                textBox.Text = 0.ToString();
            }
            if (comboBox.SelectedIndex == 7 || comboBox.SelectedIndex == 8)
            {
                checkbox1.IsChecked = false;
                stackpanel1.Visibility = Visibility.Hidden;
            }
            else
            {
                if (stackpanel1 != null && stackpanel1.Visibility != Visibility.Visible)
                {
                    stackpanel1.Visibility = Visibility.Visible;
                }
            }
            UpdateFlagWidget();
        }
        private void OnDataTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFlagWidget();
        }
        private void UpdateFlagWidget()
        {
            TBO_Flag.Text = "";
            SP_Flag.Visibility = Visibility.Hidden;
            flaglegal = true;
            switch (comboBox.Text)
            {
                case "X":
                case "Y":
                case "S":
                case "M":
                    if (DataTypeCombox.Text.Equals("WORD") || DataTypeCombox.Text.Equals("DWORD"))
                    {
                        TBL_Flag.Text = Properties.Resources.Length;
                        SP_Flag.Visibility = Visibility.Visible;
                        flaglegal = false;
                    }
                    break;
                case "D":
                case "V":
                case "Z":
                    if (DataTypeCombox.Text.Equals("BOOL"))
                    {
                        TBL_Flag.Text = Properties.Resources.Data_Bit;
                        SP_Flag.Visibility = Visibility.Visible;
                        flaglegal = false;
                    }
                    break;
            }
        }
        public void Dispose()
        {
            Close();
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (isTypeChanged)
            {
                isTypeChanged = false;
                DataTypeCombox.SelectedIndex = 0;
            }
        }
        private void OnFlagTextChanged(object sender, TextChangedEventArgs e)
        {
            flaglegal = true;
            switch (comboBox.Text)
            {
                case "X": case "Y": case "S": case "M":
                    if (DataTypeCombox.Text.Equals("WORD") || DataTypeCombox.Text.Equals("DWORD"))
                    {
                        try { StartAddr = uint.Parse(textBox.Text); Flag = int.Parse(TBO_Flag.Text); }
                        catch (Exception) { flaglegal &= false; }
                        flaglegal &= Flag > 0;
                        flaglegal &= Flag <= (DataTypeCombox.Text.Equals("WORD") ? 16 : 32);
                        Device device = PLCDeviceManager.GetPLCDeviceManager().SelectDevice;
                        switch (comboBox.Text)
                        {
                            case "X": flaglegal &= StartAddr + Flag <= device.XRange.End; break;
                            case "Y": flaglegal &= StartAddr + Flag <= device.YRange.End; break;
                            case "S": flaglegal &= StartAddr + Flag <= device.SRange.End; break;
                            case "M": flaglegal &= StartAddr + Flag <= device.MRange.End; break;
                        }
                    }
                    break;
                case "D": case "V": case "Z":
                    if (DataTypeCombox.Text.Equals("BOOL"))
                    {
                        try { Flag = int.Parse(TBO_Flag.Text, System.Globalization.NumberStyles.HexNumber); }
                        catch (Exception) { flaglegal &= false; }
                        flaglegal &= TBO_Flag.Text.Length != 1;
                    }
                    break;
            }
            TBO_Flag.Background = TBO_Flag.Text.Length > 0 && !flaglegal
                ? Brushes.Red : Brushes.White;
        }
    }
}

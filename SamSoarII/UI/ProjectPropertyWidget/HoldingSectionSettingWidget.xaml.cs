using SamSoarII.UserInterface;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.AppMain.UI.ProjectPropertyWidget
{
    /// <summary>
    /// HoldingSectionSettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class HoldingSectionSettingWidget : UserControl,ISaveDialog, INotifyPropertyChanged
    {
        private HoldingSectionParams HoldingSectionParams;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private int oldValue, newValue;
        public HoldingSectionSettingWidget()
        {
            InitializeComponent();
            InitializeDataContext();
        }
        private void InitializeDataContext()
        {
            HoldingSectionParams = (HoldingSectionParams)ProjectPropertyManager.ParamsDic["HoldingSectParams"];
            DataContext = HoldingSectionParams;
            MStackPanel1.DataContext = this;
            MStackPanel2.DataContext = this;
            DStackPanel1.DataContext = this;
            DStackPanel2.DataContext = this;
            SStackPanel1.DataContext = this;
            SStackPanel2.DataContext = this;
            CVStackPanel1.DataContext = this;
            CVStackPanel2.DataContext = this;
        }
        #region Property
        public bool M1CanUp
        {
            get
            {
                return MTextbox1.Text == string.Empty || int.Parse(MTextbox1.Text) < 7999;
            }
        }
        public bool M2CanDown
        {
            get
            {
                return MTextbox1.Text == string.Empty || int.Parse(MTextbox1.Text) > 7504;
            }
        }
        public bool M3CanUp
        {
            get
            {
                return MTextbox2.Text == string.Empty || (int.Parse(MTextbox2.Text) < 496 && int.Parse(MTextbox2.Text) + int.Parse(MTextbox1.Text) < 8000);
            }
        }
        public bool M4CanDown
        {
            get
            {
                return MTextbox2.Text == string.Empty || int.Parse(MTextbox2.Text) > 1;
            }
        }
        public bool D1CanUp
        {
            get
            {
                return DTextbox1.Text == string.Empty || int.Parse(DTextbox1.Text) < 7999;
            }
        }
        public bool D2CanDown
        {
            get
            {
                return DTextbox1.Text == string.Empty || int.Parse(DTextbox1.Text) > 5968;
            }
        }
        public bool D3CanUp
        {
            get
            {
                return DTextbox2.Text == string.Empty || (int.Parse(DTextbox2.Text) < 2032 && int.Parse(DTextbox2.Text) + int.Parse(DTextbox1.Text) < 8000);
            }
        }
        public bool D4CanDown
        {
            get
            {
                return DTextbox2.Text == string.Empty || int.Parse(DTextbox2.Text) > 1;
            }
        }
        public bool S1CanUp
        {
            get
            {
                return STextbox1.Text == string.Empty || int.Parse(STextbox1.Text) < 999;
            }
        }
        public bool S2CanDown
        {
            get
            {
                return STextbox1.Text == string.Empty || int.Parse(STextbox1.Text) > 600;
            }
        }
        public bool S3CanUp
        {
            get
            {
                return STextbox2.Text == string.Empty || (int.Parse(STextbox2.Text) < 400 && int.Parse(STextbox2.Text) + int.Parse(STextbox1.Text) < 1000);
            }
        }
        public bool S4CanDown
        {
            get
            {
                return STextbox2.Text == string.Empty || int.Parse(STextbox2.Text) > 1;
            }
        }
        public bool CV1CanUp
        {
            get
            {
                return CVTextbox1.Text == string.Empty || int.Parse(CVTextbox1.Text) < 199;
            }
        }
        public bool CV2CanDown
        {
            get
            {
                return CVTextbox1.Text == string.Empty || int.Parse(CVTextbox1.Text) > 100;
            }
        }
        public bool CV3CanUp
        {
            get
            {
                return CVTextbox2.Text == string.Empty || (int.Parse(CVTextbox2.Text) < 100 && int.Parse(CVTextbox2.Text) + int.Parse(CVTextbox1.Text) < 200);
            }
        }
        public bool CV4CanDown
        {
            get
            {
                return CVTextbox2.Text == string.Empty || int.Parse(CVTextbox2.Text) > 1;
            }
        }
        #endregion
        public void Save()
        {
            HoldingSectionParams.MStartAddr = int.Parse(MTextbox1.Text);
            HoldingSectionParams.MLength = int.Parse(MTextbox2.Text);
            HoldingSectionParams.DStartAddr = int.Parse(DTextbox1.Text);
            HoldingSectionParams.DLength = int.Parse(DTextbox2.Text);
            HoldingSectionParams.SStartAddr = int.Parse(STextbox1.Text);
            HoldingSectionParams.SLength = int.Parse(STextbox2.Text);
            HoldingSectionParams.CVStartAddr = int.Parse(CVTextbox1.Text);
            HoldingSectionParams.CVLength = int.Parse(CVTextbox2.Text);
            HoldingSectionParams.NotClear = (bool)CheckBox1.IsChecked;
        }
        private TextBox GetRefTextBox(TextBox textbox)
        {
            if (textbox == MTextbox1)
            {
                return MTextbox2;
            }
            else if (textbox == MTextbox2)
            {
                return MTextbox1;
            }
            else if (textbox == DTextbox1)
            {
                return DTextbox2;
            }
            else if (textbox == DTextbox2)
            {
                return DTextbox1;
            }
            else if (textbox == STextbox1)
            {
                return STextbox2;
            }
            else if (textbox == STextbox2)
            {
                return STextbox1;
            }
            else if (textbox == CVTextbox1)
            {
                return CVTextbox2;
            }
            else
            {
                return CVTextbox1;
            }
        }
        private bool AssertOneSideRange(TextBox textbox, int value)
        {
            if (textbox == MTextbox1)
            {
                return value <= 7999;
            }
            else if (textbox == MTextbox2)
            {
                return value <= 496;
            }
            else if (textbox == DTextbox1)
            {
                return value <= 7999;
            }
            else if (textbox == DTextbox2)
            {
                return value <= 2032;
            }
            else if (textbox == STextbox1)
            {
                return value <= 999;
            }
            else if (textbox == STextbox2)
            {
                return value <= 400;
            }
            else if (textbox == CVTextbox1)
            {
                return value <= 199;
            }
            else
            {
                return value <= 100;
            }
        }
        private bool AssertRange(TextBox textbox,int value)
        {
            if (textbox == MTextbox1)
            {
                return value <= 7999 && value >= 7504;
            }
            else if (textbox == MTextbox2)
            {
                return value <= 496 && value >= 1;
            }
            else if (textbox == DTextbox1)
            {
                return value <= 7999 && value >= 5968;
            }
            else if (textbox == DTextbox2)
            {
                return value <= 2032 && value >= 1;
            }
            else if (textbox == STextbox1)
            {
                return value <= 999 && value >= 600;
            }
            else if (textbox == STextbox2)
            {
                return value <= 400 && value >= 1;
            }
            else if (textbox == CVTextbox1)
            {
                return value <= 199 && value >= 100;
            }
            else
            {
                return value <= 100 && value >= 1;
            }
        }
        private bool AssertWholeRange(TextBox textbox,int value1,int value2)
        {
            if (textbox == MTextbox1 || textbox == MTextbox2)
            {
                return value1 + value2 <= 8000;
            }
            else if (textbox == DTextbox1 || textbox == DTextbox2)
            {
                return value1 + value2 <= 8000;
            }
            else if (textbox == STextbox1 || textbox == STextbox2)
            {
                return value1 + value2 <= 1000;
            }
            else
            {
                return value1 + value2 <= 200;
            }
        }
        private int GetWholeRange(TextBox textbox)
        {
            if (textbox == MTextbox1 || textbox == MTextbox2)
            {
                return 8000;
            }
            else if (textbox == DTextbox1 || textbox == DTextbox2)
            {
                return 8000;
            }
            else if (textbox == STextbox1 || textbox == STextbox2)
            {
                return 1000;
            }
            else
            {
                return 200;
            }
        }
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            oldValue = int.Parse((sender as TextBox).Text);
        }
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text != string.Empty)
            {
                newValue = int.Parse(textbox.Text);
            }
            else
            {
                newValue = 0;
            }
            if (!AssertRange(textbox,newValue))
            {
                textbox.Text = oldValue.ToString();
            }
            else
            {
                textbox.Text = newValue.ToString();
                if (!AssertWholeRange(textbox,newValue,int.Parse(GetRefTextBox(textbox).Text)))
                {
                    GetRefTextBox(textbox).Text = string.Format("{0}", GetWholeRange(textbox) - newValue);
                }
            }
        }
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            int oldvalue = textbox.Text == string.Empty ? 0 : int.Parse(textbox.Text);
            if (oldvalue == 0 && (e.Key == Key.D0 || e.Key == Key.NumPad0))
            {
                e.Handled = true;
            }
            if (KeyInputHelper.CanInputAssert(e.Key))
            {
                if (KeyInputHelper.NumAssert(e.Key))
                {
                    int newvalue = 10 * oldvalue + KeyInputHelper.GetKeyValue(e.Key);
                    if (!AssertOneSideRange(textbox,newvalue))
                    {
                        e.Handled = true;
                    }
                    else if(textbox == MTextbox2 || textbox == DTextbox2 || textbox == STextbox2 || textbox == CVTextbox2)
                    {
                        if (!AssertWholeRange(textbox,newvalue,int.Parse(GetRefTextBox(textbox).Text)))
                        {
                            e.Handled = true;
                        }
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }
        private int GetTextBoxDefaultValue(RepeatButton button)
        {
            if (button == MButton1 || button == MButton2)
            {
                return 7504;
            }else if (button == MButton3 || button == MButton4)
            {
                return 496;
            }else if (button == DButton1 || button == DButton2)
            {
                return 5968;
            }else if (button == DButton3 || button == DButton4)
            {
                return 2032;
            }else if (button == SButton1 || button == SButton2)
            {
                return 600;
            }else if (button == SButton3 || button == SButton4)
            {
                return 400;
            }else if (button == CVButton1 || button == CVButton2)
            {
                return 100;
            }else
            {
                return 100;
            }
        }
        private TextBox GetRefTextBox(RepeatButton button)
        {
            if (button == MButton1 || button == MButton2)
            {
                return MTextbox1;
            }
            else if (button == MButton3 || button == MButton4)
            {
                return MTextbox2;
            }
            else if (button == DButton1 || button == DButton2)
            {
                return DTextbox1;
            }
            else if (button == DButton3 || button == DButton4)
            {
                return DTextbox2;
            }
            else if (button == SButton1 || button == SButton2)
            {
                return STextbox1;
            }
            else if (button == SButton3 || button == SButton4)
            {
                return STextbox2;
            }
            else if (button == CVButton1 || button == CVButton2)
            {
                return CVTextbox1;
            }
            else
            {
                return CVTextbox2;
            }
        }
        private void OnUpClick(object sender, RoutedEventArgs e)
        {
            TextBox textbox = GetRefTextBox(sender as RepeatButton);
            if (textbox.Text != string.Empty)
            {
                ValueUpExecute(textbox);
            }
            else
            {
                EmptyValueHandler(sender as RepeatButton, textbox);
            }
        }
        private void OnDownClick(object sender, RoutedEventArgs e)
        {
            TextBox textbox = GetRefTextBox(sender as RepeatButton);
            if (textbox.Text != string.Empty)
            {
                ValueDownExecute(textbox);
            }
            else
            {
                EmptyValueHandler(sender as RepeatButton,textbox);
            }
        }
        private void ValueUpExecute(TextBox textbox)
        {
            TextBox textbox1 = GetRefTextBox(textbox);
            int value1 = int.Parse(textbox.Text);
            int value2 = int.Parse(textbox1.Text);
            textbox.Text = (value1 + 1).ToString();
            if (value1 + value2 + 1 > GetWholeRange(textbox))
            {
                textbox1.Text = (value2 - 1).ToString();
            }
        }
        private void ValueDownExecute(TextBox textbox)
        {
            textbox.Text = (int.Parse(textbox.Text) - 1).ToString();
        }
        private void EmptyValueHandler(RepeatButton button,TextBox textbox)
        {
            int defaultvalue = GetTextBoxDefaultValue(button);
            int anothervalue = int.Parse(GetRefTextBox(textbox).Text);
            int wholerange = GetWholeRange(textbox);
            if (defaultvalue + anothervalue > wholerange)
            {
                textbox.Text = string.Format("{0}", wholerange - anothervalue);
            }
            else
            {
                textbox.Text = defaultvalue.ToString();
            }
        }

        private void DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            using (DefaultValueDialog dialog = new DefaultValueDialog())
            {
                dialog.EnsureButtonClick += (sender1, e1) =>
                {
                    MTextbox1.Text = 7504.ToString();
                    MTextbox2.Text = 496.ToString();
                    DTextbox1.Text = 5968.ToString();
                    DTextbox2.Text = 2032.ToString();
                    STextbox1.Text = 600.ToString();
                    STextbox2.Text = 400.ToString();
                    CVTextbox1.Text = 100.ToString();
                    CVTextbox2.Text = 100.ToString();
                    CheckBox1.IsChecked = false;
                    dialog.Close();
                };
                dialog.ShowDialog();
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == MTextbox1 || sender == MTextbox2)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("M1CanUp"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("M2CanDown"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("M3CanUp"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("M4CanDown"));
            }else if (sender == DTextbox1 || sender == DTextbox2)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("D1CanUp"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("D2CanDown"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("D3CanUp"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("D4CanDown"));
            }else if (sender == STextbox1 || sender == STextbox2)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("S1CanUp"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("S2CanDown"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("S3CanUp"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("S4CanDown"));
            }else
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CV1CanUp"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CV2CanDown"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CV3CanUp"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CV4CanDown"));
            }
        }
    }
}

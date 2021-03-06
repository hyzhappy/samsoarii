﻿using SamSoarII.Utility;
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

namespace SamSoarII.AppMain.UI.ProjectPropertyWidget
{
    /// <summary>
    /// AnalogQuantitySettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class AnalogQuantitySettingWidget : UserControl,ISaveDialog
    {
        private AnalogQuantityParams AnalogQuantityParams;
        private int oldValue,newValue;
        public AnalogQuantitySettingWidget()
        {
            InitializeComponent();
            AnalogQuantityParams = (AnalogQuantityParams)ProjectPropertyManager.ProjectPropertyDic["AnalogQuantityParams"];
            DataContext = AnalogQuantityParams;
        }
        public void Save()
        {
            AnalogQuantityParams.IsUsed = (bool)checkbox.IsChecked;
            AnalogQuantityParams.InputPassIndex = InputCombox1.SelectedIndex;
            AnalogQuantityParams.InputModeIndex = InputCombox2.SelectedIndex;
            AnalogQuantityParams.SamplingtimesIndex = InputCombox3.SelectedIndex;
            AnalogQuantityParams.SamplingValue = int.Parse(InputTextbox1.Text);
            AnalogQuantityParams.InputStartRange = int.Parse(InputTextbox2.Text);
            AnalogQuantityParams.InputEndRange = int.Parse(InputTextbox3.Text);
            AnalogQuantityParams.OutputPassIndex = OutputComboBox1.SelectedIndex;
            AnalogQuantityParams.OutputModeIndex = OutputComboBox2.SelectedIndex;
            AnalogQuantityParams.OutputStartRange = int.Parse(OutputTextBox1.Text);
            AnalogQuantityParams.OutputEndRange = int.Parse(OutputTextBox2.Text);
        }
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            int oldvalue;
            if ((sender as TextBox).Text != string.Empty)
            {
                oldvalue = int.Parse((sender as TextBox).Text);
            }
            else
            {
                oldvalue = 0;
            }
            if (KeyInputHelper.CanInputAssert(e.Key))
            {
                if (KeyInputHelper.NumAssert(e.Key))
                {
                    int newvalue = 10 * oldvalue + KeyInputHelper.GetKeyValue(e.Key);
                    if (!AssertWholeRange((sender as TextBox), newvalue))
                    {
                        e.Handled = true;
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }
        private bool AssertWholeRange(TextBox textbox,int value)
        {
            int anothervalue;
            if (textbox == InputTextbox1)
            {
                return value <= 8191 && value >= 0;
            }else if (textbox == InputTextbox2)
            {
                anothervalue = int.Parse(InputTextbox3.Text);
                return value >= 0 && value < anothervalue;
            }
            else if (textbox == InputTextbox3)
            {
                return value <= 65535;
            }
            else if (textbox == OutputTextBox1)
            {
                anothervalue = int.Parse(OutputTextBox2.Text);
                return value >= 0 && value < anothervalue;
            }
            else
            {
                return value <= 65535;
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            oldValue = int.Parse((sender as TextBox).Text);
        }
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == string.Empty)
            {
                textbox.Text = oldValue.ToString();
            }
            else
            {
                int anothervalue = -1;
                newValue = int.Parse(textbox.Text);
                if (textbox == InputTextbox3)
                {
                    anothervalue = int.Parse(InputTextbox2.Text);
                }
                else if(textbox == OutputTextBox2)
                {
                    anothervalue = int.Parse(OutputTextBox1.Text);
                }
                if (newValue <= anothervalue)
                {
                    textbox.Text = GetDefaultValue(textbox).ToString();
                }
            }
        }
        private int GetDefaultValue(TextBox textbox)
        {
            if (textbox == InputTextbox1)
            {
                return 1000;
            }
            else if (textbox == InputTextbox2)
            {
                return 0;
            }
            else if (textbox == InputTextbox3)
            {
                return 65535;
            }
            else if (textbox == OutputTextBox1)
            {
                return 0;
            }
            else
            {
                return 65535;
            }
        }
    }
}

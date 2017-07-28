using SamSoarII.Core.Models;
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

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// OutRecPropModel.xaml 的交互逻辑
    /// </summary>
    public partial class OutRecPropModel : BasePropModel
    {
        public OutRecPropModel(LadderUnitModel _core) : base(_core)
        {
            InitializeComponent();
            TopTextBlock.Text = Core.InstName;
            Count = base.Count;
            for (int i = 0; i < Core.Children.Count; i++)
            {
                SetPropertyLabel(i, Core.Children[i].Format.Name);
                SetPropertyText(i, GetValueString(i));
            }
        }
        
        public override int SelectedIndex
        {
            get
            {
                return base.SelectedIndex;
            }
            set
            {
                if (value >= 0 && value < Count && value != SelectedIndex)
                {
                    base.SelectedIndex = value;
                    switch (value)
                    {
                        case 0:
                            if (!MiddleTextBox1.IsFocused)
                            {
                                MiddleTextBox1.Focus();
                                Keyboard.Focus(MiddleTextBox1);
                            }
                            MiddleTextBox1.SelectAll();
                            break;
                        case 1:
                            if (!MiddleTextBox2.IsFocused)
                            {
                                MiddleTextBox2.Focus();
                                Keyboard.Focus(MiddleTextBox2);
                            }
                            MiddleTextBox2.SelectAll();
                            break;
                        case 2:
                            if (!MiddleTextBox3.IsFocused)
                            {
                                MiddleTextBox3.Focus();
                                Keyboard.Focus(MiddleTextBox3);
                            }
                            MiddleTextBox3.SelectAll();
                            break;
                        case 3:
                            if (!MiddleTextBox4.IsFocused)
                            {
                                MiddleTextBox4.Focus();
                                Keyboard.Focus(MiddleTextBox4);
                            }
                            MiddleTextBox4.SelectAll();
                            break;
                        case 4:
                            if (!MiddleTextBox5.IsFocused)
                            {
                                MiddleTextBox5.Focus();
                                Keyboard.Focus(MiddleTextBox5);
                            }
                            MiddleTextBox5.SelectAll();
                            break;
                    }
                }
            }
        }
        
        public event CollectionPopupEventHandler CollectionPopup = delegate { };
        
        private void MiddleTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueString(0, MiddleTextBox1.Text);
            switch (InstructionName)
            {
                case "CALL":
                    CollectionPopup(this, new CollectionPopupEventArgs(CollectionPopupType.SUBROUTINES, MiddleTextBox1));
                    break;
                case "CALLM":
                    CollectionPopup(this, new CollectionPopupEventArgs(CollectionPopupType.FUNCBLOCKS, MiddleTextBox1));
                    break;
            }
        }
        private void MiddleTextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueString(1, MiddleTextBox2.Text);
            switch (InstructionName)
            {
                case "ATCH":
                    CollectionPopup(this, new CollectionPopupEventArgs(CollectionPopupType.SUBROUTINES, MiddleTextBox2));
                    break;
                case "MBUS":
                    CollectionPopup(this, new CollectionPopupEventArgs(CollectionPopupType.MODBUSES, MiddleTextBox2));
                    break;
            }
        }
        private void MiddleTextBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueString(2, MiddleTextBox3.Text);
        }
        private void MiddleTextBox4_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueString(3, MiddleTextBox4.Text);
        }
        private void MiddleTextBox5_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueString(4, MiddleTextBox5.Text);
        }

        private void MiddleTextBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedIndex = 0;
        }
        private void MiddleTextBox2_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedIndex = 1;
        }
        private void MiddleTextBox3_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedIndex = 2;
        }
        private void MiddleTextBox4_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedIndex = 3;
        }
        private void MiddleTextBox5_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedIndex = 4;
        }

        #region CALLM

        private int count;
        public override int Count
        {
            get
            {
                return Core.Type == LadderUnitModel.Types.CALLM ? this.count : base.Count;
            }
            set
            {
                this.count = value;
                MiddleTextBlock1.Visibility = MiddleTextBox1.Visibility = count > 0 ? Visibility.Visible : Visibility.Hidden;
                MiddleTextBlock2.Visibility = MiddleTextBox2.Visibility = count > 1 ? Visibility.Visible : Visibility.Hidden;
                MiddleTextBlock3.Visibility = MiddleTextBox3.Visibility = count > 2 ? Visibility.Visible : Visibility.Hidden;
                MiddleTextBlock4.Visibility = MiddleTextBox4.Visibility = count > 3 ? Visibility.Visible : Visibility.Hidden;
                MiddleTextBlock5.Visibility = MiddleTextBox5.Visibility = count > 4 ? Visibility.Visible : Visibility.Hidden;
            }
        }
        
        public void SetPropertyLabel(int id, string text)
        {
            switch (id)
            {
                case 0: MiddleTextBlock1.Text = text; break;
                case 1: MiddleTextBlock2.Text = text; break;
                case 2: MiddleTextBlock3.Text = text; break;
                case 3: MiddleTextBlock4.Text = text; break;
                case 4: MiddleTextBlock5.Text = text; break;
            }
        }

        public void SetPropertyText(int id, string text)
        {
            switch (id)
            {
                case 0: MiddleTextBox1.Text = text; break;
                case 1: MiddleTextBox2.Text = text; break;
                case 2: MiddleTextBox3.Text = text; break;
                case 3: MiddleTextBox4.Text = text; break;
                case 4: MiddleTextBox5.Text = text; break;
            }
        }

        #endregion

    }
}

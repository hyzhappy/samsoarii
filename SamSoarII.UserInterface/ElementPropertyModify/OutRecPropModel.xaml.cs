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

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// OutRecPropModel.xaml 的交互逻辑
    /// </summary>
    public partial class OutRecPropModel : BasePropModel
    {
        public OutRecPropModel()
        {
            InitializeComponent();
        }
        
        private string instname;
        public override string InstructionName
        {
            get
            {
                return this.instname;
            }
            set
            {
                this.instname = value;
                TopTextBlock.Text = instname;
            }
        }

        private int count;
        public override int Count
        {
            get
            {
                return this.count;
            }
            set
            {
                this.count = value;
                MiddleTextBox1.Visibility = count >= 1
                    ? Visibility.Visible
                    : Visibility.Hidden;
                MiddleTextBox2.Visibility = count >= 2
                    ? Visibility.Visible
                    : Visibility.Hidden;
                MiddleTextBox3.Visibility = count >= 3
                    ? Visibility.Visible
                    : Visibility.Hidden;
                MiddleTextBox4.Visibility = count >= 4
                    ? Visibility.Visible
                    : Visibility.Hidden;
                MiddleTextBox5.Visibility = count >= 5
                    ? Visibility.Visible
                    : Visibility.Hidden;
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
                base.SelectedIndex = value;
                switch (value)
                {
                    case 0:
                        MiddleTextBox1.Focus();
                        Keyboard.Focus(MiddleTextBox1);
                        break;
                    case 1:
                        MiddleTextBox2.Focus();
                        Keyboard.Focus(MiddleTextBox2);
                        break;
                    case 2:
                        MiddleTextBox3.Focus();
                        Keyboard.Focus(MiddleTextBox3);
                        break;
                    case 3:
                        MiddleTextBox4.Focus();
                        Keyboard.Focus(MiddleTextBox4);
                        break;
                    case 4:
                        MiddleTextBox5.Focus();
                        Keyboard.Focus(MiddleTextBox5);
                        break;
                }
            }
        }

        public override string ValueString1
        {
            get
            {
                if (MiddleTextBox1 == null)
                    return String.Empty;
                return MiddleTextBox1.Text;
            }
            set
            {
                if (MiddleTextBox1 == null)
                    return;
                MiddleTextBox1.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString2
        {
            get
            {
                if (MiddleTextBox2 == null)
                    return String.Empty;
                return MiddleTextBox2.Text;
            }
            set
            {
                if (MiddleTextBox2 == null)
                    return;
                MiddleTextBox2.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString3
        {
            get
            {
                if (MiddleTextBox3 == null)
                    return String.Empty;
                return MiddleTextBox3.Text;
            }
            set
            {
                if (MiddleTextBox3 == null)
                    return;
                MiddleTextBox3.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString4
        {
            get
            {
                if (MiddleTextBox4 == null)
                    return String.Empty;
                return MiddleTextBox4.Text;
            }
            set
            {
                if (MiddleTextBox4 == null)
                    return;
                MiddleTextBox4.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString5
        {
            get
            {
                if (MiddleTextBox5 == null)
                    return String.Empty;
                return MiddleTextBox5.Text;
            }
            set
            {
                if (MiddleTextBox5 == null)
                    return;
                MiddleTextBox5.Text = value;
                UpdateComment(value);
            }
        }

        public event CollectionPopupEventHandler CollectionPopup = delegate { };

        public void UpdateCALLM()
        {
            if (Dialog == null
             || !InstructionName.Equals("CALLM"))
            {
                return;
            }
            IEnumerable<string[]> fit = Dialog.Functions.Where(
                (string[] _msg) =>
                {
                    return ValueString1.Equals(_msg[1]);
                });
            if (fit.Count() > 0)
            {
                string[] msg = fit.First();
                Count = msg.Length / 2;
            }
        }

        private void MiddleTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateComment(MiddleTextBox1.Text);
            switch (InstructionName)
            {
                case "CALL":
                    CollectionPopup(this, new CollectionPopupEventArgs(CollectionPopupType.SUBROUTINES, MiddleTextBox1));
                    break;
                case "CALLM":
                    UpdateCALLM();
                    CollectionPopup(this, new CollectionPopupEventArgs(CollectionPopupType.FUNCBLOCKS, MiddleTextBox1));
                    break;
            }
        }
        private void MiddleTextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateComment(MiddleTextBox2.Text);
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
            UpdateComment(MiddleTextBox3.Text);
        }
        private void MiddleTextBox4_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateComment(MiddleTextBox4.Text);
        }
        private void MiddleTextBox5_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateComment(MiddleTextBox5.Text);
        }

        private void MiddleTextBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectedIndex = 0;
        }
        private void MiddleTextBox2_GotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectedIndex = 1;
        }
        private void MiddleTextBox3_GotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectedIndex = 2;
        }
        private void MiddleTextBox4_GotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectedIndex = 3;
        }
        private void MiddleTextBox5_GotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectedIndex = 4;
        }

    }
}

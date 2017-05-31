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

        public override string ValueString1
        {
            get
            {
                return MiddleTextBox1.Text;
            }
            set
            {
                MiddleTextBox1.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString2
        {
            get
            {
                return MiddleTextBox2.Text;
            }
            set
            {
                MiddleTextBox2.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString3
        {
            get
            {
                return MiddleTextBox3.Text;
            }
            set
            {
                MiddleTextBox3.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString4
        {
            get
            {
                return MiddleTextBox4.Text;
            }
            set
            {
                MiddleTextBox4.Text = value;
                UpdateComment(value);
            }
        }
        public override string ValueString5
        {
            get
            {
                return MiddleTextBox5.Text;
            }
            set
            {
                MiddleTextBox5.Text = value;
                UpdateComment(value);
            }
        }

        private void MiddleTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateComment(MiddleTextBox1.Text);
        }
        private void MiddleTextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateComment(MiddleTextBox2.Text);
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
    }
}

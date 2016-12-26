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
using System.Windows.Shapes;

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// ElementPropertyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ElementPropertyDialog : Window
    {
        public event RoutedEventHandler EnsureButtonClick;

        public string ValueString1
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public string ValueString2
        {
            get { return textBox2.Text; }
            set { textBox2.Text = value; }
        }

        public string ValueString3
        {
            get { return textBox3.Text; }
            set { textBox3.Text = value; }
        }

        public string ValueString4
        {
            get { return textBox4.Text; }
            set { textBox4.Text = value; }
        }

        public string ValueString5
        {
            get { return textBox5.Text; }
            set { textBox5.Text = value; }
        }

        public string ValueString6
        {
            get { return textBox6.Text; }
            set { textBox6.Text = value; }
        }

        public string ValueString7
        {
            get { return textBox7.Text; }
            set { textBox7.Text = value; }
        }

        public ElementPropertyDialog()
        {
            InitializeComponent();
            KeyDown += ElementPropertyDialog_KeyDown;
            EnsureButton.Click += EnsureButton_Click;
        }

        public void ShowLine1(string labelContent)
        {
            label1.Visibility = Visibility.Visible;
            textBox1.Visibility = Visibility.Visible;
            label1.Content = labelContent;
        }

        public void ShowLine2(string labelContent)
        {
            label2.Visibility = Visibility.Visible;
            textBox2.Visibility = Visibility.Visible;
            label2.Content = labelContent;
        }

        public void ShowLine3(string labelContent)
        {
            label3.Visibility = Visibility.Visible;
            textBox3.Visibility = Visibility.Visible;
            label3.Content = labelContent;
        }

        public void ShowLine4(string labelContent)
        {
            label4.Visibility = Visibility.Visible;
            textBox4.Visibility = Visibility.Visible;
            label4.Content = labelContent;
        }

        public void ShowLine5(string labelContent)
        {
            label5.Visibility = Visibility.Visible;
            textBox5.Visibility = Visibility.Visible;
            label5.Content = labelContent;
        }

        public void ShowLine6(string labelContent)
        {
            label6.Visibility = Visibility.Visible;
            textBox6.Visibility = Visibility.Visible;
            label6.Content = labelContent;
        }

        public void ShowLine7(string labelContent)
        {
            label7.Visibility = Visibility.Visible;
            textBox7.Visibility = Visibility.Visible;
            label7.Content = labelContent;
        }

        private void EnsureButton_Click(object sender, RoutedEventArgs e)
        {
            if(EnsureButtonClick != null)
            {
                EnsureButtonClick.Invoke(this, new RoutedEventArgs());
            }
        }

        private void ElementPropertyDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                EnsureButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if(e.Key == Key.Escape)
            {
                Close();
            }
        }

        
        
    }
}

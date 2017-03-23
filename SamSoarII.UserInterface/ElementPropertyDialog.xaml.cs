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
using SamSoarII.ValueModel;

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// ElementPropertyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ElementPropertyDialog : Window, IPropertyDialog
    {
        public event RoutedEventHandler Commit;

        public string ValueString1
        {
            get { return ValueTextBox1.Text; }
            set { ValueTextBox1.Text = value; }
        }

        public string ValueString2
        {
            get { return ValueTextBox2.Text; }
            set { ValueTextBox2.Text = value; }
        }

        public string ValueString3
        {
            get { return ValueTextBox3.Text; }
            set { ValueTextBox3.Text = value; }
        }

        public string ValueString4
        {
            get { return ValueTextBox4.Text; }
            set { ValueTextBox4.Text = value; }
        }

        public string ValueString5
        {
            get { return ValueTextBox5.Text; }
            set { ValueTextBox5.Text = value; }
        }

        public string ValueString6
        {
            get { return ValueTextBox6.Text; }
            set { ValueTextBox6.Text = value; }
        }

        public string ValueString7
        {
            get { return ValueTextBox7.Text; }
            set { ValueTextBox7.Text = value; }
        }

        public string CommentString1
        {
            get { return CommentTextBox1.Text; }
            set { CommentTextBox1.Text = value; }
        }
        public string CommentString2
        {
            get { return CommentTextBox2.Text; }
            set { CommentTextBox2.Text = value; }
        }
        public string CommentString3
        {
            get { return CommentTextBox3.Text; }
            set { CommentTextBox3.Text = value; }
        }
        public string CommentString4
        {
            get { return CommentTextBox4.Text; }
            set { CommentTextBox4.Text = value; }
        }
        public string CommentString5
        {
            get { return CommentTextBox5.Text; }
            set { CommentTextBox5.Text = value; }
        }
        public string CommentString6
        {
            get { return CommentTextBox6.Text; }
            set { CommentTextBox6.Text = value; }
        }
        public string CommentString7
        {
            get { return CommentTextBox7.Text; }
            set { CommentTextBox7.Text = value; }
        }

        private int _valueCount;

        public ElementPropertyDialog(int valuecount)
        {
            InitializeComponent();
            _valueCount = valuecount;
        }

        public void ShowLine1(string labelContent)
        {
            label1.Visibility = Visibility.Visible;
            ValueTextBox1.Visibility = Visibility.Visible;
            label1.Content = labelContent;
            label1_c.Visibility = Visibility.Visible;
            CommentTextBox1.Visibility = Visibility.Visible;
            ValueTextBox1.Focus();
        }

        public void ShowLine1(string labelContent, IValueModel value)
        {
            ShowLine1(labelContent);
            ValueString1 = value.ValueString;
        }

        public void ShowLine2(string labelContent)
        {
            label2.Visibility = Visibility.Visible;
            ValueTextBox2.Visibility = Visibility.Visible;
            label2.Content = labelContent;
            label2_c.Visibility = Visibility.Visible;
            CommentTextBox2.Visibility = Visibility.Visible;
            ValueTextBox2.Focus();
        }
        public void ShowLine2(string labelContent, IValueModel value)
        {
            ShowLine2(labelContent);
            ValueString2 = value.ValueString;
        }
        public void ShowLine3(string labelContent)
        {
            label3.Visibility = Visibility.Visible;
            ValueTextBox3.Visibility = Visibility.Visible;
            label3.Content = labelContent;
            label3_c.Visibility = Visibility.Visible;
            CommentTextBox3.Visibility = Visibility.Visible;
            ValueTextBox3.Focus();
        }

        public void ShowLine3(string labelContent, IValueModel value)
        {
            ShowLine3(labelContent);
            ValueString3 = value.ValueString;
        }

        public void ShowLine4(string labelContent)
        {
            label4.Visibility = Visibility.Visible;
            ValueTextBox4.Visibility = Visibility.Visible;
            label4.Content = labelContent;
            label4_c.Visibility = Visibility.Visible;
            CommentTextBox4.Visibility = Visibility.Visible;
            ValueTextBox4.Focus();
        }
        public void ShowLine4(string labelContent, IValueModel value)
        {
            ShowLine4(labelContent);
            ValueString4 = value.ValueString;
        }
        public void ShowLine5(string labelContent)
        {
            label5.Visibility = Visibility.Visible;
            ValueTextBox5.Visibility = Visibility.Visible;
            label5.Content = labelContent;
            label5_c.Visibility = Visibility.Visible;
            CommentTextBox5.Visibility = Visibility.Visible;
            ValueTextBox5.Focus();
        }
        public void ShowLine5(string labelContent, IValueModel value)
        {
            ShowLine5(labelContent);
            ValueString5 = value.ValueString;
        }
        public void ShowLine6(string labelContent)
        {
            label6.Visibility = Visibility.Visible;
            ValueTextBox6.Visibility = Visibility.Visible;
            label6.Content = labelContent;
            label6_c.Visibility = Visibility.Visible;
            CommentTextBox6.Visibility = Visibility.Visible;
            ValueTextBox6.Focus();
        }
        public void ShowLine6(string labelContent, IValueModel value)
        {
            ShowLine6(labelContent);
            ValueString6 = value.ValueString;
        }
        public void ShowLine7(string labelContent)
        {
            label7.Visibility = Visibility.Visible;
            ValueTextBox7.Visibility = Visibility.Visible;
            label7.Content = labelContent;
            label7_c.Visibility = Visibility.Visible;
            CommentTextBox7.Visibility = Visibility.Visible;
            ValueTextBox7.Focus();
        }
        public void ShowLine7(string labelContent, IValueModel value)
        {
            ShowLine7(labelContent);
            ValueString7 = value.ValueString;
        }
        #region Event handler
        private void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            Commit?.Invoke(this, new RoutedEventArgs());
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Commit.Invoke(this, new RoutedEventArgs());
            }
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
        private void ValueTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueParser.IsVariablePattern(ValueString1))
            {
                CommentString1 = ValueCommentManager.GetComment(ValueString1);
            }
            else
            {
                CommentString1 = ValueCommentManager.GetComment(ValueString1.ToUpper());
            }
        }

        private void ValueTextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueParser.IsVariablePattern(ValueString2))
            {
                CommentString2 = ValueCommentManager.GetComment(ValueString2);
            }
            else
            {
                CommentString2 = ValueCommentManager.GetComment(ValueString2.ToUpper());
            }
        }

        private void ValueTextBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueParser.IsVariablePattern(ValueString3))
            {
                CommentString3 = ValueCommentManager.GetComment(ValueString3);
            }
            else
            {
                CommentString3 = ValueCommentManager.GetComment(ValueString3.ToUpper());
            }
        }

        private void ValueTextBox4_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueParser.IsVariablePattern(ValueString4))
            {
                CommentString4 = ValueCommentManager.GetComment(ValueString4);
            }
            else
            {
                CommentString4 = ValueCommentManager.GetComment(ValueString4.ToUpper());
            }
        }

        private void ValueTextBox5_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueParser.IsVariablePattern(ValueString5))
            {
                CommentString5 = ValueCommentManager.GetComment(ValueString5);
            }
            else
            {
                CommentString5 = ValueCommentManager.GetComment(ValueString5.ToUpper());
            }
        }

        private void ValueTextBox6_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueParser.IsVariablePattern(ValueString6))
            {
                CommentString6 = ValueCommentManager.GetComment(ValueString6);
            }
            else
            {
                CommentString6 = ValueCommentManager.GetComment(ValueString6.ToUpper());
            }
        }

        private void ValueTextBox7_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueParser.IsVariablePattern(ValueString7))
            {
                CommentString7 = ValueCommentManager.GetComment(ValueString7);
            }
            else
            {
                CommentString7 = ValueCommentManager.GetComment(ValueString7.ToUpper());
            }
        }
        #endregion

        public IList<string> PropertyStrings
        {
            get
            {
                List<string> result = new List<string>();
                switch (_valueCount)
                {
                    case 1:
                        result.Add(ValueString4);
                        result.Add(CommentString4);
                        break;
                    case 2:
                        result.Add(ValueString3);
                        result.Add(CommentString3);
                        result.Add(ValueString5);
                        result.Add(CommentString5);
                        break;
                    case 3:
                        result.Add(ValueString2);
                        result.Add(CommentString2);
                        result.Add(ValueString4);
                        result.Add(CommentString4);
                        result.Add(ValueString6);
                        result.Add(CommentString6);
                        break;
                    case 4:
                        result.Add(ValueString1);
                        result.Add(CommentString1);
                        result.Add(ValueString3);
                        result.Add(CommentString3);
                        result.Add(ValueString5);
                        result.Add(CommentString5);
                        result.Add(ValueString7);
                        result.Add(CommentString7);
                        break;
                    case 5:
                        result.Add(ValueString1);
                        result.Add(CommentString1);
                        result.Add(ValueString2);
                        result.Add(CommentString2);
                        result.Add(ValueString4);
                        result.Add(CommentString4);
                        result.Add(ValueString6);
                        result.Add(CommentString6);
                        result.Add(ValueString7);
                        result.Add(CommentString7);
                        break;
                    case 6:
                        result.Add(ValueString1);
                        result.Add(CommentString1);
                        result.Add(ValueString2);
                        result.Add(CommentString2);
                        result.Add(ValueString3);
                        result.Add(CommentString3);
                        result.Add(ValueString4);
                        result.Add(CommentString4);
                        result.Add(ValueString5);
                        result.Add(CommentString5);
                        result.Add(ValueString6);
                        result.Add(CommentString6);
                        break;
                    case 7:
                        result.Add(ValueString1);
                        result.Add(CommentString1);
                        result.Add(ValueString2);
                        result.Add(CommentString2);
                        result.Add(ValueString3);
                        result.Add(CommentString3);
                        result.Add(ValueString4);
                        result.Add(CommentString4);
                        result.Add(ValueString5);
                        result.Add(CommentString5);
                        result.Add(ValueString6);
                        result.Add(CommentString6);
                        result.Add(ValueString7);
                        result.Add(CommentString7);
                        break;
                }
                return result;
            }
        
        }
        void IPropertyDialog.ShowDialog()
        {
            ShowDialog();
        }
        void IPropertyDialog.Close()
        {
            Close();
        }
    }
}

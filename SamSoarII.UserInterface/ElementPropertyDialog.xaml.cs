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
using System.ComponentModel;

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// ElementPropertyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ElementPropertyDialog : Window, IPropertyDialog, INotifyPropertyChanged
    {
        private Label _StringToLabel(string text)
        {
            Label result = new Label();
            result.Content = text;
            return result;
        }

        private IEnumerable<string> subroutines = new List<string>();
        private IEnumerable<Label> subroutinelabels = new List<Label>();
        public IEnumerable<string> SubRoutines
        {
            get { return this.subroutines; }
            set
            {
                this.subroutines = value;
                subroutinelabels = value.Select(_StringToLabel);
                string _tempstring = ValueString4;
                ValueString4 = String.Empty;
                ValueString4 = _tempstring;
                _tempstring = ValueString5;
                ValueString5 = String.Empty;
                ValueString5 = _tempstring;
            }
        }

        private IEnumerable<string[]> functions = new List<string[]>();
        private IEnumerable<Label> functionlabels = new List<Label>();
        public IEnumerable<string[]> Functions
        {
            get { return this.functions; }
            set
            {
                this.functions = value;
                functionlabels = value.Select(msgs => { return _StringToLabel(msgs[1]); });
                string _tempstring = ValueString1;
                ValueString1 = String.Empty;
                ValueString1 = _tempstring;
            }
        }

        private IEnumerable<string> modbuss = new List<string>();
        private IEnumerable<Label> modbuslabels = new List<Label>();
        public IEnumerable<string> ModbusTables
        {
            get { return this.modbuss; }
            set
            {
                this.modbuss = value;
                this.modbuslabels = value.Select(_StringToLabel);
                string _tempstring = ValueString4;
                ValueString4 = String.Empty;
                ValueString4 = _tempstring;
            }
        }
        
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
        public int ValueCount
        {
            get { return this._valueCount; }
            set
            {
                if (InstMode == INST_CALLM)
                {
                    switch (value)
                    {
                        case 1:
                            ShowLine1();
                            HideLine2();
                            HideLine3();
                            HideLine4();
                            HideLine5();
                            HideLine6();
                            HideLine7();
                            break;
                        case 2:
                            ShowLine1();
                            ShowLine2();
                            HideLine3();
                            HideLine4();
                            HideLine5();
                            HideLine6();
                            HideLine7();
                            break;
                        case 3:
                            ShowLine1();
                            ShowLine2();
                            ShowLine3();
                            HideLine4();
                            HideLine5();
                            HideLine6();
                            HideLine7();
                            break;
                        case 4:
                            ShowLine1();
                            ShowLine2();
                            ShowLine3();
                            ShowLine4();
                            HideLine5();
                            HideLine6();
                            HideLine7();
                            break;
                        case 5:
                            ShowLine1();
                            ShowLine2();
                            ShowLine3();
                            ShowLine4();
                            ShowLine5();
                            HideLine6();
                            HideLine7();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("ValueCount out of range (1-5)");
                    }
                    this._valueCount = value;
                    return;
                }
                switch (value)
                {
                    case 0:
                        HideLine1();
                        HideLine2();
                        HideLine3();
                        HideLine4();
                        HideLine5();
                        HideLine6();
                        HideLine7();
                        break;
                    case 1:
                        HideLine1();
                        HideLine2();
                        HideLine3();
                        ShowLine4();
                        HideLine5();
                        HideLine6();
                        HideLine7();
                        break;
                    case 2:
                        HideLine1();
                        HideLine2();
                        ShowLine3();
                        HideLine4();
                        ShowLine5();
                        HideLine6();
                        HideLine7();
                        break;
                    case 3:
                        HideLine1();
                        ShowLine2();
                        HideLine3();
                        ShowLine4();
                        HideLine5();
                        ShowLine6();
                        HideLine7();
                        break;
                    case 4:
                        ShowLine1();
                        HideLine2();
                        ShowLine3();
                        HideLine4();
                        ShowLine5();
                        HideLine6();
                        ShowLine7();
                        break;
                    case 5:
                        ShowLine1();
                        ShowLine2();
                        HideLine3();
                        ShowLine4();
                        HideLine5();
                        ShowLine6();
                        ShowLine7();
                        break;
                    case 6:
                        ShowLine1();
                        ShowLine2();
                        ShowLine3();
                        ShowLine4();
                        ShowLine5();
                        ShowLine6();
                        HideLine7();
                        break;
                    case 7:
                        ShowLine1();
                        ShowLine2();
                        ShowLine3();
                        ShowLine4();
                        ShowLine5();
                        ShowLine6();
                        ShowLine7();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("ValueCount out of range (0-7)");
                }
                this._valueCount = value;
            }
        }

        public const int INST_NORMAL = 0x00;
        public const int INST_CALL = 0x01;
        public const int INST_CALLM = 0x02;
        public const int INST_ATCH = 0x03;
        public const int INST_MBUS = 0x04;
        private int instmode;
        public int InstMode
        {
            get
            {
                return this.instmode;
            }
            set
            {
                switch (value)
                {
                    case INST_NORMAL:
                        SelectCollectionPopup.Visibility = Visibility.Hidden;
                        break;
                    case INST_CALL:
                        SelectCollectionPopup.Visibility = Visibility.Visible;
                        SelectCollectionPopup.PlacementTarget = ValueTextBox4;
                        break;
                    case INST_CALLM:
                        SelectCollectionPopup.Visibility = Visibility.Visible;
                        SelectCollectionPopup.PlacementTarget = ValueTextBox1;
                        break;
                    case INST_ATCH:
                        SelectCollectionPopup.Visibility = Visibility.Visible;
                        SelectCollectionPopup.PlacementTarget = ValueTextBox5;
                        break;
                    case INST_MBUS:
                        SelectCollectionPopup.Visibility = Visibility.Visible;
                        SelectCollectionPopup.PlacementTarget = ValueTextBox4;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(String.Format("Invaild value assigned to InstMode : {0:d}", value));
                }
                this.instmode = value;
            }
        }
        
        public ElementPropertyDialog(int valuecount, int _InstModemode = INST_NORMAL)
        {
            InitializeComponent();
            DataContext = this;
            InstMode = _InstModemode;
            ValueCount = valuecount;
        }
        

        #region Show & Hide Lines

        #region Line1
        public void ShowLine1()
        {
            label1.Visibility = Visibility.Visible;
            ValueTextBox1.Visibility = Visibility.Visible;
            label1_c.Visibility = Visibility.Visible;
            CommentTextBox1.Visibility = Visibility.Visible;
            ValueTextBox1.Focus();
        }

        public void HideLine1()
        {
            label1.Visibility = Visibility.Hidden;
            ValueTextBox1.Visibility = Visibility.Hidden;
            label1_c.Visibility = Visibility.Hidden;
            CommentTextBox1.Visibility = Visibility.Hidden;
        }

        public void ShowLine1(string labelContent)
        {
            ShowLine1();
            label1.Content = labelContent;
        }

        public void ShowLine1(string labelContent, IValueModel value)
        {
            ShowLine1(labelContent);
            ValueString1 = value.ValueString;
        }
        #endregion

        #region Line2
        public void ShowLine2()
        {
            label2.Visibility = Visibility.Visible;
            ValueTextBox2.Visibility = Visibility.Visible;
            label2_c.Visibility = Visibility.Visible;
            CommentTextBox2.Visibility = Visibility.Visible;
            ValueTextBox2.Focus();
        }

        public void HideLine2()
        {
            label2.Visibility = Visibility.Hidden;
            ValueTextBox2.Visibility = Visibility.Hidden;
            label2_c.Visibility = Visibility.Hidden;
            CommentTextBox2.Visibility = Visibility.Hidden;
        }

        public void ShowLine2(string labelContent)
        {
            ShowLine2();
            label2.Content = labelContent;
        }

        public void ShowLine2(string labelContent, IValueModel value)
        {
            ShowLine2(labelContent);
            ValueString2 = value.ValueString;
        }
        #endregion

        #region Line3
        public void ShowLine3()
        {
            label3.Visibility = Visibility.Visible;
            ValueTextBox3.Visibility = Visibility.Visible;
            label3_c.Visibility = Visibility.Visible;
            CommentTextBox3.Visibility = Visibility.Visible;
            ValueTextBox3.Focus();
        }

        public void HideLine3()
        {
            label3.Visibility = Visibility.Hidden;
            ValueTextBox3.Visibility = Visibility.Hidden;
            label3_c.Visibility = Visibility.Hidden;
            CommentTextBox3.Visibility = Visibility.Hidden;
        }

        public void ShowLine3(string labelContent)
        {
            ShowLine3();
            label3.Content = labelContent;
        }

        public void ShowLine3(string labelContent, IValueModel value)
        {
            ShowLine3(labelContent);
            ValueString3 = value.ValueString;
        }
        #endregion

        #region Line4
        public void ShowLine4()
        {
            label4.Visibility = Visibility.Visible;
            ValueTextBox4.Visibility = Visibility.Visible;
            label4_c.Visibility = Visibility.Visible;
            CommentTextBox4.Visibility = Visibility.Visible;
            ValueTextBox4.Focus();
        }

        public void HideLine4()
        {
            label4.Visibility = Visibility.Hidden;
            ValueTextBox4.Visibility = Visibility.Hidden;
            label4_c.Visibility = Visibility.Hidden;
            CommentTextBox4.Visibility = Visibility.Hidden;
        }

        public void ShowLine4(string labelContent)
        {
            ShowLine4();
            label4.Content = labelContent;
        }

        public void ShowLine4(string labelContent, IValueModel value)
        {
            ShowLine4(labelContent);
            ValueString4 = value.ValueString;
        }
        #endregion

        #region Line5
        public void ShowLine5()
        {
            label5.Visibility = Visibility.Visible;
            ValueTextBox5.Visibility = Visibility.Visible;
            label5_c.Visibility = Visibility.Visible;
            CommentTextBox5.Visibility = Visibility.Visible;
            ValueTextBox5.Focus();
        }

        public void HideLine5()
        {
            label5.Visibility = Visibility.Hidden;
            ValueTextBox5.Visibility = Visibility.Hidden;
            label5_c.Visibility = Visibility.Hidden;
            CommentTextBox5.Visibility = Visibility.Hidden;
        }

        public void ShowLine5(string labelContent)
        {
            ShowLine5();
            label5.Content = labelContent;
        }

        public void ShowLine5(string labelContent, IValueModel value)
        {
            ShowLine5(labelContent);
            ValueString5 = value.ValueString;
        }
        #endregion

        #region Line6
        public void ShowLine6()
        {
            label6.Visibility = Visibility.Visible;
            ValueTextBox6.Visibility = Visibility.Visible;
            label6_c.Visibility = Visibility.Visible;
            CommentTextBox6.Visibility = Visibility.Visible;
            ValueTextBox6.Focus();
        }

        public void HideLine6()
        {
            label6.Visibility = Visibility.Hidden;
            ValueTextBox6.Visibility = Visibility.Hidden;
            label6_c.Visibility = Visibility.Hidden;
            CommentTextBox6.Visibility = Visibility.Hidden;
        }

        public void ShowLine6(string labelContent)
        {
            ShowLine6();
            label6.Content = labelContent;
        }

        public void ShowLine6(string labelContent, IValueModel value)
        {
            ShowLine6(labelContent);
            ValueString6 = value.ValueString;
        }
        #endregion

        #region Line7
        public void ShowLine7()
        {
            label7.Visibility = Visibility.Visible;
            ValueTextBox7.Visibility = Visibility.Visible;
            label7_c.Visibility = Visibility.Visible;
            CommentTextBox7.Visibility = Visibility.Visible;
            ValueTextBox7.Focus();
        }

        public void HideLine7()
        {
            label7.Visibility = Visibility.Hidden;
            ValueTextBox7.Visibility = Visibility.Hidden;
            label7_c.Visibility = Visibility.Hidden;
            CommentTextBox7.Visibility = Visibility.Hidden;
        }

        public void ShowLine7(string labelContent)
        {
            ShowLine7();
            label7.Content = labelContent;
        }

        public void ShowLine7(string labelContent, IValueModel value)
        {
            ShowLine7(labelContent);
            ValueString7 = value.ValueString;
        }
        #endregion

        #endregion
        
        private void DrawFree(Label label)
        {
            System.Windows.Media.Color color = new System.Windows.Media.Color();
            color.A = 255;
            color.R = 255;
            color.G = 255;
            color.B = 255;
            label.Background = new SolidColorBrush(color);
            color.A = 255;
            color.R = 0;
            color.G = 0;
            color.B = 0;
            label.Foreground = new SolidColorBrush(color);
            label.FontWeight = FontWeights.Normal;
        }

        private void DrawCursor(Label label)
        {
            System.Windows.Media.Color color = new System.Windows.Media.Color();
            color.A = 255;
            color.R = 0;
            color.G = 0;
            color.B = 0;
            label.Background = new SolidColorBrush(color);
            color.A = 255;
            color.R = 255;
            color.G = 255;
            color.B = 255;
            label.Foreground = new SolidColorBrush(color);
            label.FontWeight = FontWeights.Heavy;
        }

        #region Event handler

        public event RoutedEventHandler Commit = delegate { };

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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
        
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Label)
            {
                Label label = sender as Label;
                DrawCursor(label);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Label)
            {
                Label label = sender as Label;
                DrawFree(label);
            }
        }

        private void OnLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label)
            {
                Label label = sender as Label;
                if (label.Content is string &&
                    SelectCollectionPopup.PlacementTarget is TextBox)
                {
                    TextBox tbox = (TextBox)(SelectCollectionPopup.PlacementTarget);
                    string text = (string)(label.Content);
                    tbox.Text = text;
                }
            }
        }

        private void ValueTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (InstMode)
            {
                case INST_CALLM:
                    IEnumerable<string[]> fit = Functions.Where(
                        (string[] msgs) => { return msgs[1].Equals(ValueString1); });
                    if (fit.Count() == 0)
                    {
                        ValueTextBox1.Background = Brushes.Red;
                        ValueCount = 1;
                    }
                    else
                    {
                        ValueTextBox1.Background = Brushes.White;
                        string[] msgs = fit.First();
                        ValueCount = msgs.Length / 2;
                        switch (ValueCount)
                        {
                            case 1:
                                ShowLine1("FUNC");
                                ValueString1 = msgs[1];
                                break;
                            case 2:
                                ShowLine1("FUNC");
                                ValueString1 = msgs[1];
                                ShowLine2(msgs[2].Remove(msgs[2].Length - 1));
                                //ValueString2 = String.Empty;
                                break;
                            case 3:
                                ShowLine1("FUNC");
                                ValueString1 = msgs[1];
                                ShowLine2(msgs[2].Remove(msgs[2].Length - 1));
                                //ValueString2 = String.Empty;
                                ShowLine3(msgs[4].Remove(msgs[4].Length - 1));
                                //ValueString3 = String.Empty;
                                break;
                            case 4:
                                ShowLine1("FUNC");
                                ValueString1 = msgs[1];
                                ShowLine2(msgs[2].Remove(msgs[2].Length - 1));
                                //ValueString2 = String.Empty;
                                ShowLine3(msgs[4].Remove(msgs[4].Length - 1));
                                //ValueString3 = String.Empty;
                                ShowLine4(msgs[6].Remove(msgs[6].Length - 1));
                                //ValueString4 = String.Empty;
                                break;
                            case 5:
                                ShowLine1("FUNC");
                                ValueString1 = msgs[1];
                                ShowLine2(msgs[2].Remove(msgs[2].Length - 1));
                                //ValueString2 = String.Empty;
                                ShowLine3(msgs[4].Remove(msgs[4].Length - 1));
                                //ValueString3 = String.Empty;
                                ShowLine4(msgs[6].Remove(msgs[6].Length - 1));
                                //ValueString4 = String.Empty;
                                ShowLine5(msgs[8].Remove(msgs[8].Length - 1));
                                //ValueString5 = String.Empty;
                                break;
                            default:
                                throw new ArgumentException("Error Function Messages Array");
                        }
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("CollectionSource"));
                    return;
            }
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
            IEnumerable<string> fit = null;
            switch (InstMode)
            {
                case INST_CALL:
                    fit = SubRoutines.Where(
                        (string name) => { return name.Equals(ValueString4); });
                    if (fit.Count() == 0)
                    {
                        ValueTextBox4.Background = Brushes.Red;
                    }
                    else
                    {
                        ValueTextBox4.Background = Brushes.White;
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("CollectionSource"));
                    return;
                case INST_MBUS:
                    fit = ModbusTables.Where(
                        (string name) => { return name.Equals(ValueString4); });
                    if (fit.Count() == 0)
                    {
                        ValueTextBox4.Background = Brushes.Red;
                    }
                    else
                    {
                        ValueTextBox4.Background = Brushes.White;
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("CollectionSource"));
                    return;
            }
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
            switch (InstMode)
            {
                case INST_ATCH:
                    IEnumerable<string> fit = SubRoutines.Where(
                        (string name) => { return name.Equals(ValueString5); });
                    if (fit.Count() == 0)
                    {
                        ValueTextBox5.Background = Brushes.Red;
                    }
                    else
                    {
                        ValueTextBox5.Background = Brushes.White;
                    }
                    PropertyChanged(this, new PropertyChangedEventArgs("CollectionSource"));
                    return;
            }
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

        private IList<string> pstrings_old;
        //private IList<string> pstrings_new;

        public void SavePropertyString()
        {
            pstrings_old = PropertyStrings;
        }

        public IList<string> PropertyStrings_Old
        {
            get
            {
                return pstrings_old;
            }
        }

        public IList<string> PropertyStrings_New
        {
            get
            {
                return PropertyStrings;
            }
        }

        public IList<string> PropertyStrings
        {
            get
            {
                List<string> result = new List<string>();
                if (InstMode == INST_CALLM)
                {
                    IEnumerable<string[]> fit = Functions.Where(
                        (string[] msgs) => { return msgs[1].Equals(ValueString1); });
                    if (fit.Count() != 0)
                    {
                        string[] msgs = fit.First();
                        result.Add(ValueString1);
                        result.Add(CommentString1);
                        if (ValueCount > 1)
                        {
                            result.Add(msgs[2]);
                            result.Add(msgs[3]);
                            result.Add(ValueString2);
                            result.Add(CommentString2);
                        }
                        if (ValueCount > 2)
                        {
                            result.Add(msgs[4]);
                            result.Add(msgs[5]);
                            result.Add(ValueString3);
                            result.Add(CommentString3);
                        }
                        if (ValueCount > 3)
                        {
                            result.Add(msgs[6]);
                            result.Add(msgs[7]);
                            result.Add(ValueString4);
                            result.Add(CommentString4);
                        }
                        if (ValueCount > 4)
                        {
                            result.Add(msgs[8]);
                            result.Add(msgs[9]);
                            result.Add(ValueString5);
                            result.Add(CommentString5);
                        }
                    }
                    return result;
                }
                switch (ValueCount)
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

        public IEnumerable<Label> CollectionSource
        {
            get
            {
                switch (InstMode)
                {
                    case INST_CALLM:
                        if (ValueTextBox1.Background == Brushes.Red)
                        {
                            return functionlabels.Where((Label label) => { return (label.Content is string && ((string)(label.Content)).StartsWith(ValueString1)); });
                        }
                        else
                        {
                            return new List<Label>();
                        }
                    case INST_CALL:
                        if (ValueTextBox4.Background == Brushes.Red)
                        {
                            return subroutinelabels.Where((Label label) => { return (label.Content is string && ((string)(label.Content)).StartsWith(ValueString4)); });
                        }
                        else
                        {
                            return new List<Label>();
                        }
                    case INST_ATCH:
                        if (ValueTextBox5.Background == Brushes.Red)
                        {
                            return subroutinelabels.Where((Label label) => { return (label.Content is string && ((string)(label.Content)).StartsWith(ValueString5)); });
                        }
                        else
                        {
                            return new List<Label>();
                        }
                    case INST_MBUS:
                        if (ValueTextBox4.Background == Brushes.Red)
                        {
                            return modbuslabels.Where((Label label) => { return (label.Content is string && ((string)(label.Content)).StartsWith(ValueString4)); });
                        }
                        else
                        {
                            return new List<Label>();
                        }
                    default:
                        return new List<Label>();
                }
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

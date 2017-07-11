using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamSoarII.UserInterface
{
    public enum LocalizedMessageResult
    {
        None,
        Cancel,
        Yes,
        No
    }

    public enum LocalizedMessageButton
    {
        YesNo,
        YesNoCancel,
        OK,
        OKCancel
    }

    public enum LocalizedMessageIcon
    {
        None,
        Warning,
        Error,
        Question,
        Information
    }
    /// <summary>
    /// LocalizedMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class LocalizedMessageBox : Window
    {
        public string Caption { get; set; }
        public string MessageToShow { get; set; }
        private LocalizedMessageResult _result = LocalizedMessageResult.None;
        private LocalizedMessageBox(string messageToShow ,string caption, LocalizedMessageButton button, LocalizedMessageIcon icon)
        {
            InitializeComponent();
            DataContext = this;
            MessageToShow = messageToShow;
            SetFontSize(messageToShow.ToCharArray().Length);
            Caption = caption;
            InitializeButton(button);
            InitializeIcon(icon);
            KeyDown += LocalizedMessageBox_KeyDown;
        }

        private void LocalizedMessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_1.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            if (e.Key == Key.Escape)
                Close();
        }

        private void InitializeButton(LocalizedMessageButton button)
        {
            switch (button)
            {
                case LocalizedMessageButton.YesNo:
                    SetButtons(new Button[] {Button_1,Button_2 });
                    Button_1.Content = Properties.Resources.YES;
                    Button_2.Content = Properties.Resources.NO;
                    Button_1.Click += (sender, e) => { _result = LocalizedMessageResult.Yes; Close(); };
                    Button_2.Click += (sender, e) => { _result = LocalizedMessageResult.No; Close(); };
                    break;
                case LocalizedMessageButton.YesNoCancel:
                    SetButtons(new Button[] { Button_1, Button_2 ,Button_3});
                    Button_1.Content = Properties.Resources.YES;
                    Button_2.Content = Properties.Resources.NO;
                    Button_3.Content = Properties.Resources.Cancel;
                    Button_1.Click += (sender, e) => { _result = LocalizedMessageResult.Yes; Close(); };
                    Button_2.Click += (sender, e) => { _result = LocalizedMessageResult.No; Close(); };
                    Button_3.Click += (sender, e) => { _result = LocalizedMessageResult.Cancel; Close(); };
                    break;
                case LocalizedMessageButton.OK:
                    SetButtons(new Button[] { Button_1});
                    Button_1.Content = Properties.Resources.Ensure;
                    Button_1.Click += (sender, e) => { _result = LocalizedMessageResult.Yes; Close(); };
                    break;
                case LocalizedMessageButton.OKCancel:
                    SetButtons(new Button[] { Button_1, Button_2 });
                    Button_1.Content = Properties.Resources.Ensure;
                    Button_2.Content = Properties.Resources.Cancel;
                    Button_1.Click += (sender, e) => { _result = LocalizedMessageResult.Yes; Close(); };
                    Button_2.Click += (sender, e) => { _result = LocalizedMessageResult.No; Close(); };
                    break;
                default:
                    break;
            }
        }
        private void SetFontSize(int CharCount)
        {
            if (CharCount < 24) TB_MessageToShow.FontSize = 20;
            if (CharCount < 72) TB_MessageToShow.FontSize = 18;
            if (CharCount < 124) TB_MessageToShow.FontSize = 16;
            if (CharCount < 180) TB_MessageToShow.FontSize = 14;
            else TB_MessageToShow.FontSize = 13;
        }
        private void SetButtons(Button[] buttons)
        {
            foreach (var button in buttons)
            {
                button.Visibility = Visibility.Visible;
            }
            switch (buttons.Length)
            {
                case 1:
                    Canvas.SetRight(buttons[0],20);
                    break;
                case 2:
                    Canvas.SetRight(buttons[0], 110);
                    Canvas.SetRight(buttons[1], 20);
                    break;
                case 3:
                    Canvas.SetRight(buttons[0], 200);
                    Canvas.SetRight(buttons[1], 110);
                    Canvas.SetRight(buttons[2], 20);
                    B_Canvas.MinWidth = 320;
                    break;
                default:
                    break;
            }
        }
        private void InitializeIcon(LocalizedMessageIcon icon)
        {
            if (icon != LocalizedMessageIcon.None)
            {
                IconToShow.Visibility = Visibility.Visible;
                TB_MessageToShow.Margin = new Thickness(10, 35, 40, 20);
            }
            else
                TB_MessageToShow.Margin = new Thickness(40, 35, 40, 20);
            switch (icon)
            {
                case LocalizedMessageIcon.Warning:
                    IconToShow.Source = new BitmapImage(new Uri("pack://application:,,,/SamSoarII;component/Resources/Image/Warning.png"));
                    break;
                case LocalizedMessageIcon.Error:
                    IconToShow.Source = new BitmapImage(new Uri("pack://application:,,,/SamSoarII;component/Resources/Image/Error.png"));
                    break;
                case LocalizedMessageIcon.Question:
                    IconToShow.Source = new BitmapImage(new Uri("pack://application:,,,/SamSoarII;component/Resources/Image/Question.png"));
                    break;
                case LocalizedMessageIcon.Information:
                    IconToShow.Source = new BitmapImage(new Uri("pack://application:,,,/SamSoarII;component/Resources/Image/Information.png"));
                    break;
                default:
                    break;
            }
        }

        public static LocalizedMessageResult Show(string messageToShow, string caption = "", LocalizedMessageButton button = LocalizedMessageButton.OK, LocalizedMessageIcon icon = LocalizedMessageIcon.None)
        {
            LocalizedMessageBox messagebox = new LocalizedMessageBox(messageToShow,caption,button,icon);
            messagebox.ShowDialog();
            return messagebox._result;
        }
        public static LocalizedMessageResult Show(string messageToShow, LocalizedMessageIcon icon)
        {
            LocalizedMessageBox messagebox = new LocalizedMessageBox(messageToShow, string.Empty, LocalizedMessageButton.OK, icon);
            messagebox.ShowDialog();
            return messagebox._result;
        }
        public static LocalizedMessageResult Show(string messageToShow, LocalizedMessageButton button, LocalizedMessageIcon icon)
        {
            LocalizedMessageBox messagebox = new LocalizedMessageBox(messageToShow, string.Empty, button, icon);
            messagebox.ShowDialog();
            return messagebox._result;
        }
        public static LocalizedMessageResult Show(string messageToShow, LocalizedMessageButton button)
        {
            LocalizedMessageBox messagebox = new LocalizedMessageBox(messageToShow, string.Empty, button, LocalizedMessageIcon.None);
            messagebox.ShowDialog();
            return messagebox._result;
        }
        public static LocalizedMessageResult Show(string messageToShow,string caption, LocalizedMessageIcon icon)
        {
            LocalizedMessageBox messagebox = new LocalizedMessageBox(messageToShow, caption, LocalizedMessageButton.OK, icon);
            messagebox.ShowDialog();
            return messagebox._result;
        }
    }
}

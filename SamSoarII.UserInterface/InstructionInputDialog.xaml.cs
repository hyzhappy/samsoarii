using System;
using System.Collections.Generic;
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

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// InstructionInputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InstructionInputDialog : Window
    {
        public event RoutedEventHandler EnsureButtonClick = delegate { };

        public string InstructionInput
        {
            get
            {
                return InstructionInputTextBox.Text;
            }
            set
            {
                InstructionInputTextBox.Text = value;
            }
        }

        public InstructionInputDialog(string initialString)
        {
            InitializeComponent();
            InstructionInput = initialString;
            Loaded += (sender, e) =>
            {
                InstructionInputTextBox.Focus();
                InstructionInputTextBox.Select(InstructionInputTextBox.Text.Length, 0);
                this.LocationChanged += (sender1, e1) =>
                {
                   
                };
            };
        }

        public void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            EnsureButtonClick.Invoke(sender, e);
        }


        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
            if(e.Key == Key.Enter)
            {
                EnsureButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void OnInstructionTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}

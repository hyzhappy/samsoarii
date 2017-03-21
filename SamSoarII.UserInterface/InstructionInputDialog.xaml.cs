using System;
using System.Collections.Generic;
using System.Drawing;
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
        private System.Windows.Forms.AutoCompleteStringCollection instructionNames = new System.Windows.Forms.AutoCompleteStringCollection();
        private Dictionary<string, List<string>> instructionNameAndToolTips;
        private string oldText = string.Empty;
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

        public InstructionInputDialog(string initialString, Dictionary<string,List<string>> instructionNameAndToolTips)
        {
            InitializeComponent();
            InstructionInput = initialString;
            Font font = new Font("Arial", 12);
            InstructionInputTextBox.Font = font;
            Loaded += (sender, e) =>
            {
                InstructionInputTextBox.Focus();
                InstructionInputTextBox.Select(InstructionInputTextBox.Text.Length, 0);
                this.instructionNameAndToolTips = instructionNameAndToolTips;
                instructionNames.AddRange(this.instructionNameAndToolTips.Keys.ToArray());
                InstructionInputTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
                InstructionInputTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
                InstructionInputTextBox.AutoCompleteCustomSource = instructionNames;
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
        private bool CheckInstructionName(string InstructionName)
        {
            return instructionNames.Contains(InstructionName);
        }

        private void OnLocationChanged(object sender, EventArgs e)
        {
            if (TextBoxPopup.IsOpen)
            {
                TextBoxPopup.Placement = PlacementMode.Absolute;
                TextBoxPopup.VerticalOffset = window.Top - 60;
                TextBoxPopup.HorizontalOffset = window.Left + 128;
            }
        }
        private void OnTextChanged(object sender, EventArgs e)
        {
            if (InstructionInputTextBox.Text.Length > oldText.Length && InstructionInputTextBox.Text.TrimEnd().Length == oldText.Length)
            {
                List<string> InstructionInput = InstructionInputTextBox.Text.ToUpper().Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                if (!TextBoxPopup.IsOpen)
                {
                    if (CheckInstructionName(InstructionInput[0]))
                    {
                        List<string> tempList = new List<string>();
                        instructionNameAndToolTips.TryGetValue(InstructionInput[0], out tempList);
                        textblock_name.Text = InstructionInput[0];
                        textblock_describe.Text = tempList[0];
                        textblock_1.Text = tempList[1];
                        textblock_1.FontWeight = FontWeights.Heavy;
                        textblock_2.Text = tempList[2];
                        textblock_3.Text = tempList[3];
                        TextBoxPopup.VerticalOffset = -29;
                        TextBoxPopup.IsOpen = true;
                    }
                }
                else
                {
                    if (InstructionInput.Count == 1)
                    {
                        textblock_1.FontWeight = FontWeights.Heavy;
                    }
                    else
                    {
                        if (InstructionInput.Count == 2)
                        {
                            textblock_2.FontWeight = FontWeights.Heavy;
                        }
                        else if (InstructionInput.Count == 3)
                        {
                            textblock_2.FontWeight = FontWeights.Light;
                            textblock_3.FontWeight = FontWeights.Heavy;
                        }
                        else if (InstructionInput.Count == 4)
                        {
                            textblock_3.FontWeight = FontWeights.Light;
                        }
                        textblock_1.FontWeight = FontWeights.Light;
                    }
                }
            }
            else if(InstructionInputTextBox.Text.Length < oldText.Length)
            {
                List<string> InstructionInput = InstructionInputTextBox.Text.ToUpper().Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                if (InstructionInputTextBox.Text == InstructionInputTextBox.Text.TrimEnd())
                {
                    switch (InstructionInput.Count)
                    {
                        case 1:
                            textblock_1.FontWeight = FontWeights.Light;
                            TextBoxPopup.IsOpen = false;
                            TextBoxPopup.Placement = PlacementMode.Top;
                            TextBoxPopup.VerticalOffset = -2;
                            TextBoxPopup.HorizontalOffset = 125;
                            break;
                        case 2:
                            textblock_2.FontWeight = FontWeights.Light;
                            textblock_1.FontWeight = FontWeights.Heavy;
                            break;
                        case 3:
                            textblock_3.FontWeight = FontWeights.Light;
                            textblock_2.FontWeight = FontWeights.Heavy;
                            break;
                        case 4:
                            textblock_3.FontWeight = FontWeights.Heavy;
                            break;
                        default:
                            break;
                    }
                }
            }
            oldText = InstructionInputTextBox.Text;
        }
    }
}

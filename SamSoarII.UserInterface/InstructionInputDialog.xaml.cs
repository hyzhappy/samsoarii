using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class InstructionInputDialog : Window, INotifyPropertyChanged
    {
        public event RoutedEventHandler EnsureButtonClick = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private List<string> instructionNames = new List<string>();
        private Dictionary<string, List<string>> instructionNameAndToolTips;
        private List<Label> _collectionSource = new List<Label>();
        private string oldText = string.Empty;
        private string currentText = string.Empty;
        public IEnumerable<Label> CollectionSource
        {
            get
            {
                if (currentText.Trim() == string.Empty)
                {
                    return new List<Label>();
                }
                else
                {
                    return _collectionSource.Where(x => { return (x.Content as string).StartsWith(currentText.ToUpper()); });
                }
            }
            set
            {
                CollectionSource = value;
            }
        }
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
            DataContext = this;
            Loaded += (sender, e) =>
            {
                InstructionInputTextBox.Focus();
                InstructionInputTextBox.Select(InstructionInputTextBox.Text.Length, 0);
                this.instructionNameAndToolTips = instructionNameAndToolTips;
                instructionNames.AddRange(this.instructionNameAndToolTips.Keys.ToArray());
                foreach (var name in instructionNames)
                {
                    Label temp = new Label();
                    temp.Content = name;
                    _collectionSource.Add(temp);
                }
            };
        }

        public void OnEnsureButtonClick(object sender, RoutedEventArgs e)
        {
            EnsureButtonClick.Invoke(sender, e);
        }


        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            if(e.Key == Key.Enter)
            {
                if (currentText != string.Empty)
                {
                    Label label = CollectionStack.SelectItem;
                    if (label != null)
                    {
                        if (label.Content as string != oldText)
                        {
                            InstructionInputTextBox.Tag = "true";
                            InstructionInputTextBox.Text = label.Content as string;
                        }
                        else
                        {
                            ResetStatus(label);
                        }
                    }
                }
                else
                {
                    EnsureButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
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
            SelectCollectionPopup.Placement = PlacementMode.Absolute;
            SelectCollectionPopup.VerticalOffset = window.Top + 69;
            SelectCollectionPopup.HorizontalOffset = window.Left + 40;
            ItemPopup.IsOpen = false;
        }
        private void OnTextChanged(object sender, EventArgs e)
        {
            StyleChange(CollectionStack.SelectItem);
            if (InstructionInputTextBox.Tag as string == "true")
            {
                InstructionInputTextBox.Tag = "false";
                currentText = string.Empty;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CollectionSource"));
                oldText = InstructionInputTextBox.Text;
                InstructionInputTextBox.SelectionStart = oldText.Length;
                CollectionStack.SelectItem = null;
            }
            else
            {
                currentText = InstructionInputTextBox.Text.ToUpper();
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CollectionSource"));
                if (CheckInstructionName(currentText))
                {
                    CollectionStack.SelectItem = CollectionSource.Where(x => { return x.Content as string == currentText; }).First();
                    SetPopup(CollectionStack.SelectItem);
                }
                else
                {
                    CollectionStack.SelectItem = null;
                }
                if (currentText.Length > oldText.Length && currentText.TrimEnd().Length == oldText.Length)
                {
                    List<string> InstructionInput = currentText.ToUpper().Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
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
                else if (currentText.Length < oldText.Length)
                {
                    List<string> InstructionInput = currentText.ToUpper().Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (currentText == currentText.TrimEnd())
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
                oldText = currentText;
            }
        }
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Label label = sender as Label;
            InstructionInputTextBox.Tag = "true";
            if (label.Content as string != oldText)
            {
                InstructionInputTextBox.Text = label.Content as string;
            }
            else
            {
                ResetStatus(label);
            }
        }
        private void StyleChange(Label label)
        {
            if (label != null)
            {
                RemovePopup();
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
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Label label = sender as Label;
            if (label != CollectionStack.SelectItem)
            {
                StyleChange(CollectionStack.SelectItem);
                CollectionStack.SelectItem = label;
                SetPopup(label);
            }
        }
        private void UpStyleChange(Label label)
        {
            if (label != null)
            {
                int index = CollectionSource.ToList().IndexOf(label);
                if (index != 0)
                {
                    int offset = GetScrollViewerOffset();
                    if (offset == index)
                    {
                        SetScrollViewerOffset(offset - 1);
                    }
                    else if (offset > index || offset + 9 < index)
                    {
                        SetScrollViewerOffset(index - 1);
                    }
                    StyleChange(label);
                    CollectionStack.SelectItem = CollectionSource.ElementAt(index - 1);
                }
            }
        }
        private void DownStyleChange(Label label)
        {
            if (label == null)
            {
                CollectionStack.SelectItem = CollectionSource.First();
            }
            else
            {
                int index = CollectionSource.ToList().IndexOf(label);
                if (index != CollectionSource.Count() - 1)
                {
                    int offset = GetScrollViewerOffset();
                    if (offset + 9 == index)
                    {
                        SetScrollViewerOffset(offset + 1);
                    }
                    else if (offset > index || offset + 9 < index)
                    {
                        SetScrollViewerOffset(index + 1);
                    }
                    StyleChange(label);
                    CollectionStack.SelectItem = CollectionSource.ElementAt(index + 1);
                }
            }
        }
        private int GetScrollViewerOffset()
        {
            return (int)scrollViewer.VerticalOffset; 
        }
        private void SetScrollViewerOffset(int offset)
        {
            scrollViewer.ScrollToVerticalOffset(offset);
        }
        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Label label = CollectionStack.SelectItem;
            if (CollectionSource.Count() != 0)
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Up)
                {
                    UpStyleChange(label);
                    SetPopup(CollectionStack.SelectItem);
                    e.Handled = true;
                }
                if (e.KeyCode == System.Windows.Forms.Keys.Down)
                {
                    DownStyleChange(label);
                    SetPopup(CollectionStack.SelectItem);
                    e.Handled = true;
                }
            }
        }
        private void ResetStatus(Label label)
        {
            StyleChange(label);
            CollectionStack.SelectItem = null;
            currentText = string.Empty;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CollectionSource"));
        }
        private void OnMouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ResetStatus(CollectionStack.SelectItem);
        }
        private void SetPopup(Label label)
        {
            if (label != null)
            {
                int index = CollectionSource.ToList().IndexOf(label);
                int offset = GetScrollViewerOffset();
                int interval = index - offset;
                ItemPopup.PlacementTarget = window;
                ItemPopup.Placement = PlacementMode.Absolute;
                ItemPopup.HorizontalOffset = window.Left + 340;
                ItemPopup.VerticalOffset = window.Top + 69 + interval * 20;
                ItemPopup.IsOpen = true;
            }
        }
        private void RemovePopup()
        {
            ItemPopup.IsOpen = false;
            ItemPopup.PlacementTarget = null;
        }
        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (CollectionStack.SelectItem != null)
            {
                int offset = GetScrollViewerOffset();
                int index = CollectionSource.ToList().IndexOf(CollectionStack.SelectItem);
                int interval = index - offset;
                if (index >= offset && index <= offset + 9)
                {
                    ItemPopup.VerticalOffset = window.Top + 69 + interval * 20;
                }
                else
                {
                    RemovePopup();
                }
            }
        }
    }
}

using SamSoarII.Core.Models;
using SamSoarII.Global;
using SamSoarII.HelpDocument;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
using System.Windows.Threading;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// InstructionInputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InstructionInputDialog : Window, IDisposable, INotifyPropertyChanged
    {
        public InstructionInputDialog(
            ProjectModel _project,
            string initialString)
        {
            InitializeComponent();
            project = _project;
            instructionNames = LadderUnitModel.Formats.Where(f => f.Type < LadderUnitModel.Types.PID).Select(f => f.Name).ToList();
            subdiagramNames = project.Diagrams.Select(
                (diagram) => { return diagram.Name; }).ToList();
            modbusNames = project.Modbus.Children.Select(
                (modbus) => { return modbus.Name; }).ToList();
            functionMessages = project.Funcs.Where(
                (FuncModel fmodel) => { return fmodel.CanCALLM(); }
            ).Select(
                (FuncModel fmodel) => { return fmodel.GetMessageList(); }
            ).ToList();

            TB_Args = new TextBlock[6];
            TBD_Args = new TextBlock[6];
            for (int i = 0; i < 6; i++)
            {
                TB_Args[i] = new TextBlock();
                Grid.SetRow(TB_Args[i], i);
                Grid.SetColumn(TB_Args[i], 0);
                TBPGrid.Children.Add(TB_Args[i]);
                TBD_Args[i] = new TextBlock();
                Grid.SetRow(TBD_Args[i], i);
                Grid.SetColumn(TBD_Args[i], 1);
                TBPGrid.Children.Add(TBD_Args[i]);
            }

            InstructionInput = initialString;
            Font font = new Font("Arial", 12);
            InstructionInputTextBox.Font = font;
            DataContext = this;
            OpenTimer.Interval = TimeSpan.FromSeconds(0.5);
            OpenTimer.Tick += OpenTimer_Tick;
            Loaded += OnWindowLoaded;
        }

        public void Dispose()
        {
            project = null;
        }
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public event RoutedEventHandler EnsureButtonClick = delegate { };
        private ProjectModel project;
        private List<string> instructionNames;
        private List<string> subdiagramNames;
        private List<string[]> functionMessages;
        private List<string> modbusNames = new List<string>();
        private List<Label> _collectionSource = new List<Label>();
        private List<Label> _subdiagramSource = new List<Label>();
        private List<Label> _functionSource = new List<Label>();
        private List<Label> _modbusSource = new List<Label>();
        private DispatcherTimer OpenTimer = new DispatcherTimer();
        private string oldText = string.Empty;
        private string currentText = string.Empty;
        private TextBlock[] TB_Args;
        private TextBlock[] TBD_Args;

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
                    string[] words = currentText.Split(' ');
                    if (words.Length == 1)
                    {
                        return _collectionSource.Where(x => { return (x.Content as string).StartsWith(words[0].ToUpper()); });
                    }
                    else if (words.Length > 1)
                    {
                        switch (words[0].ToUpper())
                        {
                            case "CALL":
                                if (words.Length == 2)
                                {
                                    return _subdiagramSource.Where(x => { return (x.Content as string).StartsWith(words[1]); });
                                }
                                break;
                            case "ATCH":
                                if (words.Length == 3)
                                {
                                    return _subdiagramSource.Where(x => { return (x.Content as string).StartsWith(words[2]); });
                                }
                                break;
                            case "CALLM":
                                if (words.Length == 2)
                                {
                                    return _functionSource.Where(x => { return (x.Content as string).StartsWith(words[1]); });
                                }
                                break;
                            case "MBUS":
                                if (words.Length == 3)
                                {
                                    return _modbusSource.Where(x => { return (x.Content as string).StartsWith(words[2]); });
                                }
                                break;
                        }
                    }
                    return new List<Label>();
                }
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
        
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            InstructionInputTextBox.Focus();
            InstructionInputTextBox.Select(InstructionInputTextBox.Text.Length, 0);
            foreach (var name in instructionNames)
            {
                Label temp = new Label();
                temp.Content = name;
                _collectionSource.Add(temp);
            }
            foreach (var name in subdiagramNames)
            {
                Label temp = new Label();
                temp.Content = name;
                _subdiagramSource.Add(temp);
            }
            foreach (var msgs in functionMessages)
            {
                Label temp = new Label();
                temp.Content = msgs[1];
                _functionSource.Add(temp);
            }
            foreach (var name in modbusNames)
            {
                Label temp = new Label();
                temp.Content = name;
                _modbusSource.Add(temp);
            }
            Loaded -= OnWindowLoaded;
        }

        private void OpenTimer_Tick(object sender, EventArgs e)
        {
            PopupTextblock1.Text = CollectionStack.SelectItem.Content as string;
            ItemPopup.IsOpen = true;
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
            if (e.Key == Key.Enter)
            {
                if (CollectionSource.Count() != 0)
                {
                    Label label = CollectionStack.SelectItem;
                    if (label != null)
                    {
                        if (label.Content as string != oldText)
                        {
                            InstructionInputTextBox.Tag = "true";
                            string[] words = InstructionInputTextBox.Text.Split(' ');
                            words[words.Length - 1] = label.Content as string;
                            InstructionInputTextBox.Text = words[0];
                            for (int i = 1; i < words.Length; i++)
                            {
                                InstructionInputTextBox.Text += " " + words[i];
                            }
                            InstructionInputTextBox.SelectionStart = InstructionInputTextBox.Text.Length;
                            //InstructionInputTextBox.Text = label.Content as string;
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
            OpenTimer.Stop();
            if (TextBoxPopup.IsOpen)
            {
                TextBoxPopup.Placement = PlacementMode.Absolute;
                TextBoxPopup.VerticalOffset = window.Top - 150;
                TextBoxPopup.HorizontalOffset = window.Left;
            }
            SelectCollectionPopup.Placement = PlacementMode.Absolute;
            SelectCollectionPopup.VerticalOffset = window.Top + 69;
            SelectCollectionPopup.HorizontalOffset = window.Left + 44;
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
            currentText = InstructionInputTextBox.Text;
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CollectionSource"));
            if (CheckInstructionName(currentText.ToUpper()))
            {
                IEnumerable<Label> fit = CollectionSource.Where(x => { return x.Content as string == currentText; });
                if (fit.Count() > 0)
                {
                    CollectionStack.SelectItem = fit.First();
                    SetPopup(CollectionStack.SelectItem);
                }
            }
            else
            {
                CollectionStack.SelectItem = null;
            }
            List<string> InstructionInput = currentText.Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            int inputcurrent = InstructionInput.Count() - 1;
            if (currentText.EndsWith(" ")) inputcurrent++;
            if (InstructionInput.Count() > 0 && CheckInstructionName(InstructionInput[0].ToUpper()))
            {
                LadderUnitFormat format = LadderUnitModel.Formats.Where(f => f.Name.Equals(InstructionInput[0].ToUpper())).First();
                if (InstructionInput[0].ToUpper().Equals("CALLM") && InstructionInput.Count() > 1)
                {
                    FuncModel fmodel = project.Funcs.Where(f => f.Name.Equals(InstructionInput[1])).FirstOrDefault();
                    if (fmodel != null && fmodel.CanCALLM())
                    {
                        IList<ValueModel> values = fmodel.GetValueModels(null);
                        format = new LadderUnitFormat(format.CatalogID, "CALLM", LadderUnitModel.Types.CALLM,
                            LadderUnitModel.Outlines.ProgramControl, LadderUnitModel.Shapes.OutputRect,
                            format.Describe, format.Detail, format.Detail,
                            new ValueFormat[] { new ValueFormat("F", ValueModel.Types.STRING, true, false, 0, new ValueRegex[] { ValueModel.AnyNameRegex }, fmodel.Comment) }
                            .Concat(values.Select(v => v.Format)).ToArray());
                    }
                }
                if (InstructionInput.Count() > 5)
                {
                    TextBoxPopup.IsOpen = false;
                }
                for (int i = 0; i < 6; i++)
                {
                    TB_Args[i].Text = i < InstructionInput.Count()
                        ? InstructionInput[i] : "";
                    TBD_Args[i].Text = i == 0
                        ? format.Describe
                        : i <= format.Formats.Count
                            ? format.Formats[i - 1].Detail : "";
                    if (TB_Args[i].Text.Length > 0 && TBD_Args[i].Text.Length == 0)
                    {
                        TextBoxPopup.IsOpen = false;
                    }
                    if (inputcurrent == i)
                    {
                        TB_Args[i].FontWeight = FontWeights.Heavy;
                        TBD_Args[i].FontWeight = FontWeights.Heavy;
                    }
                    else
                    {
                        TB_Args[i].FontWeight = FontWeights.Light;
                        TBD_Args[i].FontWeight = FontWeights.Light;
                    }
                }
                TB_Detail.Text = format.Detail;
                if (!TextBoxPopup.IsOpen)
                {
                    //TextBoxPopup.VerticalOffset = -29;
                    TextBoxPopup.IsOpen = true;
                }
            }
            else
            {
                if (TextBoxPopup.IsOpen)
                {
                    TextBoxPopup.IsOpen = false;
                }
            }
            oldText = currentText;
        }
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Label label = sender as Label;
            InstructionInputTextBox.Tag = "true";
            if (label.Content as string != oldText)
            {
                string[] words = InstructionInputTextBox.Text.Split(' ');
                words[words.Length - 1] = label.Content as string;
                InstructionInputTextBox.Text = words[0];
                for (int i = 1; i < words.Length; i++)
                {
                    InstructionInputTextBox.Text += " " + words[i];
                }
                InstructionInputTextBox.SelectionStart = InstructionInputTextBox.Text.Length;
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
                label.Background = System.Windows.Media.Brushes.White;
                label.Foreground = System.Windows.Media.Brushes.Black;
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
                ItemPopup.HorizontalOffset = window.Left + 344;
                ItemPopup.VerticalOffset = window.Top + 69 + interval * 20;
                SetPopupOpen(label);
            }
        }
        private void SetPopupOpen(Label label)
        {
            OpenTimer.Start();
        }
        private void RemovePopup()
        {
            ItemPopup.IsOpen = false;
            OpenTimer.Stop();
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
        private void OnHelpButtonClick(object sender, RoutedEventArgs e)
        {
            project.Parent.ShowHelpDocument();
        }
    }
}

using ICSharpCode.AvalonEdit;
using SamSoarII.AppMain.LadderCommand;
using SamSoarII.AppMain.Project;
using SamSoarII.LadderInstViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Xceed.Wpf.AvalonDock.Global;

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// TextReplaceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TextReplaceWindow : UserControl, INotifyPropertyChanged
    {
        #region Numbers

        private InteractionFacade parent;

        private LadderCommand.CommandManager _cmdmanager
            = new LadderCommand.CommandManager();

        private ObservableCollection<TextReplaceElement> items
            = new ObservableCollection<TextReplaceElement>();
        public IEnumerable<TextReplaceElement> Items
        {
            get
            {
                return this.items;
            }
            set
            {
                items.Clear();
                foreach (TextReplaceElement item in value)
                {
                    items.Add(item);
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Items"));
            }
        }

        public const int MODE_CURRENT = 0x00;
        public const int MODE_ALL = 0x01;
        private int mode;
        public int Mode
        {
            get { return this.mode; }
            set
            {
                this.mode = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Mode"));
                Find();
            }
        }

        #endregion

        #region Config

        private bool isregex;
        public bool IsRegex
        {
            get { return this.isregex; }
            set
            {
                this.isregex = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsRegex"));
            }
        }

        private bool ignorecase;
        public bool IgnoreCase
        {
            get { return this.ignorecase; }
            set
            {
                this.ignorecase = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsIgnoreUpper"));
            }
        }

        #endregion

        public TextReplaceWindow()
        {
            InitializeComponent();
        }

        private void Find()
        {
            items.Clear();
            switch (Mode)
            {
                case MODE_CURRENT:
                    ITabItem currenttab = parent.MainTabControl.CurrentTab;
                    if (currenttab is FuncBlockViewModel)
                    {
                        FuncBlockViewModel fbvmodel = (FuncBlockViewModel)currenttab;
                        Find(fbvmodel);
                    }
                    break;
                case MODE_ALL:
                    ProjectModel pmodel = parent.ProjectModel;
                    foreach (FuncBlockViewModel fbvmodel in pmodel.FuncBlocks)
                    {
                        Find(fbvmodel);
                    }
                    break;
            }
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
        }

        private void Find(FuncBlockViewModel fbvmodel)
        {
            string text = fbvmodel.Code;
            string word = TB_Input.Text;
            Match match = Regex.Match(
                text,
                IsRegex ? @word : word,
                IgnoreCase ? RegexOptions.IgnoreCase : 0);
            while (match != null && match.Success)
            {
                items.Add(new TextReplaceElement(fbvmodel, match.Index, match.Value));
                match = match.NextMatch();
            }
        }

        private void Replace()
        {
            FuncBlockReplaceWordCommand_Group commandall
                = new FuncBlockReplaceWordCommand_Group(
                    this, items.ToArray());
            foreach (TextReplaceElement element in DG_List.SelectedItems)
            {
                commandall.Add(
                    new FuncBlockReplaceWordCommand(
                        element, TB_Change.Text));
            }
            _cmdmanager.Execute(commandall);
        }

        #region Event Handler

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
        private void TB_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            TB_Input.Background = TB_Input.Text.Length > 0
                ? Brushes.LightGreen
                : Brushes.White;
        }
        
        private void TB_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            Find();
            TB_Input.Background = Brushes.White;
        }

        private void TB_Change_TextChanged(object sender, TextChangedEventArgs e)
        {
            TB_Change.Background = TB_Change.Text.Length > 0
                ? Brushes.LightGreen
                : Brushes.White;
        }

        private void TB_Change_KeyDown(object sender, KeyEventArgs e)
        {
            if (called_DataGridCell_KeyDown)
            {
                called_DataGridCell_KeyDown = false;
                return;
            }
            if (e.Key != Key.Enter) return;
            TB_Change.IsEnabled = false;
            Replace();
            TB_Change.Background = Brushes.White;
            TB_Change.IsEnabled = true;
        }
        
        private bool called_DataGridCell_KeyDown = false;
        private void DataGridCell_KeyDown(object sender, KeyEventArgs e)
        {
            called_DataGridCell_KeyDown = true;
            if (e.Key != Key.Enter) return;
            TB_Change.IsEnabled = false;
            Replace();
            TB_Change.Background = Brushes.White;
            TB_Change.IsEnabled = true;
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TB_Change.IsEnabled = false;
            Replace();
            TB_Change.Background = Brushes.White;
            TB_Change.IsEnabled = true;
        }

        private void DG_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!parent.MainWindow.LAReplace.IsFloat
             && !parent.MainWindow.LAReplace.IsDock)
            {
                LayoutSetting.AddDefaultDockWidthAnchorable(
                    "替换", parent.MainWindow.LAReplace.AutoHideWidth.ToString());
                LayoutSetting.AddDefaultDockHeighAnchorable(
                    "替换", parent.MainWindow.LAReplace.AutoHideHeight.ToString());
                parent.MainWindow.LAReplace.ToggleAutoHide();
            }
            if (DG_List.SelectedIndex < 0) return;

        }

        private void UndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _cmdmanager.CanUndo;
        }

        private void UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _cmdmanager.Undo();
        }

        private void RedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _cmdmanager.CanRedo;
        }

        private void RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _cmdmanager.Redo();
        }

        private void OnConfigClick(object sender, RoutedEventArgs e)
        {
            G_Main.Visibility = Visibility.Hidden;
            G_Config.Visibility = Visibility.Visible;
        }

        private void BC_Ensure_Click(object sender, RoutedEventArgs e)
        {
            G_Main.Visibility = Visibility.Visible;
            G_Config.Visibility = Visibility.Hidden;
        }

        private void BC_Cancel_Click(object sender, RoutedEventArgs e)
        {
            G_Main.Visibility = Visibility.Visible;
            G_Config.Visibility = Visibility.Hidden;
        }

        #endregion
    }

    public class TextReplaceElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private FuncBlockViewModel fbvmodel;
        private string line;
        private string word;
        private int lineoffset;
        private int textoffset;
        private int textrow;
        private int textcolumn;
        
        public string Line
        {
            get
            {
                return line;
            }
        }
        public string Word
        {
            get { return word; }
        }
        public string FuncBlock
        {
            get
            {
                return fbvmodel.ProgramName;
            }
        }
        public string Position
        {
            get
            {
                return String.Format("({0}, {1})",
                    textrow, textcolumn);
            }
        }

        public TextReplaceElement
        (
            FuncBlockViewModel  _fbvmodel,
            int                 _textoffset,
            string              _word
        )
        {
            fbvmodel = _fbvmodel;
            textoffset = _textoffset;
            word = _word;
            TextViewPosition? tvpos = fbvmodel.GetPosition(textoffset);
            if (tvpos.HasValue)
            {
                textrow = tvpos.Value.Line;
                textcolumn = tvpos.Value.Column;
            }
            string text = fbvmodel.Code;
            int start = textoffset;
            int end = textoffset;
            while (start >= 0 && text[start] != '\n') start--;
            while (end < text.Length && text[end] != '\n') end++;
            line = text.Substring(start + 1, end - start - 1);
            lineoffset = textoffset - start - 1;
        }

        public void Replace(string newword)
        {
            fbvmodel.Code = fbvmodel.Code
                .Remove(textoffset, word.Length)
                .Insert(textoffset, newword);
            line = line
                .Remove(lineoffset, word.Length)
                .Insert(lineoffset, newword);
            word = newword;
            PropertyChanged(this, new PropertyChangedEventArgs("Line"));
        }
    }

    public class FuncBlockReplaceWordCommand : IUndoableCommand
    {
        private TextReplaceElement tpelement;
        private string oldword;
        private string newword;

        public TextReplaceElement Element
        {
            get { return tpelement; }
        }

        public FuncBlockReplaceWordCommand
        (
            TextReplaceElement  _tpelement,
            string              _newword
        )
        {
            tpelement = _tpelement;
            oldword = tpelement.Word;
            newword = _newword;
        }

        public void Execute()
        {
            tpelement.Replace(newword);
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            tpelement.Replace(oldword);
        }
    }

    public class FuncBlockReplaceWordCommand_Group : IUndoableCommand
    {
        private List<IUndoableCommand> items
            = new List<IUndoableCommand>();
        private TextReplaceWindow parent;
        private IEnumerable<TextReplaceElement> eles_all;
        private List<TextReplaceElement> eles_replaced
            = new List<TextReplaceElement>();

        public FuncBlockReplaceWordCommand_Group
        (
            TextReplaceWindow tpwindow,
            IEnumerable<TextReplaceElement> tpelements
        )
        {
            parent = tpwindow;
            eles_all = tpelements;
        }

        public void Add(FuncBlockReplaceWordCommand cmd)
        {
            items.Add(cmd);
            eles_replaced.Add(cmd.Element);
        }

        public void Execute()
        {
            foreach (IUndoableCommand cmd in items)
            {
                cmd.Execute();
            }
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            foreach (IUndoableCommand cmd in items)
            {
                cmd.Undo();
            }
        }
    }
}

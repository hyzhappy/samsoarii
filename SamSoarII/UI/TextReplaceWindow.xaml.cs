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
        static string[] SPECIALLABELS = { "\\", ".", "^", "$", "(", ")", "[", "]", "{", "}" };

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

        public TextReplaceWindow(InteractionFacade _parent)
        {
            InitializeComponent();
            DataContext = this;
            parent = _parent;
            parent.CurrentTabChanged += OnCurrentTabChanged;
            Mode = MODE_CURRENT;
            //TB_Input.Background = Brushes.Red;
        }

        public void Initialize()
        {
            items.Clear();
            _cmdmanager.Initialize();
        }

        public void Find(string word = null)
        {
            if (word != null)
                TB_Input.Text = word;
            items.Clear();
            if (TB_Input.Text.Length <= 0)
                return;
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
            RegexOptions opt = RegexOptions.None;
            opt |= RegexOptions.Singleline;
            if (IgnoreCase)
                opt |= RegexOptions.IgnoreCase;
            if (!IsRegex)
            {
                foreach (string slabel in SPECIALLABELS)
                {
                    word = word.Replace(slabel, "\\" + slabel);
                }
            }
            Match match = Regex.Match(text, word, opt);
            while (match != null && match.Success)
            {
                items.Add(new TextReplaceElement(this, fbvmodel, match.Index, match.Value));
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
            TextFindElement element = null;
            if (e.AddedItems.Count > 0)
                element = (TextFindElement)(e.AddedItems[0]);
            else if (e.RemovedItems.Count > 0)
                element = (TextFindElement)(e.RemovedItems[0]);
            else
                return;
            FuncBlockViewModel fbvmodel = element.FBVModel;
            int offset = element.Offset;
            int count = element.Word.Length;
            parent.NavigateToFuncBlock(fbvmodel, offset);
            fbvmodel.SetOffset(offset, count);
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
        
        private void OnCurrentTabChanged(object sender, SelectionChangedEventArgs e)
        {
            ITabItem currenttab = parent.MainTabControl.CurrentTab;
            if (currenttab is FuncBlockViewModel)
            {
                Visibility = Visibility.Visible;
                if (Mode == MODE_CURRENT) Find();
            }
            else
            {
                Visibility = Visibility.Hidden;
            }
        }
        
        #region Config

        private bool _isregex;
        private bool _ignorecase;

        private void OnConfigClick(object sender, RoutedEventArgs e)
        {
            G_Main.Visibility = Visibility.Hidden;
            G_Config.Visibility = Visibility.Visible;
            _isregex = IsRegex;
            _ignorecase = IgnoreCase;
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
            IsRegex = _isregex;
            IgnoreCase = _ignorecase;
        }

        #endregion

        #endregion
    }

    public class TextReplaceElement : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private TextReplaceWindow tpwindow;
        private FuncBlockViewModel fbvmodel;
        private string line;
        private string word;
        private int lineoffset;
        private int textoffset;
        private int textrow;
        private int textcolumn;

        public FuncBlockViewModel FBVModel
        {
            get
            {
                return this.fbvmodel;
            }
        }
        public int Offset
        {
            get
            {
                return this.textoffset;
            }
            set
            {
                this.textoffset = value;
                if (fbvmodel != null)
                {
                    TextViewPosition? tvpos = fbvmodel.GetPosition(textoffset);
                    if (tvpos.HasValue)
                    {
                        textrow = tvpos.Value.Line;
                        textcolumn = tvpos.Value.Column;
                        PropertyChanged(this, new PropertyChangedEventArgs("Position"));
                    }
                }
            }
        }
        public string Line
        {
            get
            {
                return this.line;
            }
            set
            {
                this.line = value;
            }
        }
        public int LineOffset
        {
            get
            {
                return this.lineoffset;
            }
            set
            {
                this.lineoffset = value;
            }
        }
        public string Word
        {
            get { return word; }
        }
        public string Profix
        {
            get
            {
                return line.Substring(0, lineoffset);
            }
        }
        public string Suffix
        {
            get
            {
                return line.Substring(lineoffset + word.Length);
            }
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
            TextReplaceWindow   _tpwindow,
            FuncBlockViewModel  _fbvmodel,
            int                 _textoffset,
            string              _word
        )
        {
            tpwindow = _tpwindow;
            fbvmodel = _fbvmodel;
            fbvmodel.TextChanged += OnTextChanged;    
            Offset = _textoffset;
            word = _word;
            string text = fbvmodel.Code;
            int start = textoffset - 1;
            int end = textoffset + word.Length;
            while (start >= 0 && text[start] != '\n') start--;
            while (end < text.Length && text[end] != '\n') end++;
            Line = text.Substring(start + 1, end - start - 1);
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
            PropertyChanged(this, new PropertyChangedEventArgs("Word"));
        }

        protected virtual void OnTextChanged(object sender, RoutedEventArgs e)
        {
            tpwindow.Initialize();
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
        public string NewWord
        {
            get { return this.newword; }
        }
        public string OldWord
        {
            get { return this.oldword; }
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
        private List<FuncBlockReplaceWordCommand> items
            = new List<FuncBlockReplaceWordCommand>();
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
            items.Sort((cmd1, cmd2) =>
            {
                return cmd1.Element.Offset.CompareTo(cmd2.Element.Offset);
            });
            int offset = 0;
            string word = null;
            foreach (FuncBlockReplaceWordCommand cmd in items)
            {
                cmd.Element.Offset += offset;
                cmd.Execute();
                offset += cmd.NewWord.Length - cmd.OldWord.Length;
                word = cmd.OldWord;
            }
            parent.Find(word);
        }

        public void Redo()
        {
            Execute();
        }

        public void Undo()
        {
            int offset = 0;
            string word = null;
            foreach (FuncBlockReplaceWordCommand cmd in items)
            {
                cmd.Element.Offset -= offset;
                cmd.Undo();
                offset += cmd.NewWord.Length - cmd.OldWord.Length;
                word = cmd.OldWord;
            }
            parent.Find(word);
        }
    }
}

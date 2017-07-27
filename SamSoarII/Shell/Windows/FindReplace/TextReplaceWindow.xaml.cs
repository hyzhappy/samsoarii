using ICSharpCode.AvalonEdit;
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
using SamSoarII.Core.Models;
using SamSoarII.Shell.Models;

namespace SamSoarII.Shell.Windows
{
    /// <summary>
    /// TextReplaceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TextReplaceWindow : UserControl, INotifyPropertyChanged, IWindow
    {
        static string[] SPECIALLABELS = { "\\", ".", "^", "$", "(", ")", "[", "]", "{", "}" };

        #region Numbers

        private InteractionFacade ifParent;

        public InteractionFacade IFParent { get { return this.ifParent; } }
        
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
                Initialize(false);
                foreach (TextReplaceElement item in value)
                    items.Add(item);
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

        public TextReplaceWindow(InteractionFacade _ifParent)
        {
            InitializeComponent();
            DataContext = this;
            ifParent = _ifParent;
            ifParent.PostIWindowEvent += OnReceiveIWindowEvent;
            Mode = MODE_CURRENT;
            undos = new Stack<TextReplaceCommandGroup>();
            redos = new Stack<TextReplaceCommandGroup>();
        }

        public void Initialize(bool initcmd = true)
        {
            foreach (TextReplaceElement ele in items)
                ele.Dispose();
            items.Clear();
            if (initcmd)
            {
                UndoClear();
                RedoClear();
            }
        }

        public void Find(string word = null)
        {
            if (word != null) TB_Input.Text = word;
            Initialize(false);
            if (TB_Input.Text.Length <= 0) return;
            switch (Mode)
            {
                // 当前文本
                case MODE_CURRENT:
                    if (ifParent.CurrentFuncBlock != null)
                        Find(ifParent.CurrentFuncBlock);
                    break;
                // 所有文本
                case MODE_ALL:
                    if (ifParent.MDProj == null) break;
                    foreach (FuncBlockModel fbmodel in ifParent.MDProj.FuncBlocks)
                    {
                        if (fbmodel.View == null)
                            fbmodel.View = new FuncBlockViewModel(fbmodel, ifParent.TCMain);
                        Find(fbmodel.View);
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
                foreach (string slabel in SPECIALLABELS)
                    word = word.Replace(slabel, "\\" + slabel);
            Match match = Regex.Match(text, word, opt);
            while (match != null && match.Success)
            {
                items.Add(new TextReplaceElement(this, fbvmodel, match.Index, match.Value));
                match = match.NextMatch();
            }
        }

        private void Replace()
        {
            if (DG_List.SelectedItems == null || DG_List.SelectedItems.Count == 0) return;
            TextReplaceCommandGroup cmd = new TextReplaceCommandGroup();
            foreach (FuncBlockModel fbmodel in ifParent.MDProj.FuncBlocks)
            {
                if (fbmodel.IsLibrary) continue;
                IEnumerable<TextReplaceElement> eles = DG_List.SelectedItems.Cast<TextReplaceElement>().Where(
                    ele => ele.FBVModel == fbmodel.View);
                if (eles.Count() == 0) continue;
                List<TextReplaceElement> elelist = eles.ToList();
                elelist.Sort((e1, e2) => { return e1.Offset.CompareTo(e2.Offset); });
                int start = elelist.First().Offset;
                int end = elelist.Last().Offset + elelist.Last().Word.Length;
                string oldtext = fbmodel.View.Code.Substring(start, end - start);
                StringBuilder newtext = new StringBuilder();
                for (int i = 0; i < elelist.Count(); i++)
                {
                    int pstart = (i == 0 ? 0 : elelist[i - 1].Offset + elelist[i - 1].Word.Length - start);
                    int pend = elelist[i].Offset - start;
                    if (pend > pstart) newtext.Append(oldtext.Substring(pstart, pend - pstart));
                    newtext.Append(TB_Change.Text);
                }
                cmd.Add(new TextReplaceCommand(fbmodel.View, start, oldtext, newtext.ToString()));
            }
            cmd.Redo();
            RedoClear();
            undos.Push(cmd);
            Find();
        }

        #region Event Handler

        public event IWindowEventHandler Post = delegate { };

        private void OnReceiveIWindowEvent(IWindow sender, IWindowEventArgs e)
        {
            // TAB界面发过来的事件
            if (sender is MainTabControl && e is MainTabControlEventArgs)
            {
                MainTabControlEventArgs e1 = (MainTabControlEventArgs)e;
                if (e1.Action == TabAction.SELECT)
                {
                    // 当前界面是函数块时进行重查
                    if (e1.Tab is FuncBlockViewModel)
                    {
                        Visibility = Visibility.Visible;
                        if (Mode == MODE_CURRENT) Find();
                    }
                    // 否则隐藏窗口
                    else
                    {
                        Visibility = Visibility.Hidden;
                    }
                }
            }
        }

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
            if (!ifParent.WNDMain.LAFind.IsFloat
             && !ifParent.WNDMain.LAFind.IsDock)
            {
                LayoutSetting.AddDefaultDockWidthAnchorable(
                    Properties.Resources.MainWindow_Search, ifParent.WNDMain.LAFind.AutoHideWidth.ToString());
                LayoutSetting.AddDefaultDockHeighAnchorable(
                    Properties.Resources.MainWindow_Search, ifParent.WNDMain.LAFind.AutoHideHeight.ToString());
                ifParent.WNDMain.LAFind.ToggleAutoHide();
            }
            if (DG_List.SelectedIndex < 0) return;
            /*
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
            ifParent.NavigateToFuncBlock(fbvmodel, offset);
            fbvmodel.SetOffset(offset, count);
            */
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

        #region Commands

        private Stack<TextReplaceCommandGroup> undos;
        private Stack<TextReplaceCommandGroup> redos;

        private void UndoClear()
        {
            foreach (TextReplaceCommandGroup cmd in undos) cmd.Dispose();
            undos.Clear();
        }
        
        private void UndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = undos != null && undos.Count() > 0;
        }

        private void UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TextReplaceCommandGroup cmd = undos.Pop();
            cmd.Undo();
            redos.Push(cmd);
            Find();
        }

        private void RedoClear()
        {
            foreach (TextReplaceCommandGroup cmd in redos) cmd.Dispose();
            redos.Clear();
        }

        private void RedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = redos != null && redos.Count() > 0;
        }

        private void RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TextReplaceCommandGroup cmd = redos.Pop();
            cmd.Redo();
            undos.Push(cmd);
            Find();
        }
        
        #endregion
    }

    public class TextReplaceElement : INotifyPropertyChanged, IDisposable
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
                return fbvmodel.TabHeader;
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
            TextReplaceWindow _tpwindow,
            FuncBlockViewModel _fbvmodel,
            int _textoffset,
            string _word
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
            while (start >= 0 && text[start] != '\n' && text[start] != '\r') start--;
            while (end < text.Length && text[end] != '\n' && text[end] != '\r') end++;
            Line = text.Substring(start + 1, end - start - 1);
            lineoffset = textoffset - start - 1;
        }

        public void Dispose()
        {
            tpwindow = null;
            fbvmodel.TextChanged -= OnTextChanged;
            fbvmodel = null;
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

    public class TextReplaceCommand : IDisposable
    {
        public TextReplaceCommand(FuncBlockViewModel _fbvmodel, int _offset, string _oldtext, string _newtext)
        {
            fbvmodel = _fbvmodel;
            offset = _offset;
            oldtext = _oldtext;
            newtext = _newtext;
        }

        public void Dispose()
        {
            fbvmodel = null;
        }

        #region Number

        private FuncBlockViewModel fbvmodel;
        private int offset;
        private string oldtext;
        private string newtext;

        #endregion

        #region Undo & Redo
    
        public void Undo()
        {
            fbvmodel.Code = fbvmodel.Code.Substring(0, offset) + oldtext + fbvmodel.Code.Substring(offset + newtext.Length);
        }

        public void Redo()
        {
            fbvmodel.Code = fbvmodel.Code.Substring(0, offset) + newtext + fbvmodel.Code.Substring(offset + oldtext.Length);
        }

        #endregion

    }

    public class TextReplaceCommandGroup : IDisposable
    {
        public TextReplaceCommandGroup()
        {
            commands = new List<TextReplaceCommand>();
        }

        public void Dispose()
        {
            foreach (TextReplaceCommand cmd in commands) cmd.Dispose();
            commands = null;
        }

        private List<TextReplaceCommand> commands;

        public void Add(TextReplaceCommand cmd)
        {
            commands.Add(cmd);
        }

        public void Undo()
        {
            foreach (TextReplaceCommand cmd in commands) cmd.Undo();
        }

        public void Redo()
        {
            foreach (TextReplaceCommand cmd in commands) cmd.Redo();
        }

    }
}

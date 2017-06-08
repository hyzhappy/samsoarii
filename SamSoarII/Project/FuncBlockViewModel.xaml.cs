
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
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
using System.Windows.Threading;
using System.Xml;
using SamSoarII.Extend.FuncBlockModel;
using ICSharpCode.AvalonEdit.Document;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit;
using SamSoarII.LadderInstViewModel;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Controls;

/// <summary>
/// Namespace : SamSoarII.Simulation
/// ClassName : SimulateModel
/// Version   : 2.0
/// Date      : 2017/4/11
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 显示用户自定义的函数功能块的窗口
/// 支持修改功能，自带缩进和补全等Feature
/// </remarks>

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// FuncBlockViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class FuncBlockViewModel : UserControl, IProgram
    {
        #region Numbers

        /// <summary>
        /// 父类
        /// </summary>
        private ProjectModel parent;
        /// <summary>
        /// 函数块的逻辑模型
        /// </summary>
        private FuncBlockModel model;
        /// <summary>
        /// 工程的名称
        /// </summary>
        private string _programName;
        /// <summary>
        /// 工程的名称
        /// </summary>
        public string ProgramName
        {
            get
            {
                return _programName;
            }
            set
            {
                _programName = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ProgramName"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TabHeader"));
            }
        }
        /// <summary>
        /// 函数块的名称
        /// </summary>
        public string FuncBlockName { get; set; }
        /// <summary>
        /// Avalon库自带的函数补全窗口，已弃用
        /// </summary>
        private CompletionWindow completionWindow;
        /// <summary>
        /// 代码高亮的管理器
        /// </summary>
        private FoldingManager foldingManager;
        /// <summary>
        /// 代码高亮的策略
        /// </summary>
        private AbstractFoldingStrategy foldingStrategy;
        /// <summary>
        /// 窗口名称
        /// </summary>
        public string TabHeader
        {
            get
            {
                return _programName;
            }
            set
            {
                _programName = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TabHeader"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ProgramName"));
            }
        }
        /// <summary>
        /// 函数代码
        /// </summary>
        public string Code
        {
            get
            {
                return CodeTextBox.Text;
            }
            set
            {
                CodeTextBox.Text = value;
                model = new FuncBlockModel(value);
            }
        }
        /// <summary>
        /// 所有函数的模型
        /// </summary>
        public IEnumerable<FuncModel> Funcs
        {
            get
            {
                List<FuncModel> result = new List<FuncModel>();
                foreach (FuncBlock fb in model.Root.Childrens)
                {
                    if (fb is FuncBlock_FuncHeader)
                    {
                        FuncBlock_FuncHeader fbfh = (FuncBlock_FuncHeader)(fb);
                        result.Add(fbfh.Model);
                    }
                }
                return result;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return CodeTextBox.IsReadOnly;
            }
            set
            {
                CodeTextBox.IsReadOnly = value;
            }
        }
        
        #region Floating

        public bool IsFloat { get; set; }
        public LayoutFloatingWindow FloatWindow { get; set; }
        private LayoutFloatingWindowControl floatcontrol;
        public LayoutFloatingWindowControl FloatControl
        {
            get { return this.floatcontrol; }
            set
            {
                this.floatcontrol = value;
                floatcontrol.Closed += OnFloatClosed;
            }
        }
        public event RoutedEventHandler FloatClosed = delegate { };
        private void OnFloatClosed(object sender, EventArgs e)
        {
            FloatClosed(this, new RoutedEventArgs());
        }

        #endregion

        /// <summary>
        /// 控件的实际宽度
        /// </summary>
        protected double _actualWidth;
        /// <summary>
        /// 控件的实际高度
        /// </summary>
        protected double _actualHeight;
        /// <summary>
        /// 控件的实际宽度
        /// </summary>
        double ITabItem.ActualWidth
        {
            get
            {
                return this._actualWidth;
            }

            set
            {
                this._actualWidth = value;
            }
        }
        /// <summary>
        /// 控件的实际高度
        /// </summary>
        double ITabItem.ActualHeight
        {
            get
            {
                return this._actualHeight;
            }

            set
            {
                this._actualHeight = value;
            }
        }
        
        #endregion

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="name">函数块名称</param>
        public FuncBlockViewModel(string name, ProjectModel _pmodel)
        {
            InitializeComponent();
            parent = _pmodel;
            ProgramName = name;
            model = new FuncBlockModel(String.Empty);
            IHighlightingDefinition customHighlighting;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine(assembly.GetManifestResourceNames());
            using (Stream s = assembly.GetManifestResourceStream("SamSoarII.AppMain.Project.CustomHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, customHighlighting);
            FuncBlockName = name;
            //CodeTextBox.DataContext = this;
            CodeTextBox.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            CodeTextBox.TextArea.TextEntered += textEditer_TextArea_TextEntered;
            CodeTextBox.TextArea.CodeCompleteKeyDown += textEditer_TextArea_CodeCompleteKeyDown;
            CodeTextBox.TextArea.CodeCompleteKeyUp += textEditer_TextArea_CodeCompleteKeyUp;
            CodeTextBox.CaretChanged += textEditer_CaretChanged;
            CodeTextBox.DocumentChanged_Detail += textEditer_DocumentChanged;
            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(0.5);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();
            foldingManager = FoldingManager.Install(CodeTextBox.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, CodeTextBox.Document);
            CodeTextBox.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(CodeTextBox.Options);
            //completionWindow = new CompletionWindow(CodeTextBox.TextArea);
            completionWindow = null;
            CCSProfix = String.Empty;
            ccstblocks = new TextBlock[9];
            for (int i = 0; i < 9; i++)
            {
                ccstblocks[i] = new TextBlock();
                ccstblocks[i].FontFamily = new FontFamily("Consolas");
                ccstblocks[i].FontSize = 16;
                Grid.SetRow(ccstblocks[i], i + 1);
                Grid.SetColumn(ccstblocks[i], 0);
                CodeCompletePanel.Children.Add(ccstblocks[i]);
            }
        }
        #region TextEditer Events

        /// <summary>
        /// 当用户键入字符前发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (char.IsLetterOrDigit(e.Text[0]))
                {
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }
        /// <summary>
        /// 当代码内容改变时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textEditer_DocumentChanged(object sender, DocumentChangeEventArgs e)
        {
            int offset = e.InsertionLength - e.RemovalLength;
            model.Move(e.Offset);
            Regex localRegex = new Regex(@"[\{\}]");
            Regex stmtRegex = new Regex(@";");
            Regex blankRegex = new Regex(@"^\s*$");
            Match insertMatch1 = localRegex.Match(e.InsertedText);
            Match removeMatch1 = localRegex.Match(e.RemovedText);
            Match insertMatch2 = stmtRegex.Match(e.InsertedText);
            Match removeMatch2 = stmtRegex.Match(e.RemovedText);
            Match insertMatch3 = blankRegex.Match(e.InsertedText);
            Match removeMatch3 = blankRegex.Match(e.RemovedText);
            int start = 0;
            int end = 0;
            if (model.Current is FuncBlock_Comment)
            {
                FuncBlock _parent = model.Current.Parent;
                start = _parent.IndexStart;
                end = _parent.IndexEnd - 1;
                while (end < CodeTextBox.Text.Length && CodeTextBox.Text[end] != '\n') end++;
                _parent.Build(CodeTextBox.Text, start, end, offset);
            }
            else if (insertMatch3.Success && removeMatch3.Success)
            {
                model.Current.InnerOffset += offset;
            }
            else if (insertMatch1.Success || removeMatch1.Success)
            {
                model = new FuncBlockModel(CodeTextBox.Text);
                model.Move(e.Offset);
            }
            else
            {
                if (model.Current is FuncBlock_FuncHeader)
                {
                    model.CurrentNode = new LinkedListNode<FuncBlock>(model.Root);
                }
                if (model.Current is FuncBlock_Root)
                {
                    LinkedListNode<FuncBlock> nprev = null;
                    LinkedListNode<FuncBlock> nnext = null;
                    if (model.Current.Current != null)
                    {
                        nprev = model.Current.Current;
                        if (model.Current.Current.Next != null)
                        {
                            nnext = model.Current.Current.Next;
                        }
                    }
                    if (nprev != null &&
                        nprev.Value.IndexStart > e.Offset)
                    {
                        nnext = nprev;
                        nprev = nprev.Previous;
                    }
                    while (nprev != null
                        && !(nprev.Value is FuncBlock_Local))
                    {
                        nprev = nprev.Previous;
                    }
                    while (nnext != null
                        && !(nnext.Value is FuncBlock_Local))
                    {
                        nnext = nnext.Next;
                    }
                    start = nprev != null ? (nprev.Value.IndexEnd + 1) : (model.Current.IndexStart);
                    end = nnext != null ? (nnext.Value.IndexStart - 2) : (model.Current.IndexEnd - 1);
                    model.Root.Build(CodeTextBox.Text, start, end, offset);
                }
                else if (insertMatch2.Success || removeMatch2.Success)
                {
                    start = model.Current.IndexStart;
                    if (model.Current is FuncBlock_Local)
                    {
                        end = model.Current.IndexEnd;
                        model.Current.Build(CodeTextBox.Text, start, end - 1, offset);
                    }
                    else if (model.Current.Parent is FuncBlock_Local)
                    {
                        end = model.Current.Parent.IndexEnd;
                        model.Current.Parent.Build(CodeTextBox.Text, start, end - 1, offset);
                    }
                    else
                    {
                        throw new Exception(String.Format("Code Structure Error : {0:s} in {1:s}",
                            model.Current.ToString(), model.Current.Parent.ToString()));
                    }
                }
                else
                {
                    if (model.Current is FuncBlock_Local)
                    {
                        start = model.Current.IndexStart;
                        end = model.Current.IndexEnd - 1;
                        model.Current.Build(CodeTextBox.Text, start, end, offset);
                    }
                    else
                    {
                        start = model.Current.IndexStart;
                        end = model.Current.IndexEnd;
                        model.Current.Parent.Build(CodeTextBox.Text, start, end, offset);
                    }
                }
            }
            model.Move(e.Offset);
            if (e.InsertionLength == 1 && e.RemovalLength == 0)
            {
                if (char.IsLetterOrDigit(e.InsertedText[0]) || e.InsertedText[0] == '_')
                {
                    int wordend = CodeTextBox.CaretOffset - 1;
                    int wordstart = wordend;
                    while (wordstart >= 0 &&
                        (Char.IsLetterOrDigit(CodeTextBox.Text[wordstart]) || CodeTextBox.Text[wordstart] == '_'))
                    {
                        wordstart--;
                    }
                    while (wordend < CodeTextBox.Text.Length &&
                        (Char.IsLetterOrDigit(CodeTextBox.Text[wordend]) || CodeTextBox.Text[wordend] == '_'))
                    {
                        wordend++;
                    }
                    wordstart++;
                    wordend--;
                    CCSOffset = wordstart;
                    CCSProfix = CodeTextBox.Text.Substring(wordstart, wordend - wordstart + 1);
                    CCSProfixCursor = CodeTextBox.CaretOffset - wordstart;
                }
                else if (e.InsertedText[0] != '\n')
                {
                    CCSProfix = String.Empty;
                }
            }
            else if (e.InsertionLength == 0 && e.RemovalLength == 1)
            {
                if (CCSProfix.Length > 0 && CCSProfixCursor > 0)
                {
                    CCSProfix = CCSProfix.Remove(CCSProfixCursor - 1, 1);
                    CCSProfixCursor--;
                }
            }
            else
            {
                CCSProfix = String.Empty;
            }
            TextChanged(this, new RoutedEventArgs());
            //OutputDebug();
        }
        /// <summary>
        /// 当用户键入字符后发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private int oldTime1 = -1,oldTime2 = -1;
        void textEditer_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            int tabhigh = 0;
            int start, end;
            string inserttext = String.Empty;
            string currentline = String.Empty;
            if (e.Text.Length == 1)
            {
                if (model.Current is FuncBlock_Comment)
                {
                    return;
                }
                switch (e.Text[0])
                {
                    case '{':
                        tabhigh = Math.Max(0, model.Current.Height - 1);
                        inserttext = new string('\t', tabhigh);
                        inserttext = "\n" + inserttext + "\t\n" + inserttext + "}";
                        CodeTextBox.Text = CodeTextBox.Text.Insert(CodeTextBox.CaretOffset, inserttext);
                        model = new FuncBlockModel(CodeTextBox.Text);
                        CodeTextBox.CaretOffset += tabhigh + 2;
                        oldTime1 = e.Timestamp;
                        break;
                    case '(':
                        inserttext = ")";
                        CodeTextBox.Text = CodeTextBox.Text.Insert(CodeTextBox.CaretOffset, inserttext);
                        model.Current.InnerOffset += inserttext.Length;
                        oldTime2 = e.Timestamp;
                        break;
                    case ')':
                        if (oldTime2 > 0 && e.Timestamp - oldTime2 < 100)
                            CodeTextBox.Text = CodeTextBox.Text.Remove(CodeTextBox.CaretOffset-- - 1, 1);
                        break;
                    case '}':
                        if (oldTime1 > 0 && e.Timestamp - oldTime1 < 100)
                            CodeTextBox.Text = CodeTextBox.Text.Remove(CodeTextBox.CaretOffset-- - 1, 1);
                        break;
                    case ';':
                        break;
                    case '/':
                    case '*':
                        end = CodeTextBox.CaretOffset - 1;
                        start = end - 1;
                        if (start >= 0 && CodeTextBox.Text[start] == '/' && CodeTextBox.Text[end] == '/')
                        {
                            model = new FuncBlockModel(CodeTextBox.Text);
                            model.Move(CodeTextBox.CaretOffset);
                            break;
                        }
                        if (start >= 0 && CodeTextBox.Text[start] == '/' && CodeTextBox.Text[end] == '*')
                        {
                            tabhigh = model.Current.Height;
                            inserttext = new string('\t', tabhigh);
                            inserttext = "\n" + inserttext + " *\n" + inserttext + " */";
                            CodeTextBox.Text = CodeTextBox.Text.Insert(CodeTextBox.CaretOffset, inserttext);
                            CodeTextBox.CaretOffset += tabhigh + 3;
                            model = new FuncBlockModel(CodeTextBox.Text);
                            model.Move(CodeTextBox.CaretOffset);
                        }
                        break;
                    case '\n':
                        if (model.Current is FuncBlock_CommentParagraph)
                        {
                            tabhigh = model.Current.Height;
                            inserttext = new string('\t', tabhigh);
                            inserttext += " *";
                            CodeTextBox.Text = CodeTextBox.Text.Insert(CodeTextBox.CaretOffset, inserttext);
                            CodeTextBox.CaretOffset += inserttext.Length;
                        }
                        model.Move(CodeTextBox.CaretOffset);
                        break;
                    default:
                        break;
                }
            }
            //OutputDebug();
        }
        /// <summary>
        /// 当文本光标移动后发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        void textEditer_CaretChanged(object sender, RoutedEventArgs e)
        {
            model.Move(CodeTextBox.CaretOffset);
            if (CCSProfix.Length > 0)
            {
                CCSProfixCursor = CodeTextBox.CaretOffset - CCSOffset;
            }
            //OutputDebug();
        }
        #endregion
        
        public void SetPosition(int line, int column)
        {
            //CCSProfix = String.Empty;
            CodeTextBox.SetPosition(line, column);
            ScrollViewer sv = CodeTextBox.ScrollViewer;
            if (sv == null) return;
            double y = line * 19 - 25 - sv.ViewportHeight / 2;
            y = Math.Max(0, y);
            sv.ScrollToVerticalOffset(y);
            double x = column * 16 - sv.ViewportWidth / 2;
            x = Math.Max(0, x);
            sv.ScrollToHorizontalOffset(x);
        }

        public TextViewPosition? GetPosition(int offset = -1)
        {
            if (offset == -1)
            {
                offset = CodeTextBox.CaretOffset;
            }
            return CodeTextBox.GetPositionFromOffset(offset);
        }

        public void SetOffset(int offset, int count = 0)
        {
            //CCSProfix = String.Empty;
            if (count > 0)
                CodeTextBox.Select(offset, count);
            else
                CodeTextBox.CaretOffset = offset;
            int line = CodeTextBox.Row;
            int column = CodeTextBox.Column;
            ScrollViewer sv = CodeTextBox.ScrollViewer;
            if (sv == null) return;
            double y = line * 19 - 25 - sv.ViewportHeight / 2;
            y = Math.Max(0, y);
            sv.ScrollToVerticalOffset(y);
            double x = column * 16 - sv.ViewportWidth / 2;
            x = Math.Max(0, x);
            sv.ScrollToHorizontalOffset(x);
        }

        /// <summary>
        /// 代码高亮更新的触发时钟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, CodeTextBox.Document);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Code Completion

        #region Selected List
        /// <summary>
        /// 代码补全的词语列表
        /// </summary>
        private List<string> ccslist;
        /// <summary>
        /// 代码补全的窗口列表
        /// </summary>
        private TextBlock[] ccstblocks;
        /// <summary>
        /// 当前窗口视点
        /// </summary>
        private int ccsviewpoint;
        /// <summary>
        /// 当前窗口视点
        /// </summary>
        private int CCSViewPoint
        {
            get { return this.ccsviewpoint; }
            set
            {
                if (ccslist == null)
                    return;
                if (value < 0 || value > Math.Max(0, ccslist.Count() - 9))
                    return;
                this.ccsviewpoint = value;
                ScrollHeight = Math.Min(162, 162 * 9 / ccslist.Count());
                Canvas.SetTop(Scroll, (162 - ScrollHeight) * ccsviewpoint / (ccslist.Count() - 9));
                CCSUpdate();
            }
        }
        /// <summary>
        /// 是否已经开始选择词语
        /// </summary>
        private bool ccsisselected;
        /// <summary>
        /// 是否已经开始选择词语
        /// </summary>
        private bool CCSIsSelected
        {
            get { return this.ccsisselected; }
            set
            {
                this.ccsisselected = value;
                Color color = new Color();
                color.R = 0x80;
                color.G = 0x80;
                color.B = 0xF0;
                color.A = 0xFF;
                switch (value)
                {
                    case true:
                        Cursor.Background = new SolidColorBrush(color);
                        break;
                    case false:
                        Cursor.Background = Brushes.Transparent;
                        break;
                }
            }
        }
        /// <summary>
        /// 已输入的词语前缀
        /// </summary>
        private string ccsprofix;
        /// <summary>
        /// 已输入的词语前缀
        /// </summary>
        private string CCSProfix
        {
            get { return this.ccsprofix; }
            set
            {
                this.ccsprofix = value;
                Profix.Text = value;
                if (value.Equals(String.Empty))
                {
                    CodeCompletePanel.Visibility = Visibility.Collapsed;
                    CCSIsSelected = false;
                    CodeTextBox.TextArea.IsCodeCompleteMode = false;
                }
                else
                {
                    ccslist = model.GetCodeCompleteNames(value);
                    if (ccslist == null || ccslist.Count() == 0)
                    {
                        CCSProfix = String.Empty;
                        return;
                    }
                    CodeCompletePanel.Visibility = Visibility.Visible;
                    CCSViewPoint = 0;
                    CCSCursor = 0;
                    CCSIsSelected = false;
                    CodeTextBox.TextArea.IsCodeCompleteMode = true;
                    CCSUpdate();
                }
            }
        }
        /// <summary>
        /// 已输入的词语前缀的光标
        /// </summary>
        private int ccsprofixcursor;
        /// <summary>
        /// 已输入的词语前缀的光标
        /// </summary>
        private int CCSProfixCursor
        {
            get { return this.ccsprofixcursor; }
            set
            {
                if (value < 0 || value > CCSProfix.Length)
                {
                    CCSProfix = String.Empty;
                }
                else
                {
                    this.ccsprofixcursor = value;
                    ProfixCursor.X1 = value * 9 + 1;
                    ProfixCursor.X2 = ProfixCursor.X1;
                }
            }
        }
        /// <summary>
        /// 补全窗口的标号顶坐标
        /// </summary>
        private int ccstop;
        /// <summary>
        /// 补全窗口的标号顶坐标
        /// </summary>
        private int CCSTop
        {
            get { return this.ccstop; }
            set
            {
                this.ccstop = value;
                double top = ccstop * 19 - 17 - CodeTextBox.VerticalOffset;
                if (top + 200 > ActualHeight)
                {
                    top -= 220;
                }
                Canvas.SetTop(CodeCompletePanel, top);
            }
        }
        /// <summary>
        /// 补全窗口的标号左坐标
        /// </summary>
        private int ccsleft;
        /// <summary>
        /// 补全窗口的标号左坐标
        /// </summary>
        private int CCSLeft
        {
            get { return this.ccsleft; }
            set
            {
                this.ccsleft = value;
                Canvas.SetLeft(CodeCompletePanel, ccsleft * 10 - CodeTextBox.HorizontalOffset);
            }
        }
        /// <summary>
        /// 补全窗口对应的代码坐标
        /// </summary>
        private int ccsoffset;
        /// <summary>
        /// 补全窗口对应的代码坐标
        /// </summary>
        private int CCSOffset
        {
            get { return this.ccsoffset; }
            set
            {
                this.ccsoffset = value;
                TextDocument tdoc = CodeTextBox.TextArea.Document;
                TextLocation tloc = tdoc.GetLocation(value);
                CCSTop = tloc.Line + 1;
                CCSLeft = tloc.Column;
            }
        }
        /// <summary>
        /// 更新补全窗口
        /// </summary>
        private void CCSUpdate()
        {
            for (int i = 0; i < ccstblocks.Length; i++)
            {
                if (i + CCSViewPoint >= ccslist.Count())
                {
                    ccstblocks[i].Text = String.Empty;
                }
                else
                {
                    ccstblocks[i].Text = ccslist[i + CCSViewPoint];
                }
            }
        }
        #endregion

        #region Selected Cursor
        /// <summary>
        /// 选择词语的光标
        /// </summary>
        public int CCSCursor
        {
            get
            {
                return Grid.GetRow(Cursor) - 1;
            }
            set
            {
                if (value >= 0 && value < 9 && ccstblocks[value].Text.Length > 0)
                {
                    Grid.SetRow(Cursor, value + 1);
                }
                else if (value < 0)
                {
                    CCSViewPoint += value;
                    Grid.SetRow(Cursor, 1);
                }
                else if (value >= 9)
                {
                    CCSViewPoint += value - 8;
                    Grid.SetRow(Cursor, 9);
                }
            }
        }
        /// <summary>
        /// 当用户对补全窗口控制按键松开时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void textEditer_TextArea_CodeCompleteKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (CCSProfix.Length > 0)
                    {
                        int plen = CCSProfix.Length;
                        string inserttext = ccstblocks[CCSCursor].Text;
                        inserttext = inserttext.Substring(plen);
                        CodeTextBox.CaretOffset += inserttext.Length;
                        CCSProfix = String.Empty;
                    }
                    break;
            }
        }
        /// <summary>
        /// 当用户对补全窗口控制按键按下时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void textEditer_TextArea_CodeCompleteKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    if (CCSIsSelected)
                    {
                        int plen = CCSProfix.Length;
                        string inserttext = ccstblocks[CCSCursor].Text;
                        inserttext = inserttext.Substring(plen);
                        CodeTextBox.Text = CodeTextBox.Text.Insert(CodeTextBox.CaretOffset + (plen - CCSProfixCursor), inserttext);
                        model.Current.InnerOffset += inserttext.Length;
                        //CCSProfix = String.Empty;
                        //CodeTextBox.CaretOffset += inserttext.Length;
                    }
                    else
                    {
                        CCSProfix = String.Empty;
                    }
                    break;
                case Key.Up:
                    if (!CCSIsSelected)
                        CCSIsSelected = true;
                    else
                        CCSCursor -= 1;
                    break;
                case Key.Down:
                    if (!CCSIsSelected)
                        CCSIsSelected = true;
                    else
                        CCSCursor += 1;
                    break;
                /*
                case Key.Left:
                case Key.Back:
                    CCSProfixCursor -= 1;
                    break;
                case Key.Right:
                    CCSProfixCursor += 1;
                    break;
                */
            }
        }
        /// <summary>
        /// 当用户在补全窗口内进行鼠标移动时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void CodeCompletePanel_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(CodeCompletePanel);
            int id = (int)((p.Y - 20) / 18);
            if (p.X >= 0 && p.X < 188 && id >= 0 && id < 9)
            {
                CCSIsSelected = true;
                CCSCursor = id;
            }
        }
        /// <summary>
        /// 当用户点击补全窗口时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void CodeCompletePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(CodeCompletePanel);
            int id = (int)((p.Y - 20) / 18);
            if (p.X >= 0 && p.X < 188 && id >= 0 && id < 9)
            {
                int plen = CCSProfix.Length;
                string inserttext = ccstblocks[CCSCursor].Text;
                inserttext = inserttext.Substring(plen);
                CodeTextBox.Text = CodeTextBox.Text.Insert(CodeTextBox.CaretOffset, inserttext);
                model.Current.InnerOffset += inserttext.Length;
                CodeTextBox.CaretOffset += inserttext.Length;
                CCSProfix = String.Empty;
            }
        }
        #endregion

        #region Scroll
        /// <summary>
        /// 是否已经鼠标左键按下滚动条
        /// </summary>
        private bool scroll_ispressed;
        /// <summary>
        /// 鼠标相对于滚动条的X坐标
        /// </summary>
        private double scroll_x;
        /// <summary>
        /// 鼠标相对于滚动条的Y坐标
        /// </summary>
        private double scroll_y;
        /// <summary>
        /// 滚动条的高度
        /// </summary>
        private double scroll_height;
        /// <summary>
        /// 滚动条的高度
        /// </summary>
        public double ScrollHeight
        {
            get { return scroll_height; }
            set
            {
                scroll_height = value;
                Scroll.Height = value;
            }
        }
        /// <summary>
        /// 鼠标进入滚动条内发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void Scroll_MouseLeave(object sender, MouseEventArgs e)
        {
            Scroll.Opacity = 0.6;
        }
        /// <summary>
        /// 鼠标离开滚动条内发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void Scroll_MouseEnter(object sender, MouseEventArgs e)
        {
            Scroll.Opacity = 1.0;
        }
        /// <summary>
        /// 鼠标左键按下滚动条时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void Scroll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            scroll_ispressed = true;
            Point p = e.GetPosition(this);
            scroll_x = p.X;
            scroll_y = p.Y;
        }
        /// <summary>
        /// 鼠标左键松开滚动条时发生
        /// </summary>
        /// <param name="sender">发送源</param>
        /// <param name="e">事件</param>
        private void Scroll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            scroll_ispressed = false;
        }
        /// <summary>
        /// 鼠标移动时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scroll_MouseMove(object sender, MouseEventArgs e)
        {
            if (scroll_ispressed)
            {
                if (ccslist.Count() <= 9)
                    return;
                double step = (162 - ScrollHeight) / (ccslist.Count() - 9);
                Point p = e.GetPosition(this);
                while (scroll_y + step < p.Y && CCSViewPoint < ccslist.Count() - 9)
                {
                    scroll_y += step;
                    CCSViewPoint += 1;
                }
                while (scroll_y - step > p.Y && CCSViewPoint > 0)
                {
                    scroll_y -= step;
                    CCSViewPoint -= 1;
                }
            }
        }
        #endregion

        #endregion

        #region Event Handler

        public event RoutedEventHandler TextChanged = delegate { };

        private void FindCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ReplaceCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnFindCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            parent.IFacade.MainWindow.LACFind.Show();
        }

        private void OnReplaceCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            parent.IFacade.MainWindow.LACReplace.Show();
        }
        
        private void OnSelectAllCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CodeTextBox.SelectAll();
        }

        #endregion

    }
}

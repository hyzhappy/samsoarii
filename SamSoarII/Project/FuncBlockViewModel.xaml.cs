
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

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// FuncBlockViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class FuncBlockViewModel : UserControl, IProgram
    {
        private FuncBlockModel model;

        private string _programName;
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
        public string FuncBlockName { get; set; }
        private CompletionWindow completionWindow;
        private FoldingManager foldingManager;
        private AbstractFoldingStrategy foldingStrategy;
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

        protected double _actualWidth;
        protected double _actualHeight;

        double ITabItem.ActualWidth
        {
            get
            {
                return this._actualWidth;
            }

            set
            {
                this._actualWidth = value;
                CodeTextBox.Width = value - 10;
            }
        }

        double ITabItem.ActualHeight
        {
            get
            {
                return this._actualHeight;
            }

            set
            {
                this._actualHeight = value;
                CodeTextBox.Height = value - 32;
            }
        }

        public FuncBlockViewModel(string name)
        {
            InitializeComponent();
            ProgramName = name;
            InitializeComponent();
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
            CodeTextBox.DataContext = this;
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
        
        private void textEditer_DocumentChanged(object sender, DocumentChangeEventArgs e)
        {
            if ((e.InsertionLength == 1 || e.InsertionLength == 2 && e.InsertedText[0] >= 128) && e.RemovalLength == 0)
            {
                model.Current.InnerOffset += e.InsertionLength;
            }
            else if ((e.RemovalLength < model.Current.IndexEnd - model.Current.IndexStart + 1) && e.InsertionLength == 0)
            {
                model.Current.InnerOffset -= e.RemovalLength;
                if (model.Current is FuncBlock_Assignment)
                {
                    string statement = CodeTextBox.Text.Substring(model.Current.IndexStart, model.Current.IndexEnd - model.Current.IndexStart + 1);
                    if (FuncBlock_Assignment.TextSuit(statement))
                    {
                        ((FuncBlock_Assignment)(model.Current)).AnalyzeText(statement);
                    }
                    else
                    {
                        ((FuncBlock_Assignment)(model.Current)).Name = String.Empty;
                    }
                }
                if (CCSProfix.Length > 0)
                {
                    CCSProfix = CCSProfix.Remove(CCSProfix.Length - 1);
                }
            }
            else
            {
                Match alltab = Regex.Match(e.InsertedText, @"^\t+$");
                if (alltab.Success && e.RemovalLength == 0)
                {
                    model.Current.InnerOffset += e.InsertionLength;
                }
                else
                {
                    model = new FuncBlockModel(CodeTextBox.Text);
                }
            }
        }
        
        void textEditer_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            int tabhigh = 0;
            int start, end;
            string inserttext = String.Empty;
            string currentline = String.Empty;
            if (e.Text.Length < 2)
            {
                if (Char.IsLetterOrDigit(e.Text[0]))
                {
                    CCSProfix += e.Text[0];
                }
                switch (e.Text[0])
                {
                    case '{':
                        tabhigh = model.Current.Height;
                        inserttext = new string('\t', tabhigh);
                        inserttext = "\n" + inserttext + "\t\n" + inserttext + "}";
                        start = CodeTextBox.CaretOffset - 1;
                        end = start + inserttext.Length;
                        CodeTextBox.Text = CodeTextBox.Text.Insert(CodeTextBox.CaretOffset, inserttext);
                        model.Current.InnerOffset += inserttext.Length;
                        model.Look(CodeTextBox.Text, start, end);
                        CodeTextBox.CaretOffset += tabhigh + 2;
                        break;
                    case '(':
                        inserttext = ")";
                        CodeTextBox.Text = CodeTextBox.Text.Insert(CodeTextBox.CaretOffset, inserttext);
                        model.Current.InnerOffset += inserttext.Length;
                        break;
                    case ')':
                        break;
                    case '}':
                        break;
                    case ';':
                        end = CodeTextBox.CaretOffset - 1;
                        start = end - 1;
                        while (start >= 0 && CodeTextBox.Text[start] != ';' && CodeTextBox.Text[start] != '{') start--;
                        start++;
                        model.Look(CodeTextBox.Text, start, end);
                        break;
                    case '\n':
                        if (CCSProfix.Length > 0 && CCSIsSelected)
                        {
                            end = CodeTextBox.CaretOffset;
                            start = end - 1;
                            while (start > 0 && (CodeTextBox.Text[start] == '\t' || CodeTextBox.Text[start] == '\n'))
                                start--;
                            start++;
                            CodeTextBox.Text = CodeTextBox.Text.Remove(start, end-start);
                            model.Current.InnerOffset -= end - start;
                            CodeTextBox.CaretOffset -= end - start;
                            CCSProfix = String.Empty;
                        }
                        break;
                    default:
                        if (!Char.IsLetterOrDigit(e.Text[0]))
                        {
                            CCSProfix = String.Empty;
                        }
                        break;
                }
            }
        }

        void textEditer_CaretChanged(object sender, RoutedEventArgs e)
        {
            model.Move(CodeTextBox.CaretOffset);
        }

        void textEditer_TextArea_TextDeleted(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0)
            {
                switch (e.Text[0])
                {
                    case '{':
                        break;
                    case '(':
                        break;
                    case ')':
                        break;
                    case '}':
                        break;
                    case ';':
                        break;
                    default:
                        break;
                }
            }
        }
        

        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, CodeTextBox.Document);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #region Code Completion

        #region Selected List
        private List<string> ccslist;
        private TextBlock[] ccstblocks;

        private int ccsviewpoint;
        private string ccsprofix;
        private bool ccsisselected;
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
                    ccsviewpoint = 0;
                    CCSCursor = 0;
                    CCSUpdate();
                    CCSIsSelected = false;
                    CodeTextBox.TextArea.IsCodeCompleteMode = true;
                }
            }
        }

        private void CCSUpdate()
        {
            for (int i = 0; i < ccstblocks.Length; i++)
            {
                if (i + ccsviewpoint >= ccslist.Count())
                {
                    ccstblocks[i].Text = String.Empty;
                }
                else
                {
                    ccstblocks[i].Text = ccslist[i + ccsviewpoint];
                }
            }
        }

        #endregion

        #region Selected Cursor
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
            }
        }

        private void textEditer_TextArea_CodeCompleteKeyUp(object sender, KeyEventArgs e)
        {
            
        }

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
                        CodeTextBox.Text = CodeTextBox.Text.Insert(CodeTextBox.CaretOffset, inserttext);
                        model.Current.InnerOffset += inserttext.Length;
                        CodeTextBox.CaretOffset += inserttext.Length;
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
            }
        }

        #endregion

        #region Scroll
        private bool scroll_ispressed;
        private double scroll_x;
        private double scroll_y;

        private void Scroll_MouseLeave(object sender, MouseEventArgs e)
        {
            Scroll.Opacity = 0.6;
        }

        private void Scroll_MouseEnter(object sender, MouseEventArgs e)
        {
            Scroll.Opacity = 1.0;
        }

        private void Scroll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            scroll_ispressed = true;
            Point p = e.GetPosition(this);
            scroll_x = p.X;
            scroll_y = p.Y;
        }

        private void Scroll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            scroll_ispressed = false;
        }


        private void Scroll_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(this);

        }
        #endregion

        #endregion
    }

}

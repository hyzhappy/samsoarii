
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

namespace SamSoarII.AppMain.Project
{
    /// <summary>
    /// FuncBlockViewModel.xaml 的交互逻辑
    /// </summary>
    public partial class FuncBlockViewModel : UserControl, IProgram
    {
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
            }

        }



        public FuncBlockViewModel(string name)
        {
            InitializeComponent();
            ProgramName = name;
            InitializeComponent();
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
            ProgramName = name;
            CodeTextBox.DataContext = this;
            CodeTextBox.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(0.5);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();
            foldingManager = FoldingManager.Install(CodeTextBox.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, CodeTextBox.Document);
            CodeTextBox.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(CodeTextBox.Options);
        }
        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    completionWindow.CompletionList.RequestInsertion(e);
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
    }
}

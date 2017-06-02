using SamSoarII.AppMain.Project;
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
    /// TextFindWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TextFindWindow : UserControl, INotifyPropertyChanged
    {
        static string[] SPECIALLABELS = {"\\", ".", "^", "$", "(", ")", "[", "]", "{", "}"};

        #region Numbers

        private InteractionFacade parent;

        private LadderCommand.CommandManager _cmdmanager
            = new LadderCommand.CommandManager();

        private ObservableCollection<TextFindElement> items
            = new ObservableCollection<TextFindElement>();
        public IEnumerable<TextFindElement> Items
        {
            get
            {
                return this.items;
            }
            set
            {
                items.Clear();
                foreach (TextFindElement item in value)
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
                PropertyChanged(this, new PropertyChangedEventArgs("IgnoreCase"));
            }
        }

        #endregion

        public TextFindWindow(InteractionFacade _parent)
        {
            InitializeComponent();
            DataContext = this;
            parent = _parent;
            parent.CurrentTabChanged += OnCurrentTabChanged;
            Mode = MODE_CURRENT;
        }

        public void Initialize()
        {
            items.Clear();
        }

        private void Find()
        {
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
            if (IgnoreCase) opt |= RegexOptions.IgnoreCase;
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
                items.Add(new TextFindElement(this, fbvmodel, match.Index, match.Value));
                match = match.NextMatch();
            }
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

        private void DG_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!parent.MainWindow.LAFind.IsFloat
             && !parent.MainWindow.LAFind.IsDock)
            {
                LayoutSetting.AddDefaultDockWidthAnchorable(
                    "查找", parent.MainWindow.LAFind.AutoHideWidth.ToString());
                LayoutSetting.AddDefaultDockHeighAnchorable(
                    "查找", parent.MainWindow.LAFind.AutoHideHeight.ToString());
                parent.MainWindow.LAFind.ToggleAutoHide();
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

    public class TextFindElement : TextReplaceElement
    {
        private TextFindWindow tfwindow;

        public TextFindElement
        (
            TextFindWindow      _tfwindow,
            FuncBlockViewModel  _fbvmodel, 
            int                 _textoffset, 
            string              _word
        ) 
        : base(null, _fbvmodel, _textoffset, _word)
        {
            tfwindow = _tfwindow;
        }

        protected override void OnTextChanged(object sender, RoutedEventArgs e)
        {
            //base.OnTextChanged(sender, e);
            tfwindow.Initialize();
        }
    }
}

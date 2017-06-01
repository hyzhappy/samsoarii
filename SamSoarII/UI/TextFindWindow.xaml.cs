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

        public TextFindWindow()
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
                items.Add(new TextFindElement(fbvmodel, match.Index, match.Value));
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
        }

        #endregion
    }

    public class TextFindElement : TextReplaceElement
    {
        public TextFindElement(FuncBlockViewModel _fbvmodel, int _textoffset, string _word) 
            : base(_fbvmodel, _textoffset, _word)
        {
        }
    }
}

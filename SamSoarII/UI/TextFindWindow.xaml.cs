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

/// <summary>
/// Namespace : SamSoarII.AppMain.UI
/// ClassName : FuncModel
/// Version   : 1.0
/// Date      : 2017/6/9
/// Author    : Morenan
/// </summary>
/// <remarks>
/// 对文本（函数块）进行查找的窗口
/// </remarks>

namespace SamSoarII.AppMain.UI
{
    /// <summary>
    /// TextFindWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TextFindWindow : UserControl, INotifyPropertyChanged
    {
        /// <summary> 正则表达式中的特殊符 </summary>
        static string[] SPECIALLABELS = {"\\", ".", "^", "$", "(", ")", "[", "]", "{", "}"};

        #region Numbers

        /// <summary> 主交互类 </summary>
        private InteractionFacade parent;
        /// <summary> 所有查找到的元素 </summary>
        private ObservableCollection<TextFindElement> items
            = new ObservableCollection<TextFindElement>();
        /// <summary> 所有查找到的元素 </summary>
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
        /// <summary> 模式：当前文本 </summary>
        public const int MODE_CURRENT = 0x00;
        /// <summary> 模式：所有文本 </summary>
        public const int MODE_ALL = 0x01;
        /// <summary> 当前模式 </summary>
        private int mode;
        /// <summary> 当前模式 </summary>
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

        /// <summary> 用户选项：是否使用正则表达式 </summary>
        private bool isregex;
        /// <summary> 用户选项：是否使用正则表达式 </summary>
        public bool IsRegex
        {
            get { return this.isregex; }
            set
            {
                this.isregex = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsRegex"));
            }
        }
        /// <summary> 用户选项：是否忽视大小写 </summary>
        private bool ignorecase;
        /// <summary> 用户选项：是否忽视大小写 </summary>
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

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_parent">交互</param>
        public TextFindWindow(InteractionFacade _parent)
        {
            InitializeComponent();
            DataContext = this;
            parent = _parent;
            parent.CurrentTabChanged += OnCurrentTabChanged;
            Mode = MODE_CURRENT;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            items.Clear();
        }
        /// <summary>
        /// 查找
        /// </summary>
        private void Find()
        {
            items.Clear();
            if (TB_Input.Text.Length <= 0)
                return;
            switch (Mode)
            {
                // 当前文本
                case MODE_CURRENT:
                    ITabItem currenttab = parent.MainTabControl.CurrentTab;
                    if (currenttab is FuncBlockViewModel)
                    {
                        FuncBlockViewModel fbvmodel = (FuncBlockViewModel)currenttab;
                        Find(fbvmodel);
                    }
                    break;
                // 所有文本
                case MODE_ALL:
                    ProjectModel pmodel = parent.ProjectModel;
                    Find(pmodel.LibFuncBlock);
                    foreach (FuncBlockViewModel fbvmodel in pmodel.FuncBlocks)
                    {
                        Find(fbvmodel);
                    }
                    break;
            }
            PropertyChanged(this, new PropertyChangedEventArgs("Items"));
        }
        /// <summary>
        /// 在函数块中查找
        /// </summary>
        /// <param name="fbvmodel"></param>
        private void Find(FuncBlockViewModel fbvmodel)
        {
            // 函数文本和要查找的单词
            string text = fbvmodel.Code;
            string word = TB_Input.Text;
            // 正则匹配选项
            RegexOptions opt = RegexOptions.None;
            opt |= RegexOptions.Singleline;
            if (IgnoreCase) opt |= RegexOptions.IgnoreCase;
            // 非正则匹配时替换掉特殊符
            if (!IsRegex)
            {
                foreach (string slabel in SPECIALLABELS)
                {
                    word = word.Replace(slabel, "\\" + slabel);
                }
            }
            // 开始查找并添加到元素集
            Match match = Regex.Match(text, word, opt);
            while (match != null && match.Success)
            {
                items.Add(new TextFindElement(this, fbvmodel, match.Index, match.Value));
                match = match.NextMatch();
            }
        }

        #region Event Handler

        /// <summary>
        /// 值更改时触发
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        /// <summary>
        /// 输入文本更改时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TB_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            TB_Input.Background = TB_Input.Text.Length > 0
                ? Brushes.LightGreen
                : Brushes.White;
        }
        /// <summary>
        /// 在输入框内按下按键时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TB_Input_KeyDown(object sender, KeyEventArgs e)
        {
            // 按下Enter键开始查找
            if (e.Key != Key.Enter) return;
            Find();
            TB_Input.Background = Brushes.White;
        }
        /// <summary>
        /// 当在查找列表中选择时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DG_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!parent.MainWindow.LAFind.IsFloat
             && !parent.MainWindow.LAFind.IsDock)
            {
                LayoutSetting.AddDefaultDockWidthAnchorable(
                    Properties.Resources.MainWindow_Search, parent.MainWindow.LAFind.AutoHideWidth.ToString());
                LayoutSetting.AddDefaultDockHeighAnchorable(
                    Properties.Resources.MainWindow_Search, parent.MainWindow.LAFind.AutoHideHeight.ToString());
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
        /// <summary>
        /// 当当前TAB界面更换时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

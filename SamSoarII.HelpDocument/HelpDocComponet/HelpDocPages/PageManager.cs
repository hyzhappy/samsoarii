using SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages.Function_Features_Block;
using SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages.Function_Features_Block.Getting_Start_To_C;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.Communication_Protocol;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.Contact_Information;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.Create_New_Project_Steps;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.Function_Features_Block.C_Programming_Preliminary;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.Hardware_Manual;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Auxiliary_Function;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.BitLogic;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Communication;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Compare;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Convert;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Counter;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.FloatCalculation;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.HCNT;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.IntegerCalculation;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Interrupt;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.LogicOperation;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Move;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.PID;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.ProgramControl;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Pulse;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.RealTime;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Shift;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Instruction.Timer;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.PLC_Parameter_Setting;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.Register;
using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages.System_Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages
{
    public class PageManager
    {
        private static Dictionary<int, HelpDocFrame> _pageCollection = new Dictionary<int, HelpDocFrame>();
        private HelpDocWindow _helpDocWindow;
        private PageTabControl _pageTabControl;
        private HelpDocTreeView _helpDocTreeView;
        private HelpDocFavorite _helpDocFavorite;
        private HelpDocSearch _helpDocSearch;
        private BrowsingHistoryManager _browsingHistoryManager;
        public static Dictionary<int, HelpDocFrame> PageCollection
        {
            get
            {
                return _pageCollection;
            }
        }
        public PageManager(HelpDocWindow window)
        {
            InitializeNavigateEvent();
            _helpDocWindow = window;
            _helpDocWindow.BackCommand.Executed += BackCommand_Executed;
            _helpDocWindow.AheadCommand.Executed += AheadCommand_Executed;
            _helpDocWindow.BackCommand.CanExecute += BackCommand_CanExecute;
            _helpDocWindow.AheadCommand.CanExecute += AheadCommand_CanExecute;
            _helpDocWindow.CollectCommand.Executed += CollectCommand_Executed;
            _helpDocWindow.CollectCommand.CanExecute += CollectCommand_CanExecute;
            _helpDocWindow.Home.Click += Home_Click;
            _pageTabControl = window.MainTab;
            _pageTabControl.PageTabControlStatusChanged += _pageTabControl_PageTabControlStatusChanged;
            _helpDocTreeView = (HelpDocTreeView)_helpDocWindow.GetView(0);
            _helpDocFavorite = (HelpDocFavorite)_helpDocWindow.GetView(1);
            _helpDocSearch = (HelpDocSearch)_helpDocWindow.GetView(2);
            _helpDocTreeView.MouseDoubleClickEventHandler += _helpDocTreeView_MouseDoubleClickEventHandler;
            _helpDocTreeView.CollectCommand.Executed += CollectCommand_Executed1;
            _helpDocTreeView.CollectCommand.CanExecute += CollectCommand_CanExecute1;
            _helpDocFavorite.DeleteCommand.Executed += DeleteCommand_Executed;
            _helpDocFavorite.DeleteCommand.CanExecute += DeleteCommand_CanExecute;
            _helpDocFavorite.DeleteAllCommand.Executed += DeleteAllCommand_Executed;
            _helpDocFavorite.DeleteAllCommand.CanExecute += DeleteAllCommand_CanExecute;
            _helpDocFavorite.OpenCommand.Executed += OpenCommand_Executed;
            _helpDocFavorite.OpenCommand.CanExecute += OpenCommand_CanExecute;
            _helpDocFavorite.ItemDoubleClick += _helpDocFavorite_ItemDoubleClick;
            _helpDocSearch.OpenCommand.Executed += OpenCommand_Executed1;
            _helpDocSearch.OpenCommand.CanExecute += OpenCommand_CanExecute1;
            _helpDocSearch.ItemDoubleClick += _helpDocSearch_ItemDoubleClick;
            _helpDocSearch.CollectCommand.Executed += CollectCommand_Executed2;
            _helpDocSearch.CollectCommand.CanExecute += CollectCommand_CanExecute2;
            _browsingHistoryManager = new BrowsingHistoryManager();
            _helpDocWindow.Loaded += _helpDocWindow_Loaded;
            _helpDocWindow.FuncGrid.SizeChanged += FuncGrid_SizeChanged;
        }
        private void CollectCommand_CanExecute2(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _helpDocSearch.CollectList.SelectedItem != null;
        }

        private void CollectCommand_Executed2(object sender, ExecutedRoutedEventArgs e)
        {
            HelpDocFrame page = GetFrameByPageIndex((_helpDocSearch.CollectList.SelectedItem as HelpDocFrame).PageIndex);
            FavoriteManager.CollectPage(page);
        }
        private void _helpDocFavorite_ItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ShowPage(((sender as ListBoxItem).Content as HelpDocFrame).PageIndex);
            }
        }
        private void _helpDocSearch_ItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ShowPage(((sender as ListBoxItem).Content as HelpDocFrame).PageIndex);
            }
        }

        private void OpenCommand_CanExecute1(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _helpDocSearch.CollectList.SelectedItems.Count == 1;
        }
        private void NavigateToPage(NavigateToPageEventArgs e)
        {
            ShowPage(e.PageIndex);
        }
        private void OpenCommand_Executed1(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ShowPage((_helpDocSearch.CollectList.SelectedItem as HelpDocFrame).PageIndex);
        }
        private void OpenCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _helpDocFavorite.CollectList.SelectedItems.Count == 1;
        }
        private void OpenCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ShowPage((_helpDocFavorite.CollectList.SelectedItem as HelpDocFrame).PageIndex);
        }
        private void DeleteAllCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _helpDocFavorite.CollectList.Items.Count != 0;
        }

        private void DeleteAllCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            FavoriteManager.DeleteAllPages();
        }
        private void DeleteCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _helpDocFavorite.CollectList.SelectedItems.Count == 1;
        }

        private void DeleteCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            FavoriteManager.DeletePage(_helpDocFavorite.CollectList.SelectedItem as HelpDocFrame);
        }

        private void CollectCommand_CanExecute1(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _helpDocTreeView.HelpDocTree.SelectedItem != null;
        }
        private void CollectCommand_Executed1(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            HelpDocFrame page = GetFrameByPageIndex((_helpDocTreeView.HelpDocTree.SelectedItem as HelpDocTreeItem).PageIndex);
            FavoriteManager.CollectPage(page);
        }

        private void CollectCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _pageTabControl.SelectedItem != null;
        }

        private void CollectCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            HelpDocFrame page = _pageTabControl.SelectedItem as HelpDocFrame;
            FavoriteManager.CollectPage(page);
        }

        private void AheadCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _browsingHistoryManager.CanAhead();
        }
        private void BackCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _browsingHistoryManager.CanBack();
        }

        private void AheadCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (_browsingHistoryManager.CanAhead())
            {
                _browsingHistoryManager.backHistoryStackPush(_pageTabControl.GetCurrentStatus());
                _pageTabControl.ChangeToStatus(_browsingHistoryManager.aheadHistoryStackPop(), this);
            }
        }

        private void BackCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (_browsingHistoryManager.CanBack())
            {
                _browsingHistoryManager.aheadHistoryStackPush(_pageTabControl.GetCurrentStatus());
                _pageTabControl.ChangeToStatus(_browsingHistoryManager.backHistoryStackPop(), this);
            }
        }

        private void _pageTabControl_PageTabControlStatusChanged(PageTabControlStatusChangedEventArgs e)
        {
            _browsingHistoryManager.backHistoryStackPush(e.OldStatus);
            _browsingHistoryManager.ClearAheadHistoryStack();
        }
        private void Home_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ShowPage(1000);
        }
        private void _helpDocTreeView_MouseDoubleClickEventHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HelpDocTreeItem item = sender as HelpDocTreeItem;
            ShowPage(item.PageIndex);
            e.Handled = true;
        }
        private void _helpDocWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _helpDocWindow.ShowView(0);
            _pageTabControl.InitializeTabItemCollection(GetFrameByPageIndex(1000));
        }
        private void FuncGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _helpDocSearch.CollectList.Height = e.NewSize.Height - 75;
            _helpDocFavorite.CollectList.Height = e.NewSize.Height - 75;
            _helpDocSearch.CollectList.Width = e.NewSize.Width;
            _helpDocFavorite.CollectList.Width = e.NewSize.Width;
        }
        #region Initialize pageCollection
        static PageManager()
        {
            Add(new MainPage());
            Add(new Page2000());
            Add(new Page2100());
            Add(new Page2200());
            Add(new Page2300());
            Add(new Page2400());
            Add(new Page2500());
            Add(new Page2501());
            Add(new Page3000());
            Add(new Page3100());
            Add(new Page3200());
            Add(new Page3201());
            Add(new Page3202());
            Add(new Page3203());
            Add(new Page3204());
            Add(new Page3205());
            Add(new Page3206());
            Add(new Page4000());
            Add(new Page4100());
            Add(new Page5000());
            Add(new Page5100());
            Add(new Page5101());
            Add(new Page5102());
            Add(new Page5103());
            Add(new Page5104());
            Add(new Page5105());
            Add(new Page5106());
            Add(new Page5107());
            Add(new Page5108());
            Add(new Page5109());
            Add(new Page5110());
            Add(new Page5111());
            Add(new Page5112());
            Add(new Page5113());
            Add(new Page5114());
            Add(new Page5115());
            Add(new Page5116());
            Add(new Page5117());
            Add(new Page5200());
            Add(new Page5201());
            Add(new Page5202());
            Add(new Page5203());
            Add(new Page5204());
            Add(new Page5205());
            Add(new Page5206());
            Add(new Page5300());
            Add(new Page5301());
            Add(new Page5302());
            Add(new Page5303());
            Add(new Page5304());
            Add(new Page5305());
            Add(new Page5306());
            Add(new Page5307());
            Add(new Page5400());
            Add(new Page5401());
            Add(new Page5402());
            Add(new Page5403());
            Add(new Page5404());
            Add(new Page5405());
            Add(new Page5406());
            Add(new Page5407());
            Add(new Page5408());
            Add(new Page5500());
            Add(new Page5501());
            Add(new Page5502());
            Add(new Page5503());
            Add(new Page5504());
            Add(new Page5505());
            Add(new Page5600());
            Add(new Page5601());
            Add(new Page5602());
            Add(new Page5603());
            Add(new Page5604());
            Add(new Page5605());
            Add(new Page5606());
            Add(new Page5607());
            Add(new Page5608());
            Add(new Page5609());
            Add(new Page5610());
            Add(new Page5700());
            Add(new Page5701());
            Add(new Page5702());
            Add(new Page5703());
            Add(new Page5704());
            Add(new Page5705());
            Add(new Page5706());
            Add(new Page5707());
            Add(new Page5708());
            Add(new Page5709());
            Add(new Page5710());
            Add(new Page5711());
            Add(new Page5712());
            Add(new Page5713());
            Add(new Page5714());
            Add(new Page5800());
            Add(new Page5801());
            Add(new Page5802());
            Add(new Page5803());
            Add(new Page5900());
            Add(new Page5901());
            Add(new Page5902());
            Add(new Page5903());
            Add(new Page6000());
            Add(new Page6001());
            Add(new Page6002());
            Add(new Page6003());
            Add(new Page6004());
            Add(new Page6005());
            Add(new Page6006());
            Add(new Page6100());
            Add(new Page6101());
            Add(new Page6102());
            Add(new Page6103());
            Add(new Page6104());
            Add(new Page6105());
            Add(new Page6106());
            Add(new Page6107());
            Add(new Page6108());
            Add(new Page6109());
            Add(new Page6110());
            Add(new Page6200());
            Add(new Page6201());
            Add(new Page6202());
            Add(new Page6203());
            Add(new Page6204());
            Add(new Page6300());
            Add(new Page6301());
            Add(new Page6302());
            Add(new Page6400());
            Add(new Page6401());
            Add(new Page6402());
            Add(new Page6403());
            Add(new Page6500());
            Add(new Page6501());
            Add(new Page6502());
            Add(new Page6503());
            Add(new Page6504());
            Add(new Page6505());
            Add(new Page6506());
            Add(new Page6507());
            Add(new Page6508());
            Add(new Page6509());
            Add(new Page6510());
            Add(new Page6511());
            Add(new Page6512());
            Add(new Page6513());
            Add(new Page6514());
            Add(new Page6515());
            Add(new Page6516());
            Add(new Page6517());
            Add(new Page6600());
            Add(new Page6601());
            Add(new Page6700());
            Add(new Page6701());
            Add(new Page6702());
            Add(new Page6703());
            Add(new Page6704());
            Add(new Page6705());
            Add(new Page6706());
            Add(new Page6707());
            Add(new Page6708());
            Add(new Page6709());
            Add(new Page6710());
            Add(new Page6711());
            Add(new Page6712());
            Add(new Page6713());
            Add(new Page6714());
            Add(new Page6715());
            Add(new Page6716());
            Add(new Page6717());
            Add(new Page6718());
            Add(new Page6719());
            Add(new Page6800());
            Add(new Page7000());
            Add(new Page7100());
            Add(new Page7101());
            Add(new Page7102());
            Add(new Page7103());
            Add(new Page7104());
            Add(new Page7105());
            Add(new Page7200());
            Add(new Page7201());
            Add(new Page7202());
            Add(new Page7203());
            Add(new Page7204());
            Add(new Page7205());
            Add(new Page7206());
            Add(new Page7207());
            Add(new Page7208());
            Add(new Page7300());
            Add(new Page7400());
            Add(new Page8000());
            Add(new Page8100());
            Add(new Page8200());
            Add(new Page8300());
            Add(new Page8400());
            Add(new Page8500());
        }
        private static void Add(IPageItem page)
        {
            _pageCollection.Add(page.PageIndex, new HelpDocFrame(page));
        }
        private void InitializeNavigateEvent()
        {
            foreach (var item in _pageCollection.Values)
            {
                if (item.Page is NavigatePage)
                {
                    (item.Page as NavigatePage).NavigateToPage += NavigateToPage;
                }
            }
        }
        #endregion
        public HelpDocFrame GetFrameByPageIndex(int pageIndex)
        {
            return _pageCollection[pageIndex];
        }
        public void ShowPage(int pageIndex)
        {
            _pageTabControl.ShowItem(GetFrameByPageIndex(pageIndex));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using SamSoarII.AppMain.Project;
using System.Collections.ObjectModel;
using Xceed.Wpf.AvalonDock.Layout;
using SamSoarII.Simulation.UI;
using SamSoarII.Simulation.UI.Chart;
using SamSoarII.AppMain.UI.HelpDocComponet;
using SamSoarII.HelpDocument.HelpDocComponet;

namespace SamSoarII.AppMain.UI
{
    public partial class MainTabControl : LayoutDocumentPane
    {

        #region Numbers

        #region View Mode
        public const int VIEWMODE_LADDER = 0x01;
        public const int VIEWMODE_INST = 0x02;
        private int viewmode;
        public int ViewMode
        {
            get { return this.viewmode; }
            set
            {
                this.viewmode = value;
                foreach (MainTabDiagramItem mtditem in DiagramCollection)
                {
                    mtditem.ViewMode = value;
                }
                //ShowItem(CurrentTab);
            }
        }
        #endregion

        #region Collections

        public ObservableCollection<ITabItem> TabItemCollection { get; set; } = new ObservableCollection<ITabItem>();
        public ObservableCollection<MainTabDiagramItem> DiagramCollection { get; set; } = new ObservableCollection<MainTabDiagramItem>();
        private Dictionary<ITabItem, LayoutDocument> _lDocDict = new Dictionary<ITabItem, LayoutDocument>();

        #endregion

        #region Current
        public ITabItem SelectedItem = null;
        public ITabItem CurrentTab
        {
            get
            {
                return SelectedItem;
            }
        }
        #endregion

        #region Inner Scroll

        private CanAnimationScroll innerscroll = new CanAnimationScroll();

        public CanAnimationScroll InnerScroll
        {
            get { return innerscroll; }
        }

        #endregion

        #endregion

        public MainTabControl()
        {
            ViewMode = VIEWMODE_LADDER;
        }

        public void Reset()
        {
            TabItemCollection.Clear();
            DiagramCollection.Clear();
            _lDocDict.Clear();
            Children.Clear();
        }
        
        public void ShowItem(ITabItem item)
        {
            LayoutDocument ldoc = null;
            if (item is UserControl)
            {
                UserControl uctrl = (UserControl)(item);
                uctrl.GotFocus += OnTabGotFocus;
            }
            if (!TabItemCollection.Contains(item))
            {
                TabItemCollection.Add(item);
                ldoc = new LayoutDocument();
                ldoc.Title = item.TabHeader;
                if (item is LadderDiagramViewModel)
                {
                    MainTabDiagramItem mtditem = new MainTabDiagramItem((IProgram)item, ViewMode);
                    DiagramCollection.Add(mtditem);
                    ldoc.Content = mtditem;
                }
                else
                {
                    ldoc.Content = item;
                }
                ldoc.IsActiveChanged += OnActiveChanged;
                Children.Add(ldoc);
                _lDocDict.Add(item, ldoc);
            }
            else
            {
                ldoc = _lDocDict[item];
            }
            int ldocid = Children.IndexOf(ldoc);
            SelectedItem = item;
            SelectedContentIndex = ldocid;
        }
        
        public void CloseItem(ITabItem item)
        {
            if (_lDocDict.ContainsKey(item))
            {
                LayoutDocument ldoc = _lDocDict[item];
                Children.Remove(ldoc);
                _lDocDict.Remove(item);
                TabItemCollection.Remove(item);
                if (item is LadderDiagramViewModel)
                {
                    MainTabDiagramItem[] fit = DiagramCollection.Where(
                        (MainTabDiagramItem mtditem) => { return mtditem.LA_Ladder.Content == item; }).ToArray();
                    foreach (MainTabDiagramItem mtditem in fit)
                    {
                        DiagramCollection.Remove(mtditem);
                    }
                }
            }
        }

        public void ReplaceAllTabsToSimulate()
        {
            IEnumerable<ITabItem> ntabs = TabItemCollection.Where(
                (tab) => { return (tab is LadderDiagramViewModel || tab is FuncBlockViewModel); });
            ntabs = ntabs.ToArray();
            Simulation.Shell.Event.ShowTabItemEventArgs e = null;
            foreach (ITabItem tab in ntabs)
            {
                if (tab is LadderDiagramViewModel)
                {
                    CloseItem(tab);
                    e = new Simulation.Shell.Event.ShowTabItemEventArgs(tab.TabHeader);
                    ShowSimulateItem(this, e);
                }
                if (tab is FuncBlockViewModel)
                {
                    CloseItem(tab);
                    e = new Simulation.Shell.Event.ShowTabItemEventArgs(tab.TabHeader);
                    ShowSimulateItem(this, e);
                }
            }
            if (ntabs.Contains(SelectedItem))
            {
                IEnumerable<ITabItem> selecttab = TabItemCollection.Where(
                    (ITabItem tab) => 
                    {
                        bool b1 = tab.TabHeader.Equals(SelectedItem.TabHeader);
                        bool b2 = tab.TabHeader.Equals("main") || tab.TabHeader.Equals("主程序");
                        bool b3 = SelectedItem.TabHeader.Equals("main") || SelectedItem.TabHeader.Equals("主程序");
                        return b1 || (b2 && b3);
                    }
                );
                if (selecttab.First() != null)
                {
                    ShowItem(selecttab.First());
                }
            }
        }

        public void ReplaceAllTabsToEdit()
        {
            IEnumerable<ITabItem> ntabs = TabItemCollection.Where(
                (tab) => { return (tab is SimulateHelper.SimulateTabItem); });
            ntabs = ntabs.ToArray();
            ShowTabItemEventArgs e = null;
            foreach (ITabItem tab in ntabs)
            {
                CloseItem(tab);
                e = new ShowTabItemEventArgs(TabType.SimuToEdit);
                e.Header = tab.TabHeader;
                ShowEditItem(this, e);        
            }
            if (ntabs.Contains(SelectedItem))
            {
                IEnumerable<ITabItem> selecttab = TabItemCollection.Where(
                    (ITabItem tab) =>
                    {
                        bool b1 = tab.TabHeader.Equals(SelectedItem.TabHeader);
                        bool b2 = tab.TabHeader.Equals("main") || tab.TabHeader.Equals("主程序");
                        bool b3 = SelectedItem.TabHeader.Equals("main") || SelectedItem.TabHeader.Equals("主程序");
                        return b1 || (b2 && b3);
                    }
                );
                if (selecttab.First() != null)
                {
                    ShowItem(selecttab.First());
                }
            }
        }

        #region Event Handler

        public event Simulation.Shell.Event.ShowTabItemEventHandler ShowSimulateItem = delegate { };

        public event ShowTabItemEventHandler ShowEditItem = delegate { };

        public event SelectionChangedEventHandler SelectionChanged = delegate { };

        public event RoutedEventHandler GotFocus = delegate { };

        private void OnActiveChanged(object sender, EventArgs e)
        {
            if (sender is LayoutDocument)
            {
                LayoutDocument ldoc = (LayoutDocument)(sender);
                if (ldoc.Content != SelectedItem)
                {
                    ITabItem _SelectedItem = SelectedItem;
                    //LayoutDocument _ldoc = _lDocDict[_SelectedItem];
                    SelectedItem = (ITabItem)(ldoc.Content);
                    if (SelectionChanged != null)
                    {
                        SelectionChangedEventArgs _e = null;
                        SelectionChanged(this, _e);
                    }
                }
            }
        }
        
        protected override void OnActualWidthChanged()
        {
            base.OnActualWidthChanged();
            ILayoutPositionableElementWithActualSize _maintab = (ILayoutPositionableElementWithActualSize)(this);
            int unitwidth = GlobalSetting.LadderWidthUnit;
            int unitnumber = GlobalSetting.LadderXCapacity;
            //GlobalSetting.LadderOriginScaleX = (_maintab.ActualWidth - 40) / (unitwidth * unitnumber);
            //GlobalSetting.LadderOriginScaleY = _maintab.ActualWidth / (unitwidth * unitnumber);
            foreach (ITabItem tab in TabItemCollection)
            {
                tab.ActualWidth = _maintab.ActualWidth;
            }
        }

        protected override void OnActualHeightChanged()
        {
            base.OnActualHeightChanged();

            ILayoutPositionableElementWithActualSize _maintab = (ILayoutPositionableElementWithActualSize)(this);
            foreach (ITabItem tab in TabItemCollection)
            {
                tab.ActualHeight = _maintab.ActualHeight;
            }
        }
        
        protected override void OnChildrenCollectionChanged()
        {
            base.OnChildrenCollectionChanged();
            foreach (ITabItem tab in TabItemCollection)
            {
                if (!_lDocDict.ContainsKey(tab))
                    continue;
                LayoutDocument ldoc = _lDocDict[tab];
                if (!Children.Contains(ldoc))
                {
                    CloseItem(tab);
                    return;
                }
            }
        }

        private void OnTabGotFocus(object sender, RoutedEventArgs e)
        {
            GotFocus(this, e);
        }

        #endregion
    }
}

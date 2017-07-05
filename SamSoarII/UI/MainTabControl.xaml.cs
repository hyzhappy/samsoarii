using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SamSoarII.AppMain.Project;
using System.Collections.ObjectModel;
using Xceed.Wpf.AvalonDock.Layout;
using SamSoarII.HelpDocument.HelpDocComponet;
using SamSoarII.AppMain.UI.Style;
using Xceed.Wpf.AvalonDock.Controls;

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
        //public ObservableCollection<ITabItem> FloatingCollection { get; set; } = new ObservableCollection<ITabItem>();
        public ObservableCollection<MainTabDiagramItem> DiagramCollection { get; set; } = new ObservableCollection<MainTabDiagramItem>();

        private Dictionary<ITabItem, LayoutDocument> _lDocDict = new Dictionary<ITabItem, LayoutDocument>();

        #endregion

        #region Current

        private ITabItem selecteditem = null;
        public ITabItem SelectedItem
        {
            get
            {
                return selecteditem;
            }
            set
            {
                if (selecteditem != value)
                {
                    selecteditem = value;
                    SelectionChanged(this, null);
                }
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
            foreach (ITabItem item in TabItemCollection)
            {
                if (item.IsFloat)
                {
                    LayoutFloatingWindowControl fwctrl = item.FloatControl;
                    fwctrl.Close();
                }
            }
            TabItemCollection.Clear();
            DiagramCollection.Clear();
            _lDocDict.Clear();
            Children.Clear();
        }
        
        public void ShowItem(ITabItem item)
        {
            //bool isnew = false;
            LayoutDocument ldoc = null;
            if (item is FuncBlockViewModel)
            {
                FuncBlockViewModel fbvmodel = (FuncBlockViewModel)item;
                fbvmodel.CodeTextBox.Focus();
            }
            if (item.IsFloat)
            {
                LayoutFloatingWindowControl fwctrl = item.FloatControl;
                fwctrl.Focus();
                return;
            }
            if (!TabItemCollection.Contains(item))
            {
                TabItemCollection.Add(item);
                item.FloatClosed += OnTabFloatClosed;
                ldoc = new LayoutDocument();
                ldoc.Title = item.TabHeader;
                if (item is LadderDiagramViewModel)
                {
                    IEnumerable<MainTabDiagramItem> fit = DiagramCollection.Where(
                        (MainTabDiagramItem _mtditem) => { return _mtditem.LDVM_ladder == item; });
                    MainTabDiagramItem mtditem = null;
                    if (fit.Count() == 0)
                    {
                        mtditem = new MainTabDiagramItem((IProgram)item, ViewMode);
                        DiagramCollection.Add(mtditem);
                    }
                    else
                    {
                        mtditem = fit.First();
                    }
                    ldoc.Content = mtditem;
                }
                else
                {
                    ldoc.Content = item;
                }
                ldoc.IsActiveChanged += OnActiveChanged;
                Children.Add(ldoc);
                _lDocDict.Add(item, ldoc);
                //isnew = true;
            }
            else
            {
                ldoc = _lDocDict[item];
            }
            int ldocid = Children.IndexOf(ldoc);
            SelectedItem = item;
            SelectedContentIndex = ldocid;
        }

        public void RenameItem(ITabItem item)
        {
            if (_lDocDict.ContainsKey(item))
            {
                LayoutDocument ldoc = _lDocDict[item];
                ldoc.Title = item.TabHeader;
            }
        }
        
        public void CloseItem(ITabItem item)
        {
            if (_lDocDict.ContainsKey(item))
            {
                item.FloatClosed -= OnTabFloatClosed;
                LayoutDocument ldoc = _lDocDict[item];
                _lDocDict.Remove(item);
                TabItemCollection.Remove(item);
                if (item.IsFloat)
                {
                    LayoutFloatingWindowControl fwctrl = item.FloatControl;
                    fwctrl.Close();
                    item.IsFloat = false;
                }
                else
                {
                    Children.Remove(ldoc);
                }
                if (item is LadderDiagramViewModel)
                {
                    IEnumerable<MainTabDiagramItem> fit = DiagramCollection.Where(
                        (MainTabDiagramItem _mtditem) => { return _mtditem.LDVM_ladder == item; });
                    MainTabDiagramItem mtditem = null;
                    if (fit.Count() > 0)
                    { 
                        mtditem = fit.First();
                        DiagramCollection.Remove(mtditem);
                    }
                }
                CloseTabItem(this,new RoutedEventArgs());
            }
        }
        
        #region Event Handler

        public event Simulation.Shell.Event.ShowTabItemEventHandler ShowSimulateItem = delegate { };
        
        public event SelectionChangedEventHandler SelectionChanged = delegate { };

        public event RoutedEventHandler CloseTabItem = delegate { };
        public event RoutedEventHandler FloatingWinClosed = delegate { };
        public event RoutedEventHandler GotFocus = delegate { };

        private void OnActiveChanged(object sender, EventArgs e)
        {
            if (sender is LayoutDocument)
            {
                LayoutDocument ldoc = (LayoutDocument)(sender);
                if (ldoc.Content != SelectedItem)
                {
                    ITabItem _SelectedItem = SelectedItem;
                    SelectedItem = (ITabItem)(ldoc.Content);
                }
            }
        }
  
        protected override void OnActualWidthChanged()
        {
            base.OnActualWidthChanged();
            ILayoutPositionableElementWithActualSize _maintab = (ILayoutPositionableElementWithActualSize)(this);
            int unitwidth = GlobalSetting.LadderWidthUnit;
            int unitnumber = GlobalSetting.LadderXCapacity;
            
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
                if (tab.IsFloat)
                    continue;
                if (!_lDocDict.ContainsKey(tab))
                    continue;
                LayoutDocument ldoc = _lDocDict[tab];
                if (!Children.Contains(ldoc))
                {
                    CloseItem(tab);
                    return;
                }
            }
            foreach (var child in Children)
            {
                ITabItem tab = null;
                if (child.Content is MainTabDiagramItem)
                {
                    tab = ((MainTabDiagramItem)(child.Content)).LDVM_ladder;
                }
                else if (child.Content is ITabItem)
                {
                    tab = (ITabItem)(child.Content);
                }
                if (tab != null && !TabItemCollection.Contains(tab))
                {
                    TabItemCollection.Add(tab);
                    _lDocDict.Add(tab, (LayoutDocument)child);
                }
                if (child.IconSource == null && child.Content is MainTabDiagramItem)
                {
                    child.ImageSource = IconManager.RoutineImage;
                }
                else if (child.IconSource == null && child.Content is FuncBlockViewModel)
                {
                    child.ImageSource = IconManager.FuncImage;
                }
                else if (child.IconSource == null && child.Content is ModbusTableViewModel)
                {
                    child.ImageSource = IconManager.ModbusImage;
                }
            }
        }
        
        private void OnTabFloatClosed(object sender, RoutedEventArgs e)
        {
            if (sender is MainTabDiagramItem)
            {
                MainTabDiagramItem mtditem = (MainTabDiagramItem)sender;
                mtditem.IsFloat = false;
                //mtditem.FloatClosed -= OnTabFloatClosed;
                DiagramCollection.Remove(mtditem);
                TabItemCollection.Remove(mtditem.LDVM_ladder);
                _lDocDict.Remove(mtditem.LDVM_ladder);
            }
            else
            {
                ITabItem titem = (ITabItem)sender;
                titem.IsFloat = false;
                //titem.FloatClosed -= OnTabFloatClosed;
                TabItemCollection.Remove(titem);
                _lDocDict.Remove(titem);
            }
            FloatingWinClosed(this,null);
        }

        #endregion
    }
}

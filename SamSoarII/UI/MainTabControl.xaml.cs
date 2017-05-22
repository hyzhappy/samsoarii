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
using System.Windows.Media.Imaging;
using SamSoarII.AppMain.UI.Style;

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
            if (item is FuncBlockViewModel)
            {
                FuncBlockViewModel fbvmodel = (FuncBlockViewModel)item;
                fbvmodel.CodeTextBox.Focus();
            }
            if (!TabItemCollection.Contains(item))
            {
                TabItemCollection.Add(item);
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
                LayoutDocument ldoc = _lDocDict[item];
                Children.Remove(ldoc);
                _lDocDict.Remove(item);
                TabItemCollection.Remove(item);
                /*
                if (item is LadderDiagramViewModel)
                {
                    MainTabDiagramItem[] fit = DiagramCollection.Where(
                        (MainTabDiagramItem mtditem) => { return mtditem.LDVM_ladder == item; }).ToArray();
                    foreach (MainTabDiagramItem mtditem in fit)
                    {
                        DiagramCollection.Remove(mtditem);
                    }
                }
                */
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
            foreach (var child in Children)
            {
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

        private void OnTabGotFocus(object sender, RoutedEventArgs e)
        {
            GotFocus(this, e);
        }

        #endregion
    }
}

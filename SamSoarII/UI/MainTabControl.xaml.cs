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

namespace SamSoarII.AppMain.UI
{
    public partial class MainTabControl : LayoutDocumentPane
    {
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
        
        public ObservableCollection<ITabItem> TabItemCollection { get; set; } = new ObservableCollection<ITabItem>();
        public ObservableCollection<MainTabDiagramItem> DiagramCollection { get; set; } = new ObservableCollection<MainTabDiagramItem>();
        private Dictionary<ITabItem, LayoutDocument> _lDocDict = new Dictionary<ITabItem, LayoutDocument>();
        public ITabItem SelectedItem = null;

        public ITabItem CurrentTab
        {
            get
            {
                return SelectedItem;
            }
        }

        public MainTabControl()
        {
            ViewMode = VIEWMODE_LADDER;
        }

        public void Reset()
        {
            TabItemCollection.Clear();
        }

        public void ShowItem(ITabItem item)
        {
            LayoutDocument ldoc = null;
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
            LayoutDocument ldoc = _lDocDict[item];
            Children.Remove(ldoc);
            _lDocDict.Remove(item);
            TabItemCollection.Remove(item);
        }
        
        public SelectionChangedEventHandler SelectionChanged;
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
            GlobalSetting.LadderOriginScaleX = _maintab.ActualWidth / (unitwidth * unitnumber);
            GlobalSetting.LadderOriginScaleY = _maintab.ActualWidth / (unitwidth * unitnumber);
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
    }
}

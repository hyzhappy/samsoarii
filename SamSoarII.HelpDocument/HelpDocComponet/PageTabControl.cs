using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SamSoarII.HelpDocument.HelpDocComponet
{
    public class PageTabControlStatusChangedEventArgs : EventArgs
    {
        public Tuple<int, List<int>> OldStatus { get; set; }
        public PageTabControlStatusChangedEventArgs(Tuple<int, List<int>> oldStatus)
        {
            OldStatus = oldStatus;
        }
    }
    public delegate void PageTabControlStatusChangedHandler(PageTabControlStatusChangedEventArgs e);
    public class PageTabControl : TabControl
    {
        public event PageTabControlStatusChangedHandler PageTabControlStatusChanged = delegate { };
        public ObservableCollection<HelpDocFrame> TabItemCollection { get; set; } = new ObservableCollection<HelpDocFrame>();
        private Tuple<int, List<int>> oldStatus = null;
        public PageTabControl()
        {
            this.DataContext = this;
            Focusable = true;
        }
        public void Reset()
        {
            TabItemCollection.Clear();
        }
        public void InitializeTabItemCollection(HelpDocFrame item)
        {
            TabItemCollection.Add(item);
            item.IsUsed = true;
            SelectedItem = item;
        }
        public void ShowItem(HelpDocFrame item)
        {
            oldStatus = GetCurrentStatus();
            if (!TabItemCollection.Contains(item))
            {
                TabItemCollection.Add(item);
                item.IsUsed = true;
            }
            if (SelectedItem != item)
            {
                SelectedItem = item;
                PageTabControlStatusChanged.Invoke(new PageTabControlStatusChangedEventArgs(oldStatus));
            }
        }
        public Tuple<int, List<int>> GetCurrentStatus()
        {
            int selectindex;
            if (SelectedItem == null)
            {
                selectindex = 0;
            }
            else
            {
                selectindex = (SelectedItem as HelpDocFrame).PageIndex;
            }
            List<int> list = new List<int>();
            foreach (var item in TabItemCollection)
            {
                list.Add(item.PageIndex);
            }
            return new Tuple<int, List<int>>(selectindex, list);
        }
        public void CloseItem(HelpDocFrame item)
        {
            if (TabItemCollection.Contains(item))
            {
                oldStatus = GetCurrentStatus();
                if (item == SelectedItem)
                {
                    SelectedIndex = 0;
                }
                TabItemCollection.Remove(item);
                item.IsUsed = false;
                PageTabControlStatusChanged.Invoke(new PageTabControlStatusChangedEventArgs(oldStatus));
            }
        }
        public void ChangeToStatus(Tuple<int, List<int>> oldStatus, PageManager _pageManager)
        {
            foreach (var item in TabItemCollection)
            {
                item.IsUsed = false;
            }
            if (oldStatus.Item2.Count == 0)
            {
                SelectedItem = null;
                TabItemCollection.Clear();
            }
            else
            {
                foreach (var index in oldStatus.Item2)
                {
                    var item = _pageManager.GetFrameByPageIndex(index);
                    if (!TabItemCollection.Contains(item))
                    {
                        TabItemCollection.Add(item);
                    }
                    item.IsUsed = true;
                    if (index == oldStatus.Item1)
                    {
                        SelectedItem = item;
                    }
                }
                foreach (var item in new List<HelpDocFrame>(TabItemCollection))
                {
                    if (!item.IsUsed)
                    {
                        TabItemCollection.Remove(item);
                    }
                }
            }
        }
    }
}

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

namespace SamSoarII.AppMain.UI
{
    public class MainTabControl : TabControl
    {
        public ObservableCollection<ITabItem> TabItemCollection { get; set; } = new ObservableCollection<ITabItem>();

        public TabItem CurrentTab
        {
            get
            {
                return SelectedItem as TabItem;
            }
        }


        public MainTabControl()
        {
            this.DataContext = this;
            Focusable = true;
        }

        public void Reset()
        {
            TabItemCollection.Clear();
        }

        public void ShowItem(ITabItem item)
        {
            if(!TabItemCollection.Contains(item))
            {
                TabItemCollection.Add(item);
            }
            SelectedItem = item;
        }

        public void CloseItem(ITabItem item)
        {
            if(TabItemCollection.Contains(item))
            {
                if(item == SelectedItem)
                {
                    SelectedIndex = 0;
                }
                TabItemCollection.Remove(item);
            }
        }
    }
}

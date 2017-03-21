using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using SamSoarII.Simulation.Shell;

namespace SamSoarII.Simulation.UI
{
    /// <summary>
    /// MainTabControl.xaml 的交互逻辑
    /// </summary>
    public partial class MainTabControl : TabControl
    {
        public MainTabControl()
        {
            InitializeComponent();
        }

        public TabItem Current
        {
            get
            {
                return SelectedItem as TabItem;
            }
        }

        public void Reset()
        {
            this.Items.Clear();
        }

        public TabItem GetTabByName(string name)
        {
            foreach (var tab in Items.OfType<TabItem>())
            {
                if (tab.Header.ToString() == name)
                {
                    return tab;
                }
            }
            return null;
        }

        public void ShowItem(UserControl content, string name)
        {
            var tab = GetTabByName(name);
            if (tab == null)
            {
                tab = new TabItem();
                tab.Content = content;
                tab.Header = name;
                Items.Add(tab);
                this.SelectedItem = tab;
            }
            else
            {
                if (SelectedItem != tab)
                {
                    SelectedItem = tab;
                }
            }
        }

        public void CloseItem(string tabName)
        {
            var tab = GetTabByName(tabName);
            if (tab != null)
            {
                CloseItem(tab);
            }
        }

        public void CloseItem(TabItem tabItem)
        {
            if (tabItem == SelectedItem)
            {
                SelectedIndex = 0;
                Items.Remove(tabItem);
            }
            else
            {
                Items.Remove(tabItem);
            }
        }

        private void RemoveTabItem(TabItem tabItem)
        {
            //tabItem.MouseDown -= TabItem_MouseDown;
            //tabItem.ClearElements();
            //Items.Remove(tabItem);
        }

        #region Event Handler
        private void OnTabItemHeaderCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                TabItem tabitem = button.TemplatedParent as TabItem;
                if (tabitem != null)
                {
                    CloseTabItem(tabitem);
                }
            }
        }

        private void OnTabItemHeaderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Grid grid = sender as Grid;
                if (grid != null)
                {
                    TabItem tabitem = grid.TemplatedParent as TabItem;
                    if (tabitem != null)
                    {
                        CloseTabItem(tabitem);
                    }
                }
            }
        }
        #endregion

        private void CloseTabItem(TabItem tabitem)
        {
            if (Items.Contains(tabitem))
            {
                Items.Remove(tabitem);
            }
        }

    }
}

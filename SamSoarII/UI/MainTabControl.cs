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
namespace SamSoarII.AppMain.UI
{
    public class MainTabControl : TabControl
    {
        public TabItem CurrentTab
        {
            get
            {
                return SelectedItem as TabItem;
            }
        }

        public MainTabControl()
        {
            Focusable = true;
        }

        public void Reset()
        {
            this.Items.Clear();
        }
        public TabItem GetTabByName(string name)
        {
            foreach (var tab in Items.OfType<TabItem>())
            {
                if(tab.Header.ToString() == name)
                {
                    return tab;
                }
            }
            return null;
        }

        public void ShowItem(LadderDiagramViewModel ldmodel)
        {
            var tab = GetTabByName(ldmodel.LadderName);
            if(tab == null)
            {
                tab = new TabItem();
                tab.Content = ldmodel;
                tab.Header = ldmodel.LadderName;
                Items.Add(tab);
                this.SelectedItem = tab;
            }
            else
            {
                if(SelectedItem != tab)
                {
                    SelectedItem = tab;
                }
            }
        }

        public void ShowItem(FuncBlockViewModel fbmodel)
        {
            var tab = GetTabByName(fbmodel.FuncBlockName);
            if (tab == null)
            {
                tab = new TabItem();
                tab.Content = fbmodel;
                tab.Header = fbmodel.FuncBlockName;
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

        #region Event handler
        private void TabItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // close when middle mouse button down
            //if (e.MiddleButton == System.Windows.Input.MouseButtonState.Pressed)
            //{
            //    var tabItem = sender as TabItem;
            //    CloseItem(tabItem);
            //}
        }
        #endregion


    }
}

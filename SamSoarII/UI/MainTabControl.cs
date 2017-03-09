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
        private TabItem _variableListTabItem;

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
            _variableListTabItem = new TabItem();
            _variableListTabItem.Header = "元件变量表";
            _variableListTabItem.Content = new VariableListControl();
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
        
        public void ShowVariableList()
        {
            if(!Items.Contains(_variableListTabItem))
            {
                Items.Add(_variableListTabItem);
            }
            this.SelectedItem = _variableListTabItem;
        }

        public void UpdateVariableCollection()
        {
            var varlistcontrol = _variableListTabItem.Content as VariableListControl;
            varlistcontrol.UpdateVariableCollection();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SamSoarII.Project;
namespace SamSoarII.AppMain.UI
{
    public class LadderTabControl : TabControl
    {
        public ProjectModel CurrentProject { get; set; }

        public ScaleTransform Scale { get; set; }

        public LadderTabControl()
        {

        }

        public LadderTabItem GetTabByName(string name)
        {
            foreach (var tab in Items.OfType<LadderTabItem>())
            {
                if(tab.TabName == name)
                {
                    return tab;
                }
            }
            return null;
        }

        public void ShowItem(string name)
        {
            LadderTabItem tabItem = GetTabByName(name);
            if(tabItem == null)
            {
                var routine = CurrentProject.GetRoutineByName(name);
                if(routine != null)
                {
                    tabItem = new LadderTabItem(routine, Scale);
                    SetCurrentItem(tabItem);
                }
            }
            else
            {
                SetCurrentItem(tabItem);
            }    
        }

        public void ShowItem(LadderDiagramModel ldmodel)
        {
            ShowItem(ldmodel.Name);
        }

        private void AddLadderTabItem(LadderTabItem tabItem)
        {
            tabItem.MouseDown += TabItem_MouseDown;
            Items.Add(tabItem);
        }

        private void RemoveLadderTabItem(LadderTabItem tabItem)
        {
            tabItem.MouseDown -= TabItem_MouseDown;
            tabItem.ClearElements();
            Items.Remove(tabItem);
        }

        private void TabItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.MiddleButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                LadderTabItem tabItem = sender as LadderTabItem;
                RemoveLadderTabItem(tabItem);
            }      
        }

        private void SetCurrentItem(LadderTabItem tabItem)
        {
            if(!Items.Contains(tabItem))
            {
                AddLadderTabItem(tabItem);
            }
            SelectedItem = tabItem;
            tabItem.ItemGetKeyboardFocus();
        }
    }
}

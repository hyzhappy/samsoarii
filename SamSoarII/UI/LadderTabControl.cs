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

        public BaseTabItem GetTabByName(string name)
        {
            foreach (var tab in Items.OfType<BaseTabItem>())
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
            BaseTabItem tabItem = GetTabByName(name);
            if(tabItem == null)
            {
                var routine = CurrentProject.GetRoutineByName(name);
                if(routine != null)
                {
                    tabItem = new LadderTabItem(routine, Scale);
                    SetCurrentItem(tabItem);
                }
                else
                {
                    var fbmodel = CurrentProject.GetFuncBlockByName(name);
                    if(fbmodel != null)
                    {
                        tabItem = new FuncBlockTabItem(fbmodel);
                        SetCurrentItem(tabItem);
                    }
                }
            }
            else
            {
                SetCurrentItem(tabItem);
            }    
        }

        public void RemoveTabItem(string name)
        {
            BaseTabItem tabItem = GetTabByName(name);
            if(tabItem != null)
            {
                RemoveTabItem(tabItem);
            }
        }

        public void ShowItem(LadderDiagramModel ldmodel)
        {
            ShowItem(ldmodel.Name);
        }

        public void ShowItem(FuncBlockModel fbmodel)
        {
            ShowItem(fbmodel.Name);
        }

        private void AddTabItem(BaseTabItem tabItem)
        {
            tabItem.MouseDown += TabItem_MouseDown;
            Items.Add(tabItem);
        }

        private void RemoveTabItem(BaseTabItem tabItem)
        {
            tabItem.MouseDown -= TabItem_MouseDown;
            tabItem.ClearElements();
            Items.Remove(tabItem);
        }

        private void TabItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // close when middle mouse button down
            if(e.MiddleButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                var tabItem = sender as BaseTabItem;
                RemoveTabItem(tabItem);
            }      
        }

        private void SetCurrentItem(BaseTabItem tabItem)
        {
            if(!Items.Contains(tabItem))
            {
                AddTabItem(tabItem);
            }
            SelectedItem = tabItem;
            tabItem.OnItemSelected();
        }
    }
}

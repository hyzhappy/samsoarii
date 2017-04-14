using SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SamSoarII.AppMain.UI.HelpDocComponet
{
    public static class FavoriteManager
    {
        public static ObservableCollection<HelpDocFrame> TabItemCollection { get; set; }
        static FavoriteManager()
        {
            TabItemCollection = new ObservableCollection<HelpDocFrame>();
        }
        public static void CollectPage(HelpDocFrame page)
        {
            foreach (var item in TabItemCollection)
            {
                if (item.PageIndex == page.PageIndex)
                {
                    return;
                }
            }
            TabItemCollection.Add(page);
        }
        public static void DeletePage(HelpDocFrame page)
        {
            foreach (var item in new List<HelpDocFrame>(TabItemCollection))
            {
                if (item.PageIndex == page.PageIndex)
                {
                    TabItemCollection.Remove(item);
                }
            }
        }
        public static void DeleteAllPages()
        {
            TabItemCollection.Clear();
        }
    }
}

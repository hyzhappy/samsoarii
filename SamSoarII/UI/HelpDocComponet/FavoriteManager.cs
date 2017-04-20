using SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

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
        public static void LoadFavoritePagesByXElement(XElement rootNode)
        {
            int pageindex;
            foreach (var ele in rootNode.Elements())
            {
                pageindex = int.Parse(ele.Value);
                TabItemCollection.Add(PageManager.PageCollection[pageindex]);
            }
        }
        public static XElement CreateXElementByPageIndex()
        {
            XElement rootNode = new XElement("FavoritePages");
            foreach (var item in TabItemCollection)
            {
                rootNode.Add(new XElement("Page",item.PageIndex));
            }
            return rootNode;
        }
    }
}

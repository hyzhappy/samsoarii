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

namespace SamSoarII.AppMain.UI.HelpDocComponet
{
    /// <summary>
    /// HelpDocTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class HelpDocTreeView : UserControl
    {
        public event RoutedEventHandler SelectEventHandler = delegate { };
        public event MouseButtonEventHandler MouseDoubleClickEventHandler = delegate { };
        public HelpDocTreeView()
        {
            InitializeComponent();
        }
        private HelpDocTreeItem oldItem = null;
        private void OnOpenAll(object sender, RoutedEventArgs e)
        {
            foreach (var item in HelpDocTree.Items)
            {
                OpenItem(item as HelpDocTreeItem);
            }
        }
        private void OpenItem(HelpDocTreeItem item)
        {
            item.ExpandSubtree();
        }
        private void OnCloseAll(object sender, RoutedEventArgs e)
        {
            foreach (var item in HelpDocTree.Items)
            {
                CloseItem(item as HelpDocTreeItem);
            }
        }
        private void CloseItem(HelpDocTreeItem item)
        {
            foreach (var subitem in item.Items)
            {
                CloseItem(subitem as HelpDocTreeItem);
            }
            if (item.IsExpanded)
            {
                item.IsExpanded = false;
            }
        }
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HelpDocTreeItem item = sender as HelpDocTreeItem;
            if (item == null || !item.IsSelected)
            {
                return;
            }
            else
            {
                MouseDoubleClickEventHandler.Invoke(sender, e);
            }
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HelpDocTreeItem item = HelpDocTree.SelectedItem as HelpDocTreeItem;
                item.IsExpanded = !item.IsExpanded;
            }
        }
        private void OnOpen(object sender, RoutedEventArgs e)
        {
            if (HelpDocTree.SelectedItem != null)
            {
                HelpDocTreeItem item = HelpDocTree.SelectedItem as HelpDocTreeItem;
                item.IsExpanded = true;
            }
        }
        private void OnClose(object sender, RoutedEventArgs e)
        {
            if (HelpDocTree.SelectedItem != null)
            {
                HelpDocTreeItem item = HelpDocTree.SelectedItem as HelpDocTreeItem;
                item.IsExpanded = false;
            }
        }
    }
}

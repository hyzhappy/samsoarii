using SamSoarII.HelpDocument.HelpDocComponet.HelpDocPages;
using SamSoarII.Utility;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace SamSoarII.HelpDocument.HelpDocComponet
{
    /// <summary>
    /// HelpDocSearch.xaml 的交互逻辑
    /// </summary>
    public partial class HelpDocSearch : UserControl, INotifyPropertyChanged
    {
        public IEnumerable<HelpDocFrame> PageCollection
        {
            get
            {
                List<HelpDocFrame> templist = PageManager.PageCollection.Values.ToList();
                if (SearchTextBox.Text != string.Empty)
                {
                    templist = templist.Where(x => { return x.TabHeader.ToUpper().Contains(SearchTextBox.Text.ToUpper()); }).ToList();
                    templist.Sort((page1, page2) =>
                    {
                        int index1 = page1.TabHeader.ToUpper().IndexOf(SearchTextBox.Text.ToUpper());
                        int index2 = page2.TabHeader.ToUpper().IndexOf(SearchTextBox.Text.ToUpper());
                        int first = StringHelper.Compare(page1.TabHeader.ToUpper(), index1);
                        int second = StringHelper.Compare(page2.TabHeader.ToUpper(), index2);
                        if (first == second)
                        {
                            string temp1 = page1.TabHeader.ToUpper().Substring(index1 + SearchTextBox.Text.Length);
                            string temp2 = page2.TabHeader.ToUpper().Substring(index1 + SearchTextBox.Text.Length);
                            return StringHelper.Compare(temp1, temp2);
                        }
                        return first - second;
                    });
                }
                return templist;
            }
        }
        public HelpDocSearch()
        {
            InitializeComponent();
            DataContext = this;
        }
        public HelpDocFrame CurrentItem
        {
            get
            {
                return CollectList.SelectedItem as HelpDocFrame;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event MouseButtonEventHandler ItemDoubleClick = delegate { };
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("PageCollection"));
        }

        private void OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemDoubleClick.Invoke(sender, e);
        }
    }
}

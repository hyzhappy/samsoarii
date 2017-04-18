using SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// HelpDocSearch.xaml 的交互逻辑
    /// </summary>
    public partial class HelpDocSearch : UserControl,INotifyPropertyChanged
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
                            return StringHelper.Compare(temp1,temp2);
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event MouseButtonEventHandler ItemDoubleClick = delegate { };
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            PropertyChanged.Invoke(this,new PropertyChangedEventArgs("PageCollection"));
        }

        private void OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemDoubleClick.Invoke(sender,e);
        }
    }
}

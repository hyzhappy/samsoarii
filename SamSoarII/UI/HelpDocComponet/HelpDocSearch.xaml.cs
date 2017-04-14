using SamSoarII.AppMain.UI.HelpDocComponet.HelpDocPages;
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

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            PropertyChanged.Invoke(this,new PropertyChangedEventArgs("PageCollection"));
        }
    }
}

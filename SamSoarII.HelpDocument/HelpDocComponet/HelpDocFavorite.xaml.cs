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

namespace SamSoarII.HelpDocument.HelpDocComponet
{
    /// <summary>
    /// HelpDocFavorite.xaml 的交互逻辑
    /// </summary>
    public partial class HelpDocFavorite : UserControl
    {
        public HelpDocFavorite()
        {
            InitializeComponent();
            DataContext = FavoriteManager.TabItemCollection;
        }
        public event MouseButtonEventHandler ItemDoubleClick = delegate { };
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemDoubleClick.Invoke(sender, e);
        }
    }
}

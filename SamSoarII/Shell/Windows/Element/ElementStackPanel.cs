using SamSoarII.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SamSoarII.Shell.Windows
{
    public class ElementStackPanel : StackPanel
    {
        public static readonly DependencyProperty ItemsSourceProperty;
        static ElementStackPanel()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
            metadata.PropertyChangedCallback += OnItemsSourcePropertyChanged;
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<LadderUnitModel>), typeof(ElementStackPanel), metadata);

        }
        public IEnumerable<LadderUnitModel> ItemsSource
        {
            get
            {
                return (IEnumerable<LadderUnitModel>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        //private static List<TextBlock> textblocks = new List<TextBlock>();
        public static void OnItemsSourcePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ElementStackPanel collectionStackPanel = source as ElementStackPanel;
            IEnumerable<LadderUnitModel> units = collectionStackPanel.ItemsSource;
            collectionStackPanel.Children.Clear();
            foreach (LadderUnitModel unit in units)
            {
                TextBlock tb = new TextBlock();
                tb.Text = unit.ToString();
                collectionStackPanel.Children.Add(tb);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SamSoarII.AppMain.UI
{
    class ELementStackPanel : StackPanel
    {
        public static readonly DependencyProperty ItemsSourceProperty;
        static ELementStackPanel()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
            metadata.PropertyChangedCallback += OnItemsSourcePropertyChanged;
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<TextBlock>), typeof(ELementStackPanel), metadata);
            
        }
        public IEnumerable<TextBlock> ItemsSource
        {
            get
            {
                return (IEnumerable<TextBlock>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
        public static void OnItemsSourcePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ELementStackPanel collectionStackPanel = source as ELementStackPanel;
            collectionStackPanel.Children.Clear();
            foreach (var item in collectionStackPanel.ItemsSource)
            {
                var parent = LogicalTreeHelper.GetParent(item);
                if (parent is ELementStackPanel)
                {
                    (parent as ELementStackPanel).Children.Remove(item);
                }
                collectionStackPanel.Children.Add(item);
            }
        }
    }
}

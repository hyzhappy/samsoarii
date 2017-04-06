using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SamSoarII.UserInterface
{
    class CollectionStackPanel : StackPanel
    {
        public static readonly DependencyProperty ItemsSourceProperty;
        public static readonly DependencyProperty SelectItemProperty;
        static CollectionStackPanel()
        {
            FrameworkPropertyMetadata metadata1 = new FrameworkPropertyMetadata();
            metadata1.PropertyChangedCallback += OnItemsSourcePropertyChanged;
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource",typeof(IEnumerable<Label>),typeof(CollectionStackPanel),metadata1);
            FrameworkPropertyMetadata metadata2 = new FrameworkPropertyMetadata();
            metadata2.PropertyChangedCallback += OnSelectItemPropertyChanged;
            SelectItemProperty = DependencyProperty.Register("SelectItem",typeof(Label),typeof(CollectionStackPanel),metadata2);
        }
        public IEnumerable<Label> ItemsSource
        {
            get
            {
                return (IEnumerable<Label>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
        public Label SelectItem
        {
            get
            {
                return (Label)GetValue(SelectItemProperty);
            }
            set
            {
                SetValue(SelectItemProperty, value);
            }
        }
        public static void OnItemsSourcePropertyChanged(DependencyObject source,DependencyPropertyChangedEventArgs args)
        {
            CollectionStackPanel collectionStackPanel = source as CollectionStackPanel;
            collectionStackPanel.Children.Clear();
            foreach (var item in collectionStackPanel.ItemsSource)
            {
                collectionStackPanel.Children.Add(item);
            }
        }
        public static void OnSelectItemPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            CollectionStackPanel collectionStackPanel = source as CollectionStackPanel;
            if (collectionStackPanel.SelectItem != null)
            {
                Label lable = collectionStackPanel.SelectItem;
                Color color = new Color();
                color.A = 255;
                color.R = 26;
                color.G = 134;
                color.B = 243;
                lable.Background = new SolidColorBrush(color);
                color.A = 255;
                color.R = 255;
                color.G = 255;
                color.B = 255;
                lable.Foreground = new SolidColorBrush(color);
                lable.FontWeight = FontWeights.Heavy;
            }
        }
    }
}

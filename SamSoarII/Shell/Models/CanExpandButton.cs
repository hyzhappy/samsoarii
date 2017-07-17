using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SamSoarII.Shell.Models
{
    public class CanExpandButton : Button
    {
        public event RoutedEventHandler IsExpandChanged;
        public static readonly DependencyProperty IsExpandProperty;
        static CanExpandButton()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
            metadata.PropertyChangedCallback += OnIsExpandPropertyChanged;
            IsExpandProperty = DependencyProperty.Register("IsExpand", typeof(bool), typeof(CanExpandButton), metadata);
        }
        public static void OnIsExpandPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (((CanExpandButton)source).IsExpandChanged != null)
            {
                ((CanExpandButton)source).IsExpandChanged.Invoke(source, new RoutedEventArgs());
            }
        }
        public bool IsExpand
        {
            get
            {
                return (bool)GetValue(IsExpandProperty);
            }
            set
            {
                SetValue(IsExpandProperty, value);
            }
        }
        public CanExpandButton()
        {
            PreviewMouseDown += CanExpandButton_PreviewMouseDown;
        }
        private void CanExpandButton_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsExpand = !IsExpand;
        }
    }
}

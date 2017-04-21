using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SamSoarII.AppMain.UI.HelpDocComponet
{
    public class CanAnimationScroll : ScrollViewer
    {
        public static readonly DependencyProperty CanChangeHorizontalOffsetProperty;
        public static event PropertyChangedCallback PropertyChangedEvent = delegate { };
        static CanAnimationScroll()
        {
            PropertyChangedEvent += CanAnimationScroll_PropertyChangedEvent;
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
            metadata.IsAnimationProhibited = false;
            metadata.PropertyChangedCallback = PropertyChangedEvent;
            CanChangeHorizontalOffsetProperty = DependencyProperty.Register("CanChangeHorizontalOffset",typeof(double),typeof(CanAnimationScroll),metadata);
        }
        private static void CanAnimationScroll_PropertyChangedEvent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CanAnimationScroll).ScrollToHorizontalOffset((double)e.NewValue);
        }
        public double CanChangeHorizontalOffset
        {
            get
            {
                return (double)GetValue(HorizontalOffsetProperty);
            }
            set
            {
                SetValue(CanChangeHorizontalOffsetProperty,value);
            }
        }
    }
}

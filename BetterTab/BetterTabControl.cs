using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BetterTab.Components;
using BetterWpfControls.Components;
using BetterWpfControls;
using BetterTab.Converters;
using System.ComponentModel;

namespace BetterTab
{
    [TemplatePart(Name = "PART_SelectedContentHost", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_HeaderPanel", Type = typeof(BetterTabPanel))]
    [TemplatePart(Name = "PART_QuickLinksHost", Type = typeof(SplitButton))]
    public class BetterTabControl : TabControl
    {
        #region .ctors

        static BetterTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BetterTabControl), new FrameworkPropertyMetadata(typeof(BetterTabControl)));
        }

        public BetterTabControl()
        {
            NavigateToItemCommand = new DelegateCommand<object>(NavigateToItem);
        }

        #endregion .ctors

        #region Properties

        #region ScrollToLeftContent

        public object ScrollToLeftContent
        {
            get { return (object)GetValue(ScrollToLeftContentProperty); }
            set { SetValue(ScrollToLeftContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToLeftContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToLeftContentProperty =
            DependencyProperty.Register("ScrollToLeftContent", typeof(object), typeof(BetterTabControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion ScrollToLeftContent

        #region ScrollToRightContent

        public object ScrollToRightContent
        {
            get { return (object)GetValue(ScrollToRightContentProperty); }
            set { SetValue(ScrollToRightContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToRightContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToRightContentProperty =
            DependencyProperty.Register("ScrollToRightContent", typeof(object), typeof(BetterTabControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion ScrollToRightContent

        #region NavigateToItemCommand

        public ICommand NavigateToItemCommand
        {
            get { return (ICommand)GetValue(NavigateToItemCommandProperty); }
            set { SetValue(NavigateToItemCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigateToItemCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigateToItemCommandProperty =
            DependencyProperty.Register("NavigateToItemCommand", typeof(ICommand), typeof(BetterTabControl), new UIPropertyMetadata(null));

        #endregion NavigateToItemCommand

        #region ShowQuickLinksButton

        public bool ShowQuickLinksButton
        {
            get { return (bool)GetValue(ShowQuickLinksButtonProperty); }
            set { SetValue(ShowQuickLinksButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowQuickLinksButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowQuickLinksButtonProperty =
            DependencyProperty.Register("ShowQuickLinksButton", typeof(bool), typeof(BetterTabControl), new UIPropertyMetadata(true));

        #endregion ShowQuickLinksButton

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var tp = Template.FindName("PART_HeaderPanel", this) as BetterTabPanel;
            if (tp != null)
            {
                tp.SetBinding(BetterTabPanel.SelectedItemProperty, new Binding(SelectedItemProperty.Name) { Mode = BindingMode.TwoWay, Source = this });
                tp.SetBinding(BetterTabPanel.ScrollToLeftContentProperty, new Binding(ScrollToLeftContentProperty.Name) { Mode = BindingMode.TwoWay, Source = this });
                tp.SetBinding(BetterTabPanel.ScrollToRightContentProperty, new Binding(ScrollToRightContentProperty.Name) { Mode = BindingMode.TwoWay, Source = this });
            }
            var sb = Template.FindName("PART_QuickLinksHost", this) as SplitButton;
            if (sb != null)
            {
                if (tp != null)
                {
                    var b = new MultiBinding() { Converter = new BoolToVisibilityConverter() };
                    b.Bindings.Add(new Binding(BetterTabPanel.IsTruncatingItemsProperty.Name) { Source = tp, Mode = BindingMode.OneWay });
                    b.Bindings.Add(new Binding(BetterTabControl.ShowQuickLinksButtonProperty.Name) { Source = this, Mode = BindingMode.OneWay });
                    sb.SetBinding(FrameworkElement.VisibilityProperty, b);
                }
                sb.SetBinding(SplitButton.ItemsSourceProperty, new Binding("Items") { Mode = BindingMode.OneWay, Source = this });
                sb.SetBinding(SplitButton.CommandProperty, new Binding(NavigateToItemCommandProperty.Name) { Mode = BindingMode.OneWay, Source = this });
                if (sb.ItemTemplate == null && sb.GetBindingExpression(SplitButton.ItemTemplateProperty) == null)
                {
                    var factory = new FrameworkElementFactory(typeof(ContentPresenter));
                    factory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                    factory.SetBinding(ContentPresenter.ContentProperty, new Binding("TabHeader") { Converter = new WrapVisualConverter() });
                    sb.ItemTemplate = new DataTemplate() { VisualTree = factory };
                }
            }
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TabItem;
        }

        private void NavigateToItem(object item)
        {
            SelectedItem = item;
        }

        #endregion Methods
    }
}

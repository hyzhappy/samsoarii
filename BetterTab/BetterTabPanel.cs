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

namespace BetterTab
{
    public class BetterTabPanel : Panel
    {
		#region .ctors

        public BetterTabPanel()
		{
            AddVisualChild(_btnScrollLeft = new ContentPresenter() { });
            AddVisualChild(_btnScrollRight = new ContentPresenter() { });
            ScrollToLeftCommand = new DelegateCommand(ScrollToLeft, () => HorizontalOffsetToSelectedItem != 0.0);
            ScrollToRightCommand = new DelegateCommand(ScrollToRight);
            this.Loaded += BetterTabPanel_Loaded;
            //ScrollToRightCommand = new DelegateCommand(ScrollToRight, () =>
            //    {
            //        var viewportLeft = GetViewportLeft();
            //        var viewportRight = GetViewportRight();
            //        var x = GetViewportLeft() + InternalChildren.OfType<UIElement>().Sum(e => e.DesiredSize.Width);
            //        return x + HorizontalOffsetToSelectedItem > viewportRight && Math.Abs(x + HorizontalOffsetToSelectedItem - viewportRight) > 0.0001;
            //    });
        }

        private void BetterTabPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetEndIndex(InternalChildren.OfType<TabItem>().ToList());
        }

        private bool toleft = false;
        private bool toright = false;
        private int startindex = 0;
        private int endindex = 0;
        private BetterTabControl Father;
        private void BetterTabPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Father = GetFather();
            SizeChanged += BetterTabPanel_SizeChanged;
        }

        private BetterTabControl GetFather()
        {
            var father = VisualTreeHelper.GetParent(this);
            while (!(father is BetterTabControl))
            {
                father = VisualTreeHelper.GetParent(father);
            }
            return (BetterTabControl)father;
        }
		#endregion .ctors

		#region Fields

        private ContentPresenter _btnScrollLeft;
        private ContentPresenter _btnScrollRight;

		#endregion Fields

		#region Properties

		#region IsLocked

		public static bool GetIsLocked(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsLockedProperty);
		}

		public static void SetIsLocked(DependencyObject obj, bool value)
		{
			obj.SetValue(IsLockedProperty, value);
		}

		// Using a DependencyProperty as the backing store for IsLocked.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.RegisterAttached("IsLocked", typeof(bool), typeof(BetterTabPanel), new UIPropertyMetadata(false));

		#endregion IsLocked

		#region MaxItemWidth

		public double MaxItemWidth
		{
			get { return (double)GetValue(MaxItemWidthProperty); }
			set { SetValue(MaxItemWidthProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MaxItemWidth.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MaxItemWidthProperty =
            DependencyProperty.Register("MaxItemWidth", typeof(double), typeof(BetterTabPanel), new UIPropertyMetadata(250.0));

		#endregion MaxItemWidth

		#region MinItemWidth

		public double MinItemWidth
		{
			get { return (double)GetValue(MinItemWidthProperty); }
			set { SetValue(MinItemWidthProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MinItemWidth.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MinItemWidthProperty =
            DependencyProperty.Register("MinItemWidth", typeof(double), typeof(BetterTabPanel), new UIPropertyMetadata(100.0));

		#endregion MinItemWidth

		#region SelectedItem

		public TabItem SelectedItem
		{
            get { return (TabItem)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(TabItem), typeof(BetterTabPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));

		#endregion SelectedItem

        #region HorizontalOffsetToSelectedItem

        public double HorizontalOffsetToSelectedItem
        {
            get { return (double)GetValue(HorizontalOffsetToSelectedItemProperty); }
            set { SetValue(HorizontalOffsetToSelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalOffsetToSelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalOffsetToSelectedItemProperty =
            DependencyProperty.Register("HorizontalOffsetToSelectedItem", typeof(double), typeof(BetterTabPanel), new UIPropertyMetadata(0.0, (s, e) =>
                {
                    var p = (BetterTabPanel)s;
                    if (p.ScrollToLeftCommand is DelegateCommand)
                    {
                        (p.ScrollToLeftCommand as DelegateCommand).RaiseCanExecuteChanged();
                    }
                    if (p.ScrollToRightCommand is DelegateCommand)
                    {
                        (p.ScrollToRightCommand as DelegateCommand).RaiseCanExecuteChanged();
                    }
                }));

        #endregion HorizontalOffsetToSelectedItem

        #region IsTruncatingItems

        public bool IsTruncatingItems
		{
			get { return (bool)GetValue(IsTruncatingItemsProperty); }
			set { SetValue(IsTruncatingItemsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsTruncatingItems.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsTruncatingItemsProperty =
            DependencyProperty.Register("IsTruncatingItems", typeof(bool), typeof(BetterTabPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

		#endregion IsTruncatingItems

		#region VisualChildrenCount

		protected override int VisualChildrenCount
		{
			get
			{
				return base.VisualChildrenCount + 2;
			}
		}

		#endregion VisualChildrenCount

        #region ScrollToLeftContent

        public object ScrollToLeftContent
        {
            get { return (object)GetValue(ScrollToLeftContentProperty); }
            set { SetValue(ScrollToLeftContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToLeftContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToLeftContentProperty =
            DependencyProperty.Register("ScrollToLeftContent", typeof(object), typeof(BetterTabPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (s, e) =>
                {
                    ((BetterTabPanel)s)._btnScrollLeft.Content = e.NewValue;
                }));

        #endregion ScrollToLeftContent

        #region ScrollToLeftCommand

        public ICommand ScrollToLeftCommand
        {
            get { return (ICommand)GetValue(ScrollToLeftCommandProperty); }
            set { SetValue(ScrollToLeftCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToLeftCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToLeftCommandProperty =
            DependencyProperty.Register("ScrollToLeftCommand", typeof(ICommand), typeof(BetterTabPanel), new UIPropertyMetadata(null));

        #endregion ScrollToLeftCommand

        #region ScrollToRightContent

        public object ScrollToRightContent
        {
            get { return (object)GetValue(ScrollToRightContentProperty); }
            set { SetValue(ScrollToRightContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToRightContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToRightContentProperty =
            DependencyProperty.Register("ScrollToRightContent", typeof(object), typeof(BetterTabPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (s, e) =>
                {
                    ((BetterTabPanel)s)._btnScrollRight.Content = e.NewValue;
                }));

        #endregion ScrollToRightContent

        #region ScrollToRightCommand

        public ICommand ScrollToRightCommand
        {
            get { return (ICommand)GetValue(ScrollToRightCommandProperty); }
            set { SetValue(ScrollToRightCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToRightCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToRightCommandProperty =
            DependencyProperty.Register("ScrollToRightCommand", typeof(ICommand), typeof(BetterTabPanel), new UIPropertyMetadata(null));

        #endregion ScrollToRightCommand

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}

		protected override Visual GetVisualChild(int index)
		{
			if (index == base.VisualChildrenCount)
				return _btnScrollLeft;
			if (index == base.VisualChildrenCount + 1)
				return _btnScrollRight;
			return base.GetVisualChild(index);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (_btnScrollLeft != null)
			{
				_btnScrollLeft.Measure(availableSize);
			}
			if (_btnScrollRight != null)
			{
				_btnScrollRight.Measure(availableSize);
			}

			foreach (var item in InternalChildren.OfType<UIElement>())
			{
				item.Measure(availableSize);
			}
			var totalWidth = InternalChildren.OfType<UIElement>().Sum(e => e.DesiredSize.Width);
            var isTruncatingItems = totalWidth > availableSize.Width;

            Dispatcher.BeginInvoke((Action)(() => IsTruncatingItems = isTruncatingItems));

			//IsTruncatingItems = totalWidth > availableSize.Width;
			return new Size(
				Math.Min(availableSize.Width, totalWidth),
                15);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var x = 0.0;

			foreach (var item in InternalChildren.OfType<UIElement>().Where(e => GetIsLocked(e)))
			{
				Panel.SetZIndex(item, int.MaxValue);
                item.Arrange(new Rect(new Point(x, 0), new Size(item.DesiredSize.Width, finalSize.Height)));
				x += item.DesiredSize.Width;
			}

			if (IsTruncatingItems)
			{
                x += _btnScrollLeft.DesiredSize.Width;
			}

            var viewportLeft = x;
            var viewportRight = finalSize.Width - (IsTruncatingItems ? _btnScrollRight.DesiredSize.Width : 0);

			var floatingItems = InternalChildren.OfType<UIElement>().Where(e => !GetIsLocked(e));
			if (floatingItems.Count() > 0)
			{
                UpdateHorizontalOffset(viewportLeft, viewportRight, floatingItems, SelectedItem as UIElement);

                EnumerateFloatingItems(floatingItems, viewportLeft, (xAdj, item) =>
                    {
                        ApplyMask(viewportLeft, viewportRight, item, xAdj);
                        item.Arrange(new Rect(new Point(xAdj, 0), new Size(item.DesiredSize.Width, finalSize.Height)));
                    });
			}

            ArrangeScroller(_btnScrollLeft, viewportLeft - _btnScrollLeft.DesiredSize.Width, finalSize.Height);
            ArrangeScroller(_btnScrollRight, finalSize.Width - _btnScrollRight.DesiredSize.Width, finalSize.Height);

			return base.ArrangeOverride(finalSize);
		}

        private void ArrangeScroller(UIElement el, double xAt, double height)
        {
            if (el != null)
            {
                if (IsTruncatingItems)
                {
                    Panel.SetZIndex(el, int.MaxValue);
                    el.Visibility = Visibility.Visible;
                    el.Arrange(new Rect(new Point(xAt, 0), new Size(el.DesiredSize.Width, height)));
                }
                else
                {
                    el.Visibility = Visibility.Collapsed;
                }
            }
        }

        private static void ApplyMask(double viewportLeft, double viewportRight, UIElement item, double xLeft)
        {
            var xRight = xLeft + item.DesiredSize.Width;

            if (xLeft.LessThan(viewportLeft))
            {
                if (xRight > viewportLeft)
                {
                    item.Opacity = 1;
                    item.IsHitTestVisible = true;
                    var brush = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
                    brush.GradientStops.Add(new GradientStop() { Color = Colors.Transparent, Offset = 0 });
                    brush.GradientStops.Add(new GradientStop() { Color = Colors.Transparent, Offset = (viewportLeft - xLeft) / item.DesiredSize.Width });
                    brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = Math.Min(1, (viewportLeft - xLeft) / item.DesiredSize.Width + 0.05) });
                    item.OpacityMask = brush;
                }
                else
                {
                    item.Opacity = 0;
                    item.IsHitTestVisible = false;
                }
            }
            else
            {
                if (xLeft >= viewportRight)
                {
                    item.Opacity = 0;
                    item.IsHitTestVisible = false;
                }
                else
                {
                    item.OpacityMask = null;
                    item.IsHitTestVisible = true;
                    item.Opacity = 1;

                    if (xRight > viewportRight)
                    {
                        var brush = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
                        brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = 0 });
                        brush.GradientStops.Add(new GradientStop() { Color = Colors.White, Offset = Math.Max(0, (viewportRight - xLeft) / item.DesiredSize.Width - 0.05) });
                        brush.GradientStops.Add(new GradientStop() { Color = Colors.Transparent, Offset = (viewportRight - xLeft) / item.DesiredSize.Width });
                        item.OpacityMask = brush;
                    }
                }
            }
        }

        private double GetViewportLeft()
        {
            var wFixed = InternalChildren.OfType<UIElement>().Sum(e => e.DesiredSize.Width);

            if (IsTruncatingItems)
            {
                wFixed += _btnScrollLeft.DesiredSize.Width;
            }

            return wFixed;
        }

        private double GetViewportRight()
        {
            return ActualWidth - (IsTruncatingItems ? _btnScrollRight.DesiredSize.Width : 0);
        }

        private void UpdateHorizontalOffset(double viewportLeft, double viewportRight, IEnumerable<UIElement> floatingItems, UIElement targetItem)
		{
			if (SelectedItem != null)
			{
                var lastItemX = 0.0;
                var lastItem = (UIElement)null;

                EnumerateFloatingItems(floatingItems, viewportLeft, (x, item) =>
                    {
                        if (item == targetItem)
                        {
                            if (x <= viewportLeft)
                            {
                                HorizontalOffsetToSelectedItem += viewportLeft - x;
                            }
                            else if (x + item.DesiredSize.Width > viewportRight)
                            {
                                HorizontalOffsetToSelectedItem += viewportRight - x - item.DesiredSize.Width;
                            }
                        }
                        lastItemX = x + item.DesiredSize.Width;
                        lastItem = item;
                    });

                if (IsTruncatingItems)
                {
                    if (lastItemX < viewportRight)
                    {
                        HorizontalOffsetToSelectedItem += viewportRight - lastItemX;
                    }
                }
                else
                {
                    HorizontalOffsetToSelectedItem = 0;
                }
			}
		}
        private void EnumerateFloatingItems(IEnumerable<UIElement> items, double x0, Action<double, UIElement> action)
        {
            var xAdjusted = x0 + HorizontalOffsetToSelectedItem;
            foreach (var item in items)
            {
                action(xAdjusted, item);
                xAdjusted += item.DesiredSize.Width;
            }
        }

		private void ScrollToLeft()
		{
            toright = false;

            //if (SelectedItem == null)
            //{
            //    SelectedItem = InternalChildren.OfType<TabItem>().LastOrDefault();
            //}
            //else
            //{
                var allItems = InternalChildren.OfType<TabItem>().ToList();
                var viewportLeft = GetViewportLeft();
                if (!toleft)
                {
                    SetStartIndex(allItems);
                    SelectedItem = allItems[startindex];
                    toleft = true;
                }
                else
                {
                    int index = allItems.IndexOf(SelectedItem);
                    if (index > 0)
                    {
                        SelectedItem = allItems[index - 1];
                        startindex--;
                    }
                    else
                    {
                        EnumerateFloatingItems(allItems, viewportLeft, (x, item) =>
                        {
                            var xRight = x + item.DesiredSize.Width;
                            if (x <= viewportLeft && (xRight >= viewportLeft || Math.Abs(xRight - viewportLeft) < 0.0001))
                            {
                                UpdateHorizontalOffset(viewportLeft, GetViewportRight(), allItems, item);
                                InvalidateArrange();
                            }
                        });
                    }
                }
                //    var index = allItems.IndexOf(SelectedItem);
                //    if (index > 0)
                //    {
                //        SelectedItem = allItems[index - 1];
                //    }
                //    else
                //    {
                //        EnumerateFloatingItems(allItems, viewportLeft, (x, item) =>
                //        {
                //            var xRight = x + item.DesiredSize.Width;
                //            if (x <= viewportLeft && (xRight >= viewportLeft || Math.Abs(xRight - viewportLeft) < 0.0001))
                //            {
                //                UpdateHorizontalOffset(viewportLeft, GetViewportRight(), allItems, item);
                //                InvalidateArrange();
                //            }
                //        });
                //    }
                //}
            }
        private void SetStartIndex(List<TabItem> items)
        {
            startindex = Math.Min(endindex + 1,items.Count);
            var length = GetViewportRight();
            double templength = 0;
            while (templength < length)
            {
                startindex--;
                if (startindex < 0)
                {
                    break;
                }
                templength += items[startindex].ActualWidth;
            }
            var temp = startindex;
        }
        private void SetEndIndex(List<TabItem> items)
        {
            endindex = Math.Min(startindex - 1,items.Count - 2);
            var length = GetViewportRight();
            double templength = 0;
            while (templength < length)
            {
                endindex++;
                if (endindex > items.Count - 1)
                {
                    break;
                }
                templength += items[endindex].ActualWidth;
            }
            var temp = endindex;
        }
		private void ScrollToRight()
		{
            toleft = false;
            var allItems = InternalChildren.OfType<TabItem>().ToList();
            var viewportLeft = GetViewportLeft();
            var viewportRight = GetViewportRight();
            //SelectedItem = allItems[0];
            //if (SelectedItem == null)
            //{
            //    SelectedItem = InternalChildren.OfType<TabItem>().FirstOrDefault();
            //}
            //else
            //{
            //    var allItems = InternalChildren.OfType<TabItem>().ToList();

                if (!toright)
                {
                    SetEndIndex(allItems);
                    SelectedItem = allItems[endindex];
                    toright = true;
                }
                else
                {
                    int index = allItems.IndexOf(SelectedItem);
                    if (index < allItems.Count - 1 && index >= 0)
                    {
                        SelectedItem = allItems[index + 1];
                        endindex++;
                    }
                    else
                    {
                        EnumerateFloatingItems(allItems, viewportLeft, (x, item) =>
                        {
                            var xRight = x + item.DesiredSize.Width;
                            if (x <= viewportRight && (xRight >= viewportRight || Math.Abs(xRight - viewportRight) < 0.0001))
                            {
                                UpdateHorizontalOffset(viewportLeft, viewportRight, allItems, item);
                                InvalidateArrange();
                            }
                        });
                    }
                }

                //    var index = allItems.IndexOf(SelectedItem);
                //    if (index >= 0 && index < allItems.Count - 1)
                //    {
                //        SelectedItem = allItems[index + 1];
                //    }
                //    else
                //    {
                //        EnumerateFloatingItems(allItems, viewportLeft, (x, item) =>
                //        {
                //            var xRight = x + item.DesiredSize.Width;
                //            if (x <= viewportRight && (xRight >= viewportRight || Math.Abs(xRight - viewportRight) < 0.0001))
                //            {
                //                UpdateHorizontalOffset(viewportLeft, viewportRight, allItems, item);
                //                InvalidateArrange();
                //            }
                //        });
                //    }
                //}
            }

		#endregion Methods
    }
}

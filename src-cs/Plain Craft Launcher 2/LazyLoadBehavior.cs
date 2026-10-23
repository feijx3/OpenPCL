using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PCL
{
    public class LazyLoadBehavior
    {
        public static void OnFirstEnterScrollViewerViewport(DependencyObject obj, Action value)
        {
            obj.SetValue(LazyLoadBehavior.IsInViewportProperty, value);
        }

        private static void OnIsInViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement Element = d as FrameworkElement;
            if (Element != null && e.NewValue != null)
            {
                bool Handled = false;
                EventHandler Handler = delegate(object a0, EventArgs a1)
                {
                    base._Lambda$__0();
                };
                Element.LayoutUpdated += Handler;
            }
        }

        private static ScrollViewer FindParentScrollViewer(DependencyObject d)
        {
            while (d != null)
            {
                if (d is ScrollViewer)
                {
                    return (ScrollViewer)d;
                }
                d = VisualTreeHelper.GetParent(d);
            }
            return null;
        }

        private static readonly DependencyProperty IsInViewportProperty = DependencyProperty.RegisterAttached("IsInViewport", typeof(Action), typeof(LazyLoadBehavior), new PropertyMetadata(null, new PropertyChangedCallback(LazyLoadBehavior.OnIsInViewportChanged)));
    }
}

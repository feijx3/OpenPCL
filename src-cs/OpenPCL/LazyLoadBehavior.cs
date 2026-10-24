using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PCL
{
    public class LazyLoadBehavior
    {
        // Note: this type is marked as 'beforefieldinit'.
        static LazyLoadBehavior()
        {
        }

        public LazyLoadBehavior()
        {
        }

        public static void OnFirstEnterScrollViewerViewport(DependencyObject obj, Action value)
        {
            obj.SetValue(LazyLoadBehavior.IsInViewportProperty, value);
        }

        private static void OnIsInViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LazyLoadBehavior._Closure___4_0 CS_locals1 = new LazyLoadBehavior._Closure___4_0();
            CS_locals1._VB_Local_e = e;
            CS_locals1._VB_Local_Element = d as FrameworkElement;
            if (CS_locals1._VB_Local_Element != null && CS_locals1._VB_Local_e.NewValue != null)
            {
                CS_locals1._VB_Local_Handled = false;
                CS_locals1._VB_Local_Handler = new EventHandler(CS_locals1._Lambda___R1);
                CS_locals1._VB_Local_Element.LayoutUpdated += CS_locals1._VB_Local_Handler;
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

        [CompilerGenerated]
        private sealed class _Closure___4_0
        {
            public _Closure___4_0()
            {
            }

            internal void _Lambda___R1(object a0, EventArgs a1)
            {
                this._Lambda___0();
            }

            internal void _Lambda___0()
            {
                if (!this._VB_Local_Handled && this._VB_Local_Element.RenderSize.Width >= 5E-324 && this._VB_Local_Element.IsVisible)
                {
                    ScrollViewer scrollViewer = LazyLoadBehavior.FindParentScrollViewer(this._VB_Local_Element);
                    if (scrollViewer != null && new Rect(0.0, 0.0, scrollViewer.ViewportWidth, scrollViewer.ViewportHeight).IntersectsWith(this._VB_Local_Element.TransformToAncestor(scrollViewer).TransformBounds(new Rect(new Point(0.0, 0.0), this._VB_Local_Element.RenderSize))))
                    {
                        this._VB_Local_Handled = true;
                        ((Action)this._VB_Local_e.NewValue)();
                        this._VB_Local_Element.LayoutUpdated -= this._VB_Local_Handler;
                    }
                }
            }

            public bool _VB_Local_Handled;

            public FrameworkElement _VB_Local_Element;

            public DependencyPropertyChangedEventArgs _VB_Local_e;

            public EventHandler _VB_Local_Handler;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;

namespace PCL
{
    public class MyVirtualizingElement<T> : FrameworkElement where T : FrameworkElement
    {
        public MyVirtualizingElement(Func<T> Initializer)
        {
            this.Initializer = Initializer;
            LazyLoadBehavior.OnFirstEnterScrollViewerViewport(this, delegate
            {
                this.Init();
            });
        }

        public T Init()
        {
            T t = this.Initializer();
            if (base.Parent != null)
            {
                if (!(base.Parent is Panel))
                {
                    throw new Exception("MyVirtualizingElement 的父级必须是一个 Panel");
                }
                Panel panel = (Panel)base.Parent;
                int num = panel.Children.IndexOf(this);
                panel.Children.RemoveAt(num);
                panel.Children.Insert(num, (UIElement)((object)t));
            }
            return t;
        }

        public static implicit operator T(MyVirtualizingElement<T> Virtualized)
        {
            return Virtualized.Init();
        }

        private Func<T> Initializer;
    }
}

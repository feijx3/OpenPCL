using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace PCL
{
    public class OurVirtualizingElement<T> : FrameworkElement where T : FrameworkElement
    {
        public OurVirtualizingElement(Func<T> Initializer)
        {
            this.Initializer = Initializer;
            LazyLoadBehavior.OnFirstEnterScrollViewerViewport(this, new Action(this._Lambda___R1_1));
        }

        public T Init()
        {
            T t = this.Initializer();
            if (base.Parent != null)
            {
                if (!(base.Parent is Panel))
                {
                    throw new Exception("OurVirtualizingElement 的父级必须是一个 Panel");
                }
                Panel panel = (Panel)base.Parent;
                int num = panel.Children.IndexOf(this);
                panel.Children.RemoveAt(num);
                panel.Children.Insert(num, (UIElement)((object)t));
            }
            return t;
        }

        public static implicit operator T(OurVirtualizingElement<T> Virtualized)
        {
            return Virtualized.Init();
        }

        [CompilerGenerated]
        private void _Lambda___R1_1()
        {
            this.Init();
        }

        private Func<T> Initializer;
    }
}

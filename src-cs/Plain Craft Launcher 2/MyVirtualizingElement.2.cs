using System;
using System.Windows;
using System.Windows.Controls;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class MyVirtualizingElement : FrameworkElement
    {
        public MyVirtualizingElement(Func<FrameworkElement> Initializer)
        {
            this.Initializer = Initializer;
            LazyLoadBehavior.OnFirstEnterScrollViewerViewport(this, delegate
            {
                this.Init();
            });
        }

        public FrameworkElement Init()
        {
            FrameworkElement frameworkElement = this.Initializer();
            if (base.Parent != null)
            {
                if (!(base.Parent is Panel))
                {
                    throw new Exception("MyVirtualizingElement 的父级必须是一个 Panel");
                }
                Panel panel = (Panel)base.Parent;
                int num = panel.Children.IndexOf(this);
                panel.Children.RemoveAt(num);
                panel.Children.Insert(num, frameworkElement);
            }
            return frameworkElement;
        }

        public static FrameworkElement TryInit(FrameworkElement Element)
        {
            FrameworkElement frameworkElement;
            if (GeneralUtils.IsGenericInstanceOf(Element, typeof(MyVirtualizingElement<>)))
            {
                frameworkElement = (FrameworkElement)NewLateBinding.LateGet(Element, null, "Init", new object[0], null, null, null);
            }
            else if (Element is MyVirtualizingElement)
            {
                frameworkElement = ((MyVirtualizingElement)Element).Init();
            }
            else
            {
                frameworkElement = Element;
            }
            return frameworkElement;
        }

        private Func<FrameworkElement> Initializer;
    }
}

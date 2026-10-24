using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class OurVirtualizingElement : FrameworkElement
    {
        public OurVirtualizingElement(Func<FrameworkElement> Initializer)
        {
            this.Initializer = Initializer;
            LazyLoadBehavior.OnFirstEnterScrollViewerViewport(this, new Action(this._Lambda___R1_1));
        }

        public FrameworkElement Init()
        {
            FrameworkElement frameworkElement = this.Initializer();
            if (base.Parent != null)
            {
                if (!(base.Parent is Panel))
                {
                    throw new Exception("OurVirtualizingElement 的父级必须是一个 Panel");
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
            if (GeneralUtils.IsGenericInstanceOf(Element, typeof(OurVirtualizingElement<>)))
            {
                frameworkElement = (FrameworkElement)NewLateBinding.LateGet(Element, null, "Init", new object[0], null, null, null);
            }
            else if (Element is OurVirtualizingElement)
            {
                frameworkElement = ((OurVirtualizingElement)Element).Init();
            }
            else
            {
                frameworkElement = Element;
            }
            return frameworkElement;
        }

        [CompilerGenerated]
        private void _Lambda___R1_1()
        {
            this.Init();
        }

        private Func<FrameworkElement> Initializer;
    }
}

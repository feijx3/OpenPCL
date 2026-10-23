using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class MyPageLeft : Grid
    {
        public MyPageLeft()
        {
            this.Uuid = ModBase.GetUuid();
        }

        public string AnimatedControl
        {
            get
            {
                return Conversions.ToString(base.GetValue(MyPageLeft.AnimatedControlProperty));
            }
            set
            {
                base.SetValue(MyPageLeft.AnimatedControlProperty, value);
            }
        }

        public void TriggerShowAnimation()
        {
            if (this.AnimatedControl == null)
            {
                if (!(base.RenderTransform is ScaleTransform))
                {
                    base.RenderTransform = new ScaleTransform(0.96, 0.96);
                    base.RenderTransformOrigin = new Point(0.5, 0.5);
                }
                base.Opacity = 0.0;
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScaleTransform(this, 1.0 - ((ScaleTransform)base.RenderTransform).ScaleX, 400, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaOpacity(this, 1.0, 100, 0, null, false)
                }, "PageLeft PageChange " + Conversions.ToString(this.Uuid), false);
                return;
            }
            List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
            int num = 0;
            int num2 = 0;
            checked
            {
                try
                {
                    List<FrameworkElement>.Enumerator enumerator = this.GetAllAnimControls(true).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        MyPageLeft._Closure$__7-0 CS$<>8__locals1 = new MyPageLeft._Closure$__7-0(CS$<>8__locals1);
                        CS$<>8__locals1.$VB$Me = this;
                        CS$<>8__locals1.$VB$Local_Element = enumerator.Current;
                        CS$<>8__locals1.$VB$Local_Element = MyVirtualizingElement.TryInit(CS$<>8__locals1.$VB$Local_Element);
                        if (CS$<>8__locals1.$VB$Local_Element.Visibility == Visibility.Collapsed)
                        {
                            CS$<>8__locals1.$VB$Local_Element.Opacity = 1.0;
                            CS$<>8__locals1.$VB$Local_Element.RenderTransform = new TranslateTransform(0.0, 0.0);
                            if (CS$<>8__locals1.$VB$Local_Element is MyListItem)
                            {
                                ((MyListItem)CS$<>8__locals1.$VB$Local_Element).IsMouseOverAnimationEnabled = true;
                            }
                        }
                        else
                        {
                            CS$<>8__locals1.$VB$Local_Element.Opacity = 0.0;
                            CS$<>8__locals1.$VB$Local_Element.RenderTransform = new TranslateTransform(-25.0, 0.0);
                            if (CS$<>8__locals1.$VB$Local_Element is MyListItem)
                            {
                                ((MyListItem)CS$<>8__locals1.$VB$Local_Element).IsMouseOverAnimationEnabled = false;
                            }
                            list.Add(ModAnimation.AaOpacity(CS$<>8__locals1.$VB$Local_Element, (CS$<>8__locals1.$VB$Local_Element is TextBlock) ? 0.6 : 1.0, 100, num2, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false));
                            list.Add(ModAnimation.AaTranslateX(CS$<>8__locals1.$VB$Local_Element, 5.0, 200, num2, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false));
                            list.Add(ModAnimation.AaTranslateX(CS$<>8__locals1.$VB$Local_Element, 20.0, 300, num2, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false));
                            if (CS$<>8__locals1.$VB$Local_Element is MyListItem)
                            {
                                list.Add(ModAnimation.AaCode(delegate
                                {
                                    ((MyListItem)CS$<>8__locals1.$VB$Local_Element).IsMouseOverAnimationEnabled = true;
                                    ((MyListItem)CS$<>8__locals1.$VB$Local_Element).RefreshColor(CS$<>8__locals1.$VB$Me, new EventArgs());
                                }, num2 + 280, false));
                            }
                            num2 += Math.Max(15 - num, 7) * 2;
                            num++;
                        }
                    }
                }
                finally
                {
                    List<FrameworkElement>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                ModAnimation.AniStart(list, "PageLeft PageChange " + Conversions.ToString(this.Uuid), false);
            }
        }

        public void TriggerHideAnimation()
        {
            if (this.AnimatedControl == null)
            {
                if (!(base.RenderTransform is ScaleTransform))
                {
                    base.RenderTransform = new ScaleTransform(1.0, 1.0);
                    base.RenderTransformOrigin = new Point(0.5, 0.5);
                }
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScaleTransform(this, 0.95 - ((ScaleTransform)base.RenderTransform).ScaleX, 110, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false),
                    ModAnimation.AaOpacity(this, -base.Opacity, 80, 30, null, false)
                }, "PageLeft PageChange " + Conversions.ToString(this.Uuid), false);
                return;
            }
            List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
            int num = 0;
            List<FrameworkElement> allAnimControls = this.GetAllAnimControls(false);
            checked
            {
                try
                {
                    foreach (FrameworkElement frameworkElement in allAnimControls)
                    {
                        list.Add(ModAnimation.AaOpacity(frameworkElement, unchecked(-frameworkElement.Opacity), 50, (int)Math.Round(unchecked(70.0 / (double)allAnimControls.Count * (double)num)), null, false));
                        list.Add(ModAnimation.AaTranslateX(frameworkElement, -6.0, 50, (int)Math.Round(unchecked(70.0 / (double)allAnimControls.Count * (double)num)), null, false));
                        num++;
                    }
                }
                finally
                {
                    List<FrameworkElement>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
                ModAnimation.AniStart(list, "PageLeft PageChange " + Conversions.ToString(this.Uuid), false);
            }
        }

        private List<FrameworkElement> GetAllAnimControls(bool IgnoreInvisibility = false)
        {
            List<FrameworkElement> list = new List<FrameworkElement>();
            this.GetAllAnimControls((FrameworkElement)base.FindName(this.AnimatedControl), ref list, IgnoreInvisibility);
            return list;
        }

        private void GetAllAnimControls(FrameworkElement Element, ref List<FrameworkElement> AllControls, bool IgnoreInvisibility)
        {
            if (IgnoreInvisibility || Element.Visibility != Visibility.Collapsed)
            {
                if (Element is MyTextButton)
                {
                    AllControls.Add(Element);
                    return;
                }
                if (Element is MyListItem)
                {
                    AllControls.Add(Element);
                    return;
                }
                if (Element is ContentControl)
                {
                    this.GetAllAnimControls((FrameworkElement)((ContentControl)Element).Content, ref AllControls, IgnoreInvisibility);
                    return;
                }
                if (Element is Panel)
                {
                    try
                    {
                        foreach (object obj in ((Panel)Element).Children)
                        {
                            FrameworkElement frameworkElement = (FrameworkElement)obj;
                            this.GetAllAnimControls(frameworkElement, ref AllControls, IgnoreInvisibility);
                        }
                        return;
                    }
                    finally
                    {
                        IEnumerator enumerator;
                        if (enumerator is IDisposable)
                        {
                            (enumerator as IDisposable).Dispose();
                        }
                    }
                }
                AllControls.Add(Element);
            }
        }

        private int Uuid;

        public static readonly DependencyProperty AnimatedControlProperty = DependencyProperty.Register("AnimatedControl", typeof(string), typeof(MyPageLeft), new PropertyMetadata(null));
    }
}

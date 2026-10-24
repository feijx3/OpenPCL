using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class OurPageLeft : Grid
    {
        // Note: this type is marked as 'beforefieldinit'.
        static OurPageLeft()
        {
        }

        public OurPageLeft()
        {
            this.Uuid = ModBase.GetUuid();
        }

        public string AnimatedControl
        {
            get
            {
                return Conversions.ToString(base.GetValue(OurPageLeft.AnimatedControlProperty));
            }
            set
            {
                base.SetValue(OurPageLeft.AnimatedControlProperty, value);
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
                        OurPageLeft._Closure___7_0 CS_locals1 = new OurPageLeft._Closure___7_0(null);
                        CS_locals1._VB_Me = this;
                        CS_locals1._VB_Local_Element = enumerator.Current;
                        CS_locals1._VB_Local_Element = OurVirtualizingElement.TryInit(CS_locals1._VB_Local_Element);
                        if (CS_locals1._VB_Local_Element.Visibility == Visibility.Collapsed)
                        {
                            CS_locals1._VB_Local_Element.Opacity = 1.0;
                            CS_locals1._VB_Local_Element.RenderTransform = new TranslateTransform(0.0, 0.0);
                            if (CS_locals1._VB_Local_Element is OurListItem)
                            {
                                ((OurListItem)CS_locals1._VB_Local_Element).IsMouseOverAnimationEnabled = true;
                            }
                        }
                        else
                        {
                            CS_locals1._VB_Local_Element.Opacity = 0.0;
                            CS_locals1._VB_Local_Element.RenderTransform = new TranslateTransform(-25.0, 0.0);
                            if (CS_locals1._VB_Local_Element is OurListItem)
                            {
                                ((OurListItem)CS_locals1._VB_Local_Element).IsMouseOverAnimationEnabled = false;
                            }
                            list.Add(ModAnimation.AaOpacity(CS_locals1._VB_Local_Element, (CS_locals1._VB_Local_Element is TextBlock) ? 0.6 : 1.0, 100, num2, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false));
                            list.Add(ModAnimation.AaTranslateX(CS_locals1._VB_Local_Element, 5.0, 200, num2, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false));
                            list.Add(ModAnimation.AaTranslateX(CS_locals1._VB_Local_Element, 20.0, 300, num2, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false));
                            if (CS_locals1._VB_Local_Element is OurListItem)
                            {
                                list.Add(ModAnimation.AaCode(new ThreadStart(CS_locals1._Lambda___0), num2 + 280, false));
                            }
                            num2 += Math.Max(15 - num, 7) * 2;
                            num++;
                        }
                    }
                }
                finally
                {
                    List<FrameworkElement>.Enumerator enumerator = default;
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
                    List<FrameworkElement>.Enumerator enumerator = default;
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
                if (Element is OurTextButton)
                {
                    AllControls.Add(Element);
                    return;
                }
                if (Element is OurListItem)
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
                        IEnumerator enumerator = default;
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

        public static readonly DependencyProperty AnimatedControlProperty = DependencyProperty.Register("AnimatedControl", typeof(string), typeof(OurPageLeft), new PropertyMetadata(null));

        [CompilerGenerated]
        private sealed class _Closure___7_0
        {
            public _Closure___7_0(OurPageLeft._Closure___7_0 arg0)
            {
                if (arg0 != null)
                {
                    this._VB_Local_Element = arg0._VB_Local_Element;
                }
            }

            internal void _Lambda___0()
            {
                ((OurListItem)this._VB_Local_Element).IsMouseOverAnimationEnabled = true;
                ((OurListItem)this._VB_Local_Element).RefreshColor(this._VB_Me, new EventArgs());
            }

            public FrameworkElement _VB_Local_Element;

            public OurPageLeft _VB_Me;
        }
    }
}

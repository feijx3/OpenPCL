using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class MyScrollBar : ScrollBar
    {
        public MyScrollBar()
        {
            base.IsEnabledChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.RefreshColor();
            };
            base.GotMouseCapture += delegate(object a0, MouseEventArgs a1)
            {
                this.RefreshColor();
            };
            base.LostMouseCapture += delegate(object a0, MouseEventArgs a1)
            {
                this.RefreshColor();
            };
            base.MouseEnter += delegate(object a0, MouseEventArgs a1)
            {
                this.RefreshColor();
            };
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.RefreshColor();
            };
            base.IsVisibleChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.RefreshColor();
            };
            this.Uuid = ModBase.GetUuid();
        }

        private void RefreshColor()
        {
            try
            {
                double num;
                int num2;
                string text;
                if (!base.IsVisible)
                {
                    num = 0.0;
                    num2 = 20;
                    text = "ColorBrush4";
                }
                else if (base.IsMouseCaptureWithin)
                {
                    num = 1.0;
                    text = "ColorBrush4";
                    num2 = 50;
                }
                else if (base.IsMouseOver)
                {
                    num = 0.9;
                    text = "ColorBrush3";
                    num2 = 130;
                }
                else
                {
                    num = 0.5;
                    text = "ColorBrush4";
                    num2 = 180;
                }
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaColor(this, Control.ForegroundProperty, text, num2, 0, null, false),
                        ModAnimation.AaOpacity(this, num - base.Opacity, num2, 0, null, false)
                    }, "MyScrollBar Color " + Conversions.ToString(this.Uuid), false);
                }
                else
                {
                    ModAnimation.AniStop("MyScrollBar Color " + Conversions.ToString(this.Uuid));
                    base.SetResourceReference(Control.ForegroundProperty, text);
                    base.Opacity = num;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "滚动条颜色改变出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyScrollBar.vb");
            }
        }

        public int Uuid;
    }
}

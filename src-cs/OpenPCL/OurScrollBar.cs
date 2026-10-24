using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class OurScrollBar : ScrollBar
    {
        public OurScrollBar()
        {
            base.IsEnabledChanged += this._Lambda___R0_1;
            base.GotMouseCapture += this._Lambda___R0_2;
            base.LostMouseCapture += this._Lambda___R0_3;
            base.MouseEnter += this._Lambda___R0_4;
            base.MouseLeave += this._Lambda___R0_5;
            base.IsVisibleChanged += this._Lambda___R0_6;
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
                    }, "OurScrollBar Color " + Conversions.ToString(this.Uuid), false);
                }
                else
                {
                    ModAnimation.AniStop("OurScrollBar Color " + Conversions.ToString(this.Uuid));
                    base.SetResourceReference(Control.ForegroundProperty, text);
                    base.Opacity = num;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "滚动条颜色改变出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurScrollBar.vb");
            }
        }

        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_2(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_3(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_4(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_5(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_6(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor();
        }

        public int Uuid;
    }
}

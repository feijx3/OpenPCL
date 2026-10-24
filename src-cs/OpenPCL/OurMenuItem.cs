using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class OurMenuItem : MenuItem
    {
        public OurMenuItem()
        {
            base.Loaded += this.OurMenuItem_Loaded;
            base.MouseEnter += this._Lambda___R0_1;
            base.MouseLeave += this._Lambda___R0_2;
            base.IsEnabledChanged += this._Lambda___R0_3;
            base.Click += this.OurMenuItem_Click;
            this.Uuid = ModBase.GetUuid();
        }

        private void OurMenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (base.Icon != null)
            {
                Path path = (Path)base.GetTemplateChild("Icon");
                if (path != null)
                {
                    path.Data = (Geometry)new GeometryConverter().ConvertFromString(Conversions.ToString(base.Icon));
                }
            }
            if (base.Parent is ContextMenu)
            {
                ((ContextMenu)base.Parent).Opacity = (double)Settings.Get<int>("UiLauncherTransparent", null) / 1000.0 + 0.4;
            }
        }

        private void RefreshColor()
        {
            string text;
            string text2;
            int num;
            if (!base.IsEnabled)
            {
                text = "ColorBrushTransparent";
                text2 = "ColorBrushGray5";
                num = 200;
            }
            else if (base.IsMouseOver)
            {
                text = "ColorBrush6";
                text2 = "ColorBrush2";
                num = 100;
            }
            else
            {
                text = "ColorBrushTransparent";
                text2 = "ColorBrush1";
                num = 200;
            }
            if (Operators.CompareString(this.ColorName, text, false) != 0)
            {
                this.ColorName = text;
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaColor(this, Control.BackgroundProperty, text, num, 0, null, false),
                        ModAnimation.AaColor(this, Control.ForegroundProperty, text2, num, 0, null, false)
                    }, "OurMenuItem Color " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStop("OurMenuItem Color " + Conversions.ToString(this.Uuid));
                base.SetResourceReference(Control.BackgroundProperty, text);
                base.SetResourceReference(Control.ForegroundProperty, text2);
            }
        }

        private void OurMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseCustomEvent();
        }

        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_2(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R0_3(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor();
        }

        public int Uuid;

        private const int AnimationTimeIn = 100;

        private const int AnimationTimeOut = 200;

        private string ColorName;
    }
}

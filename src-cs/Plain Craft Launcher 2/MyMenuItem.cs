using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class MyMenuItem : MenuItem
    {
        public MyMenuItem()
        {
            base.Loaded += this.MyMenuItem_Loaded;
            base.MouseEnter += delegate(object a0, MouseEventArgs a1)
            {
                this.RefreshColor();
            };
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.RefreshColor();
            };
            base.IsEnabledChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.RefreshColor();
            };
            base.Click += this.MyMenuItem_Click;
            this.Uuid = ModBase.GetUuid();
        }

        private void MyMenuItem_Loaded(object sender, RoutedEventArgs e)
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
                    }, "MyMenuItem Color " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStop("MyMenuItem Color " + Conversions.ToString(this.Uuid));
                base.SetResourceReference(Control.BackgroundProperty, text);
                base.SetResourceReference(Control.ForegroundProperty, text2);
            }
        }

        private void MyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseCustomEvent();
        }

        public int Uuid;

        private const int AnimationTimeIn = 100;

        private const int AnimationTimeOut = 200;

        private string ColorName;
    }
}

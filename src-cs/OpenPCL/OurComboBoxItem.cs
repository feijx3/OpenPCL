using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class OurComboBoxItem : ComboBoxItem
    {
        public OurComboBoxItem()
        {
            base.Unselected += this._Lambda___R1_1;
            base.MouseMove += this._Lambda___R1_2;
            base.MouseLeave += this._Lambda___R1_3;
            base.Selected += this._Lambda___R1_4;
            base.IsEnabledChanged += this._Lambda___R1_5;
            base.MouseLeftButtonUp += this.OurComboBoxItem_MouseLeftButtonUp;
            this.Uuid = ModBase.GetUuid();
            base.Style = (Style)base.FindResource("OurComboBoxItem");
        }

        private void RefreshColor()
        {
            string text;
            double num;
            int num2;
            if (base.IsSelected)
            {
                text = "ColorBrush6";
                num = 1.0;
                num2 = 100;
            }
            else if (base.IsMouseOver)
            {
                text = "ColorBrush8";
                num = 1.0;
                num2 = 100;
            }
            else if (base.IsEnabled)
            {
                text = "ColorBrushTransparent";
                num = 1.0;
                num2 = 300;
            }
            else
            {
                text = "ColorBrushTransparent";
                num = 0.4;
                num2 = 300;
            }
            if (Operators.CompareString(this.BackColorName, text, false) != 0 || this.FontOpacity != num)
            {
                this.BackColorName = text;
                this.FontOpacity = num;
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaColor(this, Control.BackgroundProperty, this.BackColorName, num2, 0, null, false),
                        ModAnimation.AaOpacity(this, this.FontOpacity - base.Opacity, num2, 0, null, false)
                    }, "ComboBoxItem Color " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStop("ComboBoxItem Color " + Conversions.ToString(this.Uuid));
                base.SetResourceReference(Control.BackgroundProperty, this.BackColorName);
                base.Opacity = this.FontOpacity;
            }
        }

        public override string ToString()
        {
            return base.Content.ToString();
        }

        public static implicit operator string(OurComboBoxItem Value)
        {
            return Value.Content.ToString();
        }

        private void OurComboBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Logger.Info(string.Format("选择下拉列表项：{0}", this.ToString()), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurComboBoxItem.vb");
        }

        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, RoutedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R1_2(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R1_3(object a0, MouseEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R1_4(object a0, RoutedEventArgs a1)
        {
            this.RefreshColor();
        }

        [CompilerGenerated]
        private void _Lambda___R1_5(object a0, DependencyPropertyChangedEventArgs a1)
        {
            this.RefreshColor();
        }

        public int Uuid;

        private const int AnimationTimeIn = 100;

        private const int AnimationTimeOut = 300;

        private string BackColorName;

        private double FontOpacity;
    }
}

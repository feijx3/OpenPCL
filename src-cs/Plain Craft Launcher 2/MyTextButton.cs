using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class MyTextButton : Label
    {
        public event MyTextButton.ClickEventHandler Click;

        public MyTextButton()
        {
            base.PreviewMouseLeftButtonDown += this.MyTextButton_MouseLeftButtonDown;
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.MyTextButton_MouseLeave();
            };
            base.PreviewMouseLeftButtonUp += this.MyTextButton_MouseLeftButtonUp;
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
            base.MouseLeftButtonDown += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.RefreshColor();
            };
            base.MouseLeftButtonUp += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.RefreshColor();
            };
            this.Uuid = ModBase.GetUuid();
            this.IsMouseDown = false;
            base.SetResourceReference(Control.ForegroundProperty, "ColorBrush1");
            base.Background = ModSecret.ColorSemiTransparent;
        }

        public string Text
        {
            get
            {
                return Conversions.ToString(base.GetValue(MyTextButton.TextProperty));
            }
            set
            {
                base.SetValue(MyTextButton.TextProperty, value);
            }
        }

        private void MyTextButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = true;
            e.Handled = true;
        }

        private void MyTextButton_MouseLeave()
        {
            this.IsMouseDown = false;
        }

        private void MyTextButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseDown)
            {
                this.IsMouseDown = false;
                Logger.Info(string.Format("按下文本按钮：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyTextButton.vb");
                MyTextButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(this, null);
                }
                this.RaiseCustomEvent();
                e.Handled = true;
            }
        }

        private void RefreshColor()
        {
            string text;
            int num;
            if (this.IsMouseDown)
            {
                text = "ColorBrush4";
                num = 30;
            }
            else if (base.IsMouseOver)
            {
                text = "ColorBrush3";
                num = 100;
            }
            else
            {
                text = "ColorBrush1";
                num = 200;
            }
            if (Operators.CompareString(this.ColorName, text, false) != 0)
            {
                this.ColorName = text;
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    ModAnimation.AniStart(ModAnimation.AaColor(this, Control.ForegroundProperty, text, num, 0, null, false), "MyTextButton Color " + Conversions.ToString(this.Uuid), false);
                    return;
                }
                ModAnimation.AniStop("MyTextButton Color " + Conversions.ToString(this.Uuid));
                base.SetResourceReference(Control.ForegroundProperty, text);
            }
        }

        public int Uuid;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MyTextButton), new PropertyMetadata("", delegate(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
        {
            ((MyTextButton._Closure$__.$I0-0 == null) ? (MyTextButton._Closure$__.$I0-0 = delegate(MyTextButton sender, DependencyPropertyChangedEventArgs e)
            {
                if (!Operators.ConditionalCompareObjectEqual(e.OldValue, e.NewValue, false))
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaOpacity(sender, -sender.Opacity, 50, 0, null, false),
                        ModAnimation.AaCode(delegate
                        {
                            sender.Content = RuntimeHelpers.GetObjectValue(e.NewValue);
                        }, 0, true),
                        ModAnimation.AaOpacity(sender, 1.0, 170, 0, null, false)
                    }, "MyTextButton Text " + Conversions.ToString(sender.Uuid), false);
                }
            }) : MyTextButton._Closure$__.$I0-0)((MyTextButton)a0, a1);
        }));

        public bool IsMouseDown;

        private const int AnimationTimeIn = 100;

        private const int AnimationTimeOut = 200;

        private string ColorName;

        public delegate void ClickEventHandler(object sender, EventArgs e);
    }
}

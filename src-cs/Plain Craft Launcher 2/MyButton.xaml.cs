using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [ContentProperty("Inlines")]
    [DesignerGenerated]
    public partial class MyButton : Border
    {
        public MyButton()
        {
            base.MouseEnter += new MouseEventHandler(this.RefreshColor);
            base.MouseLeave += new MouseEventHandler(this.RefreshColor);
            base.Loaded += new RoutedEventHandler(this.RefreshColor);
            base.IsEnabledChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.RefreshColor(RuntimeHelpers.GetObjectValue(a0), a1);
            };
            base.MouseLeftButtonUp += this.Button_MouseUp;
            base.MouseLeftButtonDown += this.Button_MouseDown;
            base.MouseEnter += delegate(object a0, MouseEventArgs a1)
            {
                this.Button_MouseEnter();
            };
            base.MouseLeftButtonUp += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.Button_MouseUp();
            };
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.Button_MouseLeave();
            };
            this.Uuid = ModBase.GetUuid();
            this._ColorType = MyButton.ColorState.Normal;
            this.IsMouseDown = false;
            this.InitializeComponent();
        }

        public event MyButton.ClickEventHandler Click;

        public InlineCollection Inlines
        {
            get
            {
                return this.LabText.Inlines;
            }
        }

        public string Text
        {
            get
            {
                return Conversions.ToString(base.GetValue(MyButton.TextProperty));
            }
            set
            {
                base.SetValue(MyButton.TextProperty, value);
            }
        }

        public Thickness TextPadding
        {
            get
            {
                return this.LabText.Padding;
            }
            set
            {
                this.LabText.Padding = value;
            }
        }

        public MyButton.ColorState ColorType
        {
            get
            {
                return this._ColorType;
            }
            set
            {
                this._ColorType = value;
                this.RefreshColor(null, null);
            }
        }

        public new Thickness Padding
        {
            get
            {
                return this.PanFore.Padding;
            }
            set
            {
                this.PanFore.Padding = value;
            }
        }

        public Transform RealRenderTransform
        {
            get
            {
                return this.PanFore.RenderTransform;
            }
            set
            {
                this.PanFore.RenderTransform = value;
            }
        }

        private void RefreshColor(object obj = null, object e = null)
        {
            try
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    if (base.IsEnabled)
                    {
                        switch (this.ColorType)
                        {
                        case MyButton.ColorState.Normal:
                            if (base.IsMouseOver)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrush3", 100, 0, null, false) }, "MyButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrush1", 200, 0, null, false) }, "MyButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            break;
                        case MyButton.ColorState.Highlight:
                            if (base.IsMouseOver)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrush3", 100, 0, null, false) }, "MyButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrush2", 200, 0, null, false) }, "MyButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            break;
                        case MyButton.ColorState.Red:
                            if (base.IsMouseOver)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrushRedLight", 100, 0, null, false) }, "MyButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, "ColorBrushRedDark", 200, 0, null, false) }, "MyButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            break;
                        }
                    }
                    else
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaColor(this.PanFore, Border.BorderBrushProperty, ModSecret.ColorGray4 - this.PanFore.BorderBrush, 200, 0, null, false) }, "MyButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                }
                else
                {
                    ModAnimation.AniStop("MyButton Color " + Conversions.ToString(this.Uuid));
                    if (base.IsEnabled)
                    {
                        switch (this.ColorType)
                        {
                        case MyButton.ColorState.Normal:
                            if (base.IsMouseOver)
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush3");
                            }
                            else
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush1");
                            }
                            break;
                        case MyButton.ColorState.Highlight:
                            if (base.IsMouseOver)
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush3");
                            }
                            else
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrush2");
                            }
                            break;
                        case MyButton.ColorState.Red:
                            if (base.IsMouseOver)
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrushRedLight");
                            }
                            else
                            {
                                this.PanFore.SetResourceReference(Border.BorderBrushProperty, "ColorBrushRedDark");
                            }
                            break;
                        }
                    }
                    else
                    {
                        this.PanFore.BorderBrush = ModSecret.ColorGray4;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "刷新按钮颜色出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyButton.xaml.vb");
            }
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseDown)
            {
                Logger.Info(string.Format("按下按钮：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyButton.xaml.vb");
                MyButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                this.RaiseCustomEvent();
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseDown = true;
            base.Focus();
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaScaleTransform(this.PanFore, 0.955 - ((ScaleTransform)this.PanFore.RenderTransform).ScaleX, 80, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), false),
                ModAnimation.AaScaleTransform(this.PanFore, -0.01, 700, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
            }, "MyButton Scale " + Conversions.ToString(this.Uuid), false);
        }

        private void Button_MouseEnter()
        {
            ModAnimation.AniStart(ModAnimation.AaColor(this.PanFore, Border.BackgroundProperty, (this._ColorType == MyButton.ColorState.Red) ? "ColorBrushRedBack" : "ColorBrush7", 100, 0, null, false), "MyButton Background " + Conversions.ToString(this.Uuid), false);
        }

        private void Button_MouseUp()
        {
            if (this.IsMouseDown)
            {
                this.IsMouseDown = false;
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanFore, 1.0 - ((ScaleTransform)this.PanFore.RenderTransform).ScaleX, 300, 10, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false) }, "MyButton Scale " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Button_MouseLeave()
        {
            ModAnimation.AniStart(ModAnimation.AaColor(this.PanFore, Border.BackgroundProperty, "ColorBrushHalfWhite", 200, 0, null, false), "MyButton Background " + Conversions.ToString(this.Uuid), false);
            if (this.IsMouseDown)
            {
                this.IsMouseDown = false;
                ModAnimation.AniStart(ModAnimation.AaScaleTransform(this.PanFore, 1.0 - ((ScaleTransform)this.PanFore.RenderTransform).ScaleX, 800, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false), "MyButton Scale " + Conversions.ToString(this.Uuid), false);
            }
        }

        internal virtual MyButton PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Border PanFore
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabText
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public int Uuid;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MyButton), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender != null)
            {
                ((MyButton)sender).LabText.Text = Conversions.ToString(e.NewValue);
            }
        }));

        private MyButton.ColorState _ColorType;

        public new static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(MyButton), new PropertyMetadata(delegate(DependencyObject a0, DependencyPropertyChangedEventArgs a1)
        {
            ((MyButton._Closure$__.$I0-1 == null) ? (MyButton._Closure$__.$I0-1 = delegate(MyButton sender, DependencyPropertyChangedEventArgs e)
            {
                if (sender != null)
                {
                    Border panFore = sender.PanFore;
                    object newValue = e.NewValue;
                    panFore.Padding = ((newValue != null) ? ((Thickness)newValue) : default(Thickness));
                }
            }) : MyButton._Closure$__.$I0-1)((MyButton)a0, a1);
        }));

        private const int AnimationColorIn = 100;

        private const int AnimationColorOut = 200;

        private bool IsMouseDown;

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);

        public enum ColorState
        {
            Normal,
            Highlight,
            Red
        }
    }
}

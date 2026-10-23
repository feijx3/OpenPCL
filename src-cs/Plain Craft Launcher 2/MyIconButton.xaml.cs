using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class MyIconButton : Border
    {
        public MyIconButton()
        {
            base.MouseLeftButtonUp += this.Button_MouseUp;
            base.MouseLeftButtonDown += this.Button_MouseDown;
            base.MouseLeftButtonUp += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.Button_MouseUp();
            };
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.Button_MouseLeave();
            };
            base.MouseEnter += delegate(object a0, MouseEventArgs a1)
            {
                this.RefreshAnim();
            };
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.RefreshAnim();
            };
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.RefreshAnim();
            };
            base.IsEnabledChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.RefreshAnim();
            };
            this.Uuid = ModBase.GetUuid();
            this._LogoScale = 1.0;
            this.Theme = MyIconButton.Themes.Color;
            this._Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            this.IsMouseDown = false;
            this.CanClick = true;
            this.InitializeComponent();
        }

        public event MyIconButton.ClickEventHandler Click;

        public string Logo
        {
            get
            {
                return this.Path.Data.ToString();
            }
            set
            {
                this.Path.Data = (Geometry)new GeometryConverter().ConvertFromString(value);
            }
        }

        public double LogoScale
        {
            get
            {
                return this._LogoScale;
            }
            set
            {
                this._LogoScale = value;
                if (!Information.IsNothing(this.Path))
                {
                    this.Path.RenderTransform = new ScaleTransform
                    {
                        ScaleX = this.LogoScale,
                        ScaleY = this.LogoScale
                    };
                }
            }
        }

        public MyIconButton.Themes Theme { get; set; }

        public SolidColorBrush Foreground
        {
            get
            {
                return this._Foreground;
            }
            set
            {
                this._Foreground = value;
                checked
                {
                    ModAnimation.AniControlEnabled++;
                    this.RefreshAnim();
                    ModAnimation.AniControlEnabled--;
                }
            }
        }

        public bool CanClick { get; set; }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CanClick && this.IsMouseDown)
            {
                Logger.Info(string.Format("按下图标按钮{0}", string.IsNullOrEmpty(base.Name) ? "" : ("：" + base.Name)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyIconButton.xaml.vb");
                MyIconButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                e.Handled = true;
                this.Button_MouseUp();
                this.RaiseCustomEvent();
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.CanClick)
            {
                this.IsMouseDown = true;
                base.Focus();
                ModAnimation.AniStart(ModAnimation.AaScaleTransform(this.PanBack, 0.8 - ((ScaleTransform)this.PanBack.RenderTransform).ScaleX, 400, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false), "MyIconButton Scale " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void Button_MouseUp()
        {
            if (this.CanClick)
            {
                if (this.IsMouseDown)
                {
                    this.IsMouseDown = false;
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScaleTransform(this.PanBack, 1.05 - ((ScaleTransform)this.PanBack.RenderTransform).ScaleX, 250, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                        ModAnimation.AaScaleTransform(this.PanBack, -0.05, 250, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false)
                    }, "MyIconButton Scale " + Conversions.ToString(this.Uuid), false);
                }
                this.RefreshAnim();
            }
        }

        private void Button_MouseLeave()
        {
            if (this.CanClick)
            {
                this.IsMouseDown = false;
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanBack, 1.0 - ((ScaleTransform)this.PanBack.RenderTransform).ScaleX, 250, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false) }, "MyIconButton Scale " + Conversions.ToString(this.Uuid), false);
                this.RefreshAnim();
            }
        }

        public void RefreshAnim()
        {
            try
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    if (this.PanBack.Background == null)
                    {
                        this.PanBack.Background = new ModBase.MyColor(0.0, 255.0, 255.0, 255.0);
                    }
                    if (this.Path.Fill == null)
                    {
                        switch (this.Theme)
                        {
                        case MyIconButton.Themes.Black:
                            this.Path.Fill = new ModBase.MyColor(160.0, 0.0, 0.0, 0.0);
                            break;
                        case MyIconButton.Themes.Red:
                            this.Path.Fill = new ModBase.MyColor(160.0, 255.0, 76.0, 76.0);
                            break;
                        case MyIconButton.Themes.Custom:
                            this.Path.Fill = new ModBase.MyColor(160.0, this.Foreground);
                            break;
                        }
                    }
                    List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
                    if (!base.IsEnabled)
                    {
                        list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, "ColorBrushGray5", 120, 0, null, false));
                    }
                    else if (base.IsMouseOver)
                    {
                        switch (this.Theme)
                        {
                        case MyIconButton.Themes.Color:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, "ColorBrush2", 120, 0, null, false));
                            break;
                        case MyIconButton.Themes.White:
                            list.Add(ModAnimation.AaColor(this.PanBack, Border.BackgroundProperty, new ModBase.MyColor(50.0, 255.0, 255.0, 255.0) - this.PanBack.Background, 120, 0, null, false));
                            break;
                        case MyIconButton.Themes.Black:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.MyColor(230.0, 0.0, 0.0, 0.0) - this.Path.Fill, 120, 0, null, false));
                            break;
                        case MyIconButton.Themes.Red:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.MyColor(255.0, 76.0, 76.0) - this.Path.Fill, 120, 0, null, false));
                            break;
                        case MyIconButton.Themes.Custom:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.MyColor(255.0, this.Foreground) - this.Path.Fill, 120, 0, null, false));
                            break;
                        }
                    }
                    else
                    {
                        switch (this.Theme)
                        {
                        case MyIconButton.Themes.Color:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, "ColorBrush4", 150, 0, null, false));
                            this.PanBack.Background = new ModBase.MyColor(0.0, 255.0, 255.0, 255.0);
                            break;
                        case MyIconButton.Themes.White:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, "ColorBrush8", 150, 0, null, false));
                            list.Add(ModAnimation.AaColor(this.PanBack, Border.BackgroundProperty, new ModBase.MyColor(0.0, 255.0, 255.0, 255.0) - this.PanBack.Background, 150, 0, null, false));
                            break;
                        case MyIconButton.Themes.Black:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.MyColor(160.0, 0.0, 0.0, 0.0) - this.Path.Fill, 150, 0, null, false));
                            this.PanBack.Background = new ModBase.MyColor(0.0, 255.0, 255.0, 255.0);
                            break;
                        case MyIconButton.Themes.Red:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.MyColor(160.0, 255.0, 76.0, 76.0) - this.Path.Fill, 150, 0, null, false));
                            this.PanBack.Background = new ModBase.MyColor(0.0, 255.0, 255.0, 255.0);
                            break;
                        case MyIconButton.Themes.Custom:
                            list.Add(ModAnimation.AaColor(this.Path, Shape.FillProperty, new ModBase.MyColor(160.0, this.Foreground) - this.Path.Fill, 150, 0, null, false));
                            this.PanBack.Background = new ModBase.MyColor(0.0, 255.0, 255.0, 255.0);
                            break;
                        }
                    }
                    ModAnimation.AniStart(list, "MyIconButton Color " + Conversions.ToString(this.Uuid), false);
                }
                else
                {
                    ModAnimation.AniStop("MyIconButton Color " + Conversions.ToString(this.Uuid));
                    if (base.IsEnabled)
                    {
                        switch (this.Theme)
                        {
                        case MyIconButton.Themes.Color:
                            this.Path.SetResourceReference(Shape.FillProperty, "ColorBrush5");
                            break;
                        case MyIconButton.Themes.White:
                            this.Path.SetResourceReference(Shape.FillProperty, "ColorBrush8");
                            break;
                        case MyIconButton.Themes.Black:
                            this.Path.Fill = new ModBase.MyColor(160.0, 0.0, 0.0, 0.0);
                            break;
                        case MyIconButton.Themes.Red:
                            this.Path.Fill = new ModBase.MyColor(160.0, 255.0, 76.0, 76.0);
                            break;
                        case MyIconButton.Themes.Custom:
                            this.Path.Fill = new ModBase.MyColor(160.0, this.Foreground);
                            break;
                        }
                    }
                    else
                    {
                        this.Path.SetResourceReference(Shape.FillProperty, "ColorBrushGray5");
                    }
                    this.PanBack.Background = new ModBase.MyColor(0.0, 255.0, 255.0, 255.0);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "刷新图标按钮动画状态出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyIconButton.xaml.vb");
            }
        }

        internal virtual Border PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path Path
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public int Uuid;

        private double _LogoScale;

        private SolidColorBrush _Foreground;

        private bool IsMouseDown;

        private const int AnimationColorIn = 120;

        private const int AnimationColorOut = 150;

        public delegate void ClickEventHandler(object sender, EventArgs e);

        public enum Themes
        {
            Color,
            White,
            Black,
            Red,
            Custom
        }
    }
}

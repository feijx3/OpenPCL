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
using System.Windows.Shapes;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [ContentProperty("Inlines")]
    [DesignerGenerated]
    public partial class MyExtraTextButton : Grid
    {
        public MyExtraTextButton()
        {
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.RefreshColor();
            };
            base.IsEnabledChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.RefreshColor();
            };
            this.Uuid = ModBase.GetUuid();
            this._Logo = "";
            this._LogoScale = 1.0;
            this._Show = false;
            this.IsLeftMouseHeld = false;
            this.InitializeComponent();
        }

        public event MyExtraTextButton.ClickEventHandler Click;

        public string Logo
        {
            get
            {
                return this._Logo;
            }
            set
            {
                if (Operators.CompareString(value, this._Logo, false) != 0)
                {
                    this._Logo = value;
                    this.Path.Data = (Geometry)new GeometryConverter().ConvertFromString(value);
                }
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
                if (this.Path != null)
                {
                    this.Path.RenderTransform = new ScaleTransform
                    {
                        ScaleX = this.LogoScale,
                        ScaleY = this.LogoScale
                    };
                }
            }
        }

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
                return Conversions.ToString(base.GetValue(MyExtraTextButton.TextProperty));
            }
            set
            {
                base.SetValue(MyExtraTextButton.TextProperty, value);
            }
        }

        public bool Show
        {
            get
            {
                return this._Show;
            }
            set
            {
                if (this._Show != value)
                {
                    this._Show = value;
                    ModBase.RunInUi(delegate
                    {
                        if (value)
                        {
                            this.Opacity = 0.0;
                            ModAnimation.AniStart(new ModAnimation.AniData[]
                            {
                                ModAnimation.AaOpacity(this, 1.0 - this.Opacity, 80, 50, null, false),
                                ModAnimation.AaScaleTransform(this, 0.15 - ((ScaleTransform)this.RenderTransform).ScaleX, 400, 50, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false),
                                ModAnimation.AaScaleTransform(this, 0.85, 160, 50, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                            }, "MyExtraTextButton MainScale " + Conversions.ToString(this.Uuid), false);
                        }
                        else
                        {
                            ModAnimation.AniStart(new ModAnimation.AniData[]
                            {
                                ModAnimation.AaOpacity(this, -this.Opacity, 50, 50, null, false),
                                ModAnimation.AaScaleTransform(this, -((ScaleTransform)this.RenderTransform).ScaleX, 100, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false)
                            }, "MyExtraTextButton MainScale " + Conversions.ToString(this.Uuid), false);
                        }
                        this.IsHitTestVisible = value;
                    }, false);
                }
            }
        }

        private void Button_LeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsLeftMouseHeld)
            {
                Logger.Info(string.Format("按下附加图标按钮：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyExtraTextButton.xaml.vb");
                MyExtraTextButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                e.Handled = true;
                this.RaiseCustomEvent();
                this.Button_LeftMouseUp();
            }
        }

        private void Button_LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsLeftMouseHeld)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScaleTransform(this.PanScale, 0.85 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 800, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false),
                    ModAnimation.AaScaleTransform(this.PanScale, -0.05, 60, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                }, "MyExtraTextButton Scale " + Conversions.ToString(this.Uuid), false);
            }
            this.IsLeftMouseHeld = true;
            base.Focus();
        }

        private void Button_LeftMouseUp()
        {
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 300, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false) }, "MyExtraTextButton Scale " + Conversions.ToString(this.Uuid), false);
            this.IsLeftMouseHeld = false;
            this.RefreshColor();
        }

        private void Button_RightMouseUp()
        {
            if (!this.IsLeftMouseHeld)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 300, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false) }, "MyExtraTextButton Scale " + Conversions.ToString(this.Uuid), false);
            }
            this.RefreshColor();
        }

        private void Button_MouseLeave()
        {
            this.IsLeftMouseHeld = false;
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 500, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false) }, "MyExtraTextButton Scale " + Conversions.ToString(this.Uuid), false);
            this.RefreshColor();
        }

        public void RefreshColor()
        {
            try
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                {
                    if (!base.IsEnabled)
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrushGray4", 120, 0, null, false), "MyExtraTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (base.IsMouseOver)
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrush4", 120, 0, null, false), "MyExtraTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrush3", 150, 0, null, false), "MyExtraTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                }
                else
                {
                    ModAnimation.AniStop("MyExtraTextButton Color " + Conversions.ToString(this.Uuid));
                    if (!base.IsEnabled)
                    {
                        this.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrushGray4");
                    }
                    else if (base.IsMouseOver)
                    {
                        this.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrush4");
                    }
                    else
                    {
                        this.PanColor.SetResourceReference(Panel.BackgroundProperty, "ColorBrush3");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "刷新附加图标按钮颜色出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyExtraTextButton.xaml.vb");
            }
        }

        internal virtual MyExtraTextButton PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Border PanClick
        {
            [CompilerGenerated]
            get
            {
                return this._PanClick;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.Button_LeftMouseUp);
                MouseButtonEventHandler mouseButtonEventHandler2 = new MouseButtonEventHandler(this.Button_LeftMouseDown);
                MouseButtonEventHandler mouseButtonEventHandler3 = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.Button_LeftMouseUp();
                };
                MouseButtonEventHandler mouseButtonEventHandler4 = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.Button_RightMouseUp();
                };
                MouseEventHandler mouseEventHandler = delegate(object a0, MouseEventArgs a1)
                {
                    this.Button_MouseLeave();
                };
                MouseEventHandler mouseEventHandler2 = delegate(object a0, MouseEventArgs a1)
                {
                    this.RefreshColor();
                };
                MouseEventHandler mouseEventHandler3 = delegate(object a0, MouseEventArgs a1)
                {
                    this.RefreshColor();
                };
                Border border = this._PanClick;
                if (border != null)
                {
                    border.MouseLeftButtonUp -= mouseButtonEventHandler;
                    border.MouseLeftButtonDown -= mouseButtonEventHandler2;
                    border.MouseLeftButtonUp -= mouseButtonEventHandler3;
                    border.MouseRightButtonUp -= mouseButtonEventHandler4;
                    border.MouseLeave -= mouseEventHandler;
                    border.MouseEnter -= mouseEventHandler2;
                    border.MouseLeave -= mouseEventHandler3;
                }
                this._PanClick = value;
                border = this._PanClick;
                if (border != null)
                {
                    border.MouseLeftButtonUp += mouseButtonEventHandler;
                    border.MouseLeftButtonDown += mouseButtonEventHandler2;
                    border.MouseLeftButtonUp += mouseButtonEventHandler3;
                    border.MouseRightButtonUp += mouseButtonEventHandler4;
                    border.MouseLeave += mouseEventHandler;
                    border.MouseEnter += mouseEventHandler2;
                    border.MouseLeave += mouseEventHandler3;
                }
            }
        }

        internal virtual Grid PanScale
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Border PanColor
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path Path
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

        private string _Logo;

        private double _LogoScale;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MyExtraTextButton), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender != null)
            {
                ((MyExtraTextButton)sender).LabText.Text = Conversions.ToString(e.NewValue);
            }
        }));

        private bool _Show;

        private bool IsLeftMouseHeld;

        private const int AnimationColorIn = 120;

        private const int AnimationColorOut = 150;

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);
    }
}

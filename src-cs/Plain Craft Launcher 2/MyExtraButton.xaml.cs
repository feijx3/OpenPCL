using System;
using System.CodeDom.Compiler;
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
    public partial class MyExtraButton : Grid
    {
        public MyExtraButton()
        {
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.RefreshColor();
            };
            base.IsEnabledChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.RefreshColor();
            };
            this._Progress = 0.0;
            this.Uuid = ModBase.GetUuid();
            this._Logo = "";
            this._LogoScale = 1.0;
            this._Show = false;
            this.ShowCheck = null;
            this._CanRightClick = false;
            this.IsLeftMouseHeld = false;
            this.IsRightMouseHeld = false;
            this.InitializeComponent();
        }

        public event MyExtraButton.ClickEventHandler Click;

        public event MyExtraButton.RightClickEventHandler RightClick;

        public double Progress
        {
            get
            {
                return this._Progress;
            }
            set
            {
                if (this._Progress != value)
                {
                    this._Progress = value;
                    if (value < 0.0001)
                    {
                        this.PanProgress.Visibility = Visibility.Collapsed;
                        return;
                    }
                    this.PanProgress.Visibility = Visibility.Visible;
                    this.RectProgress.Rect = new Rect(0.0, 40.0 * (1.0 - value), 40.0, 40.0 * value);
                }
            }
        }

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
                            this.Visibility = Visibility.Visible;
                            ModAnimation.AniStart(new ModAnimation.AniData[]
                            {
                                ModAnimation.AaScaleTransform(this, 0.3 - ((ScaleTransform)this.RenderTransform).ScaleX, 500, 60, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false),
                                ModAnimation.AaScaleTransform(this, 0.7, 500, 60, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false),
                                ModAnimation.AaHeight(this, 50.0 - this.Height, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false)
                            }, "MyExtraButton MainScale " + Conversions.ToString(this.Uuid), false);
                        }
                        else
                        {
                            ModAnimation.AniStart(new ModAnimation.AniData[]
                            {
                                ModAnimation.AaScaleTransform(this, -((ScaleTransform)this.RenderTransform).ScaleX, 100, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false),
                                ModAnimation.AaHeight(this, -this.Height, 400, 100, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                                ModAnimation.AaCode(delegate
                                {
                                    base.Visibility = Visibility.Collapsed;
                                }, 0, true)
                            }, "MyExtraButton MainScale " + Conversions.ToString(this.Uuid), false);
                        }
                        this.IsHitTestVisible = value;
                    }, false);
                }
            }
        }

        public void ShowRefresh()
        {
            if (this.ShowCheck != null)
            {
                this.Show = this.ShowCheck();
            }
        }

        private void Button_LeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsLeftMouseHeld)
            {
                Logger.Info(string.Format("按下附加按钮{0}", RuntimeHelpers.GetObjectValue(Operators.ConditionalCompareObjectEqual(base.ToolTip, "", false) ? "" : Operators.ConcatenateObject("：", base.ToolTip))), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyExtraButton.xaml.vb");
                MyExtraButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                e.Handled = true;
                this.Button_LeftMouseUp();
            }
        }

        private void Button_RightMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsRightMouseHeld)
            {
                Logger.Info(string.Format("右键按下附加按钮{0}", RuntimeHelpers.GetObjectValue(Operators.ConditionalCompareObjectEqual(base.ToolTip, "", false) ? "" : Operators.ConcatenateObject("：", base.ToolTip))), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyExtraButton.xaml.vb");
                MyExtraButton.RightClickEventHandler rightClickEvent = this.RightClickEvent;
                if (rightClickEvent != null)
                {
                    rightClickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                e.Handled = true;
                this.Button_RightMouseUp();
            }
        }

        public bool CanRightClick
        {
            get
            {
                return this._CanRightClick;
            }
            set
            {
                this._CanRightClick = value;
            }
        }

        private void Button_LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsLeftMouseHeld && !this.IsRightMouseHeld)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScaleTransform(this.PanScale, 0.85 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 800, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false),
                    ModAnimation.AaScaleTransform(this.PanScale, -0.05, 60, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                }, "MyExtraButton Scale " + Conversions.ToString(this.Uuid), false);
            }
            this.IsLeftMouseHeld = true;
            base.Focus();
        }

        private void Button_RightMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.CanRightClick)
            {
                if (!this.IsLeftMouseHeld && !this.IsRightMouseHeld)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[]
                    {
                        ModAnimation.AaScaleTransform(this.PanScale, 0.85 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 800, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Strong), false),
                        ModAnimation.AaScaleTransform(this.PanScale, -0.05, 60, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                    }, "MyExtraButton Scale " + Conversions.ToString(this.Uuid), false);
                }
                this.IsRightMouseHeld = true;
                base.Focus();
            }
        }

        private void Button_LeftMouseUp()
        {
            if (!this.IsRightMouseHeld)
            {
                ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 300, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false) }, "MyExtraButton Scale " + Conversions.ToString(this.Uuid), false);
            }
            if (this.IsLeftMouseHeld)
            {
                this.RaiseCustomEvent();
            }
            this.IsLeftMouseHeld = false;
            this.RefreshColor();
        }

        private void Button_RightMouseUp()
        {
            if (this.CanRightClick)
            {
                if (!this.IsLeftMouseHeld)
                {
                    ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 300, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Middle), false) }, "MyExtraButton Scale " + Conversions.ToString(this.Uuid), false);
                }
                this.IsRightMouseHeld = false;
                this.RefreshColor();
            }
        }

        private void Button_MouseLeave()
        {
            this.IsLeftMouseHeld = false;
            this.IsRightMouseHeld = false;
            ModAnimation.AniStart(new ModAnimation.AniData[] { ModAnimation.AaScaleTransform(this.PanScale, 1.0 - ((ScaleTransform)this.PanScale.RenderTransform).ScaleX, 500, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false) }, "MyExtraButton Scale " + Conversions.ToString(this.Uuid), false);
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
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrushGray4", 120, 0, null, false), "MyExtraButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (base.IsMouseOver)
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrush4", 120, 0, null, false), "MyExtraButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this.PanColor, Panel.BackgroundProperty, "ColorBrush3", 150, 0, null, false), "MyExtraButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                }
                else
                {
                    ModAnimation.AniStop("MyExtraButton Color " + Conversions.ToString(this.Uuid));
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
                Logger.Warn(ex, "刷新图标按钮颜色出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyExtraButton.xaml.vb");
            }
        }

        public void Ribble()
        {
            ModBase.RunInUi(delegate
            {
                Border Shape = new Border
                {
                    CornerRadius = new CornerRadius(1000.0),
                    BorderThickness = new Thickness(0.001),
                    Opacity = 0.5,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = new ScaleTransform()
                };
                Shape.SetResourceReference(Border.BackgroundProperty, "ColorBrush5");
                this.PanScale.Children.Insert(0, Shape);
                ModAnimation.AniStart(new ModAnimation.AniData[]
                {
                    ModAnimation.AaScaleTransform(Shape, 13.0, 1000, 0, new ModAnimation.AniEaseInoutFluent(ModAnimation.AniEasePower.Strong, 0.3), false),
                    ModAnimation.AaOpacity(Shape, -Shape.Opacity, 1000, 0, null, false),
                    ModAnimation.AaCode(delegate
                    {
                        this.PanScale.Children.Remove(Shape);
                    }, 0, true)
                }, "ExtraButton Ribble " + Conversions.ToString(ModBase.GetUuid()), false);
            }, false);
        }

        internal virtual MyExtraButton PanBack
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
                MouseButtonEventHandler mouseButtonEventHandler2 = new MouseButtonEventHandler(this.Button_RightMouseUp);
                MouseButtonEventHandler mouseButtonEventHandler3 = new MouseButtonEventHandler(this.Button_LeftMouseDown);
                MouseButtonEventHandler mouseButtonEventHandler4 = new MouseButtonEventHandler(this.Button_RightMouseDown);
                MouseButtonEventHandler mouseButtonEventHandler5 = delegate(object a0, MouseButtonEventArgs a1)
                {
                    this.Button_LeftMouseUp();
                };
                MouseButtonEventHandler mouseButtonEventHandler6 = delegate(object a0, MouseButtonEventArgs a1)
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
                    border.MouseRightButtonUp -= mouseButtonEventHandler2;
                    border.MouseLeftButtonDown -= mouseButtonEventHandler3;
                    border.MouseRightButtonDown -= mouseButtonEventHandler4;
                    border.MouseLeftButtonUp -= mouseButtonEventHandler5;
                    border.MouseRightButtonUp -= mouseButtonEventHandler6;
                    border.MouseLeave -= mouseEventHandler;
                    border.MouseEnter -= mouseEventHandler2;
                    border.MouseLeave -= mouseEventHandler3;
                }
                this._PanClick = value;
                border = this._PanClick;
                if (border != null)
                {
                    border.MouseLeftButtonUp += mouseButtonEventHandler;
                    border.MouseRightButtonUp += mouseButtonEventHandler2;
                    border.MouseLeftButtonDown += mouseButtonEventHandler3;
                    border.MouseRightButtonDown += mouseButtonEventHandler4;
                    border.MouseLeftButtonUp += mouseButtonEventHandler5;
                    border.MouseRightButtonUp += mouseButtonEventHandler6;
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

        internal virtual Border PanProgress
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual RectangleGeometry RectProgress
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path Path
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        private double _Progress;

        public int Uuid;

        private string _Logo;

        private double _LogoScale;

        private bool _Show;

        public MyExtraButton.ShowCheckDelegate ShowCheck;

        private bool _CanRightClick;

        private bool IsLeftMouseHeld;

        private bool IsRightMouseHeld;

        private const int AnimationColorIn = 120;

        private const int AnimationColorOut = 150;

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);

        public delegate void RightClickEventHandler(object sender, MouseButtonEventArgs e);

        public delegate bool ShowCheckDelegate();
    }
}

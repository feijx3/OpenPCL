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
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [ContentProperty("Inlines")]
    [DesignerGenerated]
    public partial class MyIconTextButton : Border
    {
        public MyIconTextButton()
        {
            base.MouseLeftButtonUp += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.MyIconTextButton_MouseUp();
            };
            base.MouseLeftButtonDown += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.MyIconTextButton_MouseDown();
            };
            base.MouseLeave += delegate(object a0, MouseEventArgs a1)
            {
                this.MyIconTextButton_MouseLeave();
            };
            base.MouseEnter += new MouseEventHandler(this.RefreshColor);
            base.Loaded += new RoutedEventHandler(this.RefreshColor);
            base.IsEnabledChanged += delegate(object a0, DependencyPropertyChangedEventArgs a1)
            {
                this.RefreshColor(RuntimeHelpers.GetObjectValue(a0), a1);
            };
            this.Uuid = ModBase.GetUuid();
            this._LogoScale = 1.0;
            this.IsMouseDown = false;
            this.InitializeComponent();
        }

        public event MyIconTextButton.CheckEventHandler Check;

        public event MyIconTextButton.ChangeEventHandler Change;

        public string Logo
        {
            get
            {
                return this.ShapeLogo.Data.ToString();
            }
            set
            {
                this.ShapeLogo.Data = (Geometry)new GeometryConverter().ConvertFromString(value);
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
                if (!Information.IsNothing(this.ShapeLogo))
                {
                    this.ShapeLogo.RenderTransform = new ScaleTransform
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
                return Conversions.ToString(base.GetValue(MyIconTextButton.TextProperty));
            }
            set
            {
                base.SetValue(MyIconTextButton.TextProperty, value);
            }
        }

        public MyIconTextButton.ColorState ColorType
        {
            get
            {
                return (MyIconTextButton.ColorState)Conversions.ToInteger(base.GetValue(MyIconTextButton.ColorTypeProperty));
            }
            set
            {
                if (this.ColorType != value)
                {
                    base.SetValue(MyIconTextButton.ColorTypeProperty, value);
                    this.RefreshColor(null, null);
                }
            }
        }

        public event MyIconTextButton.ClickEventHandler Click;

        private void MyIconTextButton_MouseUp()
        {
            if (this.IsMouseDown)
            {
                Logger.Info(string.Format("按下带图标按钮：{0}", this.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyIconTextButton.xaml.vb");
                this.IsMouseDown = false;
                MyIconTextButton.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(this, new ModBase.RouteEventArgs(true));
                }
                this.RaiseCustomEvent();
                this.RefreshColor(null, null);
            }
        }

        private void MyIconTextButton_MouseDown()
        {
            this.IsMouseDown = true;
            this.RefreshColor(null, null);
        }

        private void MyIconTextButton_MouseLeave()
        {
            this.IsMouseDown = false;
            this.RefreshColor(null, null);
        }

        private void RefreshColor(object obj = null, object e = null)
        {
            try
            {
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0 && !false.Equals(RuntimeHelpers.GetObjectValue(e)))
                {
                    MyIconTextButton.ColorState colorType = this.ColorType;
                    if (colorType != MyIconTextButton.ColorState.Black)
                    {
                        if (colorType == MyIconTextButton.ColorState.Highlight)
                        {
                            if (this.IsMouseDown)
                            {
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrush6", 70, 0, null, false), "MyIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else if (base.IsMouseOver)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 100, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, null, false)
                                }, "MyIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrushBg1", 100, 0, null, false), "MyIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else if (base.IsEnabled)
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 150, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 150, 0, null, false)
                                }, "MyIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "MyIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                            else
                            {
                                ModAnimation.AniStart(new ModAnimation.AniData[]
                                {
                                    ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrushGray5", 100, 0, null, false),
                                    ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrushGray5", 100, 0, null, false)
                                }, "MyIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                                ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "MyIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                            }
                        }
                    }
                    else if (this.IsMouseDown)
                    {
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrush6", 70, 0, null, false), "MyIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (base.IsMouseOver)
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush3", 100, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush3", 100, 0, null, false)
                        }, "MyIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, "ColorBrushBg1", 100, 0, null, false), "MyIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else if (base.IsEnabled)
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrush1", 150, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrush1", 150, 0, null, false)
                        }, "MyIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "MyIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                    else
                    {
                        ModAnimation.AniStart(new ModAnimation.AniData[]
                        {
                            ModAnimation.AaColor(this.ShapeLogo, Shape.FillProperty, "ColorBrushGray5", 100, 0, null, false),
                            ModAnimation.AaColor(this.LabText, TextBlock.ForegroundProperty, "ColorBrushGray5", 100, 0, null, false)
                        }, "MyIconTextButton Checked " + Conversions.ToString(this.Uuid), false);
                        ModAnimation.AniStart(ModAnimation.AaColor(this, Border.BackgroundProperty, ModSecret.ColorSemiTransparent - base.Background, 150, 0, null, false), "MyIconTextButton Color " + Conversions.ToString(this.Uuid), false);
                    }
                }
                else
                {
                    ModAnimation.AniStop("MyIconTextButton Checked " + Conversions.ToString(this.Uuid));
                    ModAnimation.AniStop("MyIconTextButton Color " + Conversions.ToString(this.Uuid));
                    MyIconTextButton.ColorState colorType2 = this.ColorType;
                    if (colorType2 != MyIconTextButton.ColorState.Black)
                    {
                        if (colorType2 == MyIconTextButton.ColorState.Highlight)
                        {
                            base.Background = ModSecret.ColorSemiTransparent;
                            this.ShapeLogo.SetResourceReference(Shape.FillProperty, base.IsEnabled ? "ColorBrush3" : "ColorBrushGray5");
                            this.LabText.SetResourceReference(TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush3" : "ColorBrushGray5");
                        }
                    }
                    else
                    {
                        base.Background = ModSecret.ColorSemiTransparent;
                        this.ShapeLogo.SetResourceReference(Shape.FillProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5");
                        this.LabText.SetResourceReference(TextBlock.ForegroundProperty, base.IsEnabled ? "ColorBrush1" : "ColorBrushGray5");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "刷新带图标按钮颜色出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyIconTextButton.xaml.vb");
            }
        }

        internal virtual MyIconTextButton PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path ShapeLogo
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

        private double _LogoScale;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MyIconTextButton), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!Information.IsNothing(sender))
            {
                ((MyIconTextButton)sender).LabText.Text = Conversions.ToString(e.NewValue);
            }
        }));

        public static readonly DependencyProperty ColorTypeProperty = DependencyProperty.Register("ColorType", typeof(MyIconTextButton.ColorState), typeof(MyIconTextButton), new PropertyMetadata(MyIconTextButton.ColorState.Black));

        private bool IsMouseDown;

        private const int AnimationTimeOfMouseIn = 100;

        private const int AnimationTimeOfMouseOut = 150;

        public delegate void CheckEventHandler(object sender, bool raiseByMouse);

        public delegate void ChangeEventHandler(object sender, bool raiseByMouse);

        public enum ColorState
        {
            Black,
            Highlight
        }

        public delegate void ClickEventHandler(object sender, ModBase.RouteEventArgs e);
    }
}

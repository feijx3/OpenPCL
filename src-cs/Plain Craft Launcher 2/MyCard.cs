using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class MyCard : Grid
    {
        public MyDropShadow MainChrome { get; }

        public UIElement BorderChild
        {
            get
            {
                return this.MainBorder.Child;
            }
            set
            {
                this.MainBorder.Child = value;
            }
        }

        public TextBlock MainTextBlock
        {
            get
            {
                this.Init();
                return this._MainTextBlock;
            }
            set
            {
                this._MainTextBlock = value;
            }
        }

        public Path MainSwap
        {
            get
            {
                this.Init();
                return this._MainSwap;
            }
            set
            {
                this._MainSwap = value;
            }
        }

        public InlineCollection Inlines
        {
            get
            {
                return this.MainTextBlock.Inlines;
            }
        }

        public CornerRadius CornerRadius
        {
            get
            {
                return this.MainChrome.CornerRadius;
            }
            set
            {
                this.MainChrome.CornerRadius = value;
                this.MainBorder.CornerRadius = value;
            }
        }

        public string Title
        {
            get
            {
                return Conversions.ToString(base.GetValue(MyCard.TitleProperty));
            }
            set
            {
                base.SetValue(MyCard.TitleProperty, value);
                if (this._MainTextBlock != null)
                {
                    this.MainTextBlock.Text = value;
                }
            }
        }

        public MyCard()
        {
            base.Loaded += delegate(object a0, RoutedEventArgs a1)
            {
                this.Init();
            };
            base.MouseEnter += this.MyCard_MouseEnter;
            base.MouseLeave += this.MyCard_MouseLeave;
            base.SizeChanged += this.MySizeChanged;
            base.MouseLeftButtonDown += this.MyCard_MouseLeftButtonDown;
            base.MouseLeftButtonUp += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.MyCard_MouseLeftButtonUp();
            };
            base.MouseLeave += this.MyCard_MouseLeave_Swap;
            base.MouseMove += delegate(object a0, MouseEventArgs a1)
            {
                this.UpdateCursorStyle();
            };
            this.Uuid = ModBase.GetUuid();
            this.IsLoad = false;
            this.HasMouseAnimation = true;
            this.UseAnimation = true;
            this.IsHeightAnimating = false;
            this.CanSwap = false;
            this._IsSwapped = false;
            this.SwapLogoRight = false;
            this.IsSwapMouseDown = false;
            this.IsCustomMouseDown = false;
            this.MainChrome = new MyDropShadow
            {
                Margin = new Thickness(-3.0, -3.0, -3.0, -3.0 - ModBase.GetWPFSize(1.0)),
                ShadowRadius = 3.0,
                Opacity = 0.07,
                CornerRadius = new CornerRadius(5.0)
            };
            this.MainChrome.SetResourceReference(MyDropShadow.ColorProperty, "ColorObject1");
            base.Children.Insert(0, this.MainChrome);
            this.MainBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(245, byte.MaxValue, byte.MaxValue, byte.MaxValue)),
                CornerRadius = new CornerRadius(5.0),
                IsHitTestVisible = false
            };
            base.Children.Insert(1, this.MainBorder);
            this.MainGrid = new Grid();
            base.Children.Add(this.MainGrid);
        }

        private void Init()
        {
            if (!this.IsLoad)
            {
                this.IsLoad = true;
                if (this.MainTextBlock == null)
                {
                    this.MainTextBlock = new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(15.0, 12.0, 0.0, 0.0),
                        FontWeight = FontWeights.Bold,
                        FontSize = 13.0,
                        IsHitTestVisible = false
                    };
                    this.MainTextBlock.SetResourceReference(TextBlock.ForegroundProperty, "ColorBrush1");
                    this.MainTextBlock.SetBinding(TextBlock.TextProperty, new Binding("Title")
                    {
                        Source = this,
                        Mode = BindingMode.OneWay
                    });
                    this.MainGrid.Children.Add(this.MainTextBlock);
                }
                if (this.CanSwap || this.SwapControl != null)
                {
                    if (this.SwapControl == null && base.Children.Count > 3)
                    {
                        this.SwapControl = (FrameworkElement)base.Children[3];
                    }
                    this.MainSwap = new Path
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Stretch = Stretch.Uniform,
                        Height = 6.0,
                        Width = 10.0,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(0.0, 17.0, 16.0, 0.0),
                        Data = (Geometry)new GeometryConverter().ConvertFromString("M2,4 l-2,2 10,10 10,-10 -2,-2 -8,8 -8,-8 z"),
                        RenderTransform = new RotateTransform(180.0),
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };
                    this.MainSwap.SetResourceReference(Shape.FillProperty, "ColorBrush1");
                    this.MainGrid.Children.Add(this.MainSwap);
                }
                if (this.IsSwapped && this.SwapControl != null)
                {
                    this.MainSwap.RenderTransform = new RotateTransform((double)(this.SwapLogoRight ? 270 : 0));
                    this.SwapControl.Visibility = Visibility.Collapsed;
                    bool RawUseAnimation = this.UseAnimation;
                    this.UseAnimation = false;
                    base.Height = 40.0;
                    ModAnimation.AniStop("MyCard Height " + Conversions.ToString(this.Uuid));
                    this.IsHeightAnimating = false;
                    ModBase.RunInUi(delegate
                    {
                        this.UseAnimation = RawUseAnimation;
                    }, true);
                }
            }
        }

        public void SetContentBuilder(Action<Panel> Builder)
        {
            this.ContentBuilder = Builder;
        }

        public void SetContentBuilder<T>(IEnumerable<T> Items, Func<T, UIElement> ItemBuilder)
        {
            MyCard._Closure$__32-0<T> CS$<>8__locals1 = new MyCard._Closure$__32-0<T>(CS$<>8__locals1);
            CS$<>8__locals1.$VB$Local_Items = Items;
            CS$<>8__locals1.$VB$Local_ItemBuilder = ItemBuilder;
            this.SetContentBuilder(delegate(Panel Stack)
            {
                try
                {
                    foreach ($CLS0 $CLS in CS$<>8__locals1.$VB$Local_Items)
                    {
                        Stack.Children.Add(CS$<>8__locals1.$VB$Local_ItemBuilder($CLS));
                    }
                }
                finally
                {
                    IEnumerator<$CLS0> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                Stack.Children.Add(new FrameworkElement
                {
                    Height = 18.0
                });
            });
        }

        public void BuildContent()
        {
            if (this.ContentBuilder != null)
            {
                Panel panel = this.SwapControl as Panel;
                if (panel != null)
                {
                    try
                    {
                        this.ContentBuilder(panel);
                        this.ContentBuilder = null;
                    }
                    catch (Exception ex)
                    {
                        panel.Children.Clear();
                        throw;
                    }
                    this.TriggerForceResize();
                }
            }
        }

        public bool HasMouseAnimation { get; set; }

        private void MyCard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.HasMouseAnimation)
            {
                List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
                if (!Information.IsNothing(this.MainTextBlock))
                {
                    list.Add(ModAnimation.AaColor(this.MainTextBlock, TextBlock.ForegroundProperty, "ColorBrush2", 90, 0, null, false));
                }
                if (!Information.IsNothing(this.MainSwap))
                {
                    list.Add(ModAnimation.AaColor(this.MainSwap, Shape.FillProperty, "ColorBrush2", 90, 0, null, false));
                }
                list.AddRange(new ModAnimation.AniData[]
                {
                    ModAnimation.AaColor(this.MainChrome, MyDropShadow.ColorProperty, "ColorObject4", 90, 0, null, false),
                    ModAnimation.AaOpacity(this.MainChrome, 0.4 - this.MainChrome.Opacity, 90, 0, null, false)
                });
                ModAnimation.AniStart(list, "MyCard Mouse " + Conversions.ToString(this.Uuid), false);
            }
        }

        private void MyCard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.HasMouseAnimation)
            {
                List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
                if (!Information.IsNothing(this.MainTextBlock))
                {
                    list.Add(ModAnimation.AaColor(this.MainTextBlock, TextBlock.ForegroundProperty, "ColorBrush1", 90, 0, null, false));
                }
                if (!Information.IsNothing(this.MainSwap))
                {
                    list.Add(ModAnimation.AaColor(this.MainSwap, Shape.FillProperty, "ColorBrush1", 90, 0, null, false));
                }
                list.AddRange(new ModAnimation.AniData[]
                {
                    ModAnimation.AaColor(this.MainChrome, MyDropShadow.ColorProperty, "ColorObject1", 90, 0, null, false),
                    ModAnimation.AaOpacity(this.MainChrome, 0.07 - this.MainChrome.Opacity, 90, 0, null, false)
                });
                ModAnimation.AniStart(list, "MyCard Mouse " + Conversions.ToString(this.Uuid), false);
            }
        }

        public bool UseAnimation { get; set; }

        private void MySizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.UseAnimation)
            {
                double num = (this.IsSwapped ? 40.0 : e.NewSize.Height) - e.PreviousSize.Height;
                if (e.PreviousSize.Height != 0.0 && !this.IsHeightAnimating && Math.Abs(num) >= 1.0 && base.ActualHeight != 0.0)
                {
                    this.StartHeightAnimation(num, e.PreviousSize.Height, false);
                }
            }
        }

        private void StartHeightAnimation(double Delta, double PreviousHeight, bool IsLoadAnimation)
        {
            checked
            {
                if (!this.IsHeightAnimating && ModMain.FrmMain != null)
                {
                    List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
                    double num = Math.Abs(Delta);
                    if (num <= 800.0)
                    {
                        list.Add(ModAnimation.AaHeight(this, Delta, 150, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), false));
                    }
                    else
                    {
                        int num2;
                        int num3;
                        int num4;
                        if (Delta < 0.0 && unchecked(num - (double)num2) > 500.0)
                        {
                            num2 = 200;
                            num3 = 150;
                            num4 = (int)Math.Round(unchecked(num - (double)num2) / 0.1);
                        }
                        else if (Delta > 0.0 && unchecked(num - (double)num2) > 3000.0)
                        {
                            num4 = 5000;
                            num2 = (int)Math.Round(unchecked(num - (double)num4 * 0.3));
                            num3 = 400;
                        }
                        else
                        {
                            num2 = 150;
                            num3 = 200;
                            num4 = 4000;
                        }
                        list.Add(ModAnimation.AaHeight(this, unchecked((num - (double)num2) * (double)Math.Sign(Delta)), (int)Math.Round(unchecked((num - (double)num2) / (double)num4 * 1000.0)), 0, null, false));
                        list.Add(ModAnimation.AaHeight(this, (double)(num2 * Math.Sign(Delta)), num3, 0, new ModAnimation.AniEaseOutFluentWithInitial((double)num4, (double)num3 / 1000.0, (double)num2), true));
                    }
                    list.Add(ModAnimation.AaCode(delegate
                    {
                        this.IsHeightAnimating = false;
                        base.Height = this.ActualUsedHeight;
                        if (this.IsSwapped && this.SwapControl != null)
                        {
                            this.SwapControl.Visibility = Visibility.Collapsed;
                        }
                    }, 0, true));
                    ModAnimation.AniStart(list, "MyCard Height " + Conversions.ToString(this.Uuid), false);
                    this.IsHeightAnimating = true;
                    this.ActualUsedHeight = (this.IsSwapped ? 40.0 : base.Height);
                    base.Height = PreviousHeight;
                }
            }
        }

        public void TriggerForceResize()
        {
            base.Height = (this.IsSwapped ? 40.0 : double.NaN);
            ModAnimation.AniStop("MyCard Height " + Conversions.ToString(this.Uuid));
            this.IsHeightAnimating = false;
        }

        public bool CanSwap { get; set; }

        public bool IsSwapped
        {
            get
            {
                return this._IsSwapped;
            }
            set
            {
                if (this._IsSwapped != value)
                {
                    this._IsSwapped = value;
                    if (this.SwapControl != null)
                    {
                        if (!this.IsSwapped)
                        {
                            this.BuildContent();
                        }
                        if (base.IsLoaded)
                        {
                            this.SwapControl.Visibility = Visibility.Visible;
                            this.TriggerForceResize();
                            this.UpdateCursorStyle();
                            ModAnimation.AniStart(ModAnimation.AaRotateTransform(this.MainSwap, (double)(this._IsSwapped ? (this.SwapLogoRight ? 270 : 0) : 180) - ((RotateTransform)this.MainSwap.RenderTransform).Angle, 250, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.ExtraStrong), false), "MyCard Swap " + Conversions.ToString(this.Uuid), true);
                        }
                    }
                }
            }
        }

        [Obsolete("IsSwaped 存在拼写错误，请换用 IsSwapped 属性。")]
        public bool IsSwaped
        {
            get
            {
                return Conversions.ToBoolean(base.GetValue(MyCard.IsSwapedProperty));
            }
            set
            {
                base.SetValue(MyCard.IsSwapedProperty, value);
            }
        }

        public bool SwapLogoRight { get; set; }

        public event MyCard.PreviewSwapEventHandler PreviewSwap;

        public event MyCard.SwapEventHandler Swap;

        private void MyCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double y = Mouse.GetPosition(this).Y;
            if (this.IsSwapped || (y <= (double)(this.IsSwapped ? 40 : 34) && (y != 0.0 || base.IsMouseDirectlyOver)))
            {
                this.IsCustomMouseDown = true;
                if (this.IsSwapped || (this.SwapControl != null && y <= (double)(this.IsSwapped ? 40 : 34) && (y != 0.0 || base.IsMouseDirectlyOver)))
                {
                    this.IsSwapMouseDown = true;
                }
            }
        }

        private void MyCard_MouseLeftButtonUp()
        {
            if (this.IsCustomMouseDown)
            {
                this.IsCustomMouseDown = false;
                this.RaiseCustomEvent();
                if (this.IsSwapMouseDown)
                {
                    this.IsSwapMouseDown = false;
                    double y = Mouse.GetPosition(this).Y;
                    if (this.IsSwapped || (this.SwapControl != null && y <= (double)(this.IsSwapped ? 40 : 34) && (y != 0.0 || base.IsMouseDirectlyOver)))
                    {
                        ModBase.RouteEventArgs e = new ModBase.RouteEventArgs(true);
                        MyCard.PreviewSwapEventHandler previewSwapEvent = this.PreviewSwapEvent;
                        if (previewSwapEvent != null)
                        {
                            previewSwapEvent(this, e);
                        }
                        if (e.Handled)
                        {
                            this.IsSwapMouseDown = false;
                            return;
                        }
                        this.IsSwapped = !this.IsSwapped;
                        Logger.Info(string.Format("{0}{1}", this.IsSwapped ? "折叠卡片" : "展开卡片", (this.Title == null) ? "" : ("：" + this.Title)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\MyCard.vb");
                        MyCard.SwapEventHandler swapEvent = this.SwapEvent;
                        if (swapEvent != null)
                        {
                            swapEvent(this, e);
                        }
                    }
                }
            }
        }

        private void MyCard_MouseLeave_Swap(object sender, MouseEventArgs e)
        {
            this.IsSwapMouseDown = false;
        }

        private void UpdateCursorStyle()
        {
            if (this._MainSwap == null)
            {
                base.Cursor = null;
                return;
            }
            double y = Mouse.GetPosition(this).Y;
            base.Cursor = ((this.MainSwap.Visibility == Visibility.Visible && (this.IsSwapped || (y <= 34.0 && (y != 0.0 || base.IsMouseDirectlyOver)))) ? Cursors.Hand : null);
        }

        private readonly Grid MainGrid;

        private readonly Border MainBorder;

        private TextBlock _MainTextBlock;

        private Path _MainSwap;

        public int Uuid;

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MyCard), new PropertyMetadata(""));

        private bool IsLoad;

        private Action<Panel> ContentBuilder;

        private const double DropShadowIdleOpacity = 0.07;

        private const double DropShadowHoverOpacity = 0.4;

        private bool IsHeightAnimating;

        private double ActualUsedHeight;

        public FrameworkElement SwapControl;

        private bool _IsSwapped;

        public static readonly DependencyProperty IsSwapedProperty = DependencyProperty.Register("IsSwaped", typeof(bool), typeof(MyCard), new PropertyMetadata(false, delegate(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender != null && sender is MyCard)
            {
                ((MyCard)sender).IsSwapped = Conversions.ToBoolean(e.NewValue);
            }
        }));

        private bool IsSwapMouseDown;

        private bool IsCustomMouseDown;

        public const int SwapedHeight = 40;

        public delegate void PreviewSwapEventHandler(object sender, ModBase.RouteEventArgs e);

        public delegate void SwapEventHandler(object sender, ModBase.RouteEventArgs e);
    }
}

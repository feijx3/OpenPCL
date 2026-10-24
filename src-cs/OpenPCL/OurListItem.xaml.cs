using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [ContentProperty("Inlines")]
    [DesignerGenerated]
    public partial class OurListItem : Grid, IOurRadio
    {
        public OurListItem()
        {
            base.SizeChanged += this._Lambda___R1_1;
            base.PreviewMouseLeftButtonUp += this.Button_MouseUp;
            base.PreviewMouseLeftButtonDown += this.Button_MouseDown;
            base.MouseLeave += new MouseEventHandler(this.Button_MouseLeave);
            base.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Button_MouseLeave);
            base.MouseEnter += new MouseEventHandler(this.RefreshColor);
            base.MouseLeave += new MouseEventHandler(this.RefreshColor);
            base.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.RefreshColor);
            base.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.RefreshColor);
            base.Loaded += this.OurListItem_Loaded;
            this._RectBack = null;
            this._LabInfo = null;
            this.Uuid = ModBase.GetUuid();
            this._IsScaleAnimationEnabled = true;
            this.MinPaddingRight = 4;
            this._Info = "";
            this._Logo = "";
            this._LogoScale = 1.0;
            this.LogoClickable = false;
            this.IsLogoDown = false;
            this._Type = OurListItem.CheckType.None;
            this._Checked = false;
            this.IsMouseDown = false;
            this.IsMouseOverAnimationEnabled = true;
            this.InitializeComponent();
        }

        public event OurListItem.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurListItem.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurListItem.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurListItem.ClickEventHandler clickEventHandler3 = (OurListItem.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurListItem.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurListItem.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurListItem.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurListItem.ClickEventHandler clickEventHandler3 = (OurListItem.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurListItem.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
        }

        public event OurListItem.LogoClickEventHandler LogoClick
        {
            [CompilerGenerated]
            add
            {
                OurListItem.LogoClickEventHandler logoClickEventHandler = this.LogoClickEvent;
                OurListItem.LogoClickEventHandler logoClickEventHandler2;
                do
                {
                    logoClickEventHandler2 = logoClickEventHandler;
                    OurListItem.LogoClickEventHandler logoClickEventHandler3 = (OurListItem.LogoClickEventHandler)Delegate.Combine(logoClickEventHandler2, value);
                    logoClickEventHandler = Interlocked.CompareExchange<OurListItem.LogoClickEventHandler>(ref this.LogoClickEvent, logoClickEventHandler3, logoClickEventHandler2);
                }
                while (logoClickEventHandler != logoClickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurListItem.LogoClickEventHandler logoClickEventHandler = this.LogoClickEvent;
                OurListItem.LogoClickEventHandler logoClickEventHandler2;
                do
                {
                    logoClickEventHandler2 = logoClickEventHandler;
                    OurListItem.LogoClickEventHandler logoClickEventHandler3 = (OurListItem.LogoClickEventHandler)Delegate.Remove(logoClickEventHandler2, value);
                    logoClickEventHandler = Interlocked.CompareExchange<OurListItem.LogoClickEventHandler>(ref this.LogoClickEvent, logoClickEventHandler3, logoClickEventHandler2);
                }
                while (logoClickEventHandler != logoClickEventHandler2);
            }
        }

        public event IOurRadio.CheckEventHandler Check
        {
            [CompilerGenerated]
            add
            {
                IOurRadio.CheckEventHandler checkEventHandler = this.CheckEvent;
                IOurRadio.CheckEventHandler checkEventHandler2;
                do
                {
                    checkEventHandler2 = checkEventHandler;
                    IOurRadio.CheckEventHandler checkEventHandler3 = (IOurRadio.CheckEventHandler)Delegate.Combine(checkEventHandler2, value);
                    checkEventHandler = Interlocked.CompareExchange<IOurRadio.CheckEventHandler>(ref this.CheckEvent, checkEventHandler3, checkEventHandler2);
                }
                while (checkEventHandler != checkEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                IOurRadio.CheckEventHandler checkEventHandler = this.CheckEvent;
                IOurRadio.CheckEventHandler checkEventHandler2;
                do
                {
                    checkEventHandler2 = checkEventHandler;
                    IOurRadio.CheckEventHandler checkEventHandler3 = (IOurRadio.CheckEventHandler)Delegate.Remove(checkEventHandler2, value);
                    checkEventHandler = Interlocked.CompareExchange<IOurRadio.CheckEventHandler>(ref this.CheckEvent, checkEventHandler3, checkEventHandler2);
                }
                while (checkEventHandler != checkEventHandler2);
            }
        }

        public event IOurRadio.ChangedEventHandler Changed
        {
            [CompilerGenerated]
            add
            {
                IOurRadio.ChangedEventHandler changedEventHandler = this.ChangedEvent;
                IOurRadio.ChangedEventHandler changedEventHandler2;
                do
                {
                    changedEventHandler2 = changedEventHandler;
                    IOurRadio.ChangedEventHandler changedEventHandler3 = (IOurRadio.ChangedEventHandler)Delegate.Combine(changedEventHandler2, value);
                    changedEventHandler = Interlocked.CompareExchange<IOurRadio.ChangedEventHandler>(ref this.ChangedEvent, changedEventHandler3, changedEventHandler2);
                }
                while (changedEventHandler != changedEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                IOurRadio.ChangedEventHandler changedEventHandler = this.ChangedEvent;
                IOurRadio.ChangedEventHandler changedEventHandler2;
                do
                {
                    changedEventHandler2 = changedEventHandler;
                    IOurRadio.ChangedEventHandler changedEventHandler3 = (IOurRadio.ChangedEventHandler)Delegate.Remove(changedEventHandler2, value);
                    changedEventHandler = Interlocked.CompareExchange<IOurRadio.ChangedEventHandler>(ref this.ChangedEvent, changedEventHandler3, changedEventHandler2);
                }
                while (changedEventHandler != changedEventHandler2);
            }
        }

        public Border RectBack
        {
            get
            {
                if (this._RectBack == null)
                {
                    Border border = new Border
                    {
                        Name = "RectBack",
                        CornerRadius = new CornerRadius((double)((this.IsScaleAnimationEnabled || base.Height > 40.0) ? 6 : 0)),
                        RenderTransform = (this.IsScaleAnimationEnabled ? new ScaleTransform(0.8, 0.8) : null),
                        RenderTransformOrigin = new Point(0.5, 0.5),
                        BorderThickness = new Thickness(ModBase.GetWPFSize(1.0)),
                        SnapsToDevicePixels = true,
                        IsHitTestVisible = false,
                        Opacity = 0.0
                    };
                    border.SetResourceReference(Border.BackgroundProperty, "ColorBrush7");
                    border.SetResourceReference(Border.BorderBrushProperty, "ColorBrush6");
                    Grid.SetColumnSpan(border, 999);
                    Grid.SetRowSpan(border, 999);
                    base.Children.Insert(0, border);
                    this._RectBack = border;
                }
                return this._RectBack;
            }
        }

        public TextBlock LabInfo
        {
            get
            {
                if (this._LabInfo == null)
                {
                    TextBlock textBlock = new TextBlock
                    {
                        Name = "LabInfo",
                        SnapsToDevicePixels = false,
                        UseLayoutRounding = false,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        IsHitTestVisible = false,
                        TextTrimming = TextTrimming.CharacterEllipsis,
                        Visibility = Visibility.Collapsed,
                        FontSize = 12.0,
                        Margin = new Thickness(4.0, 0.0, 0.0, 0.0),
                        Opacity = 0.6
                    };
                    Grid.SetColumn(textBlock, 3);
                    Grid.SetRow(textBlock, 2);
                    base.Children.Add(textBlock);
                    this._LabInfo = textBlock;
                }
                return this._LabInfo;
            }
        }

        public bool IsScaleAnimationEnabled
        {
            get
            {
                return this._IsScaleAnimationEnabled;
            }
            set
            {
                this._IsScaleAnimationEnabled = value;
                if (this._RectBack != null)
                {
                    this.RectBack.CornerRadius = new CornerRadius((double)(value ? 6 : 0));
                }
            }
        }

        public int PaddingLeft
        {
            get
            {
                return checked((int)Math.Round(this.ColumnPaddingLeft.Width.Value));
            }
            set
            {
                this.ColumnPaddingLeft.Width = new GridLength((double)value);
            }
        }

        public int MinPaddingRight
        {
            [CompilerGenerated]
            get
            {
                return this._MinPaddingRight;
            }
            [CompilerGenerated]
            set
            {
                this._MinPaddingRight = value;
            }
        }

        public IEnumerable<OurIconButton> Buttons
        {
            get
            {
                return this._Buttons;
            }
            set
            {
                this._Buttons = value;
                if (this.ButtonStack != null)
                {
                    base.Children.Remove(this.ButtonStack);
                    this.ButtonStack = null;
                }
                int num = value.Count<OurIconButton>();
                if (num != 0)
                {
                    if (num == 1)
                    {
                        try
                        {
                            foreach (OurIconButton ourIconButton in value)
                            {
                                if (ourIconButton.Height.Equals(double.NaN))
                                {
                                    ourIconButton.Height = 25.0;
                                }
                                if (ourIconButton.Width.Equals(double.NaN))
                                {
                                    ourIconButton.Width = 25.0;
                                }
                                OurIconButton ourIconButton2 = ourIconButton;
                                ourIconButton2.Opacity = 0.0;
                                ourIconButton2.Margin = new Thickness(0.0, 0.0, 5.0, 0.0);
                                ourIconButton2.SnapsToDevicePixels = false;
                                ourIconButton2.HorizontalAlignment = HorizontalAlignment.Right;
                                ourIconButton2.VerticalAlignment = VerticalAlignment.Center;
                                ourIconButton2.SnapsToDevicePixels = false;
                                ourIconButton2.UseLayoutRounding = false;
                                Grid.SetColumnSpan(ourIconButton, 10);
                                Grid.SetRowSpan(ourIconButton, 10);
                                base.Children.Add(ourIconButton);
                                this.ButtonStack = ourIconButton;
                            }
                            return;
                        }
                        finally
                        {
                            IEnumerator<OurIconButton> enumerator = default;
                            if (enumerator != null)
                            {
                                enumerator.Dispose();
                            }
                        }
                    }
                    this.ButtonStack = new StackPanel
                    {
                        Opacity = 0.0,
                        Margin = new Thickness(0.0, 0.0, 5.0, 0.0),
                        SnapsToDevicePixels = false,
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        UseLayoutRounding = false
                    };
                    Grid.SetColumnSpan(this.ButtonStack, 10);
                    Grid.SetRowSpan(this.ButtonStack, 10);
                    try
                    {
                        foreach (OurIconButton ourIconButton3 in value)
                        {
                            if (ourIconButton3.Height.Equals(double.NaN))
                            {
                                ourIconButton3.Height = 25.0;
                            }
                            if (ourIconButton3.Width.Equals(double.NaN))
                            {
                                ourIconButton3.Width = 25.0;
                            }
                            ((StackPanel)this.ButtonStack).Children.Add(ourIconButton3);
                        }
                    }
                    finally
                    {
                        IEnumerator<OurIconButton> enumerator2 = default;
                        if (enumerator2 != null)
                        {
                            enumerator2.Dispose();
                        }
                    }
                    base.Children.Add(this.ButtonStack);
                }
            }
        }

        public InlineCollection Inlines
        {
            get
            {
                return this.LabTitle.Inlines;
            }
        }

        public string Title
        {
            get
            {
                return Conversions.ToString(base.GetValue(OurListItem.TitleProperty));
            }
            set
            {
                base.SetValue(OurListItem.TitleProperty, value.ReplaceLineEndings("", false));
            }
        }

        public double FontSize
        {
            get
            {
                return Conversions.ToDouble(base.GetValue(OurListItem.FontSizeProperty));
            }
            set
            {
                base.SetValue(OurListItem.FontSizeProperty, value);
            }
        }

        public string Info
        {
            get
            {
                return this._Info;
            }
            set
            {
                if (Operators.CompareString(this._Info, value, false) != 0)
                {
                    value = value.ReplaceLineEndings("", false);
                    this._Info = value;
                    this.LabInfo.Text = value;
                    this.LabInfo.Visibility = ((Operators.CompareString(value, "", false) == 0) ? Visibility.Collapsed : Visibility.Visible);
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
                if (Operators.CompareString(this._Logo, value, false) != 0)
                {
                    this._Logo = value;
                    if (!Information.IsNothing(this.PathLogo))
                    {
                        base.Children.Remove(this.PathLogo);
                    }
                    if (Operators.CompareString(this._Logo, "", false) != 0)
                    {
                        if (this._Logo.StartsWithF("http", true))
                        {
                            this.PathLogo = new OurImage
                            {
                                Tag = this,
                                IsHitTestVisible = this.LogoClickable,
                                Source = this._Logo,
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                RenderTransform = new ScaleTransform
                                {
                                    ScaleX = this.LogoScale,
                                    ScaleY = this.LogoScale
                                },
                                SnapsToDevicePixels = true,
                                UseLayoutRounding = false
                            };
                            RenderOptions.SetBitmapScalingMode(this.PathLogo, BitmapScalingMode.LowQuality);
                        }
                        else if (this._Logo.EndsWithF(".png", true) || this._Logo.EndsWithF(".jpg", true) || this._Logo.EndsWithF(".webp", true))
                        {
                            this.PathLogo = new Canvas
                            {
                                Tag = this,
                                IsHitTestVisible = this.LogoClickable,
                                Background = new OurBitmap(this._Logo),
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                RenderTransform = new ScaleTransform
                                {
                                    ScaleX = this.LogoScale,
                                    ScaleY = this.LogoScale
                                },
                                SnapsToDevicePixels = true,
                                UseLayoutRounding = false,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch
                            };
                            RenderOptions.SetBitmapScalingMode(this.PathLogo, BitmapScalingMode.LowQuality);
                        }
                        else
                        {
                            this.PathLogo = new Path
                            {
                                Tag = this,
                                IsHitTestVisible = this.LogoClickable,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Stretch = Stretch.Uniform,
                                Data = (Geometry)new GeometryConverter().ConvertFromString(this._Logo),
                                RenderTransformOrigin = new Point(0.5, 0.5),
                                RenderTransform = new ScaleTransform
                                {
                                    ScaleX = this.LogoScale,
                                    ScaleY = this.LogoScale
                                },
                                SnapsToDevicePixels = false,
                                UseLayoutRounding = false
                            };
                            this.PathLogo.SetBinding(Shape.FillProperty, new Binding("Foreground")
                            {
                                Source = this
                            });
                        }
                        Grid.SetColumn(this.PathLogo, 2);
                        Grid.SetRowSpan(this.PathLogo, 4);
                        this.OnSizeChanged();
                        base.Children.Add(this.PathLogo);
                        if (this.LogoClickable)
                        {
                            this.PathLogo.MouseLeave += this._Lambda___62_0;
                            this.PathLogo.MouseLeftButtonDown += this._Lambda___62_1;
                            this.PathLogo.MouseLeftButtonUp += this._Lambda___62_2;
                        }
                    }
                    this.ColumnLogo.Width = new GridLength((double)(checked(((Operators.CompareString(this._Logo, "", false) == 0) ? 0 : 34) + ((base.Height < 40.0) ? 0 : 4))));
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
                if (!Information.IsNothing(this.PathLogo))
                {
                    this.PathLogo.RenderTransform = new ScaleTransform
                    {
                        ScaleX = this.LogoScale,
                        ScaleY = this.LogoScale
                    };
                }
            }
        }

        public bool LogoClickable
        {
            [CompilerGenerated]
            get
            {
                return this._LogoClickable;
            }
            [CompilerGenerated]
            set
            {
                this._LogoClickable = value;
            }
        }

        public OurListItem.CheckType Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                if (this._Type != value)
                {
                    this._Type = value;
                    this.ColumnCheck.Width = new GridLength((double)((this._Type == OurListItem.CheckType.None || this._Type == OurListItem.CheckType.Clickable) ? ((base.Height < 40.0) ? 4 : 2) : 6));
                    if (this._Type == OurListItem.CheckType.None || this._Type == OurListItem.CheckType.Clickable)
                    {
                        if (!Information.IsNothing(this.RectCheck))
                        {
                            base.Children.Remove(this.RectCheck);
                            this.RectCheck = null;
                        }
                        this.SetChecked(false, false, false);
                        return;
                    }
                    if (Information.IsNothing(this.RectCheck))
                    {
                        this.RectCheck = new Border
                        {
                            Width = 5.0,
                            Height = (this.Checked ? double.NaN : 0.0),
                            CornerRadius = new CornerRadius(2.0, 2.0, 2.0, 2.0),
                            VerticalAlignment = (this.Checked ? VerticalAlignment.Stretch : VerticalAlignment.Center),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            UseLayoutRounding = false,
                            SnapsToDevicePixels = false,
                            Margin = (this.Checked ? new Thickness(-1.0, 6.0, 0.0, 6.0) : new Thickness(-1.0, 0.0, 0.0, 0.0))
                        };
                        this.RectCheck.SetResourceReference(Border.BackgroundProperty, "ColorBrush3");
                        Grid.SetRowSpan(this.RectCheck, 4);
                        base.Children.Add(this.RectCheck);
                    }
                }
            }
        }

        private void OnSizeChanged()
        {
            this.ColumnCheck.Width = new GridLength((double)((this._Type == OurListItem.CheckType.None || this._Type == OurListItem.CheckType.Clickable) ? ((base.Height < 40.0) ? 4 : 2) : 6));
            this.ColumnLogo.Width = new GridLength((double)(checked(((Operators.CompareString(this._Logo, "", false) == 0) ? 0 : 34) + ((base.Height < 40.0) ? 0 : 4))));
            if (this.PathLogo != null)
            {
                if (this._Logo.EndsWithF(".png", true) || this._Logo.EndsWithF(".jpg", true) || this._Logo.EndsWithF(".webp", true))
                {
                    this.PathLogo.Margin = new Thickness(4.0, 5.0, 3.0, 5.0);
                }
                else
                {
                    this.PathLogo.Margin = new Thickness((double)((base.Height < 40.0) ? 6 : 8), 8.0, (double)((base.Height < 40.0) ? 4 : 6), 8.0);
                }
            }
            this.LabTitle.Margin = new Thickness(4.0, 0.0, 0.0, (double)((base.Height < 40.0) ? 0 : 2));
        }

        public bool Checked
        {
            get
            {
                return this._Checked;
            }
            set
            {
                this.SetChecked(value, false, value != this._Checked);
            }
        }

        public void SetChecked(bool value, bool user, bool anime)
        {
            try
            {
                ModBase.RouteEventArgs e = new ModBase.RouteEventArgs(user);
                bool @checked = this._Checked;
                if (this.Type == OurListItem.CheckType.RadioBox)
                {
                    if (base.IsInitialized && value != this._Checked)
                    {
                        this._Checked = value;
                        IOurRadio.ChangedEventHandler changedEvent = this.ChangedEvent;
                        if (changedEvent != null)
                        {
                            changedEvent(this, e);
                        }
                        if (e.Handled)
                        {
                            this._Checked = @checked;
                            return;
                        }
                    }
                    this._Checked = value;
                }
                else
                {
                    if (value == this._Checked)
                    {
                        return;
                    }
                    this._Checked = value;
                    if (base.IsInitialized)
                    {
                        IOurRadio.ChangedEventHandler changedEvent2 = this.ChangedEvent;
                        if (changedEvent2 != null)
                        {
                            changedEvent2(this, e);
                        }
                        if (e.Handled)
                        {
                            this._Checked = @checked;
                            return;
                        }
                    }
                }
                if (value)
                {
                    ModBase.RouteEventArgs e2 = new ModBase.RouteEventArgs(user);
                    IOurRadio.CheckEventHandler checkEvent = this.CheckEvent;
                    if (checkEvent != null)
                    {
                        checkEvent(this, e2);
                    }
                    if (e2.Handled)
                    {
                        return;
                    }
                }
                checked
                {
                    if (this.Type == OurListItem.CheckType.RadioBox)
                    {
                        if (Information.IsNothing(base.Parent))
                        {
                            return;
                        }
                        List<OurListItem> list = new List<OurListItem>();
                        int num = 0;
                        try
                        {
                            foreach (object obj in ((IEnumerable)NewLateBinding.LateGet(base.Parent, null, "Children", new object[0], null, null, null)))
                            {
                                object obj2 = RuntimeHelpers.GetObjectValue(obj);
                                obj2 = OurVirtualizingElement.TryInit((FrameworkElement)obj2);
                                if (obj2 is OurListItem && ((OurListItem)obj2).Type == OurListItem.CheckType.RadioBox)
                                {
                                    list.Add((OurListItem)obj2);
                                    if (Conversions.ToBoolean(NewLateBinding.LateGet(obj2, null, "Checked", new object[0], null, null, null)))
                                    {
                                        num++;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            IEnumerator enumerator = default;
                            if (enumerator is IDisposable)
                            {
                                (enumerator as IDisposable).Dispose();
                            }
                        }
                        int num2 = num;
                        if (num2 == 0)
                        {
                            list[0].Checked = true;
                        }
                        else if (num2 > 1)
                        {
                            if (this.Checked)
                            {
                                try
                                {
                                    foreach (OurListItem ourListItem in list)
                                    {
                                        if (ourListItem.Checked && !ourListItem.Equals(this))
                                        {
                                            ourListItem.Checked = false;
                                        }
                                    }
                                    goto IL_0263;
                                }
                                finally
                                {
                                    List<OurListItem>.Enumerator enumerator2 = default;
                                    ((IDisposable)enumerator2).Dispose();
                                }
                            }
                            bool flag = false;
                            try
                            {
                                foreach (OurListItem ourListItem2 in list)
                                {
                                    if (ourListItem2.Checked)
                                    {
                                        if (flag)
                                        {
                                            ourListItem2.Checked = false;
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                List<OurListItem>.Enumerator enumerator3 = default;
                                ((IDisposable)enumerator3).Dispose();
                            }
                        }
                    }
                }
                IL_0263:
                if (base.IsLoaded && ModAnimation.AniControlEnabled == 0 && anime)
                {
                    List<ModAnimation.AniData> list2 = new List<ModAnimation.AniData>();
                    if (this.Checked)
                    {
                        if (!Information.IsNothing(this.RectCheck))
                        {
                            double num3 = base.ActualHeight - this.RectCheck.ActualHeight - 12.0;
                            list2.Add(ModAnimation.AaHeight(this.RectCheck, num3 * 0.4, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false));
                            list2.Add(ModAnimation.AaHeight(this.RectCheck, num3 * 0.6, 300, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false));
                            list2.Add(ModAnimation.AaOpacity(this.RectCheck, 1.0 - this.RectCheck.Opacity, 30, 0, null, false));
                            this.RectCheck.VerticalAlignment = VerticalAlignment.Center;
                            this.RectCheck.Margin = new Thickness(-1.0, 0.0, 0.0, 0.0);
                        }
                        list2.Add(ModAnimation.AaColor(this, OurListItem.ForegroundProperty, (base.Height < 40.0) ? "ColorBrush3" : "ColorBrush2", 200, 0, null, false));
                    }
                    else
                    {
                        if (!Information.IsNothing(this.RectCheck))
                        {
                            list2.Add(ModAnimation.AaHeight(this.RectCheck, -this.RectCheck.ActualHeight, 120, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false));
                            list2.Add(ModAnimation.AaOpacity(this.RectCheck, -this.RectCheck.Opacity, 70, 40, null, false));
                            this.RectCheck.VerticalAlignment = VerticalAlignment.Center;
                        }
                        list2.Add(ModAnimation.AaColor(this, OurListItem.ForegroundProperty, "ColorBrush1", 120, 0, null, false));
                    }
                    ModAnimation.AniStart(list2, "OurListItem Checked " + Conversions.ToString(this.Uuid), false);
                }
                else
                {
                    ModAnimation.AniStop("OurListItem Checked " + Conversions.ToString(this.Uuid));
                    if (this.Checked)
                    {
                        if (!Information.IsNothing(this.RectCheck))
                        {
                            this.RectCheck.Height = double.NaN;
                            this.RectCheck.Margin = new Thickness(-1.0, 6.0, 0.0, 6.0);
                            this.RectCheck.Opacity = 1.0;
                            this.RectCheck.VerticalAlignment = VerticalAlignment.Stretch;
                        }
                        base.SetResourceReference(OurListItem.ForegroundProperty, (base.Height < 40.0) ? "ColorBrush3" : "ColorBrush2");
                    }
                    else
                    {
                        if (!Information.IsNothing(this.RectCheck))
                        {
                            this.RectCheck.Height = 0.0;
                            this.RectCheck.Margin = new Thickness(-1.0, 0.0, 0.0, 0.0);
                            this.RectCheck.Opacity = 0.0;
                            this.RectCheck.VerticalAlignment = VerticalAlignment.Center;
                        }
                        base.SetResourceReference(OurListItem.ForegroundProperty, "ColorBrush1");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "设置 Checked 失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurListItem.xaml.vb");
            }
        }

        public Brush Foreground
        {
            get
            {
                return (Brush)base.GetValue(OurListItem.ForegroundProperty);
            }
            set
            {
                base.SetValue(OurListItem.ForegroundProperty, value);
            }
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseDown)
            {
                OurListItem.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                if (!e.Handled)
                {
                    if (CustomEventService.GetEventType((DependencyObject)sender) != CustomEvent.EventType.None || CustomEventService.GetEvents((DependencyObject)sender).Events.Count > 0)
                    {
                        this.RaiseCustomEvent();
                        e.Handled = true;
                    }
                    if (!e.Handled)
                    {
                        switch (this.Type)
                        {
                        case OurListItem.CheckType.Clickable:
                            Logger.Info(string.Format("按下单击列表项：{0}", this.Title), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurListItem.xaml.vb");
                            return;
                        case OurListItem.CheckType.RadioBox:
                            Logger.Info(string.Format("按下单选列表项：{0}", this.Title), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurListItem.xaml.vb");
                            if (!this.Checked)
                            {
                                this.SetChecked(true, true, true);
                                return;
                            }
                            break;
                        case OurListItem.CheckType.CheckBox:
                            Logger.Info(string.Format("按下复选列表项（{0}）：{1}", !this.Checked, this.Title), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurListItem.xaml.vb");
                            this.SetChecked(!this.Checked, true, true);
                            break;
                        default:
                            return;
                        }
                    }
                }
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (base.IsMouseDirectlyOver && this.Type != OurListItem.CheckType.None)
            {
                this.IsMouseDown = true;
                if (this.ButtonStack != null)
                {
                    this.ButtonStack.IsHitTestVisible = false;
                }
            }
        }

        private void Button_MouseLeave(object sender, object e)
        {
            this.IsMouseDown = false;
            if (this.ButtonStack != null)
            {
                this.ButtonStack.IsHitTestVisible = true;
            }
        }

        public void RefreshColor(object sender, EventArgs e)
        {
            if (this.ContentHandler != null)
            {
                this.ContentHandler(this, e);
                this.ContentHandler = null;
            }
            string text;
            int num;
            if (this.IsMouseDown && (this.Type != OurListItem.CheckType.RadioBox || !this.Checked))
            {
                text = "MouseDown";
                num = 120;
            }
            else if (base.IsMouseOver && this.IsMouseOverAnimationEnabled)
            {
                text = "MouseOver";
                num = 120;
            }
            else
            {
                text = "Idle";
                num = 180;
            }
            checked
            {
                if (Operators.CompareString(this.StateLast, text, false) != 0)
                {
                    this.StateLast = text;
                    if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                    {
                        List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
                        if (base.IsMouseOver && this.IsMouseOverAnimationEnabled)
                        {
                            if (this.ButtonStack != null)
                            {
                                list.Add(ModAnimation.AaOpacity(this.ButtonStack, unchecked(1.0 - this.ButtonStack.Opacity), (int)Math.Round(unchecked((double)num * 0.7)), (int)Math.Round(unchecked((double)num * 0.3)), null, false));
                                list.Add(ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___94_0), unchecked((double)Math.Max(this.MinPaddingRight, checked(5 + this.Buttons.Count<OurIconButton>() * 25)) - this.ColumnPaddingRight.Width.Value), (int)Math.Round(unchecked((double)num * 0.3)), (int)Math.Round(unchecked((double)num * 0.7)), null, false));
                            }
                            list.AddRange(new ModAnimation.AniData[]
                            {
                                ModAnimation.AaColor(this.RectBack, Border.BackgroundProperty, this.IsMouseDown ? "ColorBrush6" : "ColorBrushBg1", num, 0, null, false),
                                ModAnimation.AaOpacity(this.RectBack, unchecked(1.0 - this.RectBack.Opacity), num, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                            });
                            if (this.IsScaleAnimationEnabled)
                            {
                                list.Add(ModAnimation.AaScaleTransform(this.RectBack, unchecked(1.0 - ((ScaleTransform)this.RectBack.RenderTransform).ScaleX), (int)Math.Round(unchecked((double)num * 1.6)), 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false));
                                if (this.IsMouseDown)
                                {
                                    list.Add(ModAnimation.AaScaleTransform(this, unchecked(0.98 - ((ScaleTransform)base.RenderTransform).ScaleX), (int)Math.Round(unchecked((double)num * 0.9)), 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false));
                                }
                                else
                                {
                                    list.Add(ModAnimation.AaScaleTransform(this, unchecked(1.0 - ((ScaleTransform)base.RenderTransform).ScaleX), (int)Math.Round(unchecked((double)num * 1.2)), 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false));
                                }
                            }
                        }
                        else
                        {
                            if (this.ButtonStack != null)
                            {
                                list.Add(ModAnimation.AaOpacity(this.ButtonStack, unchecked(-this.ButtonStack.Opacity), (int)Math.Round(unchecked((double)num * 0.4)), 0, null, false));
                                list.Add(ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___94_1), unchecked((double)this.MinPaddingRight - this.ColumnPaddingRight.Width.Value), (int)Math.Round(unchecked((double)num * 0.4)), 0, null, false));
                            }
                            unchecked
                            {
                                list.Add(ModAnimation.AaOpacity(this.RectBack, -this.RectBack.Opacity, num, 0, null, false));
                                if (this.IsScaleAnimationEnabled)
                                {
                                    list.AddRange(new ModAnimation.AniData[]
                                    {
                                        ModAnimation.AaColor(this.RectBack, Border.BackgroundProperty, this.IsMouseDown ? "ColorBrush6" : "ColorBrush7", num, 0, null, false),
                                        ModAnimation.AaScaleTransform(this, 1.0 - ((ScaleTransform)base.RenderTransform).ScaleX, checked(num * 3), 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                                        ModAnimation.AaScaleTransform(this.RectBack, 0.996 - ((ScaleTransform)this.RectBack.RenderTransform).ScaleX, num, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                                        ModAnimation.AaScaleTransform(this.RectBack, -0.246, 1, 0, null, true)
                                    });
                                }
                            }
                        }
                        ModAnimation.AniStart(list, "ListItem Color " + Conversions.ToString(this.Uuid), false);
                        return;
                    }
                    if (base.IsMouseOver && this.IsMouseOverAnimationEnabled)
                    {
                        if (this.ButtonStack != null)
                        {
                            this.ButtonStack.Opacity = 1.0;
                            this.ColumnPaddingRight.Width = new GridLength((double)Math.Max(this.MinPaddingRight, 5 + this.Buttons.Count<OurIconButton>() * 25));
                        }
                        this.RectBack.Background = ModSecret.ColorBg1;
                        this.RectBack.Opacity = 1.0;
                        this.RectBack.RenderTransform = new ScaleTransform(1.0, 1.0);
                        base.RenderTransform = new ScaleTransform(1.0, 1.0);
                    }
                    else
                    {
                        if (this.ButtonStack != null)
                        {
                            this.ButtonStack.Opacity = 0.0;
                            this.ColumnPaddingRight.Width = new GridLength((double)this.MinPaddingRight);
                        }
                        base.RenderTransform = new ScaleTransform(1.0, 1.0);
                        if (this._RectBack != null)
                        {
                            if (this.IsScaleAnimationEnabled)
                            {
                                this.RectBack.RenderTransform = new ScaleTransform(0.75, 0.75);
                            }
                            this.RectBack.Background = ModSecret.Color7;
                            this.RectBack.Opacity = 0.0;
                        }
                    }
                    ModAnimation.AniStop("ListItem Color " + Conversions.ToString(this.Uuid));
                }
            }
        }

        private void OurListItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Checked)
            {
                base.SetResourceReference(OurListItem.ForegroundProperty, (base.Height < 40.0) ? "ColorBrush3" : "ColorBrush2");
            }
            else
            {
                base.SetResourceReference(OurListItem.ForegroundProperty, "ColorBrush1");
            }
            this.ColumnPaddingRight.Width = new GridLength((double)this.MinPaddingRight);
            if (CustomEventService.GetEventType(this) == CustomEvent.EventType.打开帮助 && (Operators.CompareString(this.Title, "", false) == 0 || Operators.CompareString(this.Info, "", false) == 0))
            {
                try
                {
                    new ModMain.HelpEntry(CustomEvent.GetAbsoluteUrls(CustomEventService.GetEventData(this), CustomEventService.GetEventType(this))[0]).SetToListItem(this);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "设置帮助 OurListItem 失败", LogBehavior.Alert, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Controls\\OurListItem.xaml.vb");
                    CustomEventService.SetEventType(this, CustomEvent.EventType.None);
                    CustomEventService.SetEventData(this, "");
                }
            }
        }

        public override string ToString()
        {
            return this.Title;
        }







        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, SizeChangedEventArgs a1)
        {
            this.OnSizeChanged();
        }

        [CompilerGenerated]
        private void _Lambda___62_0(object sender, MouseEventArgs e)
        {
            this.IsLogoDown = false;
        }

        [CompilerGenerated]
        private void _Lambda___62_1(object sender, MouseButtonEventArgs e)
        {
            this.IsLogoDown = true;
        }

        [CompilerGenerated]
        private void _Lambda___62_2(object sender, MouseButtonEventArgs e)
        {
            if (this.IsLogoDown)
            {
                this.IsLogoDown = false;
                OurListItem.LogoClickEventHandler logoClickEvent = this.LogoClickEvent;
                if (logoClickEvent != null)
                {
                    logoClickEvent(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null)), e);
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___94_0(object i)
        {
            this.ColumnPaddingRight.Width = new GridLength(Conversions.ToDouble(NewLateBinding.LateGet(null, typeof(Math), "Max", new object[]
            {
                0,
                Operators.AddObject(this.ColumnPaddingRight.Width.Value, i)
            }, null, null, null)));
        }

        [CompilerGenerated]
        private void _Lambda___94_1(object i)
        {
            this.ColumnPaddingRight.Width = new GridLength(Conversions.ToDouble(NewLateBinding.LateGet(null, typeof(Math), "Max", new object[]
            {
                0,
                Operators.AddObject(this.ColumnPaddingRight.Width.Value, i)
            }, null, null, null)));
        }

        [CompilerGenerated]
        private OurListItem.ClickEventHandler ClickEvent;

        [CompilerGenerated]
        private OurListItem.LogoClickEventHandler LogoClickEvent;

        [CompilerGenerated]
        private IOurRadio.CheckEventHandler CheckEvent;

        [CompilerGenerated]
        private IOurRadio.ChangedEventHandler ChangedEvent;

        private Border _RectBack;

        public FrameworkElement ButtonStack;

        public FrameworkElement PathLogo;

        public Border RectCheck;

        private TextBlock _LabInfo;

        public int Uuid;

        private bool _IsScaleAnimationEnabled;

        [CompilerGenerated]
        private int _MinPaddingRight;

        private IEnumerable<OurIconButton> _Buttons;

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(OurListItem));

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(OurListItem), new PropertyMetadata(14.0));

        private string _Info;

        private string _Logo;

        private double _LogoScale;

        [CompilerGenerated]
        private bool _LogoClickable;

        private bool IsLogoDown;

        private OurListItem.CheckType _Type;

        private bool _Checked;

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(OurListItem), new PropertyMetadata((global::System.Windows.Media.SolidColorBrush)ModSecret.Color1));

        public Action<OurListItem, EventArgs> ContentHandler;

        private bool IsMouseDown;

        private string StateLast;

        public bool IsMouseOverAnimationEnabled;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurListItem _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnCheck")]
        private ColumnDefinition _ColumnCheck;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnPaddingLeft")]
        private ColumnDefinition _ColumnPaddingLeft;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnLogo")]
        private ColumnDefinition _ColumnLogo;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnPaddingRight")]
        private ColumnDefinition _ColumnPaddingRight;

        [CompilerGenerated]
        [AccessedThroughProperty("LabTitle")]
        private TextBlock _LabTitle;

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);

        public delegate void LogoClickEventHandler(object sender, MouseButtonEventArgs e);

        public enum CheckType
        {
            None,
            Clickable,
            RadioBox,
            CheckBox
        }
    }
}

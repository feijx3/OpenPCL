using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class OurLocalModItem : Grid
    {
        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this.LabTitle.TextDecorations = (this.Entry.IsEnabled ? null : TextDecorations.Strikethrough);
                if (Operators.CompareString(this.LabTitle.Text, value, false) != 0)
                {
                    this.LabTitle.Text = value;
                    this._Title = value;
                }
            }
        }

        public string SubTitle
        {
            get
            {
                TextBlock labSubtitle = this.LabSubtitle;
                return ((labSubtitle != null) ? labSubtitle.Text : null) ?? "";
            }
            set
            {
                if (Operators.CompareString(this.LabSubtitle.Text, value, false) != 0)
                {
                    this.LabSubtitle.Text = value;
                    this.LabSubtitle.Visibility = ((Operators.CompareString(value, "", false) == 0) ? Visibility.Collapsed : Visibility.Visible);
                }
            }
        }

        public string Description
        {
            get
            {
                return this.LabInfo.Text;
            }
            set
            {
                if (Operators.CompareString(this.LabInfo.Text, value, false) != 0)
                {
                    this.LabInfo.Text = value;
                }
            }
        }

        public List<string> Tags
        {
            set
            {
                this.PanTags.Children.Clear();
                this.PanTags.Visibility = (value.Any<string>() ? Visibility.Visible : Visibility.Collapsed);
                try
                {
                    foreach (string text in value)
                    {
                        Border border = new Border
                        {
                            Background = new ModBase.OurColor("#0A000000"),
                            Padding = new Thickness(3.0, 1.0, 3.0, 1.0),
                            CornerRadius = new CornerRadius(3.0),
                            Margin = new Thickness(0.0, 0.0, 3.0, 0.0),
                            SnapsToDevicePixels = true,
                            UseLayoutRounding = false
                        };
                        TextBlock textBlock = new TextBlock
                        {
                            Text = text,
                            Foreground = new ModBase.OurColor("#B0B0B0"),
                            FontSize = 11.0
                        };
                        border.Child = textBlock;
                        this.PanTags.Children.Add(border);
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator = default;
                    ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public LocalResourceFile Entry
        {
            get
            {
                return (LocalResourceFile)base.Tag;
            }
            set
            {
                base.Tag = value;
            }
        }

        public event OurLocalModItem.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurLocalModItem.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurLocalModItem.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurLocalModItem.ClickEventHandler clickEventHandler3 = (OurLocalModItem.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurLocalModItem.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurLocalModItem.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurLocalModItem.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurLocalModItem.ClickEventHandler clickEventHandler3 = (OurLocalModItem.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurLocalModItem.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseDown)
            {
                OurLocalModItem.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                if (!e.Handled)
                {
                    Logger.Info(string.Format("按下本地 Mod 列表项：{0}", this.LabTitle.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\OurLocalModItem.xaml.vb");
                }
            }
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (base.IsMouseDirectlyOver)
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

        private static bool Swiping
        {
            get
            {
                return OurLocalModItem._Swiping;
            }
            set
            {
                OurLocalModItem._Swiping = value;
                ModMain.FrmInstanceMod.CardSelect.IsHitTestVisible = !value;
            }
        }

        private void Button_MouseSwipeStart(object sender, object e)
        {
            if (base.Parent != null)
            {
                OurLocalModItem.SwipeEnd = (OurLocalModItem.SwipeStart = ((StackPanel)base.Parent).Children.IndexOf(this));
                OurLocalModItem.Swiping = true;
                OurLocalModItem.SwipToState = !this.Checked;
            }
        }

        private void Button_MouseSwipe(object sender, object e)
        {
            checked
            {
                if (base.Parent != null)
                {
                    if (Mouse.LeftButton != MouseButtonState.Pressed || !(Mouse.DirectlyOver is OurLocalModItem))
                    {
                        OurLocalModItem.Swiping = false;
                        return;
                    }
                    UIElementCollection children = ((StackPanel)base.Parent).Children;
                    int num = children.IndexOf(this);
                    OurLocalModItem.SwipeStart = Math.Min(OurLocalModItem.SwipeStart, num).Clamp(0, children.Count - 1);
                    OurLocalModItem.SwipeEnd = Math.Max(OurLocalModItem.SwipeEnd, num).Clamp(0, children.Count - 1);
                    if (OurLocalModItem.SwipeStart != OurLocalModItem.SwipeEnd)
                    {
                        int swipeStart = OurLocalModItem.SwipeStart;
                        int swipeEnd = OurLocalModItem.SwipeEnd;
                        for (int i = swipeStart; i <= swipeEnd; i++)
                        {
                            OurLocalModItem ourLocalModItem = (OurLocalModItem)children[i];
                            ourLocalModItem.InitLate(ourLocalModItem, (EventArgs)e);
                            ourLocalModItem.Checked = OurLocalModItem.SwipToState;
                        }
                    }
                }
            }
        }

        public event OurLocalModItem.CheckEventHandler Check
        {
            [CompilerGenerated]
            add
            {
                OurLocalModItem.CheckEventHandler checkEventHandler = this.CheckEvent;
                OurLocalModItem.CheckEventHandler checkEventHandler2;
                do
                {
                    checkEventHandler2 = checkEventHandler;
                    OurLocalModItem.CheckEventHandler checkEventHandler3 = (OurLocalModItem.CheckEventHandler)Delegate.Combine(checkEventHandler2, value);
                    checkEventHandler = Interlocked.CompareExchange<OurLocalModItem.CheckEventHandler>(ref this.CheckEvent, checkEventHandler3, checkEventHandler2);
                }
                while (checkEventHandler != checkEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurLocalModItem.CheckEventHandler checkEventHandler = this.CheckEvent;
                OurLocalModItem.CheckEventHandler checkEventHandler2;
                do
                {
                    checkEventHandler2 = checkEventHandler;
                    OurLocalModItem.CheckEventHandler checkEventHandler3 = (OurLocalModItem.CheckEventHandler)Delegate.Remove(checkEventHandler2, value);
                    checkEventHandler = Interlocked.CompareExchange<OurLocalModItem.CheckEventHandler>(ref this.CheckEvent, checkEventHandler3, checkEventHandler2);
                }
                while (checkEventHandler != checkEventHandler2);
            }
        }

        public event OurLocalModItem.ChangedEventHandler Changed
        {
            [CompilerGenerated]
            add
            {
                OurLocalModItem.ChangedEventHandler changedEventHandler = this.ChangedEvent;
                OurLocalModItem.ChangedEventHandler changedEventHandler2;
                do
                {
                    changedEventHandler2 = changedEventHandler;
                    OurLocalModItem.ChangedEventHandler changedEventHandler3 = (OurLocalModItem.ChangedEventHandler)Delegate.Combine(changedEventHandler2, value);
                    changedEventHandler = Interlocked.CompareExchange<OurLocalModItem.ChangedEventHandler>(ref this.ChangedEvent, changedEventHandler3, changedEventHandler2);
                }
                while (changedEventHandler != changedEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurLocalModItem.ChangedEventHandler changedEventHandler = this.ChangedEvent;
                OurLocalModItem.ChangedEventHandler changedEventHandler2;
                do
                {
                    changedEventHandler2 = changedEventHandler;
                    OurLocalModItem.ChangedEventHandler changedEventHandler3 = (OurLocalModItem.ChangedEventHandler)Delegate.Remove(changedEventHandler2, value);
                    changedEventHandler = Interlocked.CompareExchange<OurLocalModItem.ChangedEventHandler>(ref this.ChangedEvent, changedEventHandler3, changedEventHandler2);
                }
                while (changedEventHandler != changedEventHandler2);
            }
        }

        public bool Checked
        {
            get
            {
                return this._Checked;
            }
            set
            {
                try
                {
                    bool @checked = this._Checked;
                    if (value != this._Checked)
                    {
                        this._Checked = value;
                        ModBase.RouteEventArgs e = new ModBase.RouteEventArgs(false);
                        if (base.IsInitialized)
                        {
                            OurLocalModItem.ChangedEventHandler changedEvent = this.ChangedEvent;
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
                        if (value)
                        {
                            ModBase.RouteEventArgs e2 = new ModBase.RouteEventArgs(false);
                            OurLocalModItem.CheckEventHandler checkEvent = this.CheckEvent;
                            if (checkEvent != null)
                            {
                                checkEvent(this, e2);
                            }
                            if (e2.Handled)
                            {
                                return;
                            }
                        }
                        if (this.IsVisibleInForm())
                        {
                            List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
                            if (this.Checked)
                            {
                                double num = 32.0 - this.RectCheck.ActualHeight;
                                list.Add(ModAnimation.AaHeight(this.RectCheck, num * 0.4, 200, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Weak), false));
                                list.Add(ModAnimation.AaHeight(this.RectCheck, num * 0.6, 300, 0, new ModAnimation.AniEaseOutBack(ModAnimation.AniEasePower.Weak), false));
                                list.Add(ModAnimation.AaOpacity(this.RectCheck, 1.0 - this.RectCheck.Opacity, 30, 0, null, false));
                                this.RectCheck.VerticalAlignment = VerticalAlignment.Center;
                                this.RectCheck.Margin = new Thickness(-3.0, 0.0, 0.0, 0.0);
                                list.Add(ModAnimation.AaColor(this.LabTitle, TextBlock.ForegroundProperty, this.Entry.IsEnabled ? "ColorBrush2" : "ColorBrush5", 200, 0, null, false));
                            }
                            else
                            {
                                list.Add(ModAnimation.AaHeight(this.RectCheck, -this.RectCheck.ActualHeight, 120, 0, new ModAnimation.AniEaseInFluent(ModAnimation.AniEasePower.Weak), false));
                                list.Add(ModAnimation.AaOpacity(this.RectCheck, -this.RectCheck.Opacity, 70, 40, null, false));
                                this.RectCheck.VerticalAlignment = VerticalAlignment.Center;
                                list.Add(ModAnimation.AaColor(this.LabTitle, TextBlock.ForegroundProperty, (this.LabTitle.TextDecorations == null) ? "ColorBrush1" : "ColorBrushGray4", 120, 0, null, false));
                            }
                            ModAnimation.AniStart(list, "OurLocalModItem Checked " + Conversions.ToString(this.Uuid), false);
                        }
                        else
                        {
                            this.RectCheck.VerticalAlignment = VerticalAlignment.Center;
                            this.RectCheck.Margin = new Thickness(-3.0, 0.0, 0.0, 0.0);
                            if (this.Checked)
                            {
                                this.RectCheck.Height = 32.0;
                                this.RectCheck.Opacity = 1.0;
                                this.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, this.Entry.IsEnabled ? "ColorBrush2" : "ColorBrush5");
                            }
                            else
                            {
                                this.RectCheck.Height = 0.0;
                                this.RectCheck.Opacity = 0.0;
                                this.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, this.Entry.IsEnabled ? "ColorBrush1" : "ColorBrushGray4");
                            }
                            ModAnimation.AniStop("OurLocalModItem Checked " + Conversions.ToString(this.Uuid));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "设置 Checked 失败", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\OurLocalModItem.xaml.vb");
                }
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
                        CornerRadius = new CornerRadius(3.0),
                        RenderTransform = new ScaleTransform(0.8, 0.8),
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
                if (value.Any<OurIconButton>())
                {
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
                            ((StackPanel)this.ButtonStack).Children.Add(ourIconButton);
                        }
                    }
                    finally
                    {
                        IEnumerator<OurIconButton> enumerator = default;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                    base.Children.Add(this.ButtonStack);
                }
            }
        }

        public Border RectCheck
        {
            get
            {
                if (this._RectCheck == null)
                {
                    this._RectCheck = new Border
                    {
                        Width = 5.0,
                        Height = (this.Checked ? double.NaN : 0.0),
                        CornerRadius = new CornerRadius(2.0, 2.0, 2.0, 2.0),
                        VerticalAlignment = (this.Checked ? VerticalAlignment.Stretch : VerticalAlignment.Center),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        UseLayoutRounding = false,
                        SnapsToDevicePixels = false,
                        Margin = (this.Checked ? new Thickness(-3.0, 6.0, 0.0, 6.0) : new Thickness(-3.0, 0.0, 0.0, 0.0))
                    };
                    this._RectCheck.SetResourceReference(Border.BackgroundProperty, "ColorBrush3");
                    Grid.SetRowSpan(this._RectCheck, 10);
                    base.Children.Add(this._RectCheck);
                }
                return this._RectCheck;
            }
        }

        private string GetUpdateCompareDescription()
        {
            string text = (this.Entry.ProjectVersion.Version ?? this.Entry.ProjectVersion.Display).Replace(".jar", "");
            string text2 = (this.Entry.UpdateFile.Version ?? this.Entry.UpdateFile.Display).Replace(".jar", "");
            List<string> list = text.Split("-_+ ".ToCharArray()).ToList<string>();
            List<string> list2 = text2.Split("-_+ ".ToCharArray()).ToList<string>();
            bool flag = false;
            try
            {
                foreach (string text3 in list.ToList<string>())
                {
                    if (list2.Contains(text3))
                    {
                        list.Remove(text3);
                        list2.Remove(text3);
                        flag = true;
                    }
                }
            }
            finally
            {
                List<string>.Enumerator enumerator = default;
                ((IDisposable)enumerator).Dispose();
            }
            if (flag && list.Any<string>() && list2.Any<string>())
            {
                text = list.Join("-");
                text2 = list2.Join("-");
                this.Entry._Version = text;
            }
            return string.Format("当前版本：{0}（{1}）{2}最新版本：{3}（{4}）", new object[]
            {
                text,
                StringUtils.FormatTimeSpan(this.Entry.ProjectVersion.ReleaseDate - DateTime.Now, false),
                "\r\n",
                text2,
                StringUtils.FormatTimeSpan(this.Entry.UpdateFile.ReleaseDate - DateTime.Now, false)
            });
        }

        public OurLocalModItem()
        {
            base.PreviewMouseLeftButtonUp += this.Button_MouseUp;
            base.PreviewMouseLeftButtonDown += this.Button_MouseDown;
            base.MouseLeave += new MouseEventHandler(this.Button_MouseLeave);
            base.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Button_MouseLeave);
            base.MouseLeftButtonDown += new MouseButtonEventHandler(this.Button_MouseSwipeStart);
            base.MouseEnter += new MouseEventHandler(this.Button_MouseSwipe);
            base.MouseLeave += new MouseEventHandler(this.Button_MouseSwipe);
            base.MouseLeftButtonUp += new MouseButtonEventHandler(this.Button_MouseSwipe);
            base.MouseEnter += new MouseEventHandler(this.RefreshColor);
            base.MouseLeave += new MouseEventHandler(this.RefreshColor);
            base.MouseLeftButtonDown += new MouseButtonEventHandler(this.RefreshColor);
            base.MouseLeftButtonUp += new MouseButtonEventHandler(this.RefreshColor);
            this.Changed += new OurLocalModItem.ChangedEventHandler(this.RefreshColor);
            this.Uuid = ModBase.GetUuid();
            this.IsMouseDown = false;
            this._Checked = false;
            this._RectBack = null;
            this.HasEnteredViewport = false;
            this.InitializeComponent();
            this.PanTitle_WithEvents = this.PanTitle;
            this.BtnUpdate_WithEvents = this.BtnUpdate;
            LazyLoadBehavior.OnFirstEnterScrollViewerViewport(this, new Action(this.FirstEnterScrollViewerViewport));
        }

        private void FirstEnterScrollViewerViewport()
        {
            this.HasEnteredViewport = true;
            this.Refresh();
        }

        public void Refresh()
        {
            if (this.HasEnteredViewport)
            {
                if (this.Entry.HasUpdate)
                {
                    this.BtnUpdate.Visibility = Visibility.Visible;
                    this.BtnUpdate.ToolTip = string.Format("{0}{1}点击以更新，右键查看更新日志。", this.GetUpdateCompareDescription(), "\r\n");
                }
                else
                {
                    this.BtnUpdate.Visibility = Visibility.Collapsed;
                }
                string fileNameWithoutExtension = PathUtils.GetFileNameWithoutExtension(this.Entry.EnabledName);
                string text;
                if (Settings.Get<int>("ToolModLocalNameStyle", null) == 1)
                {
                    this.Title = fileNameWithoutExtension;
                    this.SubTitle = "";
                    if (((object)(this.Entry.Project) == (object)null))
                    {
                        text = this.Entry.Display;
                    }
                    else
                    {
                        ValueTuple<string, string> controlTitle = this.Entry.Project.GetControlTitle(false);
                        text = controlTitle.Item1 + controlTitle.Item2;
                    }
                    text = text.Replace("  |  ", " / ");
                    if (this.Entry.Version != null)
                    {
                        text += string.Format(" ({0})", this.Entry.Version);
                    }
                }
                else
                {
                    if (((object)(this.Entry.Project) == (object)null))
                    {
                        this.Title = this.Entry.Display;
                        this.SubTitle = ((this.Entry.Version == null) ? "" : ("  |  " + this.Entry.Version));
                    }
                    else
                    {
                        ValueTuple<string, string> controlTitle2 = this.Entry.Project.GetControlTitle(false);
                        this.Title = controlTitle2.Item1;
                        this.SubTitle = controlTitle2.Item2 + ((this.Entry.Version == null) ? "" : ("  |  " + this.Entry.Version));
                    }
                    text = fileNameWithoutExtension;
                }
                if (((object)(this.Entry.Project) != (object)null))
                {
                    text = text + ": " + this.Entry.Project.Description.ReplaceLineEndings("", false);
                }
                else if (this.Entry.Description != null)
                {
                    text = text + ": " + this.Entry.Description.ReplaceLineEndings("", false);
                }
                this.Description = text;
                if (this.Checked)
                {
                    this.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, this.Entry.IsEnabled ? "ColorBrush2" : "ColorBrush5");
                }
                else
                {
                    this.LabTitle.SetResourceReference(TextBlock.ForegroundProperty, this.Entry.IsEnabled ? "ColorBrush1" : "ColorBrushGray4");
                }
                if (((object)(this.Entry.Project) != (object)null))
                {
                    this.Entry.Project.ApplyLogoToOurImage(this.PathLogo);
                }
                else
                {
                    this.PathLogo.Source = ModBase.PathImage + "Icons/NoIcon.png";
                }
                if (this.Entry.IsEnabled)
                {
                    if (this.ImgState != null)
                    {
                        base.Children.Remove(this.ImgState);
                        this.ImgState = null;
                    }
                }
                else
                {
                    if (this.ImgState == null)
                    {
                        this.ImgState = new Image
                        {
                            Width = 20.0,
                            Height = 20.0,
                            Margin = new Thickness(0.0, 0.0, -5.0, -3.0),
                            IsHitTestVisible = false,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Bottom
                        };
                        RenderOptions.SetBitmapScalingMode(this.ImgState, BitmapScalingMode.HighQuality);
                        Grid.SetColumn(this.ImgState, 1);
                        Grid.SetRow(this.ImgState, 1);
                        Grid.SetRowSpan(this.ImgState, 2);
                        base.Children.Add(this.ImgState);
                    }
                    this.ImgState.Source = new OurBitmap(ModBase.PathImage + "Icons/Disabled.png");
                }
                if (((object)(this.Entry.Project) != (object)null))
                {
                    this.Tags = this.Entry.Project.Tags;
                }
            }
        }

        public void RefreshColor(object sender, EventArgs e)
        {
            this.InitLate(RuntimeHelpers.GetObjectValue(sender), e);
            int num = (base.IsMouseOver ? 120 : 180);
            List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
            checked
            {
                if (this.ButtonStack != null)
                {
                    if (base.IsMouseOver)
                    {
                        list.Add(ModAnimation.AaOpacity(this.ButtonStack, unchecked(1.0 - this.ButtonStack.Opacity), (int)Math.Round(unchecked((double)num * 0.7)), (int)Math.Round(unchecked((double)num * 0.3)), null, false));
                        list.Add(ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___67_0), unchecked((double)(checked(5 + this.Buttons.Count<OurIconButton>() * 25)) - this.ColumnPaddingRight.Width.Value), (int)Math.Round(unchecked((double)num * 0.3)), (int)Math.Round(unchecked((double)num * 0.7)), null, false));
                    }
                    else
                    {
                        list.Add(ModAnimation.AaOpacity(this.ButtonStack, unchecked(-this.ButtonStack.Opacity), (int)Math.Round(unchecked((double)num * 0.4)), 0, null, false));
                        list.Add(ModAnimation.AaDouble(new ParameterizedThreadStart(this._Lambda___67_1), unchecked(4.0 - this.ColumnPaddingRight.Width.Value), (int)Math.Round(unchecked((double)num * 0.4)), 0, null, false));
                    }
                }
            }
            if (base.IsMouseOver || this.Checked)
            {
                list.AddRange(new ModAnimation.AniData[]
                {
                    ModAnimation.AaColor(this.RectBack, Border.BackgroundProperty, this.IsMouseDown ? "ColorBrush6" : "ColorBrushBg1", num, 0, null, false),
                    ModAnimation.AaOpacity(this.RectBack, 1.0 - this.RectBack.Opacity, num, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false)
                });
                checked
                {
                    if (this.IsMouseDown)
                    {
                        list.Add(ModAnimation.AaScaleTransform(this.RectBack, unchecked(0.996 - ((ScaleTransform)this.RectBack.RenderTransform).ScaleX), (int)Math.Round(unchecked((double)num * 1.2)), 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false));
                    }
                    else
                    {
                        list.Add(ModAnimation.AaScaleTransform(this.RectBack, unchecked(1.0 - ((ScaleTransform)this.RectBack.RenderTransform).ScaleX), (int)Math.Round(unchecked((double)num * 1.2)), 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false));
                    }
                }
            }
            else
            {
                list.AddRange(new ModAnimation.AniData[]
                {
                    ModAnimation.AaOpacity(this.RectBack, -this.RectBack.Opacity, num, 0, null, false),
                    ModAnimation.AaScaleTransform(this.RectBack, 0.996 - ((ScaleTransform)this.RectBack.RenderTransform).ScaleX, num, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                    ModAnimation.AaScaleTransform(this.RectBack, -0.196, 1, 0, null, true)
                });
            }
            ModAnimation.AniStart(list, "LocalModItem Color " + Conversions.ToString(this.Uuid), false);
        }

        private void InitLate(object sender, EventArgs e)
        {
            if (this.ButtonHandler != null)
            {
                this.ButtonHandler((OurLocalModItem)sender, e);
                this.ButtonHandler = null;
            }
        }

        private void BtnUpdate_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            this.ShowUpdateLog();
        }

        private void ShowUpdateLog()
        {
            string text = this.Entry.ChangelogUrls.FirstOrDefault<string>((OurLocalModItem._Closure___._I70_0 == null) ? (OurLocalModItem._Closure___._I70_0 = new Func<string, bool>(OurLocalModItem._Closure___._I._Lambda___70_0)) : OurLocalModItem._Closure___._I70_0);
            string text2 = this.Entry.ChangelogUrls.FirstOrDefault<string>((OurLocalModItem._Closure___._I70_1 == null) ? (OurLocalModItem._Closure___._I70_1 = new Func<string, bool>(OurLocalModItem._Closure___._I._Lambda___70_1)) : OurLocalModItem._Closure___._I70_1);
            if (text == null || text2 == null)
            {
                ModBase.OpenWebsite(this.Entry.ChangelogUrls.First<string>());
                return;
            }
            int num = ModMain.OurMsgBox("要在哪个网站上查看更新日志？", "查看更新日志", "Modrinth", "CurseForge", "取消", false, true, false, null, null, null);
            if (num == 1)
            {
                ModBase.OpenWebsite(text2);
                return;
            }
            if (num != 2)
            {
                return;
            }
            ModBase.OpenWebsite(text);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            switch (ModMain.OurMsgBox(string.Format("是否要更新 {0}？{1}{2}{3}", new object[]
            {
                this.Entry.Display,
                "\r\n",
                "\r\n",
                this.GetUpdateCompareDescription()
            }), "Mod 更新确认", "更新", "查看更新日志", "取消", false, true, false, null, null, null))
            {
            case 1:
                ModMain.FrmInstanceMod.UpdateMods(new LocalResourceFile[] { this.Entry });
                return;
            case 2:
                this.ShowUpdateLog();
                break;
            case 3:
                break;
            default:
                return;
            }
        }

        private void PanTitle_SizeChanged()
        {
            int num = default;
            switch (this.ColumnExtend.Width.IsStar ? 0 : (this.ColumnTitle.Width.IsStar ? 2 : 1))
            {
            case 0:
                if (this.ColumnExtend.ActualWidth >= 0.5)
                {
                    return;
                }
                num = ((this.LabSubtitle.Visibility == Visibility.Collapsed) ? 2 : 1);
                break;
            case 1:
                if (this.ColumnSubtitle.ActualWidth < 0.5)
                {
                    num = 2;
                }
                else
                {
                    if (this.LabSubtitle.IsTextTrimmed())
                    {
                        return;
                    }
                    num = 0;
                }
                break;
            case 2:
                if (this.LabTitle.IsTextTrimmed())
                {
                    return;
                }
                num = ((this.LabSubtitle.Visibility != Visibility.Collapsed) ? 1 : 0);
                break;
            }
            switch (num)
            {
            case 0:
                this.ColumnTitle.Width = GridLength.Auto;
                this.ColumnSubtitle.Width = GridLength.Auto;
                this.ColumnExtend.Width = new GridLength(1.0, GridUnitType.Star);
                return;
            case 1:
                this.ColumnTitle.Width = GridLength.Auto;
                this.ColumnSubtitle.Width = new GridLength(1.0, GridUnitType.Star);
                this.ColumnExtend.Width = new GridLength(0.0, GridUnitType.Pixel);
                return;
            case 2:
                this.ColumnTitle.Width = new GridLength(1.0, GridUnitType.Star);
                this.ColumnSubtitle.Width = new GridLength(0.0, GridUnitType.Pixel);
                this.ColumnExtend.Width = new GridLength(0.0, GridUnitType.Pixel);
                break;
            default:
                return;
            }
        }




        internal virtual Grid PanTitle_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._PanTitle;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                SizeChangedEventHandler sizeChangedEventHandler = new SizeChangedEventHandler(this._Lambda___R87_1);
                Grid grid = this._PanTitle;
                if (grid != null)
                {
                    grid.SizeChanged -= sizeChangedEventHandler;
                }
                this._PanTitle = value;
                grid = this._PanTitle;
                if (grid != null)
                {
                    grid.SizeChanged += sizeChangedEventHandler;
                }
            }
        }






        internal virtual OurIconButton BtnUpdate_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._BtnUpdate;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseButtonEventHandler mouseButtonEventHandler = new MouseButtonEventHandler(this.BtnUpdate_PreviewMouseRightButtonUp);
                OurIconButton.ClickEventHandler clickEventHandler = new OurIconButton.ClickEventHandler(this.BtnUpdate_Click);
                OurIconButton ourIconButton = this._BtnUpdate;
                if (ourIconButton != null)
                {
                    ourIconButton.PreviewMouseRightButtonUp -= mouseButtonEventHandler;
                    ourIconButton.Click -= clickEventHandler;
                }
                this._BtnUpdate = value;
                ourIconButton = this._BtnUpdate;
                if (ourIconButton != null)
                {
                    ourIconButton.PreviewMouseRightButtonUp += mouseButtonEventHandler;
                    ourIconButton.Click += clickEventHandler;
                }
            }
        }



        [CompilerGenerated]
        private void _Lambda___67_0(object i)
        {
            this.ColumnPaddingRight.Width = new GridLength(Conversions.ToDouble(NewLateBinding.LateGet(null, typeof(Math), "Max", new object[]
            {
                0,
                Operators.AddObject(this.ColumnPaddingRight.Width.Value, i)
            }, null, null, null)));
        }

        [CompilerGenerated]
        private void _Lambda___67_1(object i)
        {
            this.ColumnPaddingRight.Width = new GridLength(Conversions.ToDouble(NewLateBinding.LateGet(null, typeof(Math), "Max", new object[]
            {
                0,
                Operators.AddObject(this.ColumnPaddingRight.Width.Value, i)
            }, null, null, null)));
        }

        [CompilerGenerated]
        private void _Lambda___R87_1(object a0, SizeChangedEventArgs a1)
        {
            this.PanTitle_SizeChanged();
        }

        public int Uuid;

        private string _Title;

        [CompilerGenerated]
        private OurLocalModItem.ClickEventHandler ClickEvent;

        private bool IsMouseDown;

        private static int SwipeStart;

        private static int SwipeEnd;

        private static bool _Swiping = false;

        private static bool SwipToState;

        [CompilerGenerated]
        private OurLocalModItem.CheckEventHandler CheckEvent;

        [CompilerGenerated]
        private OurLocalModItem.ChangedEventHandler ChangedEvent;

        private bool _Checked;

        private Image ImgState;

        private Border _RectBack;

        public Action<OurLocalModItem, EventArgs> ButtonHandler;

        public FrameworkElement ButtonStack;

        private IEnumerable<OurIconButton> _Buttons;

        private Border _RectCheck;

        private bool HasEnteredViewport;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurLocalModItem _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnPaddingRight")]
        private ColumnDefinition _ColumnPaddingRight;

        [CompilerGenerated]
        [AccessedThroughProperty("PathLogo")]
        private OurImage _PathLogo;

        [CompilerGenerated]
        [AccessedThroughProperty("PanTitle")]
        private Grid _PanTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnTitle")]
        private ColumnDefinition _ColumnTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnSubtitle")]
        private ColumnDefinition _ColumnSubtitle;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnExtend")]
        private ColumnDefinition _ColumnExtend;

        [CompilerGenerated]
        [AccessedThroughProperty("LabTitle")]
        private TextBlock _LabTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("LabSubtitle")]
        private TextBlock _LabSubtitle;

        [CompilerGenerated]
        [AccessedThroughProperty("BtnUpdate")]
        private OurIconButton _BtnUpdate;

        [CompilerGenerated]
        [AccessedThroughProperty("PanTags")]
        private StackPanel _PanTags;

        [CompilerGenerated]
        [AccessedThroughProperty("LabInfo")]
        private TextBlock _LabInfo;

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);

        public delegate void CheckEventHandler(object sender, ModBase.RouteEventArgs e);

        public delegate void ChangedEventHandler(object sender, ModBase.RouteEventArgs e);

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal bool _Lambda___70_0(string x)
            {
                return x.Contains("curseforge.com");
            }

            internal bool _Lambda___70_1(string x)
            {
                return x.Contains("modrinth.com");
            }

            public static readonly OurLocalModItem._Closure___ _I = new OurLocalModItem._Closure___();

            public static Func<string, bool> _I70_0;

            public static Func<string, bool> _I70_1;
        }
    }
}

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
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class OurResourceItem : Grid
    {
        public OurResourceItem()
        {
            base.PreviewMouseLeftButtonUp += this.Button_MouseUp;
            this.Click += this._Lambda___R0_1;
            base.PreviewMouseLeftButtonDown += this.Button_MouseDown;
            base.MouseLeave += new MouseEventHandler(this.Button_MouseLeave);
            base.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Button_MouseLeave);
            base.MouseEnter += new MouseEventHandler(this.RefreshColor);
            base.MouseLeave += new MouseEventHandler(this.RefreshColor);
            base.MouseLeftButtonDown += new MouseButtonEventHandler(this.RefreshColor);
            base.MouseLeftButtonUp += new MouseButtonEventHandler(this.RefreshColor);
            this.Uuid = ModBase.GetUuid();
            this.IsMouseDown = false;
            this._RectBack = null;
            this.CanInteraction = true;
            this.InitializeComponent();
            this.LabInfo_WithEvents = this.LabInfo;
        }

        public string Title
        {
            get
            {
                return this.LabTitle.Text;
            }
            set
            {
                if (Operators.CompareString(this.LabTitle.Text, value, false) != 0)
                {
                    this.LabTitle.Text = value;
                }
            }
        }

        public string SubTitle
        {
            get
            {
                TextBlock labTitleRaw = this.LabTitleRaw;
                return ((labTitleRaw != null) ? labTitleRaw.Text : null) ?? "";
            }
            set
            {
                if (Operators.CompareString(this.LabTitleRaw.Text, value, false) != 0)
                {
                    this.LabTitleRaw.Text = value;
                    this.LabTitleRaw.Visibility = ((Operators.CompareString(value, "", false) == 0) ? Visibility.Collapsed : Visibility.Visible);
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

        private void LabInfo_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ToolTipInfo.Content = this.LabInfo.Text;
            this.LabInfo.ToolTip = (this.LabInfo.IsTextTrimmed() ? this.ToolTipInfo : null);
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
                            Background = new ModBase.OurColor("#11000000"),
                            Padding = new Thickness(3.0, 1.0, 3.0, 1.0),
                            CornerRadius = new CornerRadius(3.0),
                            Margin = new Thickness(0.0, 0.0, 3.0, 0.0),
                            SnapsToDevicePixels = true,
                            UseLayoutRounding = false
                        };
                        TextBlock textBlock = new TextBlock
                        {
                            Text = text,
                            Foreground = new ModBase.OurColor("#868686"),
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

        public event OurResourceItem.ClickEventHandler Click
        {
            [CompilerGenerated]
            add
            {
                OurResourceItem.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurResourceItem.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurResourceItem.ClickEventHandler clickEventHandler3 = (OurResourceItem.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurResourceItem.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
            [CompilerGenerated]
            remove
            {
                OurResourceItem.ClickEventHandler clickEventHandler = this.ClickEvent;
                OurResourceItem.ClickEventHandler clickEventHandler2;
                do
                {
                    clickEventHandler2 = clickEventHandler;
                    OurResourceItem.ClickEventHandler clickEventHandler3 = (OurResourceItem.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                    clickEventHandler = Interlocked.CompareExchange<OurResourceItem.ClickEventHandler>(ref this.ClickEvent, clickEventHandler3, clickEventHandler2);
                }
                while (clickEventHandler != clickEventHandler2);
            }
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseDown)
            {
                OurResourceItem.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                if (!e.Handled)
                {
                    Logger.Info(string.Format("按下资源工程列表项：{0}", this.LabTitle.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\OurResourceItem.xaml.vb");
                }
            }
        }

        private void OurResourceItem_Click(OurResourceItem sender, EventArgs e)
        {
            List<string> list = new List<string>();
            if (ModMain.FrmMain.PageCurrent.Page == FormMain.PageType.ResourceDetail)
            {
                try
                {
                    foreach (object obj in ModMain.FrmDownloadResourceDetail.PanResults.Children)
                    {
                        OurCard ourCard = (OurCard)obj;
                        if (Operators.CompareString(ourCard.Title, "", false) != 0 && !ourCard.IsSwapped)
                        {
                            list.Add(ourCard.Title);
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
                Logger.Info(string.Format("记录当前已展开的卡片：{0}", string.Join("、", list)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\OurResourceItem.xaml.vb");
            }
            string text = null;
            ModLoaders modLoaders = ModLoaders.None;
            ResourceTypes resourceTypes = default;
            if (ModMain.FrmMain.PageCurrent.Page == FormMain.PageType.Download)
            {
                switch (ModMain.FrmMain.PageCurrentSub)
                {
                case FormMain.PageSubType.DownloadMod:
                    resourceTypes = ResourceTypes.Mod;
                    text = ModMain.FrmDownloadMod.Content.Loader.Input.GameVersion;
                    modLoaders = ModMain.FrmDownloadMod.Content.Loader.Input.ModLoaders;
                    break;
                case FormMain.PageSubType.DownloadPack:
                    resourceTypes = ResourceTypes.ModPack;
                    text = ModMain.FrmDownloadPack.Content.Loader.Input.GameVersion;
                    break;
                case FormMain.PageSubType.DownloadDataPack:
                    resourceTypes = ResourceTypes.DataPack;
                    text = ModMain.FrmDownloadDataPack.Content.Loader.Input.GameVersion;
                    break;
                case FormMain.PageSubType.DownloadResourcePack:
                    resourceTypes = ResourceTypes.ResourcePack;
                    text = ModMain.FrmDownloadResourcePack.Content.Loader.Input.GameVersion;
                    break;
                case FormMain.PageSubType.DownloadShader:
                    resourceTypes = ResourceTypes.Shader;
                    text = ModMain.FrmDownloadShader.Content.Loader.Input.GameVersion;
                    break;
                }
            }
            else
            {
                resourceTypes = ResourceTypes.Any;
                text = Conversions.ToString(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, new object[] { 2 }, null));
                modLoaders = (ModLoaders)Conversions.ToInteger(NewLateBinding.LateIndexGet(ModMain.FrmMain.PageCurrent.Additional, new object[] { 3 }, null));
            }
            ModMain.FrmMain.PageChange(new FormMain.PageStackData
            {
                Page = FormMain.PageType.ResourceDetail,
                Additional = new object[] { sender.Tag, list, text, modLoaders, resourceTypes }
            }, FormMain.PageSubType.Default);
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (base.IsMouseOver && this.CanInteraction)
            {
                this.IsMouseDown = true;
            }
        }

        private void Button_MouseLeave(object sender, object e)
        {
            this.IsMouseDown = false;
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

        public bool CanInteraction
        {
            [CompilerGenerated]
            get
            {
                return this._CanInteraction;
            }
            [CompilerGenerated]
            set
            {
                this._CanInteraction = value;
            }
        }

        public void RefreshColor(object sender, EventArgs e)
        {
            if (this.CanInteraction)
            {
                string text;
                int num;
                if (base.IsMouseOver)
                {
                    if (this.IsMouseDown)
                    {
                        text = "MouseDown";
                        num = 120;
                    }
                    else
                    {
                        text = "MouseOver";
                        num = 120;
                    }
                }
                else
                {
                    text = "Idle";
                    num = 180;
                }
                if (Operators.CompareString(this.StateLast, text, false) != 0)
                {
                    this.StateLast = text;
                    if (base.IsLoaded && ModAnimation.AniControlEnabled == 0)
                    {
                        List<ModAnimation.AniData> list = new List<ModAnimation.AniData>();
                        if (base.IsMouseOver)
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
                                ModAnimation.AaColor(this.RectBack, Border.BackgroundProperty, this.IsMouseDown ? "ColorBrush6" : "ColorBrush7", num, 0, null, false),
                                ModAnimation.AaScaleTransform(this.RectBack, 0.996 - ((ScaleTransform)this.RectBack.RenderTransform).ScaleX, num, 0, new ModAnimation.AniEaseOutFluent(ModAnimation.AniEasePower.Middle), false),
                                ModAnimation.AaScaleTransform(this.RectBack, -0.196, 1, 0, null, true)
                            });
                        }
                        ModAnimation.AniStart(list, "OurResourceItem Color " + Conversions.ToString(this.Uuid), false);
                        return;
                    }
                    ModAnimation.AniStop("OurResourceItem Color " + Conversions.ToString(this.Uuid));
                    if (this._RectBack != null)
                    {
                        this.RectBack.Opacity = 0.0;
                    }
                }
            }
        }






        internal virtual TextBlock LabInfo_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._LabInfo;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                MouseEventHandler mouseEventHandler = new MouseEventHandler(this.LabInfo_MouseEnter);
                TextBlock textBlock = this._LabInfo;
                if (textBlock != null)
                {
                    textBlock.MouseEnter -= mouseEventHandler;
                }
                this._LabInfo = value;
                textBlock = this._LabInfo;
                if (textBlock != null)
                {
                    textBlock.MouseEnter += mouseEventHandler;
                }
            }
        }
















        [CompilerGenerated]
        private void _Lambda___R0_1(object a0, MouseButtonEventArgs a1)
        {
            this.OurResourceItem_Click((OurResourceItem)a0, a1);
        }

        public int Uuid;

        [CompilerGenerated]
        private OurResourceItem.ClickEventHandler ClickEvent;

        private bool IsMouseDown;

        private Border _RectBack;

        private string StateLast;

        [CompilerGenerated]
        private bool _CanInteraction;

        [CompilerGenerated]
        [AccessedThroughProperty("PanBack")]
        private OurResourceItem _PanBack;

        [CompilerGenerated]
        [AccessedThroughProperty("PathLogo")]
        private OurImage _PathLogo;

        [CompilerGenerated]
        [AccessedThroughProperty("LabTitle")]
        private TextBlock _LabTitle;

        [CompilerGenerated]
        [AccessedThroughProperty("LabTitleRaw")]
        private TextBlock _LabTitleRaw;

        [CompilerGenerated]
        [AccessedThroughProperty("PanTags")]
        private StackPanel _PanTags;

        [CompilerGenerated]
        [AccessedThroughProperty("LabInfo")]
        private TextBlock _LabInfo;

        [CompilerGenerated]
        [AccessedThroughProperty("ToolTipInfo")]
        private ToolTip _ToolTipInfo;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnVersion1")]
        private ColumnDefinition _ColumnVersion1;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnVersion2")]
        private ColumnDefinition _ColumnVersion2;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnVersion3")]
        private ColumnDefinition _ColumnVersion3;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnTime1")]
        private ColumnDefinition _ColumnTime1;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnTime2")]
        private ColumnDefinition _ColumnTime2;

        [CompilerGenerated]
        [AccessedThroughProperty("ColumnTime3")]
        private ColumnDefinition _ColumnTime3;

        [CompilerGenerated]
        [AccessedThroughProperty("PathVersion")]
        private Path _PathVersion;

        [CompilerGenerated]
        [AccessedThroughProperty("LabVersion")]
        private TextBlock _LabVersion;

        [CompilerGenerated]
        [AccessedThroughProperty("PathDownload")]
        private Path _PathDownload;

        [CompilerGenerated]
        [AccessedThroughProperty("LabDownload")]
        private TextBlock _LabDownload;

        [CompilerGenerated]
        [AccessedThroughProperty("PathTime")]
        private Path _PathTime;

        [CompilerGenerated]
        [AccessedThroughProperty("LabTime")]
        private TextBlock _LabTime;

        [CompilerGenerated]
        [AccessedThroughProperty("PathSource")]
        private Path _PathSource;

        [CompilerGenerated]
        [AccessedThroughProperty("LabSource")]
        private TextBlock _LabSource;

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);
    }
}

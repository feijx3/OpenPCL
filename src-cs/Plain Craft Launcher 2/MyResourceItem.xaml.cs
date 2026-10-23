using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class MyResourceItem : Grid
    {
        public MyResourceItem()
        {
            base.PreviewMouseLeftButtonUp += this.Button_MouseUp;
            this.Click += delegate(object a0, MouseButtonEventArgs a1)
            {
                this.MyResourceItem_Click((MyResourceItem)a0, a1);
            };
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
                            Background = new ModBase.MyColor("#11000000"),
                            Padding = new Thickness(3.0, 1.0, 3.0, 1.0),
                            CornerRadius = new CornerRadius(3.0),
                            Margin = new Thickness(0.0, 0.0, 3.0, 0.0),
                            SnapsToDevicePixels = true,
                            UseLayoutRounding = false
                        };
                        TextBlock textBlock = new TextBlock
                        {
                            Text = text,
                            Foreground = new ModBase.MyColor("#868686"),
                            FontSize = 11.0
                        };
                        border.Child = textBlock;
                        this.PanTags.Children.Add(border);
                    }
                }
                finally
                {
                    List<string>.Enumerator enumerator;
                    ((IDisposable)enumerator).Dispose();
                }
            }
        }

        public event MyResourceItem.ClickEventHandler Click;

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseDown)
            {
                MyResourceItem.ClickEventHandler clickEvent = this.ClickEvent;
                if (clickEvent != null)
                {
                    clickEvent(RuntimeHelpers.GetObjectValue(sender), e);
                }
                if (!e.Handled)
                {
                    Logger.Info(string.Format("按下资源工程列表项：{0}", this.LabTitle.Text), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\MyResourceItem.xaml.vb");
                }
            }
        }

        private void MyResourceItem_Click(MyResourceItem sender, EventArgs e)
        {
            List<string> list = new List<string>();
            if (ModMain.FrmMain.PageCurrent.Page == FormMain.PageType.ResourceDetail)
            {
                try
                {
                    foreach (object obj in ModMain.FrmDownloadResourceDetail.PanResults.Children)
                    {
                        MyCard myCard = (MyCard)obj;
                        if (Operators.CompareString(myCard.Title, "", false) != 0 && !myCard.IsSwapped)
                        {
                            list.Add(myCard.Title);
                        }
                    }
                }
                finally
                {
                    IEnumerator enumerator;
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                Logger.Info(string.Format("记录当前已展开的卡片：{0}", string.Join("、", list)), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\Resource\\MyResourceItem.xaml.vb");
            }
            string text = null;
            ModLoaders modLoaders = ModLoaders.None;
            ResourceTypes resourceTypes;
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

        public bool CanInteraction { get; set; }

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
                        ModAnimation.AniStart(list, "MyResourceItem Color " + Conversions.ToString(this.Uuid), false);
                        return;
                    }
                    ModAnimation.AniStop("MyResourceItem Color " + Conversions.ToString(this.Uuid));
                    if (this._RectBack != null)
                    {
                        this.RectBack.Opacity = 0.0;
                    }
                }
            }
        }

        internal virtual MyResourceItem PanBack
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyImage PathLogo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabTitle
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabTitleRaw
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual StackPanel PanTags
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabInfo
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

        internal virtual ToolTip ToolTipInfo
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual ColumnDefinition ColumnVersion1
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual ColumnDefinition ColumnVersion2
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual ColumnDefinition ColumnVersion3
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual ColumnDefinition ColumnTime1
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual ColumnDefinition ColumnTime2
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual ColumnDefinition ColumnTime3
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path PathVersion
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabVersion
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path PathDownload
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabDownload
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path PathTime
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabTime
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual Path PathSource
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual TextBlock LabSource
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public int Uuid;

        private bool IsMouseDown;

        private Border _RectBack;

        private string StateLast;

        public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);
    }
}

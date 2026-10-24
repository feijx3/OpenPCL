using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageSetupLeft : OurPageLeft
    {
        private void PageSetupLeft_Loaded(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            if (this.ItemLaunch.Checked && Settings.Get<bool>("UiHiddenSetupLaunch", null))
            {
                flag = true;
            }
            if (this.ItemUI.Checked && Settings.Get<bool>("UiHiddenSetupUi", null))
            {
                flag = true;
            }
            if (this.ItemSystem.Checked && Settings.Get<bool>("UiHiddenSetupSystem", null))
            {
                flag = true;
            }
            if (this.ItemLink.Checked && Settings.Get<bool>("UiHiddenSetupLink", null))
            {
                flag = true;
            }
            if (PageSetupUI.HiddenForceShow)
            {
                flag = false;
            }
            if (!this.IsLoad || flag)
            {
                this.IsLoad = true;
                PageSetupUI.HiddenRefresh();
                if (!this.IsPageSwitched)
                {
                    if (!Settings.Get<bool>("UiHiddenSetupLaunch", null))
                    {
                        this.ItemLaunch.SetChecked(true, false, false);
                        return;
                    }
                    if (!Settings.Get<bool>("UiHiddenSetupUi", null))
                    {
                        this.ItemUI.SetChecked(true, false, false);
                        return;
                    }
                    if (!Settings.Get<bool>("UiHiddenSetupSystem", null))
                    {
                        this.ItemSystem.SetChecked(true, false, false);
                        return;
                    }
                    if (!Settings.Get<bool>("UiHiddenSetupLink", null))
                    {
                        this.ItemLink.SetChecked(true, false, false);
                        return;
                    }
                    this.ItemLaunch.SetChecked(true, false, false);
                }
            }
        }

        private void PageOtherLeft_Unloaded(object sender, RoutedEventArgs e)
        {
            this.IsPageSwitched = false;
        }

        public PageSetupLeft()
        {
            base.Loaded += this.PageSetupLeft_Loaded;
            base.Unloaded += this.PageOtherLeft_Unloaded;
            this.IsLoad = false;
            this.IsPageSwitched = false;
            this.InitializeComponent();
            this.ItemLaunch_WithEvents = this.ItemLaunch;
            this.ItemLink_WithEvents = this.ItemLink;
            this.ItemUI_WithEvents = this.ItemUI;
            this.ItemSystem_WithEvents = this.ItemSystem;
            if (!Settings.Get<bool>("UiHiddenSetupLaunch", null))
            {
                this.PageID = FormMain.PageSubType.Default;
                return;
            }
            if (!Settings.Get<bool>("UiHiddenSetupUi", null))
            {
                this.PageID = FormMain.PageSubType.DownloadMod;
                return;
            }
            if (!Settings.Get<bool>("UiHiddenSetupSystem", null))
            {
                this.PageID = FormMain.PageSubType.DownloadPack;
                return;
            }
            if (!Settings.Get<bool>("UiHiddenSetupLink", null))
            {
                this.PageID = FormMain.PageSubType.SetupLink;
                return;
            }
            this.PageID = FormMain.PageSubType.Default;
        }

        private void PageCheck(OurListItem sender, EventArgs e)
        {
            if (sender.Tag != null)
            {
                this.PageChange(checked((FormMain.PageSubType)Math.Round(ModBase.Val(RuntimeHelpers.GetObjectValue(sender.Tag)))));
            }
        }

        public object PageGet(FormMain.PageSubType ID = (FormMain.PageSubType)(-1))
        {
            if (ID == (FormMain.PageSubType)(-1))
            {
                ID = this.PageID;
            }
            object obj;
            switch (ID)
            {
            case FormMain.PageSubType.Default:
                if (ModMain.FrmSetupLaunch == null)
                {
                    ModMain.FrmSetupLaunch = new PageSetupLaunch();
                }
                obj = ModMain.FrmSetupLaunch;
                break;
            case FormMain.PageSubType.SetupLink:
                if (ModMain.FrmSetupLink == null)
                {
                    ModMain.FrmSetupLink = new PageSetupLink();
                }
                obj = ModMain.FrmSetupLink;
                break;
            case FormMain.PageSubType.DownloadMod:
                if (ModMain.FrmSetupUI == null)
                {
                    ModMain.FrmSetupUI = new PageSetupUI();
                }
                obj = ModMain.FrmSetupUI;
                break;
            case FormMain.PageSubType.DownloadPack:
                if (ModMain.FrmSetupSystem == null)
                {
                    ModMain.FrmSetupSystem = new PageSetupSystem();
                }
                obj = ModMain.FrmSetupSystem;
                break;
            default:
                throw new Exception("未知的设置子页面种类：" + Conversions.ToString((int)ID));
            }
            return obj;
        }

        public void PageChange(FormMain.PageSubType ID)
        {
            checked
            {
                if (this.PageID != ID)
                {
                    ModAnimation.AniControlEnabled++;
                    this.IsPageSwitched = true;
                    try
                    {
                        switch (ID)
                        {
                        case FormMain.PageSubType.Default:
                            if (Information.IsNothing(ModMain.FrmSetupLaunch))
                            {
                                ModMain.FrmSetupLaunch = new PageSetupLaunch();
                            }
                            PageSetupLeft.PageChangeRun(ModMain.FrmSetupLaunch);
                            break;
                        case FormMain.PageSubType.SetupLink:
                            if (Information.IsNothing(ModMain.FrmSetupLink))
                            {
                                ModMain.FrmSetupLink = new PageSetupLink();
                            }
                            PageSetupLeft.PageChangeRun(ModMain.FrmSetupLink);
                            break;
                        case FormMain.PageSubType.DownloadMod:
                            if (Information.IsNothing(ModMain.FrmSetupUI))
                            {
                                ModMain.FrmSetupUI = new PageSetupUI();
                            }
                            PageSetupLeft.PageChangeRun(ModMain.FrmSetupUI);
                            break;
                        case FormMain.PageSubType.DownloadPack:
                            if (Information.IsNothing(ModMain.FrmSetupSystem))
                            {
                                ModMain.FrmSetupSystem = new PageSetupSystem();
                            }
                            PageSetupLeft.PageChangeRun(ModMain.FrmSetupSystem);
                            break;
                        default:
                            throw new Exception("未知的设置子页面种类：" + Conversions.ToString((int)ID));
                        }
                        this.PageID = ID;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, string.Format("切换分页面失败（ID {0}）", ID), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageSetup\\PageSetupLeft.xaml.vb");
                    }
                    finally
                    {
                        ModAnimation.AniControlEnabled--;
                    }
                }
            }
        }

        private static void PageChangeRun(OurPageRight Target)
        {
            ModAnimation.AniStop("FrmMain PageChangeRight");
            if (Target.Parent != null)
            {
                Target.SetValue(ContentPresenter.ContentProperty, null);
            }
            ModMain.FrmMain.PageRight = Target;
            ((OurPageRight)ModMain.FrmMain.PanMainRight.Child).PageOnExit();
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaCode((PageSetupLeft._Closure___._I9_0 == null) ? (PageSetupLeft._Closure___._I9_0 = new ThreadStart(PageSetupLeft._Closure___._I._Lambda___9_0)) : PageSetupLeft._Closure___._I9_0, 130, false),
                ModAnimation.AaCode((PageSetupLeft._Closure___._I9_1 == null) ? (PageSetupLeft._Closure___._I9_1 = new ThreadStart(PageSetupLeft._Closure___._I._Lambda___9_1)) : PageSetupLeft._Closure___._I9_1, 30, true)
            }, "PageLeft PageChange", false);
        }

        public void Reset(object sender, EventArgs e)
        {
            double num = ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null)));
            if (num == 0.0)
            {
                if (ModMain.OurMsgBox("是否要初始化 启动 页面的所有设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", true, true, false, null, null, null) == 1)
                {
                    if (Information.IsNothing(ModMain.FrmSetupLaunch))
                    {
                        ModMain.FrmSetupLaunch = new PageSetupLaunch();
                    }
                    ModMain.FrmSetupLaunch.Reset();
                    this.ItemLaunch.Checked = true;
                    return;
                }
            }
            else if (num == 2.0)
            {
                if (ModMain.OurMsgBox("是否要初始化 个性化 页面的所有设置？该操作不可撤销。\r\n（背景图片与音乐、主页等外部文件不会被删除）", "初始化确认", "确定", "取消", "", true, true, false, null, null, null) == 1)
                {
                    if (Information.IsNothing(ModMain.FrmSetupUI))
                    {
                        ModMain.FrmSetupUI = new PageSetupUI();
                    }
                    ModMain.FrmSetupUI.Reset();
                    this.ItemUI.Checked = true;
                    return;
                }
            }
            else if (num == 3.0)
            {
                if (ModMain.OurMsgBox("是否要初始化 其他 页面的所有设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", true, true, false, null, null, null) == 1)
                {
                    if (Information.IsNothing(ModMain.FrmSetupSystem))
                    {
                        ModMain.FrmSetupSystem = new PageSetupSystem();
                    }
                    ModMain.FrmSetupSystem.Reset();
                    this.ItemSystem.Checked = true;
                    return;
                }
            }
            else if (num == 1.0 && ModMain.OurMsgBox("是否要初始化 联机 页面的所有设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", true, true, false, null, null, null) == 1)
            {
                if (Information.IsNothing(ModMain.FrmSetupLink))
                {
                    ModMain.FrmSetupLink = new PageSetupLink();
                }
                ModMain.FrmSetupLink.Reset();
                this.ItemLink.Checked = true;
            }
        }


        internal virtual OurListItem ItemLaunch_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemLaunch;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R17_1);
                OurListItem ourListItem = this._ItemLaunch;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemLaunch = value;
                ourListItem = this._ItemLaunch;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurListItem ItemLink_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemLink;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R21_2);
                OurListItem ourListItem = this._ItemLink;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemLink = value;
                ourListItem = this._ItemLink;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurListItem ItemUI_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemUI;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R25_3);
                OurListItem ourListItem = this._ItemUI;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemUI = value;
                ourListItem = this._ItemUI;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurListItem ItemSystem_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemSystem;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R29_4);
                OurListItem ourListItem = this._ItemSystem;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemSystem = value;
                ourListItem = this._ItemSystem;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R17_1(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((OurListItem)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R21_2(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((OurListItem)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R25_3(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((OurListItem)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R29_4(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((OurListItem)a0, a1);
        }

        private bool IsLoad;

        private bool IsPageSwitched;

        public FormMain.PageSubType PageID;

        [CompilerGenerated]
        [AccessedThroughProperty("PanItem")]
        private StackPanel _PanItem;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemLaunch")]
        private OurListItem _ItemLaunch;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemLink")]
        private OurListItem _ItemLink;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemUI")]
        private OurListItem _ItemUI;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemSystem")]
        private OurListItem _ItemSystem;

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___9_0()
            {
                ((OurPageRight)ModMain.FrmMain.PanMainRight.Child).PageOnForceExit();
                ModMain.FrmMain.PanMainRight.Child = ModMain.FrmMain.PageRight;
                ModMain.FrmMain.PageRight.Opacity = 0.0;
            }

            internal void _Lambda___9_1()
            {
                ModMain.FrmMain.PageRight.Opacity = 1.0;
                ModMain.FrmMain.PageRight.PageOnEnter();
            }

            public static readonly PageSetupLeft._Closure___ _I = new PageSetupLeft._Closure___();

            public static ThreadStart _I9_0;

            public static ThreadStart _I9_1;
        }
    }
}

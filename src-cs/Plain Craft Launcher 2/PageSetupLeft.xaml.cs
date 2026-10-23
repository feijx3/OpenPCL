using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MeloongCore;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageSetupLeft : MyPageLeft
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

        private void PageCheck(MyListItem sender, EventArgs e)
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

        private static void PageChangeRun(MyPageRight Target)
        {
            ModAnimation.AniStop("FrmMain PageChangeRight");
            if (Target.Parent != null)
            {
                Target.SetValue(ContentPresenter.ContentProperty, null);
            }
            ModMain.FrmMain.PageRight = Target;
            ((MyPageRight)ModMain.FrmMain.PanMainRight.Child).PageOnExit();
            ModAnimation.AniStart(new ModAnimation.AniData[]
            {
                ModAnimation.AaCode((PageSetupLeft._Closure$__.$I9-0 == null) ? (PageSetupLeft._Closure$__.$I9-0 = delegate
                {
                    ((MyPageRight)ModMain.FrmMain.PanMainRight.Child).PageOnForceExit();
                    ModMain.FrmMain.PanMainRight.Child = ModMain.FrmMain.PageRight;
                    ModMain.FrmMain.PageRight.Opacity = 0.0;
                }) : PageSetupLeft._Closure$__.$I9-0, 130, false),
                ModAnimation.AaCode((PageSetupLeft._Closure$__.$I9-1 == null) ? (PageSetupLeft._Closure$__.$I9-1 = delegate
                {
                    ModMain.FrmMain.PageRight.Opacity = 1.0;
                    ModMain.FrmMain.PageRight.PageOnEnter();
                }) : PageSetupLeft._Closure$__.$I9-1, 30, true)
            }, "PageLeft PageChange", false);
        }

        public void Reset(object sender, EventArgs e)
        {
            double num = ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null)));
            if (num == 0.0)
            {
                if (ModMain.MyMsgBox("是否要初始化 启动 页面的所有设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", true, true, false, null, null, null) == 1)
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
                if (ModMain.MyMsgBox("是否要初始化 个性化 页面的所有设置？该操作不可撤销。\r\n（背景图片与音乐、主页等外部文件不会被删除）", "初始化确认", "确定", "取消", "", true, true, false, null, null, null) == 1)
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
                if (ModMain.MyMsgBox("是否要初始化 其他 页面的所有设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", true, true, false, null, null, null) == 1)
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
            else if (num == 1.0 && ModMain.MyMsgBox("是否要初始化 联机 页面的所有设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", true, true, false, null, null, null) == 1)
            {
                if (Information.IsNothing(ModMain.FrmSetupLink))
                {
                    ModMain.FrmSetupLink = new PageSetupLink();
                }
                ModMain.FrmSetupLink.Reset();
                this.ItemLink.Checked = true;
            }
        }

        internal virtual StackPanel PanItem
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyListItem ItemLaunch
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((MyListItem)a0, a1);
                };
                MyListItem myListItem = this._ItemLaunch;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemLaunch = value;
                myListItem = this._ItemLaunch;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemLink
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((MyListItem)a0, a1);
                };
                MyListItem myListItem = this._ItemLink;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemLink = value;
                myListItem = this._ItemLink;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemUI
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((MyListItem)a0, a1);
                };
                MyListItem myListItem = this._ItemUI;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemUI = value;
                myListItem = this._ItemUI;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemSystem
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((MyListItem)a0, a1);
                };
                MyListItem myListItem = this._ItemSystem;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemSystem = value;
                myListItem = this._ItemSystem;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        private bool IsLoad;

        private bool IsPageSwitched;

        public FormMain.PageSubType PageID;
    }
}

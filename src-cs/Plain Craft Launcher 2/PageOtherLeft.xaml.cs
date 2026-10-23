using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageOtherLeft : MyPageLeft
    {
        private void PageOtherLeft_Loaded(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            if (this.ItemHelp.Checked && Settings.Get<bool>("UiHiddenOtherHelp", null))
            {
                flag = true;
            }
            if (this.ItemAbout.Checked && Settings.Get<bool>("UiHiddenOtherAbout", null))
            {
                flag = true;
            }
            if (this.ItemTest.Checked && Settings.Get<bool>("UiHiddenOtherTest", null))
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
                    if (!Settings.Get<bool>("UiHiddenOtherHelp", null))
                    {
                        this.ItemHelp.SetChecked(true, false, false);
                        return;
                    }
                    if (!Settings.Get<bool>("UiHiddenOtherAbout", null))
                    {
                        this.ItemAbout.SetChecked(true, false, false);
                        return;
                    }
                    this.ItemTest.SetChecked(true, false, false);
                }
            }
        }

        private void PageOtherLeft_Unloaded(object sender, RoutedEventArgs e)
        {
            this.IsPageSwitched = false;
        }

        public PageOtherLeft()
        {
            base.Loaded += this.PageOtherLeft_Loaded;
            base.Unloaded += this.PageOtherLeft_Unloaded;
            this.IsLoad = false;
            this.IsPageSwitched = false;
            this.InitializeComponent();
            if (!Settings.Get<bool>("UiHiddenOtherHelp", null))
            {
                this.PageID = FormMain.PageSubType.Default;
                return;
            }
            if (!Settings.Get<bool>("UiHiddenOtherAbout", null))
            {
                this.PageID = FormMain.PageSubType.SetupLink;
                return;
            }
            this.PageID = FormMain.PageSubType.DownloadMod;
        }

        private void PageCheck(FrameworkElement sender, ModBase.RouteEventArgs e)
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
                if (ModMain.FrmOtherHelp == null)
                {
                    ModMain.FrmOtherHelp = new PageOtherHelp();
                }
                obj = ModMain.FrmOtherHelp;
                break;
            case FormMain.PageSubType.SetupLink:
                if (ModMain.FrmOtherAbout == null)
                {
                    ModMain.FrmOtherAbout = new PageOtherAbout();
                }
                obj = ModMain.FrmOtherAbout;
                break;
            case FormMain.PageSubType.DownloadMod:
                if (ModMain.FrmOtherTest == null)
                {
                    ModMain.FrmOtherTest = new PageOtherTest();
                }
                obj = ModMain.FrmOtherTest;
                break;
            default:
                throw new Exception("未知的更多子页面种类：" + Conversions.ToString((int)ID));
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
                        PageOtherLeft.PageChangeRun((MyPageRight)this.PageGet(ID));
                        this.PageID = ID;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, string.Format("切换分页面失败（ID {0}）", ID), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageOther\\PageOtherLeft.xaml.vb");
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
                ModAnimation.AaCode((PageOtherLeft._Closure$__.$I9-0 == null) ? (PageOtherLeft._Closure$__.$I9-0 = delegate
                {
                    ((MyPageRight)ModMain.FrmMain.PanMainRight.Child).PageOnForceExit();
                    ModMain.FrmMain.PanMainRight.Child = ModMain.FrmMain.PageRight;
                    ModMain.FrmMain.PageRight.Opacity = 0.0;
                }) : PageOtherLeft._Closure$__.$I9-0, 130, false),
                ModAnimation.AaCode((PageOtherLeft._Closure$__.$I9-1 == null) ? (PageOtherLeft._Closure$__.$I9-1 = delegate
                {
                    ModMain.FrmMain.PageRight.Opacity = 1.0;
                    ModMain.FrmMain.PageRight.PageOnEnter();
                }) : PageOtherLeft._Closure$__.$I9-1, 30, true)
            }, "PageLeft PageChange", false);
        }

        public void Refresh(object sender, EventArgs e)
        {
            if (ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null))) == 0.0)
            {
                PageOtherLeft.RefreshHelp();
                this.ItemHelp.Checked = true;
            }
            ModMain.Hint("正在刷新……", ModMain.HintType.Blue, false);
        }

        public static void RefreshHelp()
        {
            Settings.Set("SystemHelpVersion", 0, null);
            ModMain.FrmOtherHelp.PageLoaderRestart(null, true);
            ModMain.FrmOtherHelp.SearchBox.Text = "";
        }

        private void TryFeedback(object sender, ModBase.RouteEventArgs e)
        {
            if (this.ItemFeedback.Checked)
            {
                PageOtherLeft.TryFeedback();
                e.Handled = true;
            }
        }

        public static void TryFeedback()
        {
            if (false.Equals(PageSetupSystem.IsLauncherNewest()))
            {
                if (ModMain.MyMsgBox(string.Format("你的 PCL 不是最新版，因此无法提交反馈。{0}请在更新后，确认该问题在最新版中依然存在，然后再提交反馈。", "\r\n"), "无法提交反馈", "更新", "取消", "", false, true, false, null, null, null) == 1)
                {
                    ModSecret.UpdateCheckByButton();
                    return;
                }
            }
            else
            {
                int num = ModMain.MyMsgBox("在提交新反馈前，建议先搜索反馈列表，以避免重复提交。\r\n如果无法打开该网页，请使用 VPN 改善网络环境。", "反馈", "提交新反馈", "查看反馈列表", "取消", false, true, false, null, null, null);
                if (num == 1)
                {
                    ModMain.Feedback(true, false);
                    return;
                }
                if (num != 2)
                {
                    return;
                }
                ModBase.OpenWebsite("https://github.com/Meloong-Git/PCL/issues/");
            }
        }

        private void TryVote(object sender, ModBase.RouteEventArgs e)
        {
            if (this.ItemVote.Checked)
            {
                PageOtherLeft.TryVote();
                e.Handled = true;
            }
        }

        public static void TryVote()
        {
            if (ModMain.MyMsgBox("是否要打开新功能投票网页？\r\n如果无法打开该网页，请使用 VPN 改善网络环境。", "新功能投票", "打开", "取消", "", false, true, false, null, null, null) != 2)
            {
                ModBase.OpenWebsite("https://github.com/Meloong-Git/PCL/discussions/categories/%E5%8A%9F%E8%83%BD%E6%8A%95%E7%A5%A8?discussions_q=category%3A%E5%8A%9F%E8%83%BD%E6%8A%95%E7%A5%A8+sort%3Adate_created");
            }
        }

        internal virtual StackPanel PanItem
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyListItem ItemHelp
        {
            [CompilerGenerated]
            get
            {
                return this._ItemHelp;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemHelp;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemHelp = value;
                myListItem = this._ItemHelp;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemAbout
        {
            [CompilerGenerated]
            get
            {
                return this._ItemAbout;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemAbout;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemAbout = value;
                myListItem = this._ItemAbout;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemTest
        {
            [CompilerGenerated]
            get
            {
                return this._ItemTest;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemTest;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemTest = value;
                myListItem = this._ItemTest;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemFeedback
        {
            [CompilerGenerated]
            get
            {
                return this._ItemFeedback;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.ChangedEventHandler changedEventHandler = new IMyRadio.ChangedEventHandler(this.TryFeedback);
                MyListItem myListItem = this._ItemFeedback;
                if (myListItem != null)
                {
                    myListItem.Changed -= changedEventHandler;
                }
                this._ItemFeedback = value;
                myListItem = this._ItemFeedback;
                if (myListItem != null)
                {
                    myListItem.Changed += changedEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemVote
        {
            [CompilerGenerated]
            get
            {
                return this._ItemVote;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IMyRadio.ChangedEventHandler changedEventHandler = new IMyRadio.ChangedEventHandler(this.TryVote);
                MyListItem myListItem = this._ItemVote;
                if (myListItem != null)
                {
                    myListItem.Changed -= changedEventHandler;
                }
                this._ItemVote = value;
                myListItem = this._ItemVote;
                if (myListItem != null)
                {
                    myListItem.Changed += changedEventHandler;
                }
            }
        }

        private bool IsLoad;

        private bool IsPageSwitched;

        public FormMain.PageSubType PageID;
    }
}

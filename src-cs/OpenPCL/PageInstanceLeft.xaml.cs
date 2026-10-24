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
using PCLCS;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageInstanceLeft : OurPageLeft, IRefreshable
    {
        public PageInstanceLeft()
        {
            base.Loaded += this.PageInstanceLeft_Loaded;
            base.Loaded += this._Lambda___R1_1;
            base.Loaded += ((PageInstanceLeft._Closure___._IR1_2 == null) ? (PageInstanceLeft._Closure___._IR1_2 = new RoutedEventHandler(PageInstanceLeft._Closure___._I._Lambda___R1_2)) : PageInstanceLeft._Closure___._IR1_2);
            this.PageID = FormMain.PageSubType.Default;
            this.InitializeComponent();
            this.ItemOverall_WithEvents = this.ItemOverall;
            this.ItemSetup_WithEvents = this.ItemSetup;
            this.ItemModify_WithEvents = this.ItemModify;
            this.ItemMod_WithEvents = this.ItemMod;
            this.ItemModDisabled_WithEvents = this.ItemModDisabled;
            this.ItemExport_WithEvents = this.ItemExport;
        }

        private void PageInstanceLeft_Loaded(object sender, RoutedEventArgs e)
        {
            PageSetupUI.HiddenRefresh();
        }

        public void RefreshModDisabled()
        {
            if (((object)(PageInstanceLeft.Instance) != (object)null) && PageInstanceLeft.Instance.Modable)
            {
                this.ItemMod.Visibility = Visibility.Visible;
                this.ItemModDisabled.Visibility = Visibility.Collapsed;
                return;
            }
            this.ItemMod.Visibility = Visibility.Collapsed;
            this.ItemModDisabled.Visibility = Visibility.Visible;
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
                if (ModMain.FrmInstanceOverall == null)
                {
                    ModMain.FrmInstanceOverall = new PageInstanceOverall();
                }
                obj = ModMain.FrmInstanceOverall;
                break;
            case FormMain.PageSubType.SetupLink:
                if (ModMain.FrmInstanceSetup == null)
                {
                    ModMain.FrmInstanceSetup = new PageInstanceSetup();
                }
                obj = ModMain.FrmInstanceSetup;
                break;
            case FormMain.PageSubType.DownloadMod:
                if (ModMain.FrmDownloadModify == null)
                {
                    ModMain.FrmDownloadModify = new PageDownloadModify();
                }
                obj = ModMain.FrmDownloadModify;
                break;
            case FormMain.PageSubType.DownloadPack:
                if (ModMain.FrmInstanceMod == null)
                {
                    ModMain.FrmInstanceMod = new PageInstanceMod();
                }
                obj = ModMain.FrmInstanceMod;
                break;
            case FormMain.PageSubType.DownloadDataPack:
                if (ModMain.FrmInstanceModDisabled == null)
                {
                    ModMain.FrmInstanceModDisabled = new PageInstanceModDisabled();
                }
                obj = ModMain.FrmInstanceModDisabled;
                break;
            case FormMain.PageSubType.DownloadResourcePack:
                if (ModMain.FrmInstanceExport == null)
                {
                    ModMain.FrmInstanceExport = new PageInstanceExport();
                }
                obj = ModMain.FrmInstanceExport;
                break;
            default:
                throw new Exception("未知的版本设置子页面种类：" + Conversions.ToString((int)ID));
            }
            return obj;
        }

        public void PageChange(FormMain.PageSubType ID)
        {
            checked
            {
                if (this.PageID != ID)
                {
                    if (((object)(PageInstanceLeft.Instance) != (object)null) && ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance))
                    {
                        ModMain.Hint("该版本正在被修改，请稍后再切换页面！", ModMain.HintType.Red, true);
                        return;
                    }
                    ModAnimation.AniControlEnabled++;
                    try
                    {
                        PageInstanceLeft.PageChangeRun((OurPageRight)this.PageGet(ID));
                        this.PageID = ID;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, string.Format("切换分页面失败（ID {0}）", ID), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageInstance\\PageInstanceLeft.xaml.vb");
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
                ModAnimation.AaCode((PageInstanceLeft._Closure___._I9_0 == null) ? (PageInstanceLeft._Closure___._I9_0 = new ThreadStart(PageInstanceLeft._Closure___._I._Lambda___9_0)) : PageInstanceLeft._Closure___._I9_0, 130, false),
                ModAnimation.AaCode((PageInstanceLeft._Closure___._I9_1 == null) ? (PageInstanceLeft._Closure___._I9_1 = new ThreadStart(PageInstanceLeft._Closure___._I._Lambda___9_1)) : PageInstanceLeft._Closure___._I9_1, 30, true)
            }, "PageLeft PageChange", false);
        }

        public void Refresh_Click(object sender, EventArgs e)
        {
            this.Refresh(checked((FormMain.PageSubType)Math.Round(ModBase.Val(RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(sender, null, "Tag", new object[0], null, null, null))))));
        }

        public void Refresh()
        {
            this.Refresh(ModMain.FrmMain.PageCurrentSub);
        }

        public void Refresh(FormMain.PageSubType SubType)
        {
            if (((object)(PageInstanceLeft.Instance) != (object)null) && ModMinecraft.McInstanceLock.IsLocked(PageInstanceLeft.Instance))
            {
                ModMain.Hint("该版本正在被修改，请稍后再刷新！", ModMain.HintType.Red, true);
                return;
            }
            switch (SubType)
            {
            case FormMain.PageSubType.DownloadMod:
            {
                PageDownloadModify frmDownloadModify = ModMain.FrmDownloadModify;
                if (frmDownloadModify != null)
                {
                    frmDownloadModify.PageLoaderRestart(null, true);
                }
                this.ItemModify.Checked = true;
                break;
            }
            case FormMain.PageSubType.DownloadPack:
                PageInstanceMod.Refresh();
                this.ItemMod.Checked = true;
                return;
            case FormMain.PageSubType.DownloadDataPack:
                break;
            case FormMain.PageSubType.DownloadResourcePack:
            {
                PageInstanceExport frmInstanceExport = ModMain.FrmInstanceExport;
                if (frmInstanceExport != null)
                {
                    frmInstanceExport.RefreshAll();
                }
                this.ItemExport.Checked = true;
                return;
            }
            default:
                return;
            }
        }

        public void Reset(object sender, EventArgs e)
        {
            if (ModMain.OurMsgBox("是否要初始化该版本的版本独立设置？该操作不可撤销。", "初始化确认", "确定", "取消", "", true, true, false, null, null, null) == 1)
            {
                if (Information.IsNothing(ModMain.FrmInstanceSetup))
                {
                    ModMain.FrmInstanceSetup = new PageInstanceSetup();
                }
                ModMain.FrmInstanceSetup.Reset();
                this.ItemSetup.Checked = true;
            }
        }

        public static void ReloadCurrentJava()
        {
            if (ModAnimation.AniControlEnabled == 0 && ((object)(PageInstanceLeft.Instance) != (object)null))
            {
                PageInstanceLeft.CurrentJavaWorker.Start(default(CancellationToken), null);
            }
        }


        internal virtual OurListItem ItemOverall_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemOverall;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R22_3);
                OurListItem ourListItem = this._ItemOverall;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemOverall = value;
                ourListItem = this._ItemOverall;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurListItem ItemSetup_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemSetup;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R26_4);
                OurListItem ourListItem = this._ItemSetup;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemSetup = value;
                ourListItem = this._ItemSetup;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurListItem ItemModify_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemModify;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R30_5);
                OurListItem ourListItem = this._ItemModify;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemModify = value;
                ourListItem = this._ItemModify;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurListItem ItemMod_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemMod;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R34_6);
                OurListItem ourListItem = this._ItemMod;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemMod = value;
                ourListItem = this._ItemMod;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurListItem ItemModDisabled_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemModDisabled;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R38_7);
                OurListItem ourListItem = this._ItemModDisabled;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemModDisabled = value;
                ourListItem = this._ItemModDisabled;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurListItem ItemExport_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemExport;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R42_8);
                OurListItem ourListItem = this._ItemExport;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemExport = value;
                ourListItem = this._ItemExport;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R1_1(object a0, RoutedEventArgs a1)
        {
            this.RefreshModDisabled();
        }

        [CompilerGenerated]
        private void _Lambda___R22_3(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R26_4(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R30_5(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R34_6(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R38_7(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R42_8(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        public static ModMinecraft.McInstance Instance = null;

        public FormMain.PageSubType PageID;

        public static RedoableWorker<Java> CurrentJavaWorker = new RedoableWorker<Java>(new Func<CancellationToken?, ProgressProvider, Java>(PageInstanceLeft._Closure___._I._Lambda___0_0), "CurrentJavaWorker");

        [CompilerGenerated]
        [AccessedThroughProperty("PanItem")]
        private StackPanel _PanItem;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemOverall")]
        private OurListItem _ItemOverall;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemSetup")]
        private OurListItem _ItemSetup;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemModify")]
        private OurListItem _ItemModify;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemMod")]
        private OurListItem _ItemMod;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemModDisabled")]
        private OurListItem _ItemModDisabled;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemExport")]
        private OurListItem _ItemExport;

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal Java _Lambda___0_0(CancellationToken? c, ProgressProvider p)
            {
                return ModJava.SelectOrDownloadJava(PageInstanceLeft.Instance, false, c.Value, p);
            }

            internal void _Lambda___R1_2(object a0, RoutedEventArgs a1)
            {
                PageInstanceLeft.ReloadCurrentJava();
            }

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

            public static readonly PageInstanceLeft._Closure___ _I = new PageInstanceLeft._Closure___();

            public static RoutedEventHandler _IR1_2;

            public static ThreadStart _I9_0;

            public static ThreadStart _I9_1;
        }
    }
}

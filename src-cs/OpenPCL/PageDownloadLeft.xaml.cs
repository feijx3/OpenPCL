using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageDownloadLeft : OurPageLeft, IRefreshable
    {
        public PageDownloadLeft()
        {
            this.PageID = FormMain.PageSubType.Default;
            this.InitializeComponent();
            this.ItemInstall_WithEvents = this.ItemInstall;
            this.ItemMod_WithEvents = this.ItemMod;
            this.ItemPack_WithEvents = this.ItemPack;
            this.ItemDataPack_WithEvents = this.ItemDataPack;
            this.ItemResourcePack_WithEvents = this.ItemResourcePack;
            this.ItemShader_WithEvents = this.ItemShader;
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
            switch (ID)
            {
            case FormMain.PageSubType.Default:
                if (ModMain.FrmDownloadInstall == null)
                {
                    ModMain.FrmDownloadInstall = new PageDownloadInstall();
                }
                return ModMain.FrmDownloadInstall;
            case FormMain.PageSubType.DownloadMod:
                if (ModMain.FrmDownloadMod == null)
                {
                    ModMain.FrmDownloadMod = new PageDownloadMod();
                }
                return ModMain.FrmDownloadMod;
            case FormMain.PageSubType.DownloadPack:
                if (ModMain.FrmDownloadPack == null)
                {
                    ModMain.FrmDownloadPack = new PageDownloadPack();
                }
                return ModMain.FrmDownloadPack;
            case FormMain.PageSubType.DownloadDataPack:
                if (ModMain.FrmDownloadDataPack == null)
                {
                    ModMain.FrmDownloadDataPack = new PageDownloadDataPack();
                }
                return ModMain.FrmDownloadDataPack;
            case FormMain.PageSubType.DownloadResourcePack:
                if (ModMain.FrmDownloadResourcePack == null)
                {
                    ModMain.FrmDownloadResourcePack = new PageDownloadResourcePack();
                }
                return ModMain.FrmDownloadResourcePack;
            case FormMain.PageSubType.DownloadShader:
                if (ModMain.FrmDownloadShader == null)
                {
                    ModMain.FrmDownloadShader = new PageDownloadShader();
                }
                return ModMain.FrmDownloadShader;
            }
            throw new Exception("未知的下载子页面种类：" + Conversions.ToString((int)ID));
        }

        public void PageChange(FormMain.PageSubType ID)
        {
            checked
            {
                if (this.PageID != ID)
                {
                    ModAnimation.AniControlEnabled++;
                    try
                    {
                        PageDownloadLeft.PageChangeRun((OurPageRight)this.PageGet(ID));
                        this.PageID = ID;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, string.Format("切换分页面失败（ID {0}）", ID), LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Pages\\PageDownload\\PageDownloadLeft.xaml.vb");
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
                ModAnimation.AaCode((PageDownloadLeft._Closure___._I5_0 == null) ? (PageDownloadLeft._Closure___._I5_0 = new ThreadStart(PageDownloadLeft._Closure___._I._Lambda___5_0)) : PageDownloadLeft._Closure___._I5_0, 130, false),
                ModAnimation.AaCode((PageDownloadLeft._Closure___._I5_1 == null) ? (PageDownloadLeft._Closure___._I5_1 = new ThreadStart(PageDownloadLeft._Closure___._I._Lambda___5_1)) : PageDownloadLeft._Closure___._I5_1, 30, true)
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
            ResourceProject.Cache.Clear();
            ResourceVersion.ProjectFilesCache.Clear();
            switch (SubType)
            {
            case FormMain.PageSubType.Default:
                ModDownload.DlClientListLoader.Start(null, true);
                ModDownload.DlOptiFineListLoader.Start(null, true);
                ModDownload.DlForgeListLoader.Start(null, true);
                ModDownload.DlNeoForgeListLoader.Start(null, true);
                ModDownload.DlLiteLoaderListLoader.Start(null, true);
                ModDownload.DlFabricListLoader.Start(null, true);
                ModDownload.DlFabricApiLoader.Start(null, true);
                ModDownload.DlOptiFabricLoader.Start(null, true);
                this.ItemInstall.Checked = true;
                break;
            case FormMain.PageSubType.DownloadMod:
                if (ModMain.FrmDownloadMod != null)
                {
                    ModMain.FrmDownloadMod.Content.Storage = new ResourceSearcher.SearchResult();
                    ModMain.FrmDownloadMod.Content.Page = 0;
                    ModMain.FrmDownloadMod.PageLoaderRestart(null, true);
                }
                this.ItemMod.Checked = true;
                break;
            case FormMain.PageSubType.DownloadPack:
                if (ModMain.FrmDownloadPack != null)
                {
                    ModMain.FrmDownloadPack.Content.Storage = new ResourceSearcher.SearchResult();
                    ModMain.FrmDownloadPack.Content.Page = 0;
                    ModMain.FrmDownloadPack.PageLoaderRestart(null, true);
                }
                this.ItemPack.Checked = true;
                break;
            case FormMain.PageSubType.DownloadDataPack:
                if (ModMain.FrmDownloadDataPack != null)
                {
                    ModMain.FrmDownloadDataPack.Content.Storage = new ResourceSearcher.SearchResult();
                    ModMain.FrmDownloadDataPack.Content.Page = 0;
                    ModMain.FrmDownloadDataPack.PageLoaderRestart(null, true);
                }
                this.ItemDataPack.Checked = true;
                break;
            case FormMain.PageSubType.DownloadResourcePack:
                if (ModMain.FrmDownloadResourcePack != null)
                {
                    ModMain.FrmDownloadResourcePack.Content.Storage = new ResourceSearcher.SearchResult();
                    ModMain.FrmDownloadResourcePack.Content.Page = 0;
                    ModMain.FrmDownloadResourcePack.PageLoaderRestart(null, true);
                }
                this.ItemResourcePack.Checked = true;
                break;
            case FormMain.PageSubType.DownloadShader:
                if (ModMain.FrmDownloadShader != null)
                {
                    ModMain.FrmDownloadShader.Content.Storage = new ResourceSearcher.SearchResult();
                    ModMain.FrmDownloadShader.Content.Page = 0;
                    ModMain.FrmDownloadShader.PageLoaderRestart(null, true);
                }
                this.ItemShader.Checked = true;
                break;
            }
            ModMain.Hint("正在刷新……", ModMain.HintType.Blue, false);
        }

        private void ItemInstall_Click(object sender, MouseButtonEventArgs e)
        {
            if (this.ItemInstall.Checked)
            {
                ModMain.FrmDownloadInstall.ExitSelectPage();
            }
        }


        internal virtual OurListItem ItemInstall_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemInstall;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R16_1);
                OurListItem.ClickEventHandler clickEventHandler = new OurListItem.ClickEventHandler(this.ItemInstall_Click);
                OurListItem ourListItem = this._ItemInstall;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                    ourListItem.Click -= clickEventHandler;
                }
                this._ItemInstall = value;
                ourListItem = this._ItemInstall;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                    ourListItem.Click += clickEventHandler;
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
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R20_2);
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

        internal virtual OurListItem ItemPack_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemPack;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R24_3);
                OurListItem ourListItem = this._ItemPack;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemPack = value;
                ourListItem = this._ItemPack;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurListItem ItemDataPack_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemDataPack;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R28_4);
                OurListItem ourListItem = this._ItemDataPack;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemDataPack = value;
                ourListItem = this._ItemDataPack;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurListItem ItemResourcePack_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemResourcePack;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R32_5);
                OurListItem ourListItem = this._ItemResourcePack;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemResourcePack = value;
                ourListItem = this._ItemResourcePack;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual OurListItem ItemShader_WithEvents
        {
            [CompilerGenerated]
            get
            {
                return this._ItemShader;
            }
            [CompilerGenerated]
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                IOurRadio.CheckEventHandler checkEventHandler = new IOurRadio.CheckEventHandler(this._Lambda___R36_6);
                OurListItem ourListItem = this._ItemShader;
                if (ourListItem != null)
                {
                    ourListItem.Check -= checkEventHandler;
                }
                this._ItemShader = value;
                ourListItem = this._ItemShader;
                if (ourListItem != null)
                {
                    ourListItem.Check += checkEventHandler;
                }
            }
        }

        [CompilerGenerated]
        private void _Lambda___R16_1(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R20_2(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R24_3(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R28_4(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R32_5(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        [CompilerGenerated]
        private void _Lambda___R36_6(object a0, ModBase.RouteEventArgs a1)
        {
            this.PageCheck((FrameworkElement)a0, a1);
        }

        public FormMain.PageSubType PageID;

        [CompilerGenerated]
        [AccessedThroughProperty("PanItem")]
        private StackPanel _PanItem;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemInstall")]
        private OurListItem _ItemInstall;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemMod")]
        private OurListItem _ItemMod;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemPack")]
        private OurListItem _ItemPack;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemDataPack")]
        private OurListItem _ItemDataPack;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemResourcePack")]
        private OurListItem _ItemResourcePack;

        [CompilerGenerated]
        [AccessedThroughProperty("ItemShader")]
        private OurListItem _ItemShader;

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            internal void _Lambda___5_0()
            {
                ((OurPageRight)ModMain.FrmMain.PanMainRight.Child).PageOnForceExit();
                ModMain.FrmMain.PanMainRight.Child = ModMain.FrmMain.PageRight;
                ModMain.FrmMain.PageRight.Opacity = 0.0;
            }

            internal void _Lambda___5_1()
            {
                ModMain.FrmMain.PageRight.Opacity = 1.0;
                ModMain.FrmMain.PageRight.PageOnEnter();
            }

            public static readonly PageDownloadLeft._Closure___ _I = new PageDownloadLeft._Closure___();

            public static ThreadStart _I5_0;

            public static ThreadStart _I5_1;
        }
    }
}

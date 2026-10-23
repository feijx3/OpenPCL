using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MeloongCore;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [DesignerGenerated]
    public partial class PageDownloadLeft : MyPageLeft, IRefreshable
    {
        public PageDownloadLeft()
        {
            this.PageID = FormMain.PageSubType.Default;
            this.InitializeComponent();
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
                        PageDownloadLeft.PageChangeRun((MyPageRight)this.PageGet(ID));
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
                ModAnimation.AaCode((PageDownloadLeft._Closure$__.$I5-0 == null) ? (PageDownloadLeft._Closure$__.$I5-0 = delegate
                {
                    ((MyPageRight)ModMain.FrmMain.PanMainRight.Child).PageOnForceExit();
                    ModMain.FrmMain.PanMainRight.Child = ModMain.FrmMain.PageRight;
                    ModMain.FrmMain.PageRight.Opacity = 0.0;
                }) : PageDownloadLeft._Closure$__.$I5-0, 130, false),
                ModAnimation.AaCode((PageDownloadLeft._Closure$__.$I5-1 == null) ? (PageDownloadLeft._Closure$__.$I5-1 = delegate
                {
                    ModMain.FrmMain.PageRight.Opacity = 1.0;
                    ModMain.FrmMain.PageRight.PageOnEnter();
                }) : PageDownloadLeft._Closure$__.$I5-1, 30, true)
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

        internal virtual StackPanel PanItem
        {
            get; [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        internal virtual MyListItem ItemInstall
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem.ClickEventHandler clickEventHandler = new MyListItem.ClickEventHandler(this.ItemInstall_Click);
                MyListItem myListItem = this._ItemInstall;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                    myListItem.Click -= clickEventHandler;
                }
                this._ItemInstall = value;
                myListItem = this._ItemInstall;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                    myListItem.Click += clickEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemMod
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemMod;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemMod = value;
                myListItem = this._ItemMod;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemPack
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemPack;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemPack = value;
                myListItem = this._ItemPack;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemDataPack
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemDataPack;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemDataPack = value;
                myListItem = this._ItemDataPack;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemResourcePack
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemResourcePack;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemResourcePack = value;
                myListItem = this._ItemResourcePack;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        internal virtual MyListItem ItemShader
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
                IMyRadio.CheckEventHandler checkEventHandler = delegate(object a0, ModBase.RouteEventArgs a1)
                {
                    this.PageCheck((FrameworkElement)a0, a1);
                };
                MyListItem myListItem = this._ItemShader;
                if (myListItem != null)
                {
                    myListItem.Check -= checkEventHandler;
                }
                this._ItemShader = value;
                myListItem = this._ItemShader;
                if (myListItem != null)
                {
                    myListItem.Check += checkEventHandler;
                }
            }
        }

        public FormMain.PageSubType PageID;
    }
}
